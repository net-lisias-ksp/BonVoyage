/*
	This file is part of Bon Voyage /L
		© 2024 LisiasT : http://lisias.net <support@lisias.net>
		© 2018-2024 Maja
		© 2016-2018 RealGecko

	Bon Voyage /L is licensed as follows:
		* GPL 3.0 : https://www.gnu.org/licenses/gpl-3.0.txt

	Bon Voyage /L is distributed in the hope that it will be useful,
	but WITHOUT ANY WARRANTY; without even the implied warranty of
	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.

	You should have received a copy of the GNU General Public License 3.0
	along with Bon Voyage /L. If not, see <https://www.gnu.org/licenses/>.

*/
using System;

namespace BonVoyage
{
    internal class GeoUtils
    {
        private const double PI = Math.PI;

        /// <summary>
        /// Distance between two coordinates.
        /// Haversine algorithm - http://www.movable-type.co.uk/scripts/latlong.html
        /// </summary>
        /// <param name="startLatitude">Start latitude</param>
        /// <param name="startLongitude">Start longitude</param>
        /// <param name="endLatitude">End latitude</param>
        /// <param name="endLongitude">End longitude</param>
        /// <param name="radius">Radius</param>
        /// <returns>Distance</returns>
        internal static double GetDistance(double startLatitude, double startLongitude, double endLatitude, double endLongitude, double radius)
        {
            double deltaLatitude = PI / 180 * (endLatitude - startLatitude);
            double deltaLongitude = PI / 180 * (endLongitude - startLongitude);

            startLatitude = PI / 180 * startLatitude;
            startLongitude = PI / 180 * startLongitude;
            endLatitude = PI / 180 * endLatitude;
            endLongitude = PI / 180 * endLongitude;

            double a = Math.Pow(Math.Sin(deltaLatitude / 2), 2) + Math.Cos(startLatitude) * Math.Cos(endLatitude) *
                Math.Pow(Math.Sin(deltaLongitude / 2), 2);

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            double distance = radius * c;
            return distance;
        }


        /// <summary>
        /// Distance between two coordinates - rad version.
        /// Haversine algorithm - http://www.movable-type.co.uk/scripts/latlong.html
        /// </summary>
        /// <param name="startLatitude">Start latitude</param>
        /// <param name="startLongitude">Start longitude</param>
        /// <param name="endLatitude">End latitude</param>
        /// <param name="endLongitude">End longitude</param>
        /// <param name="radius">Radius</param>
        /// <returns>Distance</returns>
        internal static double GetDistanceRad(double startLatitude, double startLongitude, double endLatitude, double endLongitude, double radius)
        {
            double deltaLatitude = endLatitude - startLatitude;
            double deltaLongitude = endLongitude - startLongitude;

            double a = Math.Pow(Math.Sin(deltaLatitude / 2), 2) + Math.Cos(startLatitude) * Math.Cos(endLatitude) *
                Math.Pow(Math.Sin(deltaLongitude / 2), 2);

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            double distance = radius * c;
            return distance;
        }


        // Alternative haversine distance implementation
        // https://www.kaggle.com/c/santas-stolen-sleigh/forums/t/18049/simpler-faster-haversine-distance
        //public static double GetDistanceAlt()
		//{
		//}


        /// <summary>
        /// Bearing from start to end.
        /// http://www.movable-type.co.uk/scripts/latlong.html
        /// </summary>
        /// <param name="startLatitude">Start latitude</param>
        /// <param name="startLongitude">Start longitude</param>
        /// <param name="targetLatitude">Target latitude</param>
        /// <param name="targetLongitude">Target longitude</param>
        /// <returns>Bearing</returns>
        internal static double InitialBearing(double startLatitude, double startLongitude, double targetLatitude, double targetLongitude)
        {
            startLatitude = PI / 180 * startLatitude;
            startLongitude = PI / 180 * startLongitude;
            targetLatitude = PI / 180 * targetLatitude;
            targetLongitude = PI / 180 * targetLongitude;

            double y = Math.Sin(targetLongitude - startLongitude) * Math.Cos(targetLatitude);
            double x = Math.Cos(startLatitude) * Math.Sin(targetLatitude) -
                Math.Sin(startLatitude) * Math.Cos(targetLatitude) * Math.Cos(targetLongitude - startLongitude);

            double bearing = Math.Atan2(y, x);
            bearing = (bearing * 180.0 / PI + 360) % 360;

            return bearing;
        }


        /// <summary>
        /// Bearing from start to end - rad version.
        /// http://www.movable-type.co.uk/scripts/latlong.html
        /// </summary>
        /// <param name="startLatitude">Start latitude</param>
        /// <param name="startLongitude">Start longitude</param>
        /// <param name="targetLatitude">Target latitude</param>
        /// <param name="targetLongitude">Target longitude</param>
        /// <returns>Bearing</returns>
        internal static double InitialBearingRad(double startLatitude, double startLongitude, double targetLatitude, double targetLongitude)
        {
            double y = Math.Sin(targetLongitude - startLongitude) * Math.Cos(targetLatitude);
            double x = Math.Cos(startLatitude) * Math.Sin(targetLatitude) -
                Math.Sin(startLatitude) * Math.Cos(targetLatitude) * Math.Cos(targetLongitude - startLongitude);

            double bearing = Math.Atan2(y, x);
            return bearing;
        }


        /// <summary>
        /// Bearing at destination.
        /// http://www.movable-type.co.uk/scripts/latlong.html
        /// </summary>
        /// <param name="startLatitude">Start latitude</param>
        /// <param name="startLongitude">Start longitude</param>
        /// <param name="targetLatitude">Target latitude</param>
        /// <param name="targetLongitude">Target longitude</param>
        /// <returns>Bearing</returns>
        internal static double FinalBearing(double startLatitude, double startLongitude, double targetLatitude, double targetLongitude)
        {
            double bearing = InitialBearing(targetLatitude, targetLongitude, startLatitude, startLongitude);
            bearing = (bearing + 180) % 360;
            return bearing;
        }


        /// <summary>
        /// Bearing at destination - rad version.
        /// http://www.movable-type.co.uk/scripts/latlong.html
        /// </summary>
        /// <param name="startLatitude">Start latitude</param>
        /// <param name="startLongitude">Start longitude</param>
        /// <param name="targetLatitude">Target latitude</param>
        /// <param name="targetLongitude">Target longitude</param>
        /// <returns>Bearing</returns>
        internal static double FinalBearingRad(double startLatitude, double startLongitude, double targetLatitude, double targetLongitude)
        {
            double bearing = InitialBearingRad(targetLatitude, targetLongitude, startLatitude, startLongitude);
            //			bearing = (bearing + 180) % 360;
            return bearing;
        }


        /// <summary>
        /// "Reverse Haversine" Formula.
        /// https://gist.github.com/shayanjm/644d895c1fad80b49919
        /// </summary>
        /// <param name="latStart">Start latitude</param>
        /// <param name="lonStart">Start longitude</param>
        /// <param name="bearing">Bearing</param>
        /// <param name="distance">Distance</param>
        /// <param name="radius">Radius</param>
        /// <returns>Latitude and longitude</returns>
        internal static double[] GetLatitudeLongitude(double latStart, double lonStart, double bearing, double distance, double radius)
        {
            latStart = PI / 180 * latStart;
            lonStart = PI / 180 * lonStart;
            bearing = PI / 180 * bearing;

            var latEnd = Math.Asin(Math.Sin(latStart) * Math.Cos(distance / radius) +
                Math.Cos(latStart) * Math.Sin(distance / radius) * Math.Cos(bearing));
            var lonEnd = lonStart + Math.Atan2(Math.Sin(bearing) * Math.Sin(distance / radius) * Math.Cos(latStart),
                Math.Cos(distance / radius) - Math.Sin(latStart) * Math.Sin(latEnd));

            return new double[] {
                latEnd * 180.0 / PI,
                lonEnd * 180.0 / PI
            };
        }


        /// <summary>
        /// "Reverse Haversine" Formula - rad version.
        /// https://gist.github.com/shayanjm/644d895c1fad80b49919
        /// </summary>
        /// <param name="latStart">Start latitude</param>
        /// <param name="lonStart">Start longitude</param>
        /// <param name="bearing">Bearing</param>
        /// <param name="distance">Distance</param>
        /// <param name="radius">Radius</param>
        /// <returns>Latitude and longitude</returns>
        internal static double[] GetLatitudeLongitudeRad(double latStart, double lonStart, double bearing, double distance, double radius)
        {
            var latEnd = Math.Asin(Math.Sin(latStart) * Math.Cos(distance / radius) +
                Math.Cos(latStart) * Math.Sin(distance / radius) * Math.Cos(bearing));
            var lonEnd = lonStart + Math.Atan2(Math.Sin(bearing) * Math.Sin(distance / radius) * Math.Cos(latStart),
                Math.Cos(distance / radius) - Math.Sin(latStart) * Math.Sin(latEnd));

            return new double[] {
                latEnd,
                lonEnd
            };
        }


        /// <summary>
        /// Step back from target by 'step' meters.
        /// </summary>
        /// <param name="startLatitude">Start latitude</param>
        /// <param name="startLongitude">Start longitude</param>
        /// <param name="endLatitude">End latitude</param>
        /// <param name="endLongitude">End longitude</param>
        /// <param name="radius">Radius</param>
        /// <param name="step">Step</param>
        /// <returns>The back</returns>
        internal static double[] StepBack(double startLatitude, double startLongitude, double endLatitude, double endLongitude, double radius, double step)
        {
            double distanceToTarget = GetDistance(startLatitude, startLongitude, endLatitude, endLongitude, radius);
            if (distanceToTarget <= step)
                return null;
            distanceToTarget -= step;
            double bearing = InitialBearing(startLatitude, startLongitude, endLatitude, endLongitude);
            return GetLatitudeLongitude(startLatitude, startLongitude, bearing, distanceToTarget, radius);
        }


        /// <summary>
        /// Get altitude at point.
        /// Approximation, because the PQS controller gives the theoretical ideal smooth surface curve terrain
        /// </summary>
        /// <param name="latitude">Latitude</param>
        /// <param name="longitude">Longitude</param>
        /// <param name="body">Celestial body</param>
        /// <returns>Altitude</returns>
        internal static double TerrainHeightAt(double latitude, double longitude, CelestialBody body)
        {
            // Stars and gas giants don't have a surface
            if (body.pqsController == null)
                return 0;

            // Figure out the terrain height
            double latRads = PI / 180.0 * latitude;
            double lonRads = PI / 180.0 * longitude;
            Vector3d radialVector = new Vector3d(Math.Cos(latRads) * Math.Cos(lonRads), Math.Sin(latRads), Math.Cos(latRads) * Math.Sin(lonRads));
            return body.pqsController.GetSurfaceHeight(radialVector) - body.pqsController.radius;
        }


        /// <summary>
        /// Get terrain normal vector at specified latitude, longitude and height
        /// </summary>
        /// <param name="latitude">Latitude</param>
        /// <param name="longitude">Longitude</param>
        /// <param name="altitude">Altitude</param>
        /// <param name="body">Celestial body</param>
        /// <returns>Terrain normal vector</returns>
        internal static Vector3d GetTerrainNormal(double latitude, double longitude, double altitude, CelestialBody body)
        {
            return (body.GetWorldSurfacePosition(latitude, longitude, altitude) - body.position).normalized;
        }

    }

}