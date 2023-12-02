using System;
using Dev.Infrastructure;
using Fusion;
using UnityEngine;

namespace Dev.Scripts.PlayerLogic
{
    public class PlayerView : NetworkContext
    {
        [SerializeField] private Transform _armsTransform;
        [SerializeField] private Transform _bodyTransform;

        [SerializeField] private NetworkMecanimAnimator _animator;
        
        private static readonly int MoveX = Animator.StringToHash("MoveX");
        private static readonly int MoveY = Animator.StringToHash("MoveY");
        private static readonly int LeftHand = Animator.StringToHash("LeftHand");
        private static readonly int RightHand = Animator.StringToHash("RightHand");
        private static readonly int CenterHand = Animator.StringToHash("CenterHand");


        public override void Spawned()
        {
            if (HasInputAuthority)
            {
                _bodyTransform.gameObject.SetActive(false);                
            }
            else
            {
                _bodyTransform.gameObject.SetActive(true);
                _armsTransform.gameObject.SetActive(false);
            }
        }

        public void OnItemPickup(HandType handType, bool isCarrying)
        {
            switch (handType)
            {
                case HandType.Left:
                    _animator.Animator.SetBool(LeftHand, isCarrying);
                    break;
                case HandType.Right:
                    _animator.Animator.SetBool(RightHand, isCarrying);
                    break;
                case HandType.Center:
                    _animator.Animator.SetBool(CenterHand, isCarrying);
                    break;
            }
        }
        
        
        [Rpc]
        public void RPC_OnInput(Vector2 moveDirection)
        {
            _animator.Animator.SetFloat(MoveX, moveDirection.x);
            _animator.Animator.SetFloat(MoveY, moveDirection.y);
        }
        
    }
}