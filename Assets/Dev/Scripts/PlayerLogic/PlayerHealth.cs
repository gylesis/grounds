using System;
using UniRx;
using UnityEngine;
using Zenject;

namespace Dev.Scripts.PlayerLogic
{
    public class PlayerHealth : Health
    {
        private PlayerCharacter _playerCharacter;

        protected override float Mass => _playerCharacter.Kcc.Settings.Mass;
        protected override float Speed => _playerCharacter.Kcc.Data.RealVelocity.magnitude;

        [Inject]
        private void Construct(PlayerCharacter playerCharacter)
        {
            _playerCharacter = playerCharacter;
        }

        public override void TakeDamage(float value, IDamageInflictor damageInflictor = null)
        {
            _currentHealth -= value;

            RPC_HealthChanged(_currentHealth, _maxHealth);

            string inflictor = damageInflictor != null ? $"{damageInflictor.PlayerRef}" : $"{Fusion.PlayerRef.None}";
            
            Debug.Log($"<color=green>{transform.name}</color> was damaged by <color=yellow>{value}</color> by <color=red>{inflictor}</color>. Health: {_currentHealth}");
            
            if (_currentHealth <= 0)
            {
                ZeroHealth.OnNext(Unit.Default);
            }
        }
    }
}