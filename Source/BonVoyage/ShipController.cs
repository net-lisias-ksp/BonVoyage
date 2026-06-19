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

using BonVoyage.PowerSources;
using BonVoyage.MovementControllers;

namespace BonVoyage
{
    /// <summary>
    /// Ship controller. Child of BVController
    /// </summary>
    internal class ShipController : BVController
    {
        #region internal properties

		internal override double CalcAverageSpeed() =>
			(this.IsDay)
				? (this.moveController.averageSpeed * speedMultiplier)
				: (averageSpeedAtNight * speedMultiplier)
			;

        #endregion


        #region Private properties

        private readonly Controller moveController;

        // Config values
        private double averageSpeedAtNight = 0;
        private bool manned = false;
        // Config values

        private double speedMultiplier;
        private int crewSpeedBonus; // Speed modifier based on the available crew
        private int throttle = 100; // Allowed values: 100, 75, 50, 25

		// Basic values
		private readonly double thrust0 = 350; // 350kN
		private readonly double speed0 = 50; // 50m/s
		private readonly double mass0 = 25; // 25t
		private readonly double density0 = 1.11; // 1.11 - half of density of water plus half of density of air on Kerbin

        // Reduction of speed based on difference between required and available power in percents
        private double SpeedReduction
        {
            get
            {
                double speedReduction = 0;
				if (this.requiredPower > this.electricPower)
					speedReduction = Math.Sqrt((this.requiredPower - this.electricPower) / this.requiredPower);
				speedReduction = (speedReduction > 0.87) ? 1 : speedReduction;
                return speedReduction;
            }
        }

		#endregion


		internal static BVController Create(Vessel vessel, ConfigNode moduleConfigNode)
		{
			// TODO: To intantiate the proper PowerSources for the current Installation!
			Converter propellantPowerSource = new PropellantPoweredConverter(vessel);
			SolarPower solarPowerSource = new StockSolarPower(vessel);
			Controller moveController = new EngineController(vessel, moduleConfigNode);

			return new ShipController(vessel, moduleConfigNode, propellantPowerSource, solarPowerSource, moveController);
		}
		protected ShipController(Vessel v, ConfigNode module, Converter fuelCellPowerSource, SolarPower solarPowerSource, Controller moveController) : base(v, module, fuelCellPowerSource, solarPowerSource)
        {
			this.moveController = moveController;

            // Load values from config if it isn't the first run of the mod (we are reseting vessel on the first run)
            if (!Configuration.FirstRun)
            {
				this.averageSpeedAtNight = double.Parse(BVModule.GetValue("averageSpeedAtNight") ?? "0");
				this.manned = bool.Parse(BVModule.GetValue("manned") ?? "false");
            }

            speedMultiplier = 1.0;
            angle = 0;
            crewSpeedBonus = 0;
        }


        /// <summary>
        /// Get controller type
        /// </summary>
        /// <returns></returns>
        internal override int GetControllerType()
        {
            return 1;
        }


        #region Status window texts

		internal override List<DisplayedSystemCheckWidget[]> GetDisplayedSystemCheckResults()
        {
            base.GetDisplayedSystemCheckResults();

			DisplayedSystemCheckWidget[] result = new DisplayedSystemCheckWidget[] {
                new DisplayedSystemCheckWidget
                {
                    Label = Localizer.Format("#LOC_BV_Control_AverageSpeed"),
                    Text = this.moveController.AverageSpeedAsText,
                    Tooltip = Localizer.Format("#LOC_BV_Control_SpeedBase") + ": " + this.moveController.maxSpeedBase.ToString("F") + " m/s\n"
                        + (manned ? Localizer.Format("#LOC_BV_Control_DriverBonus") + ": " + crewSpeedBonus.ToString() + "%\n" : Localizer.Format("#LOC_BV_Control_UnmannedPenalty") + ": " + GetUnmannedSpeedPenalty().ToString() + "%\n")
                        + Localizer.Format("#LOC_BV_Control_SpeedAtNight") + ": " + averageSpeedAtNight.ToString("F") + " m/s\n"
                        + Localizer.Format("#LOC_BV_Control_UsedThrust") + ": " + ((EngineController)this.moveController).MaxThrust.ToString("F") + " kN"
                }
            };
			this.displayedSystemCheckWidgets.Add(result);

            if (requiredPower > 0)
            {
                double speedReduction = SpeedReduction;
				result = new DisplayedSystemCheckWidget[] {
					new DisplayedSystemCheckWidget {
                        Label = Localizer.Format("#LOC_BV_Control_ElectricPower"),
						Text = this.requiredPower.ToString("F") + " / " + this.electricPower.ToString("F"),
                        Tooltip = Localizer.Format("#LOC_BV_Control_RequiredPower") + ": " + requiredPower.ToString("F")
                            + (speedReduction == 0 ? "" :
                                (1 != speedReduction
                                    ? " (" + Localizer.Format("#LOC_BV_Control_PowerReduced") + " " + speedReduction.ToString("P")
                                    : " (" + Localizer.Format("#LOC_BV_Control_NotEnoughPower") + ")")) + "\n"
                            + Localizer.Format("#LOC_BV_Control_SolarPower") + ": " + electricPower_Solar.ToString("F") + "\n" + Localizer.Format("#LOC_BV_Control_GeneratorPower") + ": " + electricPower_Other.ToString("F")
                    }
                };
				this.displayedSystemCheckWidgets.Add(result);
            }

			result = new DisplayedSystemCheckWidget[] {
				new DisplayedSystemCheckWidget
                {
                    Label = Localizer.Format("#LOC_BV_Control_Throttle"),
                    Text = "",
                    Tooltip = Localizer.Format("#LOC_BV_Control_Throttle_Tooltip")
                }
            };
			this.displayedSystemCheckWidgets.Add(result);

			result = new DisplayedSystemCheckWidget[] {
				new DisplayedSystemCheckToggleResult {
                    Text = "100%",
                    Tooltip = "",
					GetValue = GetThrottle100,
					SelectedCallback = UseThrottle100
                },
				new DisplayedSystemCheckToggleResult {
                    Text = "75%",
                    Tooltip = "",
					GetValue = GetThrottle75,
					SelectedCallback = UseThrottle75
                },
				new DisplayedSystemCheckToggleResult {
                    Text = "50%",
                    Tooltip = "",
					GetValue = GetThrottle50,
					SelectedCallback = UseThrottle50
                }
            };
			this.displayedSystemCheckWidgets.Add(result);

			result = new DisplayedSystemCheckWidget[] {
				new DisplayedSystemCheckToggleResult {
                    Text = "25%",
                    Tooltip = "",
					GetValue = GetThrottle25,
					SelectedCallback = UseThrottle25
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
            return FindRoute(lat, lon, TileTypes.Ocean);
        }

        #endregion


        /// <summary>
        /// Check the systems
        /// </summary>
        internal override void SystemCheck()
        {
            base.SystemCheck();

            // Manned
            manned = (vessel.GetCrewCount() > 0);

            // Pilots and Scouts (USI) increase base average speed
            crewSpeedBonus = 0;
            if (manned)
            {
                int maxPilotLevel = -1;
                int maxScoutLevel = -1;
                int maxDriverLevel = -1;

                List<ProtoCrewMember> crewList = vessel.GetVesselCrew();
                for (int i = 0; i < crewList.Count; ++i)
                {
                    switch (crewList[i].trait)
                    {
                        case "Pilot":
                            if (maxPilotLevel < crewList[i].experienceLevel)
                                maxPilotLevel = crewList[i].experienceLevel;
                            break;
                        case "Scout":
                            if (maxScoutLevel < crewList[i].experienceLevel)
                                maxScoutLevel = crewList[i].experienceLevel;
                            break;
                        default:
                            if (crewList[i].HasEffect("AutopilotSkill"))
                                if (maxDriverLevel < crewList[i].experienceLevel)
                                    maxDriverLevel = crewList[i].experienceLevel;
                            break;
                    }
                }
                if (maxPilotLevel > 0)
                    crewSpeedBonus = 6 * maxPilotLevel; // up to 30% for a Pilot
                else if (maxDriverLevel > 0)
                    crewSpeedBonus = 4 * maxDriverLevel; // up to 20% for any driver (has AutopilotSkill skill)
                else if (maxScoutLevel > 0)
                    crewSpeedBonus = 2 * maxScoutLevel; // up to 10% for a Scout (Scouts disregard safety)
            }

			{
				double throttleCap = this.throttle;

				if (this.crewSpeedBonus > 0)
					throttleCap += this.crewSpeedBonus;
				if (!manned)    // Unmanned rovers drive with the speed penalty based on available tech
					throttleCap -= this.GetUnmannedSpeedPenalty();

				this.moveController.Check(throttleCap);
				double dragCap;
				{
					// Compute max speed - based on the drag equation
					double maxThrust = (((EngineController)this.moveController).MaxThrust / thrust0);
					double Z = (density0 / (0.5 * vessel.mainBody.atmDensityASL + 0.5 * vessel.mainBody.oceanDensity)) * (mass0 / vessel.GetTotalMass()) * maxThrust;
					double maxSpeedBase = Math.Sqrt(Z) * speed0;
    				Log.dbg("Compute max speed: speed0={0}, maxThrust={1}, Z={2}, maxSpeedBase={3}", speed0, maxThrust, Z, maxSpeedBase);
					dragCap = maxSpeedBase/speed0;
					if (maxSpeedBase > speed0) // We are over max allowed speed, then we need to decrease max available thrust
						dragCap = (speed0 / maxSpeedBase);
				}

				// Throttle
				this.requiredPower = ((EngineController)this.moveController).PowerRequired * throttleCap;

				Log.dbg("throttleCap: {0}; dragCap:{1}; requiredPower:{2}", throttleCap, dragCap, requiredPower);

				// If required power is greater then total power generated, then average speed can be lowered up to 87% (square root of (1 - powerReduction))
				if (this.requiredPower > this.electricPower)
				{
					double powerReduction = (this.requiredPower - this.electricPower) / this.requiredPower;
					if (powerReduction <= 0.75)
						throttleCap *= (1 - powerReduction);
				}

				this.moveController.Check(throttleCap*dragCap); // FIXME: We already did a full Check above. Would be nice to reuse that calculationts, uh?
				this.fuelEnergy.Check(throttleCap);
			}

            // Cheats
            if (CheatOptions.InfiniteElectricity)
                electricPower_Other = requiredPower;

            // Base average speed at night is the same as average speed, if there is other power source. Zero otherwise.
            if (electricPower_Other > 0.0)
                averageSpeedAtNight = this.moveController.averageSpeed;
            else
                averageSpeedAtNight = 0;

            // If required power is greater then other power generated, then average speed at night can be lowered up to 87% (square root of (1 - powerReduction))
            if (requiredPower > electricPower_Other)
            {
                double powerReduction = (requiredPower - electricPower_Other) / requiredPower;
                if (powerReduction <= 0.75)
                    averageSpeedAtNight = averageSpeedAtNight * Math.Sqrt(1 - powerReduction);
                else
                    averageSpeedAtNight = 0;
            }
        }

        /// <summary>
        /// Activate autopilot
        /// </summary>
        internal override bool Activate()
        {
            if (vessel.situation != Vessel.Situations.SPLASHED)
            {
                ScreenMessages.PostScreenMessage(Localizer.Format("#LOC_BV_Warning_Splashed"), 5f).color = CommonWindowProperties.Message_Colour_Warning;
                return false;
            }

            SystemCheck();
            
            // At least one engine must be on
            if (0 == ((EngineController)this.moveController).MaxThrust)
            {
                ScreenMessages.PostScreenMessage(Localizer.Format("#LOC_BV_Warning_EnginesNotOnline"), 5f).color = CommonWindowProperties.Message_Colour_Warning_User_Error;
                return false;
            }

            // Get fuel amount
			if (!this.fuelEnergy.CheckResources(this.resourceBroker))
            {
                ScreenMessages.PostScreenMessage(Localizer.Format("#LOC_BV_Warning_NotEnoughFuel"), 5f).color = CommonWindowProperties.Message_Colour_Warning_User_Error;
                return false;
            }

            // Power production
			if (this.requiredPower > this.electricPower)
            {
                // If required power is greater than total power generated, then average speed can be lowered up to 87% (square root of (1 - powerReduction))
				double powerReduction = (this.requiredPower - this.electricPower) / this.requiredPower;

                if (powerReduction > 0.75)
                {
                    ScreenMessages.PostScreenMessage(Localizer.Format("#LOC_BV_Warning_LowPowerShip"), 5f).color = CommonWindowProperties.Message_Colour_Warning_User_Error;
                    return false;
                }
            }

            BonVoyageModule module = vessel.FindPartModuleImplementing<BonVoyageModule>();
            if (module != null)
            {
                module.averageSpeed = this.moveController.averageSpeed;
                module.averageSpeedAtNight = averageSpeedAtNight;
                module.manned = manned;
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
        /// Update Ship
        /// </summary>
        /// <param name="currentTime"></param>
        protected override void update(double currentTime)
        {
            // Speed penalties at twighlight and at night
            if ((angle > 90) && manned) // night
                speedMultiplier = 0.25;
            else if ((angle > 85) && manned) // twilight
                speedMultiplier = 0.5;
            else if ((angle > 80) && manned) // twilight
                speedMultiplier = 0.75;
            else // day
                speedMultiplier = 1.0;

            double deltaT = currentTime - lastTimeUpdated; // Time delta from the last update
            double deltaTOver = 0; // deltaT which is calculated from a value over the maximum propellant amout available

            // No moving at night, if there isn't enough power
            if ((angle > 90) && (averageSpeedAtNight == 0.0))
            {
                State = VesselState.AwaitingSunlight;
                lastTimeUpdated = currentTime;
                BVModule.SetValue("lastTimeUpdated", currentTime.ToString());
                return;
            }

			this.fuelEnergy.Update(ref deltaT, ref deltaTOver);

			double deltaS = this.CalcAverageSpeed() * deltaT; // Distance delta from the last update
            Log.dbg("ShipController.update deltaS = {0}; deltaT={1}", deltaS, deltaT);
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

            // Stop the ship, we don't have enough of propellant
            if (deltaTOver > 0)
            {
                active = false;
				this.arrived = false;
                BVModule.SetValue("active", "False");
				this.BVModule.SetValue("arrived", "False");

				Tools.OptionalDewarpAndDo(
						() => ScreenMessages.PostScreenMessage(Tools.BuildVesselMessage(vessel, "#LOC_BV_Warning_Stopped"), 5f).color = CommonWindowProperties.Message_Colour_Warning_User_Error
					);

                NotifyNotEnoughFuel();
                State = VesselState.Idle;
            }
        }

        /// <summary>
        /// Notify, that rover has arrived
        /// </summary>
        private void NotifyArrival()
        {
            MessageSystem.Message message = new MessageSystem.Message(
                Localizer.Format("#LOC_BV_Title_ShipArrived"), // title
                Tools.BuildVesselMessage("#74B4E2", vessel, "#LOC_BV_VesselArrived") + ".\n<color=#AED6EE>"
                + Localizer.Format("#LOC_BV_Control_Lat") + ": " + targetLatitude.ToString("F2") + "</color>\n<color=#AED6EE>" + Localizer.Format("#LOC_BV_Control_Lon") + ": " + targetLongitude.ToString("F2") + "</color>", // message
                MessageSystemButton.MessageButtonColor.GREEN,
                MessageSystemButton.ButtonIcons.COMPLETE
            );
            MessageSystem.Instance.AddMessage(message);
        }


        /// <summary>
        /// Notify, that ship has not enough fuel
        /// </summary>
        private void NotifyNotEnoughFuel()
        {
            MessageSystem.Message message = new MessageSystem.Message(
                Localizer.Format("#LOC_BV_Title_ShipStopped"), // title
                "<color=#74B4E2>" + vessel.vesselName + "</color> " + Localizer.Format("#LOC_BV_Warning_Stopped") + ". " + Localizer.Format("#LOC_BV_Warning_NotEnoughFuel") + ".\n<color=#AED6EE>", // message
                MessageSystemButton.MessageButtonColor.RED,
                MessageSystemButton.ButtonIcons.ALERT
            );
            MessageSystem.Instance.AddMessage(message);
        }


        /// <summary>
        /// Return status of throttle
        /// </summary>
        /// <returns></returns>
        internal bool GetThrottle100()
        {
            return throttle == 100;
        }
        internal bool GetThrottle75()
        {
            return throttle == 75;
        }
        internal bool GetThrottle50()
        {
            return throttle == 50;
        }
        internal bool GetThrottle25()
        {
            return throttle == 25;
        }


        /// <summary>
        /// Set throttle level
        /// </summary>
        /// <param name="value"></param>
        internal void UseThrottle100(bool value)
        {
            throttle = 100;
        }
        internal void UseThrottle75(bool value)
        {
            throttle = 75;
        }
        internal void UseThrottle50(bool value)
        {
            throttle = 50;
        }
        internal void UseThrottle25(bool value)
        {
            throttle = 25;
        }

    }
}
