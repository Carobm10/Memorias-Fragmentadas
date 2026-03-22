using UnityEngine;

public class DetectarBotones : MonoBehaviour
{
    void Update()
    {
        // Detecta botones tipo joystick (0–19)
        for (int i = 0; i < 20; i++)
        {
            if (Input.GetKeyDown("joystick button " + i))
            {
                Debug.Log("Botón presionado: joystick button " + i);
            }
        }

        // Detecta ejes (joystick)
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        if (h != 0 || v != 0)
        {
            Debug.Log("Joystick movimiento: H=" + h + " V=" + v);
        }
    }
}