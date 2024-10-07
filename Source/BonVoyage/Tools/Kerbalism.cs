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
using System.Reflection;

namespace BonVoyage
{
    public class DetectKerbalism
    {
        private static bool didScan = false;
        private static bool kerbalismFound = false;

        public static bool Found()
        {
            if (didScan)
                return kerbalismFound;

            foreach (var a in AssemblyLoader.loadedAssemblies)
            {
                // Kerbalism comes with more than one assembly. There is Kerbalism for debug builds, KerbalismBootLoader,
                // then there are Kerbalism18 or Kerbalism16_17 depending on the KSP version, and there might be ohter
                // assemblies like KerbalismContracts etc.
                // So look at the assembly name object instead of the assembly name (which is the file name and could be renamed).

                AssemblyName nameObject = new AssemblyName(a.assembly.FullName);
                string realName = nameObject.Name; // Will always return "Kerbalism" as defined in the AssemblyName property of the csproj

                if (realName.Equals("Kerbalism"))
                {
                    kerbalismFound = true;
                    break;
                }
            }

            didScan = true;
            return kerbalismFound;
        }
    }

    /// <summary>
    /// Helper function to interact with proto modules of unloaded vessels. You will need this a lot.
    /// </summary>
    public static class Proto
    {
        public static bool GetBool(ProtoPartModuleSnapshot m, string name, bool def_value = false)
        {
            bool v;
            string s = m.moduleValues.GetValue(name);
            return s != null && bool.TryParse(s, out v) ? v : def_value;
        }

        public static uint GetUInt(ProtoPartModuleSnapshot m, string name, uint def_value = 0)
        {
            uint v;
            string s = m.moduleValues.GetValue(name);
            return s != null && uint.TryParse(s, out v) ? v : def_value;
        }

        public static int GetInt(ProtoPartModuleSnapshot m, string name, int def_value = 0)
        {
            int v;
            string s = m.moduleValues.GetValue(name);
            return s != null && int.TryParse(s, out v) ? v : def_value;
        }

        public static float GetFloat(ProtoPartModuleSnapshot m, string name, float def_value = 0.0f)
        {
            // note: we set NaN and infinity values to zero, to cover some weird inter-mod interactions
            float v;
            string s = m.moduleValues.GetValue(name);
            return s != null && float.TryParse(s, out v) && !float.IsNaN(v) && !float.IsInfinity(v) ? v : def_value;
        }

        public static double GetDouble(ProtoPartModuleSnapshot m, string name, double def_value = 0.0)
        {
            // note: we set NaN and infinity values to zero, to cover some weird inter-mod interactions
            double v;
            string s = m.moduleValues.GetValue(name);
            return s != null && double.TryParse(s, out v) && !double.IsNaN(v) && !double.IsInfinity(v) ? v : def_value;
        }

        public static string GetString(ProtoPartModuleSnapshot m, string name, string def_value = "")
        {
            string s = m.moduleValues.GetValue(name);
            return s ?? def_value;
        }

        public static T GetEnum<T>(ProtoPartModuleSnapshot m, string name, T def_value)
        {
            string s = m.moduleValues.GetValue(name);
            if (s != null && Enum.IsDefined(typeof(T), s))
            {
                T forprofiling = (T)Enum.Parse(typeof(T), s);
                UnityEngine.Profiling.Profiler.EndSample();
                return forprofiling;
            }
            return def_value;
        }

        public static T GetEnum<T>(ProtoPartModuleSnapshot m, string name)
        {
            string s = m.moduleValues.GetValue(name);
            if (s != null && Enum.IsDefined(typeof(T), s))
                return (T)Enum.Parse(typeof(T), s);
            return (T)Enum.GetValues(typeof(T)).GetValue(0);
        }

        ///<summary>set a value in a proto module</summary>
        public static void Set<T>(ProtoPartModuleSnapshot module, string value_name, T value)
        {
            module.moduleValues.SetValue(value_name, value.ToString(), true);
        }
    }
}
