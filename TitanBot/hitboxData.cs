using System.Collections.Generic;
using UnityEngine;

namespace TitanBot
{
    public class hitboxData
    {
        public PTAction PTAction;
        public HitboxTime[] SampledHitboxes;
        public float Size = 0f;
        public float HitboxSize;
        public bool Running;

        public hitboxData(PTAction pTAction, float size, bool running, float hitboxSize)
        {
            PTAction = pTAction;
            Size = size;
            Running = running;
            HitboxSize = hitboxSize;
        }

        public string[] GetDataString()
        {
            List<string> data = new List<string>();
            string result = PTAction.ToString() + ":" + Size.ToString() + ":" + Running.ToString() + ":" + HitboxSize.ToString() + "{";
            data.Add(result);
            foreach (HitboxTime hitboxTime in SampledHitboxes)
            {
                data.Add(hitboxTime.GetDataString() + ";");
            }
            data.Add("}");
            return data.ToArray();
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
