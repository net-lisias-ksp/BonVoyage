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
using System.Collections.Generic;

namespace BonVoyage
{
    /// <summary>
    /// Informations about resource for a converter
    /// </summary>
    internal class Resource
    {
        internal string Name; // Name of the resource
        internal double Ratio; // Consumption per second
        internal double MaximumAmountAvailable; // Maximum amout of the resource available for usage
        internal double CurrentAmountUsed; // Current amout of the resource used by a converter
    }


    /// <summary>
    /// Class for fuel cells and engines
    /// </summary>
    internal class Converter
    {
        internal bool Use; // Use converter
        internal double OutputValue; // Output value for any output resource (e.g. EC for fuel cells)
        internal List<Resource> InputResources = new List<Resource>();
    }

}
