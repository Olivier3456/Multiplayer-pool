using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickManager : MonoBehaviour
{
    public WhiteBall target; // L'objet autour duquel la caméra va tourner
    private Transform targetTransform;
    float desiredHorizontalAngle;
    //float desiredVerticalAngle;
    private Vector3 baseOffset;
    private Vector3 dragOffset;

    void Start()
    {

    }

    void Update()
    {
        if(NetworkManager.instance.GetLocalPlayerRef() == MyGameManager.instance.playerPlaying && !MyGameManager.instance.playedThisTurn)
        {
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }


        if (target == null)
        {
            target = FindObjectOfType<WhiteBall>();
            if (target != null)
            {
                targetTransform = target.transform;
                baseOffset = transform.position - targetTransform.position;
            }
        }
        else 
        {
            if (target.draging)
            {
                dragOffset = target.dragDistance * transform.forward;
            }
            else
            {
                dragOffset = Vector3.zero;
            }

            desiredHorizontalAngle = Camera.main.transform.rotation.eulerAngles.y;

            Quaternion rotation = Quaternion.Euler(0, desiredHorizontalAngle, 0);
            transform.position = targetTransform.position + (rotation * (baseOffset + dragOffset));


            transform.rotation = Quaternion.LookRotation(targetTransform.position - Vector3.up - transform.position, Vector3.up);
        }
      
    }
}
