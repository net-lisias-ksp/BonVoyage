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
using KSP.IO;
using UnityEngine;

namespace BonVoyage
{
    /// <summary>
    /// Configuration of the mod
    /// </summary>
    internal static class Configuration
    {
        private static PluginConfiguration configuration; // Configuration data


        #region internal properties

        /// <summary>
        /// First run
        /// </summary>
        internal static bool FirstRun
        {
            get { return configuration.GetValue<bool>("firstRun", true); }
            set
            {
                configuration.SetValue("firstRun", value);
                configuration.save();
            }
        }


        /// <summary>
        /// UI skin
        /// </summary>
        internal static byte Skin
        {
            get { return configuration.GetValue<byte>("skin", 0); }
            set
            {
                configuration.SetValue("skin", value);
                configuration.save();
            }
        }


        /// <summary>
        /// Automatic dewarp
        /// </summary>
        internal static bool AutomaticDewarp
        {
            get { return configuration.GetValue<bool>("dewarp", false); }
            set
            {
                configuration.SetValue("dewarp", value);
                configuration.save();
            }
        }


        /// <summary>
        /// Disable rotation
        /// </summary>
        internal static bool DisableRotation
        {
            get { return configuration.GetValue<bool>("disableRotation", false); }
            set
            {
                configuration.SetValue("disableRotation", value);
                configuration.save();
            }
        }


        /// <summary>
        /// Use KSP toolbar
        /// </summary>
        internal static bool KSPToolbar
        {
            get { return configuration.GetValue<bool>("kspToolbar", true); }
            set
            {
                configuration.SetValue("kspToolbar", value);
                configuration.save();
            }
        }


        /// <summary>
        /// Use Toolbar Continued
        /// </summary>
        internal static bool ToolbarContinued
        {
            get { return configuration.GetValue<bool>("toolbarContinued", false); }
            set
            {
                configuration.SetValue("toolbarContinued", value);
                configuration.save();
            }
        }


        /// <summary>
        /// Main window position
        /// </summary>
        internal static Vector2 MainWindowPosition
        {
            get { return configuration.GetValue<Vector2>("mainWindow", new Vector2(0.5f, 0.5f)); }
            set
            {
                configuration.SetValue("mainWindow", value);
                configuration.save();
            }
        }


        /// <summary>
        /// Control window position
        /// </summary>
        internal static Vector2 ControlWindowPosition
        {
            get { return configuration.GetValue<Vector2>("controlWindow", new Vector2(0.5f, 0.5f)); }
            set
            {
                configuration.SetValue("controlWindow", value);
                configuration.save();
            }
        }


        /// <summary>
        /// Active vessels
        /// </summary>
        internal static bool ActiveControllers
        {
            get { return configuration.GetValue<bool>("activeControllers", true); }
            set
            {
                configuration.SetValue("activeControllers", value);
                configuration.save();
            }
        }


        /// <summary>
        /// Disabled vessels
        /// </summary>
        internal static bool DisabledControllers
        {
            get { return configuration.GetValue<bool>("disabledControllers", false); }
            set
            {
                configuration.SetValue("disabledControllers", value);
                configuration.save();
            }
        }


        /// <summary>
        /// Show biome on map
        /// </summary>
        internal static bool ShowBiome
        {
            get { return configuration.GetValue<bool>("showBiome", false); }
            set
            {
                configuration.SetValue("showBiome", value);
                configuration.save();
            }
        }


        /// <summary>
        /// Show biome on map
        /// </summary>
        internal static double PathfinderTimer
        {
            get { return configuration.GetValue<double>("pathfinderTimer", 10); }
            set
            {
                configuration.SetValue("pathfinderTimer", value);
                configuration.save();
            }
        }


        /// <summary>
        /// Height offset
        /// </summary>
        internal static double HeightOffset
        {
            get { return configuration.GetValue<double>("heightOffset", 0); }
            set
            {
                configuration.SetValue("heightOffset", value);
                configuration.save();
            }
        }

        #endregion


        /// <summary>
        /// Load configuration from file
        /// </summary>
        internal static void Load()
        {
            configuration = PluginConfiguration.CreateForType<BonVoyage>();
            configuration.load();
        }


        /// <summary>
        /// Save configuration to file
        /// </summary>
        internal static void Save()
        {
            configuration.save();
        }

    }

}
