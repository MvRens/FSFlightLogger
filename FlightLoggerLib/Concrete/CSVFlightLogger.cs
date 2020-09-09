using CsvHelper;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;

namespace FlightLoggerLib.Concrete
{
    public class CSVFlightLogger : IFlightLogger
    {
        private readonly string path;
        private CsvWriter output;


        public CSVFlightLogger(string path)
        {
            this.path = path;
        }


        public async ValueTask DisposeAsync()
        {
            if (output != null)
                await output.DisposeAsync();
        }


        public async Task NewLog()
        {
            if (output != null)
            {
                await output.DisposeAsync();
                output = null;
            }
        }


        public async Task LogPosition(DateTime eventTime, FlightPosition position)
        {
            var record = new OutputRecord
            {
                Time = eventTime,
                Latitude = position.Latitude,
                Longitude = position.Longitude,
                Altitude = position.Altitude,
                Airspeed = position.Airspeed
            };

            if (output == null)
            {
                var filename = Path.Combine(path, eventTime.ToString("yyyy-MM-dd HH.mm.ss") + ".csv");
                var header = !File.Exists(filename);

                output = new CsvWriter(new StreamWriter(filename, true), new CsvConfiguration(CultureInfo.CurrentCulture)
                {
                    SanitizeForInjection = false,
                    HasHeaderRecord = header
                });
            }

            await output.WriteRecordsAsync(Enumerable.Repeat(record, 1));
            await output.FlushAsync();
        }



        protected class OutputRecord
        {
            [Index(0)]
            public DateTime Time { get; set; }

            [Index(1)]
            public float Latitude { get; set; }

            [Index(2)]
            public float Longitude { get; set; }

            [Index(3)]
            public float Altitude { get; set; }

            [Index(4)]
            public float Airspeed { get; set; }
        }
    }
}
