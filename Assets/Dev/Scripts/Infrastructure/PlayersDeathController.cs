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
            var playerDeathContext = new PlayerDeathContext();
            playerDeathContext.PlayerRef = playerRef;
            
            PlayerDied.OnNext(playerDeathContext);

            var playerCharacter = _playersSpawner.GetPlayer(playerRef);

            playerCharacter.BasePlayerController.SetAllowToAim(false);
            playerCharacter.BasePlayerController.SetAllowToMove(false);
            
            playerCharacter.PlayerView.RPC_OnDeath();
            
            RPC_SwitchCamera(playerRef);
        }

        
        [Rpc]
        private void RPC_SwitchCamera([RpcTarget] PlayerRef playerRef)
        {
            var playerCharacter = _playersSpawner.GetPlayer(playerRef);
            
            CursorController.SetActiveState(true);
            
            _sceneCameraController.SetActiveState(true);
            playerCharacter.CameraController.SetActiveState(false);
            
            var tryGetPopUp = _popUpService.TryGetPopUp<DeathPopUp>(out var deathPopUp);

            if (tryGetPopUp)
            {
                _popUpService.ShowPopUp<DeathPopUp>();
            }   
        }
    }

}