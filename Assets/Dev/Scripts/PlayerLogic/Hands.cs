using System.Linq;
using Dev.Infrastructure;
using DG.Tweening;
using Fusion;
using Sirenix.Utilities;
using UnityEngine;

namespace Dev.Scripts.PlayerLogic
{
    public class Hands : ItemContainer, IHandAbilities
    {
        [SerializeField] private Hand[] _hands;

        private Hand _activeHand;
        public IHandAbilities ActiveHand => GetActiveHand();

        //Там подразумевалось, что в случае, если предмет ложится в обе руки, каждая будет иметь ссылку на этот предмет, но это пока не так, что может вызвать некоторые логические конфликты
        private bool AllHandsFree => _hands.All(hand => hand.IsFree);


        private void Awake()
        {
            _activeHand = GetHandByType(HandType.Right);
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

        public ItemContainer GetOccupiedHand()
        {
            var firstHand = _hands.FirstOrDefault(hand => hand.IsFree == false);
            if (firstHand == null && this.AllHandsFree) return this;

            return firstHand;
        }

        public override void PutItem(Item item)
        {
            if (AllHandsFree == false) return;

            base.PutItem(item);
        }

        private IHandAbilities GetActiveHand()
        {
            if (this.IsFree is false) return this;

            return _activeHand;
        }

        public void PrepareToSwing()
        {
            var sequence = DOTween.Sequence();
            _hands.ForEach(hand => sequence.Join(hand.AnimatePrepare()));
        }

        public void Swing()
        {
            var sequence = DOTween.Sequence();
            _hands.ForEach(hand => sequence.Join(hand.AnimateSwing()));
        }

        public void Throw()
        {
            var sequence = DOTween.Sequence();
            _hands.ForEach(hand => sequence.Join(hand.AnimateThrow()));
            LaunchItem();
        }

        public void ToggleActiveHand()
        {
            if (_activeHand == null || _activeHand.HandType == HandType.Left)
                _activeHand = GetHandByType(HandType.Right);
            else if (_activeHand.HandType == HandType.Right) _activeHand = GetHandByType(HandType.Left);
        }

        public void OnInput(PlayerInput input, NetworkButtons wasPressed, NetworkButtons wasReleased)
        {
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

            if (wasPressed.IsSet(Buttons.AlternateHand) || wasReleased.IsSet(Buttons.AlternateHand))
            {
                ToggleActiveHand();
            }

            if (wasPressed.IsSet(Buttons.DropItem))
            {
                if (AllHandsFree == false)
                {
                    GetOccupiedHand()?.DropItem();
                }
                else if (!input.Buttons.IsSet(Buttons.AlternateHand))
                {
                    GetHandByType(HandType.Right).DropItem();
                }
                else if (input.Buttons.IsSet(Buttons.AlternateHand))
                {
                    GetHandByType(HandType.Left).DropItem();
                }
            }
        }
    }
}