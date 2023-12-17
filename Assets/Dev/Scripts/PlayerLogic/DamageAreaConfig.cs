using Sirenix.OdinInspector;
using UnityEngine;

namespace Dev.Scripts.PlayerLogic
{
    [CreateAssetMenu(fileName = "DamageAreaConfig", menuName = "GameConfigs/DamageAreaConfig", order = 0)]
    public class DamageAreaConfig : SerializedScriptableObject
    {
        [SerializeField] private DamageArea _prefab;

        [SerializeField] private float _duration;
        
        [SerializeField] private float _damagePeriod; // 0 означает, что урон нанесётся единожды

        [SerializeField] private Vector3 _extents;

        [SerializeField] private float _damage;

        [SerializeField] private LayerMask _affectMask;
        
        public DamageArea Prefab => _prefab;

        public float Duration => _duration;

        public Vector3 Extents => _extents;

        public float DamagePeriod => _damagePeriod;

        public float Damage => _damage;

        public LayerMask AffectMask => _affectMask;
    }
}