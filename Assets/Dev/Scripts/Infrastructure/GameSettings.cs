using UnityEngine;

namespace Dev.Scripts.Infrastructure
{
    [CreateAssetMenu(fileName = "GameSettings", menuName = "StaticData/GameSettings", order = 0)]
    public class GameSettings : ScriptableObject
    {
        [Header("Game")]

        [SerializeField] private bool _isFriendlyFireOn;
            
        public bool IsFriendlyFireOn => _isFriendlyFireOn;
    }
}