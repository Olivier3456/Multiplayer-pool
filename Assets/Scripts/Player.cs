using Fusion;
using UnityEngine;

public class Player : NetworkBehaviour
{
    private NetworkCharacterControllerPrototype _cc;

    

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

    [Rpc(sources: RpcSources.All, targets: RpcTargets.All)]
    public void RPC_SendMessage([RpcTarget] PlayerRef targetPlayer)
    {
        Debug.Log("Vous avez reçu un message");
    }


}