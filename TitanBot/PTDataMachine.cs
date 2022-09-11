using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.IO;

namespace TitanBot
{
    /// <summary>
    /// Stores and retrieves hitbox information at different sizes and velocities and tries to return
    /// hitboxes based on the closest match in our database
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
        public static bool DrawHitboxes = true;

        
        public static Dictionary<PTAction, List<hitboxData>> Database = new Dictionary<PTAction, List<hitboxData>>();
        public static bool RecordData = true;
        public static bool isRecording = false;
        public static float recordingStartTime = 0f;
        public static FloatingFire.MovesetData currentlyRecordingHitboxData;
        public static List<FloatingFire.Hitbox> hitBoxes = new List<FloatingFire.Hitbox>();

        public static void StartRecordingHitbox(PTAction action)
        {
            hitBoxes.Clear();
            currentlyRecordingHitboxData = new FloatingFire.MovesetData(action, QuickMenu.myLastPT.myLevel);
            recordingStartTime = Time.time;
            isRecording = true;
        }

        public static void FinishRecordingHitbox()
        {
            currentlyRecordingHitboxData.hitboxes = hitBoxes.ToArray();
            isRecording = false;
            FloatingFire.AddData(currentlyRecordingHitboxData);
            if (!Directory.Exists(KaneGameManager.Path + "HitboxData/"))
                Directory.CreateDirectory(KaneGameManager.Path + "HitboxData/");
            string[] lines = currentlyRecordingHitboxData.GetDataString();
            File.WriteAllLines(KaneGameManager.Path + "HitboxData/" + currentlyRecordingHitboxData.action.ToString() + "_" + currentlyRecordingHitboxData.titanLevel.ToString() + ".txt", lines);
        }

        public static void CreateVisualizationSphere(Vector3 pos, float radius)
        {
            if (isRecording)
            {
                hitBoxes.Add(new FloatingFire.HitboxSphere(pos, Time.time - recordingStartTime, radius));

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
                    hitBoxes.Add(new FloatingFire.HitboxSphere(pos, Time.time - recordingStartTime, radius));
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
                    hitBox.transform.position = pos;
                    hitBox.transform.rotation = rot;
                    hitboxWaitCounter = framesToKeepHitbox;
                    return;
                }
                GameObject s = GameObject.CreatePrimitive(PrimitiveType.Cube);
                GameObject.Destroy(s.GetComponent<BoxCollider>());
                // + QuickMenu.myLastPT.transform.localScale * 1.5f * 2f
                s.transform.localScale = new Vector3(size.x * pt.x, size.y * pt.y, size.z * pt.z);
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

        public static hitboxData GetClosestHitboxData(PTAction action, float size)
        {
            if (Database.ContainsKey(action))
            {
                hitboxData[] hitboxData = Database[action].ToArray();
                float lastClosestSize = float.PositiveInfinity;
                int lastClosestIndex = -1;
                for (int i = 0; i < hitboxData.Length; i++)
                {
                    if (Math.Abs(size - hitboxData[i].Size) < lastClosestSize)
                    {
                        lastClosestSize = hitboxData[i].Size;
                        lastClosestIndex = i;
                    }
                }
                if (lastClosestIndex >= 0)
                {
                    return hitboxData[lastClosestIndex];
                }
            }
            return null;
        }

        

        public static void parseTest()
        {
            string input = "(12.438454132[1223.5114,760009.5589,99000.252510000])";
            CGTools.log("Parsing string " + input);

            float time = 0;
            Vector3 pos = new Vector3();
            //(0.4332[13.5114,79.5589,99.25251])
            string timeCut = input.Substring(1, input.IndexOf('[') - 1);
            if (!float.TryParse(timeCut, out time)) CGTools.log("Could not parse string : " + timeCut);
            string posCut = input.Substring(input.IndexOf('[') + 1, input.IndexOf(']') - input.IndexOf('[') -1);
            string[] parts = posCut.Split(',');
            if (!float.TryParse(parts[0], out pos.x)) CGTools.log("Could not parse string : " + parts[0]);
            if (!float.TryParse(parts[1], out pos.y)) CGTools.log("Could not parse string : " + parts[1]);
            if (!float.TryParse(parts[2], out pos.z)) CGTools.log("Could not parse string : " + parts[2]);


            CGTools.log("Time = " + timeCut);
            CGTools.log("parts0 = " + parts[0]);
            CGTools.log("parts1 = " + parts[1]);
            CGTools.log("parts2 = " + parts[2]);
        }
    }


    
}
