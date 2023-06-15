using Fusion;
using UnityEngine;

public class Player : NetworkBehaviour
{
    // private NetworkCharacterControllerPrototype _cc;

    public static int playerId;

    [Networked(OnChanged = nameof(OnBallSpawned))]
    public static int playerTurn { get; set; }

    public static void OnBallSpawned(Changed<Player> changed)
    {
        if (playerId == playerTurn) Debug.Log("Its your turn");
        else Debug.Log("Its not your turn");

    }

    public void NextPlayerTurn()
    {
        if (playerTurn == 1) playerTurn = 2;
        else playerTurn = 1;
    }

    




    private Rigidbody _rb;

    private void Awake()
    {
      //  _cc = GetComponent<NetworkCharacterControllerPrototype>();

        _rb = GetComponent<Rigidbody>();
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