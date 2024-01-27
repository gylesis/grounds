using System;
using System.Collections.Generic;
using Dev.Scripts.Infrastructure;
using DG.Tweening;
using Fusion;
using UniRx;
using UnityEngine;
using Zenject;

namespace Dev.Scripts.PlayerLogic
{
    public class PlayerView : NetworkContext
    {
        [SerializeField] private Transform _armsTransform;
        [SerializeField] private Transform _bodyTransform;

        [SerializeField] private NetworkMecanimAnimator _animator;

        [SerializeField] private List<Renderer> _meshRenderers;
        
        private static readonly int CrackStrengthName = Shader.PropertyToID("_CrackStrength");
        private static readonly int EmissionStrengthName = Shader.PropertyToID("_EmissionStrength");
        
        private static readonly int MoveX = Animator.StringToHash("MoveX");
        private static readonly int MoveY = Animator.StringToHash("MoveY");
        private static readonly int LeftHand = Animator.StringToHash("LeftHand");
        private static readonly int RightHand = Animator.StringToHash("RightHand");
        private static readonly int CenterHand = Animator.StringToHash("CenterHand");
        
        private PlayerCharacter _playerCharacter;
        private static readonly int Death = Animator.StringToHash("Death");


        [Inject]
        private void Construct(PlayerCharacter playerCharacter)
        {
            _playerCharacter = playerCharacter;
        }

        protected override void OnDependenciesResolve()
        {
            base.OnDependenciesResolve();
            
            TrySubscribeToHealth(_playerCharacter.Health);
        }

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

        private void TrySubscribeToHealth(Health health)
        {
            health.Changed.TakeUntilDestroy(_playerCharacter).Subscribe(UpdateView);
        }

        private void UpdateView(HealthChangedContext changedContext)
        {
            float currentHealth = changedContext.CurrentHealth;
            float maxHealth = changedContext.MaxHealth;

            float ratio = 1 - (currentHealth / maxHealth);
            float rescaledRatio = ratio * 2;
            float aaaaa = rescaledRatio * rescaledRatio;
            float crackStrength = rescaledRatio;
            
            Debug.Log($"{currentHealth}/{maxHealth}");

            foreach (var meshRenderer in _meshRenderers)
            {
                DOTween.Sequence()
                    .Append(meshRenderer.material.DOFloat(crackStrength * 2f, CrackStrengthName, 0.20f))
                    .Join(meshRenderer.material.DOFloat(3, EmissionStrengthName, 0.20f))
                    .Append(meshRenderer.material.DOFloat(crackStrength * 1f, CrackStrengthName, 0.80f))
                    .Join(meshRenderer.material.DOFloat(0, EmissionStrengthName, 0.20f));
            }
        }
        
        [Rpc]
        public void RPC_OnInput(Vector2 moveDirection)
        {
            _animator.Animator.SetFloat(MoveX, moveDirection.x);
            _animator.Animator.SetFloat(MoveY, moveDirection.y);
        }

        [Rpc]
        public void RPC_OnDeath()
        {
            _animator.Animator.SetTrigger(Death);
        }
        
    }
}