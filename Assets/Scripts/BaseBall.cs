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

    protected virtual void Update()
    {
        print(_rb.velocity.magnitude);
        if(_rb.velocity.magnitude < 0.09)
        {
            _rb.velocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;            
        }
    }
}
