using System;
using TitanBot.FlatUI5;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using static UIPopupList;

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
        public static string[] tabNames = new string[]
        {
            "TitanBot",
            "Hitbox Data"
        };

        private static bool resetPositionsOnAttack = false;
        private static bool clearHitboxesOnAttack = false;

        public static void OnGUI()
        {
            
            FlatUI.Box(new Rect(menuX, menuY, 250f, 700f), tabColors[tabIndex], FengGameManagerMKII.instance.textureBackgroundBlack);
            tabIndex = tabs(new Rect(menuX + 250f, menuY, 700f, 100f), tabNames, tabIndex, false, tabColors);
            if (tabIndex == 0) TabMain();
            if (tabIndex == 1) tabHitboxData();
        }

        public static void tabHitboxData()
        {
            if (FlatUI.Button(IndexToRect(1), "test Parse"))
            {
                PTDataMachine.parseTest();
            }
            GUI.Label(IndexToRect(2), "Record Hitboxes");
            if (myLastPT != null)
            {
                GUI.Label(IndexToRect(3), "Titan Pos : " + myLastPT.transform.position.ToString());
            }
            
        }

        public static TITAN myLastPT;
        
        public static void TabMain()
        {
            if (FlatUI.Button(IndexToRect(0,2,0), "Spawn PTBot"))
            {
                Vector3 pos = Camera.main.transform.position;
                Quaternion rot = Quaternion.identity;
                GameObject myPTGO = PhotonNetwork.Instantiate("TITAN_VER3.1", new Vector3(0f,0f,0f), rot, 0);
                TITAN MyPT = myPTGO.GetComponent<TITAN>();
                myLastPT = MyPT;
                GameObject.Destroy(myPTGO.GetComponent<TITAN_CONTROLLER>());
                myPTGO.GetComponent<TITAN>().nonAI = true;
                myPTGO.GetComponent<TITAN>().speed = 30f;
                myPTGO.GetComponent<TITAN_CONTROLLER>().enabled = true;
                
                myPTGO.GetComponent<TITAN>().isCustomTitan = true;
            }
            
            if (FlatUI.Button(IndexToRect(1,2,0), "Reset Position"))
            {
                myLastPT.transform.position = Vector3.zero;
                myLastPT.transform.rotation = Quaternion.identity;

            }
            if (FlatUI.Button(IndexToRect(1, 2, 1), "Clear Spheres"))
            {
                PTDataMachine.DeleteVisualizationSpheres();
            }
            GUI.Label(IndexToRect(2), "PTBot Test Controls");
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
            if (FlatUI.Button(IndexToRect(10), "Stop Recording", PTDataMachine.isRecording ? QuickMenu.PTButtonColor : FlatUI.insideColorTex))
            {
                PTDataMachine.FinishRecordingHitbox();
            }

            if (FlatUI.Button(IndexToRect(11, 3, 0), "Camera 1", CameraPositions[0] == Vector3.zero ? FlatUI.insideColorTex : QuickMenu.PTButtonColor))
            {
                int id = 0;
                if (CameraPositions[id] == Vector3.zero)
                {
                    CameraPositions[id] = Camera.main.transform.position;
                    CameraRotations[id] = Camera.main.transform.rotation;
                }
                Camera.main.transform.position = CameraPositions[id];
                Camera.main.transform.rotation = CameraRotations[id];
            }
            if (FlatUI.Button(IndexToRect(11, 3, 1), "Camera 2", CameraPositions[1] == Vector3.zero ? FlatUI.insideColorTex : QuickMenu.PTButtonColor))
            {
                int id = 1;
                if (CameraPositions[id] == Vector3.zero)
                {
                    CameraPositions[id] = Camera.main.transform.position;
                    CameraRotations[id] = Camera.main.transform.rotation;
                }
                Camera.main.transform.position = CameraPositions[id];
                Camera.main.transform.rotation = CameraRotations[id];
            }
            if (FlatUI.Button(IndexToRect(11, 3, 2), "Camera 3", CameraPositions[2] == Vector3.zero ? FlatUI.insideColorTex : QuickMenu.PTButtonColor))
            {
                int id = 2;
                if (CameraPositions[id] == Vector3.zero)
                {
                    CameraPositions[id] = Camera.main.transform.position;
                    CameraRotations[id] = Camera.main.transform.rotation;
                }
                Camera.main.transform.position = CameraPositions[id];
                Camera.main.transform.rotation = CameraRotations[id];
            }
            if (FlatUI.Button(IndexToRect(12, 3, 0), "Camera 4", CameraPositions[3] == Vector3.zero ? FlatUI.insideColorTex : QuickMenu.PTButtonColor))
            {
                int id = 3;
                if (CameraPositions[id] == Vector3.zero)
                {
                    CameraPositions[id] = Camera.main.transform.position;
                    CameraRotations[id] = Camera.main.transform.rotation;
                }
                Camera.main.transform.position = CameraPositions[id];
                Camera.main.transform.rotation = CameraRotations[id];
            }
            if (FlatUI.Button(IndexToRect(12, 3, 1), "Camera 5", CameraPositions[4] == Vector3.zero ? FlatUI.insideColorTex : QuickMenu.PTButtonColor))
            {
                int id = 4;
                if (CameraPositions[id] == Vector3.zero)
                {
                    CameraPositions[id] = Camera.main.transform.position;
                    CameraRotations[id] = Camera.main.transform.rotation;
                }
                Camera.main.transform.position = CameraPositions[id];
                Camera.main.transform.rotation = CameraRotations[id];
            }
            if (FlatUI.Button(IndexToRect(12, 3, 2), "Camera 6", CameraPositions[5] == Vector3.zero ? FlatUI.insideColorTex : QuickMenu.PTButtonColor))
            {
                int id = 5;
                if (CameraPositions[id] == Vector3.zero)
                {
                    CameraPositions[id] = Camera.main.transform.position;
                    CameraRotations[id] = Camera.main.transform.rotation;
                }
                Camera.main.transform.position = CameraPositions[id];
                Camera.main.transform.rotation = CameraRotations[id];
            }
            if (FlatUI.Button(IndexToRect(13, 3, 0), "Camera 7", CameraPositions[6] == Vector3.zero ? FlatUI.insideColorTex : QuickMenu.PTButtonColor))
            {
                int id = 6;
                if (CameraPositions[id] == Vector3.zero)
                {
                    CameraPositions[id] = Camera.main.transform.position;
                    CameraRotations[id] = Camera.main.transform.rotation;
                }
                Camera.main.transform.position = CameraPositions[id];
                Camera.main.transform.rotation = CameraRotations[id];
            }
            if (FlatUI.Button(IndexToRect(13, 3, 1), "Camera 8", CameraPositions[7] == Vector3.zero ? FlatUI.insideColorTex : QuickMenu.PTButtonColor))
            {
                int id = 7;
                if (CameraPositions[id] == Vector3.zero)
                {
                    CameraPositions[id] = Camera.main.transform.position;
                    CameraRotations[id] = Camera.main.transform.rotation;
                }
                Camera.main.transform.position = CameraPositions[id];
                Camera.main.transform.rotation = CameraRotations[id];
            }
            if (FlatUI.Button(IndexToRect(13, 3, 2), "Camera 9", CameraPositions[8] == Vector3.zero ? FlatUI.insideColorTex : QuickMenu.PTButtonColor))
            {
                int id = 8;
                if (CameraPositions[id] == Vector3.zero)
                {
                    CameraPositions[id] = Camera.main.transform.position;
                    CameraRotations[id] = Camera.main.transform.rotation;
                }
                Camera.main.transform.position = CameraPositions[id];
                Camera.main.transform.rotation = CameraRotations[id];
            }
            if (FlatUI.Button(IndexToRect(14), "Reset Cameras"))
            {
                CameraPositions = new Vector3[10];
            }

            resetPositionsOnAttack = FlatUI.Check(IndexToRect(15), resetPositionsOnAttack, "Reset Position On Attack");
            KaneGameManager.doCameraRotation = FlatUI.Check(IndexToRect(16), KaneGameManager.doCameraRotation, "doCameraRotation");
            if(FlatUI.Button(IndexToRect(17, 4, 0), "--"))
            {
                KaneGameManager.cameraRotationSpeed -= 10f;
            }
            GUI.Label(IndexToRect(17, 4, 1, 2), "Speed :" + KaneGameManager.cameraRotationSpeed.ToString());
            if (FlatUI.Button(IndexToRect(17, 4, 3), "++"))
            {
                KaneGameManager.cameraRotationSpeed += 10f;
            }
            clearHitboxesOnAttack = FlatUI.Check(IndexToRect(18), clearHitboxesOnAttack, "clearHitboxesOnAttack");
            PTDataMachine.KeepHitboxes = FlatUI.Check(IndexToRect(19), PTDataMachine.KeepHitboxes, "HitboxHistory");
            if (FlatUI.Button(IndexToRect(20), "setup stuff"))
            {
                ((PlayerTitanBot)myLastPT.controller).CalculateMovesetData(myLastPT.myLevel);
            }
            if (FlatUI.Button(IndexToRect(21), "enable stuff"))
            {
                ((PlayerTitanBot)myLastPT.controller).doStuff = !((PlayerTitanBot)myLastPT.controller).doStuff;
                CGTools.log("Do stuff = " + ((PlayerTitanBot)myLastPT.controller).doStuff.ToString());
            }

            showHitboxs = FlatUI.Check(IndexToRect(22), showHitboxs, "showHitboxes");
        }

       // if (!Directory.Exists(KaneGameManager.Path + "HitboxData/"))
       // Directory.CreateDirectory(KaneGameManager.Path + "HitboxData/");
       //     string[] lines = currentlyRecordingHitboxData.GetDataString();
       // File.WriteAllLines(KaneGameManager.Path + "HitboxData/" + currentlyRecordingHitboxData.action.ToString() + "_" + currentlyRecordingHitboxData.titanLevel.ToString() + ".txt", lines);

        public static bool showHitboxs = false;

        public static void drawHitboxes()
        {
            if (showHitboxs && PTDataMachine.hitBoxes != null && PTDataMachine.hitBoxes.Count > 0)
            {
                foreach (FloatingFire.HitboxSphere hitboxTime in PTDataMachine.hitBoxes)
                {
                    CGTools.drawPoint(hitboxTime.pos);
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
            PTButtonColor = CGTools.ColorTex(new Color(1f,1f,1f));
            menuX = (float)Screen.width / 2f - 350f + 1000f;
            menuY = (float) Screen.height / 2f - 250f;
        }
    }
}
