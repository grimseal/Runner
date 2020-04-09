using UnityEngine;

namespace Game.Config
{
    [CreateAssetMenu(fileName = "New GameConfig", menuName = "Game Config", order = 51)]
    public class GameConfig : ScriptableObject
    {
        public int chunkGraphLength = 40;
        public int chunkGraphWidth = 3;

        public float startSpeed = 4;
        public float speedMultiplier = 0.01f;
        public float speedLimit = 10;

        public GameObject character;
    }
}
