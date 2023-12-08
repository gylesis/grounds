using Fusion;
using UnityEngine;

namespace Dev.Scripts.PlayerLogic
{
    public class DamageAreaSpawner : NetworkBehaviour
    {
        public void Spawn(DamageAreaConfig damageAreaConfig, Vector3 point)
        {
            var damageArea = Runner.Spawn(damageAreaConfig.Prefab, point, Quaternion.identity);
            damageArea.Setup(damageAreaConfig);
        }
        
    }
}