using ExitGames.Client.Photon.StructWrapping;
using Fusion;
using UnityEngine;

public class WhiteBall : BaseBall
{
    
       


    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData data))
        {
            data.direction.Normalize();
           // _cc.Move(5 * data.direction * Runner.DeltaTime);

            _rb.AddForce(data.direction * 5, ForceMode.Impulse);
        }
    }
}