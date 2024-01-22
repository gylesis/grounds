﻿using Dev.Infrastructure;
using Dev.Scripts.Items;
using Dev.Scripts.PlayerLogic.InventoryLogic;
using DG.Tweening;
using Fusion;
using UnityEngine;
using Zenject;

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
        private DamageAreaSpawner _damageAreaSpawner;
        private ItemStaticDataContainer _itemStaticDataContainer;

        [Inject]
        private void Construct(ImpactApplier impactApplier, DamageAreaSpawner damageAreaSpawner, ItemStaticDataContainer itemStaticDataContainer)
        {
            _itemStaticDataContainer = itemStaticDataContainer;
            _damageAreaSpawner = damageAreaSpawner;
            _impactApplier = impactApplier;
        }

        private void Start()
        {
            _correspondingItem.Health.Depleted += Explode;
            _correspondingItem.UpdateUseAction(StartDetonation);
        }

        private void StartDetonation()
        {
            _sparkles.Play();
            DOTween.Sequence().AppendInterval(_detonationTime).OnComplete(Explode);
        }   

        private void Explode()
        {
            RPC_ExplodeVisuals();

            _itemStaticDataContainer.TryGetItemByType(out var itemStaticData, ItemType.Explosive);
            _damageAreaSpawner.RPC_SpawnSphere(itemStaticData.ItemId, transform.position, _correspondingItem.ItemDynamicData.LastOwner);
            _impactApplier.RPC_ApplyInSphere(transform.position, 4, 200);
            DOTween.Sequence().AppendInterval(2).OnComplete(() =>  Runner.Despawn(Object));
        }

        [Rpc]
        private void RPC_ExplodeVisuals()
        {
            _explosion.Play();
            _view.SetActive(false); 
        }
        
        private void OnDestroy()
        {
            _correspondingItem.Health.Depleted -= Explode;
        }
    }
}