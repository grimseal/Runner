using System.Collections.Generic;
using Game.Config;
using Game.Level.ObjectsPool;
using UnityEngine;

namespace Game.Level
{
    public class LevelObjectsPool
    {
        /// <summary>
        /// Dictionary where key is obstacle id and value is obstacle pools
        /// </summary>
        private readonly Dictionary<GameObject, Pool> pools;

        public LevelObjectsPool(LevelConfig levelConfig, int initialCountPerPool = 10, Transform container = null)
        {
            pools = new Dictionary<GameObject, Pool>();
            
            foreach (var config in levelConfig.items)
                if (!pools.ContainsKey(config.prefab))
                    pools.Add(config.prefab, new Pool(config.prefab, initialCountPerPool, container));
            
            var configs = levelConfig.allCellsConfigs;
            foreach (var config in configs)
                if (!pools.ContainsKey(config.prefab))
                    pools.Add(config.prefab, new Pool(config.prefab, initialCountPerPool, container));
        }

        /// <summary>
        /// Return instance of config prefab from pool 
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public GameObject CreateFor(CellConfig config)
        {
            return CreateFor(config.prefab);
        }

        /// <summary>
        /// Return instance of config prefab from pool 
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public GameObject CreateFor(ItemConfig config)
        {
            return CreateFor(config.prefab);
        }

        /// <summary>
        /// Return instance of prefab from pool 
        /// </summary>
        /// <param name="prefab"></param>
        /// <returns></returns>
        public GameObject CreateFor(GameObject prefab)
        {
            return pools[prefab].Create();
        }

        /// <summary>
        /// Return instance back to pool
        /// </summary>
        /// <param name="instance"></param>
        public void ReturnToPool(GameObject instance)
        {
            instance.SetActive(false);
        }

        public void Clear()
        {
            foreach (var pool in pools.Values) pool.Clear();
            pools.Clear();
        }
    }
}