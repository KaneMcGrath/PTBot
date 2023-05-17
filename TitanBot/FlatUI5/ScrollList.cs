using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace TitanBot.FlatUI5
{
    public class ScrollList
    {
        public float ItemHeight = 30f;
        public Texture2D insideTex;

        /// <summary>
        /// when set to true, the scroll bar will snap to the bottom on the next blank
        /// and then reset itself to false
        /// you can initialize this to true to start at the bottom
        /// </summary>
        public bool snapBottom = false;

        private Rect rect;
        private int ElementCount = 0;
        private float scrollPosition = 0f;
        private float scrollOffset = 0f;
        private bool dragging = false;

        private Rect Blank1;
        private Rect Blank2;
        private Rect SliderRect;
        private Rect SliderArea;

        private static GUIStyle scrollTextStyle = new GUIStyle
        {
            border = new RectOffset(1, 1, 1, 1),
            alignment = TextAnchor.MiddleCenter,
            normal = { textColor = Color.white },
            fontSize = 18,
            fontStyle = FontStyle.Bold
        };

        public ScrollList()
        {
            ItemHeight = 30f;
            insideTex = FlatUI.insideColorTex;
        }

        public ScrollList(float ItemHeight)
        {
            this.ItemHeight = ItemHeight;
            insideTex = FlatUI.insideColorTex;
        }

        public ScrollList(float ItemHeight, Texture2D insideTex)
        {
            this.ItemHeight = ItemHeight;
            this.insideTex = insideTex;
        }

        public bool IsVisible(int i)
        {
            if (i > ElementCount)
            {
                ElementCount = i;
            }
            float num = rect.y + ItemHeight - (((float)ElementCount + 3) * this.ItemHeight - rect.height) * this.GetValue() + (float)i * this.ItemHeight;
            return num <= rect.y + rect.height && num >= rect.y;
        }

        /// <summary>
        /// After all elements have been drawn in the window, draw large boxes at the top and bottem so elements dont pop in and out of the window
        /// also draw the scroll bar
        /// </summary>
        public void DrawBlanks(Rect rect, bool draw = true)
        {
            if (!draw) return;
            this.rect = rect;
            Blank1 = new Rect(rect.x + FlatUI.defaultOutlineThickness, rect.y, rect.width - (FlatUI.defaultOutlineThickness * 2), ItemHeight);
            Blank2 = new Rect(rect.x + FlatUI.defaultOutlineThickness, rect.y + rect.height - ItemHeight, rect.width - (FlatUI.defaultOutlineThickness * 2), ItemHeight);
            SliderArea = new Rect(rect.x + rect.width - 30f, rect.y + ItemHeight, 30f, rect.height - (ItemHeight * 2f));
            SliderRect = new Rect(SliderArea.x, SliderArea.y + scrollPosition, 30f, 30f);
            GUI.DrawTexture(Blank1, insideTex);
            GUI.DrawTexture(Blank2, insideTex);
            FlatUI.Box(SliderArea);
            FlatUI.Box(SliderRect);
            GUI.Label(SliderRect, "=", scrollTextStyle);
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
            else if (Input.GetAxis("Mouse ScrollWheel") < 0f && FlatUI.IsMouseInRect(rect))
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
            if (snapBottom)
            {
                scrollPosition = SliderArea.height - SliderRect.height;
                snapBottom = false;
            }
        }

        //Returns a value from 0 to 1 based on the position of the slider from top to bottom
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
            float yPos = rect.y + ItemHeight - (((float)ElementCount + 3) * this.ItemHeight - rect.height) * this.GetValue() + ((float)i) * this.ItemHeight;
            if (yPos > rect.y + (rect.height - ItemHeight))
            {
                yPos = rect.y + (rect.height - ItemHeight);
            }
            else if (yPos < rect.y - ItemHeight)
            {
                yPos = rect.y - ItemHeight;
            }
            return new Rect(rect.x, yPos, rect.width - 30f, ItemHeight);
        }
        public Rect IndexToRect(int i, int divisions, int j)
        {
            float yPos = rect.y + ItemHeight - (((float)ElementCount + 3) * this.ItemHeight - rect.height) * this.GetValue() + ((float)i) * this.ItemHeight;
            if (yPos > rect.y + (rect.height - ItemHeight))
            {
                yPos = rect.y + (rect.height - ItemHeight);
            }
            else if (yPos < rect.y - ItemHeight)
            {
                yPos = rect.y - ItemHeight;
            }
            float width = rect.width - 30f;
            if (divisions < 1)
            {
                divisions = 1;
            }
            return new Rect(rect.x + 4f + width / (float)divisions * (float)j, yPos, width / (float)divisions, ItemHeight);
        }
        public Rect IndexToRect(int i, int divisions, int j, int n)
        {
            float yPos = rect.y + ItemHeight - (((float)ElementCount + 3) * this.ItemHeight - rect.height) * this.GetValue() + ((float)i) * this.ItemHeight;
            if (yPos > rect.y + (rect.height - ItemHeight))
            {
                yPos = rect.y + (rect.height - ItemHeight);
            }
            else if (yPos < rect.y - ItemHeight)
            {
                yPos = rect.y - ItemHeight;
            }
            float width = rect.width - 30f;
            if (divisions < 1)
            {
                divisions = 1;
            }
            return new Rect(rect.x + 4f + width / (float)divisions * (float)j, yPos, (float)n * (width / (float)divisions), ItemHeight);
        }
        public Rect IndexToRect(int i, int divisions, int j, int n, float h)
        {
            float yPos = rect.y + ItemHeight - (((float)ElementCount + 3) * this.ItemHeight - rect.height) * this.GetValue() + ((float)i) * this.ItemHeight;
            if (yPos > rect.y + (rect.height - ItemHeight))
            {
                yPos = rect.y + (rect.height - ItemHeight);
            }
            else if (yPos < rect.y - ItemHeight)
            {
                yPos = rect.y - ItemHeight;
            }
            float width = rect.width - 30f;
            if (divisions < 1)
            {
                divisions = 1;
            }
            return new Rect(rect.x + 4f + width / (float)divisions * (float)j, yPos, (float)n * (width / (float)divisions), h);
        }

    }
}
