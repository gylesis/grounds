using Dev.PlayerLogic;
using DG.Tweening;
using UnityEngine;
using Zenject;

namespace Dev.Scripts.PlayerLogic
{
    public class Hand : HandAbilities
    {
        [SerializeField] private HandType _handType;
        [SerializeField] private Transform _foreArmJoint;

        public HandType HandType => _handType;

        private Tween _activeTween;
        private Vector3 _initialArmRotation;

        private void Start()
        {
            _initialArmRotation = _foreArmJoint.localEulerAngles;
        }
        
        public override void PrepareToSwing()
        {
            _activeTween?.Kill();
            _activeTween = AnimatePrepare();
        }

        public override void Throw()
        {
            _activeTween?.Kill();
            _activeTween = AnimateThrow();
            RPC_LaunchItem();
        }

        public override Tween AnimatePrepare()
        {
            return _foreArmJoint.DOLocalRotate(_initialArmRotation - Vector3.right * 75, 0.5f);
        }

        public override Tween AnimateSwing()
        {
            return DOTween.Sequence()
                .Append(_foreArmJoint.DOLocalRotate(_initialArmRotation + Vector3.right * 15 + Vector3.forward * 70,
                    0.2f))
                .Append(_foreArmJoint.DOLocalRotate(_initialArmRotation, 0.5f));
        }

        public override Tween AnimateThrow()
        {
            return _foreArmJoint.DOLocalRotate(_initialArmRotation, 0.1f);
        }

        public bool TryGetFirearm(out Firearm firearmOut)
        {
            if (ContainingItem && ContainingItem.TryGetComponent(out Firearm firearm))
            {
                firearmOut = firearm;
                return true;
            }
            firearmOut = null;
            return false;
        }

    }
}