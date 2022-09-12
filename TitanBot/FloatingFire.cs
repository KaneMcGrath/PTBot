using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using static StyledItemButtonImageText;
using static UIPopupList;

namespace TitanBot
{
    /// <summary>
    /// Hitbox data container with all stuff needed to record a hitbox
    /// already a mess
    /// </summary>
    public class FloatingFire
    {
        public static Dictionary<PTAction, List<MovesetData>> AllHitboxData = new Dictionary<PTAction, List<MovesetData>>();
        public static float HitboxTimeGrace = 0.3f;
        public static bool debugMovementChecking = false;

        //checks hitboxes in order of time and returns the time of the first hitbox to connect
        public static float checkMoveset(MovesetData data, Vector3 target, Transform owner, float timeOffset = 0f)
        {
            for (int i = 0; i < data.hitboxes.Length; i++)
            {
                
                Hitbox h = data.hitboxes[i];
                if (h.time > timeOffset - HitboxTimeGrace && h.time < timeOffset + HitboxTimeGrace)
                {
                    if (h.CheckTrigger(target, owner))
                    {
                        return h.time;
                    }
                }
            }
            return float.MaxValue;
        }

        //calculates intercepts at the time of each hitbox then see if that hits
        //returns the lowest time that hits
        //probably a lag machine
        public static float CatchingSmoke(MovesetData data, GameObject target, Transform owner)
        {
            if (debugMovementChecking) CGTools.pointsToTrack.Add(target.transform.position);
            for (int i = 0; i < data.hitboxes.Length; i++)
            {
                Hitbox h = data.hitboxes[i];
                Vector3 future = PTTools.predictPlayerMotion(target, h.time);
                //Vector3 future = PTTools.LonelySteelSheetFlyer(target, h.time);
                
                if (h.CheckTrigger(future, owner))
                {
                    
                    return h.time;
                }
            }
            return float.MaxValue;
        }

        public static void AddData(MovesetData data)
        {
            if (AllHitboxData.ContainsKey(data.action))
            {
                AllHitboxData[data.action].Add(data);
            }
            else 
            { 
                AllHitboxData.Add(data.action, new List<MovesetData>());
                AllHitboxData[data.action].Add(data);
            }
        }

        /// <summary>
        /// returns the closest match of moveset data without any scaling
        /// </summary>
        /// <param name="action"></param>
        /// <param name="titanLevel"></param>
        /// <returns></returns>
        public static MovesetData GetClosestData(PTAction action, float titanLevel)
        {
            MovesetData closestData = null;
            float closestDataDistance = float.MaxValue;
            if (AllHitboxData.ContainsKey(action))
            {
                foreach (MovesetData data in AllHitboxData[action])
                {
                    if (Mathf.Abs(data.titanLevel - titanLevel) < closestDataDistance)
                    {
                        closestData = data;
                        closestDataDistance = data.titanLevel;
                    }
                }
            }
            
            return closestData;
        }

        public class MovesetData
        {
            public PTAction action;
            public float titanLevel;

            //load first hitboxes to last hitboxes in time order
            public Hitbox[] hitboxes;

            public MovesetData(PTAction action, float titanLevel)
            {
                this.action = action;
                this.titanLevel = titanLevel;
            }

            public string[] GetDataString()
            {
                List<string> data = new List<string>();
                string result = action.ToString() + ":" + titanLevel.ToString() + "{";
                data.Add(result);
                foreach (Hitbox hbox in hitboxes)
                {
                    data.Add(hbox.GetDataString() + ";");
                }
                data.Add("}");
                return data.ToArray();
            }
        }


        public class Hitbox
        {
            public Vector3 pos;
            public float time;

            public Hitbox(Vector3 pos, float time)
            {
                this.pos = pos;
                this.time = time;
            }

            public virtual bool CheckTrigger(Vector3 target, Transform owner)
            {
                return false;
            }

            public virtual string GetDataString()
            {
                return "(" + time.ToString() + "[" + pos.x.ToString() + "," + pos.y.ToString() + "," + pos.z.ToString() + "])";
            }
        }

        public class HitboxRectangle : Hitbox
        {
            public Vector3 RectangleDimentions;

            public HitboxRectangle(Vector3 pos, float time, Vector3 RectangleDimentions) : base(pos, time)
            {
                this.RectangleDimentions = RectangleDimentions;
            }

            public override bool CheckTrigger(Vector3 target, Transform owner)
            {
                return false;
            }
        }

        public class HitboxSphere : Hitbox
        {
            public float radius;

            public HitboxSphere(Vector3 pos, float time, float radius) : base(pos, time)
            {
                this.radius = radius;
            }

            //need to make this work at some point
            //think it through
            public override bool CheckTrigger(Vector3 target, Transform owner)
            {
                Vector3 tp = CGTools.TransformPointUnscaled(owner, pos);
                if (debugMovementChecking) CGTools.pointsToTrack.Add(tp);
                if (Vector3.Distance(target, tp) < radius)
                {
                    if (debugMovementChecking) CGTools.redPointsToTrack.Add(target);
                    return true;
                }
                if (debugMovementChecking) CGTools.greenPointsToTrack.Add(target);
                return false;
            }

            public override string GetDataString()
            {
                return "(Hsphere," + time.ToString() + "[" + pos.x.ToString() + "," + pos.y.ToString() + "," + pos.z.ToString() + "])";
            }
        }
    }
}
