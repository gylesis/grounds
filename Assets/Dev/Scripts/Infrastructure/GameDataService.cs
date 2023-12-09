using Dev.PlayerLogic;
using Fusion;
using UnityEngine;

namespace Dev.Infrastructure
{
    public class GameDataService : NetworkContext
    {
        public static GameDataService Instance { get; private set; }
        
        [SerializeField] private PlayersSpawner _playersSpawner;

        private void Awake()
        {
            if(Instance != null)
                Destroy(Instance.gameObject);
            
            Instance = this;
        }

        public PlayerCharacter GetPlayer(PlayerRef playerRef)
        {
            return _playersSpawner.GetPlayer(playerRef);
        }
        
        
    }
}