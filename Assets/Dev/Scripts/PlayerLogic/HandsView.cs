using System;
using Dev.Infrastructure;
using DG.Tweening;
using UnityEngine;

namespace Dev.Scripts.PlayerLogic
{
    public class HandsView : NetworkContext
    {
        [SerializeField] private Transform _rightForeArmJoint;
        [SerializeField] private Transform _leftForeArmJoint;

        private void AnimateSwing(HandType handType)
        {
            DOTween.Sequence()
                .Append(AnimateSwingStart(handType))
                .Append(AnimateSwingEnd(handType));
        }
        
        private Tween AnimateSwingStart(HandType handType)
        {
            switch (handType)
            {
                case HandType.Left:
                    return _leftForeArmJoint.DOLocalRotate(_leftForeArmJoint.localEulerAngles - Vector3.right * 75, 0.5f);
                case HandType.Right:
                    return _rightForeArmJoint.DOLocalRotate(_rightForeArmJoint.localEulerAngles - Vector3.right * 75, 0.5f);
                case HandType.Center:
                    return DOTween.Sequence()
                        .Append(_leftForeArmJoint.DOLocalRotate(_leftForeArmJoint.localEulerAngles - Vector3.right * 75, 0.5f))
                        .Append(_rightForeArmJoint.DOLocalRotate(_rightForeArmJoint.localEulerAngles - Vector3.right * 75, 0.5f));
                default:
                    throw new ArgumentOutOfRangeException(nameof(handType), handType, null);
            }
        }
        
        private Tween AnimateSwingEnd(HandType handType)
        {
            switch (handType)
            {
                case HandType.Left:
                    return _leftForeArmJoint.DOLocalRotate(_leftForeArmJoint.localEulerAngles + Vector3.right * 75, 0.5f);
                case HandType.Right:
                    return _rightForeArmJoint.DOLocalRotate(_rightForeArmJoint.localEulerAngles + Vector3.right * 75, 0.5f);
                case HandType.Center:
                    return DOTween.Sequence()
                        .Append(_leftForeArmJoint.DOLocalRotate(_leftForeArmJoint.localEulerAngles + Vector3.right * 75, 0.5f))
                        .Append(_rightForeArmJoint.DOLocalRotate(_rightForeArmJoint.localEulerAngles + Vector3.right * 75, 0.5f));
                default:
                    throw new ArgumentOutOfRangeException(nameof(handType), handType, null);
            }
        }
        
        
    }
}