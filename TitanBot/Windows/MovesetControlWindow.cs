using StinkMod;
using System;
using System.Collections.Generic;
using System.Linq;
using TitanBot.FlatUI5;
using UnityEngine;

namespace TitanBot.Windows
{
    public static class MovesetControlWindow
    {

        private static Window ControlWindow;
        private static ScrollList ScrollList;
        private static Dictionary<PTAction, TitanMove> modifiableMovesetControls;
        private static Dictionary<PTAction, bool> moveEnabled;
        private static Dictionary<PTAction, string> startAnimAtTextBoxs;
        private static PTAction[] movesToControl;
        private static int changesCount = 0;

        /// <summary>
        /// Init After MovesetControl
        /// </summary>
        public static void Init()
        {
            ControlWindow = new Window(new Rect(QuickMenu.menuX - 400f, QuickMenu.menuY, 400f, 800f), "Moveset Control", QuickMenu.tabColors[1]);
            ScrollList = new ScrollList(30f, QuickMenu.tabColors[1]);
            modifiableMovesetControls = new Dictionary<PTAction, TitanMove>();
            movesToControl = new PTAction[MovesetControl.movesetControlDatabase.Count];
            MovesetControl.movesetControlDatabase.Keys.CopyTo(movesToControl, 0);
            foreach (PTAction action in MovesetControl.movesetControlDatabase.Keys)
            {
                modifiableMovesetControls.Add(action, MovesetControl.movesetControlDatabase[action].Copy());
            }
        }

        public static void OnGUI()
        {
            if (ControlWindow.ContentVisible())
            {
                Rect ScrollArea = new Rect(ControlWindow.ContentRect.x, ControlWindow.ContentRect.y, ControlWindow.ContentRect.width, ControlWindow.ContentRect.height - 40f);
                Rect BottomArea = new Rect(ControlWindow.ContentRect.x, ControlWindow.ContentRect.y + ControlWindow.ContentRect.height - 40f, ControlWindow.ContentRect.width, 40f);
                for (int i = 0; i < movesToControl.Length; i++)
                {
                    MovesetControlElement(i, movesToControl[i]);
                }
            }
        }

        private static void MovesetControlElement(int i, PTAction move)
        {
            int idx = i * 2;
            FlatUI.Label(ScrollList.IndexToRect(idx, 3, 0, 2), modifiableMovesetControls[move].Name, ScrollList.IsVisible(idx));
            moveEnabled[move] = FlatUI.Check(ScrollList.IndexToRect(idx, 3, 2), moveEnabled[move], "Enabled", ScrollList.IsVisible(idx));
            FlatUI.Label(ScrollList.IndexToRect(idx + 1, 4, 0, 2), "Start At: ", ScrollList.IsVisible(idx));
            startAnimAtTextBoxs[move] = FlatUI.TextField(ScrollList.IndexToRect(idx + 1, 4, 2, 2), startAnimAtTextBoxs[move], ScrollList.IsVisible(idx + 1));
            //if (FlatUI.Button(ScrollList.IndexToRect(idx + 1, 4, 3), ""))
        }

        private static void CheckForChanges()
        {
            changesCount = 0;
            foreach (PTAction action in movesToControl)
            {
                if (!modifiableMovesetControls[action].Equals(MovesetControl.movesetControlDatabase[action]))
                {
                    changesCount++;
                }
            }
        }

        private static void ApplyChanges()
        {
            PlayerTitanBot.TempActionsList.Clear();
            foreach (PTAction action in movesToControl)
            {
                if (moveEnabled[action])
                {
                    PlayerTitanBot.TempActionsList.Add(action);
                }
            }
        }
    }
}
