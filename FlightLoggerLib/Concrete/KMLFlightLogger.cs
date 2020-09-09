using System;
using System.IO;
using System.Threading.Tasks;
using SharpKml.Base;
using SharpKml.Dom;

namespace FlightLoggerLib.Concrete
{
    public class KMLFlightLogger : IFlightLogger
    {
        private readonly string path;
        private string filename;
        private readonly System.TimeSpan flushInterval;
        private Document output;
        private Folder rootFolder;
        private LineString positionPath;
        
        private DateTime lastFlush = DateTime.MinValue;
        private Vector lastPosition;
        private float lastSpeed;
        private DateTime lastPointDate = DateTime.MinValue;


        public KMLFlightLogger(string path, System.TimeSpan flushInterval)
        {
            this.path = path;
            this.flushInterval = flushInterval;
        }


        public async Task NewLog()
        {
            if (output == null)
                return;

            CheckEndPoint();
            await Flush();
            output = null;
        }


        public async ValueTask DisposeAsync()
        {
            if (output == null)
                return;

            CheckEndPoint();
            await Flush();
        }


        protected void CheckEndPoint()
        {
            // TODO replace last "full stop" point if they match
            if (lastPosition != null)
            {
                AddPoint(lastPosition, "End", "end");
                lastPosition = null;
            }
        }

        protected async Task AutoFlush()
        {
            var now = DateTime.Now;
            var diff = now - lastFlush;

            if (diff < flushInterval)
                return;

            await Flush();
            lastFlush = now;
        }

        protected async Task Flush()
        {
            var serializer = new Serializer();
            serializer.Serialize(output);

            using (var writer = new StreamWriter(filename))
            {
                await writer.WriteAsync(serializer.Xml);
            }
        }


        protected void EnsureOutput(DateTime eventTime)
        {
            if (output != null)
                return;

            var dateString = eventTime.ToString("yyyy-MM-dd HH.mm.ss");
            filename = Path.Combine(path, dateString + ".kml");

            // Create folder
            rootFolder = new Folder
            {
                Name = dateString,
                Open = true
            };


            // Create flight path line and placemark
            positionPath = new LineString
            {
                Tessellate = false,
                AltitudeMode = AltitudeMode.Absolute,
                Coordinates = new CoordinateCollection()
            };

            var positionPlacemark = new Placemark
            {
                Name = "Flight path",
                StyleUrl = new Uri("#flightpath", UriKind.Relative),
                Geometry = positionPath
            };

            rootFolder.AddFeature(positionPlacemark);

            output = new Document();

            AddFlightPathStyleMap();

            var paddleHotspot = new Hotspot { X = 31, XUnits = Unit.Pixel, Y = 1, YUnits = Unit.Pixel };
            AddIconStyleMap("start", 1.1, "http://maps.google.com/mapfiles/kml/paddle/grn-circle.png", "http://maps.google.com/mapfiles/kml/paddle/grn-circle-lv.png", paddleHotspot);
            AddIconStyleMap("end", 1.1, "http://maps.google.com/mapfiles/kml/paddle/red-square.png", "http://maps.google.com/mapfiles/kml/paddle/red-square-lv.png", paddleHotspot);
            AddIconStyleMap("fullstop", 1.1, "http://maps.google.com/mapfiles/kml/shapes/placemark_circle.png", null, paddleHotspot);

            output.AddFeature(rootFolder);
        }


        protected void AddFlightPathStyleMap()
        {
            var styleMap = new StyleMapCollection { Id = "flightpath" };
            styleMap.Add(new Pair { State = StyleState.Normal, StyleUrl = new Uri("flightpath-normal", UriKind.Relative) });
            styleMap.Add(new Pair { State = StyleState.Highlight, StyleUrl = new Uri("flightpath-normal", UriKind.Relative) });

            var styleNormal = new Style
            {
                Id = "flightpath-normal",
                Line = new LineStyle
                {
                     Color = new Color32(255, 10, 10, 138),
                     Width = 5
                }
            };

            output.AddStyle(styleMap);
            output.AddStyle(styleNormal);
        }


        protected void AddIconStyleMap(string id, double scale, string iconUrl, string listUrl, Hotspot iconHotspot = null)
        {
            var styleMap = new StyleMapCollection { Id = id };
            styleMap.Add(new Pair { State = StyleState.Normal, StyleUrl = new Uri(id + "-normal", UriKind.Relative) });
            styleMap.Add(new Pair { State = StyleState.Highlight, StyleUrl = new Uri(id + "-normal", UriKind.Relative) });

            var styleNormal = new Style
            {
                Id = id + "-normal",
                Icon = new IconStyle
                {
                    Scale = scale,
                    Icon = new IconStyle.IconLink(new Uri(iconUrl, UriKind.Absolute)),
                    Hotspot = iconHotspot
                },
            };

            if (!string.IsNullOrEmpty(listUrl))
            {
                styleNormal.List = new ListStyle();
                styleNormal.List.AddItemIcon(new ItemIcon
                {
                    Href = new Uri(listUrl, UriKind.Absolute)
                });
            }


            output.AddStyle(styleMap);
            output.AddStyle(styleNormal);
        }


        protected string GetPointDate()
        {
            var now = DateTime.Now;

            // If the date hasn't changed since the last point label, just return the time
            if (now.Date == lastPointDate)
                return now.ToString("T");

            lastPointDate = now.Date;
            return now.ToString("F");
        }


        protected void AddPoint(Vector coordinate, string label, string styleMapId, bool includeTimestamp = true)
        {
            var point = new Point { Coordinate = coordinate, AltitudeMode = AltitudeMode.Absolute };

            var placemark = new Placemark
            {
                Name = includeTimestamp ? $"{label} ({GetPointDate()})" : label,
                StyleUrl = new Uri("#" + styleMapId, UriKind.Relative),
                Geometry = point,
                Visibility = true
            };

            rootFolder.AddFeature(placemark);
        }


        private const float MetersPerFoot = 0.3048f;


        public async Task LogPosition(DateTime eventTime, FlightPosition position)
        {
            EnsureOutput(eventTime);

            var altitudeMeters = position.Altitude * MetersPerFoot;
            var coordinate = new Vector(position.Latitude, position.Longitude, altitudeMeters);

            if (lastPosition == null)
                AddPoint(coordinate, "Start", "start");

            if (lastSpeed > 0 && position.Airspeed == 0)
                AddPoint(coordinate, "Full stop", "fullstop");

            lastPosition = coordinate;
            lastSpeed = position.Airspeed;

            positionPath.Coordinates.Add(coordinate);

            await AutoFlush();
        }


        // TODO log events, engine stop etc.
    }
}
