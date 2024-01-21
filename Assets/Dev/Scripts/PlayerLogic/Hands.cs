using System.Collections.Generic;
using System.Linq;
using Dev.Infrastructure;
using Dev.Scripts.Items;
using Dev.Scripts.PlayerLogic.InventoryLogic;
using DG.Tweening;
using Fusion;
using Sirenix.Utilities;
using UnityEngine;
using Zenject;

namespace Dev.Scripts.PlayerLogic
{
    
    public class Hands : HandAbilities, IInputListener
    {
        [SerializeField] private Hand[] _hands;

        public Hand[] HandsList => _hands;
    
        private Hand _activeHand;
        private GameInventory _gameInventory;

        public HandAbilities ActiveHand => GetActiveHand();

        //Там подразумевалось, что в случае, если предмет ложится в обе руки, каждая будет иметь ссылку на этот предмет, но это пока не так, что может вызвать некоторые логические конфликты
        private bool AllHandsFree => _hands.All(hand => hand.IsFree);

        private List<HandAbilities> _allHands = new List<HandAbilities>(3);

        [Inject]
        private void Construct(GameInventory gameInventory, ItemsDataService itemsDataService)
        {
            _gameInventory = gameInventory;
            _itemsDataService = itemsDataService;
        }

        protected override void Start()
        {
            base.Start();
            _activeHand = GetHandByType(HandType.Right);
            
            _allHands.Add(this);
            _allHands.AddRange(_hands);
        }

        public bool IsAnyHandBusy()
        {
            return _allHands.Any(x => x.IsFree == false);
        }

        public bool HasThisItemInHands(int itemId)
        {
            foreach (var hand in _allHands)
            {
                if (hand.IsFree == false)
                {
                    if (hand.ContainingItem.ItemId == itemId)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public HandAbilities GetHandWithThisItemType(ItemType itemType)
        {
            foreach (var hand in _allHands)
            {
                if (hand.IsFree == false)
                {
                    if (_itemsDataService.GetItemStaticData(hand.ContainingItem.ItemId).ItemTypes.Contains(itemType))
                    {
                        return hand;    
                    }
                }
            }

            return null;
        }

        protected override void OnDependenciesResolve()
        {
            if (HasStateAuthority)
            {
                _gameInventory.RPC_OnPlayerSpawned(Object.InputAuthority);
            }
        }

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

        public bool IsHandFree(HandType handType)
        {
            return _hands.First(x => x.HandType == handType).IsFree;
        }

        public HandAbilities GetOccupiedHand()
        {
            var firstHand = _hands.FirstOrDefault(hand => hand.IsFree == false);
            if (firstHand == null && this.AllHandsFree) return this;

            return firstHand;
        }

        public override void RPC_PutItem(Item item)
        {
            if (AllHandsFree == false) return;

            base.RPC_PutItem(item);
        }

        private HandAbilities GetActiveHand()
        {
            if (this.IsFree is false) return this;

            return _activeHand;
        }

        public override void PrepareToSwing()
        {
            var sequence = DOTween.Sequence();
            _hands.ForEach(hand => sequence.Join(hand.AnimatePrepare()));
        }

        public override void Swing()
        {
            var sequence = DOTween.Sequence();
            _hands.ForEach(hand => sequence.Join(hand.AnimateSwing()));
        }

        public override void Throw()
        {
            var sequence = DOTween.Sequence();
            _hands.ForEach(hand => sequence.Join(hand.AnimateThrow()));
            RPC_LaunchItem();
        }

        public void ToggleActiveHand()
        {
            if (_activeHand == null || _activeHand.HandType == HandType.Left)
                _activeHand = GetHandByType(HandType.Right);
            else if (_activeHand.HandType == HandType.Right) _activeHand = GetHandByType(HandType.Left);
        }

        private void RequestToPutItemInInventory(HandType handType)
        {
            Hand leftHand = GetHandByType(handType);

            PlayerRef playerRef = Object.InputAuthority;
            Item item = leftHand.ContainingItem;

            var itemData = new ItemData(item.ItemId);

            RPC_PutItemInInventory(itemData, playerRef);

            leftHand.RPC_DropItem();
            
            _itemsDataService.RPC_RemoveItemFromWorld(item);
        }

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        private void RPC_PutItemInInventory(ItemData itemData, PlayerRef playerRef)
        {
            //itemData.ItemName = leftHand.ContainingItem.TestName;
            _gameInventory.PutItemInInventory(itemData, playerRef);
        }

        public void OnInput(PlayerInput input, NetworkButtons wasPressed, NetworkButtons wasReleased)
        {
            if (Runner.IsResimulation) return;

            if (wasPressed.IsSet(Buttons.PutItemToInventory))
            {
                Hand leftHand = GetHandByType(HandType.Left);
                Hand rightHand = GetHandByType(HandType.Right);
    
                if (leftHand.IsFree == false)
                {
                    RequestToPutItemInInventory(HandType.Left);
                }
                else if (rightHand.IsFree == false)
                {
                    RequestToPutItemInInventory(HandType.Right);
                }
            }

            if (wasPressed.IsSet(Buttons.UseItem))
            {
                ActiveHand.UseItem();
            }
            if (wasPressed.IsSet(Buttons.Swing))
            {
                ActiveHand.PrepareToSwing();
            }
            else if (wasReleased.IsSet(Buttons.Swing) && !wasReleased.IsSet(Buttons.Throw))
            {
                ActiveHand.Swing();
            }
            else if (wasReleased.IsSet(Buttons.Throw))
            {
                ActiveHand.Throw();
            }

            if ((wasPressed.IsSet(Buttons.AlternateHand) || wasReleased.IsSet(Buttons.AlternateHand))
                && !input.Buttons.IsSet(Buttons.Swing))
            {
                ToggleActiveHand();
            }

            if (wasPressed.IsSet(Buttons.DropItem))
            {
                if (AllHandsFree == false)
                {
                    GetOccupiedHand()?.RPC_DropItem();
                }
                else if (!input.Buttons.IsSet(Buttons.AlternateHand))
                {
                    GetHandByType(HandType.Right).RPC_DropItem();
                }
                else if (input.Buttons.IsSet(Buttons.AlternateHand))
                {
                    GetHandByType(HandType.Left).RPC_DropItem();
                }
            }

            if (wasPressed.IsSet(Buttons.ReloadWeapon))
            {
                _hands.ForEach(hand =>
                {
                    if (hand.TryGetFirearm(out var firearm))
                    {
                        var oppositeHand = _hands.First(otherHand => otherHand != hand);
                        bool reloadSuccessful = firearm.ReloadWith(oppositeHand.ContainingItem);
                        if (reloadSuccessful)
                        {
                            oppositeHand.RPC_SetEmpty();
                        }
                    }
                });
            }
        }
    }
}