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
using KSP.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BonVoyage
{
    /// <summary>
    /// Main mod's window - model part
    /// </summary>
    internal class MainWindowModel
    {
        private readonly object refreshLock = new object(); // Object to lock access during refreshing of the list. Multiple events can be raised at the same time to request refresh.
        private bool activeControllersChecked = true;
        private bool disabledControllersChecked = false;

        // Displayed vessel list
        private DialogGUIVerticalLayout vesselListLayout = null;


        /// <summary>
        /// Constructor
        /// </summary>
        internal MainWindowModel()
        {
            // Load from configuration
            CommonWindowProperties.MainWindowPosition = Configuration.MainWindowPosition;
            activeControllersChecked = Configuration.ActiveControllers;
            disabledControllersChecked = Configuration.DisabledControllers;
        }


        /// <summary>
        /// Active controllers checkbox
        /// </summary>
        /// <param name="value"></param>
        internal void ActiveControllersChecked(bool value)
        {
            activeControllersChecked = value;
            Configuration.ActiveControllers = value;
            RefreshVesselListLayout();
        }


        /// <summary>
        /// Get the state of Active controllers toggle
        /// </summary>
        /// <returns></returns>
        internal bool GetActiveControllersToggleState()
        {
            return activeControllersChecked;
        }


        /// <summary>
        /// Disabled controllers checkbox
        /// </summary>
        /// <param name="value"></param>
        internal void DisabledControllersChecked(bool value)
        {
            disabledControllersChecked = value;
            Configuration.DisabledControllers = value;
            RefreshVesselListLayout();
        }


        /// <summary>
        /// Get the state of Disabled controllers toggle
        /// </summary>
        /// <returns></returns>
        internal bool GetDisabledControllersToggleState()
        {
            return disabledControllersChecked;
        }


        /// <summary>
        /// Return text of the control button
        /// </summary>
        /// <returns></returns>
        internal string GetControlButtonText()
        {
            if (BonVoyage.Instance.ControlViewVisible)
                return Localizer.Format("#LOC_BV_HideControl");
            else
                return Localizer.Format("#LOC_BV_ShowControl");
        }


        /// <summary>
        /// Detect, if can be control button enabled
        /// </summary>
        /// <returns></returns>
        internal bool ControlButtonCanBeEnabled()
        {
            if (HighLogic.LoadedSceneIsFlight)
                return BonVoyage.Instance.CheckActiveControllerOfVessel(FlightGlobals.ActiveVessel);
            else
                return false;
        }


        /// <summary>
        /// Switch to vessel
        /// </summary>
        /// <param name="vesselId"></param>
        private void SwitchToVessel(Guid vesselId)
        {
            BVController controller = BonVoyage.Instance.GetControllerOfVessel(vesselId);
            if (controller == null)
				return;
            Vessel v = controller.vessel;
            if (v.loaded)
            {
                FlightGlobals.SetActiveVessel(v);
            }
            else
            {
                GamePersistence.SaveGame("persistent", HighLogic.SaveFolder, SaveMode.OVERWRITE);
                FlightDriver.StartAndFocusVessel("persistent", FlightGlobals.Vessels.IndexOf(v));
            }
        }


        /// <summary>
        /// Event raised by controller, if it's state changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnControllerStateChanged(object sender, EventArgs e)
        {
            RefreshVesselListLayout();
        }


        /// <summary>
        /// Get speed of a vessel based on the index in list of controllers
        /// </summary>
        /// <param name="controllerIndex"></param>
        /// <returns></returns>
        private string GetSpeed(BVController controller)
        {
            string result = "-";
            if (controller == null)
                return result;

            if ((controller.GetVesselState() == VesselState.Moving) || (controller.GetVesselState() == VesselState.AwaitingSunlight))
            {
                result = controller.AverageSpeed.ToString("0.##") + " m/s";
            }
            return result;
        }


        /// <summary>
        /// Get distance to a target of a vessel based on the index in list of controllers
        /// </summary>
        /// <param name="controllerIndex"></param>
        /// <returns></returns>
        private string GetDistanceToTarget(BVController controller)
        {
            string result = "-";
            if (controller == null)
                return result;

            if ((controller.GetVesselState() == VesselState.Moving) || (controller.GetVesselState() == VesselState.AwaitingSunlight))
            {
                result = Tools.ConvertDistanceToText(controller.RemainingDistanceToTarget);
            }
            return result;
        }


        /// <summary>
        /// Create table row for controller
        /// </summary>
        /// <param name="controller"></param>
        /// <returns>DialogGUIHorizontalLayout row or null if controller state don't equals to selected filter</returns>
        private DialogGUIHorizontalLayout CreateListLayoutRow(BVController controller)
        {
            DialogGUIHorizontalLayout row = null;

            if ((activeControllersChecked && !controller.Shutdown) || (disabledControllersChecked && controller.Shutdown))
            {
                UIStyle statusStyle;
                switch (controller.GetVesselState())
                {
                    case VesselState.Current:
                        statusStyle = CommonWindowProperties.Style_Label_Normal_Center_White;
                        break;
                    case VesselState.Idle:
                        statusStyle = CommonWindowProperties.Style_Label_Normal_Center_Grey;
                        break;
                    case VesselState.ControllerDisabled:
                        statusStyle = CommonWindowProperties.Style_Label_Normal_Center_Red;
                        break;
                    case VesselState.AwaitingSunlight:
                        statusStyle = CommonWindowProperties.Style_Label_Normal_Center_Yellow;
                        break;
                    case VesselState.Moving:
                        statusStyle = CommonWindowProperties.Style_Label_Normal_Center_Green;
                        break;
                    default:
                        statusStyle = CommonWindowProperties.Style_Label_Normal_Center_Grey;
                        break;
                }

                row = new DialogGUIHorizontalLayout(
                    new DialogGUILabel(controller.vessel.GetDisplayName(), 150f),
                    new DialogGUISpace(10f),
                    new DialogGUILabel(controller.GetVesselStateText(), 70f) { guiStyle = statusStyle },
                    new DialogGUISpace(10f),
                    new DialogGUILabel(controller.vessel.mainBody.bodyDisplayName.Replace("^N", ""), 60f) { guiStyle = CommonWindowProperties.Style_Label_Normal_Center },
                    new DialogGUISpace(10f),
                    new DialogGUILabel(delegate { return GetSpeed(controller); }, 60f) { guiStyle = CommonWindowProperties.Style_Label_Normal_Center },
                    new DialogGUISpace(10f),
                    new DialogGUILabel(delegate { return GetDistanceToTarget(controller); }, 90f) { guiStyle = CommonWindowProperties.Style_Label_Normal_Center },
                    new DialogGUISpace(10f),
                    (
                        !controller.vessel.isActiveVessel
                        ?
                        TooltipExtension.DeferTooltip(new DialogGUIButton("->",
                            delegate { SwitchToVessel(controller.vessel.id); }, 22f, 16f, false)
                        { tooltipText = (Localizer.Format("#LOC_BV_SwitchTo") + " " + controller.vessel.GetDisplayName()) })
                        :
                        new DialogGUISpace(10f)
                    )

                );
                row.SetOptionText(controller.vessel.id.ToString()); // ID of the row (vessel ID)
                controller.OnStateChanged += OnControllerStateChanged; // Register state changed event
            }

            return row;
        }


        /// <summary>
        /// Get layout of the list of vessels
        /// </summary>
        /// <returns></returns>
        internal DialogGUIVerticalLayout GetVesselListLayout()
        {
            // Count disabled controllers
            int disabledControllersCount = 0;
            int controllersCount = BonVoyage.Instance.BVControllers.Count();
            foreach (BVController controller in BonVoyage.Instance.BVControllers.Values)
			{
                if (controller.Shutdown)
                    disabledControllersCount++;
            }

            int listLength = 1;
            if (activeControllersChecked)
                listLength += controllersCount - disabledControllersCount;
            if (disabledControllersChecked)
                listLength += disabledControllersCount;

            DialogGUIBase[] list = new DialogGUIBase[listLength];
            list[0] = new DialogGUIContentSizer(UnityEngine.UI.ContentSizeFitter.FitMode.Unconstrained, UnityEngine.UI.ContentSizeFitter.FitMode.PreferredSize, true);

            if (listLength > 1) // anything is checked
            {
                int counter = 1;
				foreach (BVController controller in BonVoyage.Instance.BVControllers.Values)
                {
                    DialogGUIHorizontalLayout row = CreateListLayoutRow(controller); 
                    if (row != null)
                    {
                        list[counter] = row;
                        counter++;
                        if (counter >= listLength) // break if we are at the end of list
                            break;
                    }
                }
            }

            vesselListLayout = new DialogGUIVerticalLayout(
                CommonWindowProperties.mainListMinWidth,
                CommonWindowProperties.mainListMinHeight,
                CommonWindowProperties.mainWindowSpacing,
                CommonWindowProperties.mainListPadding,
                TextAnchor.UpperLeft,
                list
            );

            return vesselListLayout;
        }


        /// <summary>
        /// Clear layout of the list of vessels
        /// </summary>
        internal void ClearVesselListLayout()
        {
            // Clear events
			foreach(BVController controller in BonVoyage.Instance.BVControllers.Values)
                controller.OnStateChanged -= OnControllerStateChanged;

            vesselListLayout = null;
        }


        /// <summary>
        /// Refresh list of vessels without reloading the list of controllers
        /// </summary>
        internal void RefreshVesselListLayout()
        {
            lock (refreshLock) // Only one refresh at a time
            {
                Stack<Transform> stack = new Stack<Transform>();  // some data on hierarchy of GUI components
                stack.Push(vesselListLayout.uiItem.gameObject.transform); // need the reference point of the parent GUI component for position and size

                List<DialogGUIBase> rows = vesselListLayout.children;

                // Clear list. We are skiping DialogGUIContentSizer
                while (rows.Count > 1)
                {
                    DialogGUIBase child = rows.ElementAt(1); // Get child
                    rows.RemoveAt(1); // Drop row
                    child.uiItem.gameObject.DestroyGameObjectImmediate(); // Free memory up
                }

                // Add rows
				foreach (BVController controller in BonVoyage.Instance.BVControllers.Values)
				{
                    controller.OnStateChanged -= OnControllerStateChanged; // Clear possible event
                    DialogGUIHorizontalLayout row = CreateListLayoutRow(controller);
                    if (row != null)
                    {
                        rows.Add(row);
                        rows.Last().Create(ref stack, CommonWindowProperties.ActiveSkin); // required to force the GUI creatio﻿n
                    }
                }
            }
        }


        /// <summary>
        /// Reload vessels and refresh list
        /// </summary>
        internal void ReloadVesselList()
        {
            BonVoyage.Instance.LoadControllers();
            RefreshVesselListLayout();
        }

    }

}
