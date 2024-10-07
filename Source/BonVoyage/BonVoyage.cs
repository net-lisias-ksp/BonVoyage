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
using System.Collections.Generic;
using UnityEngine;
using KSP.UI.Screens;
using UI;
using KSP.Localization;
using ToolbarWrapper;

namespace BonVoyage
{
    /// <summary>
    /// Mod's main class
    /// </summary>
    [KSPAddon(KSPAddon.Startup.SpaceCentre, true)]
    public class BonVoyage : MonoBehaviour
    {
        #region Public properties

        public static BonVoyage Instance { get; private set; } // Mod's instance
        public const string Name = "BonVoyage"; // Name of the mod

        internal MainWindowModel MainModel; // Main view's model
        internal bool MainViewVisible { get { return mainViewVisible; } }
        internal SettingsWindowModel SettingsModel; // Settings view's model
        internal ControlWindowModel ControlModel; // Control view's model
        internal bool ControlViewVisible { get { return controlViewVisible; } }

        internal Dictionary<Guid, BVController> BVControllers; // Controllers list

        internal bool GamePaused; // Is game paused?
        internal bool ShowUI; // Is UI vissible? (F2 pressed)
        internal bool MapMode; // if true, then target will be picked in the map mode

        #endregion


        #region Private properties

        private ApplicationLauncherButton appLauncherButton; // Button in the game's toolbar
        private IButton toolbarButton; // Toolbar Continued button

        private bool mainViewVisible; // Is main view visible?
        private MainWindowView MainView { get; set; } // Mod's main view

        private bool setttingsViewVisible; // Is settings view visible?
        private SettingsWindowView SettingsView { get; set; } // Mod's settings view

        private bool controlViewVisible; // Is control view visible?
        private ControlWindowView ControlView { get; set; } // Mod's control view

        // Lock mask for a vessel with active autopilot
        private ControlTypes lockMask = (
            ControlTypes.YAW |
            ControlTypes.PITCH |
            ControlTypes.ROLL |
            ControlTypes.THROTTLE |
            ControlTypes.STAGING |
            ControlTypes.RCS |
            ControlTypes.SAS |
            ControlTypes.WHEEL_STEER |
            ControlTypes.WHEEL_THROTTLE |
            ControlTypes.GROUPS_ALL
        );

        private DateTime lastUpdate; // Last time of controllers update cycle

        private bool otherStabilizerPresent; // Set to true if other stabilizing mod is present

        #endregion


        /// <summary>
        /// Plugin constructor
        /// </summary>
        public void Awake()
        {
            if (Instance != null)
            {
                Destroy(this);
                return;
            }
            Instance = this;

            CommonWindowProperties.ActiveSkin = UISkinManager.defaultSkin;
            CommonWindowProperties.UnitySkin = null;
            CommonWindowProperties.RefreshStyles();

            MainView = null;
            MainModel = null;
            mainViewVisible = false;
            SettingsView = null;
            SettingsModel = null;
            setttingsViewVisible = false;
            ControlView = null;
            ControlModel = null;
            controlViewVisible = false;

            toolbarButton = null;

            GamePaused = false;
            ShowUI = true;
            MapMode = false;
            lastUpdate = DateTime.Now;

            BVControllers = new Dictionary<Guid, BVController>();

            Configuration.Load();
        }


        /// <summary>
        /// Initial start
        /// </summary>
        public void Start()
        {
            DontDestroyOnLoad(this);
            GameEvents.onGUIApplicationLauncherReady.Add(AddLauncher);
            GameEvents.onGUIApplicationLauncherDestroyed.Add(RemoveLauncher);
            GameEvents.onGameSceneSwitchRequested.Add(OnGameSceneSwitchRequested);
            GameEvents.onVesselChange.Add(OnVesselChange);
            GameEvents.onLevelWasLoaded.Add(OnLevelWasLoaded);
            GameEvents.onVesselGoOffRails.Add(OnVesselGoOffRails);
            GameEvents.onHideUI.Add(OnHideUI);
            GameEvents.onShowUI.Add(OnShowUI);
            GameEvents.onGamePause.Add(OnGamePause);
            GameEvents.onGameUnpause.Add(OnGameUnpause);

            LoadControllers();
            AddScenario();

            // After BonVoyage was run for the first time, set FirstRun to false, because we don't need to reset path and target lat/lon
            Configuration.FirstRun = false;

            InputLockManager.RemoveControlLock("BonVoyageInputLock");

            otherStabilizerPresent = (Tools.AssemblyIsLoaded("WorldStabilizer") || Tools.AssemblyIsLoaded("BDArmory"));
        }


        /// <summary>
        /// Clean up
        /// </summary>
        public void OnDestroy()
        {
            GameEvents.onGUIApplicationLauncherReady.Remove(AddLauncher);
            GameEvents.onGUIApplicationLauncherDestroyed.Remove(RemoveLauncher);
            GameEvents.onGameSceneSwitchRequested.Remove(OnGameSceneSwitchRequested);
            GameEvents.onVesselChange.Remove(OnVesselChange);
            GameEvents.onLevelWasLoaded.Remove(OnLevelWasLoaded);
            GameEvents.onVesselGoOffRails.Remove(OnVesselGoOffRails);
            GameEvents.onHideUI.Remove(OnHideUI);
            GameEvents.onShowUI.Remove(OnShowUI);
            GameEvents.onGamePause.Remove(OnGamePause);
            GameEvents.onGameUnpause.Remove(OnGameUnpause);

            RemoveLauncher();

            Configuration.Save();
        }


        #region Game events

        /// <summary>
        /// Get Unity skin - only accessible in OnGUI
        /// </summary>
        void OnGUI()
        {
            if (GamePaused && !ShowUI)
                return;
            
            if (CommonWindowProperties.UnitySkin == null)
            {
                CommonWindowProperties.UnitySkin = StyleConverter.Convert(GUI.skin);

                // Load Unity skin, if it was saved in the config
                if (Configuration.Skin == 1)
                {
                    CommonWindowProperties.ActiveSkin = CommonWindowProperties.UnitySkin;
                    CommonWindowProperties.RefreshStyles();
                }
            }
        }


        /// <summary>
        /// Game paused
        /// </summary>
        public void OnGamePause()
        {
            GamePaused = true;
        }


        /// <summary>
        /// Game unpaused
        /// </summary>
        public void OnGameUnpause()
        {
            GamePaused = false;
        }


        /// <summary>
        /// Show UI
        /// </summary>
        private void OnShowUI()
        {
            ShowUI = true;
        }


        /// <summary>
        /// Hide UI
        /// </summary>
        private void OnHideUI()
        {
            ShowUI = false;
        }


        /// <summary>
        /// Change UI state to hidden to prevent app button weirdness (PopupDialog is automatically hidden on scene change)
        /// </summary>
        /// <param name="ev"></param>
        public void OnGameSceneSwitchRequested(GameEvents.FromToAction<GameScenes, GameScenes> ev)
        {
            if (appLauncherButton != null)
                appLauncherButton.SetFalse(true);
            else
                mainViewVisible = false;
            if (controlViewVisible)
                ToggleControlWindow();

            InputLockManager.RemoveControlLock("BonVoyageInputLock");
        }


        /// <summary>
        /// Vessel changed. Refresh vessel list in the main window if it's still visible and hide control window (scene was not switched)
        /// </summary>
        /// <param name="vessel"></param>
        public void OnVesselChange(Vessel vessel)
        {
            if (MainView != null)
                MainModel.RefreshVesselListLayout();
            if (controlViewVisible)
                ToggleControlWindow();

            BonVoyageModule currentModule = vessel.FindPartModuleImplementing<BonVoyageModule>();
            if (currentModule != null)
            {
                if (currentModule.active)
                {
                    InputLockManager.SetControlLock(lockMask, "BonVoyageInputLock");
                    return;
                }
            }
            InputLockManager.RemoveControlLock("BonVoyageInputLock");
        }


        /// <summary>
        /// Reload list of controllers when new scene is loaded.
        /// Deprecated?
        /// </summary>
        /// <param name="scenes"></param>
        public void OnLevelWasLoaded(GameScenes scene)
        {
            if ((scene == GameScenes.FLIGHT) || (scene == GameScenes.SPACECENTER) || (scene == GameScenes.TRACKSTATION))
            {
                LoadControllers();
                if (BonVoyageScenario.Instance != null)
                    BonVoyageScenario.Instance.LoadScenario();
            }

            GamePaused = false;
            ShowUI = true;
        }


        /// <summary>
        /// Move and rotate a rover or a ship
        /// Process resources
        /// </summary>
        /// <param name="vessel"></param>
        public void OnVesselGoOffRails(Vessel vessel)
        {
            if ((vessel.situation == Vessel.Situations.LANDED) || (vessel.situation == Vessel.Situations.SPLASHED))
            {
                //if (vessel.isEVA) // Kerbals
                //    return;
                if (vessel.packed) // No physics
                    return;

                BVController controller = GetControllerOfVessel(vessel);
				
                if (controller != null)
                {
                    // Move only a rover or landed Kerbal
                    if ((controller is RoverController) || ((controller is KerbalController) && (vessel.situation == Vessel.Situations.LANDED)))
                    {
                        // Only rovers/Kerbals with active controller or rovers/Kerbals that just arrived at the destination
                        if (controller.Active || controller.Arrived)
                        {
                            // Stabilize only if another stabilizer is not present
                            if (!otherStabilizerPresent)
                                StabilizeVessel.AddVesselToStabilize(vessel, controller.RotationVector, Configuration.DisableRotation);
                            else // only rotate
                            {
                                if (!Configuration.DisableRotation)
                                    StabilizeVessel.Rotate(vessel, controller.RotationVector);
                            }
                        }
                    }

                    // Move only a ship or splashed Kerbal
                    if ((controller is ShipController) || ((controller is KerbalController) && (vessel.situation == Vessel.Situations.SPLASHED)))
                    {
                        // Only ships/Kerbals with active controller or ships/Kerbals that just arrived at the destination
                        if (controller.Active || controller.Arrived)
                        {
                            if (!Configuration.DisableRotation)
                                StabilizeVessel.Rotate(vessel, controller.RotationVector);
                        }
                    }

                    // Deduct resources
                    if (!vessel.isEVA)
                        controller.ProcessResources();
                }
            }
        }

        #endregion


        #region App launcher

        /// <summary>
        /// Launcher is ready to be initialized
        /// </summary>
        public void AddLauncher()
        {
            CreatePluginButton();
        }


        /// <summary>
        /// Create buttons for the plugin
        /// </summary>
        private void CreatePluginButton()
        {
            // Create button in the KSP toolbar if the check is selected or Toolbar Continued is unavailable
            if ((appLauncherButton == null) && (Configuration.KSPToolbar || !ToolbarManager.ToolbarAvailable))
            {
                appLauncherButton = ApplicationLauncher.Instance.AddModApplication(
                    OnAppLaunchToggleOn,
                    OnAppLaunchToggleOff,
                    null,
                    null,
                    null,
                    null,
                    ApplicationLauncher.AppScenes.SPACECENTER |
                    ApplicationLauncher.AppScenes.TRACKSTATION |
                    ApplicationLauncher.AppScenes.FLIGHT |
                    ApplicationLauncher.AppScenes.MAPVIEW,
                    GameDatabase.Instance.GetTexture(Tools.TextureFilePath("bon-voyage-icon"), false)
                );

                appLauncherButton.gameObject.SetTooltip(Localizer.Format("#LOC_BV_Title"), Localizer.Format("#LOC_BV_Tooltip"));
                appLauncherButton.onRightClick = OnRightClick;
            }

            // Create button in the Toolbar Continued
            if ((toolbarButton == null) && Configuration.ToolbarContinued && ToolbarManager.ToolbarAvailable)
            {
                toolbarButton = ToolbarManager.Instance.add(Name, "AppLauncherButton");
                toolbarButton.TexturePath = Tools.TextureFilePath("bon-voyage-icon-toolbar");
                toolbarButton.ToolTip = Localizer.Format("#LOC_BV_Title") + "\n" + Localizer.Format("#LOC_BV_Tooltip");
                toolbarButton.Visibility = new GameScenesVisibility(GameScenes.SPACECENTER, GameScenes.TRACKSTATION, GameScenes.FLIGHT);
                toolbarButton.Visible = true;
                toolbarButton.OnClick += (ClickEvent e) => { OnTCClick(e); };
            }
        }


        /// <summary>
        /// Destroy app launcher button
        /// </summary>
        internal void RemoveAppLauncherButton()
        {
            if (appLauncherButton != null)
            {
                ApplicationLauncher.Instance.RemoveModApplication(appLauncherButton);
                appLauncherButton = null;
            }
        }


        /// <summary>
        /// Destroy Toolbar Continued button
        /// </summary>
        internal void RemoveToolbarContinuedButton()
        {
            if (toolbarButton != null)
            {
                toolbarButton.Destroy();
                toolbarButton = null;
            }
        }


        /// <summary>
        /// Destroy buttons for the plugin
        /// </summary>
        private void RemoveLauncher()
        {
            RemoveAppLauncherButton();

            RemoveToolbarContinuedButton();
        }


        /// <summary>
        /// Show GUI when button is clicked first time
        /// </summary>
        private void OnAppLaunchToggleOn()
        {
            mainViewVisible = true;
            ShowMainWindow();
        }


        /// <summary>
        /// Hide GUI when button is clicked second time
        /// </summary>
        private void OnAppLaunchToggleOff()
        {
            mainViewVisible = false;
            HideMainWindow();
        }


        /// <summary>
        /// Right click on the application button
        /// </summary>
        private void OnRightClick()
        {
            ToggleControlWindow();
        }


        /// <summary>
        /// Click on the application button in Toolbar Continued
        /// </summary>
        private void OnTCClick(ClickEvent e)
        {
            if (e.MouseButton == 1) // Right click
                ToggleControlWindow();
            else
            {
                if (appLauncherButton != null) // If KSP app launcher button is created, then use his events
                {
                    if (mainViewVisible) // visible? -> hide
                        appLauncherButton.SetFalse(true);
                    else // show
                        appLauncherButton.SetTrue(true);
                }
                else
                    ToggleMainWindow();
            }
        }

        #endregion


        #region Main View

        /// <summary>
        /// Toggle state of the main window dialog
        /// </summary>
        private void ToggleMainWindow()
        {
            mainViewVisible = !mainViewVisible;

            if (mainViewVisible)
                ShowMainWindow();
            else
                HideMainWindow();
        }

        /// <summary>
        /// Show main window dialog
        /// </summary>
        private void ShowMainWindow()
        {
            if (MainView == null)
            {
                if (MainModel == null) // Create model for the Main View
                    MainModel = new MainWindowModel();
                
                MainView = new MainWindowView(MainModel, ToggleSettingsWindow, ToggleControlWindow, () => { if (appLauncherButton != null) appLauncherButton.SetFalse(true); else ToggleMainWindow(); });
                MainView.Show();
            }
        }


        /// <summary>
        /// Hide main window dialog
        /// </summary>
        private void HideMainWindow()
        {
            if (MainView != null)
            {
                if (setttingsViewVisible) // hide settings, when hiding main view
                    ToggleSettingsWindow();

                MainView.Dismiss();
                MainView = null;
            }
        }

        #endregion


        #region Settings view

        /// <summary>
        /// Toggle state of the settings window dialog
        /// </summary>
        private void ToggleSettingsWindow()
        {
            setttingsViewVisible = !setttingsViewVisible;

            if (setttingsViewVisible)
                ShowSettingsWindow();
            else
                HideSettingsWindow();
        }


        /// <summary>
        /// Show settings window dialog
        /// </summary>
        private void ShowSettingsWindow()
        {
            if (SettingsView == null)
            {
                if (SettingsModel == null) // Create model for the Settings View
                    SettingsModel = new SettingsWindowModel();
                
                SettingsView = new SettingsWindowView(SettingsModel, MainView.GetWindowPosition(), ToggleSettingsWindow);
                SettingsView.Show();
            }
        }

        /// <summary>
        /// Hide settings window dialog
        /// </summary>
        private void HideSettingsWindow()
        {
            if (SettingsView != null)
            {
                SettingsView.Dismiss();
                SettingsView = null;
            }
        }

        #endregion


        #region Control view

        /// <summary>
        /// Toggle state of the control window dialog
        /// </summary>
        internal void ToggleControlWindow()
        {
            controlViewVisible = !controlViewVisible;

            if (controlViewVisible)
            {
                // Check if we are in flight, active vessel has full controll and BV controller and is not shutted down
                bool active = false;
                if (HighLogic.LoadedSceneIsFlight)
                {
                    Vessel vessel = FlightGlobals.ActiveVessel;
                    BVController controller = GetControllerOfVessel(FlightGlobals.ActiveVessel);
                    if (controller != null)
                        active = (!controller.Shutdown && controller.CheckConnection());
                }

                if (active && (ControlView == null))
                    ShowControlWindow();
                else
                    controlViewVisible = false;
            }
            else
                HideControlWindow();
        }


        /// <summary>
        /// Show control window dialog
        /// </summary>
        private void ShowControlWindow()
        {
            if (ControlView == null)
            {
                if (ControlModel == null) // Create model for the Settings View
                    ControlModel = new ControlWindowModel();

                if (HighLogic.LoadedSceneIsFlight)
                    ControlModel.SetController(GetControllerOfVessel(FlightGlobals.ActiveVessel));
                else
                    ControlModel.SetController(null);
                ControlView = new ControlWindowView(ControlModel, ToggleControlWindow);
                ControlView.Show();
            }
        }

        /// <summary>
        /// Hide control window dialog
        /// </summary>
        private void HideControlWindow()
        {
            if (ControlView != null)
            {
                ControlView.Dismiss();
                ControlView = null;
                ControlModel.SetController(null);
            }
        }

        #endregion


        /// <summary>
        /// Show/Hide windows during Update
        /// Update controllers once a second
        /// </summary>
        public void Update()
        {
            // Escape was pressed -> close opened windows (set launcher state to false, so next time a window will be opened)
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (MainView != null)
                {
                    if (appLauncherButton != null)
                        appLauncherButton.SetFalse(true);
                    else
                        ToggleMainWindow();
                }
                if (controlViewVisible)
                {
                    controlViewVisible = false;
                    HideControlWindow();
                }
            }
            
            // Update controllers once a second
            if (GamePaused || ((HighLogic.LoadedScene != GameScenes.FLIGHT) && (HighLogic.LoadedScene != GameScenes.SPACECENTER) && (HighLogic.LoadedScene != GameScenes.TRACKSTATION)))
                return;
            
            if (lastUpdate.AddSeconds(1) > DateTime.Now)
                return;
            
            lastUpdate = DateTime.Now;
            double currentTime = Planetarium.GetUniversalTime();
			foreach (BVController controller in BVControllers.Values)
                controller.Update(currentTime);
        }


        /// <summary>
        /// Stabilize vessel on fixed update
        /// </summary>
        public void FixedUpdate()
        {
            if (!otherStabilizerPresent)
                StabilizeVessel.Stabilize();
        }


        /// <summary>
        /// Reset windows when skin was changed
        /// </summary>
        internal void ResetWindows()
        {
            bool settings = setttingsViewVisible; // Settings are closed by HideMainWindow automatically, so we need to store the value
            if (MainView != null)
            {
                HideMainWindow();
                ShowMainWindow();
            }
            if (settings)
                ToggleSettingsWindow();
            if (ControlView != null)
            {
                HideControlWindow();
                ShowControlWindow();
            }
        }


        /// <summary>
        /// Set value of shutdown in a controller and refresh the main view after a change in the BonVoyageModule
        /// </summary>
        internal void SetShutdownState(Guid vesselId, bool value)
        {
            BVController controller;
			if (BVControllers.TryGetValue(vesselId, out controller))
                controller.Shutdown = value;

            if (MainView != null)
                MainModel.RefreshVesselListLayout();
            if ((ControlView != null) && value)
                ToggleControlWindow();
        }


        /// <summary>
        /// Load BV controllers from the config
        /// </summary>
        internal void LoadControllers()
        {
            Vessel vessel = null;
            ProtoPartSnapshot part = null;

            BVControllers.Clear();

            for (int i = 0; i < FlightGlobals.Vessels.Count; i++)
            {
                vessel = FlightGlobals.Vessels[i];
                ConfigNode vesselConfigNode = new ConfigNode();
                vessel.protoVessel.Save(vesselConfigNode);
                for (int k = 0; k < vessel.protoVessel.protoPartSnapshots.Count; k++)
                {
                    part = vessel.protoVessel.protoPartSnapshots[k];
                    ProtoPartModuleSnapshot module = part.FindModule("BonVoyageModule");
                    if (module != null)
                    {
                        ConfigNode BVModule = module.moduleValues;
                        string vesselType = BVModule.GetValue("vesselType");
                        if (vessel.isActiveVessel)
                        {
                            BonVoyageModule m = vessel.FindPartModuleImplementing<BonVoyageModule>();
                            if (m != null)
                                vesselType = m.vesselType;
                            else
                                continue;
                        }
                        if (vessel.isEVA) // Kerbal on EVA
                            vesselType = "2";
                        BVController controller = null;
                        switch (vesselType)
                        {
                            case "0": // rover
                                controller = new RoverController(vessel, BVModule);
                                break;
                            case "1": // ship
                                controller = new ShipController(vessel, BVModule);
                                break;
                            case "2": // Kerbal
                                controller = new KerbalController(vessel, BVModule);
                                break;
                            default: // default to rover
                                controller = new RoverController(vessel, BVModule);
                                break;
                        }
                        BVControllers[vessel.id] = controller;
                        break;
                    }
                }
            }
        }


        /// <summary>
        /// Check if vessel has controller and is active
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        internal bool CheckActiveControllerOfVessel(Vessel v)
        {
            bool active = false;
            if (HighLogic.LoadedSceneIsFlight)
            {
                BVController controller = GetControllerOfVessel(v);
                if (controller != null && !controller.Shutdown)
                    active = true;
            }
            return active;
        }


        /// <summary>
        /// Get controller of a vessel
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        internal BVController GetControllerOfVessel(Vessel v)
        {
            return GetControllerOfVessel(v.id);
        }
        internal BVController GetControllerOfVessel(Guid vesselId)
        {
            if (BVControllers.ContainsKey(vesselId))
                return BVControllers[vesselId];
            return null;
        }


        /// <summary>
        /// Actions, when autopilot was activated
        /// </summary>
        /// <param name="value"></param>
        internal void AutopilotActivated(bool value)
        {
            if (value)
            {
                InputLockManager.SetControlLock(lockMask, "BonVoyageInputLock");
                if (controlViewVisible)
                    ToggleControlWindow();
                ScreenMessages.PostScreenMessage(Localizer.Format("#LOC_BV_AutopilotActive"), 10f).color = Color.red;
            }
            else
                InputLockManager.RemoveControlLock("BonVoyageInputLock");
        }


        /// <summary>
        /// Add BonVoyage scenario to scenes (flight, space center, tracking station)
        /// </summary>
        private void AddScenario()
        {
            var game = HighLogic.CurrentGame;
            var psm = game.scenarios.Find(s => s.moduleName == typeof(BonVoyageScenario).Name);
            if (psm == null) // Our scenario doesn't exist => add it to all relevant scenes
            {
                game.AddProtoScenarioModule(typeof(BonVoyageScenario), GameScenes.FLIGHT, GameScenes.SPACECENTER, GameScenes.TRACKSTATION);
            }
            else // Check which scene don't have scenario and add it
            {
                bool flight = false, space = false, track = false;
                int count = psm.targetScenes.Count;
                for (int i = 0; i < count; i++)
                {
                    var s = psm.targetScenes[i];
                    if (s == GameScenes.FLIGHT)
                        flight = true;
                    if (s == GameScenes.SPACECENTER)
                        space = true;
                    if (s == GameScenes.TRACKSTATION)
                        track = true;
                }
                if (!flight)
                    psm.targetScenes.Add(GameScenes.FLIGHT);
                if (!space)
                    psm.targetScenes.Add(GameScenes.SPACECENTER);
                if (!track)
                    psm.targetScenes.Add(GameScenes.TRACKSTATION);
            }
        }

    }

}
