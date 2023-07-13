using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BaseBall : NetworkBehaviour
{
    protected Rigidbody _rb;

    

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();        
    }

    protected virtual void OnEnable()
    {
        MyGameManager.instance.AddBallToBallsList(this);
    }

}
