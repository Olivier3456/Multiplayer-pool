using Fusion;
using Fusion.Sockets;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : NetworkBehaviour, INetworkRunnerCallbacks
{
    public int playerId;

    public bool canPlay;

    private Camera _camera;

    [SerializeField] NetworkRunner runner;
    [SerializeField] NetworkManager networkManager;


    private void Start()
    {
        _camera = Camera.main;
    }

    Coroutine checkCoroutine;

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        Debug.Log("there was an input");
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            Debug.Log("correct scene");
            if (canPlay)
            {
                Debug.Log("playableBall is not null");
                //if ((Player.IsHostTurn && runner.IsServer)
                //|| (!Player.IsHostTurn && runner.IsClient))
                {
                    var data = new NetworkInputData();

                    if (Input.GetKey(KeyCode.UpArrow))
                        data.direction += _camera.transform.forward;

                    if (Input.GetKey(KeyCode.DownArrow))
                        data.direction -= _camera.transform.forward;

                    if (Input.GetKey(KeyCode.LeftArrow))
                        data.direction -= _camera.transform.right;

                    if (Input.GetKey(KeyCode.RightArrow))
                        data.direction += _camera.transform.right;

                    if (Input.GetKey(KeyCode.Space))
                    {
                        if (checkCoroutine == null)
                        checkCoroutine = StartCoroutine(networkManager.CheckBallsMovementRepeatively());
                    }

                    input.Set(data);

                    Debug.Log("data sent " + data);
                }
            }
        }
    }







    public void OnConnectedToServer(NetworkRunner runner)
    {

    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {

    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {

    }

    public void OnDisconnectedFromServer(NetworkRunner runner)
    {

    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {

    }


    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
     
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
    
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
     
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data)
    {
     
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
     
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
     
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
     
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
     
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
     
    }
}
