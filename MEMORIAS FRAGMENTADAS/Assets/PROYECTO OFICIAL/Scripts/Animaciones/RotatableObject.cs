using UnityEngine;

public class RotatableObject : MonoBehaviour
{
    [Header("Configuración de rotación")]
    public float rotationSpeed = 120f;

    [Header("Estado")]
    public bool isBeingRotated = false;

    public void ToggleRotationMode()
    {
        isBeingRotated = !isBeingRotated;
        Debug.Log("Modo rotación: " + isBeingRotated);
    }

    void Update()
    {
        if (!isBeingRotated) return;

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // Giro horizontal sobre eje Y
        transform.Rotate(Vector3.up, -horizontal * rotationSpeed * Time.deltaTime, Space.World);

        // Giro vertical sobre eje X
        transform.Rotate(Vector3.right, vertical * rotationSpeed * Time.deltaTime, Space.World);
    }
}