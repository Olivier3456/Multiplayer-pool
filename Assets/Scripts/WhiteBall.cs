using ExitGames.Client.Photon.StructWrapping;
using Fusion;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        _rb.AddForce(direction * 15, ForceMode.Impulse);
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
                    //Debug.Log("NetworkManager.instance.GetLocalPlayerRef() == MyGameManager.instance.playerPlaying");
                    //if ((Player.IsHostTurn && runner.IsServer)
                    //|| (!Player.IsHostTurn && runner.IsClient))
                    {
                        if (Input.GetKeyDown(KeyCode.UpArrow))
                        {
                            //networkInput = input;
                            //Debug.Log("GetKeyDown(KeyCode.UpArrow)");

                            if (MyGameManager.checkCoroutine == null)
                            {
                                Rpc_BallHit();
                            }
                        }
                    }
                }

            }
        }
    }


    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void Rpc_BallHit(RpcInfo info = default)
    {
        Debug.Log("RPC");

        //var data = new NetworkInputData();

        //data.direction += _camera.transform.forward;

        //networkInput.Set(data);

        BallKicked(Camera.main.transform.forward);

        MyGameManager.checkCoroutine = StartCoroutine(MyGameManager.instance.CheckBallsMovementRepeatedly());

        Debug.Log("Methode WhiteBall/BallKicked appelée par Player/RpcBallHit");
    }
}