using System.Collections.Generic;
using UnityEngine;

namespace Game.Level.Graph
{
    public class LevelGraphNode
    {
        public Vector2Int position;

        public bool passable;

        public readonly List<Vector2Int> exits;
        
        public readonly List<Vector2Int> enters;
        
        public LevelGraphNode(int x, int y)
        {
            position = new Vector2Int(x, y);
            enters = new List<Vector2Int>();
            exits = new List<Vector2Int>();
        }

        public LevelGraphNode(int x, int y, int width)
        {
            position = new Vector2Int(x, y);
            enters = new List<Vector2Int>();
            exits = new List<Vector2Int>();
            passable = true;
            for (var i = x - 1; i <= x + 1; i++)
            {
                if (i < 0 || i >= width) continue; 
                enters.Add(new Vector2Int(i, y - 1));
                exits.Add(new Vector2Int(i, y + 1));
            }
        }
    }
}