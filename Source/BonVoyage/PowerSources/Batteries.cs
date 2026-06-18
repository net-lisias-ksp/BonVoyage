/*
	This file is part of Bon Voyage /L
		© 2024-2026 LisiasT : http://lisias.net <support@lisias.net>
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

namespace BonVoyage.PowerSources
{
    /// <summary>
    /// Information about batteries
    /// </summary>
    internal class Batteries : PowerSupply
    {
		internal bool AllowNoGeneratedPower; // Allows rovering when there's no active source of EC.
        internal double MaxAvailableEC; // Max EC available from all activated batteries

		internal double UseableECRatio = 0.5; // By default, we are using only half of max available EC;
		internal string UseableECRatioAsText => ((int)(100*this.UseableECRatio)).ToString() + "%";

        internal double MaxUsedEC; // Max EC we can use
        internal double ECPerSecondConsumed; // EC per second consumed by wheels
        internal double ECPerSecondGenerated; // EC per second generated (generated power minus required power)
        internal double CurrentEC; // Current EC status of batteries

		internal override bool PowerIsAvailable => this.Use && (CheatOptions.InfiniteElectricity || this.CurrentEC > 0.1);
		internal override bool PowerIsExhausted => this.Use && (!CheatOptions.InfiniteElectricity && this.CurrentEC <= 0.1);

		internal override void Read(ConfigNode controllerNode)
		{
			ConfigNode subNode = controllerNode.GetNode("BATTERIES");
			if (null == subNode) return;

			this.Use = Convert.ToBoolean(subNode.GetValue("useBatteries"));
			this.AllowNoGeneratedPower = Convert.ToBoolean(subNode.GetValue("allowNoGeneratedPower" ?? "False"));
			this.MaxUsedEC = Convert.ToDouble(subNode.GetValue("maxUsedEC"));
			this.UseableECRatio = Convert.ToDouble(subNode.GetValue("useableECRatio") ?? "0.5"); // By default, we are using only half of max available EC
			this.ECPerSecondConsumed = Convert.ToDouble(subNode.GetValue("ecPerSecondConsumed"));
			this.ECPerSecondGenerated = Convert.ToDouble(subNode.GetValue("ecPerSecondGenerated"));
			this.CurrentEC = Convert.ToDouble(subNode.GetValue("currentEC"));
		}

		internal override void Write(ConfigNode controllerNode)
		{
			ConfigNode subNode = new ConfigNode("BATTERIES");

			subNode.AddValue("useBatteries", this.Use);
			subNode.AddValue("allowNoGeneratedPower", this.AllowNoGeneratedPower);
			subNode.AddValue("maxUsedEC", this.MaxUsedEC);
			subNode.AddValue("useableECRatio", this.UseableECRatio);
			subNode.AddValue("ecPerSecondConsumed", this.ECPerSecondConsumed);
			subNode.AddValue("ecPerSecondGenerated", this.ECPerSecondGenerated);
			subNode.AddValue("currentEC", this.CurrentEC);

			controllerNode.AddNode(subNode);
		}

		internal override double GetAvailablePower() => this.CurrentEC;
	}
}
