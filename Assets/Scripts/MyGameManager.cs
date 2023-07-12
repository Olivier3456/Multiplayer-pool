using Fusion;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class MyGameManager : NetworkBehaviour
{
    [SerializeField] private BoxCollider[] holes;
    private int redScore;
    private int yellowScore;
    private bool redIsPlaying;

    public List<BaseBall> balls = new List<BaseBall>();

    public static MyGameManager instance;

    [Networked(OnChanged = nameof(OnTurnChange))] public PlayerRef playerPlaying { get; set; }



    public static bool spawnedCalled = false;


    private void Awake()
    {
        if (instance == null)
        instance = this;

        DontDestroyOnLoad(gameObject);
    }

   
    public override void Spawned()
    {
        Debug.Log("La méthode Spawned de MyGameManager a été appelée.");
        base.Spawned();
        spawnedCalled = true;
    }



    public void AddBallToBallsList(BaseBall baseBall)
    {
        balls.Add(baseBall);
    }

    public bool CheckIfAllRigidbodiesAreSleeping()
    {
        Debug.Log("Checking if sleeping");
        bool result = true;
        for (int i = 0; i < balls.Count; i++)
        {

            if (balls[i].GetComponent<Rigidbody>().velocity.magnitude > 0.2)
            {
                result = false;
            }
        }
        return result;
    }

    public static Coroutine checkCoroutine;


    public IEnumerator CheckBallsMovementRepeatedly()
    {
        Debug.Log("Checking movement repeatedly");
        WaitForSeconds waitForSeconds = new WaitForSeconds(0.5f);
        do
        {
            yield return waitForSeconds;
        } while (!CheckIfAllRigidbodiesAreSleeping());

        for (int i = 0; i < NetworkManager._runner.ActivePlayers.Count(); ++i)
        {
            Debug.Log("Player n " + i + ": " + NetworkManager._runner.ActivePlayers.ElementAt(i));
        }
        Debug.Log("playerPlaying: " + playerPlaying);
        if (playerPlaying == NetworkManager._runner.ActivePlayers.First())
        {
            
            playerPlaying = NetworkManager._runner.ActivePlayers.Last();
        }
        else
        {
            playerPlaying = NetworkManager._runner.ActivePlayers.First();
        }

        yield return null;
    }


    private void OnTriggerEnter(Collider other)
    {
        balls.Remove(other.gameObject.GetComponent<BaseBall>());

        if (other.tag == "red")
        {
            redScore++;
        }
        if (other.tag == "yellow")
        {
            yellowScore++;
        }
        if (other.tag == "black")
        {
            if (redScore == 7 && redIsPlaying)
            {
                //win
            }
        }
        if (other.tag == "white")
        {
            //fault
        }
    }


    static void OnTurnChange(Changed<MyGameManager> changed)
    {
        var canvas = GameObject.Find("TurnCanvas");
        
        //Player ourPlayer = FindAnyObjectByType<Player>();


        if (NetworkManager.instance.GetLocalPlayerRef() == changed.Behaviour.playerPlaying)
        {
            //changed.Behaviour.networkPlayerObjects[0].AssignInputAuthority(_runner.ActivePlayers.Last());
            //if (_runner.IsServer)
            canvas.GetComponentInChildren<TextMeshProUGUI>().text = "it's your turn";            
        }
        else
        {
            canvas.GetComponentInChildren<TextMeshProUGUI>().text = "it's your opponent's turn";            
        }
    }
}
