using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace TitanBot
{
    public static class PTMovementHandler
    {

    }

    public class Node
    {
        public Vector3 pos;
        public float radius;
        public Node[] connections;
    }

    public class MapData
    {
        public string mapName;
        public Node[] nodes;
    }
}
