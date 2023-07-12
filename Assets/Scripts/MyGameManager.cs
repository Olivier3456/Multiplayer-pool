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


    private void Awake()
    {
        if (instance == null)
        instance = this;

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        Spawned();
    }

    public override void Spawned()
    {
        Debug.Log("La m�thode Spawned de MyGameManager a �t� appel�e.");
        base.Spawned();
    }



    public void AddBallToBallsList(BaseBall baseBall)
    {
        balls.Add(baseBall);
    }

    public bool CheckIfAllRigidbodiesAreSleeping()
    {
        bool result = true;
        for (int i = 0; i < balls.Count; i++)
        {
            if (!balls[i].GetComponent<Rigidbody>().IsSleeping())
            {
                result = false;
            }
        }
        return result;
    }

    public IEnumerator CheckBallsMovementRepeatively()
    {
        WaitForSeconds waitForSeconds = new WaitForSeconds(0.5f);

        while (!CheckIfAllRigidbodiesAreSleeping())
        {
            yield return waitForSeconds;
        }

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
