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
using TMPro;

namespace BonVoyage
{
    /// <summary>
    /// Settings window - model part
    /// </summary>
    internal class SettingsWindowModel
    {
        private bool dewarpChecked = false;
        private bool disableRotation = false;
        private bool showBiome = false;
        private bool kspSkin = true;
        private bool kspToolbarChecked = true;
        private bool toolbarContinuedChecked = false;
        private double heightOffset = 0f;
        internal string HeightOffset
        {
            get { return heightOffset.ToString(); }
            set
            {
                if ((value.Length == 0) || (value == "."))
                    heightOffset = 0f;
                else
                    heightOffset = Convert.ToDouble(value);
                Configuration.HeightOffset = heightOffset;
            }
        }


        /// <summary>
        /// Constructor
        /// </summary>
        internal SettingsWindowModel()
        {
            // Load from configuration
            if (Configuration.Skin == 1)
                kspSkin = false;
            else
                kspSkin = true;
            dewarpChecked = Configuration.AutomaticDewarp;
            disableRotation = Configuration.DisableRotation;
            showBiome = Configuration.ShowBiome;
            kspToolbarChecked = Configuration.KSPToolbar;
            toolbarContinuedChecked = Configuration.ToolbarContinued;
            heightOffset = Configuration.HeightOffset;
        }


        /// <summary>
        /// Automatic dewarp checkbox
        /// </summary>
        /// <param name="value"></param>
        internal void DewarpChecked(bool value)
        {
            dewarpChecked = value;
            Configuration.AutomaticDewarp = value;
        }


        /// <summary>
        /// Get the state of Autmatic dewarp toggle
        /// </summary>
        /// <returns></returns>
        internal bool GetDewarpToggleState()
        {
            return dewarpChecked;
        }


        /// <summary>
        /// Disable rotation checkbox
        /// </summary>
        /// <param name="value"></param>
        internal void DisableRotationChecked(bool value)
        {
            disableRotation = value;
            Configuration.DisableRotation = value;
        }


        /// <summary>
        /// Get the state of disable rotation toggle
        /// </summary>
        /// <returns></returns>
        internal bool GeDisableRotationToggleState()
        {
            return disableRotation;
        }


        /// <summary>
        /// Show biome checkbox
        /// </summary>
        /// <param name="value"></param>
        internal void ShowBiomeChecked(bool value)
        {
            showBiome = value;
            Configuration.ShowBiome = value;
        }


        /// <summary>
        /// Get the state of show biome toggle
        /// </summary>
        /// <returns></returns>
        internal bool GeShowBiomeToggleState()
        {
            return showBiome;
        }


        /// <summary>
        /// Change active skin
        /// </summary>
        private void ChangeSkin()
        {
            if (kspSkin)
            {
                CommonWindowProperties.ActiveSkin = UISkinManager.defaultSkin;
                Configuration.Skin = 0;
            }
            else
            {
                CommonWindowProperties.ActiveSkin = CommonWindowProperties.UnitySkin;
                Configuration.Skin = 1;
            }
            CommonWindowProperties.RefreshStyles();
            BonVoyage.Instance.ResetWindows();
        }


        /// <summary>
        /// KSP skin checkbox
        /// </summary>
        /// <param name="value"></param>
        internal void KSPSkinChecked(bool value)
        {
            kspSkin = value;
            ChangeSkin();
        }


        /// <summary>
        /// Unity skin checkbox
        /// </summary>
        /// <param name="value"></param>
        internal void UnitySkinChecked(bool value)
        {
            kspSkin = !value;
            ChangeSkin();
        }


        /// <summary>
        /// Get the state of KSP skin toggle
        /// </summary>
        /// <returns></returns>
        internal bool GetKSPSkinToggleState()
        {
            return kspSkin;
        }


        /// <summary>
        /// Get the state of Unity skin toggle
        /// </summary>
        /// <returns></returns>
        internal bool GetUnitySkinToggleState()
        {
            return !kspSkin;
        }


        /// <summary>
        /// KSP toolbar checkbox
        /// </summary>
        /// <param name="value"></param>
        internal void KSPToolbarChecked(bool value)
        {
            kspToolbarChecked = value;
            Configuration.KSPToolbar = value;

            if (kspToolbarChecked)
                BonVoyage.Instance.AddLauncher();
            else
                BonVoyage.Instance.RemoveAppLauncherButton();
        }


        /// <summary>
        /// Toolbar Continued checkbox
        /// </summary>
        /// <param name="value"></param>
        internal void TCChecked(bool value)
        {
            toolbarContinuedChecked = value;
            Configuration.ToolbarContinued = value;

            if (toolbarContinuedChecked)
                BonVoyage.Instance.AddLauncher();
            else
                BonVoyage.Instance.RemoveToolbarContinuedButton();
        }


        /// <summary>
        /// Get the state of KSP toolbar toggle
        /// </summary>
        /// <returns></returns>
        internal bool GetKSPToolbarToggleState()
        {
            return kspToolbarChecked;
        }


        /// <summary>
        /// Get the state of Toolbar Continued toggle
        /// </summary>
        /// <returns></returns>
        internal bool GetTCToggleState()
        {
            return toolbarContinuedChecked;
        }


        /// <summary>
        /// Add control lock/unlock listeners to a text field
        /// </summary>
        /// <param name="text"></param>
        private void TMPFieldOnSelect(string text)
        {
            InputLockManager.SetControlLock(ControlTypes.KEYBOARDINPUT | ControlTypes.UI, "BonVoyageInputFieldLock");
        }
        private void TMPFieldOnDeselect(string text)
        {
            InputLockManager.RemoveControlLock("BonVoyageInputFieldLock");
        }
        internal void AddLockControlToTextField(DialogGUITextInput field)
        {
            field.OnUpdate = () => {
                if (field.uiItem != null)
                {
                    field.OnUpdate = () => { };
                    TMP_InputField TMPField = field.uiItem.GetComponent<TMP_InputField>();
                    TMPField.onSelect.AddListener(TMPFieldOnSelect);
                    TMPField.onDeselect.AddListener(TMPFieldOnDeselect);
                }
            };
        }


        /// <summary>
        ///  Return saved height offset
        /// </summary>
        /// <returns></returns>
        internal string GetHeightOffset()
        {
            return HeightOffset;
        }

    }

}
