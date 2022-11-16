using System.Reflection.Emit;
using TitanBot.Windows;
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
        public static readonly Color defaultTextFieldColor = new Color(0.8f, 0.8f, 0.8f);
        public static readonly Color defaultTextFieldOutlineColor = new Color(0.2f, 0.2f, 0.2f);
        public static readonly Color ChangedValueOutlineColor = new Color(1f, 0.5f, 0f);
        public static Texture2D outsideColorTex;
        public static Texture2D insideColorTex;
        public static Texture2D defaultButtonTex;
        public static Texture2D defaultTextFieldTex;
        public static Texture2D defaultTextFieldOutlineTex;
        public static Texture2D ChangedValueOutlineTex;
        public static int defaultOutlineThickness = 2;


        public static GUIStyle DefaultStyle = new GUIStyle
        {
            border = new RectOffset(1, 1, 1, 1),
            alignment = TextAnchor.MiddleCenter,
            fontSize = 14,
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
        public static GUIStyle titleStyle = new GUIStyle
        {
            border = new RectOffset(1, 1, 1, 1),
            alignment = TextAnchor.MiddleCenter,
            fontSize = 18,
            fontStyle = FontStyle.Bold,
            normal = { textColor = Color.white }
        };
        /// <summary>
        /// Draws a static box with an outline that will make up most of the GUI elements of this mod
        /// </summary>
        /// <param name="Rect"></param>
        public static void Box(Rect Rect, bool visible = true)
        {
            if (!visible) return;
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

        public static void SwitchBox(Rect Rect, bool invert, Texture2D insideTex, Texture2D staticOutsideTex)
        {
            int outlineModifier = 0;
            if (IsMouseInRect(Rect))
            {
                outlineModifier = 1;
            }
            Rect insideRect = new Rect(Rect.x + defaultOutlineThickness + outlineModifier, Rect.y + defaultOutlineThickness + outlineModifier, Rect.width - (defaultOutlineThickness + outlineModifier) * 2, Rect.height - (defaultOutlineThickness + outlineModifier) * 2);
            if (invert)
            {
                GUI.DrawTexture(Rect, staticOutsideTex);
                GUI.DrawTexture(insideRect, outsideColorTex);
            }
            else
            {
                GUI.DrawTexture(Rect, staticOutsideTex);
                GUI.DrawTexture(insideRect, insideTex);
            }
        }

        /// <summary>
        /// Styled button, inverts colors when clicked
        /// </summary>
        /// <param name="Rect"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool Button(Rect Rect, string label, bool draw = true)
        {
            if (draw)
            {
                SwitchBox(Rect, IsMouseInRect(Rect) && Input.GetKey(KeyCode.Mouse0), defaultButtonTex);
                return GUI.Button(Rect, label, ButtonStyle);
            }
            else
            {
                return false;
            }
        }
        public static bool Button(Rect Rect, string label, Texture2D insideTex, bool draw = true)
        {
            if (draw)
            {
                SwitchBox(Rect, IsMouseInRect(Rect) && Input.GetKey(KeyCode.Mouse0), insideTex);
                return GUI.Button(Rect, label, BlackTextButtonStyle);
            }
            else
            {
                return false;
            }
        }

        public static bool Button(Rect Rect, string label, Texture2D insideTex, Texture2D outsideTex, bool draw = true)
        {
            if (draw)
            {
                SwitchBox(Rect, IsMouseInRect(Rect) && Input.GetKey(KeyCode.Mouse0), insideTex, outsideTex);
                return GUI.Button(Rect, label, ButtonStyle);
            }
            else
            {
                return false;
            }
        }

        public static void TooltipButton(Rect rect, string title, string content, int tabColor, bool draw = true)
        {
            if (draw)
            {
                SwitchBox(new Rect(rect.x + rect.width - 28f, rect.y + 2f, 26f, rect.height - 4f), IsMouseInRect(rect) && Input.GetKey(KeyCode.Mouse0), insideColorTex, outsideColorTex);
                if (GUI.Button(new Rect(rect.x + rect.width - 28f, rect.y + 2f, 26f, rect.height - 4f), "?", ButtonStyle))
                {
                    ToolTipWindow.Tooltip(title, content, tabColor);
                }
            }
        }

        public static void Label(Rect rect, string message, bool draw = true)
        {
            if (draw)
                GUI.Label(rect, message, DefaultStyle);
        }
        public static void Label(Rect rect, string message, GUIStyle style, bool draw = true)
        {
            if (draw)
                GUI.Label(rect, message, style);
        }

        /// <summary>
        /// Styled Check Box, inverts colors when checked.  Includes a label to the left side of the box
        /// </summary>
        /// <param name="Rect"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool Check(Rect Rect, bool value, string label, bool draw = true)
        {
            if (!draw) return value;
            SwitchBox(new Rect(Rect.x + Rect.width - 28f, Rect.y + 2f, 26f, Rect.height - 4f), value);
            if (GUI.Button(new Rect(Rect.x + Rect.width - 28f, Rect.y + 2f, 26f, Rect.height - 4f), "", ButtonStyle))
            {
                return !value;
            }
            GUI.Label(Rect, label);
            return value;
        }


        private static GUIStyle textFieldStyle = new GUIStyle
        {
            border = new RectOffset(1, 1, 1, 1),
            alignment = TextAnchor.MiddleCenter,
            fontSize = 14,
            fontStyle = FontStyle.Normal,
            normal = { textColor = Color.black }
        };
        public static string TextField(Rect Rect, string text, bool draw = true)
        {
            if (draw)
            {
                FlatUI.Box(Rect, defaultTextFieldTex, defaultTextFieldOutlineTex);
                return GUI.TextField(Rect, text, textFieldStyle);
            }
            else
                return text;
        }

        public static int tabs(Rect pos, string[] tabs, int index, bool top, Texture2D color)
        {
            int num = tabs.Length;
            float num2 = pos.width / (float)num;
            Texture2D[] array = new Texture2D[num];
            for (int i = 0; i < num; i++)
            {
                array[i] = color;
            }
            for (int j = 0; j < num; j++)
            {
                Rect rect;
                if (top)
                {
                    rect = new Rect(pos.x + num2 * (float)j, pos.y, num2, pos.height);
                }
                else
                {
                    rect = new Rect(pos.x, pos.y + (float)j * 30f, 100f, 30f);
                }
                if (index == j)
                {
                    tab(rect, top, array[j], FengGameManagerMKII.instance.textureBackgroundBlack);
                    GUI.Label(rect, tabs[j], new GUIStyle
                    {
                        border = new RectOffset(1, 1, 1, 1),
                        alignment = TextAnchor.MiddleCenter,
                        normal =
                        {
                            textColor = Color.white
                        }
                    });
                }
                else
                {
                    FlatUI.Box(rect, array[j], FengGameManagerMKII.instance.textureBackgroundBlack);
                    if (GUI.Button(rect, tabs[j], new GUIStyle
                    {
                        border = new RectOffset(1, 1, 1, 1),
                        alignment = TextAnchor.MiddleCenter,
                        normal =
                        {
                            textColor = Color.white
                        }
                    }))
                    {
                        return j;
                    }
                }
            }
            return index;
        }
        public static int tabs(Rect pos, string[] tabs, int index, bool top, Texture2D[] tabColors)
        {
            int num = tabs.Length;
            float num2 = pos.width / (float)num;
            Texture2D[] array = new Texture2D[num];
            for (int i = 0; i < num; i++)
            {
                if (i < tabColors.Length)
                {
                    array[i] = tabColors[i];
                }
                else
                {
                    array[i] = FengGameManagerMKII.instance.textureBackgroundBlue;
                }
            }
            for (int j = 0; j < num; j++)
            {
                Rect rect;
                if (top)
                {
                    rect = new Rect(pos.x + num2 * (float)j, pos.y, num2, pos.height);
                }
                else
                {
                    rect = new Rect(pos.x, pos.y + (float)j * 30f, 100f, 30f);
                }
                if (index == j)
                {
                    tab(rect, top, array[j], FengGameManagerMKII.instance.textureBackgroundBlack);
                    GUI.Label(rect, tabs[j], new GUIStyle
                    {
                        border = new RectOffset(1, 1, 1, 1),
                        alignment = TextAnchor.MiddleCenter,
                        normal =
                        {
                            textColor = Color.white
                        }
                    });
                }
                else
                {
                    FlatUI.Box(rect, array[j], FengGameManagerMKII.instance.textureBackgroundBlack);
                    if (GUI.Button(rect, tabs[j], new GUIStyle
                    {
                        border = new RectOffset(1, 1, 1, 1),
                        alignment = TextAnchor.MiddleCenter,
                        normal =
                        {
                            textColor = Color.white
                        }
                    }))
                    {
                        return j;
                    }
                }
            }
            return index;
        }
        private static void tab(Rect rect, bool top, Texture2D inside, Texture2D outside)
        {
            if (top)
            {
                GUI.DrawTexture(new Rect(rect.x, rect.y, rect.width, rect.height), outside);
                GUI.DrawTexture(new Rect(rect.x + 2f, rect.y + 2f, rect.width - 4f, rect.height), inside);
                return;
            }
            else
            {
                GUI.DrawTexture(new Rect(rect.x - 2f, rect.y, rect.width, rect.height), outside);
                GUI.DrawTexture(new Rect(rect.x - 2f, rect.y + 2f, rect.width - 2f, rect.height - 4f), inside);
                return;
            }
        }

        public struct PageData
        {
            int page;
            int elementMin;
            int elementMax;

            public PageData(int page, int elementMin, int elementMax)
            {
                this.page = page;
                this.elementMin = elementMin;
                this.elementMax = elementMax;
            }
        }

        private static PageData Page(Rect pageButtons, int page, int arrayLength, int pageLength)
        {
            if (arrayLength < pageLength)
            {
                return new PageData(0, 0, arrayLength);
            }
            int result = page;
            float width = 30f;
            if (FlatUI.Button(new Rect(pageButtons.x, pageButtons.y, width, pageButtons.height), "<"))
            {
                result = page + 1;
            }
            if (FlatUI.Button(new Rect(pageButtons.x + pageButtons.width - 30f, pageButtons.y, width, pageButtons.height), ">"))
            {
                result = page - 1;
            }
            int min = page * pageLength;
            int max = (page * pageLength) + pageLength;
            return new PageData(result, min, max);

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
            defaultTextFieldTex = new Texture2D(1, 1, TextureFormat.ARGB32, false);
            defaultTextFieldTex.SetPixel(0, 0, defaultTextFieldColor);
            defaultTextFieldTex.Apply();
            defaultTextFieldOutlineTex = new Texture2D(1, 1, TextureFormat.ARGB32, false);
            defaultTextFieldOutlineTex.SetPixel(0, 0, defaultTextFieldOutlineColor);
            defaultTextFieldOutlineTex.Apply();
            ChangedValueOutlineTex = new Texture2D(1, 1, TextureFormat.ARGB32, false);
            ChangedValueOutlineTex.SetPixel(0, 0, ChangedValueOutlineColor);
            ChangedValueOutlineTex.Apply();
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
