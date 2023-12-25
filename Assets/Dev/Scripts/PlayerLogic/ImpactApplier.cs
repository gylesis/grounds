using System.Collections.Generic;
using Dev.Infrastructure;
using Fusion;
using UnityEngine;

namespace Dev.Scripts.PlayerLogic
{
    public class ImpactApplier : NetworkContext
    {
        private List<LagCompensatedHit> _hits = new();

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        public void RPC_ApplyInSphere(Vector3 center, float radius, float force)
        {
            int hitCount = Runner.LagCompensation.OverlapSphere(center, radius, Runner.LocalPlayer, _hits);            

            for (var i = 0; i < hitCount; i++)
            {
                if (_hits[i].Hitbox.TryGetComponent(out NetworkRigidbody netRigidbody))
                {
                    netRigidbody.Rigidbody.AddForce(Vector3.up * force, ForceMode.Impulse);
                   // netRigidbody.Rigidbody.AddExplosionForce(force, center, radius);
                }
            }
        }

        public void ApplyTo(NetworkRigidbody netRigidbody, Vector3 direction, float force)
        {
            netRigidbody.Rigidbody.AddForce(direction * force, ForceMode.Impulse);
        }
    }
}