using Fusion;
using UnityEngine;

public class CameraRotation : MonoBehaviour
{
    public WhiteBall target; // L'objet autour duquel la cam�ra va tourner
    private Transform targetTransform;
    public float rotationSpeed = 5f; // La vitesse de rotation de la cam�ra
    float desiredHorizontalAngle;
    float desiredVerticalAngle;
    private Vector3 offset;

    void Start()
    {
        Cursor.visible = false;
    }

    void LateUpdate()
    {
        if (target == null)
        {
            target = FindObjectOfType<WhiteBall>();
            if (target != null)
            {
                targetTransform = target.transform;
                offset = transform.position - targetTransform.position;
            }
        }
        else if(!target.draging)
        {
            // Obtention des mouvements de la souris
            float horizontal = Input.GetAxis("Mouse X") * rotationSpeed;
            float vertical = Input.GetAxis("Mouse Y") * rotationSpeed;

            // Mise � jour de la position de la cam�ra en fonction de la rotation
            desiredHorizontalAngle += horizontal;
            desiredVerticalAngle -= vertical;
            Mathf.Clamp(desiredVerticalAngle, -30, 30);
            Quaternion rotation = Quaternion.Euler(desiredVerticalAngle, desiredHorizontalAngle, 0);
            transform.position = targetTransform.position - (rotation * offset);


            // Faire en sorte que la cam�ra regarde toujours l'objet
            transform.LookAt(targetTransform);
        }
    }
}
