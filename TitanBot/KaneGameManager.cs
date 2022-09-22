using System;
using System.Collections.Generic;
using System.IO;
using TitanBot.FlatUI5;
using UnityEngine;
using Utility;

namespace TitanBot
{
    public class KaneGameManager : MonoBehaviour
    {
        public static KaneGameManager instance;
        public static bool toggleQuickMenu = false;
        public static string GameVersionString = "PTBot 1.62 (Dev)";
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
        public static int InfTitanCount = 5;
        public static bool doInfiniteTitans = false;
        public static bool sendJoinMessage = true;

        private static GUIStyle TagStyle = new GUIStyle
        {
            border = new RectOffset(1, 1, 1, 1),
            alignment = TextAnchor.LowerRight,
            fontSize = 14,
            fontStyle = FontStyle.Bold,
            normal = { textColor = Color.white }
        };

        //every function that should be deleted or modified before release should call this fuction so it can be found through the references
        public static void MarkDevFunction()
        {
           
        }

        public static void Init()
        {
            instance = SingletonFactory.CreateSingleton(instance);
            QuickMenu.Init();
            FlatUI.Init();
            CGLog.Start();
            CGTools.Init();
            PTDataMachine.LoadHitboxData();
            LoadConfig();
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
                    GUI.Label(new Rect(Screen.width - TagWidth - 10f, Screen.height - TagHeight - 10f, TagWidth, TagHeight), GameVersionString + Environment.NewLine + "Press \"" + QuickMenuKey.ToString() + "\" to Open Quick Menu", TagStyle);
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
                        QuickMenu.infiniteTitanTextBox = i.ToString();
                        InfTitanCount = i;
                    }
                    else
                    {
                        CGTools.log("Could not parse Setting [InfiniteTitanCount]");
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
                CGTools.log("Loaded config from " + Path + "\"Config.txt\"");
            }
        }

        public static void SaveConfig()
        {
            Dictionary<string, string> config = new Dictionary<string, string>();
            config.Add("QuickMenuHotkey", QuickMenuKey.ToString());
            config.Add("SendJoinMessage", sendJoinMessage.ToString());
            config.Add("InfiniteTitanCount", InfTitanCount.ToString());
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
            InfTitanCount = 5;
            QuickMenu.infiniteTitanTextBox = "5";
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


        public void teleportTitansIfAtSpawn()
        {
            if (doSpawnTeleporting && FengGameManagerMKII.instance.gameStart && PhotonNetwork.isMasterClient)
            {
                Vector3 spawnPos = new Vector3(0f, 0f, -530f);
                Vector3 returnPos = new Vector3(0f, 0f, 530f);
                float rad = 200f;
                bool flag = false;
                if (FengGameManagerMKII.level == "The City")
                {
                    spawnPos = new Vector3(0f, 0f, -530f);
                    returnPos = new Vector3(0f, 0f, 530f);
                    flag = true;
                }
                if (FengGameManagerMKII.level == "The Forest")
                {
                    spawnPos = new Vector3(-50f, 0f, -510f);
                    returnPos = new Vector3(-30f, 0f, 500f);
                    rad = 100f;
                    flag = true;
                }
                if (flag)
                {
                    foreach (GameObject g in GameObject.FindGameObjectsWithTag("titan"))
                    {
                        if (Vector3.Distance(g.transform.position, spawnPos) < rad)
                        {
                            if (g.GetPhotonView().isMine)
                                g.transform.position = returnPos;
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
                    CGLog.showLogGui = true;
                    toggleQuickMenu = true;
                    if (!Camera.main.GetComponent<IN_GAME_MAIN_CAMERA>().enabled)
                    {
                        IN_GAME_MAIN_CAMERA.cameraMode = CAMERA_TYPE.WOW;
                        Screen.showCursor = true;
                        Screen.lockCursor = false;
                        if ((int)FengGameManagerMKII.settingsOld[64] < 100)
                        {
                            Camera.main.GetComponent<SpectatorMovement>().disable = true;
                            Camera.main.GetComponent<MouseLook>().disable = true;
                        }
                        if ((int)FengGameManagerMKII.settingsOld[64] >= 100)
                        {
                            Camera.main.GetComponent<MouseLook>().enabled = false;
                        }
                    }
                    return;
                }
                CGLog.showLogGui = false;
                toggleQuickMenu = false;
                if (!Camera.main.GetComponent<IN_GAME_MAIN_CAMERA>().enabled)
                {
                    IN_GAME_MAIN_CAMERA.cameraMode = CAMERA_TYPE.TPS;
                    Screen.showCursor = true;
                    Screen.lockCursor = true;
                    if ((int)FengGameManagerMKII.settingsOld[64] < 100)
                    {
                        Camera.main.GetComponent<SpectatorMovement>().disable = false;
                        Camera.main.GetComponent<MouseLook>().disable = false;
                    }
                    if ((int)FengGameManagerMKII.settingsOld[64] >= 100)
                    {
                        Camera.main.GetComponent<MouseLook>().enabled = true;
                    }
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




        public static void InfiniteTitanHandler()
        {
            if (doInfiniteTitans && FengGameManagerMKII.instance.gameStart && PhotonNetwork.isMasterClient && GameObject.FindGameObjectsWithTag("titan").Length < InfTitanCount)
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
                GameObject.Destroy(myPTGO.GetComponent<TITAN_CONTROLLER>());
                myPTGO.GetComponent<TITAN>().nonAI = true;
                myPTGO.GetComponent<TITAN>().speed = 30f;
                myPTGO.GetComponent<TITAN_CONTROLLER>().enabled = true;
                myPTGO.GetComponent<TITAN>().isCustomTitan = true;
            }

        }
    }
}
