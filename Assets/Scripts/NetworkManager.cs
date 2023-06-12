using Fusion;
using Fusion.Sockets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class NetworkManager : MonoBehaviour, INetworkRunnerCallbacks
{
    NetworkRunner _runner;

    

    [SerializeField] private NetworkPrefabRef _playerPrefab;
    private Dictionary<PlayerRef, NetworkObject> _spawnedCharacters = new Dictionary<PlayerRef, NetworkObject>();

    private void Awake()
    {
        _runner = GetComponent<NetworkRunner>();
        DontDestroyOnLoad(gameObject);
    }



    public async void CreateOrJoinGame()
    {
       await StartGame();  // Pour commencer.
    }

    
    public async Task StartGame()
    {
        var result = await _runner.StartGame(new StartGameArgs()
        {
            
            GameMode = GameMode.AutoHostOrClient,
            PlayerCount = 2
        }) ;

        if (result.Ok)
        {
            Debug.Log("Room joigned");

            if (_runner.GameMode == GameMode.Host)
            {
                Debug.Log("Vous hébergez la partie");
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

        

    [Rpc(sources: RpcSources.All, targets: RpcTargets.All)]
    public void RPC_SendMessage(string message)
    {
        Debug.Log(message);
    }



    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnDisconnectedFromServer(NetworkRunner runner) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }

}




