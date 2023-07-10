using Fusion;
using UnityEngine;

public class CameraRotation : MonoBehaviour
{
    public Transform target; // L'objet autour duquel la caméra va tourner
    public float rotationSpeed = 5f; // La vitesse de rotation de la caméra
    float desiredHorizontalAngle;
    float desiredVerticalAngle;
    private Vector3 offset;
    private NetworkRunner runner;

    void Start()
    {
        // Calcul de l'offset initial entre la caméra et l'objet
       
        Cursor.visible = false;
        runner = FindObjectOfType<NetworkRunner>();

    }

    void LateUpdate()
    {
        if (target == null)
        {
            target = FindObjectOfType<WhiteBall>().transform;
            if (target != null) offset = transform.position - target.position;
        }
        else
        {
            // Obtention des mouvements de la souris
            float horizontal = Input.GetAxis("Mouse X") * rotationSpeed;
            float vertical = Input.GetAxis("Mouse Y") * rotationSpeed;
            Mathf.Clamp(horizontal, -30, 30);

            // Mise à jour de la position de la caméra en fonction de la rotation
            desiredHorizontalAngle += horizontal;
            desiredVerticalAngle -= vertical;
            Quaternion rotation = Quaternion.Euler(desiredVerticalAngle, desiredHorizontalAngle, 0);
            transform.position = target.position - (rotation * offset);


            // Faire en sorte que la caméra regarde toujours l'objet
            transform.LookAt(target);
        }
    }
}
