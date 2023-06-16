using UnityEngine;

public class CameraRotation : MonoBehaviour
{
    public Transform target; // L'objet autour duquel la cam�ra va tourner
    public float rotationSpeed = 5f; // La vitesse de rotation de la cam�ra
    float desiredHorizontalAngle;
    float desiredVerticalAngle;
    private Vector3 offset;

    void Start()
    {
        // Calcul de l'offset initial entre la cam�ra et l'objet
       
        Cursor.visible = false;


    }

    void LateUpdate()
    {
        if (target == null)
        {
            target = FindAnyObjectByType<Player>().transform;
            if (target != null) offset = transform.position - target.position;
        }
        else
        {
            // Obtention des mouvements de la souris
            float horizontal = Input.GetAxis("Mouse X") * rotationSpeed;
            float vertical = Input.GetAxis("Mouse Y") * rotationSpeed;
                        

            // Mise � jour de la position de la cam�ra en fonction de la rotation
            desiredHorizontalAngle += horizontal;
            desiredVerticalAngle -= vertical;
            Quaternion rotation = Quaternion.Euler(desiredVerticalAngle, desiredHorizontalAngle, 0);
            transform.position = target.position - (rotation * offset);


            // Faire en sorte que la cam�ra regarde toujours l'objet
            transform.LookAt(target);
        }
    }
}