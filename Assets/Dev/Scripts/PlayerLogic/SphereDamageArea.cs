
using UnityEngine;

namespace Dev.Scripts.PlayerLogic
{
    public class SphereDamageArea : DamageArea
    {
        private float _radius;
        
        public void Setup(SphereDamageAreaConfig config, IDamageInflictor damageInflictor = null)
        {
            base.Setup(config, damageInflictor);
            _radius = config.Radius;
        }

        protected override void CheckForTargets()
        {
            _hits.Clear();
            int hitCount = Runner.LagCompensation.OverlapSphere(transform.position, _radius, Object.InputAuthority, _hits, _affectMask);            

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
            Gizmos.DrawWireSphere(transform.position, _radius);
        }
    }
}