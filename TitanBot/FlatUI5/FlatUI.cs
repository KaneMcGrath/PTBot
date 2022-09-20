using UnityEngine;

namespace TitanBot.FlatUI5
{
    /// <summary>
    /// All static components of FlatUI5
    /// </summary>
    public static class FlatUI
    {
        public static readonly Color insideColor = new Color(0.3f, 0.3f, 0.3f);
        public static readonly Color outsideColor = new Color(0.1f, 0.1f, 0.1f);
        public static readonly Color defaultButtonColor = new Color(0.4f, 0.4f, 0.4f);
        public static Texture2D outsideColorTex;
        public static Texture2D insideColorTex;
        public static Texture2D defaultButtonTex;
        public static int defaultOutlineThickness = 2;


        public static GUIStyle DefaultStyle = new GUIStyle
        {
            border = new RectOffset(1, 1, 1, 1),
            alignment = TextAnchor.MiddleCenter,
            fontSize = 12,
            fontStyle = FontStyle.Normal,
            normal = { textColor = Color.white }
        };
        public static GUIStyle ThickStyle = new GUIStyle
        {
            border = new RectOffset(1, 1, 1, 1),
            alignment = TextAnchor.MiddleCenter,
            fontSize = 16,
            fontStyle = FontStyle.Bold,
            normal = { textColor = Color.white }
        };

        /// <summary>
        /// Draws a static box with an outline that will make up most of the GUI elements of this mod
        /// </summary>
        /// <param name="Rect"></param>
        public static void Box(Rect Rect)
        {
            Rect insideRect = new Rect(Rect.x + defaultOutlineThickness, Rect.y + defaultOutlineThickness, Rect.width - defaultOutlineThickness * 2, Rect.height - defaultOutlineThickness * 2);
            GUI.DrawTexture(Rect, outsideColorTex);
            GUI.DrawTexture(insideRect, insideColorTex);
        }
        public static void Box(Rect Rect, Texture2D insideTex)
        {
            Rect insideRect = new Rect(Rect.x + defaultOutlineThickness, Rect.y + defaultOutlineThickness, Rect.width - defaultOutlineThickness * 2, Rect.height - defaultOutlineThickness * 2);
            GUI.DrawTexture(Rect, outsideColorTex);
            GUI.DrawTexture(insideRect, insideTex);
        }
        public static void Box(Rect Rect, Texture2D insideTex, Texture2D outsideTex)
        {
            Rect insideRect = new Rect(Rect.x + defaultOutlineThickness, Rect.y + defaultOutlineThickness, Rect.width - defaultOutlineThickness * 2, Rect.height - defaultOutlineThickness * 2);
            GUI.DrawTexture(Rect, outsideTex);
            GUI.DrawTexture(insideRect, insideTex);
        }
        public static void Box(Rect Rect, Texture2D insideTex, Texture2D outsideTex, int outlineThickness)
        {
            Rect insideRect = new Rect(Rect.x + defaultOutlineThickness, Rect.y + defaultOutlineThickness, Rect.width - defaultOutlineThickness * 2, Rect.height - defaultOutlineThickness * 2);
            GUI.DrawTexture(Rect, outsideTex);
            GUI.DrawTexture(insideRect, insideTex);
        }

        /// <summary>
        /// A box that can invert its colors easily
        /// thickens when moused over to indicate interactibility
        /// </summary>
        /// <param name="Rect"></param>
        /// <param name="invert"></param>
        public static void SwitchBox(Rect Rect, bool invert)
        {
            int outlineModifier = 0;
            if (IsMouseInRect(Rect))
            {
                outlineModifier = 1;
            }
            Rect insideRect = new Rect(Rect.x + defaultOutlineThickness + outlineModifier, Rect.y + defaultOutlineThickness + outlineModifier, Rect.width - (defaultOutlineThickness + outlineModifier) * 2, Rect.height - (defaultOutlineThickness + outlineModifier) * 2);
            if (invert)
            {
                GUI.DrawTexture(Rect, insideColorTex);
                GUI.DrawTexture(insideRect, outsideColorTex);
            }
            else
            {
                GUI.DrawTexture(Rect, outsideColorTex);
                GUI.DrawTexture(insideRect, insideColorTex);
            }
        }
        public static void SwitchBox(Rect Rect, bool invert, Texture2D insideTex)
        {
            int outlineModifier = 0;
            if (IsMouseInRect(Rect))
            {
                outlineModifier = 1;
            }
            Rect insideRect = new Rect(Rect.x + defaultOutlineThickness + outlineModifier, Rect.y + defaultOutlineThickness + outlineModifier, Rect.width - (defaultOutlineThickness + outlineModifier) * 2, Rect.height - (defaultOutlineThickness + outlineModifier) * 2);
            if (invert)
            {
                GUI.DrawTexture(Rect, insideTex);
                GUI.DrawTexture(insideRect, outsideColorTex);
            }
            else
            {
                GUI.DrawTexture(Rect, outsideColorTex);
                GUI.DrawTexture(insideRect, insideTex);
            }
        }

        /// <summary>
        /// Styled button, inverts colors when clicked
        /// </summary>
        /// <param name="Rect"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool Button(Rect Rect, string label)
        {
            SwitchBox(Rect, IsMouseInRect(Rect) && Input.GetKey(KeyCode.Mouse0), defaultButtonTex);
            return GUI.Button(Rect, label, ButtonStyle);
        }
        public static bool Button(Rect Rect, string label, Texture2D insideTex)
        {
            SwitchBox(Rect, IsMouseInRect(Rect) && Input.GetKey(KeyCode.Mouse0), insideTex);
            return GUI.Button(Rect, label, BlackTextButtonStyle);
        }

        /// <summary>
        /// Styled Check Box, inverts colors when checked.  Includes a label to the left side of the box
        /// </summary>
        /// <param name="Rect"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool Check(Rect Rect, bool value, string label)
        {
            SwitchBox(new Rect(Rect.x + Rect.width - 30f, Rect.y, 30f, Rect.height), value);
            if (GUI.Button(new Rect(Rect.x + Rect.width - 30f, Rect.y, 30f, Rect.height), "", ButtonStyle))
            {
                return !value;
            }
            GUI.Label(Rect, label);
            return value;
        }

        /// <summary>
        /// Returns true if the mouse pointer is currently inside the rectangle
        /// </summary>
        /// <param name="rect"></param>
        /// <returns></returns>
        public static bool IsMouseInRect(Rect rect)
        {
            return Input.mousePosition.x > rect.x && Input.mousePosition.x < rect.x + rect.width && (float)Screen.height - Input.mousePosition.y > rect.y && (float)Screen.height - Input.mousePosition.y < rect.y + rect.height;
        }

        public static void Init()
        {
            insideColorTex = new Texture2D(1, 1, TextureFormat.ARGB32, false);
            insideColorTex.SetPixel(0, 0, insideColor);
            insideColorTex.Apply();
            outsideColorTex = new Texture2D(1, 1, TextureFormat.ARGB32, false);
            outsideColorTex.SetPixel(0, 0, outsideColor);
            outsideColorTex.Apply();
            defaultButtonTex = new Texture2D(1, 1, TextureFormat.ARGB32, false);
            defaultButtonTex.SetPixel(0, 0, defaultButtonColor);
            defaultButtonTex.Apply();
        }

        private static GUIStyle ButtonStyle = new GUIStyle
        {
            border = new RectOffset(1, 1, 1, 1),
            alignment = TextAnchor.MiddleCenter,
            normal = { textColor = Color.white }
        };

        private static GUIStyle BlackTextButtonStyle = new GUIStyle
        {
            border = new RectOffset(1, 1, 1, 1),
            alignment = TextAnchor.MiddleCenter,
            normal = { textColor = Color.black }
        };

    }
}
