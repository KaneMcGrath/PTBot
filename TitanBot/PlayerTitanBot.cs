using Settings;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;

namespace TitanBot
{
    /// <summary>
    /// Replacement for TITAN_CONTROLLER that tries to replicate PT gameplay
    /// </summary>
    public class PlayerTitanBot : TITAN_CONTROLLER
    {
        public static bool TakeOverPT = true;
        public static int raycasts = 10;
        public static float spinrate = 0.05f;
        public static bool debugRaycasts = false;
        public static bool debugTargets = false;
        public static bool useCustomHair = true;

        public bool doStuff = false;
        public TITAN MyTitan = null;
        private Dictionary<PTAction, bool> updateNextFrameList = new Dictionary<PTAction, bool>();
        private GameObject[] players;
        private GameObject closestPlayer;
        private TitanState state = TitanState.Running;

        // public float forsight = 4f;//how far into the future titan will predict player velocity
        // public int forsightSteps = 2;//how many steps not including the current position that the titan will predict

        //calculated moveset data for this titan to use
        private Dictionary<PTAction, HitData.MovesetData> MovesetDatabase = new Dictionary<PTAction, HitData.MovesetData>();

        //All the actions we want to calculate movesetData for
        private readonly PTAction[] pTActions = { 
            PTAction.Attack,
            PTAction.Jump,
            //PTAction.bitel,
            //PTAction.biter,
            //PTAction.bite,
            PTAction.choptl,
            PTAction.choptr
        };

        private List<PTAction> usableActions = new List<PTAction>();

        public void DebugGUI()
        {
            Vector3 point = Camera.main.WorldToScreenPoint(MyTitan.transform.position);
            Rect Bounds = new Rect(point.x, Screen.height - point.y, 200f, 500f);
            GUI.Label(Bounds, "");
        }

        private void teleportIfAtSpawn()
        {
            Vector3 spawnPos = new Vector3(0f, 0f, -530f);
            Vector3 returnPos = new Vector3(0f, 0f, 530f);
            if (Vector3.Distance(MyTitan.transform.position, spawnPos) < 200f)
            {
                MyTitan.transform.position = returnPos;
            }
        }

        private float stateTimer = 0f;

        /// <summary>
        /// Main AI Decision making function
        /// </summary>
        private void AIMaster()
        {
            players = GetPlayersToCalculate();
            if (debugRaycasts)
            {
                showRaycasts();
            }
            teleportIfAtSpawn();

            if (CGTools.timer(ref stateTimer, 5f))
            {
                int s = UnityEngine.Random.Range(0, 3);
                
                if (s == 0)
                {
                    state = TitanState.Repositioning;
                    //CGTools.log(state.ToString());
                    stateTimer = Time.time + 2f;
                }
                if (s == 1)
                {
                    state = TitanState.Running;
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
                    stateTimer = Time.time + 3f;
                }
            }


            if (state == TitanState.Running)
            {
                run();
                letErRip();
            }
            else if(state == TitanState.Attacking)
            {
                bool flag = false;
                if (closestPlayer != null)
                {
                    if (Vector3.Distance(closestPlayer.transform.position, MyTitan.neck.position) < 100f)
                    {


                        if (Vector3.Distance(closestPlayer.transform.position, MyTitan.neck.position) < 30f)
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


            float wobbleRate = 30f;
            float wobble = Mathf.Sin(Time.time * 4f) * wobbleRate;
            float targetDirectionFinal = targetDirectionLerp + wobble;
            targetLerpT += spinrate * Time.deltaTime;
            targetDirection = Mathf.Lerp(targetDirection, targetDirectionFinal, targetLerpT);
        }

        private void runToTarget(Vector3 target)
        {
            Vector3 direction = target - MyTitan.transform.position;
            Quaternion rotation = Quaternion.LookRotation(direction);
            targetDirection = rotation.eulerAngles.y;
        }

        private void Agro()
        {
            Vector3 d = closestPlayer.transform.position - MyTitan.transform.position;
            Quaternion r = Quaternion.LookRotation(d);
            if (debugTargets)
                CGTools.greenPointsToTrack.Add(closestPlayer.transform.position);
            targetDirectionLerp = r.eulerAngles.y;
        }
        
        private void reposition()
        {
            Vector3 d = closestPlayer.transform.position - MyTitan.transform.position;
            Quaternion r = Quaternion.LookRotation(d);
            if (debugTargets)
                CGTools.redPointsToTrack.Add(closestPlayer.transform.position);
            targetDirectionLerp = Quaternion.Inverse(r).eulerAngles.y;
        }

        private float targetDirectionLerp = 0f;
        private float changeTargetDirectionTimer = 0f;
        private float RaycastDirectionChangeTimer = 0f;
        private float targetLerpT = 0f;

        private float SpeedTimer = 0f;

        private Vector3 lastFarthestPoint = Vector3.zero;

        

        private void run()
        {
            //every half second there is a 10% chance that we walk for that period
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

            //Raycast in 8 directions and only choose directions that dont have walls
            //If a wall is found while running, immediatly choose a new direction
            Vector3 rayOrigin = MyTitan.transform.position + Vector3.up * 5f;
            Vector3 rayDirection = MyTitan.transform.forward * 50f;
            Ray ray = new Ray(rayOrigin, rayDirection);
            //CGTools.greenPointsToTrack.Add(MyTitan.transform.position + Quaternion.Euler(new Vector3(0f,targetDirectionLerp,0f)) * MyTitan.transform.forward * 50f);
            if (Physics.Raycast(ray, out RaycastHit raycastHit, 50f))
            {
                
                if (CGTools.timer(ref RaycastDirectionChangeTimer, 1f))
                {

                    PickNewDirection();





                    //targetDirectionLerp = UnityEngine.Random.Range(0f, 359f);
                    //targetLerpT = 0f;
                    //changeTargetDirectionTimer = Time.time + 2f;

                }

            }
            else
            {
                RaycastDirectionChangeTimer = 0f;
            }
            if (debugRaycasts)
                CGTools.pointsToTrack.Add(raycastHit.point);

            
            if (targetDirection > 360f)
            {
                targetDirection = 0f;
            }
            if (targetDirection < 0f)
            {
                targetDirection = 360f;
            }
            
            if (CGTools.timer(ref changeTargetDirectionTimer, 2f))
            {
                PickNewDirection();
            }
            if (debugRaycasts)
                CGTools.redPointsToTrack.Add(lastFarthestPoint);
            
            
        }

        

        private void PickNewDirection()
        {
            //raycast 360 degrees starting at a random rotation and find the point that goes the farthest
            int start = (int)UnityEngine.Random.Range(0f, 45f);
            float lastFarthestDistance = 0f;
            

            Vector3 rayOrigin2 = MyTitan.transform.position + Vector3.up * 10f;
            for (int i = 0; i < raycasts; i++)
            {

                Vector3 rayDirection2 = Quaternion.Euler(new Vector3(0f, i * (360 / raycasts), 0f)) * MyTitan.transform.forward;
                Ray ray2 = new Ray(rayOrigin2, rayDirection2);
                if (Physics.Raycast(ray2, out RaycastHit raycastHit2))
                {
                    if (raycastHit2.distance > lastFarthestDistance)
                    {
                        lastFarthestDistance = raycastHit2.distance;
                        lastFarthestPoint = raycastHit2.point;
                    }

                }
            }


            Vector3 direction = lastFarthestPoint - MyTitan.transform.position;
            Quaternion rotation = Quaternion.LookRotation(direction);
            targetDirectionLerp = rotation.eulerAngles.y;
            targetLerpT = 0f;
        }

        private void showRaycasts()
        {
            //raycast 360 degrees starting at a random rotation and find the point that goes the farthest
            //int start = (int)UnityEngine.Random.Range(0f, 90f);
            //int start = (int)UnityEngine.Random.Range(0f, 20f);
            float lastFarthestDistance = 0f;

            Vector3 rayOrigin2 = MyTitan.transform.position + Vector3.up * 10f;
            for (int i = 0; i < raycasts; i++)
            {
                //float angle = CGTools.Wrap(start + i * 18f, 0f, 360f);
                Vector3 rayDirection2 = Quaternion.Euler(new Vector3(0f, i * (360 / raycasts), 0f)) * MyTitan.transform.forward;
                Ray ray2 = new Ray(rayOrigin2, rayDirection2);
                if (Physics.Raycast(ray2, out RaycastHit raycastHit2))
                {
                    if (raycastHit2.distance > lastFarthestDistance)
                    {
                        lastFarthestDistance = raycastHit2.distance;
                        lastFarthestPoint = raycastHit2.point;
                    }

                }
                CGTools.pointsToTrack.Add(raycastHit2.point);
                CGTools.redPointsToTrack.Add(lastFarthestPoint);
            }
        }

        private void spin()
        {
            float spinrate = 600f;
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
                targetDirection = -874f;
                choptl = true;
            }
            if (a == PTAction.choptr)
            {
                targetDirection = -874f;
                choptr = true;
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

        

        /// <summary>
        /// finds the closest movesetData from the database
        /// if its not an exact match attempt to scale it
        /// </summary>
        /// <param name="titanLevel"></param>
        public void CalculateMovesetData(float titanLevel)
        {
            foreach (PTAction action in pTActions)
            {
                HitData.MovesetData data = HitData.GetClosestData(action, titanLevel);
                if (data != null)
                    MovesetDatabase.Add(action, data);
                else
                {
                    CGTools.log("CalculateMovesetData(float titanLevel) > Null Data for PTAction [" + action.ToString() + "]");
                }
            }
        }

        private void Update()
        {
            if (MyTitan == null) return;
            AIMaster();
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
                    Vector3 pos1 = PTTools.predictPlayerMotion(g, 1f);
                    Vector3 pos2 = PTTools.predictPlayerMotion(g, 2f);
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

    public enum Difficulty
    {
        easy,
        medium,
        Hard,
        VeryHard,
        VeryVeryHard
    }
}
