using Game.Level.Object;
using UnityEngine;

namespace Game.Config
{
    public abstract class LevelObjectConfig : ScriptableObject
    {
        public GameObject prefab;
        [Range(0f, 1f)]
        public float generateChance;

        public bool CheckChance(float chance)
        {
            return generateChance >= chance;
        }

        public virtual LevelObject SetupInstance(GameObject instance, Vector3 inChunkPosition, Vector3 chunkPosition)
        {
            var component = instance.GetComponent<LevelObject>();
            component.SetupPosition(inChunkPosition, chunkPosition);
            return component;
        }
    }
}