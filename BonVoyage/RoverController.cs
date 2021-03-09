using KSP.Localization;
using KSP.UI.Screens;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace BonVoyage
{
    /// <summary>
    /// Rover controller. Child of BVController
    /// </summary>
    internal class RoverController : BVController
    {
        #region internal properties

        internal override double AverageSpeed { get { return ((angle <= 90) || (batteries.UseBatteries && (batteries.CurrentEC > 0)) ? (averageSpeed * speedMultiplier) : (averageSpeedAtNight * speedMultiplier)); } }

        #endregion


        #region Private properties

        // Config values
        private double averageSpeed = 0;
        private double averageSpeedAtNight = 0;
        private bool manned = false;
        private double vesselHeightFromTerrain = 0;
        // Config values

        private double speedMultiplier;
        private double angle; // Angle between the main body and the main sun
        private double maxSpeedBase; // maximum speed without modifiers
        private int wheelsPercentualModifier; // Speed modifier based on wheels
        private int crewSpeedBonus; // Speed modifier based on the available crew
        WheelTestResult wheelTestResult = new WheelTestResult(); // Result of a test of wheels

        // Reduction of speed based on difference between required and available power in percents
        private double SpeedReduction
        {
            get
            {
                double speedReduction = 0;
                if (requiredPower > (electricPower_Solar + electricPower_Other))
                    speedReduction = (requiredPower - (electricPower_Solar + electricPower_Other)) / requiredPower * 100;
                return speedReduction;
            }
        }

        #endregion


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="v"></param>
        /// <param name="module"></param>
        internal RoverController(Vessel v, ConfigNode module) : base (v, module)
        {
            // Load values from config if it isn't the first run of the mod (we are reseting vessel on the first run)
            if (!Configuration.FirstRun)
            {
                averageSpeed = double.Parse(BVModule.GetValue("averageSpeed") != null ? BVModule.GetValue("averageSpeed") : "0");
                averageSpeedAtNight = double.Parse(BVModule.GetValue("averageSpeedAtNight") != null ? BVModule.GetValue("averageSpeedAtNight") : "0");
                manned = bool.Parse(BVModule.GetValue("manned") != null ? BVModule.GetValue("manned") : "false");
                vesselHeightFromTerrain = double.Parse(BVModule.GetValue("vesselHeightFromTerrain") != null ? BVModule.GetValue("vesselHeightFromTerrain") : "0");
            }

            speedMultiplier = 1.0;
            angle = 0;
            maxSpeedBase = 0;
            wheelsPercentualModifier = 0;
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

        internal override List<DisplayedSystemCheckResult[]> GetDisplayedSystemCheckResults()
        {
            base.GetDisplayedSystemCheckResults();

            DisplayedSystemCheckResult[] result = new DisplayedSystemCheckResult[] {
                new DisplayedSystemCheckResult {
                    Toggle = false,
                    Label = Localizer.Format("#LOC_BV_Control_AverageSpeed"),
                    Text = averageSpeed.ToString("F") + " m/s",
                    Tooltip =
                        averageSpeed > 0
                        ?
                        Localizer.Format("#LOC_BV_Control_SpeedBase") + ": " + maxSpeedBase.ToString("F") + " m/s\n"
                            + Localizer.Format("#LOC_BV_Control_WheelsModifier") + ": " + wheelsPercentualModifier.ToString("F") + "%\n"
                            + (manned ? Localizer.Format("#LOC_BV_Control_DriverBonus") + ": " + crewSpeedBonus.ToString() + "%\n" : Localizer.Format("#LOC_BV_Control_UnmannedPenalty") + ": " + GetUnmannedSpeedPenalty().ToString() + "%\n")
                            + (SpeedReduction > 0 ? Localizer.Format("#LOC_BV_Control_PowerPenalty") + ": " + (SpeedReduction > 75 ? "100" : SpeedReduction.ToString("F")) + "%\n" : "")
                            + Localizer.Format("#LOC_BV_Control_SpeedAtNight") + ": " + averageSpeedAtNight.ToString("F") + " m/s"
                        :
                        Localizer.Format("#LOC_BV_Control_WheelsNotOnline")
                }
            };
            displayedSystemCheckResults.Add(result);

            result = new DisplayedSystemCheckResult[] {
                new DisplayedSystemCheckResult {
                    Toggle = false,
                    Label = Localizer.Format("#LOC_BV_Control_GeneratedPower"),
                    Text = (electricPower_Solar + electricPower_Other).ToString("F"),
                    Tooltip = Localizer.Format("#LOC_BV_Control_SolarPower") + ": " + electricPower_Solar.ToString("F") + "\n" + Localizer.Format("#LOC_BV_Control_GeneratorPower") + ": " + electricPower_Other.ToString("F") + "\n"
                        + Localizer.Format("#LOC_BV_Control_UseBatteries_Usage") + ": " + (batteries.UseBatteries ? (batteries.MaxUsedEC.ToString("F0") + " / " + batteries.MaxAvailableEC.ToString("F0") + " EC") : Localizer.Format("#LOC_BV_Control_No"))
                }
            };
            displayedSystemCheckResults.Add(result);

            double speedReduction = SpeedReduction;
            result = new DisplayedSystemCheckResult[] {
                new DisplayedSystemCheckResult {
                    Toggle = false,
                    Label = Localizer.Format("#LOC_BV_Control_RequiredPower"),
                    Text = requiredPower.ToString("F")
                        + (speedReduction == 0 ? "" :
                            (((speedReduction > 0) && (speedReduction <= 75))
                                ? " (" + Localizer.Format("#LOC_BV_Control_PowerReduced") + " " + speedReduction.ToString("F") + "%)"
                                : " (" + Localizer.Format("#LOC_BV_Control_NotEnoughPower") + ")")),
                    Tooltip = ""
                }
            };
            displayedSystemCheckResults.Add(result);

            result = new DisplayedSystemCheckResult[] {
                new DisplayedSystemCheckResult {
                    Toggle = true,
                    Text = Localizer.Format("#LOC_BV_Control_UseBatteries"),
                    Tooltip = Localizer.Format("#LOC_BV_Control_UseBatteries_Tooltip"),
                    GetToggleValue = GetUseBatteries,
                    ToggleSelectedCallback = UseBatteriesChanged
                }
            };
            displayedSystemCheckResults.Add(result);

            result = new DisplayedSystemCheckResult[] {
                new DisplayedSystemCheckResult {
                    Toggle = true,
                    Text = Localizer.Format("#LOC_BV_Control_UseFuelCells"),
                    Tooltip = Localizer.Format("#LOC_BV_Control_UseFuelCellsTooltip"),
                    GetToggleValue = GetUseFuelCells,
                    ToggleSelectedCallback = UseFuelCellsChanged
                }
            };
            displayedSystemCheckResults.Add(result);

            return displayedSystemCheckResults;
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

            // Test stock wheels
            WheelTestResult testResultStockWheels = CheckStockWheels();
            // Test KSPWheels
            WheelTestResult testResultKSPkWheels = CheckKSPWheels();
            // Sum it
            wheelTestResult.powerRequired = testResultStockWheels.powerRequired + testResultKSPkWheels.powerRequired;
            wheelTestResult.maxSpeedSum = testResultStockWheels.maxSpeedSum + testResultKSPkWheels.maxSpeedSum;
            wheelTestResult.inTheAir = testResultStockWheels.inTheAir + testResultKSPkWheels.inTheAir;
            wheelTestResult.operable = testResultStockWheels.operable + testResultKSPkWheels.operable;
            wheelTestResult.damaged = testResultStockWheels.damaged + testResultKSPkWheels.damaged;
            wheelTestResult.online = testResultStockWheels.online + testResultKSPkWheels.online;
            wheelTestResult.maxWheelRadius = testResultStockWheels.maxWheelRadius + testResultKSPkWheels.maxWheelRadius;

            // Generally, moving at high speed requires less power than wheels' max consumption. Maximum required power of controller will 35% of wheels power requirement 
            requiredPower = wheelTestResult.powerRequired / 100 * 35;

            // Get available EC from batteries
            if (batteries.UseBatteries)
                batteries.MaxAvailableEC = GetAvailableEC_Batteries();
            else
                batteries.MaxAvailableEC = 0;

            // Get available EC from fuell cells
            fuelCells.OutputValue = 0;
            fuelCells.InputResources.Clear();
            if (fuelCells.Use)
            {
                List<ModuleResourceConverter> mrc = vessel.FindPartModulesImplementing<ModuleResourceConverter>();
                for (int i = 0; i < mrc.Count; i++)
                {
                    double ecRatio = 0;
                    try
                    {
                        var ec = mrc[i].outputList.Find(x => x.ResourceName == "ElectricCharge"); // NullArgumentException when not found
                        ecRatio = ec.Ratio;
                    }
                    catch { }

                    if (ecRatio > 0)
                    {
                        // Add input resources
                        var iList = mrc[i].inputList;
                        for (int r = 0; r < iList.Count; r++)
                        {
                            // Check if we have fuel for converter. If not, then continue without adding output ratio.
                            if (!CheatOptions.InfinitePropellant && r == 0)
							{
                                IResourceBroker broker = new ResourceBroker();
                                if (broker.AmountAvailable(vessel.rootPart, iList[r].ResourceName, 1, ResourceFlowMode.ALL_VESSEL) == 0)
                                    break;
                            }

                            var ir = fuelCells.InputResources.Find(x => x.Name == iList[r].ResourceName);
                            if (ir == null)
                            {
                                ir = new Resource();
                                ir.Name = iList[r].ResourceName;
                                fuelCells.InputResources.Add(ir);
                            }
                            ir.Ratio += iList[r].Ratio;

                            // Add EC ration to output
                            if (r == 0)
                                fuelCells.OutputValue += ecRatio;
                        }
                    }
                }
            }
            electricPower_Other += fuelCells.OutputValue;

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
                for (int i = 0; i < crewList.Count; i++)
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

            // Average speed will vary depending on number of wheels online and crew present from 50 to 95 percent of average wheels' max speed
            if (wheelTestResult.online != 0)
            {
                maxSpeedBase = wheelTestResult.maxSpeedSum / wheelTestResult.online;
                wheelsPercentualModifier = Math.Min(70, (40 + 5 * wheelTestResult.online));
                averageSpeed = maxSpeedBase * Convert.ToDouble(wheelsPercentualModifier) / 100 * (1 + Convert.ToDouble(crewSpeedBonus) / 100);
            }
            else
                averageSpeed = 0;

            // Unmanned rovers drive with the speed penalty based on available tech
            if (!manned)
                averageSpeed = averageSpeed * (100 - Convert.ToDouble(GetUnmannedSpeedPenalty())) / 100;

            // Base average speed at night is the same as average speed, if there is other power source. Zero otherwise.
            if (electricPower_Other > 0.0)
                averageSpeedAtNight = averageSpeed;
            else
                averageSpeedAtNight = 0;

            // If required power is greater then total power generated, then average speed can be lowered up to 75%
            if (requiredPower > (electricPower_Solar + electricPower_Other))
            {
                double speedReduction = (requiredPower - (electricPower_Solar + electricPower_Other)) / requiredPower;
                if (speedReduction <= 0.75)
                    averageSpeed = averageSpeed * (1 - speedReduction);
            }

            // If required power is greater then other power generated, then average speed at night can be lowered up to 75%
            if (requiredPower > electricPower_Other)
            {
                double speedReduction = (requiredPower - electricPower_Other) / requiredPower;
                if (speedReduction <= 0.75)
                    averageSpeedAtNight = averageSpeedAtNight * (1 - speedReduction);
                else
                    averageSpeedAtNight = 0;
            }

            // If we are using batteries, compute for how long and how much EC we can use
            if (batteries.UseBatteries)
            {
                batteries.MaxUsedEC = 0;
                batteries.ECPerSecondConsumed = 0;
                batteries.ECPerSecondGenerated = 0;

                // We have enough of solar power to recharge batteries
                if (requiredPower < (electricPower_Solar + electricPower_Other))
                {
                    batteries.ECPerSecondConsumed = Math.Max(requiredPower - electricPower_Other, 0); // If there is more other power than required power, we don't need batteries
                    batteries.MaxUsedEC = batteries.MaxAvailableEC / 2; // We are using only half of max available EC
                    if (batteries.ECPerSecondConsumed > 0)
                    {
                        double halfday = vessel.mainBody.rotationPeriod / 2; // in seconds
                        batteries.ECPerSecondGenerated = electricPower_Solar + electricPower_Other - requiredPower;
                        batteries.MaxUsedEC = Math.Min(batteries.MaxUsedEC, batteries.ECPerSecondConsumed * halfday); // get lesser value of MaxUsedEC and EC consumed per night
                        batteries.MaxUsedEC = Math.Min(batteries.MaxUsedEC, batteries.ECPerSecondGenerated * halfday); // get lesser value of MaxUsedEC and max EC available for recharge during a day
                    }
                }

                if (batteries.MaxUsedEC > 0)
                    batteries.CurrentEC = batteries.MaxUsedEC; // We are starting at full available capacity
                else
                {
                    UseBatteriesChanged(false);
                    ScreenMessages.PostScreenMessage(Localizer.Format("#LOC_BV_Warning_CantUseBatteries") + " " + Localizer.Format("#LOC_BV_Warning_LowPowerRover") + ".", 5f).color = Color.yellow;
                }
            }
        }


        #region Wheels

        /// <summary>
        /// Result of the test of wheels
        /// </summary>
        private struct WheelTestResult
        {
            internal double powerRequired; // Total power required
            internal double maxSpeedSum; // Sum of max speeds of all online wheels
            internal int inTheAir; // Count of wheels in the air
            internal int operable; // Count of operable wheels
            internal int damaged; // Count of damaged wheels
            internal int online; // Count of online wheels
            internal float maxWheelRadius; // Maximum of radii of all aplicable wheels

            internal WheelTestResult(double powerRequired, double maxSpeedSum, int inTheAir, int operable, int damaged, int online, float maxWheelRadius)
            {
                this.powerRequired = powerRequired;
                this.maxSpeedSum = maxSpeedSum;
                this.inTheAir = inTheAir;
                this.operable = operable;
                this.damaged = damaged;
                this.online = online;
                this.maxWheelRadius = maxWheelRadius;
            }
        }


        /// <summary>
        /// Test stock wheels implementing standard module ModuleWheelBase
        /// </summary>
        /// <returns></returns>
        private WheelTestResult CheckStockWheels()
        {
            double powerRequired = 0;
            double maxSpeedSum = 0;
            int inTheAir = 0;
            int operable = 0;
            int damaged = 0;
            int online = 0;
            float maxWheelRadius = 0;

            // Get wheel modules
            List<Part> wheels = new List<Part>();
            for (int i = 0; i < vessel.parts.Count; i++)
            {
                var part = vessel.parts[i];
                if (part.Modules.Contains("ModuleWheelBase"))
                    wheels.Add(part);
            }

            for (int i = 0; i < wheels.Count; i++)
            {
                ModuleWheelBase wheelBase = wheels[i].FindModuleImplementing<ModuleWheelBase>();

                // Skip legs
                if (wheelBase.wheelType == WheelType.LEG)
                    continue;

                // Save max wheel radius for height compensations
                if (wheelBase.radius < maxWheelRadius)
                    maxWheelRadius = wheelBase.radius;

                // Check damaged wheels
                ModuleWheels.ModuleWheelDamage wheelDamage = wheels[i].FindModuleImplementing<ModuleWheels.ModuleWheelDamage>();
                if (wheelDamage != null)
                {
                    if (wheelDamage.isDamaged)
                    {
                        damaged++;
                        continue;
                    }
                }

                // Whether or not wheel is touching the ground
                if (!wheelBase.isGrounded)
                {
                    inTheAir++;
                    continue;
                }
                else
                    operable++;

                // Check motorized wheels
                ModuleWheels.ModuleWheelMotor wheelMotor = wheels[i].FindModuleImplementing<ModuleWheels.ModuleWheelMotor>();
                if (wheelMotor != null)
                {
                    // Wheel is on
                    if (wheelMotor.motorEnabled)
                    {
                        powerRequired += wheelMotor.avgResRate;
                        online++;
                        maxSpeedSum += wheelMotor.GetMaxSpeed();
                    }
                }
            }

            return new WheelTestResult(powerRequired, maxSpeedSum, inTheAir, operable, damaged, online, maxWheelRadius);
        }


        /// <summary>
        /// Test KSPWheels implementing module KSPWheelBase
        /// </summary>
        /// <returns></returns>
        private WheelTestResult CheckKSPWheels()
        {
            double powerRequired = 0;
            double maxSpeedSum = 0;
            int inTheAir = 0;
            int operable = 0;
            int damaged = 0;
            int online = 0;
            float maxWheelRadius = 0;

            // Get wheel modules
            List<Part> wheels = new List<Part>();
            for (int i = 0; i < vessel.parts.Count; i++)
            {
                var part = vessel.parts[i];
                if (part.Modules.Contains("KSPWheelBase"))
                    wheels.Add(part);
            }

            for (int i = 0; i < wheels.Count; i++)
            {
                List<PartModule> partModules = wheels[i].Modules.GetModules<PartModule>();
                PartModule wheelBase = partModules.Find(t => t.moduleName == "KSPWheelBase");

                // Save max wheel radius for height compensations
                float radius = (float)wheelBase.Fields.GetValue("wheelRadius");
                if (radius < maxWheelRadius)
                    maxWheelRadius = radius;

                // Check damaged wheels
                if (wheelBase.Fields.GetValue("persistentState").ToString() == "BROKEN")
                {
                    damaged++;
                    continue;
                }

                // Whether or not wheel is touching the ground
                PartModule wheelDamage = partModules.Find(t => t.moduleName == "KSPWheelDamage");
                float maxSafeSpeed = 0f; // We compare this value later with maxDrivenSpeed to eliminate unreal gear ratios
                if (wheelDamage != null)
                {
                    operable++;
                    maxSafeSpeed = (float)wheelDamage.Fields.GetValue("maxSafeSpeed");
                }

                // Check motorized wheels
                List<PartModule> wheelMotors = partModules.FindAll(t => t.moduleName == "KSPWheelMotor");
                for (int m = 0; m < wheelMotors.Count; m++)
                {
                    // Wheel is on
                    if (!(bool)wheelMotors[m].Fields.GetValue("motorLocked"))
                    {
                        online++;
                        powerRequired += (float)wheelMotors[m].Fields.GetValue("maxECDraw") * (float)wheelMotors[m].Fields.GetValue("motorOutput") / 100; // Motor output can be limited by a slider
                        if (maxSafeSpeed > 0)
                            maxSpeedSum += Math.Min((float)wheelMotors[m].Fields.GetValue("maxDrivenSpeed"), maxSafeSpeed);
                        else
                            maxSpeedSum += (float)wheelMotors[m].Fields.GetValue("maxDrivenSpeed");
                    }
                }

                // Check tracks
                PartModule wheelTracks = partModules.Find(t => t.moduleName == "KSPWheelTracks");
                if (wheelTracks != null)
                {
                    operable++; // We will count them as two wheels, so add another operable wheel
                    if (!(bool)wheelTracks.Fields.GetValue("motorLocked"))
                    {
                        online += 2;
                        powerRequired += (float)wheelTracks.Fields.GetValue("maxECDraw") * (float)wheelTracks.Fields.GetValue("motorOutput") / 100; // Motor output can be limited by a slider
                        if (maxSafeSpeed > 0)
                            maxSpeedSum += 2 * Math.Min((float)wheelTracks.Fields.GetValue("maxDrivenSpeed"), maxSafeSpeed);
                        else
                            maxSpeedSum += 2 * (float)wheelTracks.Fields.GetValue("maxDrivenSpeed");
                    }
                }
                else // Special cases
                {
                    if (wheels[i].name.StartsWith("critterCrawler")) // Critter crawler has six legs with motors
                    {
                        damaged *= 6;
                        inTheAir *= 6;
                        operable *= 6;
                    }
                }
            }

            return new WheelTestResult(powerRequired, maxSpeedSum, inTheAir, operable, damaged, online, maxWheelRadius);
        }

        #endregion


        #region Power

        /// <summary>
        /// Get maximum available EC from batteries
        /// </summary>
        /// <returns></returns>
        private double GetAvailableEC_Batteries()
        {
            double maxEC = 0;

            for (int i = 0; i < vessel.parts.Count; ++i)
            {
                var part = vessel.parts[i];
                if (part.Resources.Contains("ElectricCharge") && part.Resources["ElectricCharge"].flowState)
                    maxEC += part.Resources["ElectricCharge"].maxAmount;
            }

            return maxEC;
        }

        #endregion


        /// <summary>
        /// Activate autopilot
        /// </summary>
        internal override bool Activate()
        {
            if (vessel.situation != Vessel.Situations.LANDED && vessel.situation != Vessel.Situations.PRELAUNCH)
            {
                ScreenMessages.PostScreenMessage(Localizer.Format("#LOC_BV_Warning_Landed"), 5f).color = Color.yellow;
                return false;
            }

            SystemCheck();

            // No driving until at least 3 operable wheels are touching the ground - tricycles are allowed
            if ((wheelTestResult.inTheAir > 0) && (wheelTestResult.operable < 3))
            {
                ScreenMessages.PostScreenMessage(Localizer.Format("#LOC_BV_Warning_WheelsNotTouching"), 5f).color = Color.yellow;
                return false;
            }
            if (wheelTestResult.operable < 3)
            {
                ScreenMessages.PostScreenMessage(Localizer.Format("#LOC_BV_Warning_WheelsNotOperable"), 5f).color = Color.yellow;
                return false;
            }

            // At least 2 wheels must be on
            if (wheelTestResult.online < 2)
            {
                ScreenMessages.PostScreenMessage(Localizer.Format("#LOC_BV_Warning_WheelsNotOnline"), 5f).color = Color.yellow;
                return false;
            }

            // Get fuel amount if fuel cells are used
            if (fuelCells.Use && !CheatOptions.InfinitePropellant)
            {
                IResourceBroker broker = new ResourceBroker();
                var iList = fuelCells.InputResources;
                for (int i = 0; i < iList.Count; i++)
                {
                    iList[i].MaximumAmountAvailable = broker.AmountAvailable(vessel.rootPart, iList[i].Name, 1, ResourceFlowMode.ALL_VESSEL);

                    if (iList[i].MaximumAmountAvailable == 0)
                    {
                        ScreenMessages.PostScreenMessage(Localizer.Format("#LOC_BV_Warning_NotEnoughFuel"), 5f).color = Color.yellow;
                        return false;
                    }
                }
            }

            // Power production
            if (requiredPower > (electricPower_Solar + electricPower_Other))
            {
                // If required power is greater than total power generated, then average speed can be lowered up to 75%
                double speedReduction = (requiredPower - (electricPower_Solar + electricPower_Other)) / requiredPower;

                if (speedReduction > 0.75)
                {
                    ScreenMessages.PostScreenMessage(Localizer.Format("#LOC_BV_Warning_LowPowerRover"), 5f).color = Color.yellow;
                    return false;
                }
            }

            BonVoyageModule module = vessel.FindPartModuleImplementing<BonVoyageModule>();
            if (module != null)
            {
                vesselHeightFromTerrain = vessel.radarAltitude;

                module.averageSpeed = averageSpeed;
                module.averageSpeedAtNight = averageSpeedAtNight;
                module.manned = manned;
                module.vesselHeightFromTerrain = vesselHeightFromTerrain;
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
        /// Update vessel
        /// </summary>
        /// <param name="currentTime"></param>
        internal override void Update(double currentTime)
        {
            if (vessel == null)
                return;
            if (vessel.isActiveVessel)
            {
                lastTimeUpdated = 0;
                if (active)
                    ScreenMessages.PostScreenMessage(Localizer.Format("#LOC_BV_AutopilotActive"), 10f).color = Color.red;
                return;
            }
            
            if (!active || vessel.loaded)
                return;

            // If we don't know the last time of update, then set it and wait for the next update cycle
            if (lastTimeUpdated == 0)
            {
                State = VesselState.Idle;
                lastTimeUpdated = currentTime;
                BVModule.SetValue("lastTimeUpdated", currentTime.ToString());
                return;
            }
            
            Vector3d roverPos = vessel.mainBody.position - vessel.GetWorldPos3D();
            Vector3d toMainStar = vessel.mainBody.position - FlightGlobals.Bodies[mainStarIndex].position;
            angle = Vector3d.Angle(roverPos, toMainStar); // Angle between rover and the main star

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
            double deltaTOver = 0; // deltaT which is calculated from a value over the maximum resource amout available

            // Compute increase or decrease in EC from the last update
            if (!CheatOptions.InfiniteElectricity && batteries.UseBatteries && !DetectKerbalism.Found())
            {
                // Process fuel cells before batteries
                if (!CheatOptions.InfinitePropellant 
                    && fuelCells.Use 
                    && ((angle > 90) 
                        || (batteries.ECPerSecondGenerated - fuelCells.OutputValue <= 0)
                        || (batteries.CurrentEC < batteries.MaxUsedEC))) // Night, not enough solar power or we need to recharge batteries
                {
                    if (!((angle > 90) && (batteries.CurrentEC == 0))) // Don't use fuel cells, if it's night and current EC of batteries is zero. This means, that there isn't enough power to recharge them and fuel is wasted.
                    {
                        var iList = fuelCells.InputResources;
                        for (int i = 0; i < iList.Count; i++)
                        {
                            iList[i].CurrentAmountUsed += iList[i].Ratio * deltaT;
                            if (iList[i].CurrentAmountUsed > iList[i].MaximumAmountAvailable)
                                deltaTOver = Math.Max(deltaTOver, (iList[i].CurrentAmountUsed - iList[i].MaximumAmountAvailable) / iList[i].Ratio);
                        }
                        if (deltaTOver > 0)
                        {
                            deltaT -= deltaTOver;
                            // Reduce the amount of used resources
                            for (int i = 0; i < iList.Count; i++)
                                iList[i].CurrentAmountUsed -= iList[i].Ratio * deltaTOver;
                        }
                    }
                }

                if (angle <= 90) // day
                    batteries.CurrentEC = Math.Min(batteries.CurrentEC + batteries.ECPerSecondGenerated * deltaT, batteries.MaxUsedEC);
                else // night
                    batteries.CurrentEC = Math.Max(batteries.CurrentEC - batteries.ECPerSecondConsumed * deltaT, 0);
            }

            // No moving at night, if there isn't enough power
            if ((angle > 90) && (averageSpeedAtNight == 0.0) && !(batteries.UseBatteries && (batteries.CurrentEC > 0)))
            {
                State = VesselState.AwaitingSunlight;
                lastTimeUpdated = currentTime;
                BVModule.SetValue("lastTimeUpdated", currentTime.ToString());
                return;
            }

            double deltaS = AverageSpeed * deltaT; // Distance delta from the last update
            distanceTravelled += deltaS;

            if (distanceTravelled >= distanceToTarget) // We reached the target
            {
                if (!MoveSafely(targetLatitude, targetLongitude))
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
                    if (!MoveSafely(newCoordinates[0], newCoordinates[1]))
                    {
                        distanceTravelled -= deltaS;
                        State = VesselState.Idle;
                    }
                    else
                        State = VesselState.Moving;
                }
                catch { }
            }

            Save(currentTime);

            // Stop the rover, we don't have enough of fuel
            if (deltaTOver > 0 || (!CheatOptions.InfiniteElectricity && batteries.UseBatteries && batteries.CurrentEC <= 0.1))
            {
                active = false;
                arrived = true;
                BVModule.SetValue("active", "False");
                BVModule.SetValue("arrived", "True");
                BVModule.SetValue("pathEncoded", "");

                // Dewarp
                if (Configuration.AutomaticDewarp)
                {
                    if (TimeWarp.CurrentRate > 3) // Instant drop to 50x warp
                        TimeWarp.SetRate(3, true);
                    if (TimeWarp.CurrentRate > 0) // Gradual drop out of warp
                        TimeWarp.SetRate(0, false);
                    ScreenMessages.PostScreenMessage(vessel.vesselName + " " + Localizer.Format("#LOC_BV_Warning_Stopped") + ".", 5f).color = Color.red;
                }

                if (!CheatOptions.InfiniteElectricity && batteries.UseBatteries && batteries.CurrentEC <= 0.1)
                    NotifyBatteryEmpty();
				else
	                NotifyNotEnoughFuel();

                State = VesselState.Idle;
            }
        }


        /// <summary>
        /// Save move of a rover. We need to prevent hitting an active vessel.
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <returns>true if rover was moved, false otherwise</returns>
        private bool MoveSafely(double latitude, double longitude)
        {
            double altitude = GeoUtils.TerrainHeightAt(latitude, longitude, vessel.mainBody);
            if (FlightGlobals.ActiveVessel != null)
            {
                Vector3d newPos = vessel.mainBody.GetWorldSurfacePosition(latitude, longitude, altitude);
                Vector3d actPos = FlightGlobals.ActiveVessel.GetWorldPos3D();
                double distance = Vector3d.Distance(newPos, actPos);
                if (distance <= 2400)
                    return false;
            }

            vessel.latitude = latitude;
            vessel.longitude = longitude;
            vessel.altitude = altitude + vesselHeightFromTerrain + Configuration.HeightOffset;

            return true;
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
        /// Return status of batteries usage
        /// </summary>
        /// <returns></returns>
        internal bool GetUseBatteries()
        {
            return batteries.UseBatteries;
        }


        /// <summary>
        /// Set batteries usage
        /// </summary>
        /// <param name="value"></param>
        internal void UseBatteriesChanged(bool value)
        {
            batteries.UseBatteries = value;
            if (!value)
                fuelCells.Use = false;
        }


        /// <summary>
        /// Return status of fuel cells usage
        /// </summary>
        /// <returns></returns>
        internal bool GetUseFuelCells()
        {
            return fuelCells.Use;
        }


        /// <summary>
        /// Set fuel cells usage
        /// </summary>
        /// <param name="value"></param>
        internal void UseFuelCellsChanged(bool value)
        {
            fuelCells.Use = value;
            if (value)
                batteries.UseBatteries = true;
        }

    }

}
