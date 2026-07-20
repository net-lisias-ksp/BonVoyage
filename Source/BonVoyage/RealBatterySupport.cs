/*
	This file is part of Bon Voyage /L

	Contributed by Rjoande (RealBattery Recharged) to add optional interoperability with
	RealBattery — no effect whatsoever when RealBattery is not installed.

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
    /// Detectable marker for third-party mods that integrate with BonVoyage's battery
    /// accounting (currently: RealBattery). A third party's own compatibility layer
    /// (e.g. RealBattery's optional Harmony bridge) can check <see cref="Enabled"/> at
    /// startup and skip applying its own patches when native support like this is already
    /// present, avoiding double patching.
    /// </summary>
    public static class RealBatterySupport
    {
        public const bool Enabled = true;

        /// <summary>
        /// RealBattery's StoredCharge resource (kWh-equivalent) is converted to BonVoyage's
        /// internal EC accounting (an instantaneous, non-persisted unit) at a fixed 1:3600
        /// ratio, agreed with RealBattery's own bridge/background-simulation conventions.
        /// </summary>
        internal const double EcPerSc = 3600.0;
    }
}
