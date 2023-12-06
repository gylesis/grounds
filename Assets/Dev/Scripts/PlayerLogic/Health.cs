using System;
using Dev.Infrastructure;
using Fusion;
using UnityEngine;

namespace Dev.Scripts.PlayerLogic
{
    public class Health : NetworkContext
    {
        [SerializeField] private float _health = 100;
        [SerializeField] private NetworkRigidbody _networkRigidbody;
     
        
        
        public Action HealthDepleted;

        private void Awake()
        {
            if (_networkRigidbody == null) _networkRigidbody = GetComponent<NetworkRigidbody>();
        }

        private void OnCollisionEnter(Collision other)
        {
            if(other.transform.TryGetComponent(out Health health))
            {
                var mass = _networkRigidbody.Rigidbody.mass;
                var speed = _networkRigidbody.Rigidbody.velocity.magnitude;
                if (speed < 1) return;
                
                var damage = speed * mass;
                
                
                Debug.Log($"Damage: {damage} Mass: {_networkRigidbody.Rigidbody.mass} Speed: {_networkRigidbody.Rigidbody.velocity.magnitude}");
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