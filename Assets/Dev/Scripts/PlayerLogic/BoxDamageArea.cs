using System;
using UnityEngine;

namespace Dev.Scripts.PlayerLogic
{
    public class BoxDamageArea : DamageArea
    {
        private Vector3 _extents;

        public void Setup(BoxDamageAreaConfig config, IDamageInflictor damageInflictor = null)
        {
            base.Setup(config, damageInflictor);
            _extents = config.Extents;
        }
        
        protected override void CheckForTargets()
        {
            _hits.Clear();
            int hitCount = Runner.LagCompensation.OverlapBox(transform.position, _extents, Quaternion.identity, Object.InputAuthority, _hits, _affectMask);
            for (var i = 0; i < hitCount; i++)
            {
                if (_hits[i].Hitbox.TryGetComponent(out IDamageVictim victim))
                {
                    TryDealDamage(victim);
                }
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position,_extents);
        }
    }
}