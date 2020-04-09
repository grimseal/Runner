using System.Collections.Generic;
using System.Linq;
using Game.Level.Graph;
using Game.Level.Object;
using UnityEngine;

namespace Game.Level
{
    public class Chunk
    {
        public readonly LevelGraph graph;

        public Vector3 worldPosition;
        
        public bool isEmpty => objects.Count < 1;
        
        public float length => graph.length * LevelGraph.CellsPerNode;

        private readonly List<LevelObject> objects;

        private const float CutDistance = -3; // todo remove from here

        public Chunk(LevelGraph graph, IEnumerable<LevelObject> chunkObjects, Vector3 worldPosition)
        {
            this.graph = graph;
            this.worldPosition = worldPosition;
            objects = chunkObjects.ToList();
            foreach (var levelObject in objects) levelObject.SetActive(true);
        }

        /// <summary>
        /// Remove chunk elements invisible for the camera (by character position)
        /// </summary>
        /// <param name="characterPosition"></param>
        public void ScrollHandler(Vector3 characterPosition)
        {
            for (var i = objects.Count - 1; i >= 0; i--)
                if ((objects[i].position - characterPosition).z < CutDistance)
                    RemoveObject(i);
        }

        /// <summary>
        /// Translate chunk position
        /// </summary>
        /// <param name="newWorldPosition"></param>
        public void SetPosition(Vector3 newWorldPosition)
        {
            worldPosition = newWorldPosition;
            foreach (var levelObject in objects) levelObject.SetPosition(worldPosition);
        }

        private void RemoveObject(int i)
        {
            objects[i].SetActive(false);
            objects.RemoveAt(i);
        }
    }
}