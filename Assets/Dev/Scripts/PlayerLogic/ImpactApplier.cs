using System.Collections.Generic;
using Dev.Infrastructure;
using Fusion;
using UnityEngine;

namespace Dev.Scripts.PlayerLogic
{
    public class ImpactApplier : NetworkContext
    {
        private List<LagCompensatedHit> _hits;

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        public void RPC_ApplyInSphere(Vector3 center, float radius, float force)
        {
            int hitCount = Runner.LagCompensation.OverlapSphere(transform.position, radius, Object.InputAuthority, _hits);            

            Debug.Log($"Found {hitCount} objects to impact .............");
            for (var i = 0; i < hitCount; i++)
            {
                if (_hits[i].Hitbox.TryGetComponent(out NetworkRigidbody netRigidbody))
                {
                    netRigidbody.Rigidbody.AddExplosionForce(force, center, radius);
                }
            }
        }

        public void ApplyTo(NetworkRigidbody netRigidbody, Vector3 direction, float force)
        {
            netRigidbody.Rigidbody.AddForce(direction * force, ForceMode.Impulse);
        }
    }
}