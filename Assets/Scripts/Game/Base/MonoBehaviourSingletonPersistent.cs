using UnityEngine;

namespace Game.Base
{
    public class MonoBehaviourSingletonPersistent<T> : MonoBehaviourSingleton<T> where T : Component
    {
        protected override void Awake ()
        {
            if (instance == null) DontDestroyOnLoad (this);
            base.Awake();
        }
    }
}
