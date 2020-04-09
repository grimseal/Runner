using System;
using UnityEngine;

namespace Game.Level.ObjectsPool
{
    public class PoolObjectComponent : MonoBehaviour
    {

        /// <summary>
        /// Link to next pool object
        /// </summary>
        public PoolObjectComponent next { get; private set; }

        /// <summary>
        /// Object release callback
        /// </summary>
        private Func<PoolObjectComponent, PoolObjectComponent> returnToPool;

        private void OnDisable()
        {
            // when object become a not active in hierarchy
            // make it free for use in the pool
            if (returnToPool == null) return;
            next = returnToPool(this);
        }

        public static PoolObjectComponent AddComponentTo(GameObject target, PoolObjectComponent next,
            Func<PoolObjectComponent, PoolObjectComponent> returnCallback)
        {
            var component = target.AddComponent<PoolObjectComponent>();
            component.next = next;
            component.returnToPool = returnCallback;
            return component;
        }

        public void Start()
        {
            if (returnToPool == null)
                Debug.LogError("PoolObjectComponent is not set up correctly. " +
                               "Use PoolObjectComponent.AddComponentTo method");
        }
    }
}