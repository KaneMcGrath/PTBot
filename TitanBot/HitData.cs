using StinkMod;
using System.Collections.Generic;
using UnityEngine;

namespace TitanBot
{
    /// <summary>
    /// Hitbox data container with all stuff needed to record a hitbox
    /// already a mess
    /// </summary>
    public class HitData
    {
        public static Dictionary<PTAction, List<MovesetData>> AllHitboxData = new Dictionary<PTAction, List<MovesetData>>();
        public static float HitboxTimeGrace = 0.3f;
        public static bool debugMovementChecking = false;
        public static int pruneHitboxes = 1; //ignore every n hitboxes to save some performance

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
        public static float AdvanceMovesetCheck(MovesetData data, GameObject target, Transform owner)
        {
            if (debugMovementChecking) CGTools.pointsToTrack.Add(target.transform.position);
            float startOffset = MovesetControl.movesetControlDatabase[data.action].startAnimationAt;
            for (int i = 0; i < data.hitboxes.Length; i++)
            {
                Hitbox h = data.hitboxes[i];
                Vector3 future = PTTools.PredictPlayerMotion(target, Mathf.Max(0f, h.time - startOffset));

                if (h.CheckTrigger(future, owner))
                {
                    if (PlayerTitanBot.debugTargets)
                        CGTools.redPointsToTrack.Add(future);
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
                    float dist = Mathf.Abs(data.titanLevel - titanLevel);
                    if (dist < closestDataDistance)
                    {
                        closestData = data;
                        closestDataDistance = dist;
                    }
                }
            }
            //CGTools.log("ClosestData for size " + titanLevel.ToString() + " is titan size " + closestData.titanLevel.ToString());
            return closestData;
        }

        public class MovesetData
        {
            public PTAction action;
            public float titanLevel;
            public bool scaledData = false;

            //load first hitboxes to last hitboxes in time order
            public Hitbox[] hitboxes;

            public MovesetData(PTAction action, float titanLevel)
            {
                this.action = action;
                this.titanLevel = titanLevel;
            }

            public MovesetData Copy()
            {
                MovesetData copy = new MovesetData(action, titanLevel);
                List<Hitbox> hitboxCopies = new List<Hitbox>();
                foreach (Hitbox hitbox in hitboxes)
                {
                    hitboxCopies.Add(hitbox.Copy());
                }
                copy.hitboxes = hitboxCopies.ToArray();
                return copy;
            }

            //although Im not running into performance issues, this will take out a certain number of hitboxes from the middle
            //so they wont be calculated.  most are overlapping and close together anyway.
            //this will save a lot of performance especially with large playercounts
            public void pruneData(int pruningLevel)
            {
                if (hitboxes == null || hitboxes.Length == 0 || pruningLevel <= 1) return;

                List<Hitbox> newHitboxes = new List<Hitbox>();
                if (hitboxes.Length > 4) //ignore the 1 frame hitboxes like the bite and slam
                {                        //we want those
                    for (int i = 0; i < hitboxes.Length; i++)
                    {
                        //the starts and ends of hitData tend to be more important
                        if (i > 4 && i < hitboxes.Length - 4)
                        {
                            if (i % pruningLevel == 0)
                            {
                                newHitboxes.Add(hitboxes[i]);
                            }
                        }
                        else
                        {
                            newHitboxes.Add(hitboxes[i]);
                        }
                    }
                }
                if (newHitboxes.Count > 0)
                {
                    hitboxes = newHitboxes.ToArray();
                }
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
            public float level;
            public bool scaledData = false;

            public Hitbox(Vector3 pos, float time, float level)
            {
                this.pos = pos;
                this.time = time;
                this.level = level;
            }

            public virtual Hitbox Copy()
            {
                return new Hitbox(pos, time, level);
            }

            /// <summary>
            /// checks if the selected point is within this hitbox
            /// </summary>
            /// <param name="target"></param>
            /// <param name="owner"></param>
            /// <returns></returns>

            public virtual bool CheckTrigger(Vector3 target, Transform owner)
            {
                return false;
            }

            public virtual void Scale(float newTitanLevel)
            {
                scaledData = true;
            }

            public virtual string GetDataString()
            {
                if (scaledData)
                    return "Scaled Data! this should never be saved";
                return "(" + time.ToString() + "[" + pos.x.ToString() + "," + pos.y.ToString() + "," + pos.z.ToString() + "])";
            }
        }

        public class HitboxRectangle : Hitbox
        {
            public Vector3 RectangleDimentions;
            private Vector3 HalfDimentions;
            public Quaternion RectangleRotation;

            public HitboxRectangle(Vector3 pos, float time, float level, Vector3 RectangleDimentions, Quaternion rectangleRotation) : base(pos, time, level)
            {
                this.RectangleDimentions = RectangleDimentions;
                RectangleRotation = rectangleRotation;
                //CGTools.log("RectangleRotation: " + rectangleRotation.ToString());
                HalfDimentions = new Vector3(RectangleDimentions.x * level * 1.5f / 2f, RectangleDimentions.y * level * 1.5f / 2f, RectangleDimentions.z * level * 1.5f / 2f);
            }

            public override Hitbox Copy()
            {
                return new HitboxRectangle(pos, time, level, RectangleDimentions, RectangleRotation);
            }

            public override bool CheckTrigger(Vector3 target, Transform owner)
            {
                Vector3 center = CGTools.TransformPointUnscaled(owner, pos);
                Vector3 dx = owner.TransformDirection(RectangleRotation * Vector3.right).normalized;
                Vector3 dy = owner.TransformDirection(RectangleRotation * Vector3.up).normalized;
                Vector3 dz = owner.TransformDirection(RectangleRotation * Vector3.forward).normalized;
                Vector3 d = target - center;
                bool b = Mathf.Abs(Vector3.Dot(d, dx)) <= HalfDimentions.x && Mathf.Abs(Vector3.Dot(d, dy)) <= HalfDimentions.y && Mathf.Abs(Vector3.Dot(d, dz)) <= HalfDimentions.z;
                return b;
            }

            public override void Scale(float newTitanLevel)
            {
                pos = Vector3.Scale(pos / level, new Vector3(newTitanLevel, newTitanLevel, newTitanLevel));
                HalfDimentions = new Vector3(RectangleDimentions.x * newTitanLevel * 1.5f / 2f, RectangleDimentions.y * newTitanLevel * 1.5f / 2f, RectangleDimentions.z * newTitanLevel * 1.5f / 2f);
                scaledData = true;
            }

            public override string GetDataString()
            {
                return "(Hrect{" + RectangleDimentions.x + "|" + RectangleDimentions.y + "|" + RectangleDimentions.z + "|" + RectangleRotation.x + "|" + RectangleRotation.y + "|" + RectangleRotation.z + "|" + RectangleRotation.w + "}," + time.ToString() + "[" + pos.x.ToString() + "," + pos.y.ToString() + "," + pos.z.ToString() + "])";
            }
        }

        public class HitboxSphere : Hitbox
        {
            public float radius;

            public HitboxSphere(Vector3 pos, float time, float level, float radius) : base(pos, time, level)
            {
                this.radius = radius;
            }

            public override Hitbox Copy()
            {
                return new HitboxSphere(pos, time, level, radius);
            }

            public override void Scale(float newTitanLevel)
            {
                Vector3 newpos = Vector3.Scale(pos / level, new Vector3(newTitanLevel, newTitanLevel, newTitanLevel));
                float newradius = 2.4f * newTitanLevel + 1f;
                pos = newpos;
                radius = newradius;
                level = newTitanLevel;
                scaledData = true;
            }


            public override bool CheckTrigger(Vector3 target, Transform owner)
            {
                Vector3 tp = CGTools.TransformPointUnscaled(owner, pos);
                if (debugMovementChecking) CGTools.pointsToTrack.Add(tp);
                if (Vector3.Distance(target, tp) < radius - level)
                {
                    if (debugMovementChecking) CGTools.redPointsToTrack.Add(target);
                    return true;
                }
                if (debugMovementChecking) CGTools.greenPointsToTrack.Add(target);
                return false;
            }

            public override string GetDataString()
            {
                return "(Hsphere{" + radius.ToString() + "}," + time.ToString() + "[" + pos.x.ToString() + "," + pos.y.ToString() + "," + pos.z.ToString() + "])";
            }
        }
    }
}
