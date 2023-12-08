using System;
using Dev.Infrastructure;
using Fusion;
using UnityEngine;

namespace Dev.Scripts.PlayerLogic
{
    public class Health : NetworkContext
    {
        [SerializeField] private float _health = 100;
        [SerializeField] private Rigidbody _rigidbody;
     
        
        
        public Action HealthDepleted;

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
                health.DecreaseHealth(damage, this);
            }
        }

        public void DecreaseHealth(float value, Health damageInflictor)
        {
            _health -= value;
            Debug.Log($"{transform.name} was damaged by {damageInflictor.name}. Health: {_health}");
            if (_health <= 0)
            {
                HealthDepleted?.Invoke();
            }
        }
        
        
    }
}