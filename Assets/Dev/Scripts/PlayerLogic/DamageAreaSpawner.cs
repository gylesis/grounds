using Fusion;
using UnityEngine;

namespace Dev.Scripts.PlayerLogic
{
    public class DamageAreaSpawner : NetworkBehaviour
    {
        [SerializeField] private Vector3 _pointToSpawn;
        
        public void Spawn(DamageAreaConfig damageAreaConfig)
        {
            var damageArea = Instantiate(damageAreaConfig.Prefab, _pointToSpawn, Quaternion.identity);
            damageArea.Setup(damageAreaConfig);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawSphere(transform.position + _pointToSpawn, 0.25f);
        }
    }
}