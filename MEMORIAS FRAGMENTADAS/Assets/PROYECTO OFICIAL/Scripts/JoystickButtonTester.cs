using UnityEngine;
using TMPro;

public class JoystickButtonTester : MonoBehaviour
{
    public TMP_Text debugText;

    void Update()
    {
        string texto = "PRUEBA DE JOYSTICK\n\n";
        texto += "Presiona X, A, Y, B y gatillo\n\n";
        texto += "Botones detectados:\n";

        bool alguno = false;

        for (int i = 0; i <= 19; i++)
        {
            KeyCode boton = (KeyCode)System.Enum.Parse(typeof(KeyCode), "JoystickButton" + i);

            if (Input.GetKey(boton))
            {
                texto += "JoystickButton" + i + "\n";
                alguno = true;
            }

            if (Input.GetKeyDown(boton))
            {
                Debug.Log("DOWN: JoystickButton" + i);
            }
        }

        if (!alguno)
        {
            texto += "Ninguno presionado todavía";
        }

        debugText.text = texto;
    }
}