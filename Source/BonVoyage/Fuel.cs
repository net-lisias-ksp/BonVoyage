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

namespace BonVoyage
{
    /// <summary>
    /// Informations about propellant for engines
    /// </summary>
    internal class Fuel
    {
        internal string Name; // Name of the propellant
        internal double FuelFlow; // Consumption per second
        internal double MaximumAmountAvailable; // Maximum amout of the propellant available for usage
        internal double CurrentAmountUsed; // Current amout of the propellant used by engines
    }
}
