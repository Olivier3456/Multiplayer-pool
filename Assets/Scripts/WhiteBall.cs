using ExitGames.Client.Photon.StructWrapping;
using Fusion;
using UnityEngine;

public class WhiteBall : NetworkBehaviour
{
    private Rigidbody _rb;
    
    

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        //balls = FindObjectsByType<Player>(FindObjectsSortMode.InstanceID);
    }

    private void Start()
    {
        //_networkManager = FindAnyObjectByType<NetworkManager>();
    }


   


    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData data))
        {
            data.direction.Normalize();
           // _cc.Move(5 * data.direction * Runner.DeltaTime);

            _rb.AddForce(data.direction * 5);
        }
    }
}