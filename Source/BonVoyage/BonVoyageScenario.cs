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
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
            gameNode.ClearNodes();

			foreach (BVController controller in BonVoyage.Instance.BVControllers.Values)
			{
                if (controller.vessel == null)
                    continue;

                ConfigNode controllerNode = new ConfigNode("CONTROLLER");
                controllerNode.AddValue("vesselId", controller.vessel.id);

                ConfigNode subNode = new ConfigNode("BATTERIES");
                subNode.AddValue("useBatteries", controller.batteries.UseBatteries);
                subNode.AddValue("maxUsedEC", controller.batteries.MaxUsedEC);
                subNode.AddValue("ecPerSecondConsumed", controller.batteries.ECPerSecondConsumed);
                subNode.AddValue("ecPerSecondGenerated", controller.batteries.ECPerSecondGenerated);
                subNode.AddValue("currentEC", controller.batteries.CurrentEC);
                controllerNode.AddNode(subNode);

                subNode = new ConfigNode("FUEL_CELLS");
                subNode.AddValue("useFuelCells", controller.fuelCells.Use);
                subNode.AddValue("outputEC", controller.fuelCells.OutputValue);
                List<Resource> res = controller.fuelCells.InputResources;
                ConfigNode resourceNode;
                for (int r = 0; r < res.Count; r++)
                {
                    resourceNode = new ConfigNode("RESOURCE");
                    resourceNode.AddValue("name", res[r].Name);
                    resourceNode.AddValue("ratio", res[r].Ratio);
                    resourceNode.AddValue("maximumAmount", res[r].MaximumAmountAvailable);
                    resourceNode.AddValue("currentAmount", res[r].CurrentAmountUsed);
                    subNode.AddNode(resourceNode);
                }
                controllerNode.AddNode(subNode);

                subNode = new ConfigNode("PROPELLANTS");
                List<Fuel> props = controller.propellants;
                ConfigNode propellantNode;
                for (int r = 0; r < props.Count; r++)
                {
                    propellantNode = new ConfigNode("FUEL");
                    propellantNode.AddValue("name", props[r].Name);
                    propellantNode.AddValue("fuelFlow", props[r].FuelFlow);
                    propellantNode.AddValue("maximumAmount", props[r].MaximumAmountAvailable);
                    propellantNode.AddValue("currentAmount", props[r].CurrentAmountUsed);
                    subNode.AddNode(propellantNode);
                }
                controllerNode.AddNode(subNode);

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
                        ConfigNode subNode = controllerNode.GetNode("BATTERIES");
                        if (subNode != null)
                        {
                            controller.batteries.UseBatteries = Convert.ToBoolean(subNode.GetValue("useBatteries"));
                            controller.batteries.MaxUsedEC = Convert.ToDouble(subNode.GetValue("maxUsedEC"));
                            controller.batteries.ECPerSecondConsumed = Convert.ToDouble(subNode.GetValue("ecPerSecondConsumed"));
                            controller.batteries.ECPerSecondGenerated = Convert.ToDouble(subNode.GetValue("ecPerSecondGenerated"));
                            controller.batteries.CurrentEC = Convert.ToDouble(subNode.GetValue("currentEC"));
                        }

                        subNode = controllerNode.GetNode("FUEL_CELLS");
                        if (subNode != null)
                        {
                            controller.fuelCells.Use = Convert.ToBoolean(subNode.GetValue("useFuelCells"));
                            controller.fuelCells.OutputValue = Convert.ToDouble(subNode.GetValue("outputEC"));
                            var resources = subNode.GetNodes("RESOURCE");
                            controller.fuelCells.InputResources.Clear();
                            for (int r = 0; r < resources.Length; r++)
                            {
                                Resource ir = new Resource();
                                ir.Name = resources[r].GetValue("name");
                                ir.Ratio = Convert.ToDouble(resources[r].GetValue("ratio"));
                                ir.MaximumAmountAvailable = Convert.ToDouble(resources[r].GetValue("maximumAmount"));
                                ir.CurrentAmountUsed = Convert.ToDouble(resources[r].GetValue("currentAmount"));
                                controller.fuelCells.InputResources.Add(ir);
                            }
                        }

                        subNode = controllerNode.GetNode("PROPELLANTS");
                        if (subNode != null)
                        {
                            var propellants = subNode.GetNodes("FUEL");
                            controller.propellants.Clear();
                            for (int r = 0; r < propellants.Length; r++)
                            {
                                Fuel ir = new Fuel();
                                ir.Name = propellants[r].GetValue("name");
                                ir.FuelFlow = Convert.ToDouble(propellants[r].GetValue("fuelFlow"));
                                ir.MaximumAmountAvailable = Convert.ToDouble(propellants[r].GetValue("maximumAmount"));
                                ir.CurrentAmountUsed = Convert.ToDouble(propellants[r].GetValue("currentAmount"));
                                controller.propellants.Add(ir);
                            }
                        }
                    }
                }
            }
        }

    }

}
