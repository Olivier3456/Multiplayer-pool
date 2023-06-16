using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyGameManager : MonoBehaviour
{
    [SerializeField] private BoxCollider[] holes;
    private int redScore;
    private int yellowScore;
    private bool redIsPlaying;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "red")
        {
            redScore++;
        }
        if(other.tag == "yellow")
        {
            yellowScore++;
        }
        if (other.tag == "black")
        {
            if(redScore==7 && redIsPlaying)
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
