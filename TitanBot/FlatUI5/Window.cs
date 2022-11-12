using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace TitanBot.FlatUI5
{
    public class Window
    {
        public Rect rect;
        public string title;
        public Texture2D insideTex;
        public bool minimize = false;
        public bool showWindow = false;
        public Rect ContentRect;
        public float MinimizedWidth;
        public bool isDragging = false;

        private Rect titleBarRect;
        private Rect titleBarDragRect;
        private Rect minimizeButtonRect;
        private Rect closeButtonRect;

        private float dragXOffset = 0f;
        private float dragYOffset = 0f;

        protected static GUIStyle titlebarStyle = new GUIStyle
        {
            border = new RectOffset(1, 1, 1, 1),
            alignment = TextAnchor.MiddleCenter,
            normal = { textColor = Color.white },
            fontSize = 18,
            fontStyle = FontStyle.Bold
        };

        public Window(Rect rect, string title, Texture2D insideTex)
        {
            this.rect = rect;
            this.title = title;
            this.insideTex = insideTex;
            MinimizedWidth = rect.width;
            UpdateRects();
        }

        public Window(Rect rect, string title)
        {
            this.rect = rect;
            this.title = title;
            this.insideTex = FlatUI.insideColorTex;
            MinimizedWidth = rect.width;
            UpdateRects();
        }

        private void UpdateRects()
        {
            if (minimize)
            {
                titleBarRect = new Rect(rect.x + rect.width - MinimizedWidth, rect.y, MinimizedWidth, 30f);
                titleBarDragRect = new Rect(rect.x + rect.width - MinimizedWidth, rect.y, MinimizedWidth - 60f, 30f);
            }
            else
            {
                titleBarRect = new Rect(rect.x, rect.y, rect.width, 30f);
                titleBarDragRect = new Rect(rect.x, rect.y, rect.width - 60f, 30f);
            }
            minimizeButtonRect = new Rect(rect.x + rect.width - 60f, rect.y, 30f, 30f);
            closeButtonRect = new Rect(rect.x + rect.width - 30f, rect.y, 30f, 30f);
            ContentRect = new Rect(rect.x, rect.y + 30f, rect.width, rect.height - 30f);
        }

        /// <summary>
        /// Returns true if the window contents should be visible
        /// meaning it is not minimized or closed
        /// </summary>
        /// <returns></returns>
        public bool ContentVisible()
        {
            if (showWindow)
            {
                if (minimize)
                {
                    return false;
                }
                return true;
            }
            return false;
        }

        public void OnGUI()
        {
            if (showWindow)
            {
                if (isDragging)
                {
                    UpdateRects();
                }
                if (!minimize)
                {
                    FlatUI.Box(rect, insideTex);
                }
                FlatUI.Box(titleBarRect, insideTex);
                GUI.Label(titleBarDragRect, title, titlebarStyle);
                if (FlatUI.Button(minimizeButtonRect, "-"))
                {
                    minimize = !minimize;
                    UpdateRects();
                }
                if (FlatUI.Button(closeButtonRect, "x"))
                {
                    showWindow = false;
                }
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    if (FlatUI.IsMouseInRect(titleBarDragRect))
                    {
                        isDragging = true;
                        dragXOffset = Input.mousePosition.x - this.rect.x;
                        dragYOffset = (float)Screen.height - Input.mousePosition.y - this.rect.y;
                    }
                }
                if (Input.GetKeyUp(KeyCode.Mouse0))
                {
                    isDragging = false;
                }
                if (this.isDragging)
                {
                    this.rect.x = Input.mousePosition.x - this.dragXOffset;
                    this.rect.y = (float)Screen.height - Input.mousePosition.y - this.dragYOffset;
                }
                drawWindowContents();
            }
        }

        public virtual void drawWindowContents()
        {

        }
    }
}
