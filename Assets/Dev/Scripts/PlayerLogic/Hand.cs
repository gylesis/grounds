using System;
using Dev.Infrastructure;
using DG.Tweening;
using UnityEngine;

namespace Dev.Scripts.PlayerLogic
{
    public class Hand : ItemContainer, IHandAbilities
    {
        [SerializeField] private HandType _handType;
        [SerializeField] private Transform _foreArmJoint;

        private DamageAreaSpawner _damageAreaSpawner;
        
        public HandType HandType => _handType;

        private Tween _activeTween;

        
        private void Start()
        {
           // _damageAreaSpawner = DependenciesContainer.Instance.GetDependency<DamageAreaSpawner>();
        }
        
        public void PrepareToSwing()
        {
            _activeTween?.Complete();
            _activeTween = AnimatePrepare();
        }

        public void Swing()
        {
            _activeTween?.Complete();
            _activeTween = AnimateSwing();
            // _damageAreaSpawner.Spawn();
        }

        public void Throw()
        {
            _activeTween?.Complete();
            _activeTween = AnimateThrow();
            RPC_LaunchItem();
        }

        public Tween AnimatePrepare()
        {
            return _foreArmJoint.DOLocalRotate(_foreArmJoint.localEulerAngles - Vector3.right * 75, 0.5f);
        }

        public Tween AnimateSwing()
        {
            return _foreArmJoint.DOLocalRotate(_foreArmJoint.localEulerAngles + Vector3.right * 75, 0.5f);
        }

        public Tween AnimateThrow()
        {
            return _foreArmJoint.DOLocalRotate(_foreArmJoint.localEulerAngles + Vector3.right * 75, 0.1f);
        }
    }
}