using ExitGames.Client.Photon.StructWrapping;
using Fusion;
using UnityEngine;

public class Player : NetworkBehaviour
{
    // private NetworkCharacterControllerPrototype _cc;

    static NetworkRunner _nr;


    
    [Networked]
    public bool IsHostTurn { get; set; }


    private void Start()
    {
    }

    /*public static void OnTurnChange(Changed<Player> changed)
    {
        if (_nr.IsServer == changed.Behaviour.IsHostTurn) changed.Behaviour.CanPlay = true;
        else changed.Behaviour.CanPlay = false;
    }*/

    public void NextPlayerTurn()
    {
        IsHostTurn = !IsHostTurn;
    }

    




    private Rigidbody _rb;

    private void Awake()
    {
      //  _cc = GetComponent<NetworkCharacterControllerPrototype>();

        _rb = GetComponent<Rigidbody>();
        _nr = FindAnyObjectByType<NetworkRunner>();
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