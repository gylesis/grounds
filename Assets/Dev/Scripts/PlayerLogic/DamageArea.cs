using System.Collections.Generic;
using Dev.Infrastructure;
using Fusion;
using Unity.Mathematics;
using UnityEngine;

namespace Dev.Scripts.PlayerLogic
{
    public class DamageArea : NetworkContext
    {
        private float _damage;
        private float _timer;
        private float _damagePeriod;
        private bool _active;
        private Vector3 _extents;

        private LayerMask _affectMask;
        private IDamageInflictor _damageInflictor;

        private List<LagCompensatedHit> _hits;
        private Dictionary<IDamageVictim, float> _periodTimerForEveryVictim;

        public void Setup(DamageAreaConfig config, IDamageInflictor damageInflictor = null)
        {
            _damage = config.Damage;
            _timer = config.Duration;
            _damagePeriod = config.DamagePeriod;
            _damageInflictor = damageInflictor;
            _active = true;
            _extents = config.Extents;
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
            foreach (var keyValuePair in _periodTimerForEveryVictim)
            {
                _periodTimerForEveryVictim[keyValuePair.Key] -= Time.deltaTime;
            }
        }

        public override void FixedUpdateNetwork()
        {
            CheckForTargets();
        }

        private void CheckForTargets()
        {
            _hits.Clear();
            int hitCount = Runner.LagCompensation.OverlapBox(transform.position, _extents, quaternion.identity, Object.InputAuthority, _hits, _affectMask);
            for (var i = 0; i < hitCount; i++)
            {
                if (_hits[i].Hitbox.TryGetComponent(out IDamageVictim victim))
                {
                    TryDealDamage(victim);
                }
            }
        }
        
        private void TryDealDamage(IDamageVictim victim)
        {
            if (_damageInflictor.GameObject == victim.GameObject) return;
            
            if (_damagePeriod == 0)
            {
                if (_periodTimerForEveryVictim.ContainsKey(victim))
                {
                    victim.TakeDamage(_damage, _damageInflictor);
                    _periodTimerForEveryVictim.Add(victim, 0);
                }
            }
            else if (_periodTimerForEveryVictim.ContainsKey(victim))
            {
                if (_periodTimerForEveryVictim[victim] <= 0)
                {
                    victim.TakeDamage(_damage, _damageInflictor);
                    _periodTimerForEveryVictim.Add(victim, _damagePeriod);
                }
            }
        }

        private void Disable()
        {
            _active = false;
        }
    }
}