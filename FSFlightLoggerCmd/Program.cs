using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using CommandLine;
using FlightLoggerLib;
using FlightLoggerLib.Concrete;
using SimConnect;
using SimConnect.Attribute;
using SimConnect.Concrete;
using SimConnect.Lib;

namespace FSFlightLoggerCmd
{
    // TODO verb for converting CSV to KML


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
    }


    public class Program
    {
        private enum OutputFormat
        {
            None,
            CSV,
            KML
        };


        // ReSharper disable once ClassNeverInstantiated.Local - used by CommandLineParser
        // ReSharper disable UnusedAutoPropertyAccessor.Local - used by CommandLineParser
        private class Options
        { 
            [Option('o', "outputPath", Required = false, HelpText = "Specifies the output path for the log files. Defaults to a 'Flight logs' folder on the desktop.")]
            public string OutputPath { get; set; }

            [Option('i', "interval", Required = false, Default = 1, HelpText = "The minimum time, in seconds, between log entries.")]
            public int IntervalTime { get; set; }

            [Option('d', "distance", Required = false, Default = 1, HelpText = "The minimum distance, in meters, between log entries.")]
            public int IntervalDistance { get; set; }

            [Option('v', "verbose", Required = false, HelpText = "Enable verbose logging.")]
            public bool Verbose { get; set; }

            [Option('f', "format", Required = false, Default = OutputFormat.CSV, HelpText = "The output format to use. Possible values: CSV, KML.")]
            public OutputFormat OutputFormat { get; set; }

            [Option('s', "format2", Required = false, Default = OutputFormat.None, HelpText = "The secondary output format to use. Possible values: None, CSV, KML.")]
            public OutputFormat OutputFormat2 { get; set; }
        }
        // ReSharper restore UnusedAutoPropertyAccessor.Local


        public static void Main(string[] args)
        {
            var parser = new Parser(settings =>
            {
                settings.CaseInsensitiveEnumValues = true;
            });

            parser.ParseArguments<Options>(args)
                .WithParsed(Run);
        }

        private static void Run(Options o)
        {
            var intervalTime = TimeSpan.FromSeconds(o.IntervalTime);
            void VerboseLog(string message)
            {
                if (o.Verbose)
                    Console.WriteLine(message);
            }


            var factory = new SimConnectClientFactory();
            var client = TryConnect(factory).Result;

            var outputPath = o.OutputPath;
            if (string.IsNullOrEmpty(outputPath))
                outputPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "Flight logs");


            Directory.CreateDirectory(outputPath);
            var loggers = new List<IFlightLogger>();
            AddLogger(loggers, o.OutputFormat, outputPath);

            if (o.OutputFormat2 != o.OutputFormat)
                AddLogger(loggers, o.OutputFormat2, outputPath);


            var lastTime = DateTime.MinValue;
            var lastData = new PositionData
            {
                Latitude = 0,
                Longitude = 0,
                Altitude = 0,
                Airspeed = 0
            };


            client.AddDefinition<PositionData>(data =>
            {
                // TODO take vertical position into account when going straight up or down (not a common use case, but still)
                var distanceMeters = LatLon.DistanceBetweenInMeters(lastData.Latitude, lastData.Longitude, data.Latitude, data.Longitude);
                if (distanceMeters < o.IntervalDistance)
                {
                    if (data.Airspeed < 0.1)
                        data.Airspeed = 0;

                    // Make an exception if we were last moving and have now stopped, so the 0 velocity record is logged as well
                    if (data.Airspeed > 0 || lastData.Airspeed == 0)
                        return;
                }

                var now = DateTime.Now;
                var time = now - lastTime;
                if (time < intervalTime)
                    return;

                VerboseLog("Logging position, elapsed time: " + time.TotalSeconds + ", distance: " + distanceMeters);

                lastTime = now;
                lastData = data;

                // ReSharper disable once AccessToDisposedClosure - covered by disposing the client first
                loggers.ForEach(logger =>
                    logger.LogPosition(now, new FlightPosition
                    {
                        Latitude = data.Latitude,
                        Longitude = data.Longitude,
                        Altitude = data.Altitude,
                        Airspeed = data.Airspeed
                    }));
            });


            var stopEvent = new ManualResetEventSlim(false);
            Console.CancelKeyPress += (sender, args) =>
            {
                stopEvent.Set();
                args.Cancel = true;
            };

            Console.WriteLine("Flight log active, press Ctrl-C to stop");
            Console.WriteLine("Output path: " + outputPath);
            stopEvent.Wait(Timeout.Infinite);

            Console.WriteLine("Closing...");
            client.Dispose();
            loggers.ForEach(logger => logger.Dispose());

            if (!Debugger.IsAttached) 
                return;

            Console.WriteLine("Press any Enter key to continue");
            Console.ReadLine();
        }


        private static void AddLogger(ICollection<IFlightLogger> loggers, OutputFormat outputFormat, string outputPath)
        {
            switch (outputFormat)
            {
                case OutputFormat.CSV:
                    loggers.Add(new CSVFlightLogger(outputPath));
                    break;

                case OutputFormat.KML:
                    loggers.Add(new KMLFlightLogger(outputPath, TimeSpan.FromSeconds(5)));
                    break;

                case OutputFormat.None:
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }


        private static async Task<ISimConnectClient> TryConnect(ISimConnectClientFactory factory)
        {
            while (true)
            {
                Console.WriteLine("Attempting to connect to SimConnect...");

                var client = await factory.TryConnect("FS Flight Logger");
                if (client != null)
                {
                    Console.WriteLine("Success!");
                    return client;
                }

                Console.WriteLine("Failed to connect, retrying in 5 seconds");
                await Task.Delay(TimeSpan.FromSeconds(5));
            }
        }
    }
}
