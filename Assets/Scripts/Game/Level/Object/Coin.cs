using Game.Character;
using UnityEngine;

namespace Game.Level.Object
{
    public class Coin : Item
    {
        [HideInInspector]
        public int value;

        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent<Wallet>(out var wallet)) return;
            wallet.AddValue(value);
            transform.position += Vector3.down;
        }
    }
}