using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Level
{
    /// <summary>
    /// Generate and scroll level
    /// </summary>
    public class LevelScroller
    {

        /// <summary>
        /// Chunk generator
        /// </summary>
        private ChunkGenerator generator;

        /// <summary>
        /// Current chunks of level objects
        /// </summary>
        private List<Chunk> chunks;
        
        public float chunkLength => generator.chunkLength;

        private GameObject borders;

        public LevelScroller(ChunkGenerator generator, Transform container = null)
        {
            this.generator = generator;
            CreateLevelBorders(container);
        }

        // Setup level generator
        // var objectsPool = new LevelObjectsPool(levelConfig, 2, container);
        // generator = new ChunkGenerator(levelConfig, objectsPool, config.chunkGraphLength, config.chunkGraphWidth);

        public void StartLevel()
        {
            // Generate first chunks
            var firstChunk = generator.GenerateChunk();
            chunks = new List<Chunk>
            {
                firstChunk,
                generator.GenerateChunk(firstChunk, firstChunk.length)
            };
        }

        /// <summary>
        /// Clear state 
        /// </summary>
        public void Destroy()
        {
            generator?.Destroy();
            generator = null;
            chunks = null;
            UnityEngine.Object.Destroy(borders);
        }

        public void Update(Vector3 targetPosition)
        {
            // Clear chunks
            var removedCount = 0;
            for (var i = chunks.Count - 1; i >= 0; i--)
            {
                chunks[i].ScrollHandler(targetPosition);
                if (!chunks[i].isEmpty) continue;
                chunks.RemoveAt(i);
                removedCount++;
            }

            // Generate next chunks instead of deleted
            var length = generator.chunkLength;
            for (var i = 0; i < removedCount; i++)
                chunks.Add(generator.GenerateChunk(chunks.Last(), chunks.Count * length));
        }

        public void OriginOffset(Vector3 offsetVector)
        {
            foreach (var chunk in chunks)
                chunk.SetPosition(chunk.worldPosition + offsetVector);
        }

        /// <summary>
        /// Add global level border colliders
        /// </summary>
        private void CreateLevelBorders(Transform container = null)
        {
            borders  = new GameObject();
            var boxCollider = borders.AddComponent<BoxCollider>();
            boxCollider.center = new Vector3(generator.chunkWidth / 2, -1, generator.chunkLength);
            boxCollider.size = new Vector3(generator.chunkWidth + 2, 1, generator.chunkLength * 2);
            
            boxCollider = borders.AddComponent<BoxCollider>();
            boxCollider.center = new Vector3(-1, 0, generator.chunkLength);
            boxCollider.size = new Vector3(1, 1, generator.chunkLength * 2);            
            
            boxCollider = borders.AddComponent<BoxCollider>();
            boxCollider.center = new Vector3(generator.chunkWidth, 0, generator.chunkLength);
            boxCollider.size = new Vector3(1, 1, generator.chunkLength * 2);
            
            if (container != null) boxCollider.transform.SetParent(container);
        }

        [Serializable]
        public class OriginShift : UnityEvent<Vector3> {}
    }
}