using System.Linq;
using Dev.Infrastructure;
using UnityEngine;

namespace Dev.Scripts.PlayerLogic
{
    public class Hands : NetworkContext
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private Hand[] _hands;

        public bool IsFree => _hands.All(hand => hand.IsFree);
        
        protected override void CorrectState()
        {
            base.CorrectState();

            foreach (Hand hand in _hands)
            {
                if (hand.IsFree == false)
                {
                    hand.CarryingItem.RPC_SetParentAndSetZero(hand.Parent);
                    Debug.Log($"Set parent for {hand.CarryingItem.name}", hand.CarryingItem);
                }
            }
        }

        public Hand GetHandByType(HandType handType)
        {
            return _hands.First(hand => hand.HandType == handType);
        }

        public Hand GetOccupiedHand()
        {
            return _hands.FirstOrDefault(hand => hand.IsFree == false);
        }

        public void PutItem(Item item)
        {
            if (IsFree == false) return;
            GetHandByType(HandType.Center).PutItem(item);
        }
    }
}