using Sirenix.OdinInspector;
using UnityEngine;

namespace Dev.Scripts.PlayerLogic
{
    [CreateAssetMenu(fileName = "DamageAreaConfig", menuName = "GameConfigs/DamageAreaConfig", order = 0)]
    public abstract class DamageAreaConfig : SerializedScriptableObject
    {
        [SerializeField] private float _duration;
        
        [SerializeField] private float _damagePeriod; // 0 означает, что урон нанесётся единожды
        
        [SerializeField] private float _damage;

        [SerializeField] private LayerMask _affectMask;
        
        public float Duration => _duration;


        public float DamagePeriod => _damagePeriod;

        public float Damage => _damage;

        public LayerMask AffectMask => _affectMask;
    }
}