using System;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Ionic.BZip2;
using Nito.AsyncEx;
using SharpKml.Base;
using SharpKml.Dom;

namespace FlightLoggerLib.Concrete
{
    public class KMLLiveFlightLogger : IFlightLogger
    {
        private readonly CancellationTokenSource listenerCancellationTokenSource = new CancellationTokenSource();
        private readonly HttpListener listener;
        private readonly Task listenerTask;
        private readonly string baseUrl;

        private FlightPosition currentPosition;
        private readonly AsyncLock currentPositionLock = new AsyncLock();

        private byte[] entryDocument;
        private Document dynamicDocument;
        private Placemark dynamicPlacemark;


        public KMLLiveFlightLogger(int port)
        {
            baseUrl = $"http://127.0.0.1:{port}/";
            PrepareEntryDocument();

            listener = new HttpListener();
            listener.Prefixes.Add(baseUrl);
            listener.Start();

            listenerTask = Task.Run(RunServer);
        }


        public async ValueTask DisposeAsync()
        {
            listener?.Stop();
            listenerCancellationTokenSource.Cancel();

            await listenerTask;
        }


        protected void PrepareEntryDocument()
        {
            var networkLink = new NetworkLink
            {
                Name = "Refreshes every 5 seconds",
                Link = new Link
                {
                    Href = new Uri($"{baseUrl}live/dynamic", UriKind.Absolute),
                    RefreshMode = RefreshMode.OnInterval,
                    RefreshInterval = 5
                }
            };

            var output = new Document();
            output.AddFeature(networkLink);

            var serializer = new Serializer();
            serializer.Serialize(output);
            entryDocument = Encoding.UTF8.GetBytes(serializer.Xml);
        }


        protected void PrepareDynamicDocument(Vector coordinate, float altitudeInFeet)
        {
            var name = $"Live location ({altitudeInFeet:#} feet)";

            if (dynamicPlacemark != null)
            {
                dynamicPlacemark.Name = name;
                ((Point)dynamicPlacemark.Geometry).Coordinate = coordinate;
                return;
            }

            var point = new Point { Coordinate = coordinate, AltitudeMode = AltitudeMode.Absolute };
            dynamicPlacemark = new Placemark
            {
                Name = name,
                //StyleUrl = new Uri("#" + styleMapId, UriKind.Relative),
                Geometry = point,
                Visibility = true
            };

            dynamicDocument = new Document();
            dynamicDocument.AddFeature(dynamicPlacemark);
        }


        public Task NewLog()
        {
            return Task.CompletedTask;
        }


        public async Task LogPosition(DateTime eventTime, FlightPosition position)
        {
            using (await currentPositionLock.LockAsync())
            {
                currentPosition = position;
            }
        }


        private const float MetersPerFoot = 0.3048f;


        private async Task RunServer()
        {
            while (!listenerCancellationTokenSource.IsCancellationRequested)
            {
                var context = await listener.GetContextAsync();

                switch (context.Request.Url.AbsolutePath)
                {
                    case "/live":
                        context.Response.StatusCode = 200;
                        await context.Response.OutputStream.WriteAsync(entryDocument, 0, entryDocument.Length);
                        break;

                    case "/live/dynamic":
                    {
                        byte[] buffer;

                        using (await currentPositionLock.LockAsync())
                        {
                            var altitudeFeet = currentPosition?.Altitude ?? -10;
                            var altitudeMeters = altitudeFeet * MetersPerFoot;

                            PrepareDynamicDocument(new Vector
                            {
                                Latitude = currentPosition?.Latitude ?? 24.999979,
                                Longitude = currentPosition?.Longitude ?? -70.999997,
                                Altitude = altitudeMeters
                            },
                                altitudeFeet);

                            var serializer = new Serializer();
                            serializer.Serialize(dynamicDocument);
                            buffer = Encoding.UTF8.GetBytes(serializer.Xml);
                        }

                        context.Response.StatusCode = 200;
                        await context.Response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
                        break;
                    }

                    default:
                        context.Response.StatusCode = 404;
                        break;
                }

                context.Response.Close();
            }
        }
    }
}
