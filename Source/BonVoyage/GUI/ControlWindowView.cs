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
    class ControlWindowView : DialogGUIVerticalLayout
    {
        private PopupDialog dialog { get; set; }
        private ControlWindowModel model;
        private UnityAction closeCallback { get; set; }


        /// <summary>
        /// Constructor
        /// </summary>
        internal ControlWindowView(ControlWindowModel m, UnityAction close) : base(
            CommonWindowProperties.controlMinWidth, // min width
            CommonWindowProperties.controlMinHeight, // min height
            CommonWindowProperties.controlWindowSpacing, // spacing
            CommonWindowProperties.controlElementPadding, // padding
            TextAnchor.UpperLeft // text anchor
        )
        {
            model = m;
            closeCallback = close;

            AddChild(model.GetStatsListLayout());            

            // Set a target section
            DialogGUITextInput latField = new DialogGUITextInput("", false, 20, (string s) => { model.Latitude = s; return s; }, model.GetLatitude, TMPro.TMP_InputField.ContentType.DecimalNumber, CommonWindowProperties.buttonHeight);
            model.AddLockControlToTextField(latField);
            DialogGUITextInput lonField = new DialogGUITextInput("", false, 20, (string s) => { model.Longitude = s; return s; }, model.GetLongitude, TMPro.TMP_InputField.ContentType.DecimalNumber, CommonWindowProperties.buttonHeight);
            model.AddLockControlToTextField(lonField);
            AddChild(new DialogGUILabel(Localizer.Format("#LOC_BV_Control_SetTarget") + ":"));
            AddChild(new DialogGUIHorizontalLayout(TextAnchor.MiddleLeft,
                new DialogGUILabel(Localizer.Format("#LOC_BV_Control_Lat") + ":"),
                latField,
                new DialogGUISpace(2f),
                new DialogGUILabel(Localizer.Format("#LOC_BV_Control_Lon") + ":"),
                lonField,
                new DialogGUIButton(Localizer.Format("#LOC_BV_Control_Set"), model.SetButtonClicked, model.EnableButtons, 40f, CommonWindowProperties.buttonHeight - 4, false)
            ));
            AddChild(new DialogGUIHorizontalLayout(
                 new DialogGUIButton(Localizer.Format("#LOC_BV_Control_PickOnMap"), model.PickOnMapButtonClicked, model.EnableButtons, 90f, CommonWindowProperties.buttonHeight - 4, false),
                 new DialogGUIButton(Localizer.Format("#LOC_BV_Control_CurrenTarget"), model.CurrentTargetButtonClicked, model.EnableButtons, 104f, CommonWindowProperties.buttonHeight - 4, false)
            ));
            AddChild(new DialogGUIHorizontalLayout(
                new DialogGUIButton(Localizer.Format("#LOC_BV_Control_CurrentWaypoint"), model.CurrentWaypointButtonClicked, model.EnableButtons, 124f, CommonWindowProperties.buttonHeight - 4, false)
            ));

            AddChild(new DialogGUISpace(3f));

            AddChild(new DialogGUIHorizontalLayout(
                new DialogGUIFlexibleSpace(),
                TooltipExtension.DeferTooltip(new DialogGUIButton(Localizer.Format("#LOC_BV_Control_Check"), model.SystemCheckButtonClicked, model.EnableButtons, 120f, CommonWindowProperties.buttonHeight, false) { tooltipText = Localizer.Format("#LOC_BV_Control_Check_Tooltip") }),
                new DialogGUIFlexibleSpace()
            ));

            AddChild(new DialogGUISpace(5f));

            AddChild(new DialogGUIHorizontalLayout(
                new DialogGUIFlexibleSpace(),
                new DialogGUIButton(model.GetGoButtonText, model.GoButtonClicked, null, 120f, CommonWindowProperties.buttonHeight + 4, false, CommonWindowProperties.Style_Button_Bold_Yellow),
                new DialogGUIFlexibleSpace()
            ));
        }


        #region Window geometry
        private static Rect geometry
        {
            get
            {
                Vector2 pos = CommonWindowProperties.ControlWindowPosition;
                return new Rect(pos.x, pos.y, CommonWindowProperties.controlWindowWidth, CommonWindowProperties.controlWindowHeight);
            }
            set
            {
                CommonWindowProperties.ControlWindowPosition = new Vector2(value.x, value.y);
                Configuration.ControlWindowPosition = CommonWindowProperties.ControlWindowPosition;
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
                    CommonWindowProperties.controlWindowWidth, CommonWindowProperties.controlWindowHeight);
            }
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
                    CommonWindowProperties.controlWindowAnchorMin, // min anchor
                    CommonWindowProperties.controlWindowAnchorMax, // max anchor
                    new MultiOptionDialog(
                        "BVControlWindow", // name
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
                    -CommonWindowProperties.controlElementPadding.right - CommonWindowProperties.controlWindowSpacing, -CommonWindowProperties.controlElementPadding.top,
                    CommonWindowProperties.ActiveSkin.button,
                    "X",
                    Localizer.Format("#LOC_BV_Close"),
                    closeCallback
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
                geometry = new Rect(g.x, g.y, CommonWindowProperties.controlWindowWidth, CommonWindowProperties.controlWindowHeight);

                dialog.Dismiss();
                dialog = null;
                model.ClearStatsListLayout();
            }
        }

    }

}
