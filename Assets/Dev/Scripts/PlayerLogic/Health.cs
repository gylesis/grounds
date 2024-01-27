using System;
using Dev.Scripts.Infrastructure;
using DG.Tweening;
using Fusion;
using UniRx;
using UnityEngine;

namespace Dev.Scripts.PlayerLogic
{
    public class Health : NetworkContext, IDamageInflictor, IDamageVictim
    {
        [SerializeField] protected float _maxHealth = 100;
        
        [Header("Dont attach if this is Player")]
        [SerializeField] private NetworkRigidbody _rigidbody;
        
        protected float _currentHealth;

        public Subject<HealthChangedContext> Changed { get; } = new Subject<HealthChangedContext>();
        public Subject<Unit> ZeroHealth { get; } = new Subject<Unit>();

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

        public virtual void TakeDamage(float value, IDamageInflictor damageInflictor = null)
        {
            _currentHealth -= value;

            RPC_HealthChanged(_currentHealth, _maxHealth);

            string inflictor = damageInflictor != null ? $"{damageInflictor.PlayerRef}" : $"{PlayerRef.None}";
            
            Debug.Log($"<color=green>{transform.name}</color> was damaged by <color=yellow>{value}</color> by <color=red>{inflictor}</color>. Health: {_currentHealth}");
            
            if (_currentHealth <= 0)
            {
                ZeroHealth.OnNext(Unit.Default);

                RPC_OnDeath();

                Observable.Timer(TimeSpan.FromSeconds(3)).Subscribe((l =>
                {
                    Runner.Despawn(Object);
                }));
            }
        }
        
        [Rpc]
        protected void RPC_OnDeath()
        {
            transform.DOScale(0, 0.6f);
        }

        [Rpc]
        protected void RPC_HealthChanged(float currentHealth, float maxHealth)
        {
            var context = new HealthChangedContext();
            context.CurrentHealth = currentHealth;
            context.MaxHealth = maxHealth;

            Changed.OnNext(context);
        }
    }
    
    public struct HealthChangedContext
    {
        public float CurrentHealth;
        public float MaxHealth;
    }
}