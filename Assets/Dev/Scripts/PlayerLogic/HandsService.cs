using System.Linq;
using Dev.Infrastructure;
using UnityEngine;

namespace Dev.Scripts.PlayerLogic
{
    public class HandsService : NetworkContext
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private HandInfoContainer[] _handInfoContainers;
        
        protected override void CorrectState()
        {
            base.CorrectState();

            foreach (HandInfoContainer hand in _handInfoContainers)
            {
                if (hand.IsFreeHand == false)
                {
                    hand.CarryingItem.RPC_SetParentAndSetZero(hand.Parent);
                    Debug.Log($"Set parent for {hand.CarryingItem.name}", hand.CarryingItem);
                }
            }
        }

        private HandInfoContainer GetHandInfoByHandType(HandType handType)
        {
            return _handInfoContainers.First(x => x.HandType == handType);
        }
        
        public void PutItemInHand(HandType handType, Item item)
        {
            HandInfoContainer handInfoContainer = GetHandInfoByHandType(handType);
            handInfoContainer.CarryingItem = item;
            handInfoContainer.CarryingItem.RPC_OnPickup(true);
            item.RPC_SetParent(handInfoContainer.Parent);
            item.RPC_SetLocalPos(Vector3.zero);
            item.RPC_SetLocalRotation(Vector3.zero);
        }

        public bool HasItemInHand(HandType handType)
        {
            return GetHandInfoByHandType(handType).IsFreeHand == false;
        }
        
        public bool AbleToPutItem(HandType handType) 
        {
            if (handType == HandType.Center)
            {
                HandInfoContainer leftHand = GetHandInfoByHandType(HandType.Left);
                HandInfoContainer rightHand = GetHandInfoByHandType(HandType.Right);

                return leftHand.IsFreeHand && rightHand.IsFreeHand;
            }
            
            return GetHandInfoByHandType(HandType.Center).IsFreeHand && GetHandInfoByHandType(handType).IsFreeHand;
        }

        public void DropItemFromHand(HandType handType)
        {
            HandInfoContainer handInfoContainer = GetHandInfoByHandType(handType);
            Item item = handInfoContainer.CarryingItem;

            item.RPC_SetParent(null);
            
            item.RPC_SetPos(item.transform.position + Vector3.ProjectOnPlane(_camera.transform.forward, Vector3.up) * 1.5f);
            
            item.RPC_OnPickup(false);


            handInfoContainer.CarryingItem = null;
        }
        
    }
}