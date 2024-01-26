﻿using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Dev.Scripts.Infrastructure
{
    public class SceneLoader : NetworkSceneManagerBase
    {
        [SerializeField] private string _sceneName = "Main";

        private NetworkRunner _networkRunner;
        private Scene _loadedScene;
        private NetworkRunner _runner;

        private void Awake()
        {
            _loadedScene = SceneManager.GetActiveScene();
        }

        private void TryGetNetRunner()
        {
            if (_networkRunner == null)
                _networkRunner = FindObjectOfType<NetworkRunner>();
        }

        [ContextMenu(nameof(LoadScene))]
        private void LoadScene()
        {
            TryGetNetRunner();
            _networkRunner.SetActiveScene(_sceneName);
        }

        public void LoadScene(string sceneName)
        {
            if (SceneManager.GetActiveScene().name == sceneName) return;

            TryGetNetRunner();
            Debug.Log($"Setting active scene: {sceneName}");
            _networkRunner.SetActiveScene(sceneName);
        }

        protected override IEnumerator SwitchScene(SceneRef prevScene, SceneRef newScene,
            FinishedLoadingDelegate finished)
        {
            Debug.Log($"Switching Scene from {prevScene} to {newScene}");

            if (newScene == _loadedScene.buildIndex)
            {
                var activeScene = SceneManager.GetActiveScene();
                finished(FindNetworkObjects(activeScene));
                yield break;
            }

            if (prevScene > 0)
            {
                // yield return new WaitForSeconds(1.0f);

                // Debug.Log("De-spawning all players");

                /*foreach (Player player in _playersSpawner.Players)
                {
                    PlayerRef playerRef = player.PlayerRef;

                    //Debug.Log($"De-spawning player {_playersDataService.GetNickname(playerRef)}");
                    Debug.Log($"De-spawning player {playerRef}");

                    _playersSpawner.SetPlayerActiveState(playerRef, false);

                    yield return new WaitForSeconds(0.1f);
                }*/

                // yield return new WaitForSeconds(1.5f - PlayerManager.allPlayers.Count * 0.1f);

                //Debug.Log("De-spawned all players");
            }

            yield return null;
            Debug.Log($"Start loading scene {newScene} in single peer mode");

            /*
            if (_loadedScene != default)
            {
                Debug.Log($"Unloading Scene {_loadedScene.buildIndex}");
                yield return SceneManager.UnloadSceneAsync(_loadedScene);
            }
            */
            
            _loadedScene = default;
            Debug.Log($"Loading scene {newScene}");

            List<NetworkObject> sceneObjects = new List<NetworkObject>();
            if (newScene >= 0)
            {
                yield return SceneManager.LoadSceneAsync(newScene, LoadSceneMode.Additive);
                
                _loadedScene = SceneManager.GetSceneByBuildIndex(newScene);
                Debug.Log($"Loaded scene {newScene}: {_loadedScene}");
                sceneObjects = FindNetworkObjects(_loadedScene, disable: false);
            }

            // Delay one frame
            yield return null;

            Debug.Log($"Switched Scene from {prevScene} to {newScene} - loaded {sceneObjects.Count} scene objects");
            finished(sceneObjects);

            //yield return new WaitForSeconds(5);
            SceneManager.UnloadSceneAsync(0);
        }
    }
}