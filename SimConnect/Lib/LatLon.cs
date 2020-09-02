using System;

namespace SimConnect.Lib
{
    /// <summary>
    /// Provides helpers methods for latitude / longitude calculations.
    /// </summary>
    public static class LatLon
    {
        /// <summary>
        /// Defines the approximate radius of the earth in kilometers.
        /// </summary>
        public const double EarthRadiusKm = 6378.137;


        // Source: https://stackoverflow.com/questions/639695/how-to-convert-latitude-or-longitude-to-meters
        /// <summary>
        /// Calculates the distance between two coordinates.
        /// </summary>
        /// <param name="lat1">Latitude of point 1 in degrees</param>
        /// <param name="lon1">Longitude of point 1 in degrees</param>
        /// <param name="lat2">Latitude of point 2 in degrees</param>
        /// <param name="lon2">Longitude of point 2 in degrees</param>
        /// <returns></returns>
        public static double DistanceBetweenInMeters(float lat1, float lon1, float lat2, float lon2)
        {
            var distanceLat = lat2 * Math.PI / 180 - lat1 * Math.PI / 180;
            var distanceLon = lon2 * Math.PI / 180 - lon1 * Math.PI / 180;

            var a = Math.Sin(distanceLat / 2) * Math.Sin(distanceLat / 2) +
                    Math.Cos(lat1 * Math.PI / 180) * Math.Cos(lat2 * Math.PI / 180) *
                    Math.Sin(distanceLon / 2) * Math.Sin(distanceLon / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            var distanceKm = EarthRadiusKm * c;

            return distanceKm * 1000;
        }
    }
}
