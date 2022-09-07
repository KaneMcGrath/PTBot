using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace TitanBot
{
    public static class CGTools
    {
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

        public static void log(string message)
        {
            Debug.Log(message);
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
