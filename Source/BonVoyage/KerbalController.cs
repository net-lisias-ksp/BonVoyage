/*
	This file is part of Bon Voyage /L
		© 2024-2026 LisiasT : http://lisias.net <support@lisias.net>
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
using System.Collections.Generic;

using KSP.Localization;
using KSP.UI.Screens;

using BonVoyage.MovementControllers;
using BonVoyage.PowerSources;


namespace BonVoyage
{
    internal class KerbalController : BVController
    {
        #region internal properties

        internal override double CalcAverageSpeed() => this.moveController.averageSpeed;

        #endregion


        #region Private properties

		private readonly Controller moveController;

		#endregion


		internal static BVController Create(Vessel vessel, ConfigNode moduleConfigNode)
		{
			// TODO: To intantiate the proper PowerSources for the current Installation!
			Converter fuelCellPowerSource = new SnacksPoweredConverter(vessel);
			SolarPower solarPowerSource = new NoSolarPower(vessel);
			Controller moveController = new FootController(vessel, moduleConfigNode);

			return new KerbalController(vessel, moduleConfigNode, fuelCellPowerSource, solarPowerSource, moveController);
		}
		protected KerbalController(Vessel v, ConfigNode module, Converter fuelCellPowerSource, SolarPower solarPowerSource, Controller moveController) : base(v, module, fuelCellPowerSource, solarPowerSource)
		{
			this.moveController = moveController;
		}


        /// <summary>
        /// Get controller type
        /// </summary>
        /// <returns></returns>
        internal override int GetControllerType()
        {
            return 2;
        }


        #region Status window texts

        internal override List<DisplayedSystemCheckWidget[]> GetDisplayedSystemCheckResults()
        {
            base.GetDisplayedSystemCheckResults();

			DisplayedSystemCheckWidget[] result = new DisplayedSystemCheckWidget[] {
				new DisplayedSystemCheckWidget {
                    Label = Localizer.Format("#LOC_BV_Control_AverageSpeed"),
                    Text = this.moveController.AverageSpeedAsText,
                    Tooltip = ""
                }
            };
			this.displayedSystemCheckWidgets.Add(result);

			return this.displayedSystemCheckWidgets;
        }

        #endregion


        #region Pathfinder

        /// <summary>
        /// Find a route to the target
        /// </summary>
        /// <param name="lat"></param>
        /// <param name="lon"></param>
        /// <returns></returns>
        internal override bool FindRoute(double lat, double lon)
        {
            return FindRoute(lat, lon, TileTypes.Land | TileTypes.Ocean);
        }

        #endregion


        /// <summary>
        /// Check the systems
        /// </summary>
        internal override void SystemCheck()
        {
            base.SystemCheck();
			this.fuelEnergy.Check(100);
			this.moveController.Check(100);
			this.calcCurrentSituation();
        }


        /// <summary>
        /// Activate autopilot
        /// </summary>
        internal override bool Activate()
        {
            if (vessel.situation != Vessel.Situations.LANDED && vessel.situation != Vessel.Situations.SPLASHED)
            {
                ScreenMessages.PostScreenMessage(Localizer.Format("#LOC_BV_Warning_Landed_Splashed"), 5f).color = CommonWindowProperties.Message_Colour_Warning;
                return false;
            }

            SystemCheck();

            BonVoyageModule module = vessel.FindPartModuleImplementing<BonVoyageModule>();
            if (module != null)
            {
				module.averageSpeed = this.moveController.averageSpeed;
				module.vesselHeightFromTerrain = this.moveController.vesselHeightFromTerrain;
            }

            return base.Activate();
        }


        /// <summary>
        /// Deactivate autopilot
        /// </summary>
        internal override bool Deactivate()
        {
            SystemCheck();
            return base.Deactivate();
        }

        /// <summary>
        /// Update Kerbal
        /// </summary>
        /// <param name="currentTime"></param>
        protected override void update(double currentTime)
        {
			this.calcCurrentSituation();

            double deltaT = currentTime - lastTimeUpdated; // Time delta from the last update
            double deltaTOver = 0; // deltaT which is calculated from a value over the maximum resource amout available

            this.fuelEnergy.Update(ref deltaT, ref deltaTOver);

			double deltaS = this.CalcAverageSpeed() * deltaT; // Distance delta from the last update
            distanceTravelled += deltaS;

            if (distanceTravelled >= distanceToTarget) // We reached the target
            {
                if (!this.moveController.MoveSafely(targetLatitude, targetLongitude))
                    distanceTravelled -= deltaS;
                else
                {
                    distanceTravelled = distanceToTarget;

                    active = false;
                    arrived = true;
                    BVModule.SetValue("active", "False");
                    BVModule.SetValue("arrived", "True");
                    BVModule.SetValue("distanceTravelled", distanceToTarget.ToString());
                    BVModule.SetValue("pathEncoded", "");

					Tools.OptionalDewarpAndDo(
							() => ScreenMessages.PostScreenMessage(Tools.BuildVesselMessage(vessel, "#LOC_BV_VesselArrived"), 5f)
						);

                    NotifyArrival();
                }
                State = VesselState.Idle;
            }
            else
            {
                try // There is sometimes null ref exception during scene change
                {
                    int step = Convert.ToInt32(Math.Floor(distanceTravelled / PathFinder.StepSize)); // In which step of the path we are
                    double remainder = distanceTravelled % PathFinder.StepSize; // Current remaining distance from the current step
                    double bearing = 0;

                    if (step < path.Count - 1)
                        bearing = GeoUtils.InitialBearing( // Bearing to the next step from previous step
                            path[step].latitude,
                            path[step].longitude,
                            path[step + 1].latitude,
                            path[step + 1].longitude
                        );
                    else
                        bearing = GeoUtils.InitialBearing( // Bearing to the target from previous step
                            path[step].latitude,
                            path[step].longitude,
                            targetLatitude,
                            targetLongitude
                        );

                    // Compute new coordinates, we are moving from the current step, distance is "remainder"
                    double[] newCoordinates = GeoUtils.GetLatitudeLongitude(
                        path[step].latitude,
                        path[step].longitude,
                        bearing,
                        remainder,
                        vessel.mainBody.Radius
                    );

                    // Move
                    if (!this.moveController.MoveSafely(newCoordinates[0], newCoordinates[1]))
                    {
                        distanceTravelled -= deltaS;
                        State = VesselState.Idle;
                    }
                    else
                        State = VesselState.Moving;
                }
				catch (Exception e)
				{
					Log.dbg(e);
				}
            }
        }

        /// <summary>
        /// Notify, that Kerbal has arrived
        /// </summary>
        private void NotifyArrival()
        {
            MessageSystem.Message message = new MessageSystem.Message(
                Localizer.Format("#LOC_BV_Title_KerbalArrived"), // title
                Tools.BuildVesselMessage("#74B4E2", vessel, "#LOC_BV_VesselArrived") + ".\n<color=#AED6EE>"
                + Localizer.Format("#LOC_BV_Control_Lat") + ": " + targetLatitude.ToString("F2") + "</color>\n<color=#AED6EE>" + Localizer.Format("#LOC_BV_Control_Lon") + ": " + targetLongitude.ToString("F2") + "</color>", // message
                MessageSystemButton.MessageButtonColor.GREEN,
                MessageSystemButton.ButtonIcons.COMPLETE
            );
            MessageSystem.Instance.AddMessage(message);
        }

		private void calcCurrentSituation()
		{
			if (this.vessel != FlightGlobals.ActiveVessel) return;
			this.vessel.ActionGroups.SetGroup(KSPActionGroup.Light, this.IsNight);
		}
	}

}
