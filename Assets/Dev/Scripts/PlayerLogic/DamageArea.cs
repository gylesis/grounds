using System.Collections.Generic;
using System.Linq;
using Dev.Infrastructure;
using Fusion;
using UnityEngine;

namespace Dev.Scripts.PlayerLogic
{
    public abstract class DamageArea : NetworkContext
    {
        private float _damage;
        private float _timer;
        private float _damagePeriod;
        private bool _active;

        protected LayerMask _affectMask;
        private IDamageInflictor _damageInflictor;

        protected readonly List<LagCompensatedHit> _hits = new();
        private readonly Dictionary<IDamageVictim, float> _periodTimerForEveryVictim = new();

        public virtual void Setup(DamageAreaConfig config, IDamageInflictor damageInflictor = null)
        {
            _damage = config.Damage;
            _timer = config.Duration;
            _damagePeriod = config.DamagePeriod;
            _damageInflictor = damageInflictor;
            _active = true;
            _affectMask = config.AffectMask;
        }

        public override void Render()
        {
            if (_active is not true) return;

            UpdateAreaTimer();
            UpdateDamageTimersForVictims();
        }

        private void UpdateAreaTimer()
        {
            if (_timer > 0)
            {
                _timer -= Time.deltaTime;
            }
            else
            {
                Disable();
            }
        }

        private void UpdateDamageTimersForVictims()
        {
            foreach (var keyValuePair in _periodTimerForEveryVictim.ToList())
            {
                _periodTimerForEveryVictim[keyValuePair.Key] -= Time.deltaTime;
            }
        }

        public override void FixedUpdateNetwork()
        {
            CheckForTargets();
        }

        protected abstract void CheckForTargets();

        protected void TryDealDamage(IDamageVictim victim)
        {
            if (_damageInflictor.GameObject == victim.GameObject) return;
            
            if (_damagePeriod == 0)
            {
                if (!_periodTimerForEveryVictim.ContainsKey(victim))
                {
                    victim.TakeDamage(_damage, _damageInflictor);
                    _periodTimerForEveryVictim.Add(victim, 0);
                }
            }
            else if (!_periodTimerForEveryVictim.ContainsKey(victim))
            {
                _periodTimerForEveryVictim.Add(victim, _damagePeriod);

                if (_periodTimerForEveryVictim[victim] <= 0)
                {
                    victim.TakeDamage(_damage, _damageInflictor);
                }
            }
            else
            {
                victim.TakeDamage(_damage, _damageInflictor);
            }
        }

        private void Disable()
        {
            _active = false;
            Destroy(gameObject);
        }
    }
}