﻿using StinkMod;
using System;
using System.Collections.Generic;
using System.Linq;
using TitanBot.FlatUI5;
using UnityEngine;

namespace TitanBot.Windows
{
    public static class MovesetControlWindow
    {

        public static Window ControlWindow;
        private static ScrollList ScrollList;
        private static Dictionary<PTAction, TitanMove> modifiableMovesetControls;
        public static Dictionary<PTAction, bool> moveEnabled;
        public static Dictionary<PTAction, string> startAnimAtTextBoxs;
        private static PTAction[] movesToControl;
        private static int changesCount = 0;
        private static float checkForChangesTimer = 0f;

        /// <summary>
        /// Init After MovesetControl
        /// </summary>
        public static void Init()
        {
            ControlWindow = new Window(new Rect(QuickMenu.menuX - 400f, QuickMenu.menuY, 400f, 700f), "Moveset Control", QuickMenu.tabColors[1]);
            ScrollList = new ScrollList(30f, QuickMenu.tabColors[1]);
            modifiableMovesetControls = new Dictionary<PTAction, TitanMove>();
            startAnimAtTextBoxs = new Dictionary<PTAction, string>();
            moveEnabled = new Dictionary<PTAction, bool>();
            movesToControl = new PTAction[MovesetControl.movesetControlDatabase.Count];
            MovesetControl.movesetControlDatabase.Keys.CopyTo(movesToControl, 0);
            foreach (PTAction action in MovesetControl.movesetControlDatabase.Keys)
            {
                modifiableMovesetControls.Add(action, MovesetControl.movesetControlDatabase[action].Copy());
                startAnimAtTextBoxs.Add(action, "0");
                moveEnabled[action] = true;
            }
        }

        public static void OnGUI()
        {
            if (ControlWindow.showWindow)
            {
                ControlWindow.OnGUI();
                if (ControlWindow.ContentVisible())
                {
                    if (CGTools.timer(ref checkForChangesTimer, 0.5f))
                    {
                        CheckForChanges();
                    }
                    
                    Rect ScrollArea = new Rect(ControlWindow.ContentRect.x, ControlWindow.ContentRect.y, ControlWindow.ContentRect.width, ControlWindow.ContentRect.height - 50f);
                    Rect BottomArea = new Rect(ControlWindow.ContentRect.x, ControlWindow.ContentRect.y + ControlWindow.ContentRect.height - 32f, ControlWindow.ContentRect.width, 30f);
                    Rect WarningArea = new Rect(ControlWindow.ContentRect.x + 2f, ControlWindow.ContentRect.y + ControlWindow.ContentRect.height - 80f, ControlWindow.ContentRect.width - 4f, 50f);

                    for (int i = 0; i < movesToControl.Length; i++)
                    {
                        MovesetControlElement(i, movesToControl[i]);
                    }
                    ScrollList.DrawBlanks(ScrollArea);
                    FlatUI.Label(WarningArea, "Warning! setting StartAt too high on certain moves can cause spaming explosions which will disconnect you from the game.  Try out moves in offline mode first", WarningTextStyle);
                    if (changesCount > 0)
                    {
                        FlatUI.Label(RTools.SplitH(BottomArea, 8, 0, 4), changesCount.ToString() + " unsaved changes", FlatUI.titleStyle);
                        if (FlatUI.Button(RTools.SplitH(BottomArea, 8, 4, 2), "Undo Changes"))
                        {
                            UpdateWindowData();
                        }
                        if (FlatUI.Button(RTools.SplitH(BottomArea, 8, 6, 2), "Apply Changes"))
                        {
                            ApplyChanges();
                        }
                    }
                }
            }
        }

        private static void MovesetControlElement(int i, PTAction move)
        {
            int idx = i * 2;
            Rect r = ScrollList.IndexToRect(idx);
            if (ScrollList.IsVisible(idx))
            {
                GUI.DrawTexture(new Rect(r.x, r.y, r.width, 2f), FlatUI.outsideColorTex);
                FlatUI.Label(ScrollList.IndexToRect(idx, 8, 0, 4), modifiableMovesetControls[move].Name, FlatUI.titleStyle);
                
            }
            if (ScrollList.IsVisible(idx + 1))
            {
                FlatUI.Label(ScrollList.IndexToRect(idx + 1, 8, 0, 2), "Start At: ");
                startAnimAtTextBoxs[move] = FlatUI.TextField(ScrollList.IndexToRect(idx + 1, 8, 2, 2), startAnimAtTextBoxs[move]);
                moveEnabled[move] = FlatUI.Check(ScrollList.IndexToRect(idx + 1, 12, 7, 4), moveEnabled[move], "Move Enabled");
            }
        }
        public static void UpdateWindowData()
        {
            foreach (PTAction action in movesToControl)
            {
                startAnimAtTextBoxs[action] = MovesetControl.movesetControlDatabase[action].startAnimationAt.ToString();
                if (PlayerTitanBot.TempActionsList.Contains(action))
                {
                    MovesetControlWindow.moveEnabled[action] = true;
                }
                else
                {
                    MovesetControlWindow.moveEnabled[action] = false;
                }
            }
        }


        private static void CheckForChanges()
        {
            changesCount = 0;
            foreach (PTAction action in movesToControl)
            {
                if (PlayerTitanBot.TempActionsList.Contains(action) != moveEnabled[action])
                {
                    changesCount++;
                }
                if (float.TryParse(startAnimAtTextBoxs[action], out float f))
                {
                    modifiableMovesetControls[action].startAnimationAt = f;
                }
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
                MovesetControl.movesetControlDatabase[action].startAnimationAt = modifiableMovesetControls[action].startAnimationAt;
            }
            PlayerTitanBot.pTActions = PlayerTitanBot.TempActionsList.ToArray();
            if (FengGameManagerMKII.instance.gameStart)
            {
                foreach (GameObject t in GameObject.FindGameObjectsWithTag("titan"))
                {
                    TITAN titan = t.GetComponent<TITAN>();
                    if (titan.isCustomTitan)
                    {
                        PlayerTitanBot b = (PlayerTitanBot)titan.controller;
                        b.LiveUpdateMovesetData();
                    }
                }
            }
            CGTools.log("Moveset Updated for All Titans!");
        }

        private static void UndoChanges()
        {
            foreach (PTAction action in movesToControl)
            {
                if (PlayerTitanBot.TempActionsList.Contains(action) && !moveEnabled[action])
                {
                    moveEnabled[action] = true;
                }
                if (!PlayerTitanBot.TempActionsList.Contains(action) && moveEnabled[action])
                {
                    moveEnabled[action] = false;
                }
                
                if (!modifiableMovesetControls[action].Equals(MovesetControl.movesetControlDatabase[action]))
                {
                    changesCount++;
                }

            }
        }

        public static GUIStyle WarningTextStyle = new GUIStyle
        {
            border = new RectOffset(1, 1, 1, 1),
            alignment = TextAnchor.UpperLeft,
            fontSize = 14,
            fontStyle = FontStyle.Bold,
            clipping = TextClipping.Overflow,
            wordWrap = true,
            normal = { textColor = Color.red }
        };
    }
}