using KSP.Localization;
using UnityEngine;
using UnityEngine.Events;

namespace BonVoyage
{
    /// <summary>
    /// Settings window - view part.
    /// For emmbeding in a MultiOptionDialog.
    /// </summary>
    internal class SettingsWindowView : DialogGUIVerticalLayout
    {
        private PopupDialog dialog { get; set; }
        private SettingsWindowModel model;
        private UnityAction closeCallback { get; set; }
        private static Vector3 mainWindowPosition;


        /// <summary>
        /// Constructor
        /// </summary>
        internal SettingsWindowView(SettingsWindowModel m, Vector3 mwp, UnityAction close) : base(
            CommonWindowProperties.settingsMinWidth, // min width
            CommonWindowProperties.settingsMinHeight, // min height
            CommonWindowProperties.settingsWindowSpacing, // spacing
            CommonWindowProperties.settingsElementPadding, // padding
            TextAnchor.UpperLeft // text anchor
        )
        {
            model = m;
            mainWindowPosition = mwp;
            closeCallback = close;

            AddChild(TooltipExtension.DeferTooltip(new DialogGUIToggle(model.GetDewarpToggleState(), Localizer.Format("#LOC_BV_AutomaticDewarp"), model.DewarpChecked, 130f) { tooltipText = Localizer.Format("#LOC_BV_AutomaticDewarp_Tooltip") }));
            AddChild(TooltipExtension.DeferTooltip(new DialogGUIToggle(model.GeDisableRotationToggleState(), Localizer.Format("#LOC_BV_DisableRotation"), model.DisableRotationChecked, 130f) { tooltipText = Localizer.Format("#LOC_BV_DisableRotation_Tooltip") }));
            AddChild(TooltipExtension.DeferTooltip(new DialogGUIToggle(model.GeShowBiomeToggleState(), Localizer.Format("#LOC_BV_ShowBiome"), model.ShowBiomeChecked, 130f) { tooltipText = Localizer.Format("#LOC_BV_ShowBiome_Tooltip") }));

            AddChild(new DialogGUIBox("", 140f, 80f, null,
                new DialogGUIVerticalLayout(140f, 80f, CommonWindowProperties.settingsWindowSpacing, CommonWindowProperties.boxPadding, TextAnchor.UpperLeft,
                    new DialogGUILabel(Localizer.Format("#LOC_BV_Style"), 135f),
                    new DialogGUISpace(10f),
                    new DialogGUIToggleGroup(
                        new DialogGUIToggle(model.GetKSPSkinToggleState(), Localizer.Format("#LOC_BV_Style_KSP"), model.KSPSkinChecked, 135f),
                        new DialogGUIToggle(model.GetUnitySkinToggleState(), Localizer.Format("#LOC_BV_Style_Unity"), model.UnitySkinChecked, 135f)
                    )
                )
            ));

            AddChild(new DialogGUISpace(4f));

            AddChild(new DialogGUIBox("", 140f, 80f, null,
                new DialogGUIVerticalLayout(140f, 80f, CommonWindowProperties.settingsWindowSpacing, CommonWindowProperties.boxPadding, TextAnchor.UpperLeft,
                    new DialogGUILabel(Localizer.Format("#LOC_BV_Toolbar"), 135f),
                    new DialogGUISpace(10f),
                    TooltipExtension.DeferTooltip(new DialogGUIToggle(model.GetKSPToolbarToggleState(), Localizer.Format("#LOC_BV_Toolbar_KSP"), model.KSPToolbarChecked, 135f) { tooltipText = Localizer.Format("#LOC_BV_Toolbar_KSP_Tooltip") }),
                    TooltipExtension.DeferTooltip(new DialogGUIToggle(model.GetTCToggleState(), Localizer.Format("#LOC_BV_Toolbar_TC"), model.TCChecked, 135f) { tooltipText = Localizer.Format("#LOC_BV_Toolbar_TC_Tooltip") })
                )
            ));

            AddChild(new DialogGUISpace(4f));

            DialogGUITextInput heightOffsetField = new DialogGUITextInput("", false, 20, (string s) => { model.HeightOffset = s; return s; }, model.GetHeightOffset, TMPro.TMP_InputField.ContentType.DecimalNumber, CommonWindowProperties.buttonHeight);
            model.AddLockControlToTextField(heightOffsetField);
            AddChild(new DialogGUIVerticalLayout(
                new DialogGUILabel(Localizer.Format("#LOC_BV_HeightOffset")),
                new DialogGUIHorizontalLayout(TextAnchor.MiddleLeft,
                    heightOffsetField,
                    new DialogGUILabel(Localizer.Format("m")))
            ));
        }


        #region Window geometry
        private static Rect geometry
        {
            get
            {
                return new Rect(
                    (mainWindowPosition.x / GameSettings.UI_SCALE + CommonWindowProperties.mainWindowWidth / 2 + CommonWindowProperties.settingsWindowWidth / 2) / Screen.width + 0.5f,
                    (mainWindowPosition.y / GameSettings.UI_SCALE + CommonWindowProperties.mainWindowHeight / 2 - CommonWindowProperties.settingsWindowHeight / 2 - 11) / Screen.height + 0.5f, 
                    CommonWindowProperties.settingsWindowWidth, CommonWindowProperties.settingsWindowHeight);
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
                    CommonWindowProperties.settingsWindowAnchorMin, // min anchor
                    CommonWindowProperties.settingsWindowAnchorMax, // max anchor
                    new MultiOptionDialog(
                        "BVSettingsWindow", // name
                        "", // message
                        Localizer.Format("#LOC_BV_Settings"), // title
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
                    -CommonWindowProperties.settingsElementPadding.right - CommonWindowProperties.settingsWindowSpacing, -CommonWindowProperties.settingsElementPadding.top,
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
                dialog.Dismiss();
                dialog = null;
            }
        }

    }

}
