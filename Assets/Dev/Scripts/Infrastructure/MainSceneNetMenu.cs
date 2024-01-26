using Fusion;
using UnityEngine;

namespace Dev.Scripts.Infrastructure
{
    public class MainSceneNetMenu : MonoBehaviour
    {
        [SerializeField] private ConnectionManager _connectionManager;
        [SerializeField] private float _width = 300;
        [SerializeField] private float _height = 200;
        [SerializeField] private float _startPosOffset = 200;

        private bool _isConnecting;
        private bool _isConnected;

        private void OnEnable()
        {
            _connectionManager.LastGameStartResult += LastGameStartResult;
        }

        private void OnDisable()
        {
            _connectionManager.LastGameStartResult -= LastGameStartResult;
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
                _connectionManager.StartSinglePlayer();
            }
            position.position += Vector2.down * _height;
            if (GUI.Button(position, "Start Host"))
            {
                _isConnecting = true;
                _connectionManager.StartHost();
            }
            
            position.position += Vector2.down * _height;
            if (GUI.Button(position, "Start Client"))
            {
                _isConnecting = true;
                _connectionManager.StartClient();
            }
            
             position.position += Vector2.down * _height;
            if (GUI.Button(position, "Start Server"))
            {
                _isConnecting = true;
                _connectionManager.StartServer();
            }
            
        }
    }
}