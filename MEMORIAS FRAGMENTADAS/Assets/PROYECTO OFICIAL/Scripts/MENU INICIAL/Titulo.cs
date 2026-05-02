using UnityEngine;

public class Titulo : MonoBehaviour
{
    void Update()
    {
        if (Camera.main != null)
        {
            Vector3 direccion = Camera.main.transform.position - transform.position;
            direccion.y = 0; // evita inclinación

            transform.rotation = Quaternion.LookRotation(direccion) * Quaternion.Euler(0, 180f, 0);
        }
    }
}