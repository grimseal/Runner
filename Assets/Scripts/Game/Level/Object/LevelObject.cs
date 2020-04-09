using UnityEngine;

namespace Game.Level.Object
{
    [RequireComponent(typeof(Collider)), DisallowMultipleComponent]
    public abstract class LevelObject : MonoBehaviour
    {
        private GameObject go;
        private Transform tr;
        private Vector3 inChunkPosition;

        public Vector3 position => tr.position;

        public void Awake()
        {
            go = gameObject;
            tr = transform;
        }

        public void SetActive(bool active)
        {
            go.SetActive(active);
        }

        public void SetupPosition(Vector3 inChunkPosition, Vector3 chunkPosition)
        {
            this.inChunkPosition = inChunkPosition;
            SetPosition(chunkPosition);
        }

        public void SetPosition(Vector3 chunkPosition)
        {
            tr.position = inChunkPosition + chunkPosition;
        }

    }
}
