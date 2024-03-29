﻿using Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection.Emit;
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
        public static int dataPruningLevel = 2;
        public static string TitanName = "PTBot";
        public static bool useCustomSpeed = false;
        public static float titanSpeed = 60f;
        public static bool AllowEyeHits = true;
        public static bool AllowAnkleHits = true;

        public static int[] AIWeightTable = new int[]
        {
            2,
            1,
            4,
            1
        };
        public static float AIChangeStateTime = 5f;
        public static float AIWobbleMagnitude = 30f;
        public static float AIWobbleRate = 4f;
        public static float AIWalkChance = 10f;

        public bool doStuff = true;
        public TITAN MyTitan = null;

        private Dictionary<PTAction, bool> updateNextFrameList = new Dictionary<PTAction, bool>();
        private GameObject[] players;
        private GameObject closestPlayer;
        private TitanBotState state = TitanBotState.Running;
        private List<Vector3> lastRaycasts = new List<Vector3>();
        private float stateTimer = 0f;
        private float targetDirectionLerp = 0f;
        private float changeTargetDirectionTimer = 0f;
        private float RaycastDirectionChangeTimer = 0f;
        private float targetLerpT = 0f;
        private float SpeedTimer = 0f;
        private Vector3 lastFarthestPoint = Vector3.zero;
        private Vector3 cityGate = new Vector3(0f, 0f, 900f);
        private float myOriginalSpeed = 0f;


        public bool is_combo_2 = false;
        public bool is_combo_3 = false;
        public bool is_front_ground = false;
        public bool is_kick = false;
        public bool is_slap_back = false;
        public bool is_slap_face = false;
        public bool is_stomp = false;
        public bool is_crawler_jump_0 = false;
        public bool is_grab_head_front_r = false;
        public bool is_grab_head_front_l = false;

        // public float forsight = 4f;//how far into the future titan will predict player velocity
        // public int forsightSteps = 2;//how many steps not including the current position that the titan will predict

        //calculated moveset data for this titan to use
        public Dictionary<PTAction, HitData.MovesetData> MovesetDatabase = new Dictionary<PTAction, HitData.MovesetData>();

        //All the actions we want to calculate movesetData for
        public static PTAction[] pTActions = 
        {
            PTAction.Attack, 
            PTAction.Jump,   
            PTAction.choptl, 
            PTAction.choptr  
        };

        /// <summary>
        /// Main AI Decision making function
        /// </summary>
        private void AIMaster()
        {
            players = GetPlayersToCalculate();

            //every half second there is a 10% chance that we walk for that period
            //unless a function overrides the walk input
            //we will also check and set the titans speed on this timer, because it is set like 30 different times when it is spawned
            //and the final speed is set in an rpc which makes it hard to set consistantly
            if (CGTools.timer(ref SpeedTimer, 0.5f))
            {
                if (useCustomSpeed)
                {
                    if (MyTitan.speed != titanSpeed)
                    {
                        myOriginalSpeed = MyTitan.speed;
                        MyTitan.speed = titanSpeed;
                    }
                }
                else
                {
                    if (myOriginalSpeed != 0f && MyTitan.speed != myOriginalSpeed)
                    {
                        MyTitan.speed = myOriginalSpeed;
                    }
                }
                if (CGTools.Chance(AIWalkChance))
                {
                    isWALKDown = true;
                }
                else
                {
                    isWALKDown = false;
                }
            }

            //every 5 seconds we change state
            if (CGTools.timer(ref stateTimer, AIChangeStateTime))
            {
                int s = CGTools.WeightTable(AIWeightTable);
                if (s == 0)
                {
                    state = TitanBotState.Repositioning;
                    //CGTools.log(state.ToString());
                    stateTimer = Time.time + 2f;
                }
                if (s == 1)
                {
                    state = TitanBotState.Running;
                    stateTimer = Time.time + 2f;
                    //CGTools.log(state.ToString());
                }
                if (s == 2)
                {
                    state = TitanBotState.Attacking;
                    //CGTools.log(state.ToString());
                }
                if (s == 3)
                {
                    state = TitanBotState.Spinning;
                    //CGTools.log(state.ToString());
                    stateTimer = Time.time + 1f;
                }
            }
            if (state == TitanBotState.Running)
            {
                run();
                letErRip();
            }
            else if (state == TitanBotState.Attacking)
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
            else if (state == TitanBotState.Spinning)
            {
                spin();
            }
            else if (state == TitanBotState.Repositioning)
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
            float wobble = Mathf.Sin(Time.time * AIWobbleRate) * AIWobbleMagnitude;
            float targetDirectionFinal = targetDirectionLerp + wobble;
            targetLerpT += turnrate * Time.deltaTime;
            if (state != TitanBotState.Spinning)
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
            LayerMask mask = 1 << PhysicsLayer.Ground;
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
                        state = TitanBotState.Running;
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
            bool isCity = (FengGameManagerMKII.level == "The City");
            bool isForest = (LevelInfo.getInfo(FengGameManagerMKII.level).mapName.Contains("Forest"));
            float distance = 0f;
            if (isForest)
            {
                distance = Vector3.Distance(MyTitan.transform.position, new Vector3(0f, MyTitan.transform.position.y, 0f));
            }
            Vector3 rayOrigin2 = MyTitan.transform.position + Vector3.up * 10f;
            for (int i = 0; i < raycasts; i++)
            {

                Vector3 rayDirection2 = Quaternion.Euler(new Vector3(0f, i * (360 / raycasts), 0f)) * MyTitan.transform.forward;
                Ray ray2 = new Ray(rayOrigin2, rayDirection2);
                
                LayerMask mask = ((int)1) << PhysicsLayer.Ground;
                if (Physics.Raycast(ray2, out RaycastHit raycastHit2, 10000f, mask.value))
                {
                    if (isForest)
                    {
                        if (distance > 250f)
                        {
                            if (raycastHit2.distance - Vector3.Distance(raycastHit2.point, new Vector3(0f, MyTitan.transform.position.y, 0f)) > lastFarthestDistance)
                            {
                                lastFarthestDistance = raycastHit2.distance;
                                lastFarthestPoint = raycastHit2.point;
                            }
                        }
                        else
                        {
                            if (raycastHit2.distance > lastFarthestDistance)
                            {
                                lastFarthestDistance = raycastHit2.distance;
                                lastFarthestPoint = raycastHit2.point;
                            }
                        }
                        
                    }
                    else if (isCity && Vector3.Distance(raycastHit2.point, cityGate) < 300f)
                    {

                    }
                    else if (raycastHit2.distance > lastFarthestDistance)
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
            if (MyTitan.state != TitanState.attack)
            {
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
                    gameObject.rigidbody.velocity = Vector3.zero;
                    bite = true;
                }
                if (a == PTAction.bitel)
                {
                    gameObject.rigidbody.velocity = Vector3.zero;
                    bitel = true;
                }
                if (a == PTAction.biter)
                {
                    gameObject.rigidbody.velocity = Vector3.zero;
                    biter = true;
                }
                if (a == PTAction.choptl)
                {
                    gameObject.rigidbody.velocity = Vector3.zero;
                    choptl = true;
                }
                if (a == PTAction.choptr)
                {
                    gameObject.rigidbody.velocity = Vector3.zero;
                    choptr = true;
                }
                if (a == PTAction.grabbackl)
                {
                    gameObject.rigidbody.velocity = Vector3.zero;
                    grabbackl = true;
                }
                if (a == PTAction.grabbackr)
                {
                    gameObject.rigidbody.velocity = Vector3.zero;
                    grabbackr = true;
                }
                if (a == PTAction.grabfrontl)
                {
                    gameObject.rigidbody.velocity = Vector3.zero;
                    grabfrontl = true;
                }
                if (a == PTAction.grabfrontr)
                {
                    gameObject.rigidbody.velocity = Vector3.zero;
                    grabfrontr = true;
                }
                if (a == PTAction.grabnapel)
                {
                    gameObject.rigidbody.velocity = Vector3.zero;
                    grabnapel = true;
                }
                if (a == PTAction.grabnaper)
                {
                    gameObject.rigidbody.velocity = Vector3.zero;
                    grabnaper = true;
                }
                if (a == PTAction.chopl)
                {
                    gameObject.rigidbody.velocity = Vector3.zero;
                    chopl = true;
                }
                if (a == PTAction.chopr)
                {
                    gameObject.rigidbody.velocity = Vector3.zero;
                    chopr = true;
                }
                if (a == PTAction.AttackII)
                {
                    gameObject.rigidbody.velocity = Vector3.zero;
                    isAttackIIDown = true;
                }
                if (a == PTAction.combo_2)
                {
                    gameObject.rigidbody.velocity = Vector3.zero;
                    is_combo_2 = true;
                }
                if (a == PTAction.combo_3)
                {
                    gameObject.rigidbody.velocity = Vector3.zero;
                    is_combo_3 = true;
                }
                if (a == PTAction.front_ground)
                {
                    gameObject.rigidbody.velocity = Vector3.zero;
                    is_front_ground = true;
                }
                if (a == PTAction.kick)
                {
                    gameObject.rigidbody.velocity = Vector3.zero;
                    is_kick = true;
                }
                if (a == PTAction.slap_back)
                {
                    gameObject.rigidbody.velocity = Vector3.zero;
                    is_slap_back = true;
                }
                if (a == PTAction.slap_face)
                {
                    gameObject.rigidbody.velocity = Vector3.zero;
                    is_slap_face = true;
                }
                if (a == PTAction.stomp)
                {
                    gameObject.rigidbody.velocity = Vector3.zero;
                    is_stomp = true;
                }
                if (a == PTAction.crawler_jump_0)
                {
                    is_crawler_jump_0 = true;
                }
                if (a == PTAction.grab_head_front_l)
                {
                    gameObject.rigidbody.velocity = Vector3.zero;
                    is_grab_head_front_l = true;
                }
                if (a == PTAction.grab_head_front_r)
                {
                    gameObject.rigidbody.velocity = Vector3.zero;
                    is_grab_head_front_r = true;
                }
            }
        }

        private void Start()
        {
            MyTitan = gameObject.GetComponent<TITAN>();
            foreach (PTAction action in Enum.GetValues(typeof(PTAction)))
            {
                updateNextFrameList.Add(action, false);
            }
            CalculateMovesetData();
        }

        public void LiveUpdateMovesetData()
        {
            MovesetDatabase.Clear();
            CalculateMovesetData();
        }

        /// <summary>
        /// finds the closest movesetData from the database
        /// if its not an exact match create a scaled movesetData and keep it in the database
        /// Also apply pruning here as it takes no time to calculate and we dont want to reload the data
        /// so we just make a copy here, and we can recalculate pruning on the fly
        /// </summary>
        /// <param name="titanLevel"></param>
        public void CalculateMovesetData()
        {
            float titanLevel = MyTitan.myLevel;

            foreach (PTAction action in pTActions)
            {
                HitData.MovesetData closestData = HitData.GetClosestData(action, titanLevel);

                if (closestData == null)
                {
                    CGTools.log("There is no hitbox data for action: " + action.ToString());
                    continue;
                }

                if (closestData.titanLevel == titanLevel)
                {
                    HitData.MovesetData copy = closestData.Copy();
                    copy.pruneData(dataPruningLevel);
                    MovesetDatabase.Add(action, copy);
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
                    HitData.MovesetData scaledDataCopy = scaledData.Copy();
                    scaledDataCopy.pruneData(dataPruningLevel);
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

            //if something is set on one tick unset it the next tick to make my life easier
            //attacks wont be registered otherwize
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
            if (is_combo_2)
            {
                if (updateNextFrameList[PTAction.combo_2])
                {
                    this.is_combo_2 = false;
                    updateNextFrameList[PTAction.combo_2] = false;
                }
                else
                    updateNextFrameList[PTAction.combo_2] = true;
            }
            if (is_combo_3)
            {
                if (updateNextFrameList[PTAction.combo_3])
                {
                    this.is_combo_3 = false;
                    updateNextFrameList[PTAction.combo_3] = false;
                }
                else
                    updateNextFrameList[PTAction.combo_3] = true;
            }
            if (is_front_ground)
            {
                if (updateNextFrameList[PTAction.front_ground])
                {
                    this.is_front_ground = false;
                    updateNextFrameList[PTAction.front_ground] = false;
                }
                else
                    updateNextFrameList[PTAction.front_ground] = true;
            }
            if (is_kick)
            {
                if (updateNextFrameList[PTAction.kick])
                {
                    this.is_kick = false;
                    updateNextFrameList[PTAction.kick] = false;
                }
                else
                    updateNextFrameList[PTAction.kick] = true;
            }
            if (is_slap_back)
            {
                if (updateNextFrameList[PTAction.slap_back])
                {
                    this.is_slap_back = false;
                    updateNextFrameList[PTAction.slap_back] = false;
                }
                else
                    updateNextFrameList[PTAction.slap_back] = true;
            }
            if (is_slap_face)
            {
                if (updateNextFrameList[PTAction.slap_face])
                {
                    this.is_slap_face = false;
                    updateNextFrameList[PTAction.slap_face] = false;
                }
                else
                    updateNextFrameList[PTAction.slap_face] = true;
            }
            if (is_stomp)
            {
                if (updateNextFrameList[PTAction.stomp])
                {
                    this.is_stomp = false;
                    updateNextFrameList[PTAction.stomp] = false;
                }
                else
                    updateNextFrameList[PTAction.stomp] = true;
            }
            if (is_crawler_jump_0)
            {
                if (updateNextFrameList[PTAction.crawler_jump_0])
                {
                    this.is_crawler_jump_0 = false;
                    updateNextFrameList[PTAction.crawler_jump_0] = false;
                }
                else
                    updateNextFrameList[PTAction.crawler_jump_0] = true;
            }
            if (is_grab_head_front_l)
            {
                if (updateNextFrameList[PTAction.grab_head_front_l])
                {
                    this.is_grab_head_front_l = false;
                    updateNextFrameList[PTAction.grab_head_front_l] = false;
                }
                else
                    updateNextFrameList[PTAction.grab_head_front_l] = true;
            }
            if (is_grab_head_front_r)
            {
                if (updateNextFrameList[PTAction.grab_head_front_r])
                {
                    this.is_grab_head_front_r = false;
                    updateNextFrameList[PTAction.grab_head_front_r] = false;
                }
                else
                    updateNextFrameList[PTAction.grab_head_front_r] = true;
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

    public enum TitanBotState
    {
        Running,
        Repositioning,
        Attacking,
        Spinning
    }


}
