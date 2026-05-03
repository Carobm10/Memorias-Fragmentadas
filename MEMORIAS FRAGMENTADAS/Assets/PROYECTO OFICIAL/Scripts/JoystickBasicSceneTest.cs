using UnityEngine;
using TMPro;

/// <summary>
/// Script de prueba básica para la escena Test_Joystick.
/// 
/// Objetivo:
/// Confirmar que InputManagerCustom está leyendo correctamente:
/// X, A, Y, B y Gatillo.
/// 
/// Este script NO controla puertas, NPC ni misiones.
/// Solo sirve para validar botones antes de volver al juego real.
/// </summary>
public class JoystickBasicSceneTest : MonoBehaviour
{
    [Header("Texto 3D donde se mostrará el resultado")]
    public TMP_Text debugText;

    private string ultimoBoton = "Ninguno todavía";

    void Update()
    {
        // Detectamos cada botón usando el InputManagerCustom.
        if (InputManagerCustom.PressX())
        {
            ultimoBoton = "X detectado correctamente";
            Debug.Log("X detectado");
        }

        if (InputManagerCustom.PressA())
        {
            ultimoBoton = "A detectado correctamente";
            Debug.Log("A detectado");
        }

        if (InputManagerCustom.PressY())
        {
            ultimoBoton = "Y detectado correctamente";
            Debug.Log("Y detectado");
        }

        if (InputManagerCustom.PressB())
        {
            ultimoBoton = "B detectado correctamente";
            Debug.Log("B detectado");
        }

        if (InputManagerCustom.PressTrigger())
        {
            ultimoBoton = "Gatillo / OK detectado correctamente";
            Debug.Log("Gatillo detectado");
        }

        // Actualizamos el texto visible en pantalla.
        debugText.text =
            "PRUEBA BASICA DE BOTONES\n\n" +
            "Presiona un boton del joystick:\n\n" +
            "X = salir / confirmar\n" +
            "A = opcion secundaria\n" +
            "Y = hablar / opcion principal\n" +
            "B = abrir puertas/cajones\n" +
            "Gatillo OK = accion alternativa\n\n" +
            "Ultimo boton:\n" +
            ultimoBoton;
    }
}