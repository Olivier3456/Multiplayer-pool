using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AimLineManager : MonoBehaviour
{
    [SerializeField] LineRenderer _firstLineRenderer;
    [SerializeField] LineRenderer _reboundLineRenderer;
    
    Camera _mainCamera => Camera.main;
    Transform _whiteBall => MyGameManager.instance.whiteBall.transform;

    public GameObject cylinder;

    public bool showLine;
    // Start is called before the first frame update


    // Update is called once per frame
    void Update()
    {
        if (showLine)
        {
            Debug.Log("Direction : " + Vector3.ProjectOnPlane(_whiteBall.position - _mainCamera.transform.position, Vector3.up));
            Physics.Raycast(_whiteBall.position, Vector3.ProjectOnPlane(_whiteBall.position - _mainCamera.transform.position, Vector3.up), out var hit);
            cylinder.transform.position = hit.point;
            _firstLineRenderer.positionCount = 2;
            _firstLineRenderer.transform.position = _whiteBall.position;
            _firstLineRenderer.SetPositions(new Vector3[2] { Vector3.zero, hit.point - _firstLineRenderer.transform.position });
            _reboundLineRenderer.transform.position = hit.point;
            _reboundLineRenderer.positionCount = 2;
            Vector3 newDirection = 2 * Vector3.Dot(hit.normal , hit.point - _firstLineRenderer.transform.position)* hit.normal - hit.point - _firstLineRenderer.transform.position;
            _reboundLineRenderer.SetPositions(new Vector3[2] { Vector3.zero, newDirection.normalized * 3 });
            _firstLineRenderer.enabled = true;
        }
        else
        { 
            enabled = false;
        }
    }
}
