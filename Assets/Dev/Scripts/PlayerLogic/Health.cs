using System;
using Dev.Infrastructure;
using Fusion;
using UnityEngine;

namespace Dev.Scripts.PlayerLogic
{
    public class Health : NetworkContext, IDamageInflictor, IDamageVictim
    {
        [SerializeField] private float _maxHealth = 100;
        [Networked] private float CurrentHealth { get; set; }
        [SerializeField] private Rigidbody _rigidbody;

        public Action<float, float> Changed;
        
        public Action Depleted;

        public string GameObjectName => gameObject.name;

        private void Awake()
        {
            if (_rigidbody == null) _rigidbody = GetComponent<Rigidbody>();
        }

        public override void Spawned()
        {
            base.Spawned();
            CurrentHealth = _maxHealth;
        }

        private void OnCollisionEnter(Collision other)
        {
            if(other.transform.TryGetComponent(out Health health))
            {
                var mass = _rigidbody.mass;
                var speed = _rigidbody.velocity.magnitude;
                if (speed < 1) return;
                
                var damage = speed * mass;
                
                Debug.Log($"Damage: {damage} Mass: {_rigidbody.mass} Speed: {_rigidbody.velocity.magnitude}");
                health.TakeDamage(damage, this);
            }
        }

        public void TakeDamage(float value, IDamageInflictor damageInflictor)
        {
            CurrentHealth -= value;
            Changed?.Invoke(CurrentHealth ,_maxHealth);
            Debug.Log($"<color=green>{transform.name}</color> was damaged by <color=yellow>{value}</color> by <color=red>{damageInflictor.GameObjectName}</color>. Health: {_maxHealth}");
            if (_maxHealth <= 0)
            {
                Depleted?.Invoke();
            }
        }
    }
}