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
        
        public bool doStuff = false;
        public TITAN MyTitan = null;
        private Dictionary<PTAction, bool> updateNextFrameList = new Dictionary<PTAction, bool>();
        
        public float forsight = 4f;//how far into the future titan will predict player velocity
        public int forsightSteps = 2;//how many steps not including the current position that the titan will predict

        //calculated moveset data for this titan to use
        private Dictionary<PTAction, FloatingFire.MovesetData> MovesetDatabase = new Dictionary<PTAction, FloatingFire.MovesetData>();

        //All the actions we want to calculate movesetData for
        private readonly PTAction[] pTActions = { 
            PTAction.Attack,
            PTAction.Jump,
            PTAction.bitel,
            PTAction.biter,
            PTAction.bite,
            PTAction.choptl,
            PTAction.choptr
        };

        /// <summary>
        /// Main AI Decision making function
        /// </summary>
        private void TheBalrog()
        {
            letErRip();
        }

        private void letErRip()
        {
            GameObject[] players = GetPlayersToCalculate();
            if (players.Length == 0)
            {
                return;
            }
            PTAction NextAction = CheckAttacks(players);
            if (NextAction != PTAction.nothing)
            {
                ExecuteAction(NextAction);
            }
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
                choptl = true;
            }
            if (a == PTAction.choptr)
            {
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
                FloatingFire.MovesetData data = FloatingFire.GetClosestData(action, titanLevel);
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
            TheBalrog();
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
                    for (int i = 0; i < forsightSteps + 1; i++)
                    {
                        //float f = FloatingFire.checkMoveset(MovesetDatabase[action], PTTools.predictPlayerMotion(p, (forsight / forsightSteps) * i), (forsight / forsightSteps) * i);
                        float f = FloatingFire.CatchingSmoke(MovesetDatabase[action], p, (forsight / forsightSteps) * i);
                        if (f < lowestTime)
                        {
                            lowestTime = f;
                            result = action;
                        }
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
            List<GameObject> list = new List<GameObject>();
            foreach (GameObject g in CGTools.GetHeros())
            {
                if (Vector3.Distance(g.transform.position, MyTitan.transform.position) < 200f)
                {
                    list.Add(g);
                }
                else
                {
                    Vector3 pos1 = PTTools.predictPlayerMotion(g, 0.5f);
                    Vector3 pos2 = PTTools.predictPlayerMotion(g, 1f);
                    if (Vector3.Distance(pos1, MyTitan.transform.position) < 200f || Vector3.Distance(pos1, MyTitan.transform.position) < 200f)
                    {
                        list.Add(g);
                    }
                }
            }
            if (list.Count > 0)
            {
                return list.ToArray();
            }
            else
            {
                return new GameObject[0];
            }
        }
    }
}
