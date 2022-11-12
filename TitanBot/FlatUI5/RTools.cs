using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace TitanBot.FlatUI5
{
    public static class RTools
    {
        public static Rect SplitH(Rect source, int divisions, int j, int n = 1)
        {
            return new Rect(source.x + (source.width / (float)divisions * (float)j), source.y, (float)n * (source.width / (float)divisions), source.height);
        }
        public static Rect SplitV(Rect source, int divisions, int j, int n = 1)
        {
            return new Rect(source.x, source.y + (source.height / (float)divisions * (float)j), source.width, (float)n * (source.height / (float)divisions));
        }
    }
}
