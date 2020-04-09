using System.Collections.Generic;
using UnityEngine;

namespace Game.Level.ObjectsPool
{
    public class Pool
    {
        private readonly List<PoolObjectComponent> objects;
        private readonly GameObject prefab;
        private readonly Transform container;
        
        private PoolObjectComponent firstAvailable;

        public Pool(GameObject prefab, int initialCount = 0, Transform container = null)
        {
            this.prefab = prefab;
            this.container = container;
            objects = new List<PoolObjectComponent>();
            for (var i = 0; i < initialCount; i++) InstantiateAndAddToPool();
        }

        /// <summary>
        /// Get first available or create new object
        /// </summary>
        /// <returns>GameObject instance</returns>
        public GameObject Create()
        {
            // if no one object in the PoolObjectComponent linked list or reach the end of the list
            // instantiate and add that new instance to linked list
            if (firstAvailable.next == null) InstantiateAndAddToPool();

            // return current first available object from list
            // and set next object as first available
            var obj = firstAvailable;
            firstAvailable = obj.next;
            return obj.gameObject;
        }

        public void Clear()
        {
            firstAvailable = null;
            foreach (var poolObject in objects) UnityEngine.Object.Destroy(poolObject.gameObject);
            objects.Clear();
        }

        private GameObject InstantiateAndAddToPool()
        {
            var go = UnityEngine.Object.Instantiate(prefab);
            if (container != null) go.transform.SetParent(container);
            var poolObject = PoolObjectComponent.AddComponentTo(go, firstAvailable, ReturnToPool);
            go.SetActive(false);
            objects.Add(poolObject);
            return go;
        }

        private PoolObjectComponent ReturnToPool(PoolObjectComponent component)
        {
            var next = firstAvailable;
            firstAvailable = component;
            return next;
        }
    }
}