
using Game.Character;
using UnityEngine;

namespace Game.Level.Object
{
    public class Cell : LevelObject
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<CharacterControllerComponent>(out var character)) character.ObstacleCollideHandler();
        }
    }
}