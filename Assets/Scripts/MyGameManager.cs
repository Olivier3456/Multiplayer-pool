using System.Collections.Generic;
using UnityEngine;

public class MyGameManager : MonoBehaviour
{
    [SerializeField] private BoxCollider[] holes;
    private int redScore;
    private int yellowScore;
    private bool redIsPlaying;

    public List<Rigidbody> ballRigidbodies = new List<Rigidbody>();



    public void AddPlayerRigidbodyToBallsList(Rigidbody playerRigidbody)
    {
        ballRigidbodies.Add(playerRigidbody);
    }

    public bool CheckIfAllRigidbodiesAreSleeping()
    {
        bool result = true;
        for (int i = 0; i < ballRigidbodies.Count; i++)
        {
            if (!ballRigidbodies[i].IsSleeping())
            {
                result = false;
            }
        }
        return result;
    }


    private void OnTriggerEnter(Collider other)
    {
        ballRigidbodies.Remove(other.gameObject.GetComponent<Rigidbody>());

        if (other.tag == "red")
        {
            redScore++;
        }
        if (other.tag == "yellow")
        {
            yellowScore++;
        }
        if (other.tag == "black")
        {
            if (redScore == 7 && redIsPlaying)
            {
                //win
            }
        }
        if (other.tag == "white")
        {
            //fault
        }
    }
}
