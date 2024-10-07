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
    /// <summary>
    /// Wrapper for communication with Kopernicus assemblies
    /// </summary>
    internal static class KopernicusWrapper
    {
        private static int kopernicusComponentsIndex = -1; // Index of Kopernicus.Components assembly in AssemblyLoader.loadedAssemblies
        private static Type kopernicusStar = null; // KopernicusStar type


        /// <summary>
        /// Get KopernicusStar type from Kopernicus.Components
        /// </summary>
        /// <returns></returns>
        private static Type GetKopernicusStar()
        {
            if (kopernicusComponentsIndex == -1)
                kopernicusComponentsIndex = Tools.GetAssemblyIndex("Kopernicus.Components");
            if ((kopernicusComponentsIndex != -1) && (kopernicusStar == null))
                kopernicusStar = AssemblyLoader.loadedAssemblies[kopernicusComponentsIndex].assembly.GetType("Kopernicus.Components.KopernicusStar");
            return kopernicusStar;
        }


        /// <summary>
        /// Get name of the current star
        /// </summary>
        /// <returns></returns>
        internal static string GetCurrentStarName()
        {
            string name = "";
            
            if (GetKopernicusStar() != null)
            {
                FieldInfo currentStar = kopernicusStar.GetField("Current", BindingFlags.Public | BindingFlags.Static);
                if (currentStar != null)
                    name = ((Sun)currentStar.GetValue(null)).name;
            }

            return name;
        }

    }

}
