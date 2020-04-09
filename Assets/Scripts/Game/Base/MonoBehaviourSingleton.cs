using UnityEngine;

namespace Game.Base
{
    [DisallowMultipleComponent]
    public class MonoBehaviourSingleton<T> : MonoBehaviour where T : Component
    {
        public static T instance { get; private set; }
	
        protected virtual void Awake ()
        {
            if (instance == null) instance = this as T;
            else
            {
                Debug.LogError($"Multiple {typeof(T).Name} instances", gameObject);
                Destroy(this);
            }
        }

        protected void OnDestroy()
        {
            if (instance == this) instance = null;
        }
    }
}