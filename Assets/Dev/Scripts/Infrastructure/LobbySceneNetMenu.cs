using Cysharp.Threading.Tasks;
using Fusion;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Dev.Scripts.Infrastructure
{
    public class LobbySceneNetMenu : MonoBehaviour
    {
        [SerializeField] private FusionLobbyConnector _lobbyConnector;
        [SerializeField] private float _width = 300;
        [SerializeField] private float _height = 200;
        [SerializeField] private float _startPosOffset = 200;

        [SerializeField] private NetworkRunner _networkRunner;
        
        private bool _isConnecting;
        private bool _isConnected;
        
        private SceneLoader _sceneLoader;

        [Inject]
        private void Construct(SceneLoader sceneLoader)
        {
            _sceneLoader = sceneLoader;
        }
        
        private void LastGameStartResult(StartGameResult startGameResult)
        {
            _isConnecting = false;
            _isConnected = startGameResult.Ok;
        }

        private async void OnGUI()
        {
            if(_isConnecting || _isConnected) return;
            
            var center = new Vector2(Screen.width / 2f, Screen.height / 2);

            var position = new Rect(center.x - (_width / 2), center.y + _startPosOffset, _width,_height);
           
            if (GUI.Button(position, "Start Single Player"))
            {
                _isConnecting = true;
                StartSinglePlayer();
            }
            position.position += Vector2.down * _height;
            
            if (GUI.Button(position, "Start Host"))
            {
                _isConnecting = true;
                StartHost();
            }
            
            /*position.position += Vector2.down * _height;
            if (GUI.Button(position, "Start Client"))
            {
                _isConnecting = true;
                StartClient();
            }*/
            
        }
        
        private async UniTask<StartGameResult> StartGame(StartGameArgs startGameArgs)
        {
            _networkRunner.gameObject.SetActive(true);
            
            Curtains.Instance.SetText("Connecting to the game");
            Curtains.Instance.Show(0.5f);
            
            var startGameResult = await _networkRunner.StartGame(startGameArgs);

            if (startGameResult.Ok == false)
            {
                Debug.LogError($"{startGameResult.ErrorMessage}");
            }
            else
            {
                DontDestroyOnLoad(_networkRunner.gameObject);
                Curtains.Instance.HideWithDelay(1);
            }

            LastGameStartResult(startGameResult);
            
            return startGameResult;
        }

        public void StartSinglePlayer()
        {
            var startGameArgs = new StartGameArgs();
            startGameArgs.GameMode = GameMode.Single;
            startGameArgs.SceneManager = _sceneLoader;
            startGameArgs.Scene = SceneManager.GetActiveScene().buildIndex;
            
            StartGame(startGameArgs);
        }

        public void StartHost()
        {
            var startGameArgs = new StartGameArgs();
            startGameArgs.GameMode = GameMode.Host;
            startGameArgs.SceneManager = _sceneLoader;
            startGameArgs.Scene = SceneManager.GetActiveScene().buildIndex;

            StartGame(startGameArgs);
        }

        public void StartClient()
        {
            var startGameArgs = new StartGameArgs();
            startGameArgs.GameMode = GameMode.Client;
            startGameArgs.SceneManager = _sceneLoader;
            startGameArgs.Scene = SceneManager.GetActiveScene().buildIndex;

            StartGame(startGameArgs);
        }

    }
}