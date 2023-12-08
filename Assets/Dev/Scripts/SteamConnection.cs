using System;
using System.Linq;
using Dev.Utils;
using Steamworks;
using Steamworks.Data;
using UnityEngine;

namespace Dev.Scripts
{
    public class SteamConnection : MonoBehaviour
    {
        [SerializeField] private string _lobbyName = "TestingS";
    
        private void Awake()
        {
            try
            {
                SteamClient.Init(480);
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