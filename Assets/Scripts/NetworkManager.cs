using Fusion;
using Fusion.Sockets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;

public class NetworkManager : MonoBehaviour, INetworkRunnerCallbacks
{
    NetworkRunner _runner;

    [SerializeField] private TMP_InputField _sessionNameInputField;
    [SerializeField] private TMP_Dropdown _sessionListDropdown;

    [SerializeField] private NetworkPrefabRef _playerPrefab;
    private Dictionary<PlayerRef, NetworkObject> _spawnedCharacters = new Dictionary<PlayerRef, NetworkObject>();

    private void Awake()
    {
        _runner = GetComponent<NetworkRunner>();
        DontDestroyOnLoad(gameObject);
    }


     


    public async void HostGame()
    {
        var result = await _runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Host,
            PlayerCount = 2,
            CustomLobbyName = "MyLobby",
            SessionName = _sessionNameInputField.text
        }) ;
        DebugLogConnexion(result);
    }


    public void StartJoinLobbyTask()
    {
        Task t = JoinLobby();
    }

    public async Task JoinLobby()
    {

        var result = await _runner.JoinSessionLobby(SessionLobby.Custom, "MyCustomLobby");

        if (result.Ok)
        {
            // all good
        }
        else
        {
            Debug.LogError($"Failed to Start: {result.ShutdownReason}");
        }


        
    }





    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {

        Debug.Log($"Session List Updated with {sessionList.Count} session(s)");

              
        // Store the target session
        SessionInfo session = null;


        List<String> sessionNames = new List<String>();

        foreach (var sessionItem in sessionList)
        {
            sessionNames.Add(sessionItem.Name);
        }
        _sessionListDropdown.AddOptions(sessionNames);


        // Check if there is any valid session
        if (session != null)
        {
            Debug.Log($"Joining {session.Name}");

            // Join
            
        }
    }













    public async void JoinGame()
    {

        if (_sessionListDropdown.options.Count > 0)
        {

            var result = await _runner.StartGame(new StartGameArgs()
            {
                GameMode = GameMode.Client, // Client GameMode, could be Shared as well
                SessionName = _sessionListDropdown.value.ToString() // Session to Join                                                
            });

            DebugLogConnexion(result);
        }
        else
        {
            Debug.Log("Pas de bras, pas de chocolat");
        }


        //var result = await _runner.StartGame(new StartGameArgs()
        //{
        //    GameMode = GameMode.Client
        //});
        //DebugLogConnexion(result);
    }



    public async Task StartGame()
    {
        var result = await _runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.AutoHostOrClient,
            PlayerCount = 2
        });

        DebugLogConnexion(result);
    }


    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (runner.IsServer)
        {
            // Create a unique position for the player
            Vector3 spawnPosition = new Vector3((player.RawEncoded % runner.Config.Simulation.DefaultPlayers) * 3, 1, 0);
            NetworkObject networkPlayerObject = runner.Spawn(_playerPrefab, spawnPosition, Quaternion.identity, player);
            // Keep track of the player avatars so we can remove it when they disconnect
            _spawnedCharacters.Add(player, networkPlayerObject);

            Debug.Log(player.PlayerId);
        }
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        // Find and remove the players avatar
        if (_spawnedCharacters.TryGetValue(player, out NetworkObject networkObject))
        {
            runner.Despawn(networkObject);
            _spawnedCharacters.Remove(player);
        }
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        var data = new NetworkInputData();

        if (Input.GetKey(KeyCode.UpArrow))
            data.direction += Vector3.forward;

        if (Input.GetKey(KeyCode.DownArrow))
            data.direction += Vector3.back;

        if (Input.GetKey(KeyCode.LeftArrow))
            data.direction += Vector3.left;

        if (Input.GetKey(KeyCode.RightArrow))
            data.direction += Vector3.right;

        input.Set(data);


    }


    private void DebugLogConnexion(StartGameResult result)
    {
        if (result.Ok)
        {
            Debug.Log("Room joigned");

            if (_runner.GameMode == GameMode.Host)
            {
                Debug.Log("Vous h�bergez la partie");
            }
            else
            {
                Debug.Log(_runner.GameMode);
            }

        }
        else
        {
            Debug.LogError($"Failed to Start: {result.ShutdownReason}");
        }
    }


    //[Rpc(sources: RpcSources.All, targets: RpcTargets.All)]
    //public void RPC_SendMessage(string message)
    //{
    //    Debug.Log(message);
    //}

    //public void SendMessageStatic(string message)
    //{
    //    Rpc_StaticSendMessage(_runner, message);
    //}

    //[Rpc]
    //public static void Rpc_StaticSendMessage(NetworkRunner runner, string message)
    //{
    //    Debug.Log(message);
    //}




    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnDisconnectedFromServer(NetworkRunner runner) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }

    //public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }

}




