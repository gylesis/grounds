using UnityEngine;

namespace Dev.Scripts.PlayerLogic
{
    [CreateAssetMenu(fileName = "DamageAreaConfig", menuName = "GameConfigs/SphereDamageAreaConfig", order = 0)]
    public class SphereDamageAreaConfig : DamageAreaConfig
    {
        [SerializeField] private float _radius;

        public float Radius => _radius;
    }
}