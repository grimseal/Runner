using UnityEngine;
using UnityEngine.Events;

namespace Game.Character
{
    public class Wallet : MonoBehaviour
    {
        public int amount;

        public AmountEvent onAmountChange = new AmountEvent();

        public void AddValue(int val)
        {
            amount += val;
            onAmountChange.Invoke(amount);
        }


        public class AmountEvent : UnityEvent<int>
        {
            
        }

    }
}