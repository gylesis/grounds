using System.Collections.Generic;
using Dev.Scripts.PlayerLogic;
using Dev.Scripts.UI.PopUpsAndMenus;
using Fusion;
using UniRx;
using UnityEngine;
using Zenject;

namespace Dev.Scripts.Infrastructure
{
    public class PlayersDeathController : NetworkContext
    {
        private PlayersSpawner _playersSpawner;
        private List<PlayerRef> _players = new();
        private SceneCameraController _sceneCameraController;
        private PopUpService _popUpService;

        public Subject<PlayerDeathContext> PlayerDied { get; } = new();
            
        [Inject]
        private void Construct(PlayersSpawner playersSpawner, SceneCameraController sceneCameraController, PopUpService popUpService)
        {
            _popUpService = popUpService;
            _sceneCameraController = sceneCameraController;
            _playersSpawner = playersSpawner;
        }

        protected override void OnDependenciesResolve()
        {
            base.OnDependenciesResolve();

            _playersSpawner.PlayerSpawned.TakeUntilDestroy(this).Subscribe((OnPlayerSpawned));
            _playersSpawner.PlayerDeSpawned.TakeUntilDestroy(this).Subscribe((OnPlayerDeSpawned));
            _playersSpawner.PlayerRespawned.TakeUntilDestroy(this).Subscribe((OnPlayerRespawned));
        }

        private void OnPlayerSpawned(PlayerRef playerRef)
        {
            _players.Add(playerRef);
            PlayerCharacter playerCharacter = _playersSpawner.GetPlayer(playerRef);

            playerCharacter.Health.ZeroHealth.TakeUntilDestroy(this).Subscribe((unit => OnPlayerHealthZero(playerRef)));
        }

        private void OnPlayerDeSpawned(PlayerRef playerRef)
        {
            _players.Remove(playerRef);
        }

        private void OnPlayerHealthZero(PlayerRef playerRef)
        {
            if(HasStateAuthority == false) return;
            
            var playerCharacter = _playersSpawner.GetPlayer(playerRef);

            
                /*Debug.Log($"Teleport");
                playerCharacter.Kcc.SetPosition(Vector3.zero + Vector3.one * 2);
                playerCharacter.Kcc.SetLookRotation(Vector2.zero);

                playerCharacter.Kcc.SetDynamicVelocity(Vector3.zero);
            
                playerCharacter.Kcc.SetKinematicVelocity(Vector3.zero);*/
            
            
            var playerDeathContext = new PlayerDeathContext();
            playerDeathContext.PlayerRef = playerRef;
            
            PlayerDied.OnNext(playerDeathContext);

           // var playerCharacter = _playersSpawner.GetPlayer(playerRef);

            playerCharacter.PlayerController.SetAllowToAim(false);
            playerCharacter.PlayerController.SetAllowToMove(false);
            
            playerCharacter.HitboxRoot.HitboxRootActive = false;
            
            playerCharacter.PlayerView.RPC_OnDeath(true);
            
            RPC_SwitchCamera(playerRef, false);
        }

        private void OnPlayerRespawned(PlayerRef playerRef)
        {
            Debug.Log($"Player respawned");
            RPC_SwitchCamera(playerRef, true);
        }

        [Rpc]
        private void RPC_SwitchCamera([RpcTarget] PlayerRef playerRef, bool isOn)
        {
            var playerCharacter = _playersSpawner.GetPlayer(playerRef);
            
            CursorController.SetActiveState(!isOn);
            
            _sceneCameraController.SetActiveState(!isOn);
            playerCharacter.CameraController.SetActiveState(isOn);
            
            var tryGetPopUp = _popUpService.TryGetPopUp<DeathPopUp>(out var deathPopUp);

            if (tryGetPopUp)
            {
                if (isOn)
                {
                    _popUpService.HidePopUp<DeathPopUp>();
                }
                else
                {
                    _popUpService.ShowPopUp<DeathPopUp>();
                }
            }   
        }
    }

}