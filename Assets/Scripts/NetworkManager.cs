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

public class NetworkManager : MonoBehaviour, INetworkRunnerCallbacks
{
    public static NetworkRunner _runner;

    [SerializeField] private TMP_InputField _sessionNameInputField;
    [SerializeField] private TMP_Dropdown _sessionListDropdown;
    private Canvas _opponentLeftMessage;

    [SerializeField] private NetworkPrefabRef _whiteBallPrefab;
    private Dictionary<PlayerRef, NetworkObject> _spawnedCharacters = new Dictionary<PlayerRef, NetworkObject>();

    private CustomNetworkSceneManager _sceneManager;

    private CustomSceneLoader _sceneLoader;
    private Canvas _connectionLostMessage;
    
   

    public List<Player> playerObjects;


    private MyGameManager _gameManager;

    public Player playerPrefab;

    public static NetworkManager instance;
   

    private void Awake()
    {
        _runner = GetComponent<NetworkRunner>();
        DontDestroyOnLoad(gameObject);


        if (instance == null)
            instance = this;
    }


    // must be on a networkBehaviour
    //[Rpc(RpcSources.All, RpcTargets.All, HostMode = RpcHostMode.SourceIsHostPlayer)]
    //public void Rpc_LoadDone(RpcInfo info = default)
    //{
    //    Debug.Log("RPC");
    //}


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

        Player playerToSpawn;
        if (runner.IsServer)
        {
            playerToSpawn = runner.Spawn(playerPrefab);
            playerToSpawn.playerRef = player;
            playerObjects.Add(playerToSpawn);

        }



        if (runner.ActivePlayers.Count() == 2)
        {
            _sceneLoader.LoadGameScene();
        }



    }

    public PlayerRef GetLocalPlayerRef()
    {
        return _runner.LocalPlayer;
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
        Debug.Log("Scène chargée.");
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            if (runner.IsServer)
            {
                // Create a unique position for the player
                Vector3 spawnPosition = new Vector3(33f, 4.7f, 5.2f);
                //networkPlayerObjects[1] = runner.Spawn(_playerPrefab, spawnPosition, Quaternion.identity, runner.ActivePlayers.Last());
                //Player.IsHostTurn = true;
                runner.Spawn(_whiteBallPrefab, spawnPosition, Quaternion.identity, runner.ActivePlayers.First());



                // Keep track of the player avatars so we can remove it when they disconnect
                
                //_spawnedCharacters.Add(runner.ActivePlayers.Last(), networkPlayerObjects[1]);
               

               // _gameManager.AddPlayerRigidbodyToBallsList(.gameObject.GetComponent<Rigidbody>());

                Debug.Log(runner.ActivePlayers.First().PlayerId + " joined the game.");

            }
            else
            {
                
                // La balle a-t-elle eu le temps d'être spawnée quand cette méthode s'exécute chez le client ? Mettons un délai pour voir :
                StartCoroutine(WaitForPlayerToSpawn());
            }
            var canvas = GameObject.Find("TurnCanvas");

           
        }
    }


    IEnumerator WaitForPlayerToSpawn()
    {
        yield return new WaitForSeconds(2);
        //playableBall = FindAnyObjectByType<Player>();
        //networkPlayerObjects = FindObjectsByType<NetworkObject>(FindObjectsSortMode.InstanceID);
        //_gameManager.AddPlayerRigidbodyToBallsList(networkPlayerObjects[0].gameObject.GetComponent<Rigidbody>());
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
            Cursor.visible = true;
        }
    }




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
        Cursor.visible = true;
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

            Cursor.visible = true;
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

    public void OnInput(NetworkRunner runner, NetworkInput input) { }

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