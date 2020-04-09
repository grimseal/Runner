using Game.Level.Object;
using UnityEngine;

namespace Game.Config
{
    [CreateAssetMenu(fileName = "New CoinConfig", menuName = "Coin Config", order = 51)]
    public class CoinConfig : ItemConfig
    {
        public int score;

        public override LevelObject SetupInstance(GameObject instance, Vector3 inChunkPosition, Vector3 chunkPosition)
        {
            var item = base.SetupInstance(instance, inChunkPosition, chunkPosition) as Coin;
            if (item != null) item.value = score;
            return item;
        }
    }
}