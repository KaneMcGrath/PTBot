using TitanBot.FlatUI5;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Utility;
using System.IO;

namespace TitanBot
{
    public class KaneGameManager : MonoBehaviour
    {
        public static KaneGameManager instance;
        public static bool toggleQuickMenu = false;
        public static string GameVersionString = "PTBot 1.4 (Dev)";
        public static bool doCameraRotation = false;
        public static float cameraRotationSpeed = 30f;
        public static string Path = Application.dataPath + "/PTBot/";
        public static bool waitToAnnounce = false;
        public static float waitToAnnounceTimer = -999999f;

        private static float movetoRPCTimer = 0f;
        public static bool doSpawnTeleporting = false;
        public static int InfTitanCount = 5;
        public static bool doInfiniteTitans = false;
        public static bool sendJoinMessage = true;

        public static void Init()
        {
            instance = SingletonFactory.CreateSingleton(instance);
            QuickMenu.Init();
            FlatUI.Init();
            CGLog.Start();
            CGTools.Init();
            PTDataMachine.LoadHitboxData();
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
            //GUI.Label(new Rect(200f, 200f, 200f, 30f), Time.deltaTime.ToString() + ":" + cameraRotationSpeed.ToString());
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
            if (doSpawnTeleporting)
            {
                Vector3 spawnPos = new Vector3(0f, 0f, -530f);
                Vector3 returnPos = new Vector3(0f, 0f, 530f);
                float rad = 200f;
                bool flag = false;
                if (LevelInfo.getInfo(FengGameManagerMKII.currentLevel).mapName == "The City I")
                {
                    spawnPos = new Vector3(0f, 0f, -530f);
                    returnPos = new Vector3(0f, 0f, 530f);
                    flag = true;
                }
                if (LevelInfo.getInfo(FengGameManagerMKII.currentLevel).mapName == "The Forest")
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
            if (Input.GetKeyDown(KeyCode.Equals))
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
                   "</b>[FFFFFF]<b>https://discord.gg/BgaBuhT",
                   "\n",
                   "\n",
                   "</b>[00bfff]<b>My Ping is currently ",
                   (PhotonNetwork.GetPing()).ToString(),
                   "\n",
                   "</b>[ff4800]<b>This mod is still in development and I will probably crash randomly or DC</b>"
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
