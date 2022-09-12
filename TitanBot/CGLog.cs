using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TitanBot.FlatUI5;
using UnityEngine;

namespace TitanBot
{
    public class CGLog
    {
        public static bool WriteToFile = true;
        public static string filePath = KaneGameManager.Path + "log.txt";

        public static StreamWriter logWriter = new StreamWriter(KaneGameManager.Path + "log.txt");

        public static void log(string message)
        {
            if (WriteToFile)
            {
                logWriter.WriteLine(message);
                logWriter.Flush();
            }
            CGLog.fullLog.Add(message);
            CGLog.logTimer = Time.time + CGLog.waitTime;
        }


  //      public static void log(string message, Color color)
  //      {
  //          CGLog.fullLog.Add(new logMessage(message, color));
  //          CGLog.logTimer = Time.time + CGLog.waitTime;
  //      }
  //      public static void log(string message, int severity)
  //      {
  //          CGLog.fullLog.Add(new logMessage(message, severity));
  //          CGLog.logTimer = Time.time + CGLog.waitTime;
  //      }
  //      public static void log(string message, Color color, int severity)
  //      {
  //          CGLog.fullLog.Add(message);
  //          CGLog.logTimer = Time.time + CGLog.waitTime;
  //      }

        public static void OnGUI()
        {
            if (Time.time <= CGLog.logTimer || CGLog.showLogGui)
            {
                int num = Math.Min(15, CGLog.fullLog.Count);
                for (int i = 0; i < num; i++)
                {
                    Rect position = new Rect((float)Screen.width - CGLog.width, (float)(i * 25) + 50f, CGLog.width, 25f);
                    GUI.DrawTexture(position, CGLog.LogBackground);
                    GUI.Label(position, CGLog.fullLog[CGLog.fullLog.Count - num + i], FlatUI.DefaultStyle);
                }
            }
            if (CGLog.trackedValues.Keys.Count > 0)
            {
                string[] array = new string[CGLog.trackedValues.Keys.Count];
                CGLog.trackedValues.Keys.CopyTo(array, 0);
                for (int j = 0; j < array.Length; j++)
                {
                    if (CGLog.trackedValueTimers[array[j]] <= Time.time)
                    {
                        CGLog.trackedValues.Remove(array[j]);
                        CGLog.trackedValueTimers.Remove(array[j]);
                    }
                    Rect position2 = new Rect((float)Screen.width - CGLog.trackedWidth - CGLog.trackedWidth * (float)j, 0f, CGLog.trackedWidth, 30f);
                    Rect position3 = new Rect((float)Screen.width - CGLog.trackedWidth - CGLog.trackedWidth * (float)j, 30f, CGLog.trackedWidth, 20f);
                    GUI.DrawTexture(position2, CGLog.LogBackground);
                    GUI.DrawTexture(position3, CGLog.TrackedBackground);
                    GUI.Label(position2, array[j], FlatUI.ThickStyle);
                    GUI.Label(position3, CGLog.trackedValues[array[j]]);
                }
            }
        }

        // Token: 0x0600001A RID: 26 RVA: 0x0000218D File Offset: 0x0000038D
        public static void clear()
        {
            CGLog.fullLog.Clear();
        }

        // Token: 0x0600001B RID: 27 RVA: 0x00002199 File Offset: 0x00000399
        public static void track(string key, string value)
        {
            CGLog.trackedValues[key] = value;
            CGLog.trackedValueTimers[key] = Time.time + 0.5f;
        }

        // Token: 0x0600001C RID: 28 RVA: 0x000020E1 File Offset: 0x000002E1
        public static void FullLogGUI()
        {
        }

        // Token: 0x0600001D RID: 29 RVA: 0x0000514C File Offset: 0x0000334C
        public static void Start()
        {
            CGLog.LogBackground = CGTools.ColorTex(new Color(0.2f, 0.2f, 0.2f, 0.6f));
            CGLog.TrackedBackground = CGTools.ColorTex(new Color(0.5f, 0f, 0f, 0.6f));
        }

        // Token: 0x0400001A RID: 26
        public static List<string> fullLog = new List<string>();

        // Token: 0x0400001B RID: 27
        public static float logTimer = 0f;

        // Token: 0x0400001C RID: 28
        public static bool showLogGui;

        // Token: 0x0400001D RID: 29
        public static Dictionary<string, string> trackedValues = new Dictionary<string, string>();

        // Token: 0x0400001E RID: 30
        public static Dictionary<string, float> trackedValueTimers = new Dictionary<string, float>();

        // Token: 0x0400001F RID: 31
        private static Texture2D LogBackground;

        // Token: 0x04000020 RID: 32
        private static Texture2D TrackedBackground;

        // Token: 0x04000021 RID: 33
        public static float waitTime = 5f;

        // Token: 0x04000022 RID: 34
        public static float trackedWidth = 150f;

        // Token: 0x04000023 RID: 35
        public static float width = 600f;
    }
}
