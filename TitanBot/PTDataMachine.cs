using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace TitanBot
{
    /// <summary>
    /// Stores and retrieves hitbox information at different sizes and velocities and tries to return
    /// hitboxes based on the closest match in our database
    /// </summary>
    public static class PTDataMachine
    {
        public static List<GameObject> VisualizationSpheres = new List<GameObject>();
        public static Dictionary<PTAction, List<hitboxData>> Database = new Dictionary<PTAction, List<hitboxData>>();
        public static bool RecordData = false;
        public static bool KeepHitboxes = false;
        public static GameObject hitSphere;
        public static GameObject hitBox;
        public static GameObject PlayerCapsule;
        public static bool skipResetThisFrame = false;
        public static int framesToKeepHitbox = 60;
        public static int hitboxWaitCounter = 0;
        public static bool showPlayerCapsule = false;
        public static GameObject player;

        public static void CreateVisualizationSphere(Vector3 pos, float scale)
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
                hitSphere.transform.localScale = new Vector3(scale * 2f, scale * 2f, scale * 2f);
                hitSphere.transform.position = pos;
                hitboxWaitCounter = framesToKeepHitbox;
                return;
            }
            GameObject s = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            GameObject.Destroy(s.GetComponent<SphereCollider>());
            s.transform.localScale = new Vector3(scale * 2f, scale * 2f, scale * 2f);
            foreach (Renderer renderer in s.GetComponentsInChildren<Renderer>())
            {
                renderer.material = (Material)FengGameManagerMKII.RCassets.Load("killMaterial");
            }
            VisualizationSpheres.Add(s);
            s.transform.position = pos;
        }

        public static void CreateColliderSphere(Vector3 pos, float radius, int damage = 1)
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
        public static void CreateColliderBox(Vector3 pos, Vector3 size, Quaternion rot, int damage = 1)
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

        public static void UpdatePlayerCapsule()
        {
            if (showPlayerCapsule)
            {
                if (PlayerCapsule == null)
                {
                    PlayerCapsule = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                    GameObject.Destroy(PlayerCapsule.GetComponent<CapsuleCollider>());
                    foreach (Renderer renderer in PlayerCapsule.GetComponentsInChildren<Renderer>())
                    {
                        renderer.material = (Material)FengGameManagerMKII.RCassets.Load("checkpointMat");
                    }
                }
                if (player != null)
                {
                    CapsuleCollider ccc = player.GetComponent<CapsuleCollider>();
                    PlayerCapsule.transform.position = ccc.transform.position + ccc.center;
                    PlayerCapsule.transform.rotation = ccc.transform.rotation;

                }
            }
            else
            {
                if (PlayerCapsule != null)
                {
                    PlayerCapsule.transform.position = new Vector3(0f, -999f, 0f);
                }
            }
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
    public class hitboxData
    {
        public PTAction PTAction;
        public HitboxTime[] SampledHitboxes;
        public float Size;
        public float HitboxSize;
        public bool Running;

        public hitboxData(PTAction pTAction, float size, bool running, float hitboxSize)
        {
            PTAction = pTAction;
            Size = size;
            Running = running;
            HitboxSize = hitboxSize;
        }

        public void GetDataString()
        {
            string result = PTAction.ToString() + ":" + Size.ToString() + ":" + Running.ToString() + ":" + HitboxSize.ToString() + "{";
            foreach (HitboxTime hitboxTime in SampledHitboxes)
            {
                result += hitboxTime.GetDataString() + ";";
            }
            result += "}";
        }

        public static hitboxData ReadDataString(string input)
        {
            PTAction action = PTAction.sit;
            float size = 0f;
            float hitboxSize = 0f;
            bool running = false;
            List<HitboxTime> hitboxes = new List<HitboxTime>();

            return new hitboxData(action, size, running, hitboxSize);
        }
    }


    public struct HitboxTime
    {
        public float Time;
        public Vector3 Position;

        public HitboxTime(Vector3 position, float time)
        {
            Position = position;
            Time = time;
        }

        public string GetDataString()
        {
            return "(" + Time.ToString() + "[" + Position.x.ToString() + "," + Position.y.ToString() + "," + Position.z.ToString() + "])";
        }

        public static HitboxTime ReadDataString(string input)
        {
            float time = 0;
            Vector3 pos = new Vector3();
            string timeCut = input.Substring(1, input.IndexOf('[') - 1);
            if (!float.TryParse(timeCut, out time)) CGTools.log("Could not parse string : " + timeCut);
            string posCut = input.Substring(input.IndexOf('[') + 1, input.IndexOf(']') - input.IndexOf('[') - 1);
            string[] parts = posCut.Split(',');
            if (!float.TryParse(parts[0], out pos.x)) CGTools.log("Could not parse string : " + parts[0]);
            if (!float.TryParse(parts[1], out pos.y)) CGTools.log("Could not parse string : " + parts[1]);
            if (!float.TryParse(parts[2], out pos.z)) CGTools.log("Could not parse string : " + parts[2]);

            return new HitboxTime(pos, time);
        }
    }
}
