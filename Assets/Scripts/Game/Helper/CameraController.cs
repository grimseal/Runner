using UnityEngine;

namespace Game.Helper
{
    public class CameraController : MonoBehaviour
    {
        private Vector3 startPosition;
        
        private void Awake()
        {
            startPosition = transform.position;
        }

        public void AttachTo(Transform target)
        {
            Reset();
            transform.SetParent(target);
        }
        
        public void Detach()
        {
            transform.SetParent(null);
        }

        public void Reset()
        {
            var tr = transform;
            tr.SetParent(null);
            tr.position = startPosition;
        }
    }
}