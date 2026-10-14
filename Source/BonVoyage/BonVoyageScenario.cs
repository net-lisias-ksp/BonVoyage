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

namespace BonVoyage
{
    class BonVoyageScenario : ScenarioModule
    {
        public static BonVoyageScenario Instance { get; private set; }

        private ConfigNode scenarioNode;


        /// <summary>
        /// Constructor
        /// </summary>
        public BonVoyageScenario()
        {
            Instance = this;
        }


        /// <summary>
        /// Load data
        /// </summary>
        /// <param name="gameNode"></param>
        public override void OnLoad(ConfigNode gameNode)
        {
            base.OnLoad(gameNode);
            scenarioNode = gameNode;
        }


        /// <summary>
        /// Save data
        /// </summary>
        /// <param name="gameNode"></param>
        public override void OnSave(ConfigNode gameNode)
        {
            base.OnSave(gameNode);
            SaveScenario(gameNode);
        }


        /// <summary>
        /// Save scenario details for each vessel with BV controller
        /// </summary>
        /// <param name="node"></param>
        private void SaveScenario(ConfigNode gameNode)
        {
            // BonVoyage only starts at the Space Center. A save loaded straight into flight is saved
            // before that, and throwing here aborts the whole save (and the scene change with it).
            // Keep what was loaded until BonVoyage is up.
            if (BonVoyage.Instance == null)
            {
                if (scenarioNode != null && scenarioNode != gameNode)
                {
                    gameNode.ClearNodes();
                    foreach (ConfigNode controllerNode in scenarioNode.GetNodes("CONTROLLER"))
                        gameNode.AddNode(controllerNode);
                }
                return;
            }

            gameNode.ClearNodes();

			foreach (BVController controller in BonVoyage.Instance.BVControllers.Values)
			{
                if (controller.vessel == null)
                    continue;

                ConfigNode controllerNode = new ConfigNode("CONTROLLER");
                controllerNode.AddValue("vesselId", controller.vessel.id);

				controllerNode.AddValue("electricPower_Solar", controller.electricPower_Solar);
				controllerNode.AddValue("electricPower_Other", controller.electricPower_Other);
				controllerNode.AddValue("requiredPower", controller.requiredPower);

				controller.solarPower.Write(controllerNode);
				controller.batteries.Write(controllerNode);
				controller.fuelEnergy.Write(controllerNode);

                gameNode.AddNode(controllerNode);
            }
        }


        /// <summary>
        /// Load scenario details for each vessel with BV controller
        /// </summary>
        public void LoadScenario()
        {
            if (scenarioNode != null)
            {
				foreach (BVController controller in BonVoyage.Instance.BVControllers.Values)
                {
                    if (controller.vessel == null)
                        continue;

                    ConfigNode controllerNode = scenarioNode.GetNode("CONTROLLER", "vesselId", controller.vessel.id.ToString());
                    if (controllerNode != null)
                    {
						controller.electricPower_Solar = Convert.ToDouble(controllerNode.GetValue("electricPower_Solar") ?? "0");
						controller.electricPower_Other = Convert.ToDouble(controllerNode.GetValue("electricPower_Other") ?? "0");
						controller.requiredPower = Convert.ToDouble(controllerNode.GetValue("requiredPower") ?? "0");

						controller.solarPower.Read(controllerNode);
						controller.batteries.Read(controllerNode);
						controller.fuelEnergy.Read(controllerNode);
                    }
                }
            }
        }

    }

}
