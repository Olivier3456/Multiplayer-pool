using ExitGames.Client.Photon.StructWrapping;
using Fusion;
using UnityEngine;

public class Player : NetworkBehaviour
{
    //[Networked]
    //public bool IsHostTurn { get; set; }

    private Rigidbody _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();        
    }



    public static bool IsHostTurn;
    [Networked(OnChanged = nameof(SwitchTurn))]
    public bool MyProperty { get; set; }

    protected static void SwitchTurn(Changed<Player> changed)
    {
        changed.LoadNew();
        IsHostTurn = changed.Behaviour.MyProperty;
        changed.LoadOld();
        var oldval = changed.Behaviour.MyProperty;
        Debug.Log($"IsHostTurn changed from {oldval} to {IsHostTurn}");
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