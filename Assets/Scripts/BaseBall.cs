using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseBall : NetworkBehaviour
{
    protected Rigidbody _rb;
    Vector3 lastFramePosition;
    public bool isStopped = false;

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
        //print(_rb.velocity.magnitude);
        if(Vector3.Distance(transform.position, lastFramePosition) < 0.1 * Time.deltaTime)
        {
            _rb.velocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
            isStopped = true;
        }
        else
        {
            isStopped = false;
        }
    }
}
