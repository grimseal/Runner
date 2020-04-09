using System.Collections.Generic;
using System.Linq;
using Game.Base;
using Game.Config;
using Game.Helper;
using Game.Level.Graph;
using Game.Level.Object;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Level
{
    /// <summary>
    /// Level chunk generator
    /// </summary>
    public class ChunkGenerator
    {
        private readonly LevelObjectsPool levelObjectsPool;
        private readonly LevelConfig levelConfig;
        private readonly int length;
        private readonly int width;

        public int chunkLength => length * LevelGraph.CellsPerNode;

        public int chunkWidth => width;

        public ChunkGenerator(LevelConfig config, LevelObjectsPool pool, int length, int width)
        {
            levelConfig = config;
            levelObjectsPool = pool;
            this.length = Mathf.Max(length, LevelGraph.MinLength);
            this.width = Mathf.Max(width, LevelGraph.MinWidth);
        }

        /// <summary>
        /// Chunk generate method
        /// </summary>
        /// <param name="prevChunk"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public Chunk GenerateChunk(Chunk prevChunk = null, float offset = 0)
        {
            var graph = LevelGraph.Generate(width, length, prevChunk?.graph);
            var offsetVector = new Vector3(0, 0, offset);
            var objects = GenerateCells(graph, levelConfig, levelObjectsPool, offsetVector);
            objects.AddRange(GenerateItems(graph, levelConfig, levelObjectsPool, offsetVector));
            return new Chunk(graph, objects, offsetVector);
        }
        
        
        /// <summary>
        /// Clear generator pool
        /// </summary>
        public void Destroy()
        {
            levelObjectsPool.Clear();
        }

        /// <summary>
        /// Generate chunk cells by graph nodes
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="levelConfig"></param>
        /// <param name="pool"></param>
        /// <param name="chunkPosition"></param>
        /// <returns></returns>
        private static List<LevelObject> GenerateCells(LevelGraph graph, LevelConfig levelConfig, LevelObjectsPool pool,
            Vector3 chunkPosition)
        {
            const int cellsPerNode = LevelGraph.CellsPerNode;
            var chunkLength = graph.length * cellsPerNode;
            var chunkWidth = graph.width;
            var obstaclePlaces = new List<Vector2Int>();

            // Fill grid by wall and floor random configs
            var cells = new Grid<CellConfig>(chunkWidth, chunkLength);
            for (var l = 0; l < graph.length; l++)
            for (var x = 0; x < graph.width; x++)
            {
                var node = graph.nodes.Get(x, l);
                var end = l * cellsPerNode + cellsPerNode;
                if (node.passable)
                {
                    var y = l * cellsPerNode;
                    var coordinates = new Vector2Int(x, y);
                    // Store passable coordinate for placing some obstacle
                    obstaclePlaces.Add(coordinates);
                    for (; y < end; y++) cells.Set(x, y, GetRandom(levelConfig.floors));
                }
                else
                {
                    var y = l * cellsPerNode;
                    cells.Set(x, y, levelConfig.walls[Random.Range(0, levelConfig.walls.Length)]);
                    y++;
                    if (graph.nodes.TryGet(x, l + 1, out var nextNode) && !nextNode.passable)
                        for (; y < end; y++) cells.Set(x, y, GetRandom(levelConfig.walls));
                    else
                        for (; y < end; y++) cells.Set(x, y,GetRandom(levelConfig.floors));
                }   
            }

            // Try to place random obstacle config in previously founded coordinates
            foreach (var coordinates in obstaclePlaces)
            {
                var chance = Random.value;
                var obstacleConfigs = Shuffle<CellConfig>.ToQueue(levelConfig.obstacles);
                while (obstacleConfigs.Count > 0)
                {
                    var config = obstacleConfigs.Dequeue();
                    // Check if obstacle placement restrictions passed
                    if (!config.CheckPlacement(chance, coordinates, cells)) continue;
                    cells.Set(coordinates, config);
                    break;
                }
            }
            
            // Setup instances
            var gameObjects = new List<LevelObject>(cells.items.Length);
            gameObjects.AddRange(cells.items.Select((t, i) => 
                t.SetupInstance(pool.CreateFor(t.prefab), 
                    ToVector3(cells.GetIndexCoordinates(i)), chunkPosition)));
            return gameObjects;
        }

        /// <summary>
        /// Generate chunk items by graph nodes
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="levelConfig"></param>
        /// <param name="pool"></param>
        /// <param name="chunkPosition"></param>
        /// <returns></returns>
        private static List<LevelObject> GenerateItems(LevelGraph graph, LevelConfig levelConfig, LevelObjectsPool pool,
            Vector3 chunkPosition)
        {
            const int itemsCount = 5;
            const int itemsCountOffset = 1;
            
            var levelObjects = new List<LevelObject>();
            var orderedItems = levelConfig.items.OrderBy(item => item.generateChance).ToArray();
            
            // Generate one items chain per graph row in random lane
            for (var y = 0; y < graph.length; y++)
            {
                var xShuffle = Shuffle<int>.ToQueue(Enumerable.Range(0, graph.width));
                while (xShuffle.Count > 0)
                {
                    var x = xShuffle.Dequeue();
                    var node = graph.nodes.Get(x, y);
                    if (!node.passable) continue;
                    var target = GetRandom(node.exits);
                    
                    // Build smooth items "path" by Bezier curve
                    var offset = new Vector3(0,0, 0.5f);
                    var a = new Vector3(node.position.x, 0, node.position.y * LevelGraph.CellsPerNode) + offset;
                    var d = new Vector3(target.x, 0,target.y * LevelGraph.CellsPerNode) + offset;
                    var middle = new Vector3(0, 0, d.z - a.z) * 0.5f;
                    var b = a + middle;
                    var c = d - middle;
                    var bezier = new Bezier(a, b, c, d);
                    
                    // Place items on curve in some range
                    var from = Random.Range(0, itemsCountOffset + 1);
                    var to = Random.Range(0, itemsCountOffset + 1) + itemsCount - itemsCountOffset;
                    for (var i = from; i < to; i++)
                    {
                        var chance = Random.value;
                        var inChunkPosition = bezier.GetPoint(1f / itemsCount * i);
                        foreach (var itemConfig in orderedItems)
                        {
                            if (!itemConfig.CheckChance(chance)) continue;
                            levelObjects.Add(itemConfig.SetupInstance(pool.CreateFor(itemConfig), 
                                inChunkPosition, chunkPosition));
                            break;
                        }
                    }
                    break;
                }
            }
            return levelObjects;
        }

        private static Vector3 ToVector3(Vector2Int coordinates)
        {
            return new Vector3(coordinates.x, 0, coordinates.y);
        }
        
        private static T GetRandom<T>(IReadOnlyList<T> configs)
        {
            return configs[Random.Range(0, configs.Count)];
        }
    }
}