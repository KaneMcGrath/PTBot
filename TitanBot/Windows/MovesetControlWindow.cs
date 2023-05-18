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

        public static Window ControlWindow;
        private static ScrollList ScrollList;
        private static Dictionary<PTAction, TitanMove> modifiableMovesetControls;
        public static Dictionary<PTAction, bool> moveEnabled;
        public static Dictionary<PTAction, string> startAnimAtTextBoxs;
        private static PTAction[] movesToControl;
        private static int changesCount = 0;
        private static float checkForChangesTimer = 0f;
        private static Texture2D[] tabColors;

        private static string[] tabs = new string[]
        {
            "Moves",
            "Profile"
        };
        private static int tabIndex = 0;

        private static string ProfileName = "";
        private static ScrollList ProfileScrollList;

        private static int selectedProfileIndex = -1;

        /// <summary>
        /// Init After MovesetControl
        /// </summary>
        public static void Init()
        {
            tabColors = new Texture2D[]
            {
                CGTools.ColorTex(CGTools.randomColor()),
                CGTools.ColorTex(CGTools.randomColor())
            };
            ControlWindow = new Window(new Rect(QuickMenu.menuX - 400f, QuickMenu.menuY, 400f, 700f), "Moveset Control", tabColors[0]);
            ScrollList = new ScrollList(30f, tabColors[0]);
            ProfileScrollList = new ScrollList(30f, tabColors[1]);
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
                    //FlatUI.Box(new Rect(ControlWindow.rect.x, ControlWindow.rect.y + 60f, ControlWindow.rect.width, ControlWindow.rect.height - 60f), tabColors[tabIndex]);
                    Rect TabsRect = new Rect(ControlWindow.rect.x, ControlWindow.rect.y - 30f, ControlWindow.rect.width - 120f, 30f);
                    tabIndex = FlatUI.tabs(TabsRect, tabs, tabIndex, true, tabColors);
                    ControlWindow.insideTex = tabColors[tabIndex];
                    if (tabIndex == 0)
                    {
                        if (CGTools.timer(ref checkForChangesTimer, 0.1f))
                        {
                            CheckForChanges();
                        }

                        Rect ScrollArea = new Rect(ControlWindow.ContentRect.x, ControlWindow.ContentRect.y, ControlWindow.ContentRect.width, ControlWindow.ContentRect.height - 50f);
                        Rect TopArea = new Rect(ControlWindow.ContentRect.x, ControlWindow.ContentRect.y, ControlWindow.ContentRect.width, 30f);
                        Rect BottomArea = new Rect(ControlWindow.ContentRect.x, ControlWindow.ContentRect.y + ControlWindow.ContentRect.height - 32f, ControlWindow.ContentRect.width, 30f);
                        Rect WarningArea = new Rect(ControlWindow.ContentRect.x + 2f, ControlWindow.ContentRect.y + ControlWindow.ContentRect.height - 50f, ControlWindow.ContentRect.width - 4f, 50f);

                        for (int i = 0; i < movesToControl.Length; i++)
                        {
                            MovesetControlElement(i, movesToControl[i]);
                        }
                        ScrollList.DrawBlanks(ScrollArea);
                        FlatUI.TooltipButton(TopArea, "Titan Moves", "This window will allow you to edit each of the moves availible to PTBot.  You can enable or disable each move and set the start time in seconds.  The start time of a move will skip forward in the animation to make moves come out quicker.  \n\nWarning! \nSetting StartAt too high on certain moves can cause spaming explosions which will disconnect you from the game.  These are moves like slam or slap face most of these moves are around 2 seconds in length and you should be safe setting it around 1 second \n\nTry out moves in offline mode first if you are unsure", 9);
                        //FlatUI.Label(WarningArea, "Warning! setting StartAt too high on certain moves can cause spamming explosions which will disconnect you from the game.  Try out moves in offline mode first", WarningTextStyle);
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
                    else if(tabIndex == 1)
                    {
                        MovesetProfile.ProfilesUpdateCheck();
                        Rect LabelRect = new Rect(ControlWindow.ContentRect.x, ControlWindow.ContentRect.y, ControlWindow.ContentRect.width, 30f);
                        Rect TopRect = new Rect(ControlWindow.ContentRect.x, ControlWindow.ContentRect.y + 30f, ControlWindow.ContentRect.width, 30f);
                        Rect TopLabelRect = RTools.SplitH(TopRect, 4, 0);
                        Rect TopTextBoxRect = RTools.SplitH(TopRect, 4, 1, 2);
                        Rect TopButtonRect = RTools.SplitH(TopRect, 4, 3);
                        FlatUI.Label(LabelRect, "New Profile", labelStyle);
                        FlatUI.Label(TopLabelRect, "Profile Name");
                        ProfileName = FlatUI.TextField(TopTextBoxRect, ProfileName);
                        if (FlatUI.Button(TopButtonRect, "Save"))
                        {
                            if (!ProfileName.IsNullOrEmpty())
                            {
                                MovesetProfile.SaveProfile(ProfileName);
                                ProfileName = "";
                                MovesetProfile.UpdateProfiles();
                            }
                        }
                        
                        Rect ScrollArea = new Rect(ControlWindow.ContentRect.x, ControlWindow.ContentRect.y + 60f, ControlWindow.ContentRect.width, ControlWindow.ContentRect.height - 90f);
                        for (int i = 0; i < MovesetProfile.profiles.Count; i++)
                        {
                            string Profile = MovesetProfile.profiles[i];
                            if (selectedProfileIndex == i)
                            {
                                if (FlatUI.Button(ProfileScrollList.IndexToRect(i), Profile, FlatUI.defaultButtonTex, FlatUI.ChangedValueOutlineTex, ProfileScrollList.IsVisible(i)))
                                {
                                    selectedProfileIndex = -1;
                                }
                            }
                            else
                            {
                                if (FlatUI.Button(ProfileScrollList.IndexToRect(i), Profile, FlatUI.defaultButtonTex, FlatUI.outsideColorTex, ProfileScrollList.IsVisible(i)))
                                {
                                    selectedProfileIndex = i;
                                }
                            }
                        }
                        ProfileScrollList.DrawBlanks(ScrollArea);
                        if (MovesetProfile.profiles.Contains(ProfileName))
                        {
                            FlatUI.Label(new Rect(TopRect.x, TopRect.y + 30f, TopRect.width, 30f), "Profile already exists.  Do you want to overwrite?", WarningTextStyle);
                        }
                        if (selectedProfileIndex != -1)
                        {
                            Rect bottom = new Rect(ControlWindow.ContentRect.x, ControlWindow.ContentRect.y + ControlWindow.ContentRect.height - 30f, ControlWindow.ContentRect.width, 30f);
                            if (FlatUI.Button(bottom, "Load Profile"))
                            {
                                MovesetProfile.LoadProfile(MovesetProfile.profiles[selectedProfileIndex]);
                                selectedProfileIndex = -1;
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

        private static GUIStyle labelStyle = new GUIStyle
        {
            border = new RectOffset(1, 1, 1, 1),
            alignment = TextAnchor.LowerCenter,
            fontSize = 16,
            fontStyle = FontStyle.Bold,
            normal = { textColor = Color.white }
        };
    }
}
