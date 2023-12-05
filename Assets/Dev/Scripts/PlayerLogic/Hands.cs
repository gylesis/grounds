using System.Linq;
using UnityEngine;

namespace Dev.Scripts.PlayerLogic
{
    public class Hands : ItemContainer
    {
        [SerializeField] private Hand[] _hands;

        private Hand _activeHand => GetHandByType(HandType.Right);
        public Hand ActiveHand => _activeHand;
        
        public new bool IsFree => _hands.All(hand => hand.IsFree);
        
        protected override void CorrectState()
        {
            base.CorrectState();

            foreach (Hand hand in _hands)
            {
                if (hand.IsFree == false)
                {
                    hand.ContainingItem.RPC_SetParentAndSetZero(hand.ItemMountPoint);
                    Debug.Log($"Set parent for {hand.ContainingItem.name}", hand.ContainingItem);
                }
            }
        }

        public Hand GetHandByType(HandType handType)
        {
            return _hands.First(hand => hand.HandType == handType);
        }

        public ItemContainer GetOccupiedHand()
        {
            var firstHand = _hands.FirstOrDefault(hand => hand.IsFree == false);
            if (firstHand == null && this.IsFree) return this;
            
            return firstHand;
        }

        public override void PutItem(Item item)
        {
            if (IsFree == false) return;
            
            base.PutItem(item);
        }


    }
}