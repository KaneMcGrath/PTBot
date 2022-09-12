using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace TitanBot
{
    public static class CGTools
    {

        public static Texture2D crosshairTex;
        public static Texture2D redCrosshairTex;
        public static Texture2D greenCrosshairTex;
        public static float crosshairSize = 100f;
        public static List<Vector3> pointsToTrack = new List<Vector3>();
        public static List<Vector3> redPointsToTrack = new List<Vector3>();
        public static List<Vector3> greenPointsToTrack = new List<Vector3>();

        public static void OnGUI()
        {
            //GUI.DrawTexture(new Rect(0f, 0f, 50f, 50f), crosshairTex);
            //GUI.DrawTexture(new Rect(50f, 0f, 50f, 50f), redCrosshairTex);
            //GUI.DrawTexture(new Rect(100f, 0f, 50f, 50f), greenCrosshairTex);
            QuickMenu.drawHitboxes();
            if (pointsToTrack.Count > 0)
            {
                foreach (Vector3 point in pointsToTrack)
                {
                    drawPoint(point);
                }
            }
            if (redPointsToTrack.Count > 0)
            {
                foreach (Vector3 point in redPointsToTrack)
                {
                    drawRedPoint(point);
                }
            }
            if (greenPointsToTrack.Count > 0)
            {
                foreach (Vector3 point in greenPointsToTrack)
                {
                    drawGreenPoint(point);
                }
            }
        }
        public static void Update()
        {
            pointsToTrack.Clear();
            redPointsToTrack.Clear();
            greenPointsToTrack.Clear();
        }

        public static void Init()
        {
            crosshairTex = readTextureFromFile(KaneGameManager.Path + "crosshairTex.png");
            redCrosshairTex = applyColorToTexture(crosshairTex, Color.red);
            greenCrosshairTex = applyColorToTexture(crosshairTex, Color.green);
            

        }

        //Thanks Bunny83.  that was easier than I expected
        //https://answers.unity.com/questions/1238142/version-of-transformtransformpoint-which-is-unaffe.html
        public static Vector3 TransformPointUnscaled(this Transform transform, Vector3 position)
        {
            return transform.position + transform.rotation * position;
        }

        public static Texture2D readTextureFromFile(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    byte[] data = File.ReadAllBytes(path);
                    Texture2D texture2D = new Texture2D(2, 2);
                    texture2D.LoadImage(data);
                    return texture2D;
                }
                else
                {
                    CGTools.log("Could not read texture [" + path + "]");
                    return null;
                }
            }
            catch (Exception e)
            {
                CGTools.log(e.Message);
                return null;
            }
        }

        public static Texture2D applyColorToTexture(Texture2D tex, Color c)
        {
            Texture2D result = new Texture2D(tex.width, tex.height);
            for (int y = 0; y < tex.height; y++)
            {
                for (int x = 0; x < tex.width; x++)
                {
                    if (tex.GetPixel(x, y).a > 0.5f)
                    {
                        CGTools.log(x.ToString() + "," + y.ToString());
                        result.SetPixel(x, y, c);
                    }
                    else
                    {
                        result.SetPixel(x, y, Color.clear);
                    }
                }
            }
            result.Apply();
            return result;
        }

        //should scale from a distance of 5 to 200
        //but it fucking wont
        //I did math just please work
        public static void drawPoint(Vector3 point)
        {
            Vector3 screenPosition = Camera.main.camera.WorldToScreenPoint(point);
            float distance = Vector3.Distance(Camera.main.transform.position, point);
            float scale = Mathf.Clamp(((1/distance) * 1000f) + 10f, 5f, 200f);
            if (screenPosition.z >= 0f)
            {
                GUI.DrawTexture(new Rect(screenPosition.x - (scale / 2f), ((float)Screen.height - screenPosition.y) - (scale / 2f), scale, scale), crosshairTex);
            }
        }

        public static void drawRedPoint(Vector3 point)
        {
            Vector3 screenPosition = Camera.main.camera.WorldToScreenPoint(point);
            float distance = Vector3.Distance(Camera.main.transform.position, point);
            float scale = Mathf.Clamp(((1 / distance) * 1000f) + 10f, 5f, 200f);
            if (screenPosition.z >= 0f)
            {
                GUI.DrawTexture(new Rect(screenPosition.x - (scale / 2f), ((float)Screen.height - screenPosition.y) - (scale / 2f), scale, scale), redCrosshairTex);
            }
        }

        public static void drawGreenPoint(Vector3 point)
        {
            Vector3 screenPosition = Camera.main.camera.WorldToScreenPoint(point);
            float distance = Vector3.Distance(Camera.main.transform.position, point);
            float scale = Mathf.Clamp(((1 / distance) * 1000f) + 10f, 5f, 200f);
            if (screenPosition.z >= 0f)
            {
                GUI.DrawTexture(new Rect(screenPosition.x - (scale / 2f), ((float)Screen.height - screenPosition.y) - (scale / 2f), scale, scale), greenCrosshairTex);
            }
        }

        //public static void drawPoint(Vector3 point)
        //{
        //    Vector3 screenPosition = Camera.main.camera.WorldToScreenPoint(point);
        //    if (screenPosition.z >= 0f)
        //    {
        //        GUI.DrawTexture(new Rect(screenPosition.x - (crosshairSize / 2f), ((float)Screen.height - screenPosition.y) - (crosshairSize / 2f), crosshairSize, crosshairSize), crosshairTex);
        //    }
        //}

        /// <summary>
        /// When called multiple times only returns true after a fixed amount of time after it was first called
        /// then is reset
        /// </summary>
        /// <param name="timer"></param>
        /// <param name="waitTime"></param>
        /// <returns></returns>
        public static bool timer(ref float timer, float waitTime)
        {
            if (timer <= Time.time)
            {
                timer = Time.time + waitTime;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Creates a Texture that is a flat color
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static Texture2D ColorTex(Color color)
        {
            Texture2D texture2D = new Texture2D(1, 1, TextureFormat.ARGB32, false);
            texture2D.SetPixel(0, 0, color);
            texture2D.Apply();
            return texture2D;
        }



        //public static StreamWriter logWriter = new StreamWriter(KaneGameManager.Path + "log.txt");
        public static void log(string message)
        {
            CGLog.log(message);
            //logWriter.WriteLine(message);
            //logWriter.Flush();
        }

        public static Color randomColor()
        {
            float num = UnityEngine.Random.Range(0f, 0.7f);
            float num2 = UnityEngine.Random.Range(0f, 0.7f);
            float num3 = UnityEngine.Random.Range(0f, 0.7f);
            if ((double)(num + num3 + num2) < 0.3)
            {
                num = UnityEngine.Random.Range(0.5f, 0.7f);
            }
            if ((double)(num + num3 + num2) > 1.8)
            {
                num2 = UnityEngine.Random.Range(0.1f, 0.5f);
                num = UnityEngine.Random.Range(0.1f, 0.5f);
            }
            return new Color(num, num2, num3);
        }

        public static GameObject[] GetHeros()
        {
            return GameObject.FindGameObjectsWithTag("Player");

        }

        public static GameObject player()
        {
            foreach (GameObject gameObject in GameObject.FindGameObjectsWithTag("Player"))
            {
                if (IN_GAME_MAIN_CAMERA.gametype != GAMETYPE.SINGLE && gameObject.GetPhotonView().owner.isLocal)
                {
                    return gameObject;
                }
            }
            if (Camera.main.GetComponent<IN_GAME_MAIN_CAMERA>().main_object.GetComponent<HERO>())
            {
                return Camera.main.GetComponent<IN_GAME_MAIN_CAMERA>().main_object;
            }
            return null;
        }
    }
}
