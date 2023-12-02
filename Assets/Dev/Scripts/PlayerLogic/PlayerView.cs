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

        [Rpc]
        public void RPC_OnInput(Vector2 moveDirection)
        {
            _animator.Animator.SetFloat(MoveX, moveDirection.x);
            _animator.Animator.SetFloat(MoveY, moveDirection.y);
        }
        
    }
}