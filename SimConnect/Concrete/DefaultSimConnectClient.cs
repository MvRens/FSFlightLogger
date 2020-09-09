using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace SimConnect.Concrete
{
    /// <summary>
    /// Default implementation of ISimConnectClient. Compatible with Flight Simulator 2020, X Stream Edition, X and Prepar3D.
    /// Requires the following DLL files to be present: FSX2020-SimConnect.dll, FSX-SE-SimConnect.dll, FSX-SE-SimConnect.dll, FSX-SimConnect.dll.
    /// These are renamed versions of the SimConnect.dll from the various flight simulator installations.
    /// </summary>
    public class DefaultSimConnectClient : ISimConnectClient
    {
        private readonly ISimConnectLibrary simConnectLibrary;
        private SimConnectWorker worker;

        private uint nextDefinitionID = 1;
        private uint nextEventID = 1;

        private readonly object observersLock = new object();
        private readonly List<ISimConnectClientObserver> observers = new List<ISimConnectClientObserver>();


        /// <summary>
        /// Creates an instance of DefaultSimConnectClient.
        /// </summary>
        /// <param name="simConnectLibrary">The low-level SimConnect library interface to use</param>
        public DefaultSimConnectClient(ISimConnectLibrary simConnectLibrary)
        {
            this.simConnectLibrary = simConnectLibrary;
        }


        /// <inheritdoc />
        public async ValueTask DisposeAsync()
        {
            worker?.Close();

            if (simConnectLibrary != null)
                await simConnectLibrary.DisposeAsync();
        }

        /// <summary>
        /// Attempts to open a connection to the SimConnect server
        /// </summary>
        /// <param name="appName">The application name passed to the SimConnect server.</param>
        /// <returns></returns>
        public async Task<bool> TryOpen(string appName)
        {
            worker?.Close();
            worker = new SimConnectWorker(simConnectLibrary, appName);
            return await worker.Open();
        }


        /// <inheritdoc />
        public void AttachObserver(ISimConnectClientObserver observer)
        {
            Monitor.Enter(observersLock);
            try
            {
                observers.Add(observer);
            }
            finally
            {
                Monitor.Exit(observersLock);
            }
        }


        /// <inheritdoc />
        public IAsyncDisposable AddDefinition<T>(SimConnectDataHandlerAction<T> onData) where T : class
        {
            if (worker == null)
                throw new InvalidOperationException("TryOpen must be called first");

            var definition = new SimConnectDefinition(typeof(T));
            void HandleData(Stream stream)
            {
                var data = definition.ParseData(stream);
                onData((T)data);
            }

            var definitionRegistration = new SimConnectDefinitionRegistration<T>(nextDefinitionID, definition, HandleData, worker);
            nextDefinitionID++;

            return definitionRegistration;
        }


        /// <inheritdoc />
        public IAsyncDisposable SubscribeToSystemEvent(SimConnectSystemEvent systemEvent, SimConnectSystemEventAction onEvent)
        {
            if (worker == null)
                throw new InvalidOperationException("TryOpen must be called first");

            void HandleData(SimConnectRecvEvent recvEvent)
            {
                SimConnectSystemEventArgs args;

                switch (systemEvent)
                {
                    case SimConnectSystemEvent.Pause:
                        args = new SimConnectPauseSystemEventArgs { Paused = recvEvent.dwData == 1 };
                        break;

                    case SimConnectSystemEvent.Sim:
                        args = new SimConnectSimSystemEventArgs { SimRunning = recvEvent.dwData == 1 };
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(systemEvent), systemEvent, null);
                }

                onEvent(args);
            }

            var eventRegistration = new SimConnectSystemEventRegistration(nextEventID, systemEvent, HandleData, worker);
            nextEventID++;

            return eventRegistration;
        }


        private class SimConnectDefinitionRegistration<T> : IAsyncDisposable where T : class
        {
            private readonly uint definitionID;
            private readonly SimConnectWorker worker;


            public SimConnectDefinitionRegistration(uint definitionID, SimConnectDefinition definition, Action<Stream> onData, SimConnectWorker worker)
            {
                this.definitionID = definitionID;
                this.worker = worker;

                worker.RegisterDefinition(definitionID, definition, onData);
            }


            public ValueTask DisposeAsync()
            {
                worker.UnregisterDefinition(definitionID);
                return default;
            }
        }


        private class SimConnectSystemEventRegistration : IAsyncDisposable
        {
            private readonly uint eventID;
            private readonly SimConnectWorker worker;


            public SimConnectSystemEventRegistration(uint eventID, SimConnectSystemEvent systemEvent, Action<SimConnectRecvEvent> onData, SimConnectWorker worker)
            {
                this.eventID = eventID;
                this.worker = worker;

                worker.SubscribeToSystemEvent(eventID, systemEvent, onData);
            }


            public ValueTask DisposeAsync()
            {
                worker.UnsubscribeFromSystemEvent(eventID);
                return default;
            }
        }


        private class SimConnectWorker
        {
            private readonly ISimConnectLibrary simConnectLibrary;
            private readonly string appName;

            private Task workerTask;

            private readonly AutoResetEvent workerPulse = new AutoResetEvent(false);
            private readonly object workerLock = new object();
            private volatile bool closed;
            private readonly Queue<Action<IntPtr>> workQueue = new Queue<Action<IntPtr>>();

            private readonly TaskCompletionSource<bool> openResult = new TaskCompletionSource<bool>();
            private readonly ConcurrentDictionary<uint, Action<Stream>> definitionDataHandler = new ConcurrentDictionary<uint, Action<Stream>>();
            private readonly ConcurrentDictionary<uint, Action<SimConnectRecvEvent>> eventDataHandler = new ConcurrentDictionary<uint, Action<SimConnectRecvEvent>>();


            public SimConnectWorker(ISimConnectLibrary simConnectLibrary, string appName)
            {
                this.simConnectLibrary = simConnectLibrary;
                this.appName = appName;
            }


            public async Task<bool> Open()
            {
                if (workerTask == null)
                    workerTask = Task.Run(RunInBackground);

                return await openResult.Task;
            }


            public void Close()
            {
                closed = true;
                workerPulse.Set();
            }


            public void RegisterDefinition(uint definitionID, SimConnectDefinition definition, Action<Stream> onData)
            {
                Enqueue(hSimConnect =>
                {
                    foreach (var variable in definition.Variables)
                    {
                        simConnectLibrary.SimConnect_AddToDataDefinition(hSimConnect, definitionID, variable.VariableName, variable.UnitsName,
                            variable.DataType, variable.Epsilon);
                    }

                    definitionDataHandler.AddOrUpdate(definitionID, onData, (key, value) => onData);
                    simConnectLibrary.SimConnect_RequestDataOnSimObject(hSimConnect, definitionID, definitionID, 0, SimConnectPeriod.SimFrame, 1);
                });
            }


            public void UnregisterDefinition(uint definitionID)
            {
                Enqueue(hSimConnect =>
                {
                    definitionDataHandler.TryRemove(definitionID, out var unused);
                    simConnectLibrary.SimConnect_ClearDataDefinition(hSimConnect, definitionID);
                });
            }


            public void SubscribeToSystemEvent(uint eventID, SimConnectSystemEvent systemEvent, Action<SimConnectRecvEvent> onData)
            {
                Enqueue(hSimConnect =>
                {
                    eventDataHandler.AddOrUpdate(eventID, onData, (key, value) => onData);
                    var result = simConnectLibrary.SimConnect_SubscribeToSystemEvent(hSimConnect, eventID, systemEvent.ToString());

                    if (result == 0)
                        return;
                });
            }


            public void UnsubscribeFromSystemEvent(uint eventID)
            {
                Enqueue(hSimConnect =>
                {
                    eventDataHandler.TryRemove(eventID, out var unused);
                    simConnectLibrary.SimConnect_UnsubscribeFromSystemEvent(hSimConnect, eventID);
                });
            }


            private void Enqueue(Action<IntPtr> work)
            {
                lock(workerLock)
                {
                    workQueue.Enqueue(work);
                }

                workerPulse.Set();
            }


            private void RunInBackground()
            {
                var dataEvent = new EventWaitHandle(false, EventResetMode.AutoReset);

                var errorCode = simConnectLibrary.SimConnect_Open(out var simConnectHandle, appName, IntPtr.Zero, 0, dataEvent.SafeWaitHandle.DangerousGetHandle(), 0);
                openResult.TrySetResult(errorCode == 0);

                if (errorCode != 0)
                    return;


                while(!closed)
                {
                    switch (WaitHandle.WaitAny(new WaitHandle[] { workerPulse, dataEvent }))
                    {
                        case 0:
                            HandleWorkQueue(simConnectHandle);
                            break;

                        case 1:
                            HandleData(ref simConnectHandle);
                            break;
                    }
                }

                if (simConnectHandle != IntPtr.Zero)
                    simConnectLibrary.SimConnect_Close(simConnectHandle);
            }


            private void HandleWorkQueue(IntPtr simConnectHandle)
            {
                while (!closed)
                {
                    Action<IntPtr> work;

                    lock(workerLock)
                    {
                        work = workQueue.Count > 0 ? workQueue.Dequeue() : null;
                    }

                    if (work == null)
                        break;

                    work(simConnectHandle);
                }
            }


            private void HandleData(ref IntPtr simConnectHandle)
            {
                while (!closed && simConnectLibrary.SimConnect_GetNextDispatch(simConnectHandle, out var dataPtr, out var dataSize) == 0)
                {
                    var recv = Marshal.PtrToStructure<SimConnectRecv>(dataPtr);

                    // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
                    switch ((SimConnectRecvID) recv.dwID)
                    {
                        case SimConnectRecvID.Exception:
                            var recvException = Marshal.PtrToStructure<SimConnectRecvException>(dataPtr);
                            // TODO provide a way to get insight into exceptions
                            if (recvException.dwException == 0)
                                break;

                            break;

                        case SimConnectRecvID.Event:
                            var recvEvent = Marshal.PtrToStructure<SimConnectRecvEvent>(dataPtr);
                            if (!eventDataHandler.TryGetValue(recvEvent.uEventID, out var eventHandler))
                                break;

                            eventHandler(recvEvent);
                            break;

                        case SimConnectRecvID.SimobjectData:
                        case SimConnectRecvID.SimobjectDataByType:
                            var recvSimobjectData = Marshal.PtrToStructure<SimConnectRecvSimobjectData>(dataPtr);
                            if (!definitionDataHandler.TryGetValue(recvSimobjectData.dwDefineID, out var dataHandler))
                                break;

                            unsafe
                            {
                                var streamOffset = Marshal.OffsetOf<SimConnectRecvSimobjectData>("dwData").ToInt32();
                                var stream = new UnmanagedMemoryStream((byte*)IntPtr.Add(dataPtr, streamOffset).ToPointer(), dataSize - streamOffset);
                                dataHandler(stream);
                            }
                            break;

                        case SimConnectRecvID.Quit:
                            simConnectHandle = IntPtr.Zero;
                            closed = true;
                            break;
                    }
                }
            }
        }
    }
}
