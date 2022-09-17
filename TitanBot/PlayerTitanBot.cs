using Constants;
using System;
using System.Collections.Generic;
using UnityEngine;
using static TitanBot.HitData;

namespace TitanBot
{
    /// <summary>
    /// Replacement for TITAN_CONTROLLER that tries to replicate PT gameplay
    /// We replace the TITAN_CONTROLLER so that all of our movements and actions are constrained in the same way a normal PT would be
    /// and only output to the variables that would be set by your keyboard inputs
    /// </summary>
    public class PlayerTitanBot : TITAN_CONTROLLER
    {
        public static bool TakeOverPT = false;
        public static bool ReplaceSpawnedTitans = false;
        public static int raycasts = 10;
        public static float spinrate = 800f;
        public static float turnrate = 0.005f;
        public static bool debugRaycasts = false;
        public static bool debugTargets = false;
        public static bool useCustomHair = true;
        public static List<PTAction> TempActionsList = new List<PTAction>();

        public bool doStuff = true;
        public TITAN MyTitan = null;

        private Dictionary<PTAction, bool> updateNextFrameList = new Dictionary<PTAction, bool>();
        private GameObject[] players;
        private GameObject closestPlayer;
        private TitanState state = TitanState.Running;
        private List<Vector3> lastRaycasts = new List<Vector3>();
        private float stateTimer = 0f;
        private float targetDirectionLerp = 0f;
        private float changeTargetDirectionTimer = 0f;
        private float RaycastDirectionChangeTimer = 0f;
        private float targetLerpT = 0f;
        private float SpeedTimer = 0f;
        private Vector3 lastFarthestPoint = Vector3.zero;
        private Difficulty myDifficulty = Difficulty.VeryVeryHard;


        // public float forsight = 4f;//how far into the future titan will predict player velocity
        // public int forsightSteps = 2;//how many steps not including the current position that the titan will predict

        //calculated moveset data for this titan to use
        private Dictionary<PTAction, HitData.MovesetData> MovesetDatabase = new Dictionary<PTAction, HitData.MovesetData>();

        //All the actions we want to calculate movesetData for
        public static PTAction[] pTActions = {
            PTAction.Attack,
            PTAction.Jump,
            PTAction.choptl,
            PTAction.choptr,
        };

        /// <summary>
        /// Main AI Decision making function
        /// </summary>
        private void AIMaster()
        {
            players = GetPlayersToCalculate();

            //every half second there is a 10% chance that we walk for that period
            //unless a function overrides the walk input
            if (CGTools.timer(ref SpeedTimer, 0.5f))
            {
                if (CGTools.Chance(10f))
                {
                    isWALKDown = true;
                }
                else
                {
                    isWALKDown = false;
                }
            }

            //every 5 seconds we change state
            if (CGTools.timer(ref stateTimer, 5f))
            {
                int s = CGTools.WeightTable(new int[]
                {
                    2,
                    1,
                    4,
                    1
                });

                if (s == 0)
                {
                    state = TitanState.Repositioning;
                    //CGTools.log(state.ToString());
                    stateTimer = Time.time + 2f;
                }
                if (s == 1)
                {
                    state = TitanState.Running;
                    stateTimer = Time.time + 2f;
                    //CGTools.log(state.ToString());
                }
                if (s == 2)
                {
                    state = TitanState.Attacking;
                    //CGTools.log(state.ToString());
                }
                if (s == 3)
                {
                    state = TitanState.Spinning;
                    //CGTools.log(state.ToString());
                    stateTimer = Time.time + 1f;
                }
            }

            if (state == TitanState.Running)
            {
                run();
                letErRip();
            }
            else if (state == TitanState.Attacking)
            {
                bool flag = false;
                if (closestPlayer != null)
                {
                    if (Vector3.Distance(closestPlayer.transform.position, MyTitan.transform.position) < 200f)
                    {
                        if (Vector3.Distance(closestPlayer.transform.position, MyTitan.neck.position) < 40f || Vector3.Distance(closestPlayer.transform.position, MyTitan.transform.position) < 40f)
                        {
                            flag = true;
                            reposition();
                        }
                        else
                        {
                            flag = true;
                            Agro();
                        }
                    }
                }
                if (!flag)
                {
                    run();
                }
                letErRip();
            }
            else if (state == TitanState.Spinning)
            {
                spin();
            }
            else if (state == TitanState.Repositioning)
            {
                bool flag = false;
                if (closestPlayer != null)
                {
                    if (Vector3.Distance(closestPlayer.transform.position, MyTitan.neck.position) < 100f)
                    {
                        flag = true;
                        reposition();
                    }
                }
                if (!flag)
                {
                    run();
                }
                letErRip();
            }

            //turn left and right on a sin wave to keep our movements a bit sparratic
            //also convinently helps our raycasting
            float wobbleRate = 30f;
            float wobble = Mathf.Sin(Time.time * 4f) * wobbleRate;
            float targetDirectionFinal = targetDirectionLerp + wobble;
            targetLerpT += turnrate * Time.deltaTime;
            if (state != TitanState.Spinning)
                targetDirection = Mathf.Lerp(targetDirection, targetDirectionFinal, targetLerpT);
            if (debugRaycasts)
            {
                CGTools.pointsToTrack.AddRange(lastRaycasts);
                //showRaycasts();
            }
        }

        private bool checkWalls()
        {
            Vector3 rayOrigin = MyTitan.transform.position + Vector3.up * 5f;
            Vector3 rayOrigin2 = MyTitan.transform.position + (MyTitan.transform.right * (MyTitan.myLevel + 1f)) + Vector3.up * 5f;
            Vector3 rayOrigin3 = MyTitan.transform.position + ((MyTitan.transform.right * (MyTitan.myLevel + 1f)) * -1f) + Vector3.up * 5f;
            Vector3 rayDirection = MyTitan.transform.forward;
            Ray ray = new Ray(rayOrigin, rayDirection);
            Ray ray2 = new Ray(rayOrigin2, rayDirection);
            Ray ray3 = new Ray(rayOrigin3, rayDirection);
            LayerMask mask = ((int)1) << PhysicsLayer.Ground;
            //CGTools.greenPointsToTrack.Add(MyTitan.transform.position + Quaternion.Euler(new Vector3(0f,targetDirectionLerp,0f)) * MyTitan.transform.forward * 50f);
            bool flag = false;
            if (Physics.Raycast(ray, out RaycastHit raycastHit, 50f, mask.value))
            {
                flag = true;
            }
            if (Physics.Raycast(ray2, out RaycastHit raycastHit2, 50f, mask.value))
            {
                flag = true;
            }
            if (Physics.Raycast(ray3, out RaycastHit raycastHit3, 50f, mask.value))
            {
                flag = true;
            }
            if (!flag)
            {
                RaycastDirectionChangeTimer = 0f;
            }
            if (debugRaycasts)
            {
                CGTools.redPointsToTrack.Add(raycastHit.point);
                CGTools.redPointsToTrack.Add(raycastHit2.point);
                CGTools.redPointsToTrack.Add(raycastHit3.point);
            }
            return flag;
        }

        //run towards the closest player
        private void Agro()
        {
            Vector3 d = closestPlayer.transform.position - MyTitan.transform.position;
            Quaternion r = Quaternion.LookRotation(d);
            if (debugTargets)
                CGTools.greenPointsToTrack.Add(closestPlayer.transform.position);
            targetDirectionLerp = r.eulerAngles.y;
        }

        //run away from the closest player
        private void reposition()
        {
            Vector3 d = closestPlayer.transform.position - MyTitan.transform.position;
            Quaternion r = Quaternion.LookRotation(d);

            //if someone is on our ass check if we are running into a wall and if we are then change are state to running to hopefully avoid an easy kill
            if (Vector3.Distance(closestPlayer.transform.position, MyTitan.neck.position) < 20f)
            {
                if (checkWalls())
                {
                    if (CGTools.timer(ref RaycastDirectionChangeTimer, 1f))
                    {
                        PickNewDirection();
                        isWALKDown = false;
                        state = TitanState.Running;
                        stateTimer = Time.time + 1.5f;
                    }
                }
            }

            if (debugTargets)
                CGTools.redPointsToTrack.Add(closestPlayer.transform.position);
            targetDirectionLerp = Quaternion.Inverse(r).eulerAngles.y;
        }
        //run to the furthest point around us
        private void run()
        {
            //check if there is a wall in front of us, if there is we pick a new direction
            if (checkWalls())
            {
                if (CGTools.timer(ref RaycastDirectionChangeTimer, 1f))
                {
                    PickNewDirection();
                }
            }

            //not really sure if this is neccessary but why not
            if (targetDirection > 360f)
            {
                targetDirection = 0f;
            }
            if (targetDirection < 0f)
            {
                targetDirection = 360f;
            }

            //every 2 seconds find a new direction to run
            if (CGTools.timer(ref changeTargetDirectionTimer, 2f))
            {
                PickNewDirection();
            }

            //lastFarthestPoint is set by PickNewDirection() and should be the point we are running to
            if (debugRaycasts)
                CGTools.greenPointsToTrack.Add(lastFarthestPoint);


        }


        //raycast 360 degrees starting at set intervals and find the point that goes the farthest
        //we can get stuck in a gap like this but the wobble helps us not be stuck forever
        private void PickNewDirection()
        {

            int start = (int)UnityEngine.Random.Range(0f, 45f);
            float lastFarthestDistance = 0f;
            if (debugRaycasts)
            {
                lastRaycasts.Clear();
            }

            Vector3 rayOrigin2 = MyTitan.transform.position + Vector3.up * 10f;
            for (int i = 0; i < raycasts; i++)
            {

                Vector3 rayDirection2 = Quaternion.Euler(new Vector3(0f, i * (360 / raycasts), 0f)) * MyTitan.transform.forward;
                Ray ray2 = new Ray(rayOrigin2, rayDirection2);
                LayerMask mask = ((int)1) << PhysicsLayer.Ground;
                if (Physics.Raycast(ray2, out RaycastHit raycastHit2, 10000f, mask.value))
                {
                    if (raycastHit2.distance > lastFarthestDistance)
                    {
                        lastFarthestDistance = raycastHit2.distance;
                        lastFarthestPoint = raycastHit2.point;
                    }

                }
                if (debugRaycasts)
                {
                    lastRaycasts.Add(raycastHit2.point);
                }
            }


            Vector3 direction = lastFarthestPoint - MyTitan.transform.position;
            Quaternion rotation = Quaternion.LookRotation(direction);
            targetDirectionLerp = rotation.eulerAngles.y;
        }

        private void showRaycasts()
        {
            Vector3 rayOrigin2 = MyTitan.transform.position + Vector3.up * 10f;
            for (int i = 0; i < raycasts; i++)
            {
                //float angle = CGTools.Wrap(start + i * 18f, 0f, 360f);
                Vector3 rayDirection2 = Quaternion.Euler(new Vector3(0f, i * (360 / raycasts), 0f)) * MyTitan.transform.forward;
                Ray ray2 = new Ray(rayOrigin2, rayDirection2);
                Physics.Raycast(ray2, out RaycastHit raycastHit2);
                CGTools.pointsToTrack.Add(raycastHit2.point);
            }
        }

        //spin in a circle really quickly
        private void spin()
        {
            isWALKDown = true;
            if (targetDirection > 360f)
            {
                targetDirection = 0f;
            }
            if (targetDirection < 0f)
            {
                targetDirection = 360f;
            }
            targetDirection += spinrate * Time.deltaTime;
        }

        //check for players that will run into a hitbox and attack if they do
        private bool letErRip()
        {
            if (players.Length == 0)
            {
                return false;
            }
            PTAction NextAction = CheckAttacks(players);
            if (NextAction != PTAction.nothing)
            {
                ExecuteAction(NextAction);
                return true;
            }
            return false;
        }


        private void ExecuteAction(PTAction a)
        {
            gameObject.rigidbody.velocity = Vector3.zero;
            if (a == PTAction.Attack)
            {
                isAttackDown = true;
            }
            if (a == PTAction.Jump)
            {
                isJumpDown = true;
            }
            if (a == PTAction.bite)
            {
                bite = true;
            }
            if (a == PTAction.bitel)
            {
                bitel = true;
            }
            if (a == PTAction.biter)
            {
                biter = true;
            }
            if (a == PTAction.choptl)
            {
                choptl = true;
            }
            if (a == PTAction.choptr)
            {
                choptr = true;
            }
            if (a == PTAction.grabbackl)
            {
                grabbackl = true;
            }
            if (a == PTAction.grabbackr)
            {
                grabbackr = true;
            }
            if (a == PTAction.grabfrontl)
            {
                grabfrontl = true;
            }
            if (a == PTAction.grabfrontr)
            {
                grabfrontr = true;
            }
            if (a == PTAction.grabnapel)
            {
                grabnapel = true;
            }
            if (a == PTAction.grabnaper)
            {
                grabnaper = true;
            }
            if (a == PTAction.chopl)
            {
                chopl = true;
            }
            if (a == PTAction.chopr)
            {
                chopr = true;
            }
            if (a == PTAction.AttackII)
            {
                isAttackIIDown = true;
            }


        }

        private void Start()
        {
            MyTitan = gameObject.GetComponent<TITAN>();
            foreach (PTAction action in Enum.GetValues(typeof(PTAction)))
            {
                updateNextFrameList.Add(action, false);
            }
        }

        public void LiveUpdateMovesetData()
        {
            MovesetDatabase.Clear();

            CalculateMovesetData(MyTitan.myLevel);
        }

        /// <summary>
        /// finds the closest movesetData from the database
        /// if its not an exact match create a scaled movesetData and keep it in the database
        /// </summary>
        /// <param name="titanLevel"></param>
        public void CalculateMovesetData(float titanLevel)
        {
            foreach (PTAction action in pTActions)
            {
                HitData.MovesetData closestData = HitData.GetClosestData(action, titanLevel);
                if (closestData.titanLevel == titanLevel)
                {
                    MovesetDatabase.Add(action, closestData);
                }
                else
                {
                    MovesetData scaledData = new MovesetData(action, titanLevel);
                    scaledData.scaledData = true;
                    List<HitData.Hitbox> HitboxList = new List<HitData.Hitbox>();
                    foreach (HitData.Hitbox h in closestData.hitboxes)
                    {
                        HitData.Hitbox newH = h.Copy();
                        newH.scaledData = true;
                        newH.Scale(titanLevel);
                        HitboxList.Add(newH);
                    }
                    scaledData.hitboxes = HitboxList.ToArray();
                    HitData.AddData(scaledData);
                    MovesetDatabase.Add(action, scaledData);
                }



                //HitData.MovesetData data = HitData.GetClosestData(action, titanLevel);
                //if (data != null)
                //    MovesetDatabase.Add(action, data);
                //else
                //{
                //    CGTools.log("CalculateMovesetData(float titanLevel) > Null Data for PTAction [" + action.ToString() + "]");
                //}
            }
        }

        private void Update()
        {
            if (MyTitan == null) return;
            if (doStuff)
                AIMaster();

            //if something is set on one frame unset it the next frame to make my life easier
            if (isAttackDown)
            {
                if (updateNextFrameList[PTAction.Attack])
                {
                    this.isAttackDown = false;
                    updateNextFrameList[PTAction.Attack] = false;
                }
                else
                    updateNextFrameList[PTAction.Attack] = true;
            }
            if (isAttackIIDown)
            {
                if (updateNextFrameList[PTAction.AttackII])
                {
                    this.isAttackIIDown = false;
                    updateNextFrameList[PTAction.AttackII] = false;
                }
                else
                    updateNextFrameList[PTAction.AttackII] = true;
            }
            if (isJumpDown)
            {
                if (updateNextFrameList[PTAction.Jump])
                {
                    this.isJumpDown = false;
                    updateNextFrameList[PTAction.Jump] = false;
                }
                else
                    updateNextFrameList[PTAction.Jump] = true;
            }
            if (isSuicide)
            {
                if (updateNextFrameList[PTAction.Suicide])
                {
                    this.isSuicide = false;
                    updateNextFrameList[PTAction.Suicide] = false;
                }
                else
                    updateNextFrameList[PTAction.Suicide] = true;
            }
            if (grabbackl)
            {
                if (updateNextFrameList[PTAction.grabbackl])
                {
                    this.grabbackl = false;
                    updateNextFrameList[PTAction.grabbackl] = false;
                }
                else
                    updateNextFrameList[PTAction.grabbackl] = true;
            }
            if (grabbackr)
            {
                if (updateNextFrameList[PTAction.grabbackr])
                {
                    this.grabbackr = false;
                    updateNextFrameList[PTAction.grabbackr] = false;
                }
                else
                    updateNextFrameList[PTAction.grabbackr] = true;
            }
            if (grabfrontl)
            {
                if (updateNextFrameList[PTAction.grabfrontl])
                {
                    this.grabfrontl = false;
                    updateNextFrameList[PTAction.grabfrontl] = false;
                }
                else
                    updateNextFrameList[PTAction.grabfrontl] = true;
            }
            if (grabfrontr)
            {
                if (updateNextFrameList[PTAction.grabfrontr])
                {
                    this.grabfrontr = false;
                    updateNextFrameList[PTAction.grabfrontr] = false;
                }
                else
                    updateNextFrameList[PTAction.grabfrontr] = true;
            }
            if (grabnapel)
            {
                if (updateNextFrameList[PTAction.grabnapel])
                {
                    this.grabnapel = false;
                    updateNextFrameList[PTAction.grabnapel] = false;
                }
                else
                    updateNextFrameList[PTAction.grabnapel] = true;
            }
            if (grabnaper)
            {
                if (updateNextFrameList[PTAction.grabnaper])
                {
                    this.grabnaper = false;
                    updateNextFrameList[PTAction.grabnaper] = false;
                }
                else
                    updateNextFrameList[PTAction.grabnaper] = true;
            }
            if (choptl)
            {
                if (updateNextFrameList[PTAction.choptl])
                {
                    this.choptl = false;
                    updateNextFrameList[PTAction.choptl] = false;
                }
                else
                    updateNextFrameList[PTAction.choptl] = true;
            }
            if (chopr)
            {
                if (updateNextFrameList[PTAction.chopr])
                {
                    this.chopr = false;
                    updateNextFrameList[PTAction.chopr] = false;
                }
                else
                    updateNextFrameList[PTAction.chopr] = true;
            }
            if (chopl)
            {
                if (updateNextFrameList[PTAction.chopl])
                {
                    this.chopl = false;
                    updateNextFrameList[PTAction.chopl] = false;
                }
                else
                    updateNextFrameList[PTAction.chopl] = true;
            }
            if (choptr)
            {
                if (updateNextFrameList[PTAction.choptr])
                {
                    this.choptr = false;
                    updateNextFrameList[PTAction.choptr] = false;
                }
                else
                    updateNextFrameList[PTAction.choptr] = true;
            }
            if (bite)
            {
                if (updateNextFrameList[PTAction.bite])
                {
                    this.bite = false;
                    updateNextFrameList[PTAction.bite] = false;
                }
                else
                    updateNextFrameList[PTAction.bite] = true;
            }
            if (bitel)
            {
                if (updateNextFrameList[PTAction.bitel])
                {
                    this.bitel = false;
                    updateNextFrameList[PTAction.bitel] = false;
                }
                else
                    updateNextFrameList[PTAction.bitel] = true;
            }
            if (biter)
            {
                if (updateNextFrameList[PTAction.biter])
                {
                    this.biter = false;
                    updateNextFrameList[PTAction.biter] = false;
                }
                else
                    updateNextFrameList[PTAction.biter] = true;
            }
            if (cover)
            {
                if (updateNextFrameList[PTAction.cover])
                {
                    this.cover = false;
                    updateNextFrameList[PTAction.cover] = false;
                }
                else
                    updateNextFrameList[PTAction.cover] = true;
            }
            if (sit)
            {
                if (updateNextFrameList[PTAction.sit])
                {
                    this.cover = false;
                    updateNextFrameList[PTAction.sit] = false;
                }
                else
                    updateNextFrameList[PTAction.sit] = true;
            }

        }

        /// <summary>
        /// checks for the quickest attack that can be excecuted and returns the corresponding PTAction
        /// </summary>
        /// <param name="PlayersToCheck"></param>
        /// <returns></returns>
        private PTAction CheckAttacks(GameObject[] PlayersToCheck)
        {
            PTAction result = PTAction.nothing;
            float lowestTime = float.MaxValue;

            foreach (PTAction action in MovesetDatabase.Keys)
            {
                foreach (GameObject p in PlayersToCheck)
                {

                    //float f = FloatingFire.checkMoveset(MovesetDatabase[action], PTTools.predictPlayerMotion(p, (forsight / forsightSteps) * i), (forsight / forsightSteps) * i);
                    float f = HitData.AdvanceMovesetCheck(MovesetDatabase[action], p, MyTitan.transform);
                    if (f < lowestTime)
                    {
                        lowestTime = f;
                        result = action;
                    }


                }
            }

            return result;
        }

        /// <summary>
        /// Finds nearby players as well as players on a trajectory towards the titan
        /// </summary>
        /// <returns></returns>
        private GameObject[] GetPlayersToCalculate()
        {
            float shortestDistance = float.MaxValue;
            GameObject closest = null;
            List<GameObject> list = new List<GameObject>();
            foreach (GameObject g in CGTools.GetHeros())
            {
                float dist = Vector3.Distance(g.transform.position, MyTitan.transform.position);
                if (dist < 200f)
                {
                    if (dist < shortestDistance)
                    {
                        shortestDistance = dist;
                        closest = g;
                    }
                    list.Add(g);
                }
                else
                {
                    Vector3 pos1 = PTTools.PredictPlayerMotion(g, 1f);
                    Vector3 pos2 = PTTools.PredictPlayerMotion(g, 2f);
                    if (Vector3.Distance(pos1, MyTitan.transform.position) < 200f || Vector3.Distance(pos1, MyTitan.transform.position) < 200f)
                    {
                        list.Add(g);
                    }
                }
            }
            if (list.Count > 0)
            {
                closestPlayer = closest;
                return list.ToArray();
            }
            else
            {
                return new GameObject[0];
            }
        }
    }

    public enum TitanState
    {
        Running,
        Repositioning,
        Attacking,
        Spinning
    }


}
