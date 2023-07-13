using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoleBehaviour : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Contains("Ball"))
        {
            MyGameManager.instance.BallInAHole(other.gameObject);
            Debug.Log(other.gameObject.name + " est tombée dans un trou.");
        }
    }
}
