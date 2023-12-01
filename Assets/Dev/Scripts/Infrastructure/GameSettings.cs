using Dev.Utils;
using UnityEngine;
using UnityEngine.Serialization;

namespace Dev.Infrastructure
{
    [CreateAssetMenu(fileName = "GameSettings", menuName = "StaticData/GameSettings", order = 0)]
    public class GameSettings : ScriptableObject
    {
        [Header("Game")]

        [SerializeField] private bool _isFriendlyFireOn;
            
        public bool IsFriendlyFireOn => _isFriendlyFireOn;

    }
}