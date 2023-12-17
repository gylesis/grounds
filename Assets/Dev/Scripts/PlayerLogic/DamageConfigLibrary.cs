using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Dev.Scripts.PlayerLogic
{
    [CreateAssetMenu(fileName = "DamageConfigLibrary", menuName = "GameConfigs/DamageConfigLibrary", order = 0)]
    public class DamageConfigLibrary : SerializedScriptableObject
    {
        [SerializeField] private List<DamageConfigLibraryElement> _configs;
        [ReadOnly] [SerializeField] private Dictionary<ItemEnumeration, DamageAreaConfig> _configsDictionary;

        [Button("Refresh")]
        private void OnEnable()
        {
            _configsDictionary = new Dictionary<ItemEnumeration, DamageAreaConfig>();
            foreach (var config in _configs)
            {
                _configsDictionary.Add(config.ItemEnumeration, config.DamageAreaConfig);
            }
        }

        public DamageAreaConfig GetConfig(ItemEnumeration itemEnumeration)
        {
            return _configsDictionary[itemEnumeration];
        }
    }

    [Serializable]
    public class DamageConfigLibraryElement
    {
        public ItemEnumeration ItemEnumeration;
        public DamageAreaConfig DamageAreaConfig;
    }
}