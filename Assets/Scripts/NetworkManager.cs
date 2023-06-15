using Fusion;
using Fusion.Sockets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.PlasticSCM.Editor.WebApi;

public class NetworkManager : MonoBehaviour, INetworkRunnerCallbacks
{
    static NetworkRunner _runner;

    [SerializeField] private TMP_InputField _sessionNameInputField;
    [SerializeField] private TMP_Dropdown _sessionListDropdown;
    private Canvas _opponentLeftMessage;

    [SerializeField] private NetworkPrefabRef _playerPrefab;
    private Dictionary<PlayerRef, NetworkObject> _spawnedCharacters = new Dictionary<PlayerRef, NetworkObject>();

    private CustomNetworkSceneManager _sceneManager;

    private CustomSceneLoader _sceneLoader;
    private Canvas _connectionLostMessage;
    NetworkObject networkPlayerObject;



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
            CustomLobbyName = "MyCustomLobby",
            SessionName = _sessionNameInputField.text,
            SceneManager = gameObject.AddComponent<CustomSceneLoader>()
        });
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

        _sessionListDropdown.ClearOptions();

        List<String> sessionNames = new List<String>();
        foreach (var sessionItem in sessionList)
        {
            sessionNames.Add(sessionItem.Name);
        }
        _sessionListDropdown.AddOptions(sessionNames);
    }





    public async void JoinGame()
    {
        if (_sessionListDropdown.options.Count > 0)
        {
            Debug.Log("Session rejointe : " + _sessionListDropdown.options[_sessionListDropdown.value].text);

            var result = await _runner.StartGame(new StartGameArgs()
            {
                GameMode = GameMode.Client, // Client GameMode, could be Shared as well
                SessionName = _sessionListDropdown.options[_sessionListDropdown.value].text, // Session to Join
                SceneManager = gameObject.AddComponent<CustomSceneLoader>()
            });



            DebugLogConnexion(result);

        }
        else
        {
            Debug.Log("Pas de bras, pas de chocolat");
        }
    }




    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        _sceneLoader = GetComponent<CustomSceneLoader>();

        if (runner.ActivePlayers.Count() == 2)
        {
            _sceneLoader.LoadGameScene();
        }

        //if (runner.IsServer)
        //{
        //    // Create a unique position for the player
        //    Vector3 spawnPosition = new Vector3((player.RawEncoded % runner.Config.Simulation.DefaultPlayers) * 3, 1, 0);
        //    NetworkObject networkPlayerObject = runner.Spawn(_playerPrefab, spawnPosition, Quaternion.identity, player);
        //    // Keep track of the player avatars so we can remove it when they disconnect
        //    _spawnedCharacters.Add(player, networkPlayerObject);

        //    Debug.Log(player.PlayerId);
        //}
    }


    public void OnSceneLoadDone(NetworkRunner runner)
    {
        Debug.Log("Scène chargée.");

        if (runner.IsServer && SceneManager.GetActiveScene().buildIndex == 1)
        {

            // Create a unique position for the player
            // Vector3 spawnPosition = new Vector3((runner.ActivePlayers.First().RawEncoded % runner.Config.Simulation.DefaultPlayers) * 3, 1, 0);
            // NetworkObject networkPlayerObject = runner.Spawn(_playerPrefab, spawnPosition, Quaternion.identity, runner.ActivePlayers.First());

             networkPlayerObject = GameObject.Find("Notre Player Prefab").GetComponent<NetworkObject>();

            // Keep track of the player avatars so we can remove it when they disconnect
            _spawnedCharacters.Add(runner.ActivePlayers.First(), networkPlayerObject);

            Debug.Log(runner.ActivePlayers.First().PlayerId + " joined the game.");

        }
    }


    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        // Find and remove the players avatar
        if (_spawnedCharacters.TryGetValue(player, out NetworkObject networkObject))
        {
            runner.Despawn(networkObject);
            _spawnedCharacters.Remove(player);
            _opponentLeftMessage = GameObject.FindObjectsOfType<Canvas>(true).Where(go => go.name == "ClientDisconnectedCanvas").First();
            _opponentLeftMessage.gameObject.SetActive(true);
            _opponentLeftMessage.GetComponentInChildren<Button>().onClick.RemoveAllListeners();
            _opponentLeftMessage.GetComponentInChildren<Button>().onClick.AddListener(_sceneLoader.LoadLobbyScene);
        }
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        var data = new NetworkInputData();

        if (SceneManager.GetActiveScene().buildIndex == 1)
        {

            if (networkPlayerObject.GetComponent<Player>().CanPlay)
            {
                if (Input.GetKey(KeyCode.UpArrow))
                    data.direction += Vector3.forward;

                if (Input.GetKey(KeyCode.DownArrow))
                    data.direction += Vector3.back;

                if (Input.GetKey(KeyCode.LeftArrow))
                    data.direction += Vector3.left;

                if (Input.GetKey(KeyCode.RightArrow))
                    data.direction += Vector3.right;
            }

            if (Input.GetKeyDown(KeyCode.M)) networkPlayerObject.GetComponent<Player>().NextPlayerTurn();

        }
        input.Set(data);
    }





    public void SendMessageStatic(string message)
    {
        Rpc_StaticSendMessage(_runner, message);
    }

    [Rpc]
    public static void Rpc_StaticSendMessage(NetworkRunner runner, string message)
    {
        Debug.Log(message);
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

    public void DestroyNetworkRunnerAndReloadMenuScene()
    {
        Destroy(GetComponent<NetworkRunner>());
        SceneManager.LoadScene(0);
    }


    public void OnDisconnectedFromServer(NetworkRunner runner)
    {
        Debug.Log("connection lost");
        _connectionLostMessage = GameObject.FindObjectsOfType<Canvas>(true).Where(go => go.name == "ClientLostConnectionCanvas").First();
        _connectionLostMessage.gameObject.SetActive(true);
        _connectionLostMessage.GetComponentInChildren<Button>().onClick.RemoveAllListeners();
        _connectionLostMessage.GetComponentInChildren<Button>().onClick.AddListener(() =>
        {
            SceneManager.LoadScene(0);
        });
    }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            Debug.Log("shut");
            _connectionLostMessage = GameObject.FindObjectsOfType<Canvas>(true).Where(go => go.name == "ClientLostConnectionCanvas").First();
            _connectionLostMessage.gameObject.SetActive(true);
            _connectionLostMessage.GetComponentInChildren<Button>().onClick.RemoveAllListeners();
            _connectionLostMessage.GetComponentInChildren<Button>().onClick.AddListener(() =>
            {
                SceneManager.LoadScene(0);
            });
        }
    }



    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }


    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data) { }

    public void OnSceneLoadStart(NetworkRunner runner) { Debug.Log("Chargement de la scène..."); }





    private void DebugLogConnexion(StartGameResult result)
    {
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
}




