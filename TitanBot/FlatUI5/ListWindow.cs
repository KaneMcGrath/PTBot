using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace TitanBot.FlatUI5
{
    public class ListWindow : Window
    {
        public float ItemHeight = 30f;
        public bool useScrollBar;

        private int ElementCount = 0;
        private float scrollPosition = 0f;
        private float scrollOffset = 0f;
        private float relativeY;
        private float relativeHeight;
        private float contentWidth;
        private bool dragging = false;

        private Rect Blank1;
        private Rect Blank2;
        private Rect SliderRect;
        private Rect SliderArea;

        public ListWindow(Rect rect, string title, Texture2D insideTex, float itemHeight, bool useScrollBar) : base(rect, title, insideTex)
        {
            this.ItemHeight = itemHeight;
            this.useScrollBar = useScrollBar;
            init();
        }
        public ListWindow(Rect rect, string title, Texture2D insideTex, bool useScrollBar) : base(rect, title, insideTex)
        {
            this.ItemHeight = 30f;
            this.useScrollBar = useScrollBar;
            init();
        }

        public ListWindow(Rect rect, string title, float itemHeight) : base(rect, title)
        {
            this.ItemHeight = itemHeight;
            useScrollBar = false;
            init();
        }
        public ListWindow(Rect rect, string title) : base(rect, title)
        {
            this.ItemHeight = 30f;
            useScrollBar = false;
            init();
        }
        private void init()
        {
            if (useScrollBar)
            {
                relativeHeight = rect.height - (ItemHeight * 2f);
                relativeY = 30f + ItemHeight;
                contentWidth = rect.width - 30f;
            }
            else
            {
                relativeHeight = rect.height - 30f;
                relativeY = 30f;
                contentWidth = rect.width;
            }
        }

        public bool IsVisible(int i)
        {
            if (!useScrollBar) return true;
            if (i > ElementCount)
            {
                ElementCount = i;
            }
            float num = (rect.y + 30f) - (((float)ElementCount + 2) * this.ItemHeight - (rect.height - 30f)) * this.GetValue() + (float)i * this.ItemHeight;
            return num <= (rect.y + 30f) + (rect.height - 30f) && num >= (rect.y + 30f);
        }

        /// <summary>
        /// After all elements have been drawn in the window, draw large boxes at the top and bottem so elements dont pop in and out of the window
        /// also draw the scroll bar
        /// </summary>
        public void DrawBlanks()
        {
            if (useScrollBar)
            {
                Blank1 = new Rect(rect.x + FlatUI.defaultOutlineThickness, rect.y + 30f, rect.width - (FlatUI.defaultOutlineThickness * 2), ItemHeight);
                Blank2 = new Rect(rect.x + FlatUI.defaultOutlineThickness, rect.y + rect.height - ItemHeight - FlatUI.defaultOutlineThickness, rect.width - (FlatUI.defaultOutlineThickness * 2), ItemHeight);
                SliderArea = new Rect(rect.x + rect.width - 30f, rect.y + 30f + ItemHeight, 30f, rect.height - 30f - (ItemHeight * 2f));
                SliderRect = new Rect(SliderArea.x, SliderArea.y + scrollPosition, 30f, 30f);
                GUI.DrawTexture(Blank1, insideTex);
                GUI.DrawTexture(Blank2, insideTex);
                FlatUI.Box(SliderArea);
                FlatUI.Box(SliderRect);
                GUI.Label(SliderRect, "=", titlebarStyle);
                if (Input.GetAxis("Mouse ScrollWheel") > 0f && FlatUI.IsMouseInRect(rect))
                {
                    float num = scrollPosition -= 5f;
                    if (num < 0f)
                    {
                        num = 0f;
                    }
                    if (num > SliderArea.height - 30f)
                    {
                        num = SliderArea.height - 30f;
                    }
                    scrollPosition = num;
                }
                else if (Input.GetAxis("Mouse ScrollWheel") < 0f && FlatUI.IsMouseInRect(SliderArea))
                {
                    float num2 = scrollPosition += 5f;
                    if (num2 < 0f)
                    {
                        num2 = 0f;
                    }
                    if (num2 > SliderArea.height - 30f)
                    {
                        num2 = SliderArea.height - 30f;
                    }
                    scrollPosition = num2;
                }
                if (Input.GetMouseButtonDown(0) && FlatUI.IsMouseInRect(SliderRect) && !dragging)
                {
                    dragging = true;
                    scrollOffset = Mousey() - SliderArea.y - scrollPosition;
                }
                else if (Input.GetMouseButtonDown(0) && !FlatUI.IsMouseInRect(SliderRect) && FlatUI.IsMouseInRect(SliderArea))
                {
                    float num3 = Mousey() - SliderArea.y - 15f;
                    if (num3 < 0f)
                    {
                        num3 = 0f;
                    }
                    if (num3 > SliderArea.height - 30f)
                    {
                        num3 = SliderArea.height - 30f;
                    }
                    scrollPosition = num3;
                }
                if (Input.GetMouseButtonUp(0))
                {
                    dragging = false;
                }
                if (dragging)
                {
                    float num4 = Mousey() - scrollOffset - SliderArea.y;
                    if (num4 < 0f)
                    {
                        num4 = 0f;
                    }
                    if (num4 > SliderArea.height - 30f)
                    {
                        num4 = SliderArea.height - 30f;
                    }
                    scrollPosition = num4;
                }
            }
        }

        //Returns a value from 0 to 1 based on the position of the slider from top to bottem
        private float GetValue()
        {
            return scrollPosition / (SliderArea.height - SliderRect.height);
        }
        private float Mousey()
        {
            return (float)Screen.height - Input.mousePosition.y;
        }
        public Rect IndexToRect(int i)
        {
            if (useScrollBar)
            {
                float num = (rect.y + 30f) - (((float)ElementCount + 2) * this.ItemHeight - (rect.height - 30f)) * this.GetValue() + (float)i * this.ItemHeight;
                if (num > (rect.y + 30f) + (rect.height - 30f - ItemHeight))
                {
                    num = (rect.y + 30f) + (rect.height - 30f - ItemHeight);
                }
                else if (num < (rect.y + 30f) - ItemHeight)
                {
                    num = (rect.y + 30f) - ItemHeight;
                }
                return new Rect(this.rect.x, num, this.rect.width - 30f, this.ItemHeight);
            }
            else
            {
                return new Rect(rect.x + 4f, rect.y + 30f + (float)(i * ItemHeight), contentWidth - 8f, ItemHeight);
            }
        }
        public Rect IndexToRect(int i, int divisions, int j)
        {
            float num = contentWidth - 8f;
            if (divisions < 1)
            {
                divisions = 1;
            }
            return new Rect(rect.x + 4f + num / (float)divisions * (float)j, rect.y + 30f + (float)i * ItemHeight, num / (float)divisions, ItemHeight);
        }
        public Rect IndexToRect(int i, int divisions, int j, int n)
        {
            float num = contentWidth - 8f;
            if (divisions < 1)
            {
                divisions = 1;
            }
            return new Rect(rect.x + 4f + num / (float)divisions * (float)j, rect.y + 30f + (float)i * ItemHeight, (float)n * (num / (float)divisions), ItemHeight);
        }
        public Rect IndexToRect(int i, int divisions, int j, int n, float h)
        {
            float num = contentWidth - 8f;
            if (divisions < 1)
            {
                divisions = 1;
            }
            return new Rect(rect.x + 4f + num / (float)divisions * (float)j, rect.y + 30f + (float)i * ItemHeight, (float)n * (num / (float)divisions), h);
        }

    }
}
