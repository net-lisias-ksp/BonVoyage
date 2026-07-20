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
using UnityEngine;

using KSP.Localization;
using KSP.UI.Screens;

using BonVoyage.PowerSources;
using BonVoyage.MovementControllers;

namespace BonVoyage
{
    /// <summary>
    /// Rover controller. Child of BVController
    /// </summary>
    internal class RoverController : BVController
    {
        #region internal properties

		internal override double CalcAverageSpeed() =>
				this.IsDay || this.batteries.PowerIsAvailable
					? (this.moveController.averageSpeed * this.speedMultiplier)
					: (this.averageSpeedAtNight * this.speedMultiplier)
			;

		#endregion


		#region Private properties
		private readonly Controller moveController;

        // Config values
        private double averageSpeedAtNight = 0;
		private string AverageSpeedAtNightAsText => (Double.IsNaN(this.averageSpeedAtNight) || Double.IsInfinity(this.averageSpeedAtNight) ? "---" : this.averageSpeedAtNight.ToString("F") + " m/s");
        private bool manned = false;
        // Config values

        private double speedMultiplier;
        private int crewSpeedBonus; // Speed modifier based on the available crew

		// Reduction of speed based on difference between required and available power in Ratio [0..1]
		private double SpeedReduction
		{
			get
			{
				double speedReduction = 0;
				if (this.batteries.AllowNoGeneratedPower)
				{
					double nightLength = this.vessel.mainBody.rotationPeriod / 2; // half a day in seconds;
					double timeToTarget = this.RemainingDistanceToTarget / this.CalcAverageSpeed();
					if (nightLength > timeToTarget)
					{
						double totalEc = this.batteries.ECPerSecondConsumed * nightLength; // Total energy consumed by a whole night driving.
						speedReduction = this.batteries.MaxUsedEC / totalEc;
					}
					// else Go full throttle, baby!
				}
				else if (this.requiredPower > this.electricPower)
				{   // If required power is greater than total power generated, then average speed can be lowered up to 75%
					speedReduction = (this.requiredPower - this.electricPower) / this.requiredPower;
					speedReduction = (speedReduction > 0.75) ? 1 : speedReduction;
				}
				Log.dbg("SpeedReduction : {0}", speedReduction);
				return speedReduction;
			}
		}

		#endregion


		internal static BVController Create(Vessel vessel, ConfigNode moduleConfigNode)
		{
			// TODO: To intantiate the proper PowerSources for the current Installation!
			Converter fuelCellPowerSource = new ElectricityPoweredConverter(vessel);
			SolarPower solarPowerSource = new StockSolarPower(vessel);
            Controller moveController = new WheelController(vessel, moduleConfigNode);

			return new RoverController(vessel, moduleConfigNode, fuelCellPowerSource, solarPowerSource, moveController);
		}
		protected RoverController(Vessel v, ConfigNode module, Converter fuelCellPowerSource, SolarPower solarPowerSource, Controller moveController) : base(v, module, fuelCellPowerSource, solarPowerSource)
        {
			this.moveController = moveController;

            // Load values from config if it isn't the first run of the mod (we are reseting vessel on the first run)
            if (!Configuration.FirstRun)
            {
				this.averageSpeedAtNight = double.Parse(BVModule.GetValue("averageSpeedAtNight") ?? "0");
				this.manned = bool.Parse(BVModule.GetValue("manned") ?? "false");
            }

            speedMultiplier = 1.0;
            crewSpeedBonus = 0;
        }


        /// <summary>
        /// Get controller type
        /// </summary>
        /// <returns></returns>
        internal override int GetControllerType()
        {
            return 0;
        }


        #region Status window texts

        internal override List<DisplayedSystemCheckWidget[]> GetDisplayedSystemCheckResults()
        {
            base.GetDisplayedSystemCheckResults();
			double speedReduction = SpeedReduction;

			DisplayedSystemCheckWidget[] result = new DisplayedSystemCheckWidget[] {
                new DisplayedSystemCheckWidget {
                    Label = Localizer.Format("#LOC_BV_Control_AverageSpeed"),
                    Text = this.moveController.AverageSpeedAsText,
                    Tooltip =
                        this.moveController.averageSpeed > 0
                        ?
						Localizer.Format("#LOC_BV_Control_SpeedBase") + ": " + this.moveController.MaxSpeedBaseAsText + "\n"
							+ Localizer.Format("#LOC_BV_Control_WheelsModifier") + ": " + ((WheelController)this.moveController).WheelsPercentualModifierAsText + "\n"
                            + (manned ? Localizer.Format("#LOC_BV_Control_DriverBonus") + ": " + crewSpeedBonus.ToString() + "%\n" : Localizer.Format("#LOC_BV_Control_UnmannedPenalty") + ": " + GetUnmannedSpeedPenalty().ToString() + "%\n")
                            + (speedReduction > 0 ? Localizer.Format("#LOC_BV_Control_PowerPenalty") + ": " + (100*speedReduction).ToString("F") + "%\n" : "")
                            + Localizer.Format("#LOC_BV_Control_SpeedAtNight") + ": " + this.AverageSpeedAtNightAsText
                        :
                        Localizer.Format("#LOC_BV_Control_WheelsNotOnline")
                }
            };
			this.displayedSystemCheckWidgets.Add(result);

			result = new DisplayedSystemCheckWidget[] {
                new DisplayedSystemCheckWidget {
                    Label = Localizer.Format("#LOC_BV_Control_GeneratedPower"),
					Text = this.electricPower.ToString("F"),
                    Tooltip = Localizer.Format("#LOC_BV_Control_SolarPower") + ": " + electricPower_Solar.ToString("F") + "\n" + Localizer.Format("#LOC_BV_Control_GeneratorPower") + ": " + electricPower_Other.ToString("F") + "\n"
						+ Localizer.Format("#LOC_BV_Control_UseBatteries_Usage") + ": " + (this.batteries.Use ? (this.batteries.MaxUsedEC.ToString("F0") + " / " + this.batteries.MaxAvailableEC.ToString("F0") + " EC") : Localizer.Format("#LOC_BV_Control_No")) + "\n"
						+ Localizer.Format("#LOC_BV_Control_UseSolarPanels_Usage") + ": " + (this.solarPower.Use ? Localizer.Format("#LOC_BV_Control_Yes") : Localizer.Format("#LOC_BV_Control_No"))
						+ (this.batteries.AllowNoGeneratedPower ? "\n" + Localizer.Format("#LOC_BV_Control_UseBatteriesOnly_Usage") : "")
                }
            };
			this.displayedSystemCheckWidgets.Add(result);

			result = new DisplayedSystemCheckWidget[] {
                new DisplayedSystemCheckWidget {
                    Label = Localizer.Format("#LOC_BV_Control_RequiredPower"),
                    Text = requiredPower.ToString("F")
                        + (speedReduction == 0 || this.batteries.AllowNoGeneratedPower ? "" :
                            (1 != speedReduction
                                ? " (" + Localizer.Format("#LOC_BV_Control_PowerReduced") + " " + (speedReduction).ToString("P")
                                : " (" + Localizer.Format("#LOC_BV_Control_NotEnoughPower") + ")")),
                    Tooltip = ""
                }
            };
			this.displayedSystemCheckWidgets.Add(result);

			result = new DisplayedSystemCheckWidget[] {
				new DisplayedSystemCheckToggleResult {
                    Text = Localizer.Format("#LOC_BV_Control_UseBatteries"),
                    Tooltip = Localizer.Format("#LOC_BV_Control_UseBatteries_Tooltip", this.batteries.UseableECRatioAsText),
					GetValue = GetUseBatteries,
					SelectedCallback = UseBatteriesChanged
                }
            };
			this.displayedSystemCheckWidgets.Add(result);

			result = new DisplayedSystemCheckWidget[] {
				new DisplayedSystemCheckToggleResult {
					Text = Localizer.Format("#LOC_BV_Control_UseBatteriesOnly"),
					Tooltip = Localizer.Format("#LOC_BV_Control_UseBatteriesOnly_Tooltip"),
					GetValue = GetUseBatteriesOnly,
					SelectedCallback = UseBatteriesOnlyChanged
				}
			};
			this.displayedSystemCheckWidgets.Add(result);

			result = new DisplayedSystemCheckWidget[] {
				new DisplayedSystemCheckPercentResult {
					Label = Localizer.Format("#LOC_BV_Control_UseableECRatio"),
					Tooltip = Localizer.Format("#LOC_BV_Control_UseableECRatio_Tooltip", this.batteries.UseableECRatioAsText),
					GetValue = GetUseableECRatio,
					SelectedCallback = UseUseableECRatioChanged
				}
			};
			this.displayedSystemCheckWidgets.Add(result);

			result = new DisplayedSystemCheckWidget[] {
				new DisplayedSystemCheckToggleResult {
					Text = Localizer.Format("#LOC_BV_Control_UseSolarPanels"),
					Tooltip = Localizer.Format("#LOC_BV_Control_UseSolarPanels_Tooltip"),
					GetValue = GetUseSolarPanels,
					SelectedCallback = UseSolarPanelsChanged
				}
			};
			this.displayedSystemCheckWidgets.Add(result);

			result = new DisplayedSystemCheckWidget[] {
				new DisplayedSystemCheckToggleResult {
                    Text = Localizer.Format("#LOC_BV_Control_UseFuelCells"),
                    Tooltip = Localizer.Format("#LOC_BV_Control_UseFuelCellsTooltip"),
					GetValue = GetUseFuelCells,
					SelectedCallback = UseFuelCellsChanged
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
            return FindRoute(lat, lon, TileTypes.Land);
        }

        #endregion


        /// <summary>
        /// Check the systems
        /// </summary>
        internal override void SystemCheck()
        {
            base.SystemCheck();

            // Generally, moving at high speed requires less power than wheels' max consumption. Maximum required power of controller will 35% of wheels power requirement 
            requiredPower = ((WheelController)this.moveController).PowerRequired / 100 * 35;

            // Get available EC from batteries
			if (this.batteries.Use)
                batteries.MaxAvailableEC = GetAvailableEC_Batteries();
            else
                batteries.MaxAvailableEC = 0;

            electricPower_Other += fuelEnergy.OutputValue;

            // Cheats
            if (CheatOptions.InfiniteElectricity)
                electricPower_Other = requiredPower;

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

            // If we are using batteries, compute for how long and how much EC we can use
			if (this.batteries.Use)
            {
                batteries.MaxUsedEC = 0;
                batteries.ECPerSecondConsumed = 0;
                batteries.ECPerSecondGenerated = 0;

                // We have enough of solar power to recharge batteries
				if (this.batteries.AllowNoGeneratedPower || this.requiredPower < this.electricPower)
                {
                    batteries.ECPerSecondConsumed = Math.Max(requiredPower - electricPower_Other, 0); // If there is more other power than required power, we don't need batteries
					this.batteries.MaxUsedEC = this.batteries.MaxAvailableEC * this.batteries.UseableECRatio; 
                    if (batteries.ECPerSecondConsumed > 0)
                    {
                        double halfday = vessel.mainBody.rotationPeriod / 2; // in seconds
						this.batteries.ECPerSecondGenerated = this.electricPower - this.requiredPower;
                        batteries.MaxUsedEC = Math.Min(batteries.MaxUsedEC, batteries.ECPerSecondConsumed * halfday); // get lesser value of MaxUsedEC and EC consumed per night
						if (!this.batteries.AllowNoGeneratedPower)
							batteries.MaxUsedEC = Math.Min(batteries.MaxUsedEC, batteries.ECPerSecondGenerated * halfday); // get lesser value of MaxUsedEC and max EC available for recharge during a day
                    }
                }

                if (batteries.MaxUsedEC > 0)
                    batteries.CurrentEC = batteries.MaxUsedEC; // We are starting at full available capacity
                else
                {
                    UseBatteriesChanged(false);
                    ScreenMessages.PostScreenMessage(Localizer.Format("#LOC_BV_Warning_CantUseBatteries") + " " + Localizer.Format("#LOC_BV_Warning_LowPowerRover") + ".", 5f).color = CommonWindowProperties.Message_Colour_Warning_User_Error;
                }
				this.calcCurrentSituation();
            }

			double speedReduction = this.SpeedReduction;
			{
				double throttleCap = 100;

				throttleCap += this.crewSpeedBonus;

				// Unmanned rovers drive with the speed penalty based on available tech
				if (!this.manned)
					throttleCap -= this.GetUnmannedSpeedPenalty();

				// If required power is greater then total power generated, then average speed can be lowered up to 75%
				if (this.requiredPower > this.electricPower)
				{
					throttleCap *= (1 - speedReduction);
				}

				// Average speed will vary depending on number of wheels online and crew present from 50 to 95 percent of average wheels' max speed
				this.moveController.Check(throttleCap);
				this.fuelEnergy.Check(throttleCap);
			}

			// Base average speed at night is the same as average speed, if there is other power source. Zero otherwise.
			this.averageSpeedAtNight = (this.batteries.AllowNoGeneratedPower || this.electricPower_Other > 0.0)
					? this.moveController.averageSpeed
					: 0
				;

			// If required power is greater then other power generated, then average speed at night can be lowered up to 75%
			this.averageSpeedAtNight *= (1 - speedReduction);
		}


        #region Power

        /// <summary>
        /// Get maximum available EC from batteries
        /// </summary>
        /// <returns></returns>
        private double GetAvailableEC_Batteries()
        {
            // RealBattery integration: if installed, look at StoredCharge on RealBattery-enabled
            // parts instead of plain ElectricCharge buffers, which on RB vessels are just a small
            // instantaneous cache (a few units) and would make BV think the rover has no
            // meaningful battery capacity at all.
            if (Tools.AssemblyIsLoaded("RealBattery"))
                return GetAvailableEC_RealBattery();

            double maxEC = 0;

            for (int i = 0; i < vessel.parts.Count; ++i)
            {
				Part part = vessel.parts[i];
                if (part.Resources.Contains("ElectricCharge") && part.Resources["ElectricCharge"].flowState)
                    maxEC += part.Resources["ElectricCharge"].maxAmount;
            }

            return maxEC;
        }

        /// <summary>
        /// RealBattery integration: sums StoredCharge.maxAmount (converted to EC-equivalent via
        /// RealBatterySupport.EcPerSc) of all non-disabled parts carrying the RealBattery module.
        /// Capacity is reported at nominal (undegraded) value on purpose: RealBattery's own
        /// wear/BatteryLife derating is left to RealBattery's optional Harmony bridge, which also
        /// credits wear for energy used this way. Keeps this integration's footprint here minimal.
        /// </summary>
        private double GetAvailableEC_RealBattery()
        {
            double maxSc = 0.0;
            for (int i = 0; i < vessel.parts.Count; ++i)
            {
                Part part = vessel.parts[i];
                if (!part.Modules.Contains("RealBattery"))
                    continue;

                PartModule rbModule = part.Modules["RealBattery"];
                BaseField disabledField = rbModule.Fields["BatteryDisabled"];
                if (disabledField != null && disabledField.GetValue(rbModule) is bool disabled && disabled)
                    continue;

                if (!part.Resources.Contains("StoredCharge"))
                    continue;

                maxSc += part.Resources["StoredCharge"].maxAmount;
            }

            return maxSc * RealBatterySupport.EcPerSc;
        }

		#endregion


		/// <summary>
		/// Activate autopilot
		/// </summary>
		internal override bool Activate()
        {
            if (vessel.situation != Vessel.Situations.LANDED && vessel.situation != Vessel.Situations.PRELAUNCH)
            {
                ScreenMessages.PostScreenMessage(Localizer.Format("#LOC_BV_Warning_Landed"), 5f).color = CommonWindowProperties.Message_Colour_Warning;
                return false;
            }

            SystemCheck();

			{
				WheelController wc = (WheelController)this.moveController;

				// No driving until at least 3 operable wheels are touching the ground - tricycles are allowed
				if ((wc.InTheAir > 0) && (wc.Operable < 3))
				{
					ScreenMessages.PostScreenMessage(Localizer.Format("#LOC_BV_Warning_WheelsNotTouching"), 5f).color = CommonWindowProperties.Message_Colour_Warning_User_Error;
					return false;
				}
				if (wc.Operable < 3)
				{
					ScreenMessages.PostScreenMessage(Localizer.Format("#LOC_BV_Warning_WheelsNotOperable"), 5f).color = CommonWindowProperties.Message_Colour_Warning_User_Error;
					return false;
				}

				// At least 2 wheels must be on
				if (wc.OnLine < 2)
				{
					ScreenMessages.PostScreenMessage(Localizer.Format("#LOC_BV_Warning_WheelsNotOnline"), 5f).color = CommonWindowProperties.Message_Colour_Warning_User_Error;
					return false;
				}
			}

			// Check fuel amount if fuel cells are used
			if (!this.fuelEnergy.CheckResources(this.resourceBroker))
			{
				ScreenMessages.PostScreenMessage(Localizer.Format("#LOC_BV_Warning_NotEnoughFuel"), 5f).color = CommonWindowProperties.Message_Colour_Warning_User_Error;
				return false;
			}

			// A SpeedReducion of 100% means we are kaput. No movement possible due no power available.
			if (1 == this.SpeedReduction)
			{
				ScreenMessages.PostScreenMessage(Localizer.Format("#LOC_BV_Warning_LowPowerRover"), 5f).color = CommonWindowProperties.Message_Colour_Warning;
				return false;
			}

            BonVoyageModule module = vessel.FindPartModuleImplementing<BonVoyageModule>();
            if (module != null)
            {
                module.averageSpeed = this.moveController.averageSpeed;
                module.averageSpeedAtNight = averageSpeedAtNight;
                module.manned = manned;
                module.vesselHeightFromTerrain = this.moveController.vesselHeightFromTerrain;
            }

			this.engageBrakesOrNot(true);

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
        /// Update Rover
        /// </summary>
        /// <param name="currentTime"></param>
        protected override void update(double currentTime)
        {
			this.calcCurrentSituation();

			// Speed penalties at twighlight and at night if not in Only-Battery mode
			this.speedMultiplier = 1.0; // day or Only-Battery mode.
			if (!this.batteries.AllowNoGeneratedPower)
            {
				if ((angle > 90) && manned) // night
					speedMultiplier = 0.25;
				else if ((angle > 85) && manned) // twilight
					speedMultiplier = 0.5;
				else if ((angle > 80) && manned) // twilight
					speedMultiplier = 0.75;
			}
            double deltaT = currentTime - lastTimeUpdated; // Time delta from the last update
            double deltaTOver = 0; // deltaT which is calculated from a value over the maximum resource amout available

            // Compute increase or decrease in EC from the last update
			if (!CheatOptions.InfiniteElectricity && this.batteries.Use && !DetectKerbalism.Found())
            {
                // Process fuel cells before batteries
                if (!CheatOptions.InfinitePropellant 
                    && fuelEnergy.Use 
                    && (this.IsNight
                        || (batteries.ECPerSecondGenerated - fuelEnergy.OutputValue <= 0)
                        || (batteries.CurrentEC < batteries.MaxUsedEC))) // Night, not enough solar power or we need to recharge batteries
                {
                    if (!(this.IsNight && (batteries.CurrentEC == 0))) // Don't use fuel cells, if it's night and current EC of batteries is zero. This means, that there isn't enough power to recharge them and fuel is wasted.
                        this.fuelEnergy.Update(ref deltaT, ref deltaTOver);
                }

				if (this.IsDay) // day
                    batteries.CurrentEC = Math.Min(batteries.CurrentEC + batteries.ECPerSecondGenerated * deltaT, batteries.MaxUsedEC);
                else // night
                    batteries.CurrentEC = Math.Max(batteries.CurrentEC - batteries.ECPerSecondConsumed * deltaT, 0);
            }

            // No moving at night, if there isn't enough power
			if (this.IsNight && (this.averageSpeedAtNight == 0.0) && !this.batteries.PowerIsAvailable)
            {
                State = VesselState.AwaitingSunlight;
                lastTimeUpdated = currentTime;
                BVModule.SetValue("lastTimeUpdated", currentTime.ToString());
                return;
            }

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

                    // Dewarp
                    if (Configuration.AutomaticDewarp)
                    {
                        if (TimeWarp.CurrentRate > 3) // Instant drop to 50x warp
                            TimeWarp.SetRate(3, true);
                        if (TimeWarp.CurrentRate > 0) // Gradual drop out of warp
                            TimeWarp.SetRate(0, false);
                        ScreenMessages.PostScreenMessage(vessel.vesselName + " " + Localizer.Format("#LOC_BV_VesselArrived") + " " + vessel.mainBody.bodyDisplayName.Replace("^N", ""), 5f);
                    }

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

            // Stop the rover, we don't have enough juice
			if (deltaTOver > 0 || (!CheatOptions.InfiniteElectricity && this.batteries.PowerIsExhausted))
            {
                active = false;
				this.arrived = false;
                BVModule.SetValue("active", "False");
				this.BVModule.SetValue("arrived", "False");

                // Dewarp
                if (Configuration.AutomaticDewarp)
                {
                    if (TimeWarp.CurrentRate > 3) // Instant drop to 50x warp
                        TimeWarp.SetRate(3, true);
                    if (TimeWarp.CurrentRate > 0) // Gradual drop out of warp
                        TimeWarp.SetRate(0, false);
                    ScreenMessages.PostScreenMessage(vessel.vesselName + " " + Localizer.Format("#LOC_BV_Warning_Stopped") + ".", 5f).color = CommonWindowProperties.Message_Colour_Warning;
                }

				if (!CheatOptions.InfiniteElectricity && this.batteries.PowerIsExhausted)
                    NotifyBatteryEmpty();
				else
	                NotifyNotEnoughFuel();

                State = (distanceTravelled < distanceToTarget) ? VesselState.AwaitingSunlight : VesselState.Idle;
            }
        }

        /// <summary>
        /// Notify, that rover has arrived
        /// </summary>
        private void NotifyArrival()
        {
            MessageSystem.Message message = new MessageSystem.Message(
                Localizer.Format("#LOC_BV_Title_RoverArrived"), // title
                "<color=#74B4E2>" + vessel.vesselName + "</color> " + Localizer.Format("#LOC_BV_VesselArrived") + " " + vessel.mainBody.bodyDisplayName.Replace("^N", "") + ".\n<color=#AED6EE>"
                + Localizer.Format("#LOC_BV_Control_Lat") + ": " + targetLatitude.ToString("F2") + "</color>\n<color=#AED6EE>" + Localizer.Format("#LOC_BV_Control_Lon") + ": " + targetLongitude.ToString("F2") + "</color>", // message
                MessageSystemButton.MessageButtonColor.GREEN,
                MessageSystemButton.ButtonIcons.COMPLETE
            );
            MessageSystem.Instance.AddMessage(message);
        }


        /// <summary>
        /// Notify, that rover has not enough fuel
        /// </summary>
        private void NotifyNotEnoughFuel()
        {
            MessageSystem.Message message = new MessageSystem.Message(
                Localizer.Format("#LOC_BV_Title_RoverStopped"), // title
                "<color=#74B4E2>" + vessel.vesselName + "</color> " + Localizer.Format("#LOC_BV_Warning_Stopped") + ". " + Localizer.Format("#LOC_BV_Warning_NotEnoughFuel") + ".\n<color=#AED6EE>", // message
                MessageSystemButton.MessageButtonColor.RED,
                MessageSystemButton.ButtonIcons.ALERT
            );
            MessageSystem.Instance.AddMessage(message);
        }


        /// <summary>
        /// Notify, that rover has empty batteries
        /// </summary>
        private void NotifyBatteryEmpty()
        {
            MessageSystem.Message message = new MessageSystem.Message(
                Localizer.Format("#LOC_BV_Title_RoverStopped"), // title
                "<color=#74B4E2>" + vessel.vesselName + "</color> " + Localizer.Format("#LOC_BV_Warning_Stopped") + ". " + Localizer.Format("#LOC_BV_Warning_LowPowerRover") + ".\n<color=#AED6EE>", // message
                MessageSystemButton.MessageButtonColor.RED,
                MessageSystemButton.ButtonIcons.ALERT
            );
            MessageSystem.Instance.AddMessage(message);
        }


		/// <summary>
		/// Return status of solar panels usage
		/// </summary>
		/// <returns></returns>
		internal bool GetUseSolarPanels() => this.solarPower.Use;


		/// <summary>
		/// Set solar panels usage
		/// </summary>
		/// <param name="value"></param>
		internal void UseSolarPanelsChanged(bool value)
		{
			this.solarPower.Use = value;
			if (value)
			{
				this.batteries.Use = true;
				this.batteries.AllowNoGeneratedPower = false;
			}
		}


        /// <summary>
        /// Return status of batteries usage
        /// </summary>
        /// <returns></returns>
        internal bool GetUseBatteries()
        {
            return batteries.Use;
        }


        /// <summary>
        /// Set batteries usage
        /// </summary>
        /// <param name="value"></param>
        internal void UseBatteriesChanged(bool value)
        {
			this.batteries.Use = value;
            if (!value)
            {
				this.batteries.AllowNoGeneratedPower = false;
				if (this.batteries.UseableECRatio < 0.01)
					this.batteries.UseableECRatio = 0.5;
                fuelEnergy.Use = false;
            }
        }


		/// <summary>
		/// Return how much of the battery is allowed to be used.
		/// </summary>
		/// <returns></returns>
		internal float GetUseableECRatio() => (float)(100d*this.batteries.UseableECRatio);


		/// <summary>
		/// Set how much of the battery is allowed to be used.
		/// </summary>
		/// <param name="value"></param>
		internal void UseUseableECRatioChanged(float value)
		{
			this.batteries.UseableECRatio = (double)(value/100f);
			this.batteries.Use = (value > 0);
			if (!this.batteries.Use)
				this.batteries.AllowNoGeneratedPower = false;
		}


		/// <summary>
		/// Return status of batteries only usage
		/// </summary>
		/// <returns></returns>
		internal bool GetUseBatteriesOnly() => this.batteries.AllowNoGeneratedPower;


		/// <summary>
		/// Set batteries only usage
		/// </summary>
		/// <param name="value"></param>
		internal void UseBatteriesOnlyChanged(bool value)
		{
			this.batteries.AllowNoGeneratedPower = value;
			if (value)
			{
				this.batteries.Use = true;
				if (this.batteries.UseableECRatio < 0.01)
					this.batteries.UseableECRatio = 0.5;
				this.fuelEnergy.Use = false;
				this.solarPower.Use = false;
			}
		}


		/// <summary>
		/// Return status of fuel cells usage
		/// </summary>
		/// <returns></returns>
		internal bool GetUseFuelCells()
        {
            return fuelEnergy.Use;
        }


        /// <summary>
        /// Set fuel cells usage
        /// </summary>
        /// <param name="value"></param>
        internal void UseFuelCellsChanged(bool value)
        {
            fuelEnergy.Use = value;
            if (value)
			{
				this.batteries.Use = true;
				this.batteries.AllowNoGeneratedPower = false;
			}
        }

		private void engageBrakesOrNot(bool v)
		{
			if (this.vessel == FlightGlobals.ActiveVessel && Configuration.AutoEngageBreaks)
				FlightGlobals.ActiveVessel.ActionGroups.SetGroup(KSPActionGroup.Brakes, v);
		}

		private void calcCurrentSituation()
		{
			if (this.vessel == FlightGlobals.ActiveVessel && this.manned)
				this.vessel.ActionGroups.SetGroup(KSPActionGroup.Light, this.IsNight);
		}

	}

}
