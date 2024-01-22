using System;
using Dev.Infrastructure;
using DG.Tweening;
using Fusion;
using UniRx;
using UnityEngine;

namespace Dev.Scripts.PlayerLogic
{
    public class Health : NetworkContext, IDamageInflictor, IDamageVictim
    {
        [SerializeField] private float _maxHealth = 100;
        
        [Header("Dont attach if this is Player")]
        [SerializeField] private NetworkRigidbody _rigidbody;
        
        private float _currentHealth;

        public Action<float, float> Changed;
        public Action Depleted;

        protected virtual float Mass => _rigidbody.Rigidbody.mass;
        protected virtual float Speed => _rigidbody.Rigidbody.velocity.magnitude;
        
        public PlayerRef PlayerRef => Object.InputAuthority;

        public override void Spawned()
        {
            base.Spawned();

            if (HasStateAuthority)
            {
                _currentHealth = _maxHealth;
            }
        }

        private void OnCollisionEnter(Collision other)
        {
            if(Runner.IsServer == false) return;
            
            if(other.transform.TryGetComponent(out Health health))
            {
                var mass = Mass;
                var speed = Speed;
                if (speed < 1) return;
                
                var damage = speed * mass;
                
                Debug.Log($"Damage: {damage} Mass: {Mass} Speed: {Speed}");
                health.TakeDamage(damage, this);
            }
        }

        public void TakeDamage(float value, IDamageInflictor damageInflictor)
        {
            _currentHealth -= value;

            RPC_HealthChanged(_currentHealth, _maxHealth);
            Debug.Log($"<color=green>{transform.name}</color> was damaged by <color=yellow>{value}</color> by <color=red>{damageInflictor.PlayerRef}</color>. Health: {_currentHealth}");
            
            if (_currentHealth <= 0)
            {
                Depleted?.Invoke();

                RPC_OnDeath();

                Observable.Timer(TimeSpan.FromSeconds(3)).Subscribe((l =>
                {
                    Runner.Despawn(Object);
                }));
            }
        }

        [Rpc]
        private void RPC_OnDeath()
        {
            transform.DOScale(0, 0.6f);
        }

        [Rpc]
        private void RPC_HealthChanged(float currentHealth, float maxHealth)    
        {
            Changed?.Invoke(currentHealth, maxHealth);
        }
    }
}