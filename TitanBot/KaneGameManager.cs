using TitanBot.FlatUI5;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Utility;

namespace TitanBot
{
    public class KaneGameManager : MonoBehaviour
    {
        public static KaneGameManager instance;
        public static bool toggleQuickMenu = false;
        public static string GameVersionString = "PTBot 1.0 (Dev)";

        public static void Init()
        {
            instance = SingletonFactory.CreateSingleton(instance);
            QuickMenu.Init();
            FlatUI.Init();
        }

        public void OnGUI()
        {
            if (toggleQuickMenu)
            {
                QuickMenu.OnGUI();
            }
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Mouse3))
            {
                if (!toggleQuickMenu)
                {
                    toggleQuickMenu = true;
                    return;
                }
                toggleQuickMenu = false;


            }

        }
    }
}
