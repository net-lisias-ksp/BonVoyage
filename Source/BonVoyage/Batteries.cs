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
    /// Information about batteries
    /// </summary>
    internal class Batteries
    {
        internal bool UseBatteries; // Use batteries during a night
        internal double MaxAvailableEC; // Max EC available from all activated batteries
        internal double MaxUsedEC; // Max EC we can use
        internal double ECPerSecondConsumed; // EC per second consumed by wheels
        internal double ECPerSecondGenerated; // EC per second generated (generated power minus required power)
        internal double CurrentEC; // Current EC status of batteries
    }

}
