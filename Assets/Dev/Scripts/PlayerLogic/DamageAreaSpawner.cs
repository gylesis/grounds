using Dev.Scripts.Infrastructure;
using Fusion;
using UnityEngine;

namespace Dev.Scripts.PlayerLogic
{
    public class DamageAreaSpawner : NetworkContext
    {
        [SerializeField] private DamageConfigLibrary _damageConfigLibrary;
        [SerializeField] private BoxDamageArea _boxDamageAreaPrefab;
        [SerializeField] private SphereDamageArea _sphereDamageAreaPrefab;
                

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        public void RPC_SpawnBox(int itemId, Vector3 point, Health inflictor)
        {
            var config = (BoxDamageAreaConfig) _damageConfigLibrary.GetConfig(itemId);
            SpawnBox(config, point, inflictor);
        }

        private void SpawnBox(BoxDamageAreaConfig boxDamageAreaConfig ,Vector3 point, Health inflictor)
        {
            BoxDamageArea damageArea = Runner.Spawn(_boxDamageAreaPrefab, point, Quaternion.identity);
            damageArea.Setup(boxDamageAreaConfig, inflictor);
        }
        
        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        public void RPC_SpawnSphere(int itemId, Vector3 point, Health inflictor)
        {
            var config = (SphereDamageAreaConfig)_damageConfigLibrary.GetConfig(itemId);
            SpawnSphere(config, point, inflictor);
        }

        private void SpawnSphere(SphereDamageAreaConfig sphereDamageAreaConfig ,Vector3 point, Health inflictor)
        {
            SphereDamageArea damageArea = Runner.Spawn(_sphereDamageAreaPrefab, point, Quaternion.identity);
            damageArea.Setup(sphereDamageAreaConfig, inflictor);
        }
    }
}