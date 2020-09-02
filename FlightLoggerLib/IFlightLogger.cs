using System;
using System.Threading.Tasks;

namespace FlightLoggerLib
{
    public class FlightPosition
    {
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public float Altitude { get; set; }
        public float Airspeed { get; set; }
    }



    public interface IFlightLogger : IDisposable
    {
        Task LogPosition(DateTime eventTime, FlightPosition position);
        //void LogEvent
    }
}
