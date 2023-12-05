using DG.Tweening;
using UnityEngine;

namespace Dev.Scripts.PlayerLogic
{
    public class Hand : ItemContainer
    {
        [SerializeField] private HandType _handType;
        [SerializeField] private Transform _foreArmJoint;

        public HandType HandType => _handType;

        private Tween _activeTween;

        public void PrepareToSwing()
        {
            _activeTween?.Complete();
            _activeTween = _foreArmJoint.DOLocalRotate(_foreArmJoint.localEulerAngles - Vector3.right * 75, 0.5f);
        }

        public void Swing()
        {
            _activeTween?.Complete();
            _activeTween = _foreArmJoint.DOLocalRotate(_foreArmJoint.localEulerAngles + Vector3.right * 75, 0.5f);
            //Spawn Damage Area
        }

        public void Throw()
        {
            _activeTween?.Complete();
            _activeTween = _foreArmJoint.DOLocalRotate(_foreArmJoint.localEulerAngles + Vector3.right * 75, 0.1f);
            LaunchItem();
        }
    }
}