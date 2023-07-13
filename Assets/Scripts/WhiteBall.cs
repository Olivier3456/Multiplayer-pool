using ExitGames.Client.Photon.StructWrapping;
using Fusion;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WhiteBall : BaseBall
{
    Player[] players;
    float hitForce = 10;


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
        _rb.AddForce(direction, ForceMode.Impulse);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        players = FindObjectsByType<Player>(FindObjectsSortMode.None);

        for (int i = 0; i < players.Length; i++)
        {
            players[i].whiteBall = this;
        }
    }

    protected override void Update()
    {
        base.Update();

        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            //Debug.Log("correct scene");

            if (MyGameManager.spawnedCalled)
            {
                //Debug.Log("Player : MyGameManager.spawnedCalled = true");

                if (NetworkManager.instance.GetLocalPlayerRef() == MyGameManager.instance.playerPlaying)
                {
                    if (Input.GetKeyDown(KeyCode.UpArrow))
                    {
                        //networkInput = input;
                        //Debug.Log("GetKeyDown(KeyCode.UpArrow)");

                        if (!MyGameManager.instance.playedThisTurn)
                        {
                            Rpc_BallHit(Vector3.ProjectOnPlane(Camera.main.transform.forward, Vector3.up) * hitForce);
                        }
                    }
                }
            }
        }
    }


    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void Rpc_BallHit(Vector3 direction)
    {
        Debug.Log("RPC");

        MyGameManager.instance.playedThisTurn = true;

        //var data = new NetworkInputData();

        //data.direction += _camera.transform.forward;

        //networkInput.Set(data);

        BallKicked(direction);

        StartCoroutine(MyGameManager.instance.CheckBallsMovementRepeatedly());

        Debug.Log("Methode WhiteBall/BallKicked appelée par Player/RpcBallHit");
    }
}