using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace TitanBot
{
    public static class PTMovementHandler
    {
        public static Dictionary<string, MapData> MapDatabase = new Dictionary<string, MapData>();

        public static bool isMapDataAvailible()
        {
            if (MapDatabase.ContainsKey(FengGameManagerMKII.level))
            {
                return true;
            }
            return false;
        }
    }

    public static class MapBuilder
    {
        public static void Init()
        {

        }

        public static void OnGUI()
        {

        }
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

    public class Path
    {
        public Vector3[] points;
    }
}
