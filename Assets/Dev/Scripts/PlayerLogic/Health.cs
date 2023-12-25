using System;
using Dev.Infrastructure;
using Fusion;
using UnityEngine;

namespace Dev.Scripts.PlayerLogic
{
    public class Health : NetworkContext, IDamageInflictor, IDamageVictim
    {
        [Networked] [SerializeField] private float _health { get; set; } = 100;
        [SerializeField] private Rigidbody _rigidbody;

        public Action HealthDepleted;

        public string GameObjectName => gameObject.name;

        private void Awake()
        {
            if (_rigidbody == null) _rigidbody = GetComponent<Rigidbody>();
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
            _health -= value;
            Debug.Log($"<color=green>{transform.name}</color> was damaged by <color=yellow>{value}</color> by <color=red>{damageInflictor.GameObjectName}</color>. Health: {_health}");
            if (_health <= 0)
            {
                HealthDepleted?.Invoke();
            }
        }
    }
}