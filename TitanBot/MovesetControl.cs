using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using TitanBot;

namespace StinkMod
{
    public static class MovesetControl
    {
        //store all TitanMoves here, this will be referenced for all titanBots when they throw an attack
        public static Dictionary<PTAction, TitanMove> movesetControlDatabase;

        public static void Init()
        {
            movesetControlDatabase = new Dictionary<PTAction, TitanMove>();
            SetDefaults();
        }



        public static void SetDefaults()
        {
            movesetControlDatabase.Clear();
            movesetControlDatabase.Add(PTAction.Attack, new TitanMove("Punch Attack", "combo_1", false, 0f, -1f, 0f));
            CGTools.log("Added: Attack");
            movesetControlDatabase.Add(PTAction.AttackII, new TitanMove("Slam Attack", "abnormal_jump", false, 0f, -1f, 0f));
            CGTools.log("Added: AttackII");
            movesetControlDatabase.Add(PTAction.Jump, new TitanMove("Jump", "jumper_0", false, 0f, -1f, 0f));
            CGTools.log("Added: Jump");
            movesetControlDatabase.Add(PTAction.bite, new TitanMove("Bite", "bite", false, 0f, -1f, 0f));
            CGTools.log("Added: bite");
            movesetControlDatabase.Add(PTAction.bitel, new TitanMove("Bite Left", "bite_l", false, 0f, -1f, 0f));
            CGTools.log("Added: bitel");
            movesetControlDatabase.Add(PTAction.biter, new TitanMove("Bite Right", "bite_r", false, 0f, -1f, 0f));
            CGTools.log("Added: biter");
            movesetControlDatabase.Add(PTAction.chopl, new TitanMove("Slap Low Left", "anti_AE_low_l", false, 0f, -1f, 0f));
            CGTools.log("Added: chopl");
            movesetControlDatabase.Add(PTAction.chopr, new TitanMove("Slap Low Right", "anti_AE_low_r", false, 0f, -1f, 0f));
            CGTools.log("Added: chopr");
            movesetControlDatabase.Add(PTAction.choptl, new TitanMove("Slap Left", "anti_AE_l", false, 0f, -1f, 0f));
            CGTools.log("Added: choptl");
            movesetControlDatabase.Add(PTAction.choptr, new TitanMove("Slap Right", "anti_AE_r", false, 0f, -1f, 0f));
            CGTools.log("Added: choptr");
            movesetControlDatabase.Add(PTAction.grabbackl, new TitanMove("Grab Back Left", "ground_back_l", true, 0f, -1f, 0f));
            CGTools.log("Added: grabbackl");
            movesetControlDatabase.Add(PTAction.grabbackr, new TitanMove("Grab Back Right", "ground_back_r", true, 0f, -1f, 0f));
            CGTools.log("Added: grabbackr");
            movesetControlDatabase.Add(PTAction.grabfrontl, new TitanMove("Grab Front Left", "ground_front_l", true, 0f, -1f, 0f));
            CGTools.log("Added: grabfrontl");
            movesetControlDatabase.Add(PTAction.grabfrontr, new TitanMove("Grab Front Right", "ground_front_r", true, 0f, -1f, 0f));
            CGTools.log("Added: grabfrontr");
            movesetControlDatabase.Add(PTAction.grabnapel, new TitanMove("Grab Nape Left", "head_back_l", true, 0f, -1f, 0f));
            CGTools.log("Added: grabnapel");
            movesetControlDatabase.Add(PTAction.grabnaper, new TitanMove("Grab Nape Right", "head_back_r", true, 0f, -1f, 0f));
            CGTools.log("Added: grabnaper");
            //new Moves                
            movesetControlDatabase.Add(PTAction.combo_2, new TitanMove("Punch 2", "combo_2", false, 0f, -1f, 0f));
            CGTools.log("Added: combo_2");
            movesetControlDatabase.Add(PTAction.combo_3, new TitanMove("Punch Ground", "combo_3", false, 0f, -1f, 0f));
            CGTools.log("Added: combo_3");
            movesetControlDatabase.Add(PTAction.front_ground, new TitanMove("Slow Punch Ground", "front_ground", false, 0f, -1f, 0f));
            CGTools.log("Added: front_ground");
            movesetControlDatabase.Add(PTAction.kick, new TitanMove("Kick", "kick", false, 0f, -1f, 0f));
            CGTools.log("Added: kick");
            movesetControlDatabase.Add(PTAction.slap_back, new TitanMove("Slap Nape", "slap_back", false, 0f, -1f, 0f));
            CGTools.log("Added: slap_back");
            movesetControlDatabase.Add(PTAction.slap_face, new TitanMove("Slap Face", "slap_face", false, 0f, -1f, 0f));
            CGTools.log("Added: slap_face");
            movesetControlDatabase.Add(PTAction.stomp, new TitanMove("Stomp", "stomp", false, 0f, -1f, 0f));
            CGTools.log("Added: stomp");
            movesetControlDatabase.Add(PTAction.crawler_jump_0, new TitanMove("Crawler Jump", "crawler_jump_0", false, 0f, -1f, 0f));
            CGTools.log("Added: crawler_jump_0");

        }
    }

    public class TitanMove
    {
        public string Name;
        public string AttackName;
        public bool isGrab;
        public float startAnimationAt;
        public float endAnimationAt;
        public float attackEndDelay;

        public TitanMove(string name, string attackName, bool isGrab, float startAnimationAt, float endAnimationAt, float attackEndDelay)
        {
            Name = name;
            AttackName = attackName;
            this.isGrab = isGrab;
            this.startAnimationAt = startAnimationAt;
            this.endAnimationAt = endAnimationAt;
            this.attackEndDelay = attackEndDelay;
        }
    }
}

