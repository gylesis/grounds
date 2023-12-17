using Dev.Infrastructure;
using Dev.PlayerLogic;
using DG.Tweening;
using UnityEngine;

namespace Dev.Scripts.PlayerLogic
{
    public class Hand : ItemContainer, IHandAbilities
    {
        [SerializeField] private HandType _handType;
        [SerializeField] private Transform _foreArmJoint;
        [SerializeField] private ItemEnumeration _itemEnumeration;
        [SerializeField] private PlayerCharacter _player;
        

        private DamageAreaSpawner _damageAreaSpawner;
        
        public HandType HandType => _handType;

        private Tween _activeTween;
        private Vector3 _initialArmRotation;
        private Camera _camera;
        
        private void Start()
        {
            _camera = FindObjectOfType<Camera>();
            _initialArmRotation = _foreArmJoint.localEulerAngles;
            _damageAreaSpawner = DependenciesContainer.Instance.GetDependency<DamageAreaSpawner>();
        }
        
        public void PrepareToSwing()
        {
            _activeTween?.Kill();
            _activeTween = AnimatePrepare();
        }

        public void Swing()
        {
            _activeTween?.Kill();
            _activeTween = AnimateSwing();
            var itemEnumeration = ContainingItem == null ? this._itemEnumeration : ContainingItem.ItemEnumeration;
            var point = _camera.transform.position + _camera.transform.forward * 4f;
            
            _damageAreaSpawner.RPC_Spawn(itemEnumeration, point, _player.Health);
        }

        public void Throw()
        {
            _activeTween?.Kill();
            _activeTween = AnimateThrow();
            RPC_LaunchItem();
        }

        public Tween AnimatePrepare()
        {
            return _foreArmJoint.DOLocalRotate(_initialArmRotation - Vector3.right * 75, 0.5f);
        }

        public Tween AnimateSwing()
        {
            return DOTween.Sequence()
                .Append(_foreArmJoint.DOLocalRotate(_initialArmRotation + Vector3.right * 15 + Vector3.forward * 70, 0.2f))
                .Append(_foreArmJoint.DOLocalRotate(_initialArmRotation, 0.5f));
        }

        public Tween AnimateThrow()
        {
            return _foreArmJoint.DOLocalRotate(_initialArmRotation, 0.1f);
        }
    }
}