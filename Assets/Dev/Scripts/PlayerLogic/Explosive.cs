using Dev.Infrastructure;
using DG.Tweening;
using UnityEngine;

namespace Dev.Scripts.PlayerLogic
{
    public class Explosive : NetworkContext
    {
        [SerializeField] private Item _correspondingItem;
        [SerializeField] private ParticleSystem _sparkles;
        [SerializeField] private ParticleSystem _explosion;
        [SerializeField] private GameObject _view;
                
        
        [Header("Options")] 
        [SerializeField] private float _detonationTime;

        private ImpactApplier _impactApplier;
        
        private void Start()
        {
            _correspondingItem.Health.HealthDepleted += Explode;
            _impactApplier = DependenciesContainer.Instance.GetDependency<ImpactApplier>();
            _correspondingItem.UpdateUseAction(StartDetonation);
        }

        private void StartDetonation()
        {
            _sparkles.Play();
            DOTween.Sequence().AppendInterval(_detonationTime).OnComplete(Explode);
        }   

        private void Explode()
        {
            _explosion.Play();
            _view.SetActive(false);
            DamageAreaSpawner.Instance.RPC_SpawnSphere(ItemEnumeration.DynamiteExplosion, transform.position, _correspondingItem.ItemDynamicData.LastOwner);
            _impactApplier.RPC_ApplyInSphere(transform.position, 4, 2);
            DOTween.Sequence().AppendInterval(2).OnComplete(() =>  Runner.Despawn(Object));
        }

        private void OnDestroy()
        {
            _correspondingItem.Health.HealthDepleted -= Explode;
        }
    }
}