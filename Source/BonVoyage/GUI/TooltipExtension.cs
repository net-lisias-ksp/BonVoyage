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
using UnityEngine;
using KSP.UI.TooltipTypes;

namespace BonVoyage
{
    /// <summary>
    /// Extensions for a tooltip of DialogGUI* components
    /// </summary>
    static class TooltipExtension
    {
        private static readonly Tooltip_TitleAndText titleAndTextTooltipPrefab = AssetBase.GetPrefab<Tooltip_TitleAndText>("Tooltip_TitleAndText");

        /// <summary>
        /// Create a tooltip object for a given GameObject, containing both
        /// a title and a subtitle.
        /// </summary>
        /// <param name="gameObj">GameObject to which we want to add a tooltip</param>
        /// <param name="title">Highlighted text for the tooltip</param>
        /// <param name="text">Less emphasized text for the tooltip</param>
        internal static void SetTooltip(this GameObject gameObj, string title, string text)
        {
            if (gameObj != null)
            {
                TooltipController_TitleAndText tt = (gameObj?.GetComponent<TooltipController_TitleAndText>() ?? gameObj?.AddComponent<TooltipController_TitleAndText>());
                if (tt != null)
                {
                    tt.prefab = titleAndTextTooltipPrefab;
                    tt.titleString = title;
                    tt.textString = text;
                }
            }
        }


        private static readonly Tooltip_Text textTooltipPrefab = AssetBase.GetPrefab<Tooltip_Text>("Tooltip_Text");

        /// <summary>
        /// Create a tooltip object for a given GameObject
        /// </summary>
        /// <param name="gameObj"></param>
        /// <param name="tooltip"></param>
        /// <returns></returns>
        internal static bool SetTooltip(this GameObject gameObj, string tooltip)
        {
            if (gameObj != null)
            {
                TooltipController_Text tt = (gameObj.GetComponent<TooltipController_Text>() ?? gameObj.AddComponent<TooltipController_Text>());
                if (tt != null)
                {
                    tt.textString = tooltip;
                    tt.prefab = textTooltipPrefab;
                    return true;
                }
            }
            return false;
        }


        /// <summary>
        /// Set up callbacks to activate a tooltip for a DialogGUI* object later.
        /// DialogGUI* objects don't have GameObjects until they're displayed, and you need that to add a tooltip, so we need an asynchronous strategy.
        /// </summary>
        /// <param name="gb"></param>
        /// <returns></returns>
        internal static DialogGUIBase DeferTooltip(DialogGUIBase gb)
        {
            if (gb.tooltipText != "")
            {
                gb.OnUpdate = () => {
                    if ((gb.uiItem != null) && gb.uiItem.SetTooltip(gb.tooltipText))
                    {
                        gb.OnUpdate = () => { };
                    }
                };
            }
            return gb;
        }

    }

}
