using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Dev.Infrastructure;
using Dev.PlayerLogic;
using Dev.UI.PopUpsAndMenus;
using Fusion;
using UniRx;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace Dev.Levels.Interactions
{
    public class ExitZoneHandler : NetworkContext
    {
        [SerializeField] private Transform[] _spawnPoints;
        [SerializeField] private ExitZone _exitZonePrefab;
        [SerializeField] private int _exitZonesAmount;
        
        private List<PlayerRef> _playersInExitZone = new List<PlayerRef>();
       // private List<TickTimer> _timers = new List<TickTimer>();

        private MarkersHandler _markersHandler;
        private PopUpService _popUpService;
        private SceneCameraController _sceneCameraController;

        [Inject]
        private void Init(MarkersHandler markersHandler, PopUpService popUpService, SceneCameraController sceneCameraController)
        {
            _sceneCameraController = sceneCameraController;
            _popUpService = popUpService;
            _markersHandler = markersHandler;
        }

        public override async void Spawned()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(2));
            SpawnExitZones(_exitZonesAmount);
        }

        public void SpawnExitZones(int amount)
        {   
            for (int i = 0; i < amount; i++)
            {
                Vector3 spawnPos = _spawnPoints[Random.Range(i, _spawnPoints.Length)].position;
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

            RPC_OnPlayerExit(playerRef, playerCharacter);
        }
    
        [Rpc]
        private void RPC_OnPlayerExit([RpcTarget] PlayerRef playerRef, PlayerCharacter playerCharacter)
        {
            _sceneCameraController.Camera.gameObject.SetActive(true); 
            playerCharacter.CameraController.CharacterCamera.gameObject.SetActive(false);
            
            _popUpService.ShowPopUp<WinPopUp>();
            
            /*var tryGetPopUp = _popUpService.TryGetPopUp<WinPopUp>(out var winPopUp);

            if (tryGetPopUp)
            {
                //winPopUp.RPC_Setup(playerRef);
                winPopUp.Show();
            }*/
        }
        
        public override void FixedUpdateNetwork()
        {
            
            
        }
    }
}
