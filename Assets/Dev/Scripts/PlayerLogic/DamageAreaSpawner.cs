using Dev.Infrastructure;
using Fusion;
using UnityEngine;

namespace Dev.Scripts.PlayerLogic
{
    public class DamageAreaSpawner : NetworkContext
    {
        [SerializeField] private DamageConfigLibrary _damageConfigLibrary;
        
        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        public void RPC_Spawn(ItemEnumeration itemEnumeration, Vector3 point, Health owner)
        {
            var config = _damageConfigLibrary.GetConfig(itemEnumeration);
            Spawn(config, point, owner);
        }

        private void Spawn(DamageAreaConfig damageAreaConfig ,Vector3 point, Health owner)
        {
            DamageArea damageArea = Runner.Spawn(damageAreaConfig.Prefab, point, Quaternion.identity);
            damageArea.Setup(damageAreaConfig, owner);
        }
    }
}