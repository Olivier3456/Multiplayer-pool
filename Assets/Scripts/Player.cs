using ExitGames.Client.Photon.StructWrapping;
using Fusion;
using UnityEngine;

public class Player : NetworkBehaviour
{
    //[Networked]
    //public bool IsHostTurn { get; set; }

    private Rigidbody _rb;
    private static Player[] balls;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        balls = FindObjectsByType<Player>(FindObjectsSortMode.InstanceID);
    }



    public static bool IsHostTurn;
    [Networked(OnChanged = nameof(SwitchTurn))]
    public bool MyProperty { get; set; }

    protected static void SwitchTurn(Changed<Player> changed)
    {
        //changed.LoadNew();
        //IsHostTurn = changed.Behaviour.MyProperty;
        //changed.LoadOld();
        //var oldval = changed.Behaviour.MyProperty;
        //Debug.Log($"IsHostTurn changed from {oldval} to {IsHostTurn}");
        NextPlayerTurn();
    }





    /*public static void OnTurnChange(Changed<Player> changed)
    {
        if (_nr.IsServer == changed.Behaviour.IsHostTurn) changed.Behaviour.CanPlay = true;
        else changed.Behaviour.CanPlay = false;
    }*/


    public static void NextPlayerTurn()
    {
        if (IsHostTurn)
        {
            balls[1].transform.position = balls[0].transform.position;
            balls[1].gameObject.SetActive(true);
            balls[0].gameObject.SetActive(false);
        }
        else
        {
            balls[0].transform.position = balls[1].transform.position;
            balls[0].gameObject.SetActive(true);
            balls[1].gameObject.SetActive(false);

        }
    }


    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData data))
        {
            data.direction.Normalize();
           // _cc.Move(5 * data.direction * Runner.DeltaTime);

            _rb.AddForce(data.direction);
        }
    }
}