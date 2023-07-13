using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BaseBall : NetworkBehaviour
{
    protected Rigidbody _rb;
    protected AudioSource _as;
    

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _as = GetComponent<AudioSource>();
    }

    protected virtual void OnEnable()
    {
        MyGameManager.instance.AddBallToBallsList(this);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.tag.Contains("ball"))
            _as.Play();
    }

}
