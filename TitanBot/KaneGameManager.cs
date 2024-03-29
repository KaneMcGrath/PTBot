﻿using StinkMod;
using System;
using System.Collections.Generic;
using System.IO;
using TitanBot.FlatUI5;
using TitanBot.Windows;
using UnityEngine;
using Utility;

namespace TitanBot
{
    public class KaneGameManager : MonoBehaviour
    {
        public static KaneGameManager instance;
        public static bool toggleQuickMenu = false;
        public static string GameVersionString = "PTBot 1.85";
        public static bool doCameraRotation = false;
        public static float cameraRotationSpeed = 30f;
        public static string Path = Application.dataPath + "/PTBot/";
        public static bool waitToAnnounce = false;
        public static float waitToAnnounceTimer = -999999f;
        public static KeyCode QuickMenuKey = KeyCode.I;
        public static float TagWidth = 200f;
        public static float TagHeight = 40f;

        private static float movetoRPCTimer = 0f;
        public static bool doSpawnTeleporting = false;
        public static int InfPTBotCount = 2;
        public static int InfiniteTitansCount = 10;
        public static bool doInfiniteTitans = false;
        public static bool sendJoinMessage = true;
        public static bool ShowHotkeyNotification = true;
        private static float spawnRad = 420f;

        public static bool isDevMode = false;

        private static GUIStyle TagStyle = new GUIStyle
        {
            border = new RectOffset(1, 1, 1, 1),
            alignment = TextAnchor.LowerRight,
            fontSize = 14,
            fontStyle = FontStyle.Bold,
            normal = { textColor = Color.white }
        };

        public static void Init()
        {
            instance = SingletonFactory.CreateSingleton(instance);
            QuickMenu.Init();
            FlatUI.Init();
            CGDebugConsole.Init();
            CGLog.Start();
            CGTools.Init();
            PTDataMachine.LoadHitboxData();
            MovesetControl.Init();
            MovesetControlWindow.Init();
            ToolTipWindow.Init();
            LoadConfig();
            if (isDevMode)
            {
                QuickMenu.tabNames = new string[]
                {
                    "PTBot Settings",
                    "Game Settings",
                    "Config",
                    "Controller"
                };
            }
            CGTools.log(GameVersionString);
        }

        public void OnGUI()
        {
            CGLog.OnGUI();
            CGTools.OnGUI();
            if (toggleQuickMenu)
            {
                QuickMenu.OnGUI();
            }
            else
            {
                if (FengGameManagerMKII.instance.gameStart)
                {
                    if (ShowHotkeyNotification) GUI.Label(new Rect(Screen.width - TagWidth - 10f, Screen.height - TagHeight - 20f, TagWidth, TagHeight), GameVersionString + Environment.NewLine + "Press \"" + QuickMenuKey.ToString() + "\" to Open Quick Menu", TagStyle);
                }
            }
            //GUI.Label(new Rect(200f, 200f, 200f, 30f), Time.deltaTime.ToString() + ":" + cameraRotationSpeed.ToString());
        }

        public static void LoadConfig()
        {
            if (File.Exists(Path + "Config.txt"))
            {
                Dictionary<string, string> config = new Dictionary<string, string>();
                string[] lines = File.ReadAllLines(Path + "Config.txt");
                for (int i = 0; i < lines.Length; i++)
                {
                    string line = lines[i];

                    string[] parts = line.Split(':');
                    if (parts.Length == 2)
                    {
                        config.Add(parts[0], parts[1]);
                    }
                    else
                        CGTools.log("Error on line " + i + ".  Makes sure each setting is on a seperate line");
                }
                if (config.ContainsKey("QuickMenuHotkey"))
                {
                    try
                    {
                        QuickMenuKey = (KeyCode)Enum.Parse(typeof(KeyCode), config["QuickMenuHotkey"]);
                    }
                    catch (Exception e)
                    {
                        CGTools.log("Could not parse Setting [QuickMenuHotkey] : " + e.Message);
                    }
                }
                if (config.ContainsKey("SendJoinMessage"))
                {
                    if (bool.TryParse(config["SendJoinMessage"], out bool b))
                    {
                        sendJoinMessage = b;
                    }
                    else
                    {
                        CGTools.log("Could not parse Setting [SendJoinMessage]");
                    }
                }
                if (config.ContainsKey("InfiniteTitanCount"))
                {
                    if (int.TryParse(config["InfiniteTitanCount"], out int i))
                    {
                        QuickMenu.infinitePTBotTextBox = i.ToString();
                        InfPTBotCount = i;
                    }
                    else
                    {
                        CGTools.log("Could not parse Setting [InfiniteTitanCount]");
                    }
                }
                if (config.ContainsKey("InfiniteNormalTitanCount"))
                {
                    if (int.TryParse(config["InfiniteNormalTitanCount"], out int i))
                    {
                        QuickMenu.infiniteNormalTitanTextBox = i.ToString();
                        InfiniteTitansCount = i;
                    }
                    else
                    {
                        CGTools.log("Could not parse Setting [InfiniteNormalTitanCount]");
                    }
                }
                if (config.ContainsKey("ReplaceSpawnedTitans"))
                {
                    if (bool.TryParse(config["ReplaceSpawnedTitans"], out bool b))
                    {
                        PlayerTitanBot.ReplaceSpawnedTitans = b;
                    }
                    else
                    {
                        CGTools.log("Could not parse Setting [ReplaceSpawnedTitans]");
                    }
                }
                if (config.ContainsKey("doSpawnTeleporting"))
                {
                    if (bool.TryParse(config["doSpawnTeleporting"], out bool b))
                    {
                        KaneGameManager.doSpawnTeleporting = b;
                    }
                    else
                    {
                        CGTools.log("Could not parse Setting [doSpawnTeleporting]");
                    }
                }
                if (config.ContainsKey("Difficulty"))
                {
                    try
                    {
                        PTTools.difficulty = (Difficulty)Enum.Parse(typeof(Difficulty), config["Difficulty"]);
                    }
                    catch (Exception e)
                    {
                        CGTools.log("Could not parse Setting [Difficulty] : " + e.Message);
                    }
                }
                if (config.ContainsKey("Moves"))
                {
                    try
                    {
                        PlayerTitanBot.TempActionsList.Clear();
                        if (config["Moves"].IsNullOrEmpty())
                        {

                        }
                        else
                        {
                            string[] moves = config["Moves"].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                            foreach (string move in moves)
                            {
                                PlayerTitanBot.TempActionsList.Add((PTAction)Enum.Parse(typeof(PTAction), move));
                            }
                        }
                        PlayerTitanBot.pTActions = PlayerTitanBot.TempActionsList.ToArray();
                    }
                    catch (Exception e)
                    {
                        CGTools.log("Could not parse Setting [Moves] : " + e.Message);
                    }
                }
                if (config.ContainsKey("PruningLevel"))
                {
                    if (int.TryParse(config["PruningLevel"], out int i))
                    {
                        QuickMenu.prunningSettingTextbox = i.ToString();
                        PlayerTitanBot.dataPruningLevel = i;
                    }
                    else
                    {
                        CGTools.log("Could not parse Setting [PruningLevel]");
                    }
                }
                if (config.ContainsKey("TitanName"))
                {
                    PlayerTitanBot.TitanName = config["TitanName"];
                    QuickMenu.titanNameTextBox = config["TitanName"];
                }
                if (config.ContainsKey("Timing"))
                {
                    bool hasmoves = config.ContainsKey("Moves");
                    string[] individuals = config["Timing"].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string s in individuals)
                    {
                        string[] split = s.Split('>');
                        PTAction action = (PTAction)Enum.Parse(typeof(PTAction), split[0]);
                        float StartAt = 0f;
                        if (float.TryParse(split[1], out StartAt))
                        {
                            MovesetControl.movesetControlDatabase[action].startAnimationAt = StartAt;
                        }
                        else
                        {
                            CGLog.log("Could not parse Timing for attack: " + split[0]);
                        }
                    }
                }
                if (config.ContainsKey("SpeedEnabled"))
                {
                    if (bool.TryParse(config["SpeedEnabled"], out bool b))
                    {
                        PlayerTitanBot.useCustomSpeed = b;
                    }
                    else
                    {
                        CGTools.log("Could not parse Setting [SpeedEnabled]");
                    }
                }
                if (config.ContainsKey("Speed"))
                {
                    if (float.TryParse(config["Speed"], out float f))
                    {
                        QuickMenu.speedTextBox = f.ToString();
                        PlayerTitanBot.titanSpeed = f;
                    }
                    else
                    {
                        CGTools.log("Could not parse Setting [Speed]");
                    }
                }
                if (config.ContainsKey("Dev"))
                {
                    isDevMode = true;
                }
                if (config.ContainsKey("AllowEyeHits"))
                {
                    if (bool.TryParse(config["AllowEyeHits"], out bool b))
                    {
                        PlayerTitanBot.AllowEyeHits = b;
                    }
                    else
                    {
                        CGTools.log("Could not parse Setting [AllowEyeHits]");
                    }
                }
                if (config.ContainsKey("AllowAnkleHits"))
                {
                    if (bool.TryParse(config["AllowAnkleHits"], out bool b))
                    {
                        PlayerTitanBot.AllowAnkleHits = b;
                    }
                    else
                    {
                        CGTools.log("Could not parse Setting [AllowAnkleHits]");
                    }
                }
                if (config.ContainsKey("ShowHotkeyNotification"))
                {
                    if (bool.TryParse(config["ShowHotkeyNotification"], out bool b))
                    {
                        ShowHotkeyNotification = b;
                    }
                    else
                    {
                        CGTools.log("Could not parse Setting [ShowHotkeyNotification]");
                    }
                }
                MovesetControlWindow.UpdateWindowData();
                CGTools.log("Loaded config from " + Path + "\"Config.txt\"");
            }
        }

        public static void SaveConfig()
        {
            Dictionary<string, string> config = new Dictionary<string, string>();
            config.Add("QuickMenuHotkey", QuickMenuKey.ToString());
            config.Add("SendJoinMessage", sendJoinMessage.ToString());
            config.Add("InfiniteTitanCount", InfPTBotCount.ToString());
            config.Add("InfiniteNormalTitanCount", InfiniteTitansCount.ToString());
            config.Add("ReplaceSpawnedTitans", PlayerTitanBot.ReplaceSpawnedTitans.ToString());
            config.Add("doSpawnTeleporting", doSpawnTeleporting.ToString());
            config.Add("Difficulty", PTTools.difficulty.ToString());
            config.Add("PruningLevel", PlayerTitanBot.dataPruningLevel.ToString());
            config.Add("TitanName", PlayerTitanBot.TitanName);
            string moves = "";
            if (PlayerTitanBot.pTActions.Length > 0)
            {
                for (int i = 0; i < PlayerTitanBot.pTActions.Length; i++)
                {
                    PTAction action = PlayerTitanBot.pTActions[i];
                    moves += action.ToString();
                    if (i != PlayerTitanBot.pTActions.Length - 1)
                        moves += ',';
                }
            }
            config.Add("Moves", moves);

            string moveTiming = "";
            PTAction[] actions = new PTAction[MovesetControl.movesetControlDatabase.Keys.Count];
            MovesetControl.movesetControlDatabase.Keys.CopyTo(actions, 0);
            for (int i = 0; i < actions.Length; i++)
            {
                TitanMove move = MovesetControl.movesetControlDatabase[actions[i]];
                moveTiming += actions[i].ToString() + ">" + move.startAnimationAt.ToString();
                if (i != actions.Length - 1)
                    moveTiming += ',';
            }
            config.Add("Timing", moveTiming);
            config.Add("SpeedEnabled", PlayerTitanBot.useCustomSpeed.ToString());
            config.Add("Speed", PlayerTitanBot.titanSpeed.ToString());
            if (KaneGameManager.isDevMode)
            {
                config.Add("Dev", "True");
            }
            config.Add("AllowEyeHits", PlayerTitanBot.AllowEyeHits.ToString());
            config.Add("AllowAnkleHits", PlayerTitanBot.AllowAnkleHits.ToString());
            config.Add("ShowHotkeyNotification", ShowHotkeyNotification.ToString());
            List<string> lines = new List<string>();
            foreach (string key in config.Keys)
            {
                lines.Add(key + ":" + config[key]);
            }
            try
            {
                File.WriteAllLines(Path + "Config.txt", lines.ToArray());
                CGTools.log("Saved config to " + Path + "\"Config.txt\"");
            }
            catch (Exception e)
            {
                CGTools.log(e.Message);
            }
        }

        public static void ResetDefaults()
        {
            sendJoinMessage = true;
            InfPTBotCount = 2;
            QuickMenu.infinitePTBotTextBox = "2";
            PlayerTitanBot.ReplaceSpawnedTitans = false;
            doSpawnTeleporting = false;
            PTTools.difficulty = Difficulty.VeryVeryHard;
            PlayerTitanBot.dataPruningLevel = 2;
            QuickMenu.prunningSettingTextbox = "2";
            PlayerTitanBot.pTActions = new PTAction[] { PTAction.Attack, PTAction.Jump, PTAction.choptl, PTAction.choptr };
            PlayerTitanBot.TempActionsList.Clear();
            PlayerTitanBot.TempActionsList.AddRange(PlayerTitanBot.pTActions);
            PlayerTitanBot.TitanName = "PTBot";
            QuickMenu.titanNameTextBox = "PTBot";
            MovesetControl.SetDefaults();
            MovesetControlWindow.UpdateWindowData();
            PlayerTitanBot.useCustomSpeed = false;
            PlayerTitanBot.titanSpeed = 60f;
            QuickMenu.speedTextBox = "60";
            QuickMenu.infiniteNormalTitanTextBox = "10";
            InfiniteTitansCount = 10;
            ShowHotkeyNotification = true;
            PlayerTitanBot.AllowAnkleHits = true;
            PlayerTitanBot.AllowEyeHits = true;
            CGTools.log("Reset settings to default values");
        }

        private void Announce()
        {
            string text = string.Concat(new object[]
            {
               "[FF8000]<b>",
               KaneGameManager.GameVersionString,
               "\n",
               "</b>[eaef5d]<b>Created by ",
               "</b>[FF8000]<b>Avisite",
               "\n",
               "</b>[FFFFFF]<b>discord.gg/BgaBuhT",
               "\n",
               "</b>[5DDE8E]<b>My Ping is currently ",
               PhotonNetwork.GetPing().ToString(),
               "ms</b>",

            });
            FengGameManagerMKII.instance.photonView.RPC("Chat", PhotonTargets.All, new object[]
            {
                text.hexColor(),
                ""
            });
        }

        private float checkSpawnsTimer = 0f;

        public void teleportTitansIfAtSpawn()
        {
            if (!CGTools.timer(ref checkSpawnsTimer, 0.2f)) return;
            if (PhotonNetwork.isMasterClient)
            {
                if (doSpawnTeleporting && FengGameManagerMKII.instance.gameStart)
                {
                    Vector3 spawnPos = new Vector3(0f, 0f, -530f);
                    Vector3 GatePos = new Vector3(0f, 0f, 750f);
                    float spawnRad = 200f;
                    float gateRad = 60f;
                    bool flag = false;
                    if (FengGameManagerMKII.level == "The City")
                    {
                        spawnPos = new Vector3(0f, 0f, -530f);
                        GatePos = new Vector3(0f, 0f, 750f);
                        flag = true;
                    }
                    if (flag)
                    {
                        foreach (GameObject g in GameObject.FindGameObjectsWithTag("titan"))
                        {
                            if (Vector3.Distance(g.transform.position, spawnPos) < spawnRad || Vector3.Distance(g.transform.position, GatePos) < gateRad)
                            {
                                float x = UnityEngine.Random.Range(-360f, 360f);
                                float z = UnityEngine.Random.Range(-360f, 360f);
                                Vector3 returnPos = new Vector3(x, 10f, z);
                                if (g.GetPhotonView().isMine)
                                {
                                    g.transform.position = returnPos;
                                }
                                else
                                {
                                    if (PhotonNetwork.isMasterClient)
                                    {
                                        if (CGTools.timer(ref movetoRPCTimer, 2f))
                                        {
                                            g.GetComponent<TITAN>().photonView.RPC("moveToRPC", g.GetPhotonView().owner, new object[]
                                            {
                                                returnPos.x,
                                                returnPos.y,
                                                returnPos.z
                                            });
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            //else
            //{
            //    Vector3 spawnPos = new Vector3(0f, 0f, -530f);
            //    Vector3 GatePos = new Vector3(0f, 0f, 750f);
            //    float spawnRad = 200f;
            //    float gateRad = 60f;
            //    bool flag = false;
            //    if (FengGameManagerMKII.level == "The City")
            //    {
            //        spawnPos = new Vector3(0f, 0f, -530f);
            //        GatePos = new Vector3(0f, 0f, 750f);
            //        flag = true;
            //    }
            //    if (FengGameManagerMKII.level == "The Forest")
            //    {
            //        spawnPos = new Vector3(-50f, 0f, -510f);
            //        GatePos = new Vector3(-30f, 0f, 500f);
            //        spawnRad = 100f;
            //        flag = true;
            //    }
            //    if (flag)
            //    {
            //        foreach (GameObject g in GameObject.FindGameObjectsWithTag("titan"))
            //        {
            //            if (Vector3.Distance(g.transform.position, spawnPos) < spawnRad || Vector3.Distance(g.transform.position, GatePos) < gateRad)
            //            {
            //                if (g.GetPhotonView().isMine)
            //                {
            //                    float x = UnityEngine.Random.Range(-360f, 360f);
            //                    float z = UnityEngine.Random.Range(-360f, 360f);
            //                    g.transform.position = new Vector3(x, 10f, z);
            //                }
            //            }
            //        }
            //    }
            //}
        }

        private static CAMERA_TYPE lastCameraType;

        public void Update()
        {
            InfiniteTitanHandler();
            teleportTitansIfAtSpawn();
            CGTools.Update();
            if (doCameraRotation)
            {
                Camera.main.transform.RotateAround(Vector3.zero, Vector3.up, cameraRotationSpeed * Time.deltaTime);
            }
            if (Input.GetKeyDown(QuickMenuKey))
            {
                if (!toggleQuickMenu)
                {
                    QuickMenu.QuickMenuWindow.showWindow = true;
                    CGLog.showLogGui = true;
                    toggleQuickMenu = true;
                    if (FengGameManagerMKII.instance.gameStart)
                    {
                        lastCameraType = IN_GAME_MAIN_CAMERA.cameraMode;
                        IN_GAME_MAIN_CAMERA.cameraMode = CAMERA_TYPE.WOW;
                        Screen.showCursor = true;
                        Screen.lockCursor = false;
                    }
                    return;
                }
                CGLog.showLogGui = false;
                toggleQuickMenu = false;
                if (FengGameManagerMKII.instance.gameStart)
                {
                    IN_GAME_MAIN_CAMERA.cameraMode = lastCameraType;
                    Screen.showCursor = true;
                    Screen.lockCursor = true;
                    return;
                }
            }
            if (!PTDataMachine.KeepHitboxes)
            {
                if (PTDataMachine.hitboxWaitCounter < 0)
                {
                    if (PTDataMachine.hitSphere != null)
                        PTDataMachine.hitSphere.transform.position = new Vector3(0f, -999f, 0f);
                    if (PTDataMachine.hitBox != null)
                        PTDataMachine.hitBox.transform.position = new Vector3(0f, -999f, 0f);

                }
                else
                {
                    PTDataMachine.hitboxWaitCounter--;
                }
            }
        }

        public static void OnJoinedRoom()
        {
        }

        public static void OnInstantiate(PhotonPlayer player, string key, GameObject GO)
        {

        }

        public static void OnPhotonPlayerConnected(PhotonPlayer photonPlayer)
        {
            if (PhotonNetwork.isMasterClient && sendJoinMessage)
            {
                string text = string.Concat(new object[]
                {
                   "[FF8000]<b>",
                   KaneGameManager.GameVersionString,
                   "\n",
                   "</b>[eaef5d]<b>Created by ",
                   "</b>[FF8000]<b>Avisite",
                   "\n",
                   "</b>[FFFFFF]<b>github.com/KaneMcGrath/PTBot",
                   "\n",
                   "</b>[FFFFFF]<b>discord.gg/BgaBuhT",
                   "\n",
                   "</b>[00bfff]<b>My Ping is currently ",
                   (PhotonNetwork.GetPing()).ToString(),
                   "</b>"
                });
                FengGameManagerMKII.instance.photonView.RPC("Chat", photonPlayer, new object[]
                {
                    text.hexColor(),
                    ""
                });

            }
        }

        /// <summary>
        /// For use with Custom Logic
        /// </summary>
        public static void SpawnPTBotAction(float size, int health, int number)
        {
            Vector3 position = new Vector3(UnityEngine.Random.Range((float)-400f, (float)400f), 0f, UnityEngine.Random.Range((float)-400f, (float)400f));
            Quaternion rotation = new Quaternion(0f, 0f, 0f, 1f);
            if (FengGameManagerMKII.instance.titanSpawns.Count > 0)
            {
                position = FengGameManagerMKII.instance.titanSpawns[UnityEngine.Random.Range(0, FengGameManagerMKII.instance.titanSpawns.Count)];
            }
            else
            {
                GameObject[] objArray = GameObject.FindGameObjectsWithTag("titanRespawn");
                if (objArray.Length > 0)
                {
                    int index = UnityEngine.Random.Range(0, objArray.Length);
                    GameObject obj2 = objArray[index];
                    position = obj2.transform.position;
                    rotation = obj2.transform.rotation;
                }
            }
            for (int i = 0; i < number; i++)
            {
                GameObject obj3 = PhotonNetwork.Instantiate("TITAN_VER3.1", position, rotation, 0);
                obj3.GetComponent<TITAN>().resetLevel(size);
                obj3.GetComponent<TITAN>().hasSetLevel = true;
                if (health > 0f)
                {
                    obj3.GetComponent<TITAN>().currentHealth = health;
                    obj3.GetComponent<TITAN>().maxHealth = health;
                }
                obj3.GetComponent<TITAN>().setAbnormalType2(AbnormalType.NORMAL, false);
                GameObject.Destroy(obj3.GetComponent<TITAN_CONTROLLER>());
                obj3.GetComponent<TITAN>().nonAI = true;
                obj3.GetComponent<TITAN>().speed = 30f;
                obj3.GetComponent<TITAN_CONTROLLER>().enabled = true;
                obj3.GetComponent<TITAN>().isCustomTitan = true;
            }
        }

        /// <summary>
        /// For use with Custom Logic
        /// </summary>
        public static void SpawnPTBotAtAction(float size, int health, int number, float posX, float posY, float posZ)
        {
            Vector3 position = new Vector3(posX, posY, posZ);
            Quaternion rotation = new Quaternion(0f, 0f, 0f, 1f);
            for (int i = 0; i < number; i++)
            {
                GameObject obj2 = PhotonNetwork.Instantiate("TITAN_VER3.1", position, rotation, 0);
                obj2.GetComponent<TITAN>().resetLevel(size);
                obj2.GetComponent<TITAN>().hasSetLevel = true;
                if (health > 0f)
                {
                    obj2.GetComponent<TITAN>().currentHealth = health;
                    obj2.GetComponent<TITAN>().maxHealth = health;
                }
                obj2.GetComponent<TITAN>().setAbnormalType2(AbnormalType.NORMAL, false);
                GameObject.Destroy(obj2.GetComponent<TITAN_CONTROLLER>());
                obj2.GetComponent<TITAN>().nonAI = true;
                obj2.GetComponent<TITAN>().speed = 30f;
                obj2.GetComponent<TITAN_CONTROLLER>().enabled = true;
                obj2.GetComponent<TITAN>().isCustomTitan = true;
            }
        }

        private static float infiniteTitanTimer = 0f;
        public static void InfiniteTitanHandler()
        {
            if (!CGTools.timer(ref infiniteTitanTimer, 1f)) return;
            if (doInfiniteTitans && FengGameManagerMKII.instance.gameStart && PhotonNetwork.isMasterClient)
            {
                int realCount = InfiniteTitansCount + InfPTBotCount;
                GameObject[] ts = GameObject.FindGameObjectsWithTag("titan");
                if (ts.Length < realCount)
                {
                    int ptcount = 0;
                    int titancount = 0;
                    foreach (GameObject g in ts)
                    {
                        TITAN tt = g.GetComponent<TITAN>();
                        if (tt.isCustomTitan)
                        {
                            ptcount++;
                        }
                        else
                        {
                            titancount++;
                        }
                    }
                    if (titancount < InfiniteTitansCount)
                    {
                        Vector3 pos = Camera.main.transform.position;
                        Quaternion rot = Quaternion.identity;
                        float x = UnityEngine.Random.Range(-spawnRad, spawnRad);
                        float z = UnityEngine.Random.Range(-spawnRad, spawnRad);

                        Vector3 rayOrigin = new Vector3(x, 200f, z);
                        Vector3 rayDirection = Vector3.down;
                        Ray ray = new Ray(rayOrigin, rayDirection);

                        Vector3 spawnPosition = rayOrigin;

                        if (Physics.Raycast(ray, out RaycastHit raycastHit))
                        {
                            spawnPosition = raycastHit.point;
                        }


                        GameObject t = PhotonNetwork.Instantiate("TITAN_VER3.1", spawnPosition, rot, 0);
                        return;

                    }
                    if (ptcount < InfPTBotCount)
                    {
                        Vector3 pos = Camera.main.transform.position;
                        Quaternion rot = Quaternion.identity;
                        float x = UnityEngine.Random.Range(-spawnRad, spawnRad);
                        float z = UnityEngine.Random.Range(-spawnRad, spawnRad);

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
                        GameObject.Destroy(myPTGO.GetComponent<TITAN_CONTROLLER>());
                        myPTGO.GetComponent<TITAN>().nonAI = true;
                        myPTGO.GetComponent<TITAN>().speed = 30f;
                        myPTGO.GetComponent<TITAN_CONTROLLER>().enabled = true;
                        myPTGO.GetComponent<TITAN>().isCustomTitan = true;
                        return;
                    }
                }
                return;
            }
        }
    }
}
