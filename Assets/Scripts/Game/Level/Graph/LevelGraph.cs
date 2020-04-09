using System;
using Game.Base;
using Random = UnityEngine.Random;

namespace Game.Level.Graph
{
    /// <summary>
    /// Stores level chunk paths graph
    /// </summary>
    public class LevelGraph
    {
        public readonly int width;
        public readonly int length;
        public readonly Grid<LevelGraphNode> nodes;

        private LevelGraph(Grid<LevelGraphNode> nodes)
        {
            width = nodes.width;
            length = nodes.height;
            this.nodes = nodes;
        }

        public const int CellsPerNode = 4;
        public const int MinLength = 2;
        public const int MinWidth = 1;

        /// <summary>
        /// Generate new random level chunk graph
        /// </summary>
        /// <param name="width"></param>
        /// <param name="length"></param>
        /// <param name="prevGraph"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static LevelGraph Generate(int width, int length, LevelGraph prevGraph = null)
        {
            if (length < MinLength) throw new Exception("Length must be >= 2");
            if (width < MinWidth) throw new Exception("Width must be >= 1");
            if (prevGraph != null && prevGraph.width != width) throw new Exception("Prev graph width not equal");

            var nodes = new Grid<LevelGraphNode>(width, length);
            
            // todo handle prevGraph
            
            // Fill graph by connected nodes
            for (var y = 0; y < length; y++)
            for (var x = 0; x < width; x++)
                nodes.Set(x, y, new LevelGraphNode(x, y, width));

            // Close one random node on each grid row
            for (var y = 1; y < length; y++)
                CloseNode(Random.Range(0, width), y, nodes);

            return new LevelGraph(nodes);
        }

        private static void CloseNode(int x, int y, Grid<LevelGraphNode> nodes)
        {
            var node = nodes.Get(x, y);
            foreach (var position in node.enters)
                if (nodes.TryGet(position, out var neighbour))
                    neighbour.exits.Remove(node.position);
            node.enters.Clear();
            foreach (var position in node.exits)
                if (nodes.TryGet(position, out var neighbour))
                    neighbour.enters.Remove(node.position);
            node.exits.Clear();
            node.passable = false;
        }
    }
}