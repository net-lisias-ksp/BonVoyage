using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

namespace BonVoyage
{
    /// <summary>
    /// Common settings for all windows
    /// </summary>
    static class CommonWindowProperties
    {
        #region internal properties

        internal static UISkinDef UnitySkin { get; set; }
        internal static UISkinDef ActiveSkin { get; set; } // Actual skin used

        internal static RectOffset mainElementPadding = new RectOffset(5, 5, 10, 10);
        internal const float mainWindowSpacing = 3f;
        internal static RectOffset mainListPadding = new RectOffset(4, 4, 3, 3);
        internal const float mainListMinWidth = 570f;
        internal const float mainListMinHeight = 100f;
        internal const float mainWindowWidth = mainListMinWidth + 20f;
        internal const float mainWindowHeight = mainListMinHeight + 330f;
        internal static readonly Vector2 mainWindowAnchorMin = new Vector2(0.5f, 0.5f);
        internal static readonly Vector2 mainWindowAnchorMax = new Vector2(0.5f, 0.5f);
        internal static Vector2 MainWindowPosition = new Vector2(0.5f, 0.5f);

        internal static RectOffset settingsElementPadding = new RectOffset(5, 5, 10, 10);
        internal const float settingsWindowSpacing = 3f;
        internal const float settingsMinWidth = 140f;
        internal const float settingsMinHeight = 336f;
        internal const float settingsWindowWidth = settingsMinWidth + 20f;
        internal const float settingsWindowHeight = settingsMinHeight + 20f;
        internal static readonly Vector2 settingsWindowAnchorMin = new Vector2(0.5f, 0.5f);
        internal static readonly Vector2 settingsWindowAnchorMax = new Vector2(0.5f, 0.5f);

        internal static RectOffset controlElementPadding = new RectOffset(5, 5, 10, 10);
        internal const float controlWindowSpacing = 3f;
        internal const float controlMinWidth = 201f;
        internal const float controlMinHeight = 370f;
        internal const float controlWindowWidth = controlMinWidth + 20f;
        internal const float controlWindowHeight = controlMinHeight + 20f;
        internal static readonly Vector2 controlWindowAnchorMin = new Vector2(0.5f, 0.5f);
        internal static readonly Vector2 controlWindowAnchorMax = new Vector2(0.5f, 0.5f);
        internal static Vector2 ControlWindowPosition = new Vector2(0.5f, 0.5f);

        internal static RectOffset boxPadding = new RectOffset(4, 4, 4, 4);
        internal static float buttonHeight = 24f;

        internal const int buttonIconWidth = 20;

        #endregion


        #region Styles

        internal static readonly UIStyleState StyleState_White = new UIStyleState() { textColor = Color.white };
        internal static readonly UIStyleState StyleState_Green = new UIStyleState() { textColor = Color.green };
        internal static readonly UIStyleState StyleState_Yellow = new UIStyleState() { textColor = Color.yellow };
        internal static readonly UIStyleState StyleState_Red = new UIStyleState() { textColor = Color.red };
        internal static readonly UIStyleState StyleState_Grey = new UIStyleState() { textColor = Color.grey };

        internal static UIStyle Style_Label_Bold_Left;
        internal static UIStyle Style_Label_Bold_Center;
        internal static UIStyle Style_Label_Normal_Center;
        internal static UIStyle Style_Label_Normal_Center_White;
        internal static UIStyle Style_Label_Normal_Center_Green;
        internal static UIStyle Style_Label_Normal_Center_Yellow;
        internal static UIStyle Style_Label_Normal_Center_Red;
        internal static UIStyle Style_Label_Normal_Center_Grey;

        internal static UIStyle Style_Button_Bold_Yellow;
        internal static UIStyle Style_Button_Label;


        /// <summary>
        /// Get sprite form texture
        /// </summary>
        /// <param name="tex"></param>
        /// <returns></returns>
        private static Sprite SpriteFromTexture(Texture2D tex)
        {
            if (tex != null)
            {
                return Sprite.Create(
                    tex,
                    new Rect(0, 0, tex.width, tex.height),
                    new Vector2(0.5f, 0.5f),
                    tex.width
                );
            }
            else
            {
                return null;
            }
        }


        /// <summary>
		/// Borrowed from Astrogator
		/// </summary>
		/// <param name="c">The color to use</param>
		/// <returns>
		/// A 1x1 texture
		/// </returns>
		private static Texture2D SolidColorTexture(Color c)
        {
            Texture2D tex = new Texture2D(1, 1, TextureFormat.ARGB32, false);
            tex.SetPixel(1, 1, c);
            tex.Apply();
            return tex;
        }


        /// <returns>
		/// A 1x1 sprite object of the given color.
		/// </returns>
		private static Sprite SolidColorSprite(Color c)
        {
            return SpriteFromTexture(SolidColorTexture(c));
        }


        /// <value>
		/// Completely transparent sprite so we can use buttons for the headers
		/// without the default button graphic.
		/// </value>
		internal static readonly Sprite transparent = SolidColorSprite(new Color(0f, 0f, 0f, 0f));


        /// <summary>
        /// Refresh styles after skin change
        /// </summary>
        internal static void RefreshStyles()
        {
            // Style_Label_Bold_Left
            Style_Label_Bold_Left = new UIStyle(ActiveSkin.label)
            {
                fontStyle = FontStyle.Bold
            };

            // Style_Label_Bold_Center
            Style_Label_Bold_Center = new UIStyle(ActiveSkin.label)
            {
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.UpperCenter
            };

            // Style_Label_Normal_Center
            Style_Label_Normal_Center = new UIStyle(ActiveSkin.label)
            {
                alignment = TextAnchor.UpperCenter
            };

            // Style_Label_Normal_Center_White
            Style_Label_Normal_Center_White = new UIStyle(ActiveSkin.label)
            {
                alignment = TextAnchor.UpperCenter,
                active = StyleState_White,
                normal = StyleState_White,
                disabled = StyleState_White,
                highlight = StyleState_White
            };

            // Style_Label_Normal_Center_Green
            Style_Label_Normal_Center_Green = new UIStyle(ActiveSkin.label)
            {
                alignment = TextAnchor.UpperCenter,
                active = StyleState_Green,
                normal = StyleState_Green,
                disabled = StyleState_Green,
                highlight = StyleState_Green
            };

            // Style_Label_Normal_Center_Yellow
            Style_Label_Normal_Center_Yellow = new UIStyle(ActiveSkin.label)
            {
                alignment = TextAnchor.UpperCenter,
                active = StyleState_Yellow,
                normal = StyleState_Yellow,
                disabled = StyleState_Yellow,
                highlight = StyleState_Yellow
            };

            // Style_Label_Normal_Center_Red
            Style_Label_Normal_Center_Red = new UIStyle(ActiveSkin.label)
            {
                alignment = TextAnchor.UpperCenter,
                active = StyleState_Red,
                normal = StyleState_Red,
                disabled = StyleState_Red,
                highlight = StyleState_Red
            };

            // Style_Label_Normal_Center_Grey
            Style_Label_Normal_Center_Grey = new UIStyle(ActiveSkin.label)
            {
                alignment = TextAnchor.UpperCenter,
                active = StyleState_Grey,
                normal = StyleState_Grey,
                disabled = StyleState_Grey,
                highlight = StyleState_Grey
            };

            // Style_Button_Bold_Yellow
            Style_Button_Bold_Yellow = new UIStyle(ActiveSkin.button)
            {
                fontStyle = FontStyle.Bold,
                active = new UIStyleState() { textColor = Color.yellow, background = ActiveSkin.button.active.background },
                normal = new UIStyleState() { textColor = Color.yellow, background = ActiveSkin.button.normal.background },
                disabled = new UIStyleState() { textColor = Color.yellow, background = ActiveSkin.button.disabled.background },
                highlight = new UIStyleState() { textColor = Color.yellow, background = ActiveSkin.button.highlight.background }
            };

            // Style_Button_Label
            Style_Button_Label = new UIStyle(ActiveSkin.label)
            {
                active = new UIStyleState() { textColor = ActiveSkin.label.normal.textColor, background = transparent },
                normal = new UIStyleState() { textColor = ActiveSkin.label.normal.textColor, background = transparent },
                disabled = new UIStyleState() { textColor = ActiveSkin.label.normal.textColor, background = transparent },
                highlight = new UIStyleState() { textColor = ActiveSkin.label.normal.textColor, background = transparent }
            };
        }

        #endregion


        /// <summary>
		/// Add a button outside of the normal DialogGUI* flow layout,
		/// with positioning relative to edges of a parent element.
		/// By DMagic, with modifications by HebaruSan.
		/// </summary>
		/// <param name="parentTransform">Transform of UI object within which to place this button</param>
		/// <param name="innerHorizOffset">Horizontal position; if positive, number of pixels between left edge of window and left edge of button, if negative, then vice versa on right side</param>
		/// <param name="innerVertOffset">Vertical position; if positive, number of pixels between bottom edge of window and bottom edge of button, if negative, then vice versa on top side</param>
		/// <param name="style">Style object containing the sprites for the button</param>
		/// <param name="tooltip">String to show when user hovers on button</param>
		/// <param name="onClick">Function to call when the user clicks the button</param>
		internal static void AddFloatingButton(Transform parentTransform, float innerHorizOffset, float innerVertOffset, UIStyle style, string text, string tooltip, UnityAction onClick)
        {
            // This creates a new button object using the prefab from KSP's UISkinManager.
            // The same prefab is used for the PopupDialog system buttons.
            // Anything we set on this object will be reflected in the button we create.
            GameObject btnGameObj = GameObject.Instantiate<GameObject>(UISkinManager.GetPrefab("UIButtonPrefab"));

            // Set the button's parent transform.
            btnGameObj.transform.SetParent(parentTransform, false);

            // Add a layout element and set it to be ignored.
            // Otherwise the button will end up on the bottom of the window.
            btnGameObj.AddComponent<LayoutElement>().ignoreLayout = true;

            // This is how we position the button.
            // The anchors and pivot make the button positioned relative to the top-right corner.
            // The anchored position sets the position with values in pixels.
            RectTransform rect = btnGameObj.GetComponent<RectTransform>();
            rect.anchoredPosition = new Vector2(innerHorizOffset, innerVertOffset);
            rect.sizeDelta = new Vector2(buttonIconWidth, buttonIconWidth);
            rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(
                rect.anchoredPosition.x < 0 ? 1 : 0,
                rect.anchoredPosition.y < 0 ? 1 : 0
            );

            // Set the button's image component to the normal sprite.
            // Since this object comes from the button's GameObject,
            // changing it affects the button directly!
            Image btnImg = btnGameObj.GetComponent<Image>();
            btnImg.sprite = style.normal.background;

            // Now set the different states to their respective sprites.
            Button button = btnGameObj.GetComponent<Button>();
            button.transition = Selectable.Transition.SpriteSwap;
            button.spriteState = new SpriteState()
            {
                highlightedSprite = style.highlight.background,
                pressedSprite = style.active.background,
                disabledSprite = style.disabled.background
            };

            // The text will be "Button" if we don't clear it.
            btnGameObj.GetChild("Text").GetComponent<TextMeshProUGUI>().text = text;

            // Set the tooltip
            btnGameObj.SetTooltip(tooltip);

            // Set the code to call when clicked.
            button.onClick.AddListener(onClick);

            // Activate the button object, making it visible.
            btnGameObj.SetActive(true);
        }

    }

}
