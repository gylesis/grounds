using System;
using System.Linq;
using Dev.Scripts.Utils;
using Steamworks;
using Steamworks.Data;
using UnityEngine;

namespace Dev.Scripts
{
    public class SteamConnectionManager : MonoBehaviour
    {
        [SerializeField] private string _lobbyName = "TestingS";
        [Min(0)][SerializeField] private int _appId = 480;
            
        private void Awake()
        {
            try
            {
                SteamClient.Init((uint)_appId);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
                return;
            }
            
            SteamMatchmaking.CreateLobbyAsync(3);
        }

        private void OnEnable()
        {
            SteamMatchmaking.OnLobbyInvite += OnLobbyInvited;
            SteamMatchmaking.OnLobbyMemberJoined += OnLobbyJoined;
            SteamMatchmaking.OnLobbyEntered += OnLobbyEntered;
            SteamMatchmaking.OnLobbyCreated += OnLobbyCreated;
        }

        private void OnDisable()
        {
            SteamMatchmaking.OnLobbyInvite -= OnLobbyInvited;
            SteamMatchmaking.OnLobbyMemberJoined -= OnLobbyJoined;
            SteamMatchmaking.OnLobbyEntered -= OnLobbyEntered;
            SteamMatchmaking.OnLobbyCreated -= OnLobbyCreated;
            
            SteamClient.Shutdown();
        }

        public string GetNickname()
        {
            if (SteamClient.IsLoggedOn == false) return "Player";
            
            return SteamClient.Name;
        }
        
        
        private void OnLobbyInvited(Friend friend, Lobby lobby)
        {
            Debug.Log($"Friend {friend.Name} invited to lobby {lobby.GetData(Constants.Steam.LobbyNameKey)}");
        }

        private void OnLobbyJoined(Lobby lobby, Friend friend)
        {
            Debug.Log($"{friend.Name} joined lobby {lobby.GetData(Constants.Steam.LobbyNameKey)}");
        }

        private void OnLobbyEntered(Lobby lobby)
        {
            Debug.Log($"Lobby Entered - {lobby.GetData(Constants.Steam.LobbyNameKey)}");
        }
    
        private void OnLobbyCreated(Result result, Lobby lobby)
        {
            if (result == Result.OK)
            {
                lobby.SetPublic(); 
                lobby.SetData(Constants.Steam.LobbyNameKey, $"{SteamClient.Name}'s game" );
                lobby.SetJoinable(true);
                lobby.SetFriendsOnly();
                lobby.SetGameServer(lobby.Owner.Id);
                Debug.Log($"lobby created {result}"); 
            }
            else
            {
                Debug.Log($"Lobby was not created {result}");
            }
            
        }
    }

   
    
}