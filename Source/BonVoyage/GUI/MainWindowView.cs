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
using UnityEngine;
using UnityEngine.Events;

namespace BonVoyage
{
    /// <summary>
    /// Main mod's window - view part.
    /// For emmbeding in a MultiOptionDialog.
    /// </summary>
    internal class MainWindowView : DialogGUIVerticalLayout
    {
        internal delegate void SettingsCallback();
        internal delegate void ControlCallback();

        private PopupDialog dialog { get; set; }
        private MainWindowModel model;
        private UnityAction closeCallback { get; set; }
        private SettingsCallback settingsCallback { get; set; }
        private ControlCallback controlCallback { get; set; }


        /// <summary>
        /// Constructor
        /// </summary>
        internal MainWindowView(MainWindowModel m, SettingsCallback settings, ControlCallback control, UnityAction close) : base(
            CommonWindowProperties.mainListMinWidth + 20, // min width
            CommonWindowProperties.mainListMinHeight, // min height
            CommonWindowProperties.mainWindowSpacing, // spacing
            CommonWindowProperties.mainElementPadding, // padding
            TextAnchor.UpperLeft // text anchor
        )
        {
            model = m;
            settingsCallback = settings;
            controlCallback = control;
            closeCallback = close;

            // Filter
            AddChild(new DialogGUIHorizontalLayout(
                new DialogGUIToggle(model.GetActiveControllersToggleState(), Localizer.Format("#LOC_BV_ActiveControllers"), model.ActiveControllersChecked, 140f),
                new DialogGUIToggle(model.GetDisabledControllersToggleState(), Localizer.Format("#LOC_BV_DisabledControllers"), model.DisabledControllersChecked, 140f),
                new DialogGUIFlexibleSpace(),
                new DialogGUIButton(model.GetControlButtonText, ToggleControl, model.ControlButtonCanBeEnabled, 150f, CommonWindowProperties.buttonHeight, false, CommonWindowProperties.ActiveSkin.button)
            ));

            // Column headers
            AddChild(new DialogGUIHorizontalLayout(
                new DialogGUISpace(0f),
                new DialogGUILabel(Localizer.Format("#LOC_BV_ColumnHeader_Name"), 150f) { guiStyle = CommonWindowProperties.Style_Label_Bold_Left },
                new DialogGUISpace(10f),
                new DialogGUILabel(Localizer.Format("#LOC_BV_ColumnHeader_Status"), 70f) { guiStyle = CommonWindowProperties.Style_Label_Bold_Center },
                new DialogGUISpace(10f),
                new DialogGUILabel(Localizer.Format("#LOC_BV_ColumnHeader_Body"), 60f) { guiStyle = CommonWindowProperties.Style_Label_Bold_Center },
                new DialogGUISpace(10f),
                new DialogGUILabel(Localizer.Format("#LOC_BV_ColumnHeader_Speed"), 60f) { guiStyle = CommonWindowProperties.Style_Label_Bold_Center },
                new DialogGUISpace(10f),
                new DialogGUILabel(Localizer.Format("#LOC_BV_ColumnHeader_Distance"), 90f) { guiStyle = CommonWindowProperties.Style_Label_Bold_Center }
            ));

            AddChild(new DialogGUIScrollList(new Vector2(CommonWindowProperties.mainListMinWidth, CommonWindowProperties.mainListMinHeight + 220), false, true, model.GetVesselListLayout()));
        }


        /// <summary>
        /// Settings callback encapsulation
        /// </summary>
        private void ShowSettings()
        {
            settingsCallback();
        }


        /// <summary>
        /// Control callback encapsulation
        /// </summary>
        private void ToggleControl()
        {
            controlCallback();
        }


        #region Window geometry
        private static Rect geometry
        {
            get
            {
                Vector2 pos = CommonWindowProperties.MainWindowPosition;
                return new Rect(pos.x, pos.y, CommonWindowProperties.mainWindowWidth, CommonWindowProperties.mainWindowHeight);
            }
            set
            {
                CommonWindowProperties.MainWindowPosition = new Vector2(value.x, value.y);
                Configuration.MainWindowPosition = CommonWindowProperties.MainWindowPosition;
            }
        }


        private Rect currentGeometry
        {
            get
            {
                Vector3 rt = dialog.GetComponent<RectTransform>().position;
                return new Rect(
                    rt.x / GameSettings.UI_SCALE / Screen.width + 0.5f,
                    rt.y / GameSettings.UI_SCALE / Screen.height + 0.5f,
                    CommonWindowProperties.mainWindowWidth, CommonWindowProperties.mainWindowHeight);
            }
        }


        /// <summary>
        /// Get position of the window
        /// </summary>
        /// <returns></returns>
        internal Vector3 GetWindowPosition()
        {
            return dialog.GetComponent<RectTransform>().position;
        }
        #endregion


        /// <summary>
        /// Show dialog window
        /// </summary>
        /// <returns></returns>
        internal PopupDialog Show()
        {
            if (dialog == null)
            {
                dialog = PopupDialog.SpawnPopupDialog(
                    CommonWindowProperties.mainWindowAnchorMin, // min anchor
                    CommonWindowProperties.mainWindowAnchorMax, // max anchor
                    new MultiOptionDialog(
                        "BVMainWindow", // name
                        "", // message
                        Localizer.Format("#LOC_BV_Title"), // title
                        CommonWindowProperties.ActiveSkin, // skin
                        geometry, // position and size
                        this // dialog layout
                    ),
                    false, // persist across scenes
                    CommonWindowProperties.ActiveSkin, // skin
                    false // is modal
                );

                CommonWindowProperties.AddFloatingButton(
                    dialog.transform,
                    -CommonWindowProperties.mainElementPadding.right - CommonWindowProperties.mainWindowSpacing, -CommonWindowProperties.mainElementPadding.top,
                    CommonWindowProperties.ActiveSkin.button,
                    "X",
                    Localizer.Format("#LOC_BV_Close"),
                    closeCallback
                );

                CommonWindowProperties.AddFloatingButton(
                    dialog.transform,
                    -CommonWindowProperties.mainElementPadding.right - 3 * CommonWindowProperties.mainWindowSpacing - CommonWindowProperties.buttonIconWidth, -CommonWindowProperties.mainElementPadding.top,
                    CommonWindowProperties.ActiveSkin.button,
                    "S",
                    Localizer.Format("#LOC_BV_Settings"),
                    ShowSettings
                );

                CommonWindowProperties.AddFloatingButton(
                    dialog.transform,
                    -CommonWindowProperties.mainElementPadding.right - 5 * CommonWindowProperties.mainWindowSpacing - 2 * CommonWindowProperties.buttonIconWidth, -CommonWindowProperties.mainElementPadding.top,
                    CommonWindowProperties.ActiveSkin.button,
                    "R",
                    Localizer.Format("#LOC_BV_Reload"),
                    model.ReloadVesselList
                );
            }

            return dialog;
        }


        /// <summary>
        /// Dismiss dialog window
        /// </summary>
        internal void Dismiss()
        {
            if (dialog != null)
            {
                Rect g = currentGeometry;
                geometry = new Rect(g.x, g.y, CommonWindowProperties.mainWindowWidth, CommonWindowProperties.mainWindowHeight);

                dialog.Dismiss();
                dialog = null;
                model.ClearVesselListLayout();
            }
        }

    }

}
