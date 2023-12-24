using UnityEngine;

namespace Dev.Scripts.PlayerLogic
{
    [CreateAssetMenu(fileName = "DamageAreaConfig", menuName = "GameConfigs/BoxDamageAreaConfig", order = 0)]
    public class BoxDamageAreaConfig : DamageAreaConfig
    {
        [SerializeField] private Vector3 _extents;
        
        public Vector3 Extents => _extents;
    }
}