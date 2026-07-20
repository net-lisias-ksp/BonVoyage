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

using BonVoyage.PowerSources;


namespace BonVoyage
{
    /// <summary>
    /// Enum of vessel states
    /// </summary>
    internal enum VesselState
    {
        Idle = 0,
        ControllerDisabled = 1,
        Current = 2,
        Moving = 3,
        AwaitingSunlight = 4
    }


	internal class DisplayedSystemCheckWidget
	{
		internal string Label;
		internal string Text;
		internal string Tooltip;
	}

	internal class DisplayedSystemCheckToggleResult : DisplayedSystemCheckWidget
	{
		internal Func<bool> GetValue;
		internal Callback<bool> SelectedCallback;
	}

	internal class DisplayedSystemCheckPercentResult : DisplayedSystemCheckWidget
	{
		internal Func<float> GetValue;
		internal UnityEngine.Events.UnityAction<float> SelectedCallback;
	}

    /// <summary>
    /// Basic controller
    /// </summary>
    internal abstract class BVController
    {
        #region internal properties

        internal Vessel vessel; // Vessel containing BonVoyageModule
		protected Vector3d vesselPos;
		protected double angle; // Angle between the main body and the main sun
		protected readonly IResourceBroker resourceBroker = new ResourceBroker();

        internal bool IsNight => this.angle > 90;
        internal bool IsDay => this.angle <= 90;

        internal bool Shutdown
        {
            get { return shutdown; }
            set
            {
                shutdown = value;
                if (shutdown)
                    State = VesselState.ControllerDisabled;
                else
                    State = VesselState.Idle;
            }
        }

        internal bool Active {  get { return active; } }

        internal bool Arrived
        {
            get { return arrived; }
            set
            {
                arrived = value;
                BonVoyageModule module = vessel.FindPartModuleImplementing<BonVoyageModule>();
                if (module != null)
                    module.arrived = value;
            }
        }

        internal Vector3d RotationVector
        {
            get { return rotationVector; }
            set { rotationVector = value; }
        }

        internal double RemainingDistanceToTarget { get { return distanceToTarget - distanceTravelled; } }
		internal string RemainingDistanceToTargetAsText
		{
			get
			{
				double rddt = this.RemainingDistanceToTarget;
				if (double.IsNaN(rddt) || double.IsInfinity(rddt)) return "---";
				return Tools.ConvertDistanceToText(this.RemainingDistanceToTarget);
			}
		}

		internal virtual double CalcAverageSpeed() => 0;
		internal string AverageSpeedAsText
		{
			get
			{
				double avs = this.CalcAverageSpeed();
				if (double.IsNaN(avs) || double.IsInfinity(avs)) return "---";
				return avs.ToString("0.##") + " m/s";
			}
		}

		internal double EstimatedTimeOfArrival() => this.RemainingDistanceToTarget / this.CalcAverageSpeed();
		internal string EstimatedTimeOfArrivalAsText
		{
			get
			{
				double eta = this.EstimatedTimeOfArrival();
				if (double.IsNaN(eta) || double.IsInfinity(eta)) return "---";
				TimeSpan ts = TimeSpan.FromSeconds(eta);
				return ts.ToString(@"d\.hh\:mm\:ss");
			}
		}
		internal string EstimatedTimeOfArrivalAsLongText
		{
			get
			{
				double eta = this.EstimatedTimeOfArrival();
				if (double.IsNaN(eta) || double.IsInfinity(eta)) return "---";
				TimeSpan ts = TimeSpan.FromSeconds(eta);
				return Localizer.Format("#LOC_BV_Control_ETA_Tooltip", ts.Days, ts.Hours, ts.Minutes, ts.Seconds);
			}
		}

		internal event EventHandler OnStateChanged;

        internal double electricPower_Solar; // Electric power from solar panels
        internal double electricPower_Other; // Electric power from other power sources
		internal double electricPower => this.electricPower_Solar + this.electricPower_Other;
        internal double requiredPower; // Power required by wheels
        internal readonly Batteries batteries = new Batteries(); // Information about batteries
        internal readonly Converter fuelEnergy;      // Information about fuel cells or propellant engines.
		internal readonly PowerSupply solarPower;    // Information about solar panels

        #endregion


        #region Private and protected properties

        protected ConfigNode BVModule; // Config node of BonVoyageModule
		protected readonly List<DisplayedSystemCheckWidget[]> displayedSystemCheckWidgets = new List<DisplayedSystemCheckWidget[]>();
        protected int mainStarIndex; // Vessel's main star's index in the FlightGlobals.Bodies

        // Config values
        protected bool active = false;
        private bool shutdown = false;
        protected bool arrived = false;
        protected double targetLatitude = 0;
        protected double targetLongitude = 0;
        protected double distanceToTarget = 0;
        protected double distanceTravelled = 0;
        protected double lastTimeUpdated = 0;
        private Vector3d rotationVector = Vector3d.back; // Rotation of a craft
        // Config values

        internal List<PathUtils.WayPoint> path = null; // Path to destination

        private VesselState _state;
        internal VesselState State
        {
            get { return _state; }
            set
            {
                if (_state != value)
                {
                    _state = value;
                    if (OnStateChanged != null)
                        OnStateChanged(this, EventArgs.Empty);
                }
            }
        }

        #endregion


		/**
		 * Some technically challenged individual is monkey patching me, and since he did a lousy job on his Harmony Patch,
		 * it causes a failure that ended up being attributed to me.
		 * 
		 * This is my attempt to push back the blame where it belongs.
		 */
		protected BVController(Vessel v, ConfigNode module)
		{
			Log.err("This constructor is not supported, you really should not be seeing this. The stack dump below, hopefully, will help on diagnosing the problem.");
			this.vessel = v;
			this.BVModule = module;
			this.displayedSystemCheckWidgets.Clear();

			this.fuelEnergy = new Dummy.Converter(this.vessel);
			this.solarPower = new Dummy.PowerSupply();

			this.init();
		}

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="v"></param>
        /// <param name="module"></param>
        protected BVController(Vessel v, ConfigNode module, Converter fuelCellPowerSource, SolarPower solarPowerSource)
        {
            vessel = v;
            BVModule = module;

			this.fuelEnergy = fuelCellPowerSource;
			this.solarPower = solarPowerSource;

			this.init();
        }

		private void init()
		{
			this.angle = 0;
			this.displayedSystemCheckWidgets.Clear();

            // Load values from config if it isn't the first run of the mod (we are reseting vessel on the first run)
            if (!Configuration.FirstRun)
            {
				this.active = bool.Parse(this.BVModule.GetValue("active") ?? "false");
				this.shutdown = bool.Parse(this.BVModule.GetValue("shutdown") ?? "false");
				this.arrived = bool.Parse(this.BVModule.GetValue("arrived") ?? "false");
				this.targetLatitude = double.Parse(this.BVModule.GetValue("targetLatitude") ?? "0");
				this.targetLongitude = double.Parse(this.BVModule.GetValue("targetLongitude") ?? "0");
				this.distanceToTarget = double.Parse(this.BVModule.GetValue("distanceToTarget") ?? "0");
				this.distanceTravelled = double.Parse(this.BVModule.GetValue("distanceTravelled") ?? "0");

                if (BVModule.GetValue("pathEncoded") != null)
                    path = PathUtils.DecodePath(BVModule.GetValue("pathEncoded"));

                if (BVModule.GetValue("rotationVector") != null)
                {
                    switch (BVModule.GetValue("rotationVector"))
                    {
                        case "0":
                            rotationVector = Vector3d.up;
                            break;
                        case "1":
                            rotationVector = Vector3d.down;
                            break;
                        case "2":
                            rotationVector = Vector3d.forward;
                            break;
                        case "3":
                            rotationVector = Vector3d.back;
                            break;
                        case "4":
                            rotationVector = Vector3d.right;
                            break;
                        case "5":
                            rotationVector = Vector3d.left;
                            break;
                        default:
                            rotationVector = Vector3d.back;
                            break;
                    }
                }
                else
                    rotationVector = Vector3d.back;
            }

            State = VesselState.Idle;
            if (shutdown)
                State = VesselState.ControllerDisabled;

            lastTimeUpdated = 0;
            mainStarIndex = 0; // In the most cases The Sun
            electricPower_Solar = 0;
            electricPower_Other = 0;
            requiredPower = 0;
        }


        /// <summary>
        /// Get controller type
        /// </summary>
        /// <returns></returns>
        internal virtual int GetControllerType()
        {
            return -1;
        }


        #region Main window texts

        /// <summary>
        /// Get vessel state
        /// </summary>
        /// <returns></returns>
        internal VesselState GetVesselState()
        {
            if (vessel.isActiveVessel)
                return VesselState.Current;
            return State;
        }


        /// <summary>
        /// Get textual reprezentation of the vessel status
        /// </summary>
        /// <returns></returns>
        internal string GetVesselStateText()
        {
            if (vessel.isActiveVessel)
                return Localizer.Format("#LOC_BV_Status_Current");
            switch (State)
            {
                case VesselState.Idle:
                    return Localizer.Format("#LOC_BV_Status_Idle");
                case VesselState.ControllerDisabled:
                    return Localizer.Format("#LOC_BV_Status_Disabled");
                case VesselState.AwaitingSunlight:
                    return Localizer.Format("#LOC_BV_Status_AwaitingSunlight");
                case VesselState.Moving:
                    return Localizer.Format("#LOC_BV_Status_Moving");
                default:
                    return Localizer.Format("#LOC_BV_Status_Idle");
            }
        }

        #endregion



		#region Status window texts

		internal virtual List<DisplayedSystemCheckWidget[]> GetDisplayedSystemCheckResults()
		{
			this.displayedSystemCheckWidgets.Clear();

			DisplayedSystemCheckWidget[] result = new DisplayedSystemCheckWidget[] {
				new DisplayedSystemCheckWidget {
                    Label = Localizer.Format("#LOC_BV_Control_TargetLat"),
                    Text = targetLatitude.ToString("0.####"),
                    Tooltip = ""
                }
            };
			this.displayedSystemCheckWidgets.Add(result);

			result = new DisplayedSystemCheckWidget[] {
				new DisplayedSystemCheckWidget {
                    Label = Localizer.Format("#LOC_BV_Control_TargetLon"),
                    Text = targetLongitude.ToString("0.####"),
                    Tooltip = ""
                }
            };
			this.displayedSystemCheckWidgets.Add(result);

			result = new DisplayedSystemCheckWidget[] {
				new DisplayedSystemCheckWidget {
					Label = Localizer.Format("#LOC_BV_Control_Distance"),
					Text = this.RemainingDistanceToTargetAsText,
                    Tooltip = ""
                }
            };
			this.displayedSystemCheckWidgets.Add(result);

			result = new DisplayedSystemCheckWidget[] {
				new DisplayedSystemCheckWidget {
					Label = Localizer.Format("#LOC_BV_Control_ETA"),
					Text = this.EstimatedTimeOfArrivalAsText,
					Tooltip = this.EstimatedTimeOfArrivalAsLongText
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
        internal virtual bool FindRoute(double lat, double lon)
        {
            return FindRoute(lat, lon, TileTypes.Land | TileTypes.Ocean);
        }


        /// <summary>
        /// Find a route to the target using only specified tile types (route on land, water or both)
        /// </summary>
        /// <param name="lat"></param>
        /// <param name="lon"></param>
        /// <param name="tileType"></param>
        /// <returns></returns>
        protected bool FindRoute(double targetLat, double targetLon, TileTypes tileType)
        {
            bool result = false;
            
            PathFinder pathFinder = new PathFinder(vessel.latitude, vessel.longitude, targetLat, targetLon, vessel.mainBody, tileType);
            pathFinder.FindPath();

            double dist = pathFinder.GetDistance();
            if (dist > 0) // Path found
            {
                targetLatitude = targetLat;
                targetLongitude = targetLon;
                distanceToTarget = dist;
                distanceTravelled = 0;
                path = PathUtils.HexToWaypoint(pathFinder.path);
                result = true;
            }
            else // Path not found
                result = false;

            return result;
        }

        #endregion


        /// <summary>
        /// Check the systems
        /// </summary>
        internal virtual void SystemCheck()
        {
            mainStarIndex = Tools.GetMainStar(vessel).flightGlobalsIndex;
			this.calcCurrentSituation();

            // Get power production
			this.electricPower_Solar = this.solarPower.GetAvailablePower();
			this.electricPower_Other = this.fuelEnergy.GetAvailablePower();
        }


        /// <summary>
        /// Activate autopilot
        /// </summary>
        internal virtual bool Activate()
        {
            if (distanceToTarget == 0)
            {
                ScreenMessages.PostScreenMessage(Localizer.Format("#LOC_BV_Warning_NoRoute", 5f)).color = CommonWindowProperties.Message_Colour_Warning_User_Error;
                return false;
            }

            BonVoyageModule module = vessel.FindPartModuleImplementing<BonVoyageModule>();
            if (module != null)
            {
                distanceTravelled = 0;
                lastTimeUpdated = 0;
                active = true;

                module.active = active;
                module.targetLatitude = targetLatitude;
                module.targetLongitude = targetLongitude;
                module.distanceToTarget = distanceToTarget;
                module.distanceTravelled = distanceTravelled;
                module.pathEncoded = PathUtils.EncodePath(path);
                module.requiredPower = requiredPower;

                BonVoyage.Instance.AutopilotActivated(true);
                ScreenMessages.PostScreenMessage(Localizer.Format("#LOC_BV_BonVoyage"), 5f);
            }

            return active;
        }


        /// <summary>
        /// Deactivate autopilot
        /// </summary>
        internal virtual bool Deactivate()
        {
            BonVoyageModule module = vessel.FindPartModuleImplementing<BonVoyageModule>();
            if (module != null)
            {
                active = false;
                requiredPower = 0;
                targetLatitude = 0;
                targetLongitude = 0;
                distanceToTarget = 0;
                distanceTravelled = 0;
                path = null;

                module.active = active;
                module.targetLatitude = targetLatitude;
                module.targetLongitude = targetLongitude;
                module.distanceToTarget = distanceToTarget;
                module.distanceTravelled = distanceTravelled;
                module.pathEncoded = "";
                module.requiredPower = requiredPower;

                BonVoyage.Instance.AutopilotActivated(false);
            }

            return !active;
        }


        /// <summary>
        /// Update vessel
        /// </summary>
        /// <param name="currentTime"></param>
        internal void Update(double currentTime)
        {
            if (vessel == null)
                return;
            if (vessel.isActiveVessel)
            {
                if (active)
                    ScreenMessages.PostScreenMessage(Localizer.Format("#LOC_BV_AutopilotActive"), 14f).color = CommonWindowProperties.Message_Colour_Confirm_Restrictions;
                return;
            }
            if (!active || vessel.loaded)
                return;

			// If we don't know the last time of update, then set it and wait for the next update cycle
			if (0 == this.lastTimeUpdated)
			{
				State = VesselState.Idle;
				this.lastTimeUpdated = currentTime;
				BVModule.SetValue("lastTimeUpdated", currentTime.ToString());
				return;
			}

			State = VesselState.Idle;   // Prevent flawed logic on the updates below to play havoc.
			this.calcCurrentSituation();

			this.update(currentTime);

            Save(currentTime);
        }

        protected abstract void update(double currentTime);


        /// <summary>
        /// Save data to ProtoVessel
        /// </summary>
        protected void Save(double currentTime)
        {
            lastTimeUpdated = currentTime;

            BVModule.SetValue("distanceTravelled", distanceTravelled.ToString());
            BVModule.SetValue("lastTimeUpdated", currentTime.ToString());

            vessel.protoVessel.latitude = vessel.latitude;
            vessel.protoVessel.longitude = vessel.longitude;
            vessel.protoVessel.altitude = vessel.altitude;
            vessel.protoVessel.landedAt = vessel.mainBody.bodyName;
            vessel.protoVessel.displaylandedAt = vessel.mainBody.bodyDisplayName.Replace("^N", "");
        }


        /// <summary>
        /// Check if unmanned vessel has connection
        /// </summary>
        /// <returns></returns>
        internal bool CheckConnection()
        {
            if ((vessel.GetCrewCount() == 0) && !vessel.isEVA) // Unmanned -> check connection
            {
                // CommNet
                if (vessel.Connection.ControlState != CommNet.VesselControlState.ProbeFull)
                {
                    ScreenMessages.PostScreenMessage(Localizer.Format("#LOC_BV_Warning_NoConnection", 5f)).color = CommonWindowProperties.Message_Colour_Warning;
                    return false;
                }

                // RemoteTech
                if (Tools.AssemblyIsLoaded("RemoteTech"))
                {
                    if (RemoteTechWrapper.IsRemoteTechEnabled() && !RemoteTechWrapper.HasAnyConnection(vessel.id) && !RemoteTechWrapper.HasLocalControl(vessel.id))
                    {
                        ScreenMessages.PostScreenMessage(Localizer.Format("#LOC_BV_Warning_NoConnection", 5f)).color = CommonWindowProperties.Message_Colour_Warning;
                        return false;
                    }
                }
            }
            return true;
        }


        /// <summary>
        /// Deduct used amount from resource tanks
        /// </summary>
        internal void ProcessResources()
        {
			// Leave resource processing to Kerbalism if it is there
            if (DetectKerbalism.Found())
                return;

			this.fuelEnergy.ProcessResources(this.resourceBroker);

            // RealBattery integration: if we travelled as a rover and actually used the battery
            // budget, drain that energy from StoredCharge on RealBattery-enabled parts. BV's own
            // battery accounting (batteries.CurrentEC) is otherwise purely virtual — it never
            // touches a real resource tank on its own.
            if (Tools.AssemblyIsLoaded("RealBattery") && this is RoverController rover)
                DrainRealBattery(rover);
        }

        /// <summary>
        /// RealBattery integration: drains the EC deficit accumulated by the rover's virtual
        /// battery accounting (MaxUsedEC - CurrentEC) from StoredCharge, pro-quota of capacity
        /// across all non-disabled RealBattery parts so no single part is emptied first while
        /// the rest of the bank sits untouched. Resets CurrentEC to MaxUsedEC afterwards so the
        /// same deficit isn't drained again on a later vessel load (this method may run once per
        /// physics load, not just once per completed trip).
        /// </summary>
        private void DrainRealBattery(RoverController rover)
        {
            if (!rover.batteries.Use || rover.batteries.MaxUsedEC <= 0.0)
                return;

            double ecUsed = rover.batteries.MaxUsedEC - rover.batteries.CurrentEC;
            if (ecUsed <= 0.01)
                return; // nothing to drain

            double scToDrain = ecUsed / RealBatterySupport.EcPerSc;

            double totalCapacity = 0.0;
            List<PartResource> rbResources = new List<PartResource>();
            for (int i = 0; i < vessel.parts.Count; ++i)
            {
                Part part = vessel.parts[i];
                if (!part.Modules.Contains("RealBattery")) continue;

                PartModule rbModule = part.Modules["RealBattery"];
                BaseField disabledField = rbModule.Fields["BatteryDisabled"];
                if (disabledField != null && disabledField.GetValue(rbModule) is bool disabled && disabled)
                    continue;

                if (!part.Resources.Contains("StoredCharge")) continue;

                PartResource sc = part.Resources["StoredCharge"];
                rbResources.Add(sc);
                totalCapacity += sc.maxAmount;
            }

            if (totalCapacity <= 0.0)
                return;

            for (int i = 0; i < rbResources.Count; ++i)
            {
                PartResource sc = rbResources[i];
                double share = sc.maxAmount / totalCapacity;
                sc.amount = Math.Max(0.0, sc.amount - scToDrain * share);
            }

            // Anti-double-drain fix: this deficit has now been accounted for.
            rover.batteries.CurrentEC = rover.batteries.MaxUsedEC;
        }


        /// <summary>
        /// Get speed penalty for unmanned ship based on tech available
        /// </summary>
        /// <returns></returns>
        internal double GetUnmannedSpeedPenalty()
        {
            BonVoyageModule module = vessel.FindPartModuleImplementing<BonVoyageModule>();
            if (module != null)
            {
                switch (module.techLevel)
                {
                    case 2: // unmannedTech
                        return 60;
                    case 3: // automation
                        return 40;
                    case 4: // artificialIntelligence
                        return 20;
                    default: // no tech
                        return 80;
                }
            }

            return 80;
        }

		private void calcCurrentSituation()
		{
			Vector3d toMainStar = this.vessel.mainBody.position - FlightGlobals.Bodies[mainStarIndex].position;
			this.vesselPos = vessel.mainBody.position - this.vessel.GetWorldPos3D();
			this.angle = Vector3d.Angle(this.vesselPos, toMainStar); // Angle between rover and the main star
		}
    }

}
