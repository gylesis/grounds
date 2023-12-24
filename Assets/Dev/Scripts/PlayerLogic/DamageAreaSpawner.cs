using Dev.Infrastructure;
using Fusion;
using UnityEngine;

namespace Dev.Scripts.PlayerLogic
{
    public class DamageAreaSpawner : NetworkContext
    {
        [SerializeField] private DamageConfigLibrary _damageConfigLibrary;
        [SerializeField] private BoxDamageArea _boxDamageAreaPrefab;
        [SerializeField] private SphereDamageArea _sphereDamageAreaPrefab;
                
        
        private static DamageAreaSpawner _instance;

        public static DamageAreaSpawner Instance
        {
            get
            {
                if (_instance)
                {
                    return _instance;
                }
                _instance = DependenciesContainer.Instance.GetDependency<DamageAreaSpawner>();
                return _instance;
            }
        }

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        public void RPC_SpawnBox(ItemEnumeration itemEnumeration, Vector3 point, Health owner)
        {
            var config = (BoxDamageAreaConfig) _damageConfigLibrary.GetConfig(itemEnumeration);
            SpawnBox(config, point, owner);
        }

        private void SpawnBox(BoxDamageAreaConfig boxDamageAreaConfig ,Vector3 point, Health owner)
        {
            BoxDamageArea damageArea = Runner.Spawn(_boxDamageAreaPrefab, point, Quaternion.identity);
            damageArea.Setup(boxDamageAreaConfig, owner);
        }
        
        
        
        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        public void RPC_SpawnSphere(ItemEnumeration itemEnumeration, Vector3 point, Health owner)
        {
            var config = (SphereDamageAreaConfig)_damageConfigLibrary.GetConfig(itemEnumeration);
            SpawnSphere(config, point, owner);
        }
        
        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        public void RPC_SpawnSphere(SphereDamageAreaConfig boxDamageAreaConfig, Vector3 point, Health owner)
        {
            SpawnSphere(boxDamageAreaConfig, point, owner);
        }

        private void SpawnSphere(SphereDamageAreaConfig sphereDamageAreaConfig ,Vector3 point, Health owner)
        {
            SphereDamageArea damageArea = Runner.Spawn(_sphereDamageAreaPrefab, point, Quaternion.identity);
            damageArea.Setup(sphereDamageAreaConfig, owner);
        }
    }
}