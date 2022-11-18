using StinkMod;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TitanBot.FlatUI5;
using UnityEngine;

namespace TitanBot.Windows
{
    internal class ToolTipWindow
    {
        public static Window MyWindow;
        public static string ToolTipTitle = "Tooltip";
        public static string ToolTipText = "This window will show help text for a UI control or feature.  You should not be able to see this.";

        public static void Tooltip(string title, string content, int color = 0)
        {
            ToolTipTitle = title;
            ToolTipText = content;
            MyWindow.title = title;
            MyWindow.insideTex = QuickMenu.tabColors[color];
            MyWindow.showWindow = true;
        }

        public static void Init()
        {
            MyWindow = new Window(new Rect(QuickMenu.menuX - 800f, QuickMenu.menuY, 400f, 500f), "Tooltip", QuickMenu.tabColors[0]);
        }

        public static void OnGUI()
        {
            if (MyWindow.showWindow)
            {
                MyWindow.OnGUI();
                if (MyWindow.ContentVisible())
                {
                    Rect title = new Rect(MyWindow.ContentRect.x, MyWindow.ContentRect.y, MyWindow.ContentRect.width, 40f);
                    Rect Content = new Rect(MyWindow.ContentRect.x + 5f, MyWindow.ContentRect.y + 40f, MyWindow.ContentRect.width - 10f, MyWindow.ContentRect.height - 35f);
                    FlatUI.Label(title, ToolTipTitle, FlatUI.titleStyle);
                    FlatUI.Label(Content, ToolTipText, TextStyle);
                }
            }
        }

        public static GUIStyle TextStyle = new GUIStyle
        {
            border = new RectOffset(1, 1, 1, 1),
            alignment = TextAnchor.UpperLeft,
            fontSize = 14,
            fontStyle = FontStyle.Normal,
            normal = { textColor = Color.white },
            wordWrap = true
        };
    }
}
