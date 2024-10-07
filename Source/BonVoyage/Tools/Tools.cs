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
using UnityEngine;
using System.Collections.Generic;

namespace BonVoyage
{
    internal static class Tools
    {
        /// <returns>
		/// The full relative path from the main KSP folder to a given texture resource from this mod.
		/// </returns>
		/// <param name="filename">Name of file located in our plugin folder</param>
		/// <param name="GameDataRelative">True if the KSP/GameData portion of the path is assumed, false if we need to provide the full path</param>
		internal static string TextureFilePath(string filename, bool GameDataRelative = true)
        {
            if (GameDataRelative)
            {
                return string.Format("{0}/Textures/{1}", BonVoyage.Name, filename);
            }
            else
            {
                return string.Format("{0}/Textures/{1}",
                    System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location),
                    filename);
            }
        }


        /// <summary>
        /// Convert distance in meters to text (meters or kilometers)
        /// </summary>
        /// <param name="distance"></param>
        /// <returns></returns>
        internal static string ConvertDistanceToText(double distance)
        {
            string result = "-";
            double n = distance;
            if (n > 0)
            {
                if (n < 1000)
                    result = n.ToString("N0") + " m";
                else
                {
                    n = n / 1000;
                    result = n.ToString("0.##") + " km";
                }
            }
            return result;
        }


        /// <summary>
        /// Returns latitude and longitude of a waypoint or double.MinValue, if waypoint is not valid
        /// </summary>
        /// <param name="v"></param>
        /// <returns>[latitude, longitude]</returns>
        internal static double[] GetCurrentWaypointLatLon(Vessel v)
        {
            NavWaypoint waypoint = NavWaypoint.fetch;
            if ((waypoint == null) || !waypoint.IsActive || (waypoint.Body != v.mainBody))
                return new double[2] { double.MinValue, double.MinValue };
            else
                return new double[2] { waypoint.Latitude, waypoint.Longitude };
        }


        /// <summary>
        /// Returns latitude and longitude of a target or double.MinValue, if target is not valid
        /// </summary>
        /// <param name="v"></param>
        /// <returns>[latitude, longitude]</returns>
        internal static double[] GetCurrentTargetLatLon(Vessel v)
        {
            if (v.targetObject == null)
                return new double[2] { double.MinValue, double.MinValue };

            Vessel target = v.targetObject.GetVessel();
            if ((target == null) || (target.situation != Vessel.Situations.LANDED) || (target.mainBody != v.mainBody))
                return new double[2] { double.MinValue, double.MinValue };
            else
                return new double[2] { target.latitude, target.longitude };
        }


        /// <summary>
        /// Place target at cursor in the map mode. Borrowed Waypoint Manager and changed.
        /// Returns latitude and longitude of a target or double.MinValue, if target is not valid
        /// </summary>
        /// <param name="targetBody"></param>
        /// <returns>[latitude, longitude]</returns>
        internal static double[] PlaceTargetAtCursor(CelestialBody targetBody)
        {
            if (targetBody.pqsController == null)
                return new double[2] { double.MinValue, double.MinValue };

            Ray mouseRay = PlanetariumCamera.Camera.ScreenPointToRay(Input.mousePosition);
            mouseRay.origin = ScaledSpace.ScaledToLocalSpace(mouseRay.origin);
            var bodyToOrigin = mouseRay.origin - targetBody.position;
            double curRadius = targetBody.pqsController.radiusMax;
            double lastRadius = 0;
            int loops = 0;
            while (loops < 50)
            {
                Vector3d relSurfacePosition;
                if (PQS.LineSphereIntersection(bodyToOrigin, mouseRay.direction, curRadius, out relSurfacePosition))
                {
                    var surfacePoint = targetBody.position + relSurfacePosition;
                    double alt = targetBody.pqsController.GetSurfaceHeight(QuaternionD.AngleAxis(targetBody.GetLongitude(surfacePoint), Vector3d.down) * QuaternionD.AngleAxis(targetBody.GetLatitude(surfacePoint), Vector3d.forward) * Vector3d.right);
                    double error = Math.Abs(curRadius - alt);
                    if (error < (targetBody.pqsController.radiusMax - targetBody.pqsController.radiusMin) / 100)
                    {
                        return new double[2] {
                            targetBody.GetLatitude(surfacePoint),
                            targetBody.GetLongitude(surfacePoint)
                        };
                    }
                    else
                    {
                        lastRadius = curRadius;
                        curRadius = alt;
                        loops++;
                    }
                }
                else
                {
                    if (loops == 0)
                    {
                        break;
                    }
                    else // Went too low, needs to try higher
                    {
                        curRadius = (lastRadius * 9 + curRadius) / 10;
                        loops++;
                    }
                }
            }
            return new double[2] { double.MinValue, double.MinValue };
        }


        /// <summary>
        /// Return main star for a vessel
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        internal static CelestialBody GetMainStar(Vessel v)
        {
            CelestialBody body = v.mainBody;
            if (!AssemblyIsLoaded("Kopernicus"))
            {
                while (body.referenceBody != body) // Last body has reference to itself???
                    body = body.referenceBody;
            }
            else // If Kopernicus is present, find the name of a current star
            {
                bool found = false;
                string currentStarName = KopernicusWrapper.GetCurrentStarName();
                while ((body.referenceBody != body) && !found)
                {
                    body = body.referenceBody;
                    if (body.name == currentStarName)
                        found = true;
                }
            }
            return body;
        }


        /// <summary>
        /// Check if specific assembly is loaded
        /// </summary>
        /// <param name="name">Assembly name</param>
        /// <returns>True = assembly loaded. False = Assembly not loaded.</returns>
        internal static bool AssemblyIsLoaded(string assemblyName)
        {
            bool result = false;

            int i = 0;
            while (!result && (i < AssemblyLoader.loadedAssemblies.Count))
            {
                if (AssemblyLoader.loadedAssemblies[i].name == assemblyName)
                    result = true;
                i++;
            }

            return result;
        }


        /// <summary>
        /// Get index of specific assembly
        /// </summary>
        /// <param name="assemblyName"></param>
        /// <returns></returns>
        internal static int GetAssemblyIndex(string assemblyName)
        {
            int i = 0;
            while (i < AssemblyLoader.loadedAssemblies.Count)
            {
                if (AssemblyLoader.loadedAssemblies[i].name == assemblyName)
                    return i;
                i++;
            }
            return -1;
        }


        internal static bool TechIsAvailable(string techId)
        {
            if ((HighLogic.CurrentGame.Mode != Game.Modes.CAREER) && (HighLogic.CurrentGame.Mode != Game.Modes.SCIENCE_SANDBOX))
                return true;

            if (ResearchAndDevelopment.Instance == null)
                return false;
            
            var techstate = ResearchAndDevelopment.Instance.GetTechState(techId);
            if (techstate != null)
                return techstate.state == RDTech.State.Available;

            return false;
        }

    }

}
