using System.Collections.Generic;
using System.Linq;
using Dev.Scripts.Infrastructure;
using Dev.Scripts.PlayerLogic;
using Dev.Scripts.PlayerLogic.InventoryLogic;
using Dev.Scripts.UI.PopUpsAndMenus;
using Dev.Scripts.Utils;
using Fusion;
using UniRx;
using UnityEngine;
using Zenject;

namespace Dev.Scripts.LevelLogic
{
    public class ExitZoneHandler : NetworkContext
    {
        [SerializeField] private SpawnPoint[] _spawnPoints;
        [SerializeField] private ExitZone _exitZonePrefab;
        [SerializeField] private int _exitZonesAmount;
            
        private List<PlayerRef> _playersInExitZone = new List<PlayerRef>();
       // private List<TickTimer> _timers = new List<TickTimer>();

        private MarkersHandler _markersHandler;
        private PopUpService _popUpService;
        private SceneCameraController _sceneCameraController;
        private GameInventory _gameInventory;

        [Inject]
        private void Init(MarkersHandler markersHandler, PopUpService popUpService, SceneCameraController sceneCameraController, GameInventory gameInventory)
        {
            _gameInventory = gameInventory;
            _sceneCameraController = sceneCameraController;
            _popUpService = popUpService;
            _markersHandler = markersHandler;
        }

        public override void Spawned()
        {
            base.Spawned();
            
            if (HasStateAuthority)
            {
                SpawnExitZones(_exitZonesAmount);
            }
        }

        public void SpawnExitZones(int amount)
        {   
            for (int i = 0; i < amount; i++)
            {
                SpawnPoint freeSpawnPoint = _spawnPoints.GetFreeSpawnPoint();
                Vector3 spawnPos = freeSpawnPoint.SpawnPos;
                ExitZone exitZone = Runner.Spawn(_exitZonePrefab, spawnPos, Quaternion.identity);
                
                _markersHandler.SpawnWorldMarkerAt(exitZone.transform.position);
                
                exitZone.PlayerEntered.TakeUntilDestroy(this).Subscribe((character => OnExitZoneEntered(character, exitZone)));
                exitZone.PlayerExit.TakeUntilDestroy(this).Subscribe((character => OnExitZoneExit(character, exitZone)));
            }
        }

        private void OnExitZoneExit(PlayerCharacter character, ExitZone exitZone)
        {
            PlayerRef playerRef = character.Object.InputAuthority;
            _playersInExitZone.Remove(playerRef);
        }

        private void OnExitZoneEntered(PlayerCharacter playerCharacter, ExitZone exitZone)
        {
            PlayerRef playerRef = playerCharacter.Object.InputAuthority;

            InventoryData inventoryData = _gameInventory.GetInventoryData(playerRef);

            RPC_OnPlayerExit(playerRef, playerCharacter, inventoryData);
        }
    
        [Rpc]
        private void RPC_OnPlayerExit([RpcTarget] PlayerRef playerRef, PlayerCharacter playerCharacter,
            InventoryData inventoryData)
        {
            CursorController.SetActiveState(true);
            
            _sceneCameraController.SetActiveState(true);
            playerCharacter.CameraController.SetActiveState(false);
            
            var tryGetPopUp = _popUpService.TryGetPopUp<WinPopUp>(out var winPopUp);

            if (tryGetPopUp)
            {
                winPopUp.RPC_Setup(playerRef,inventoryData.InventoryItems.ToList());
            }
        }
        
    }
}
