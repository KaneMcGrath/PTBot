using TitanBot.FlatUI5;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace TitanBot.Windows
{
    public static class CGDebugConsole
    {
        public static bool UseCGDebugConsole = true;

        public static Window ChatWindow = new Window(new Rect(100f, 50f, 1000f, 1000f), "Console") { MinimizedWidth = 200f };
        public static ScrollList MessageScrollBar = new ScrollList(50f) { snapBottom = true };
        public static ScrollList RPCScrollBar = new ScrollList(30f);
        public static List<ConsoleMessage> Messages = new List<ConsoleMessage>();
        private static Rect ChatRect = new Rect(ChatWindow.ContentRect.x, ChatWindow.ContentRect.y, ChatWindow.ContentRect.width, ChatWindow.ContentRect.height - 30f);
        private static Rect chatInputBox = new Rect(ChatWindow.ContentRect.x, ChatWindow.ContentRect.y + ChatWindow.ContentRect.height - 30f, ChatWindow.ContentRect.width, 30f);
        private static int TabIndex = 0;
        private static string consoleInput = "";
        private static ConsoleMessage messageToView = ConsoleMessage.None;
        private static string[] tabNames = new string[] {
            "Console",
            "Detailed View"
        };

        public static int MaxConsoleMessages = 1000;
        private static Texture2D[] LogTypeColors; //Error, Assert, Warning, Log, Exception;


        public static void Init()
        {
            ChatWindow.insideTex = QuickMenu.tabColors[0];
            MessageScrollBar.insideTex = QuickMenu.tabColors[0];
            LogTypeColors = new Texture2D[]
            {
                CGTools.ColorTex(new Color(0.35f,0.02f,0.01f)),   //Error
                CGTools.ColorTex(new Color(0.54f,0.12f,0.46f)),   //Assert
                CGTools.ColorTex(new Color(0.35f,0.02f,0.01f)),   //Warning
                CGTools.ColorTex(new Color(0.0f,0.0f,0.0f)),      //Log
                CGTools.ColorTex(new Color(0.75f,0.28f,0.0f))     //Exception
            };
        }

        public static void DebugKeyPressed()
        {
            if (!ChatWindow.showWindow || ChatWindow.minimize)
            {
                ChatWindow.showWindow = true;
                ChatWindow.minimize = false;
            }
            else
            {
                ChatWindow.showWindow = false;
            }
        }

        public static void OnDebugMessage(string stackTrace, string log, LogType logType)
        {
            if (Messages.Count > MaxConsoleMessages)
            {
                Messages.RemoveAt(0);
            }
            Messages.Add(new ConsoleMessage(Time.realtimeSinceStartup, stackTrace, log, logType));
        }

        public static void OnGUI()
        {
            if (ChatWindow.isDragging)
            {
                ChatRect = new Rect(ChatWindow.ContentRect.x, ChatWindow.ContentRect.y, ChatWindow.ContentRect.width, ChatWindow.ContentRect.height - 30f);
                chatInputBox = new Rect(ChatWindow.ContentRect.x, ChatWindow.ContentRect.y + ChatWindow.ContentRect.height - 30f, ChatWindow.ContentRect.width, 30f);
            }
            ChatWindow.OnGUI();
            if (ChatWindow.showWindow && !ChatWindow.minimize)
            {
                TabIndex = FlatUI.tabs(new Rect(ChatWindow.ContentRect.x + ChatWindow.ContentRect.width, ChatWindow.ContentRect.y, 100f, 200f), tabNames, TabIndex, false, QuickMenu.tabColors[0]);
                if (TabIndex == 0)
                {
                    if (Messages.Count > 0)
                    {
                        for (int i = 0; i < Messages.Count; i++)
                        {
                            drawMessage(i, Messages[i]);
                        }
                    }
                    MessageScrollBar.DrawBlanks(ChatRect);

                    //FlatUI.Box(chatInputBox);
                    //GUI.SetNextControlName("StinkconsoleInput");
                    //
                    //
                    //if (FlatUI.Button(new Rect(ChatWindow.ContentRect.x + ChatWindow.ContentRect.width - 200f, ChatWindow.ContentRect.y + ChatWindow.ContentRect.height - 42f, 200f, 34f), "Send"))
                    //{
                    //    sendDebugCommand(consoleInput);
                    //    consoleInput = "";
                    //}
                    //if (Event.current.type == EventType.KeyUp && Event.current.keyCode == KeyCode.Return && GUI.GetNameOfFocusedControl().Equals("StinkconsoleInput"))
                    //{
                    //    string resultMessage = consoleInput;
                    //    sendDebugCommand(resultMessage);
                    //    consoleInput = "";
                    //}
                }
                else if (TabIndex == 1)
                {
                    Rect TopInfoBar = new Rect(ChatRect.x, ChatRect.y, ChatRect.width, 30f);
                    Rect TimeLabelSection = new Rect(ChatRect.x, ChatRect.y, 100f, 30f);
                    Rect TimeContentSection = new Rect(ChatRect.x + 100f, ChatRect.y, 200f, 30f);
                    Rect TypeLabelSection = new Rect(ChatRect.x + 300f, ChatRect.y, 100f, 30f);
                    Rect TypeContentSection = new Rect(ChatRect.x + 400f, ChatRect.y, 200f, 30f);
                    Rect StackTraceLabelSection = new Rect(ChatRect.x, ChatRect.y + 30f, ChatRect.width / 2f, 30f);
                    Rect LogContentLabelSection = new Rect(ChatRect.x + ChatRect.width / 2f, ChatRect.y + 30f, ChatRect.width / 2f, 30f);
                    Rect MainContentSection = new Rect(ChatRect.x, ChatRect.y + 60f, ChatRect.width, ChatRect.height - 30f);
                    Rect StackTraceLefthand = new Rect(MainContentSection.x, MainContentSection.y, MainContentSection.width / 2f, MainContentSection.height);
                    Rect LogContentRighthand = new Rect(MainContentSection.x + MainContentSection.width / 2f, MainContentSection.y, MainContentSection.width / 2f, MainContentSection.height);

                    int LogTypeIndex = 0;
                    if (messageToView.LogType == LogType.Error) LogTypeIndex = 0;
                    if (messageToView.LogType == LogType.Assert) LogTypeIndex = 1;
                    if (messageToView.LogType == LogType.Warning) LogTypeIndex = 2;
                    if (messageToView.LogType == LogType.Log) LogTypeIndex = 3;
                    if (messageToView.LogType == LogType.Exception) LogTypeIndex = 4;

                    FlatUI.Box(TimeLabelSection);
                    FlatUI.Label(TimeLabelSection, "Time: ", LabelTextStyle);
                    FlatUI.Box(TimeContentSection);
                    FlatUI.Label(TimeContentSection, messageToView.time.ToString(), BigChatTextStyle);
                    FlatUI.Box(TypeLabelSection);
                    FlatUI.Label(TypeLabelSection, "Type: ", LabelTextStyle);
                    FlatUI.Box(TypeContentSection, LogTypeColors[LogTypeIndex]);
                    FlatUI.Label(TypeContentSection, messageToView.LogType.ToString(), BigChatTextStyle);
                    FlatUI.Box(StackTraceLabelSection);
                    FlatUI.Label(StackTraceLabelSection, "Stack Trace", LabelTextStyle);
                    FlatUI.Box(StackTraceLefthand);
                    FlatUI.Label(StackTraceLefthand, messageToView.stackTrace, BigChatTextStyle);

                    FlatUI.Box(LogContentLabelSection);
                    FlatUI.Label(LogContentLabelSection, "Log", LabelTextStyle);
                    FlatUI.Box(LogContentRighthand);
                    FlatUI.Label(LogContentRighthand, messageToView.Log, BigChatTextStyle);

                }
            }
        }


        private static void drawMessage(int i, ConsoleMessage consoleMessage)
        {
            if (MessageScrollBar.IsVisible(i))
            {
                Rect messageRect = MessageScrollBar.IndexToRect(i);
                Rect TimeBox = new Rect(messageRect.x, messageRect.y, 100f, messageRect.height);
                Rect LogTypeBox = new Rect(messageRect.x + 100f, messageRect.y, 100f, messageRect.height);
                Rect StackTraceBox = new Rect(messageRect.x + 200f, messageRect.y, messageRect.width - 210f, messageRect.height / 2f);
                Rect LogBox = new Rect(messageRect.x + 200f, messageRect.y + messageRect.height / 2f, messageRect.width - 210f, messageRect.height / 2f);
                Rect MoreBox = new Rect(messageRect.x + messageRect.width - 50f, messageRect.y, 50f, messageRect.height);

                int LogTypeIndex = 0;
                if (consoleMessage.LogType == LogType.Error) LogTypeIndex = 0;
                if (consoleMessage.LogType == LogType.Assert) LogTypeIndex = 1;
                if (consoleMessage.LogType == LogType.Warning) LogTypeIndex = 2;
                if (consoleMessage.LogType == LogType.Log) LogTypeIndex = 3;
                if (consoleMessage.LogType == LogType.Exception) LogTypeIndex = 4;

                FlatUI.Box(TimeBox, LogTypeColors[LogTypeIndex]);
                TimeBox.x += 4f;
                FlatUI.Label(TimeBox, consoleMessage.time.ToString(), LabelTextStyle);

                FlatUI.Box(LogTypeBox, LogTypeColors[LogTypeIndex]);
                LogTypeBox.x += 4f;
                FlatUI.Label(LogTypeBox, consoleMessage.LogType.ToString(), LabelTextStyle);

                FlatUI.Box(StackTraceBox, LogTypeColors[LogTypeIndex]);
                StackTraceBox.x += 4f;
                FlatUI.Label(StackTraceBox, consoleMessage.stackTrace, ChatTextStyle);

                FlatUI.Box(LogBox, LogTypeColors[LogTypeIndex]);
                LogBox.x += 4f;
                FlatUI.Label(LogBox, consoleMessage.Log, ChatTextStyle);

                if (FlatUI.Button(MoreBox, "View"))
                {
                    TabIndex = 1;
                    messageToView = consoleMessage;
                }
            }
        }

        private static void sendDebugCommand(string message)
        {

        }


        public struct ConsoleMessage
        {
            public static int cutoffLength = 170;

            public float time;
            public string stackTrace;
            public string Log;
            public LogType LogType;

            public ConsoleMessage(float time, string stackTrace, string log, LogType logType)
            {
                this.time = time;
                this.stackTrace = stackTrace;
                Log = log;
                LogType = logType;
            }

            public static ConsoleMessage None = new ConsoleMessage(0f, "", "", LogType.Log);
        }

        public static GUIStyle LabelTextStyle = new GUIStyle
        {
            border = new RectOffset(1, 1, 1, 1),
            alignment = TextAnchor.MiddleCenter,
            fontSize = 16,
            fontStyle = FontStyle.Normal,
            clipping = TextClipping.Clip,
            normal = { textColor = Color.white }
        };

        public static GUIStyle ChatTextStyle = new GUIStyle
        {
            border = new RectOffset(1, 1, 1, 1),
            alignment = TextAnchor.MiddleLeft,
            fontSize = 14,
            fontStyle = FontStyle.Normal,
            clipping = TextClipping.Clip,
            normal = { textColor = Color.white }
        };

        public static GUIStyle BigChatTextStyle = new GUIStyle
        {
            border = new RectOffset(1, 1, 1, 1),
            alignment = TextAnchor.UpperLeft,
            fontSize = 14,
            fontStyle = FontStyle.Normal,
            clipping = TextClipping.Overflow,
            wordWrap = true,
            normal = { textColor = Color.white }
        };

        public static GUIStyle BigChatTextNoStyle = new GUIStyle
        {
            border = new RectOffset(1, 1, 1, 1),
            alignment = TextAnchor.UpperLeft,
            fontSize = 14,
            fontStyle = FontStyle.Normal,
            clipping = TextClipping.Overflow,
            wordWrap = true,
            richText = false,
            normal = { textColor = Color.white }
        };
    }
}
