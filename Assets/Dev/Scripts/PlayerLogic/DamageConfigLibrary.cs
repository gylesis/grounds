using System;
using System.Collections.Generic;
using Dev.Scripts.Items;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Dev.Scripts.PlayerLogic
{
    [CreateAssetMenu(fileName = "DamageConfigLibrary", menuName = "GameConfigs/DamageConfigLibrary", order = 0)]
    public class DamageConfigLibrary : SerializedScriptableObject
    {
        [TableList] [SerializeField] private List<DamageConfigLibraryElement> _configs;
        [ReadOnly] [SerializeField] private Dictionary<int, DamageAreaConfig> _configsDictionary;

        [Button("Refresh")]
        private void OnEnable()
        {
            _configsDictionary = new Dictionary<int, DamageAreaConfig>();
            foreach (var config in _configs)
            {
                if (_configsDictionary.ContainsKey(config.ItemId))
                {
                    Debug.LogError($"Item {config.ItemId} with that id already exits, Fix this!!!!!!!");
                    continue;
                }
                _configsDictionary.Add(config.ItemId, config.DamageAreaConfig);
            }
        }

        public DamageAreaConfig GetConfig(int itemId)
        {
            return _configsDictionary[itemId];
        }
    }

    [Serializable]
    public class DamageConfigLibraryElement
    {
        [SerializeField] private ItemNameTag _itemNameTag;
        public int ItemId => _itemNameTag.ItemId;
        public DamageAreaConfig DamageAreaConfig;
    }
}