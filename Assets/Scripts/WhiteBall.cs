using ExitGames.Client.Photon.StructWrapping;
using Fusion;
using System.Collections.Generic;
using UnityEngine;

public class WhiteBall : BaseBall
{
    Player[] players;


    //public override void FixedUpdateNetwork()
    //{
    //    //if (GetInput(out NetworkInputData data))
    //    //{
    //    //    data.direction.Normalize();
    //    //   // _cc.Move(5 * data.direction * Runner.DeltaTime);

    //    //    _rb.AddForce(data.direction * 5, ForceMode.Impulse);
    //    //}
    //
    //}

    public void BallKicked(Vector3 direction)
    {
        direction.Normalize();
        _rb.AddForce(direction * 5, ForceMode.Impulse);
    }

    private void OnEnable()
    {
        players = FindObjectsByType<Player>(FindObjectsSortMode.None);

        for (int i = 0; i < players.Length; i++)
        {
            players[i].whiteBall = this;
        }
    }
}