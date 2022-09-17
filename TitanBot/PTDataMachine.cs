using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.IO;
using Mono.Security.X509.Extensions;

namespace TitanBot
{
    /// <summary>
    /// Stores and retrieves hitbox information at different sizes and velocities and tries to return
    /// hitboxes based on the closest match in our database
    /// I tried to scale it to match with transform point but that didnt work for some reason
    /// </summary>
    public static class PTDataMachine
    {
        public static List<GameObject> VisualizationSpheres = new List<GameObject>();
        public static bool KeepHitboxes = false;
        public static GameObject hitSphere;
        public static GameObject hitBox;
        public static GameObject HurtCapsule;
        public static GameObject PlayerCapsule;
        public static GameObject topSphere;
        public static GameObject bottomSphere;
        public static bool skipResetThisFrame = false;
        public static int framesToKeepHitbox = 60;
        public static int hitboxWaitCounter = 0;
        public static bool showPlayerCapsule = false;
        public static GameObject player;
        public static bool DrawHitboxes = false;
        

             
        //public static Dictionary<PTAction, List<hitboxData>> Database = new Dictionary<PTAction, List<hitboxData>>();
        
        public static bool RecordData = true;
        public static bool isRecording = false;
        public static float recordingStartTime = 0f;
        public static HitData.MovesetData currentlyRecordingHitboxData;
        public static List<HitData.Hitbox> hitBoxes = new List<HitData.Hitbox>();
        private static Vector3 RecordingPosOffset;
        
        public static void SaveHitboxData(bool overwrite = false)
        {
            string dataPath = KaneGameManager.Path + "/HitboxData/";
            foreach (PTAction action in HitData.AllHitboxData.Keys)
            {
                List<HitData.MovesetData> list = HitData.AllHitboxData[action];
                foreach (HitData.MovesetData data in list)
                {
                    string filename = data.action.ToString() + "_" + data.titanLevel.ToString() + ".txt";
                    bool flag = true;
                    if (File.Exists(dataPath + filename) && !overwrite)
                        flag = false;
                    if (flag)
                    {
                        string[] lines = data.GetDataString();
                        File.WriteAllLines(dataPath + filename, lines);
                    }
                }
            }
        }

        public static void LoadHitboxData()
        {
            string dataPath = KaneGameManager.Path + "/HitboxData/";
            string[] files = Directory.GetFiles(dataPath);
            //1 moveset data per file
            foreach (string file in files)
            {
                string text = File.ReadAllText(file).Replace(Environment.NewLine, "");

                List<HitData.Hitbox> readHitBoxData = new List<HitData.Hitbox>();

                

                //ex. Attack:5
                string preData = text.Substring(0, text.IndexOf('{'));
                
                string body = ParseMaster.FirstEncapsulatedString(text, '{', '}');

                PTAction action = PTAction.nothing;
                float titanLevel = 0f;
                string[] preDataSplit = preData.Split(':');

                try
                {
                    action = (PTAction)Enum.Parse(typeof(PTAction), preDataSplit[0]);
                }
                catch (Exception e)
                {
                    CGTools.log("Failed to parse file \\\"\" + file + \"\\\" > PTAction");
                }
                if (!float.TryParse(preDataSplit[1], out titanLevel))
                {
                    CGTools.log("Failed to parse file \"" + file + "\" > titanLevel");
                    return;
                }

                //ex. (Hsphere{13},0.4196033[22.95233,67.94828,31.8136])
                string[] hitboxDataStrings = body.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string hitboxDataString in hitboxDataStrings)
                {

                    //ex. Hsphere{13},0.4196033
                    string typeAndTime = hitboxDataString.Substring(1, hitboxDataString.IndexOf('[') - 1);

                    string[] typeAndTimeData = typeAndTime.Split(',');

                    //ex. 22.95233,67.94828,31.8136
                    string vector3Data = ParseMaster.FirstEncapsulatedString(hitboxDataString, '[', ']');
                    string[] vector3Floats = vector3Data.Split(',');
                    if (!float.TryParse(vector3Floats[0], out float x))
                    {
                        CGTools.log("Failed to parse file \"" + file + "\" > Vector3 x");
                        return;
                    }
                    if (!float.TryParse(vector3Floats[1], out float y))
                    {
                        CGTools.log("Failed to parse file \"" + file + "\" > Vector3 y");
                        return;
                    }
                    if (!float.TryParse(vector3Floats[2], out float z))
                    {
                        CGTools.log("Failed to parse file \"" + file + "\" > Vector3 z");
                        return;
                    }
                    if (!float.TryParse(typeAndTimeData[1], out float time))
                    {
                        CGTools.log("Failed to parse file \"" + file + "\" > time");
                        return;
                    }



                    if (typeAndTimeData[0].StartsWith("Hsphere"))
                    {
                        string radius = ParseMaster.FirstEncapsulatedString(typeAndTimeData[0], '{', '}');
                        if (!float.TryParse(radius, out float rad))
                        {
                            CGTools.log("Failed to parse file \"" + file + "\" > radius");
                            return;
                        }
                        readHitBoxData.Add(new HitData.HitboxSphere(new Vector3(x, y, z), time, titanLevel, rad));
                    }
                    else if (typeAndTimeData[0].StartsWith("Hrect"))
                    {
                        string frontString = ParseMaster.FirstEncapsulatedString(typeAndTimeData[0], '{', '}');
                        string[] rectData = frontString.Split('|');
                        if (rectData.Length != 7)
                        {
                            CGTools.log("Failed to parse file \"" + file + "\" > rectData needs 7 arguments");
                            return;
                        }
                        if (!float.TryParse(rectData[0], out float sizex))
                        {
                            CGTools.log("Failed to parse file \"" + file + "\" > sizex");
                            return;
                        }
                        if (!float.TryParse(rectData[1], out float sizey))
                        {
                            CGTools.log("Failed to parse file \"" + file + "\" > sizey");
                            return;
                        }
                        if (!float.TryParse(rectData[2], out float sizez))
                        {
                            CGTools.log("Failed to parse file \"" + file + "\" > sizez");
                            return;
                        }
                        if (!float.TryParse(rectData[3], out float rotw))
                        {
                            CGTools.log("Failed to parse file \"" + file + "\" > rotw");
                            return;
                        }
                        if (!float.TryParse(rectData[4], out float rotx))
                        {
                            CGTools.log("Failed to parse file \"" + file + "\" > rotx");
                            return;
                        }
                        if (!float.TryParse(rectData[5], out float roty))
                        {
                            CGTools.log("Failed to parse file \"" + file + "\" > roty");
                            return;
                        }
                        if (!float.TryParse(rectData[6], out float rotz))
                        {
                            CGTools.log("Failed to parse file \"" + file + "\" > rotz");
                            return;
                        }

                        readHitBoxData.Add(new HitData.HitboxRectangle(new Vector3(x, y, z), time, titanLevel, new Vector3(sizex, sizey, sizez), new Quaternion(rotw, rotx, roty, rotz)));
                    }
                }

                


                HitData.MovesetData movesetData = new HitData.MovesetData(action, titanLevel);
                movesetData.hitboxes = readHitBoxData.ToArray();

                HitData.AddData(movesetData);
                CGTools.log("Loaded data for [" + movesetData.action + "] Size: " + movesetData.titanLevel);
            }
        }

        public static void StartRecordingHitbox(PTAction action)
        {
            hitBoxes.Clear();
            RecordingPosOffset = QuickMenu.myLastPT.transform.position;
            CGTools.log("Offset: " + RecordingPosOffset.ToString());
            currentlyRecordingHitboxData = new HitData.MovesetData(action, QuickMenu.myLastPT.myLevel);
            recordingStartTime = Time.time;
            isRecording = true;
        }

        public static void FinishRecordingHitbox()
        {
            currentlyRecordingHitboxData.hitboxes = hitBoxes.ToArray();
            isRecording = false;
            HitData.AddData(currentlyRecordingHitboxData);
            
        }

        public static void CreateVisualizationSphere(Vector3 pos, float radius)
        {
            if (isRecording)
            {
                hitBoxes.Add(new HitData.HitboxSphere(pos - RecordingPosOffset, Time.time - recordingStartTime, QuickMenu.myLastPT.myLevel, radius));
            }
            if (DrawHitboxes)
            {
                if (!KeepHitboxes)
                {
                    if (hitSphere == null)
                    {
                        hitSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                        GameObject.Destroy(hitSphere.GetComponent<SphereCollider>());
                    }
                    foreach (Renderer renderer in hitSphere.GetComponentsInChildren<Renderer>())
                    {
                        renderer.material = (Material)FengGameManagerMKII.RCassets.Load("killMaterial");
                    }
                    hitSphere.transform.localScale = new Vector3(radius * 2f, radius * 2f, radius * 2f);
                    hitSphere.transform.position = pos;
                    hitboxWaitCounter = framesToKeepHitbox;
                    return;
                }
                GameObject s = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                GameObject.Destroy(s.GetComponent<SphereCollider>());
                s.transform.localScale = new Vector3(radius * 2f, radius * 2f, radius * 2f);
                foreach (Renderer renderer in s.GetComponentsInChildren<Renderer>())
                {
                    renderer.material = (Material)FengGameManagerMKII.RCassets.Load("killMaterial");
                }
                VisualizationSpheres.Add(s);
                s.transform.position = pos;
            }
        }

        public static void CreateColliderSphere(Vector3 pos, float radius, int damage = 1)
        {
            if (isRecording)
            {
                if (damage > 0)
                    hitBoxes.Add(new HitData.HitboxSphere(pos - RecordingPosOffset, Time.time - recordingStartTime, QuickMenu.myLastPT.myLevel, radius * QuickMenu.myLastPT.transform.localScale.y * 1.5f));
            }
            if (DrawHitboxes)
            {
                if (!KeepHitboxes)
                {
                    if (damage < 1) return;
                    if (hitSphere == null)
                    {
                        hitSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                        GameObject.Destroy(hitSphere.GetComponent<SphereCollider>());
                        foreach (Renderer renderer in hitSphere.GetComponentsInChildren<Renderer>())
                        {
                            renderer.material = (Material)FengGameManagerMKII.RCassets.Load("killMaterial");
                        }
                    }
                    foreach (Renderer renderer in hitSphere.GetComponentsInChildren<Renderer>())
                    {
                        if (damage == 1)
                            renderer.material = (Material)FengGameManagerMKII.RCassets.Load("killMaterial");
                        if (damage < 1)
                            renderer.material = (Material)FengGameManagerMKII.RCassets.Load("barriereditormat");
                    }
                    hitSphere.transform.localScale = radius * QuickMenu.myLastPT.transform.localScale * 1.5f * 2f;
                    hitSphere.transform.position = pos;
                    hitboxWaitCounter = framesToKeepHitbox;
                    return;
                }
                GameObject s = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                GameObject.Destroy(s.GetComponent<SphereCollider>());

                s.transform.localScale = radius * QuickMenu.myLastPT.transform.localScale * 1.5f * 2f;
                foreach (Renderer renderer in s.GetComponentsInChildren<Renderer>())
                {
                    if (damage == 1)
                        renderer.material = (Material)FengGameManagerMKII.RCassets.Load("killMaterial");
                    if (damage < 1)
                        renderer.material = (Material)FengGameManagerMKII.RCassets.Load("barriereditormat");
                }
                VisualizationSpheres.Add(s);
                s.transform.position = pos;
            }
        }
        public static void CreateColliderBox(Vector3 pos, Vector3 size, Quaternion rot, int damage = 1)
        {
            if (isRecording)
            {
                Vector3 pt = QuickMenu.myLastPT.transform.localScale * 1.5f;
                if (damage > 0)
                    hitBoxes.Add(new HitData.HitboxRectangle(pos - RecordingPosOffset, Time.time - recordingStartTime, QuickMenu.myLastPT.myLevel, size, rot));
            }
            if (DrawHitboxes)
            {
                Vector3 pt = QuickMenu.myLastPT.transform.localScale * 1.5f;
                if (!KeepHitboxes)
                {
                    if (damage < 1) return;
                    if (hitBox == null)
                    {
                        hitBox = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        GameObject.Destroy(hitBox.GetComponent<BoxCollider>());
                        foreach (Renderer renderer in hitBox.GetComponentsInChildren<Renderer>())
                        {
                            renderer.material = (Material)FengGameManagerMKII.RCassets.Load("killMaterial");
                        }
                    }
                    foreach (Renderer renderer in hitBox.GetComponentsInChildren<Renderer>())
                    {
                        if (damage == 1)
                            renderer.material = (Material)FengGameManagerMKII.RCassets.Load("killMaterial");
                        if (damage < 1)
                            renderer.material = (Material)FengGameManagerMKII.RCassets.Load("barriereditormat");
                    }
                    hitBox.transform.localScale = new Vector3(size.x * pt.x, size.y * pt.y, size.z * pt.z);
                    //hitBox.transform.localScale = new Vector3(size.x, size.y, size.z);
                    hitBox.transform.position = pos;
                    hitBox.transform.rotation = rot;
                    hitboxWaitCounter = framesToKeepHitbox;
                    return;
                }
                GameObject s = GameObject.CreatePrimitive(PrimitiveType.Cube);
                GameObject.Destroy(s.GetComponent<BoxCollider>());
                // + QuickMenu.myLastPT.transform.localScale * 1.5f * 2f

                s.transform.localScale = new Vector3(size.x * pt.x, size.y * pt.y, size.z * pt.z);
                //s.transform.localScale = new Vector3(size.x, size.y, size.z);
                s.transform.rotation = rot;
                foreach (Renderer renderer in s.GetComponentsInChildren<Renderer>())
                {
                    if (damage == 1)
                        renderer.material = (Material)FengGameManagerMKII.RCassets.Load("killMaterial");
                    if (damage < 1)
                        renderer.material = (Material)FengGameManagerMKII.RCassets.Load("barriereditormat");
                }
                VisualizationSpheres.Add(s);
                s.transform.position = pos;
            }
        }

        public static void CreateColliderCapsule(Vector3 pos, float radius, float height)
        {
            if (DrawHitboxes)
            {
                if (!KeepHitboxes)
                {

                }
                else
                {
                    if (HurtCapsule == null)
                    {
                        HurtCapsule = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                        GameObject.Destroy(HurtCapsule.GetComponent<CapsuleCollider>());
                        foreach (Renderer renderer in HurtCapsule.GetComponentsInChildren<Renderer>())
                        {
                            renderer.material = (Material)FengGameManagerMKII.RCassets.Load("checkpointMat");
                        }
                    }
                    if (player != null)
                    {
                        CapsuleCollider ccc = player.GetComponent<CapsuleCollider>();
                        HurtCapsule.transform.position = ccc.transform.position + ccc.center;
                        HurtCapsule.transform.rotation = ccc.transform.rotation;
                    }
                }
            }
        }

        public static void CreateCapsule(CapsuleCollider collider, GameObject _parent)
        {
            PlayerCapsule = CreateCapsule(collider.radius, collider.height, collider.transform.position + collider.center, collider.transform.rotation);
            PlayerCapsule.transform.parent = _parent.transform;
        }

        public static GameObject CreateCapsule(float _radius, float _height, Vector3 _center, Quaternion _rotation)
        {
            GameObject parent = new GameObject("CGCapsule");

            GameObject tSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            tSphere.transform.parent = parent.transform;
            GameObject.Destroy(tSphere.GetComponent<Collider>());

            GameObject bSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            bSphere.transform.parent = parent.transform;
            GameObject.Destroy(bSphere.GetComponent<Collider>());
            bSphere.transform.Rotate(180, 0, 0);

            GameObject cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            cylinder.transform.parent = parent.transform;
            GameObject.Destroy(cylinder.GetComponent<Collider>());



            parent.transform.position = _center;
            parent.transform.rotation = _rotation;

            float diameter = _radius * 2f;
            float midHeight = _height / 2f - _radius;

            topSphere.transform.localScale = Vector3.one * diameter;
            topSphere.transform.localPosition = Vector3.up * midHeight;

            bottomSphere.transform.localScale = Vector3.one * diameter;
            bottomSphere.transform.localPosition = -Vector3.up * midHeight;

            cylinder.transform.localScale = new Vector3(
                    diameter,
                    midHeight,
                    diameter
                );
            return parent;
        }


        public static void CreateVisualizationMesh(Vector3 pos, Mesh mesh)
        {
            GameObject s = new GameObject();
            s.AddComponent<MeshFilter>();
            s.GetComponent<MeshFilter>().mesh = mesh;
            s.AddComponent<MeshRenderer>();
            s.GetComponent<MeshRenderer>().material = (Material)FengGameManagerMKII.RCassets.Load("killMaterial");
            
            VisualizationSpheres.Add(s);
            s.transform.position = pos;
        }

        public static void DeleteVisualizationSpheres()
        {
            foreach (GameObject g in VisualizationSpheres)
            {
                GameObject.Destroy(g);
            }
            VisualizationSpheres.Clear();
        }
    }


    
}
