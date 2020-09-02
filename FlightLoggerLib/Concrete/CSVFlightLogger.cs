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
        private readonly CsvWriter output;


        public CSVFlightLogger(string path)
        {
            var filename = Path.Combine(path, DateTime.Now.ToString("yyyy-MM-dd HH.mm.ss") + ".csv");
            var header = !File.Exists(filename);

            output = new CsvWriter(new StreamWriter(filename, true), new CsvConfiguration(CultureInfo.CurrentCulture)
            {
                SanitizeForInjection = false,
                HasHeaderRecord = header
            });
        }


        public void Dispose()
        {
            output?.Dispose();
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
