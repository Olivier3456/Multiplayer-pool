using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoleBehaviour : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        MyGameManager.instance.BallInAHole(other.gameObject);
    }
}
