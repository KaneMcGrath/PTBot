using System;
using System.Collections.Generic;
using TitanBot.FlatUI5;
using UnityEngine;

namespace TitanBot
{
    public static class QuickMenu
    {
        public static Texture2D PTButtonColor;
        public static Texture2D[] tabColors = new Texture2D[20];
        public static float menuX;
        public static float menuY;
        public static int tabIndex = 0;
        public static List<GameObject> platforms = new List<GameObject>();
        public static Vector3[] CameraPositions = new Vector3[10];
        public static Quaternion[] CameraRotations = new Quaternion[10];
        public static float mouseScale = 20f;
        public static TITAN myLastPT;
        public static string[] textBoxText = new string[100];
        public static string[] tabNames = new string[]
        {
            "PTBot Settings",
            "Game Settings",
            "Config"
        };
        public static bool showHitboxs = false;
        public static bool doAttacks = true;

        private static float checkDataLevel = 0f;
        private static bool resetPositionsOnAttack = false;
        private static bool clearHitboxesOnAttack = false;
        public static string prunningSettingTextbox = "2";
        public static string infiniteTitanTextBox = "5";
        public static string titanNameTextBox = "PTBot";
        private static GUIStyle labelStyle = new GUIStyle
        {
            border = new RectOffset(1, 1, 1, 1),
            alignment = TextAnchor.LowerCenter,
            fontSize = 14,
            fontStyle = FontStyle.Bold,
            normal = { textColor = Color.white }
        };

        public static void TabConfig()
        {
            if (FlatUI.Button(IndexToRect(1), "Save"))
            {
                KaneGameManager.SaveConfig();
            }
            if (FlatUI.Button(IndexToRect(2), "Load"))
            {
                KaneGameManager.LoadConfig();
            }
            if (FlatUI.Button(IndexToRect(3), "Default"))
            {
                KaneGameManager.ResetDefaults();
            }
        }

        public static void OnGUI()
        {
            menuX = Screen.width - 450f;
            FlatUI.Box(new Rect(menuX, menuY, 250f, 700f), tabColors[tabIndex], FengGameManagerMKII.instance.textureBackgroundBlack);
            tabIndex = tabs(new Rect(menuX + 250f, menuY, 700f, 100f), tabNames, tabIndex, false, tabColors);
            if (tabIndex == 0) tabPTBotSettings();
            if (tabIndex == 1) moreSettings();
            if (tabIndex == 2) TabConfig();
            //if (tabIndex == 3) TabMain();
            GUI.DrawTexture(new Rect(Input.mousePosition.x - (mouseScale / 2f), Screen.height - Input.mousePosition.y - (mouseScale / 2f), mouseScale, mouseScale), CGTools.mouseTex);
        }

        private static void moreSettings()
        {
            KaneGameManager.sendJoinMessage = FlatUI.Check(IndexToRect(1), KaneGameManager.sendJoinMessage, "Send Join Message");

            Label(IndexToRect(2), "Endless Spawning");
            KaneGameManager.doInfiniteTitans = FlatUI.Check(IndexToRect(3), KaneGameManager.doInfiniteTitans, "Enable Endless Spawning");
            GUI.Label(IndexToRect(4, 3, 0), "Count");
            infiniteTitanTextBox = GUI.TextField(IndexToRect(4, 3, 1), infiniteTitanTextBox);
            if (FlatUI.Button(IndexToRect(4, 3, 2), "Apply"))
            {
                if (int.TryParse(infiniteTitanTextBox, out int i))
                {
                    KaneGameManager.InfTitanCount = i;
                    CGTools.log("Infinite titan count updated to " + i);
                }
                else
                {
                    CGTools.log("Could not parse input!");
                }
            }
            PlayerTitanBot.ReplaceSpawnedTitans = FlatUI.Check(IndexToRect(6), PlayerTitanBot.ReplaceSpawnedTitans, "Replace Normal Titans");
            KaneGameManager.doSpawnTeleporting = FlatUI.Check(IndexToRect(7), KaneGameManager.doSpawnTeleporting, "Teleport Titans Away from spawn");

            Label(IndexToRect(8), "Debug");
            PlayerTitanBot.debugRaycasts = FlatUI.Check(IndexToRect(9), PlayerTitanBot.debugRaycasts, "Debug Raycasts");
            PlayerTitanBot.debugTargets = FlatUI.Check(IndexToRect(10), PlayerTitanBot.debugTargets, "Debug Targets");
            PTTools.debugPlayerData = FlatUI.Check(IndexToRect(11), PTTools.debugPlayerData, "Debug Predictions");
            if (FlatUI.Button(IndexToRect(12), "Teleport titans back inside"))
            {
                foreach (GameObject t in GameObject.FindGameObjectsWithTag("titan"))
                {
                    if (t.GetPhotonView().isMine)
                    {
                        float x = UnityEngine.Random.Range(-300f, 300f);
                        float z = UnityEngine.Random.Range(-300f, 300f);
                        t.transform.position = new Vector3(x, 10f, z);
                    }
                }
            }
        }

        private static void tabPTBotSettings()
        {
            Label(IndexToRect(0), "Titan Name");
            titanNameTextBox = GUI.TextField(IndexToRect(1, 4, 0, 3), titanNameTextBox);
            if (FlatUI.Button(IndexToRect(1,4,3), "Apply"))
            {
                PlayerTitanBot.TitanName = titanNameTextBox;
            }
            Label(IndexToRect(2), "Difficulty");
            if (FlatUI.Button(IndexToRect(3), "Very Very Hard", (PTTools.difficulty == Difficulty.VeryVeryHard) ? QuickMenu.PTButtonColor : FlatUI.insideColorTex))
            {
                PTTools.difficulty = Difficulty.VeryVeryHard;
            }
            if (FlatUI.Button(IndexToRect(4), "Very Hard", (PTTools.difficulty == Difficulty.VeryHard) ? QuickMenu.PTButtonColor : FlatUI.insideColorTex))
            {
                PTTools.difficulty = Difficulty.VeryHard;
            }
            if (FlatUI.Button(IndexToRect(5), "Hard", (PTTools.difficulty == Difficulty.Hard) ? QuickMenu.PTButtonColor : FlatUI.insideColorTex))
            {
                PTTools.difficulty = Difficulty.Hard;
            }
            if (FlatUI.Button(IndexToRect(6), "Medium", (PTTools.difficulty == Difficulty.Medium) ? QuickMenu.PTButtonColor : FlatUI.insideColorTex))
            {
                PTTools.difficulty = Difficulty.Medium;
            }
            if (FlatUI.Button(IndexToRect(7), "Easy", (PTTools.difficulty == Difficulty.Easy) ? QuickMenu.PTButtonColor : FlatUI.insideColorTex))
            {
                PTTools.difficulty = Difficulty.Easy;
            }
            if (FlatUI.Button(IndexToRect(8), "Very Easy", (PTTools.difficulty == Difficulty.VeryEasy) ? QuickMenu.PTButtonColor : FlatUI.insideColorTex))
            {
                PTTools.difficulty = Difficulty.VeryEasy;
            }

            Label(IndexToRect(9), "Moves");
            toggleAttackButton(10, 0, PTAction.Attack);
            toggleAttackButton(10, 1, PTAction.Jump);
            toggleAttackButton(11, 0, PTAction.bite);
            toggleAttackButton(11, 1, PTAction.bitel);
            toggleAttackButton(12, 0, PTAction.biter);
            toggleAttackButton(12, 1, PTAction.choptl);
            toggleAttackButton(13, 0, PTAction.choptr);
            toggleAttackButton(13, 1, PTAction.grabbackl);
            toggleAttackButton(14, 0, PTAction.grabbackr);
            toggleAttackButton(14, 1, PTAction.grabfrontl);
            toggleAttackButton(15, 0, PTAction.grabfrontr);
            toggleAttackButton(15, 1, PTAction.grabnapel);
            toggleAttackButton(16, 0, PTAction.grabnaper);
            toggleAttackButton(16, 1, PTAction.AttackII);
            if (FlatUI.Button(IndexToRect(17), "Apply"))
            {
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
            Label(IndexToRect(18), "Pruning");
            GUI.Label(IndexToRect(19, 4, 0, 2), "Pruning Level");
            prunningSettingTextbox = GUI.TextField(IndexToRect(19, 4, 2), prunningSettingTextbox);
            if (FlatUI.Button(IndexToRect(19, 4, 3), "Apply"))
            {
                if (int.TryParse(prunningSettingTextbox, out int p))
                {
                    PlayerTitanBot.dataPruningLevel = p;
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
                }
                else
                {
                    CGTools.log("Unable to parse input");
                }
            }
            GUI.Label(IndexToRectMultiLine(20, 3), "*only keep 1 of every n number of hitboxes so the rest dont have to be calculated.  Most of them overlap so it is highly reccomended on large player or titan counts");
        }




        private static void Label(Rect rect, string text)
        {
            GUI.Label(rect, text, labelStyle);
        }

        public static void tabMoveset()
        {

        }

        private static void toggleAttackButton(int index, int halfIndex, PTAction action)
        {
            if (FlatUI.Button(IndexToRect(index, 2, halfIndex), action.ToString(), PlayerTitanBot.TempActionsList.Contains(action) ? QuickMenu.PTButtonColor : FlatUI.insideColorTex))
            {
                if (PlayerTitanBot.TempActionsList.Contains(action))
                {
                    PlayerTitanBot.TempActionsList.Remove(action);
                }
                else
                {
                    PlayerTitanBot.TempActionsList.Add(action);
                }
            }
        }

        public static void tabExtra()
        {
            if (FlatUI.Button(IndexToRect(1), "Spawn Random"))
            {
                Vector3 pos = Camera.main.transform.position;
                Quaternion rot = Quaternion.identity;
                float x = UnityEngine.Random.Range(-400f, 400f);
                float z = UnityEngine.Random.Range(-400f, 400f);

                Vector3 rayOrigin = new Vector3(x, 200f, z);
                Vector3 rayDirection = Vector3.down;
                Ray ray = new Ray(rayOrigin, rayDirection);

                Vector3 spawnPosition = rayOrigin;

                if (Physics.Raycast(ray, out RaycastHit raycastHit))
                {
                    spawnPosition = raycastHit.point;
                }

                GameObject myPTGO = PhotonNetwork.Instantiate("TITAN_VER3.1", spawnPosition, rot, 0);
                TITAN MyPT = myPTGO.GetComponent<TITAN>();
                myLastPT = MyPT;
                GameObject.Destroy(myPTGO.GetComponent<TITAN_CONTROLLER>());
                myPTGO.GetComponent<TITAN>().nonAI = true;
                myPTGO.GetComponent<TITAN>().speed = 30f;
                myPTGO.GetComponent<TITAN_CONTROLLER>().enabled = true;
                myPTGO.GetComponent<TITAN>().isCustomTitan = true;


            }
            if (FlatUI.Button(IndexToRect(2), "Print Camera Pos"))
            {
                CGTools.log(Camera.main.transform.position.ToString());
            }
            if (FlatUI.Button(IndexToRect(3), "Test Connect me"))
            {
                KaneGameManager.OnPhotonPlayerConnected(PhotonNetwork.player);
            }
            PlayerTitanBot.TakeOverPT = FlatUI.Check(IndexToRect(4), PlayerTitanBot.TakeOverPT, "TakeOverPT");
            if (FlatUI.Button(IndexToRect(5), "Announce"))
            {
                KaneGameManager.OnJoinedRoom();
            }
            PlayerTitanBot.raycasts = SetTextbox(IndexToRect(6), PlayerTitanBot.raycasts, "raycasts", 0);
            PlayerTitanBot.debugRaycasts = FlatUI.Check(IndexToRect(7), PlayerTitanBot.debugRaycasts, "Debug Raycasts");
            PlayerTitanBot.debugTargets = FlatUI.Check(IndexToRect(8), PlayerTitanBot.debugTargets, "Debug Targets");
            KaneGameManager.doInfiniteTitans = FlatUI.Check(IndexToRect(9), KaneGameManager.doInfiniteTitans, "Infinite Titans");
            KaneGameManager.InfTitanCount = SetTextbox(IndexToRect(10), KaneGameManager.InfTitanCount, "Titan Count", 1);
            if (FlatUI.Button(IndexToRect(11, 2, 0), "Load Data"))
            {
                PTDataMachine.LoadHitboxData();
            }
            if (FlatUI.Button(IndexToRect(11, 2, 1), "Check Data"))
            {

                foreach (PTAction key in HitData.AllHitboxData.Keys)
                {

                    CGTools.log(key.ToString());
                    foreach (HitData.MovesetData movesetData in HitData.AllHitboxData[key])
                    {
                        float realLevel = 0f;
                        CGTools.log(" > Size = " + movesetData.titanLevel);
                        foreach (HitData.Hitbox hh in movesetData.hitboxes)
                        {
                            realLevel = hh.level;
                        }
                        CGTools.log("       > RealSize = " + realLevel);
                    }
                }
            }
            PlayerTitanBot.useCustomHair = FlatUI.Check(IndexToRect(12), PlayerTitanBot.useCustomHair, "Custom Hair");
            if (FlatUI.Button(IndexToRect(13), "Print Database Info"))
            {

                foreach (PTAction key in HitData.AllHitboxData.Keys)
                {
                    CGTools.log(key.ToString());
                }
            }
            if (textBoxText[3] == null)
            {
                textBoxText[3] = "";
            }
            textBoxText[3] = GUI.TextField(IndexToRect(15), textBoxText[3]);
            if (FlatUI.Button(IndexToRect(16, 3, 2), "Chat"))
            {

                FengGameManagerMKII.instance.photonView.RPC("Chat", PhotonTargets.All, new object[]
                {
                textBoxText[3].hexColor(),
                ""
                });
            }
            checkDataLevel = SetTextbox(IndexToRect(17), checkDataLevel, "Titan Level", 55);
            if (FlatUI.Button(IndexToRect(18, 2, 0), "Show Hitboxes"))
            {
                foreach (PTAction key in HitData.AllHitboxData.Keys)
                {
                    foreach (HitData.MovesetData movesetData in HitData.AllHitboxData[key])
                    {
                        if (movesetData.titanLevel == checkDataLevel)
                        {
                            foreach (HitData.Hitbox hitbox in movesetData.hitboxes)
                            {
                                if (hitbox.GetType() == typeof(HitData.HitboxSphere))
                                {
                                    PTDataMachine.CreateVisualizationSphere(hitbox.pos, ((HitData.HitboxSphere)hitbox).radius);
                                }
                                if (hitbox.GetType() == typeof(HitData.HitboxRectangle))
                                {
                                    PTDataMachine.CreateColliderBox(hitbox.pos, ((HitData.HitboxRectangle)hitbox).RectangleDimentions, ((HitData.HitboxRectangle)hitbox).RectangleRotation);
                                }
                            }
                        }
                    }
                }
            }
            if (FlatUI.Button(IndexToRect(19), "Unit Test weight table"))
            {
                int[] table = new int[] { 8, 6, 9, 44, 2, 7 };
                int[] tableOccurences = new int[table.Length];
                for (int o = 0; o < table.Length; o++)
                {
                    tableOccurences[o] = 0;
                }
                int num = 10000;

                for (int i = 0; i < num; i++)
                {
                    int r = CGTools.WeightTable(table);
                    tableOccurences[r]++;

                }
                CGTools.log("Finished " + num + " cycles");
                for (int i = 0; i < table.Length; i++)
                {
                    CGTools.log("index [" + i + "] weight {" + table[i] + "} occurences :" + tableOccurences[i]);
                }
            }
            PTTools.difficulty = (Difficulty)SetTextbox(IndexToRect(20), (int)PTTools.difficulty, "difficulty", 88);
            PTTools.debugPlayerData = FlatUI.Check(IndexToRect(21), PTTools.debugPlayerData, "Debug Predictions");
        }
        public static void TabMain()
        {
            if (FlatUI.Button(IndexToRect(0, 2, 0), "Spawn PTBot"))
            {
                Vector3 pos = Camera.main.transform.position;
                Quaternion rot = Quaternion.identity;
                GameObject myPTGO = PhotonNetwork.Instantiate("TITAN_VER3.1", new Vector3(0f, 0f, 0f), rot, 0);
                TITAN MyPT = myPTGO.GetComponent<TITAN>();
                myLastPT = MyPT;
                GameObject.Destroy(myPTGO.GetComponent<TITAN_CONTROLLER>());
                myPTGO.GetComponent<TITAN>().nonAI = true;
                myPTGO.GetComponent<TITAN>().speed = 30f;
                myPTGO.GetComponent<TITAN_CONTROLLER>().enabled = true;
                myPTGO.GetComponent<TITAN>().isCustomTitan = true;
            }
            if (FlatUI.Button(IndexToRect(0, 2, 1), "Do Stuff"))
            {
                ((PlayerTitanBot)myLastPT.controller).doStuff = !((PlayerTitanBot)myLastPT.controller).doStuff;
            }
            if (FlatUI.Button(IndexToRect(1, 2, 0), "Reset Position"))
            {
                myLastPT.transform.position = Vector3.zero;
                myLastPT.transform.rotation = Quaternion.identity;

            }
            if (FlatUI.Button(IndexToRect(1, 2, 1), "Clear Spheres"))
            {
                PTDataMachine.DeleteVisualizationSpheres();
            }
            if (FlatUI.Button(IndexToRect(2, 2, 1), "Show Data"))
            {
                foreach (PTAction key in HitData.AllHitboxData.Keys)
                {
                    foreach (HitData.MovesetData movesetData in HitData.AllHitboxData[key])
                    {
                        foreach (HitData.Hitbox hitbox in movesetData.hitboxes)
                        {
                            if (hitbox.GetType() == typeof(HitData.HitboxSphere))
                            {
                                PTDataMachine.CreateVisualizationSphere(hitbox.pos, ((HitData.HitboxSphere)hitbox).radius);
                            }
                            if (hitbox.GetType() == typeof(HitData.HitboxRectangle))
                            {
                                PTDataMachine.CreateColliderBox(hitbox.pos, ((HitData.HitboxRectangle)hitbox).RectangleDimentions, ((HitData.HitboxRectangle)hitbox).RectangleRotation);
                            }
                        }
                    }
                }
            }
            if (FlatUI.Button(IndexToRect(2, 2, 0), "Clear Data"))
            {
                HitData.AllHitboxData.Clear();
            }

            if (FlatUI.Button(IndexToRect(3, 8, 0), "↖"))
            {
                myLastPT.controller.targetDirection = 315f;
            }
            if (FlatUI.Button(IndexToRect(3, 8, 1), "↑"))
            {
                myLastPT.controller.targetDirection = 0f;
            }
            if (FlatUI.Button(IndexToRect(3, 8, 2), "↗"))
            {
                myLastPT.controller.targetDirection = 45f;
            }
            if (FlatUI.Button(IndexToRect(4, 8, 0), "←"))
            {
                myLastPT.controller.targetDirection = 270f;
            }
            if (FlatUI.Button(IndexToRect(4, 8, 1), "·"))
            {
                myLastPT.controller.targetDirection = -874f;
            }
            if (FlatUI.Button(IndexToRect(4, 8, 2), "→"))
            {
                myLastPT.controller.targetDirection = 90f;
            }
            if (FlatUI.Button(IndexToRect(5, 8, 0), "↙"))
            {
                myLastPT.controller.targetDirection = 225f;
            }
            if (FlatUI.Button(IndexToRect(5, 8, 1), "↓"))
            {
                myLastPT.controller.targetDirection = 180f;
            }
            if (FlatUI.Button(IndexToRect(5, 8, 2), "↘"))
            {
                myLastPT.controller.targetDirection = 135f;
            }
            if (FlatUI.Button(IndexToRect(3, 8, 4, 2), "combo", PTButtonColor))
            {
                myLastPT.controller.isAttackDown = true;
                if (resetPositionsOnAttack)
                {
                    myLastPT.transform.position = Vector3.zero;
                    myLastPT.transform.rotation = Quaternion.identity;
                }
                if (clearHitboxesOnAttack)
                {
                    PTDataMachine.DeleteVisualizationSpheres();
                }
                if (PTDataMachine.RecordData)
                {
                    PTDataMachine.StartRecordingHitbox(PTAction.Attack);
                }
            }
            if (FlatUI.Button(IndexToRect(3, 8, 6, 2), "slam", PTButtonColor))
            {
                myLastPT.controller.isAttackIIDown = true;
                if (resetPositionsOnAttack)
                {
                    myLastPT.transform.position = Vector3.zero;
                    myLastPT.transform.rotation = Quaternion.identity;
                }
                if (clearHitboxesOnAttack)
                {
                    PTDataMachine.DeleteVisualizationSpheres();
                }
                if (PTDataMachine.RecordData)
                {
                    PTDataMachine.StartRecordingHitbox(PTAction.AttackII);
                }
            }
            if (FlatUI.Button(IndexToRect(4, 8, 4, 2), "jump", PTButtonColor))
            {
                myLastPT.controller.isJumpDown = true;
                if (resetPositionsOnAttack)
                {
                    myLastPT.transform.position = Vector3.zero;
                    myLastPT.transform.rotation = Quaternion.identity;
                }
                if (clearHitboxesOnAttack)
                {
                    PTDataMachine.DeleteVisualizationSpheres();
                }
                if (PTDataMachine.RecordData)
                {
                    PTDataMachine.StartRecordingHitbox(PTAction.Jump);
                }
            }
            if (FlatUI.Button(IndexToRect(4, 8, 6, 2), "cover", PTButtonColor))
            {
                myLastPT.controller.cover = true;
                if (resetPositionsOnAttack)
                {
                    myLastPT.transform.position = Vector3.zero;
                    myLastPT.transform.rotation = Quaternion.identity;
                }
                if (clearHitboxesOnAttack)
                {
                    PTDataMachine.DeleteVisualizationSpheres();
                }
            }
            if (FlatUI.Button(IndexToRect(5, 8, 4, 2), "chopl"))
            {
                myLastPT.controller.chopl = true;
                if (resetPositionsOnAttack)
                {
                    myLastPT.transform.position = Vector3.zero;
                    myLastPT.transform.rotation = Quaternion.identity;
                }
                if (clearHitboxesOnAttack)
                {
                    PTDataMachine.DeleteVisualizationSpheres();
                }
            }
            if (FlatUI.Button(IndexToRect(5, 8, 6, 2), "chopr"))
            {
                myLastPT.controller.chopr = true;
                if (resetPositionsOnAttack)
                {
                    myLastPT.transform.position = Vector3.zero;
                    myLastPT.transform.rotation = Quaternion.identity;
                }
                if (clearHitboxesOnAttack)
                {
                    PTDataMachine.DeleteVisualizationSpheres();
                }
            }
            if (FlatUI.Button(IndexToRect(6, 8, 4, 2), "grabnapel", PTButtonColor))
            {
                myLastPT.controller.grabnapel = true;
                if (resetPositionsOnAttack)
                {
                    myLastPT.transform.position = Vector3.zero;
                    myLastPT.transform.rotation = Quaternion.identity;
                }
                if (clearHitboxesOnAttack)
                {
                    PTDataMachine.DeleteVisualizationSpheres();
                }
                if (PTDataMachine.RecordData)
                {
                    PTDataMachine.StartRecordingHitbox(PTAction.grabnapel);
                }
            }
            if (FlatUI.Button(IndexToRect(6, 8, 6, 2), "grabnaper", PTButtonColor))
            {
                myLastPT.controller.grabnaper = true;
                if (resetPositionsOnAttack)
                {
                    myLastPT.transform.position = Vector3.zero;
                    myLastPT.transform.rotation = Quaternion.identity;
                }
                if (clearHitboxesOnAttack)
                {
                    PTDataMachine.DeleteVisualizationSpheres();
                }
                if (PTDataMachine.RecordData)
                {
                    PTDataMachine.StartRecordingHitbox(PTAction.grabnaper);
                }
            }
            if (FlatUI.Button(IndexToRect(7, 8, 0, 2), "grabbackl", PTButtonColor))
            {
                myLastPT.controller.grabbackl = true;
                if (resetPositionsOnAttack)
                {
                    myLastPT.transform.position = Vector3.zero;
                    myLastPT.transform.rotation = Quaternion.identity;
                }
                if (clearHitboxesOnAttack)
                {
                    PTDataMachine.DeleteVisualizationSpheres();
                }
                if (PTDataMachine.RecordData)
                {
                    PTDataMachine.StartRecordingHitbox(PTAction.grabbackl);
                }
            }
            if (FlatUI.Button(IndexToRect(7, 8, 2, 2), "grabbackr", PTButtonColor))
            {
                myLastPT.controller.grabbackr = true;
                if (resetPositionsOnAttack)
                {
                    myLastPT.transform.position = Vector3.zero;
                    myLastPT.transform.rotation = Quaternion.identity;
                }
                if (clearHitboxesOnAttack)
                {
                    PTDataMachine.DeleteVisualizationSpheres();
                }
                if (PTDataMachine.RecordData)
                {
                    PTDataMachine.StartRecordingHitbox(PTAction.grabbackr);
                }
            }
            if (FlatUI.Button(IndexToRect(7, 8, 4, 2), "grabfrontl", PTButtonColor))
            {
                myLastPT.controller.grabfrontl = true;
                if (resetPositionsOnAttack)
                {
                    myLastPT.transform.position = Vector3.zero;
                    myLastPT.transform.rotation = Quaternion.identity;
                }
                if (clearHitboxesOnAttack)
                {
                    PTDataMachine.DeleteVisualizationSpheres();
                }
                if (PTDataMachine.RecordData)
                {
                    PTDataMachine.StartRecordingHitbox(PTAction.grabfrontl);
                }
            }
            if (FlatUI.Button(IndexToRect(7, 8, 6, 2), "grabfrontr", PTButtonColor))
            {
                myLastPT.controller.grabfrontr = true;
                if (resetPositionsOnAttack)
                {
                    myLastPT.transform.position = Vector3.zero;
                    myLastPT.transform.rotation = Quaternion.identity;
                }
                if (clearHitboxesOnAttack)
                {
                    PTDataMachine.DeleteVisualizationSpheres();
                }
                if (PTDataMachine.RecordData)
                {
                    PTDataMachine.StartRecordingHitbox(PTAction.grabfrontr);
                }
            }
            if (FlatUI.Button(IndexToRect(8, 8, 0, 2), "Suicide", PTButtonColor))
            {
                myLastPT.controller.isSuicide = true;
                if (resetPositionsOnAttack)
                {
                    myLastPT.transform.position = Vector3.zero;
                    myLastPT.transform.rotation = Quaternion.identity;
                }
                if (clearHitboxesOnAttack)
                {
                    PTDataMachine.DeleteVisualizationSpheres();
                }
            }
            if (FlatUI.Button(IndexToRect(8, 8, 2, 2), "choptl", PTButtonColor))
            {
                myLastPT.controller.choptl = true;
                if (resetPositionsOnAttack)
                {
                    myLastPT.transform.position = Vector3.zero;
                    myLastPT.transform.rotation = Quaternion.identity;
                }
                if (clearHitboxesOnAttack)
                {
                    PTDataMachine.DeleteVisualizationSpheres();
                }
                if (PTDataMachine.RecordData)
                {
                    PTDataMachine.StartRecordingHitbox(PTAction.choptl);
                }
            }
            if (FlatUI.Button(IndexToRect(8, 8, 4, 2), "choptr", PTButtonColor))
            {
                myLastPT.controller.choptr = true;
                if (resetPositionsOnAttack)
                {
                    myLastPT.transform.position = Vector3.zero;
                    myLastPT.transform.rotation = Quaternion.identity;
                }
                if (clearHitboxesOnAttack)
                {
                    PTDataMachine.DeleteVisualizationSpheres();
                }
                if (PTDataMachine.RecordData)
                {
                    PTDataMachine.StartRecordingHitbox(PTAction.choptr);
                }
            }
            if (FlatUI.Button(IndexToRect(8, 8, 6, 2), "bite", PTButtonColor))
            {
                myLastPT.controller.bite = true;
                if (resetPositionsOnAttack)
                {
                    myLastPT.transform.position = Vector3.zero;
                    myLastPT.transform.rotation = Quaternion.identity;
                }
                if (clearHitboxesOnAttack)
                {
                    PTDataMachine.DeleteVisualizationSpheres();
                }
                if (PTDataMachine.RecordData)
                {
                    PTDataMachine.StartRecordingHitbox(PTAction.bite);
                }
            }
            if (FlatUI.Button(IndexToRect(9, 8, 0, 2), "bitel", PTButtonColor))
            {
                myLastPT.controller.bitel = true;
                if (resetPositionsOnAttack)
                {
                    myLastPT.transform.position = Vector3.zero;
                    myLastPT.transform.rotation = Quaternion.identity;
                }
                if (clearHitboxesOnAttack)
                {
                    PTDataMachine.DeleteVisualizationSpheres();
                }
                if (PTDataMachine.RecordData)
                {
                    PTDataMachine.StartRecordingHitbox(PTAction.bitel);
                }
            }
            if (FlatUI.Button(IndexToRect(9, 8, 2, 2), "biter", PTButtonColor))
            {
                myLastPT.controller.biter = true;
                if (resetPositionsOnAttack)
                {
                    myLastPT.transform.position = Vector3.zero;
                    myLastPT.transform.rotation = Quaternion.identity;
                }
                if (clearHitboxesOnAttack)
                {
                    PTDataMachine.DeleteVisualizationSpheres();
                }
                if (PTDataMachine.RecordData)
                {
                    PTDataMachine.StartRecordingHitbox(PTAction.biter);
                }
            }
            if (FlatUI.Button(IndexToRect(9, 8, 4, 2), "sit"))
            {
                myLastPT.controller.sit = true;
                if (resetPositionsOnAttack)
                {
                    myLastPT.transform.position = Vector3.zero;
                    myLastPT.transform.rotation = Quaternion.identity;
                }
                if (clearHitboxesOnAttack)
                {
                    PTDataMachine.DeleteVisualizationSpheres();
                }
            }

            //New Attacks
            if (FlatUI.Button(IndexToRect(10, 8, 0, 2), "combo_2", PTButtonColor))
            {
                ((PlayerTitanBot)myLastPT.controller).is_combo_2 = true;
                if (resetPositionsOnAttack)
                {
                    myLastPT.transform.position = Vector3.zero;
                    myLastPT.transform.rotation = Quaternion.identity;
                }
                if (clearHitboxesOnAttack)
                {
                    PTDataMachine.DeleteVisualizationSpheres();
                }
                if (PTDataMachine.RecordData)
                {
                    PTDataMachine.StartRecordingHitbox(PTAction.combo_2);
                }
            }
            if (FlatUI.Button(IndexToRect(10, 8, 2, 2), "combo_3", PTButtonColor))
            {
                ((PlayerTitanBot)myLastPT.controller).is_combo_3 = true;
                if (resetPositionsOnAttack)
                {
                    myLastPT.transform.position = Vector3.zero;
                    myLastPT.transform.rotation = Quaternion.identity;
                }
                if (clearHitboxesOnAttack)
                {
                    PTDataMachine.DeleteVisualizationSpheres();
                }
                if (PTDataMachine.RecordData)
                {
                    PTDataMachine.StartRecordingHitbox(PTAction.combo_3);
                }

            }
            if (FlatUI.Button(IndexToRect(10, 8, 4, 2), "front_ground", PTButtonColor))
            {
                ((PlayerTitanBot)myLastPT.controller).is_front_ground = true;
                if (resetPositionsOnAttack)
                {
                    myLastPT.transform.position = Vector3.zero;
                    myLastPT.transform.rotation = Quaternion.identity;
                }
                if (clearHitboxesOnAttack)
                {
                    PTDataMachine.DeleteVisualizationSpheres();
                }
                if (PTDataMachine.RecordData)
                {
                    PTDataMachine.StartRecordingHitbox(PTAction.front_ground);
                }
            }
            if (FlatUI.Button(IndexToRect(10, 8, 6, 2), "kick", PTButtonColor))
            {
                ((PlayerTitanBot)myLastPT.controller).is_kick = true;
                if (resetPositionsOnAttack)
                {
                    myLastPT.transform.position = Vector3.zero;
                    myLastPT.transform.rotation = Quaternion.identity;
                }
                if (clearHitboxesOnAttack)
                {
                    PTDataMachine.DeleteVisualizationSpheres();
                }
                if (PTDataMachine.RecordData)
                {
                    PTDataMachine.StartRecordingHitbox(PTAction.kick);
                }
            }
            if (FlatUI.Button(IndexToRect(11, 8, 0, 2), "slap_back", PTButtonColor))
            {
                ((PlayerTitanBot)myLastPT.controller).is_slap_back = true;
                if (resetPositionsOnAttack)
                {
                    myLastPT.transform.position = Vector3.zero;
                    myLastPT.transform.rotation = Quaternion.identity;
                }
                if (clearHitboxesOnAttack)
                {
                    PTDataMachine.DeleteVisualizationSpheres();
                }
                if (PTDataMachine.RecordData)
                {
                    PTDataMachine.StartRecordingHitbox(PTAction.slap_back);
                }
            }
            if (FlatUI.Button(IndexToRect(11, 8, 2, 2), "slap_face", PTButtonColor))
            {
                ((PlayerTitanBot)myLastPT.controller).is_slap_face = true;
                if (resetPositionsOnAttack)
                {
                    myLastPT.transform.position = Vector3.zero;
                    myLastPT.transform.rotation = Quaternion.identity;
                }
                if (clearHitboxesOnAttack)
                {
                    PTDataMachine.DeleteVisualizationSpheres();
                }
                if (PTDataMachine.RecordData)
                {
                    PTDataMachine.StartRecordingHitbox(PTAction.slap_face);
                }
            }
            if (FlatUI.Button(IndexToRect(11, 8, 4, 2), "stomp", PTButtonColor))
            {
                ((PlayerTitanBot)myLastPT.controller).is_stomp = true;
                if (resetPositionsOnAttack)
                {
                    myLastPT.transform.position = Vector3.zero;
                    myLastPT.transform.rotation = Quaternion.identity;
                }
                if (clearHitboxesOnAttack)
                {
                    PTDataMachine.DeleteVisualizationSpheres();
                }
                if (PTDataMachine.RecordData)
                {
                    PTDataMachine.StartRecordingHitbox(PTAction.stomp);
                }
            }
            if (FlatUI.Button(IndexToRect(11, 8, 6, 2), "crawler_jump_0", PTButtonColor))
            {
                ((PlayerTitanBot)myLastPT.controller).is_crawler_jump_0 = true;
                if (resetPositionsOnAttack)
                {
                    myLastPT.transform.position = Vector3.zero;
                    myLastPT.transform.rotation = Quaternion.identity;
                }
                if (clearHitboxesOnAttack)
                {
                    PTDataMachine.DeleteVisualizationSpheres();
                }
                if (PTDataMachine.RecordData)
                {
                    PTDataMachine.StartRecordingHitbox(PTAction.crawler_jump_0);
                }
            }
            if (FlatUI.Button(IndexToRect(13), "Stop Recording", PTDataMachine.isRecording ? QuickMenu.PTButtonColor : FlatUI.insideColorTex))
            {
                PTDataMachine.FinishRecordingHitbox();
            }
            resetPositionsOnAttack = FlatUI.Check(IndexToRect(14, 2, 1), resetPositionsOnAttack, "Reset Position");
            if (FlatUI.Button(IndexToRect(15, 2, 0), "Save Data"))
            {
                PTDataMachine.SaveHitboxData(true);
            }
            if (FlatUI.Button(IndexToRect(15, 2, 1), "Load Data"))
            {
                PTDataMachine.LoadHitboxData();
            }
            KaneGameManager.doCameraRotation = FlatUI.Check(IndexToRect(16), KaneGameManager.doCameraRotation, "doCameraRotation");
            if (FlatUI.Button(IndexToRect(17, 4, 0), "--"))
            {
                KaneGameManager.cameraRotationSpeed -= 10f;
            }
            GUI.Label(IndexToRect(17, 4, 1, 2), "Speed :" + KaneGameManager.cameraRotationSpeed.ToString());
            if (FlatUI.Button(IndexToRect(17, 4, 3), "++"))
            {
                KaneGameManager.cameraRotationSpeed += 10f;
            }
            clearHitboxesOnAttack = FlatUI.Check(IndexToRect(18, 2, 0), clearHitboxesOnAttack, "clearHitboxes");
            PTDataMachine.KeepHitboxes = FlatUI.Check(IndexToRect(18, 2, 1), PTDataMachine.KeepHitboxes, "HitboxHistory");
            if (FlatUI.Button(IndexToRect(19, 2, 0), "setup stuff"))
            {
                ((PlayerTitanBot)myLastPT.controller).CalculateMovesetData();
            }
            if (FlatUI.Button(IndexToRect(19, 2, 1), "enable stuff"))
            {
                ((PlayerTitanBot)myLastPT.controller).doStuff = !((PlayerTitanBot)myLastPT.controller).doStuff;
                CGTools.log("Do stuff = " + ((PlayerTitanBot)myLastPT.controller).doStuff.ToString());
            }
            PTDataMachine.DrawHitboxes = FlatUI.Check(IndexToRect(20), PTDataMachine.DrawHitboxes, "DrawHitboxes");
            GUI.Label(IndexToRect(21, 4, 0, 2), "Prune Hitboxes");
            if (FlatUI.Button(IndexToRect(22), "Show Calculated Data"))
            {
                PlayerTitanBot pt = (PlayerTitanBot)myLastPT.controller;
                foreach (PTAction key in pt.MovesetDatabase.Keys)
                {
                    foreach (HitData.Hitbox hitbox in pt.MovesetDatabase[key].hitboxes)
                    {
                        if (hitbox.GetType() == typeof(HitData.HitboxSphere))
                        {
                            PTDataMachine.CreateVisualizationSphere(hitbox.pos, ((HitData.HitboxSphere)hitbox).radius);
                        }
                        if (hitbox.GetType() == typeof(HitData.HitboxRectangle))
                        {
                            PTDataMachine.CreateColliderBox(hitbox.pos, ((HitData.HitboxRectangle)hitbox).RectangleDimentions, ((HitData.HitboxRectangle)hitbox).RectangleRotation);
                        }
                    }
                }
            }
        }
        public static Rect IndexToRect(int i)
        {
            return new Rect(menuX + 4f, menuY + (float)(i * 30), 240f, 30f);
        }
        public static Rect IndexToRect(int i, int divisions, int j)
        {
            float num = 240f;
            if (divisions < 1)
            {
                divisions = 1;
            }
            return new Rect(menuX + 4f + num / (float)divisions * (float)j, menuY + (float)i * 30f, num / (float)divisions, 30f);
        }
        public static Rect IndexToRect(int i, int divisions, int j, int n)
        {
            float num = 240f;
            if (divisions < 1)
            {
                divisions = 1;
            }
            return new Rect(menuX + 4f + num / (float)divisions * (float)j, menuY + (float)i * 30f, (float)n * (num / (float)divisions), 30f);
        }
        public static Rect IndexToRectMultiLine(int i, int rows)
        {
            return new Rect(menuX + 4f, menuY + (float)(i * 30), 240f, 30f * rows);
        }
        public static Rect IndexToRectMultiLine(int i, int divisions, int j, int n, int rows)
        {
            float num = 240f;
            if (divisions < 1)
            {
                divisions = 1;
            }
            return new Rect(menuX + 4f + num / (float)divisions * (float)j, menuY + (float)i * 30f, (float)n * (num / (float)divisions), 30f * rows);
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
        public static void Init()
        {
            for (int i = 0; i < tabColors.Length; i++)
            {
                tabColors[i] = CGTools.ColorTex(CGTools.randomColor());
            }
            PTButtonColor = CGTools.ColorTex(new Color(1f, 1f, 1f));
            menuX = (float)Screen.width / 2f - 350f + 1000f;
            menuY = (float)Screen.height / 2f - 250f;
        }
        public static float SetTextbox(Rect position, float source, string label, int arrayID)
        {
            if (textBoxText[arrayID] == null)
            {
                textBoxText[arrayID] = source.ToString();
            }
            GUI.Label(new Rect(position.x, position.y, position.width - 150f, position.height), label);
            textBoxText[arrayID] = GUI.TextField(new Rect(position.x + position.width - 150f, position.y, 100f, position.height), textBoxText[arrayID]);
            if (FlatUI.Button(new Rect(position.x + position.width - 50f, position.y, 50f, position.height), "✓"))
            {
                return Convert.ToSingle(textBoxText[arrayID]);
            }
            return source;
        }
        public static int SetTextbox(Rect position, int source, string label, int arrayID)
        {
            if (textBoxText[arrayID] == null)
            {
                textBoxText[arrayID] = source.ToString();
            }
            GUI.Label(new Rect(position.x, position.y, position.width - 150f, position.height), label);
            textBoxText[arrayID] = GUI.TextField(new Rect(position.x + position.width - 150f, position.y, 100f, position.height), textBoxText[arrayID]);
            if (FlatUI.Button(new Rect(position.x + position.width - 50f, position.y, 50f, position.height), "✓"))
            {
                return Convert.ToInt32(textBoxText[arrayID]);
            }
            return source;
        }
    }
}
