using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FlightLoggerLib;
using FlightLoggerLib.Concrete;
using Nito.AsyncEx;
using SimConnect;
using SimConnect.Attribute;
using SimConnect.Lib;

namespace FSFlightLogger
{
    public class SimConnectLogger : IAsyncDisposable
    {
        private readonly ISimConnectClient client;
        private SimConnectLoggerWorker worker;

        public class Config
        {
            public string CSVOutputPath { get; set; }
            public string KMLOutputPath { get; set; }
            public int? KMLLivePort { get; set; }

            public TimeSpan IntervalTime { get; set; }
            public int IntervalDistance { get; set; }

            public bool WaitForMovement { get; set; }
            public TimeSpan? NewLogWhenIdleSeconds { get; set;  }
        }


        public SimConnectLogger(ISimConnectClient client)
        {
            this.client = client;
        }


        public async ValueTask DisposeAsync()
        {
            // Do not dispose of client, it is managed by the caller
            await Stop();
        }


        public async Task Start(Config config)
        {
            await Stop();
            worker = new SimConnectLoggerWorker(client, config);
        }


        public async Task Stop()
        {
            if (worker != null)
            {
                await worker.DisposeAsync();
                worker = null;
            }
        }



        private class SimConnectLoggerWorker : IAsyncDisposable
        {
            private readonly Config config;
            private readonly List<IFlightLogger> loggers = new List<IFlightLogger>();
            private IAsyncDisposable definition;
            private IAsyncDisposable simEvent;
            private IAsyncDisposable pauseEvent;

            private DateTime lastTime;
            private PositionData lastData = PositionData.Empty();
            private bool waitingForMovement;
            private DateTime lastStopped;

            private readonly AsyncLock simStateLock = new AsyncLock();
            private volatile bool paused = true;
            private volatile bool running = false;


            public SimConnectLoggerWorker(ISimConnectClient client, Config config)
            {
                this.config = config;

                if (!string.IsNullOrEmpty(config.CSVOutputPath))
                {
                    Directory.CreateDirectory(config.CSVOutputPath);
                    loggers.Add(new CSVFlightLogger(config.CSVOutputPath));
                }

                if (!string.IsNullOrEmpty(config.KMLOutputPath))
                {
                    Directory.CreateDirectory(config.KMLOutputPath);
                    loggers.Add(new KMLFlightLogger(config.KMLOutputPath, TimeSpan.FromSeconds(5)));
                }

                if (config.KMLLivePort.HasValue)
                    loggers.Add(new KMLLiveFlightLogger(config.KMLLivePort.Value));


                waitingForMovement = config.WaitForMovement;
                lastStopped = DateTime.Now;

                
                pauseEvent = client.SubscribeToSystemEvent(SimConnectSystemEvent.Pause, HandlePauseEvent);
                simEvent = client.SubscribeToSystemEvent(SimConnectSystemEvent.Sim, HandleSimEvent);
                definition = client.AddDefinition<PositionData>(HandlePositionData);
            }


            public async ValueTask DisposeAsync()
            {
                if (definition != null)
                {
                    await definition.DisposeAsync();
                    definition = null;
                }

                if (pauseEvent != null)
                {
                    await pauseEvent.DisposeAsync();
                    pauseEvent = null;
                }

                if (simEvent != null)
                {
                    await simEvent.DisposeAsync();
                    simEvent = null;
                }

                foreach (var logger in loggers)
                    await logger.DisposeAsync();
            }


            private async Task HandlePauseEvent(SimConnectSystemEventArgs args)
            {
                using (await simStateLock.LockAsync())
                {
                    paused = ((SimConnectPauseSystemEventArgs)args).Paused;
                }
            }


            private async Task HandleSimEvent(SimConnectSystemEventArgs args)
            {
                using (await simStateLock.LockAsync())
                {
                    running = ((SimConnectSimSystemEventArgs)args).SimRunning;
                }
            }


            private async Task HandlePositionData(PositionData data)
            {
                if (paused || !running)
                    return;


                var moving = data.Airspeed > 0;
                if (!moving && waitingForMovement)
                    return;

                waitingForMovement = false;


                // TODO take vertical position into account when going straight up or down (not a common use case, but still)
                var distanceMeters = LatLon.DistanceBetweenInMeters(lastData.Latitude, lastData.Longitude, data.Latitude, data.Longitude);
                if (distanceMeters < config.IntervalDistance)
                {
                    if (data.Airspeed < 0.1)
                        data.Airspeed = 0;

                    // Make an exception if we were last moving and have now stopped, so the 0 velocity record is logged as well
                    if (data.Airspeed > 0 || lastData.Airspeed == 0)
                        return;
                }


                var now = DateTime.Now;

                if (config.NewLogWhenIdleSeconds.HasValue)
                {
                    if (moving)
                    {
                        if (now - lastStopped >= config.NewLogWhenIdleSeconds)
                        {
                            await Task.WhenAll(loggers.Select(logger =>
                                logger.NewLog()));
                        }
                    }

                    lastStopped = now;
                }


                var time = now - lastTime;
                if (time < config.IntervalTime)
                    return;

                lastTime = now;
                lastData = data;

                // ReSharper disable once AccessToDisposedClosure - covered by disposing the client first
                await Task.WhenAll(loggers.Select(logger =>
                    logger.LogPosition(now, new FlightPosition
                    {
                        Latitude = data.Latitude,
                        Longitude = data.Longitude,
                        Altitude = data.Altitude,
                        Airspeed = data.Airspeed
                    })));
            }
        }

        // ReSharper disable once ClassNeverInstantiated.Local - it is, by the SimConnect client
        public class PositionData
        {
            [SimConnectVariable("PLANE LATITUDE", "degrees")]
            public float Latitude;

            [SimConnectVariable("PLANE LONGITUDE", "degrees")]
            public float Longitude;

            [SimConnectVariable("PLANE ALTITUDE", "feet")]
            public float Altitude;

            [SimConnectVariable("AIRSPEED INDICATED", "knots")]
            public float Airspeed;


            public static PositionData Empty()
            {
                return new PositionData
                {
                    Latitude = 0,
                    Longitude = 0,
                    Altitude = 0,
                    Airspeed = 0
                };
            }
        }
    }
}
