using TitanBot.FlatUI5;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Utility;

namespace TitanBot
{
    public class KaneGameManager : MonoBehaviour
    {
        public static KaneGameManager instance;
        public static bool toggleQuickMenu = false;
        public static string GameVersionString = "PTBot 1.0 (Dev)";
        public static bool doCameraRotation = false;
        public static float cameraRotationSpeed = 30f;

        public static void Init()
        {
            instance = SingletonFactory.CreateSingleton(instance);
            QuickMenu.Init();
            FlatUI.Init();
        }

        public void OnGUI()
        {
            if (toggleQuickMenu)
            {
                QuickMenu.OnGUI();
            }
            //GUI.Label(new Rect(200f, 200f, 200f, 30f), Time.deltaTime.ToString() + ":" + cameraRotationSpeed.ToString());
        }

        public void Update()
        {
            PTDataMachine.UpdatePlayerCapsule();
            if (doCameraRotation)
            {
                Camera.main.transform.RotateAround(Vector3.zero, Vector3.up, cameraRotationSpeed * Time.deltaTime);
            }
            if (Input.GetKeyDown(KeyCode.Mouse3) || Input.GetKeyDown(KeyCode.KeypadPeriod))
            {
                if (!toggleQuickMenu)
                {
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
    }
}
