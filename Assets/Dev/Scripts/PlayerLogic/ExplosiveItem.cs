using Dev.Infrastructure;
using DG.Tweening;
using UnityEngine;

namespace Dev.Scripts.PlayerLogic
{
    public class ExplosiveItem : Item
    {
        [SerializeField] private ParticleSystem _sparkles;
        [SerializeField] private ParticleSystem _explosion;
        [SerializeField] private GameObject _view;
                
        
        [Header("Options")] 
        [SerializeField] private float _detonationTime;

        private ImpactApplier _impactApplier;
        
        private void Start()
        {
            Health.HealthDepleted += Explode;
            _impactApplier = DependenciesContainer.Instance.GetDependency<ImpactApplier>();
        }

        public override void Use()
        {
            StartDetonation();
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
            DamageAreaSpawner.Instance.RPC_SpawnSphere(ItemEnumeration.DynamiteExplosion, transform.position, Health);
            _impactApplier.RPC_ApplyInSphere(transform.position, 4, 2);
            Destroy(gameObject,2);
        }

        private void OnDestroy()
        {
            Health.HealthDepleted -= Explode;
        }
    }
}