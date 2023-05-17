using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StinkMod;
using TitanBot.Windows;
using UnityEngine;

namespace TitanBot
{
    public static class MovesetProfile
    {
        public static List<string> profiles = new List<string>();

        private static string ProfilesPath = KaneGameManager.Path + "Profiles\\";
        private static float UpdateLevelsTimer = 0f; //so we dont accidently end up spamming IO


        /// <summary>
        /// Get all the text files in the profiles directory and add them to a list
        /// </summary>
        public static void ProfilesUpdateCheck()
        {
            if (!CGTools.timer(ref UpdateLevelsTimer, 5f)) return;
            UpdateProfiles();
        }

        public static void UpdateProfiles()
        {
            if (Directory.Exists(ProfilesPath))
            {
                profiles.Clear();
                string[] files = Directory.GetFiles(ProfilesPath);
                foreach (string file in files)
                {
                    string fileName = Path.GetFileName(file);
                    string name = fileName.Split('.')[0];
                    string ext = Path.GetExtension(file);
                    if (ext.ToLower() == ".txt")
                    {
                        profiles.Add(name);
                    }
                }
            }
            else
            {
                CGLog.log("The Profiles Directory does not exist.  Default profiles will be regenerated.");
                GenerateDefaultProfiles();
            }
        }

        public static void GenerateDefaultProfiles()
        {
            if (!Directory.Exists(ProfilesPath))
            {
                Directory.CreateDirectory(ProfilesPath);
            }
            if (!File.Exists(ProfilesPath + "Default.txt"))
            {
                File.WriteAllLines(ProfilesPath + "Default.txt", new string[]
                {
                    "Moves:Attack,AttackII,Jump,choptl,choptr",
                    "Timing:Attack>0,AttackII>0,Jump>0,bite>0,bitel>0,biter>0,chopl>0,chopr>0,choptl>0,choptr>0,grabbackl>0,grabbackr>0,grabfrontl>0,grabfrontr>0,grabnapel>0,grabnaper>0,combo_2>0,combo_3>0,front_ground>0,kick>0,slap_back>0,slap_face>0,stomp>0,crawler_jump_0>0,grab_head_front_l>0,grab_head_front_r>0"
                });
            }
        }


        // literally just copied the load config function from KaneGameManager.
        // I decided to not delete the other loading options incase anyone wants some more detailed profiles
        // they can copy their config.txt into the profiles directory and it will work
        // some hidden functionality
        public static void LoadProfile(string name)
        {
            string file = ProfilesPath + name + ".txt";
            if (File.Exists(file))
            {
                Dictionary<string, string> config = new Dictionary<string, string>();
                string[] lines = File.ReadAllLines(file);
                for (int i = 0; i < lines.Length; i++)
                {
                    string line = lines[i];

                    string[] parts = line.Split(':');
                    if (parts.Length == 2)
                    {
                        config.Add(parts[0], parts[1]);
                    }
                    else
                        CGTools.log("Error on line " + i + ".  Makes sure each setting is on a seperate line");
                }
                if (config.ContainsKey("SendJoinMessage"))
                {
                    if (bool.TryParse(config["SendJoinMessage"], out bool b))
                    {
                        KaneGameManager.sendJoinMessage = b;
                    }
                    else
                    {
                        CGTools.log("Could not parse Setting [SendJoinMessage]");
                    }
                }
                if (config.ContainsKey("InfiniteTitanCount"))
                {
                    if (int.TryParse(config["InfiniteTitanCount"], out int i))
                    {
                        QuickMenu.infinitePTBotTextBox = i.ToString();
                        KaneGameManager.InfPTBotCount = i;
                    }
                    else
                    {
                        CGTools.log("Could not parse Setting [InfiniteTitanCount]");
                    }
                }
                if (config.ContainsKey("InfiniteNormalTitanCount"))
                {
                    if (int.TryParse(config["InfiniteNormalTitanCount"], out int i))
                    {
                        QuickMenu.infiniteNormalTitanTextBox = i.ToString();
                        KaneGameManager.InfiniteTitansCount = i;
                    }
                    else
                    {
                        CGTools.log("Could not parse Setting [InfiniteNormalTitanCount]");
                    }
                }
                if (config.ContainsKey("ReplaceSpawnedTitans"))
                {
                    if (bool.TryParse(config["ReplaceSpawnedTitans"], out bool b))
                    {
                        PlayerTitanBot.ReplaceSpawnedTitans = b;
                    }
                    else
                    {
                        CGTools.log("Could not parse Setting [ReplaceSpawnedTitans]");
                    }
                }
                if (config.ContainsKey("doSpawnTeleporting"))
                {
                    if (bool.TryParse(config["doSpawnTeleporting"], out bool b))
                    {
                        KaneGameManager.doSpawnTeleporting = b;
                    }
                    else
                    {
                        CGTools.log("Could not parse Setting [doSpawnTeleporting]");
                    }
                }
                if (config.ContainsKey("Difficulty"))
                {
                    try
                    {
                        PTTools.difficulty = (Difficulty)Enum.Parse(typeof(Difficulty), config["Difficulty"]);
                    }
                    catch (Exception e)
                    {
                        CGTools.log("Could not parse Setting [Difficulty] : " + e.Message);
                    }
                }
                if (config.ContainsKey("Moves"))
                {
                    try
                    {
                        PlayerTitanBot.TempActionsList.Clear();
                        if (config["Moves"].IsNullOrEmpty())
                        {

                        }
                        else
                        {
                            string[] moves = config["Moves"].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                            foreach (string move in moves)
                            {
                                PlayerTitanBot.TempActionsList.Add((PTAction)Enum.Parse(typeof(PTAction), move));
                            }
                        }
                        PlayerTitanBot.pTActions = PlayerTitanBot.TempActionsList.ToArray();
                    }
                    catch (Exception e)
                    {
                        CGTools.log("Could not parse Setting [Moves] : " + e.Message);
                    }
                }
                if (config.ContainsKey("PruningLevel"))
                {
                    if (int.TryParse(config["PruningLevel"], out int i))
                    {
                        QuickMenu.prunningSettingTextbox = i.ToString();
                        PlayerTitanBot.dataPruningLevel = i;
                    }
                    else
                    {
                        CGTools.log("Could not parse Setting [PruningLevel]");
                    }
                }
                if (config.ContainsKey("TitanName"))
                {
                    PlayerTitanBot.TitanName = config["TitanName"];
                    QuickMenu.titanNameTextBox = config["TitanName"];
                }
                if (config.ContainsKey("Timing"))
                {
                    bool hasmoves = config.ContainsKey("Moves");
                    string[] individuals = config["Timing"].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string s in individuals)
                    {
                        string[] split = s.Split('>');
                        PTAction action = (PTAction)Enum.Parse(typeof(PTAction), split[0]);
                        float StartAt = 0f;
                        if (float.TryParse(split[1], out StartAt))
                        {
                            MovesetControl.movesetControlDatabase[action].startAnimationAt = StartAt;
                        }
                        else
                        {
                            CGLog.log("Could not parse Timing for attack: " + split[0]);
                        }
                    }
                }
                if (config.ContainsKey("SpeedEnabled"))
                {
                    if (bool.TryParse(config["SpeedEnabled"], out bool b))
                    {
                        PlayerTitanBot.useCustomSpeed = b;
                    }
                    else
                    {
                        CGTools.log("Could not parse Setting [SpeedEnabled]");
                    }
                }
                if (config.ContainsKey("Speed"))
                {
                    if (float.TryParse(config["Speed"], out float f))
                    {
                        QuickMenu.speedTextBox = f.ToString();
                        PlayerTitanBot.titanSpeed = f;
                    }
                    else
                    {
                        CGTools.log("Could not parse Setting [Speed]");
                    }
                }
                if (config.ContainsKey("AllowEyeHits"))
                {
                    if (bool.TryParse(config["AllowEyeHits"], out bool b))
                    {
                        PlayerTitanBot.AllowEyeHits = b;
                    }
                    else
                    {
                        CGTools.log("Could not parse Setting [AllowEyeHits]");
                    }
                }
                if (config.ContainsKey("AllowAnkleHits"))
                {
                    if (bool.TryParse(config["AllowAnkleHits"], out bool b))
                    {
                        PlayerTitanBot.AllowAnkleHits = b;
                    }
                    else
                    {
                        CGTools.log("Could not parse Setting [AllowAnkleHits]");
                    }
                }
                MovesetControlWindow.UpdateWindowData();
                CGTools.log("Loaded profile " + name + ".txt");
            }
            else
            {
                CGTools.log("The profile " + file + " does not exist!");
            }
        }

        public static void SaveProfile(string name)
        {
            Dictionary<string, string> config = new Dictionary<string, string>();
            string moves = "";
            if (PlayerTitanBot.pTActions.Length > 0)
            {
                for (int i = 0; i < PlayerTitanBot.pTActions.Length; i++)
                {
                    PTAction action = PlayerTitanBot.pTActions[i];
                    moves += action.ToString();
                    if (i != PlayerTitanBot.pTActions.Length - 1)
                        moves += ',';
                }
            }
            config.Add("Moves", moves);

            string moveTiming = "";
            PTAction[] actions = new PTAction[MovesetControl.movesetControlDatabase.Keys.Count];
            MovesetControl.movesetControlDatabase.Keys.CopyTo(actions, 0);
            for (int i = 0; i < actions.Length; i++)
            {
                TitanMove move = MovesetControl.movesetControlDatabase[actions[i]];
                moveTiming += actions[i].ToString() + ">" + move.startAnimationAt.ToString();
                if (i != actions.Length - 1)
                    moveTiming += ',';
            }
            config.Add("Timing", moveTiming);
            List<string> lines = new List<string>();
            foreach (string key in config.Keys)
            {
                lines.Add(key + ":" + config[key]);
            }
            try
            {
                File.WriteAllLines(ProfilesPath + name + ".txt", lines.ToArray());
                CGTools.log("Saved profile " + name + ".txt");
            }
            catch (Exception e)
            {
                CGTools.log(e.Message);
            }
            
        }
    }
}
