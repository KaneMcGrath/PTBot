using System;
using TitanBot.FlatUI5;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace TitanBot
{
    public static class QuickMenu
    {
        public static Texture2D[] tabColors = new Texture2D[10];
        public static float menuX;
        public static float menuY;
        public static int tabIndex = 0;
        public static string[] tabNames = new string[]
        {
            "main",
            "test"
        };

        public static void OnGUI()
        {
            FlatUI.Box(new Rect(menuX, menuY, 250f, 700f), tabColors[tabIndex], FengGameManagerMKII.instance.textureBackgroundBlack);
            tabIndex = tabs(new Rect(menuX + 250f, menuY, 700f, 100f), tabNames, tabIndex, false, tabColors);
        }





        public static void Init()
        {
            for (int i = 0; i < tabColors.Length; i++)
            {
                tabColors[i] = CGTools.ColorTex(CGTools.randomColor());
            }
            menuX = (float)Screen.width / 2f - 350f + 700f;
            menuY = (float) Screen.height / 2f - 250f;
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
    }
}
