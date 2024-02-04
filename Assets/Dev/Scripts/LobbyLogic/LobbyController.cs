using Dev.Scripts.Infrastructure;
using Dev.Scripts.LevelLogic;
using UnityEngine;

namespace Dev.Scripts.LobbyLogic
{
    public class LobbyController : NetworkContext
    {
        [SerializeField] private IslandCloudsController _islandCloudsController;
        [SerializeField] private Island _islandPrefab;

        [SerializeField] private SpawnPoint _leftIslandSpawnPoint;
        [SerializeField] private SpawnPoint _rightIslandSpawnPoint;
        
        [SerializeField] private int _ownerId = 1;

        public override void Spawned()
        {
            base.Spawned();
            
           // SpawnIsland();
        }

        private void SpawnIsland()
        {
            var islandSaveData = new IslandSaveData();
            islandSaveData.OwnerId = _ownerId;
            
            LoadIsland(islandSaveData);
        }

        public void LoadIsland(IslandSaveData saveData)
        {
            var isLeft = true;
            
            _islandCloudsController.RemoveWall(isLeft);

            Vector3 spawnPos;
            
            if (isLeft)
            {
                spawnPos = _leftIslandSpawnPoint.SpawnPos;
            }
            else
            {
                spawnPos = _rightIslandSpawnPoint.SpawnPos;
            }

            Island island = Runner.Spawn(_islandPrefab, spawnPos);
        }
        
    }
}