using System;
using System.Threading.Tasks;

namespace SimConnect
{
    public enum SimConnectSystemEvent
    {
        /// <summary>
        /// Request notifications when the flight is paused or unpaused. args will be of type SimConnectPauseSystemEventArgs.
        /// </summary>
        Pause,

        /// <summary>
        /// Request notifications when the flight is running or not. args will be of type SimConnectSimSystemEventArgs.
        /// </summary>
        Sim
    }


    public abstract class SimConnectSystemEventArgs
    {
    }

    public class SimConnectPauseSystemEventArgs : SimConnectSystemEventArgs
    {
        public bool Paused { get; set; }
    }

    public class SimConnectSimSystemEventArgs : SimConnectSystemEventArgs
    {
        public bool SimRunning { get; set; }
    }



    public delegate Task SimConnectSystemEventAction(SimConnectSystemEventArgs args);


    /// <summary>
    /// Called when new data arrives from the SimConnect server.
    /// </summary>
    /// <param name="data">An instance of the data class as passed to AddDefinition containing the variable values</param>
    /// <typeparam name="T">The data class as passed to AddDefinition</typeparam>
    public delegate Task SimConnectDataHandlerAction<in T>(T data) where T : class;





    /// <summary>
    /// Gets notified of changes to the SimConnect state.
    /// </summary>
    public interface ISimConnectClientObserver
    {
        /// <summary>
        /// Gets called when the SimConnect connection is lost. The client will not receive further notifications,
        /// a new connection should be attempted if desired.
        /// </summary>
        void OnQuit();
    }


    /// <summary>
    /// Provides access to the SimConnect library.
    /// </summary>
    public interface ISimConnectClient : IAsyncDisposable
    {
        /// <summary>
        /// Attaches the specified observer to receive status notifications.
        /// </summary>
        /// <param name="observer">The observer to receive status notifications</param>
        void AttachObserver(ISimConnectClientObserver observer);


        /// <summary>
        /// Registers a definition to receive updates from the SimConnect server.
        /// </summary>
        /// <param name="onData">A callback method which is called whenever a data update is received</param>
        /// <typeparam name="T">A class defining the variables to monitor annotated using the SimConnectVariable attribute</typeparam>
        /// <returns>An IAsyncDisposable which can be disposed to unregister the definition. Dispose is not required to be called when the client is disconnected, but will not throw an exception.</returns>
        IAsyncDisposable AddDefinition<T>(SimConnectDataHandlerAction<T> onData) where T : class;


        /// <summary>
        /// Subscribes to a SimConnect system event.
        /// </summary>
        /// <param name="systemEvent">The type of event to subscribe to.</param>
        /// <param name="onEvent">The callback method called whenever the system event occurs.</param>
        /// <returns>An IAsyncDisposable which can be disposed to unregister the definition. Dispose is not required to be called when the client is disconnected, but will not throw an exception.</returns>
        IAsyncDisposable SubscribeToSystemEvent(SimConnectSystemEvent systemEvent, SimConnectSystemEventAction onEvent);
    }
}
