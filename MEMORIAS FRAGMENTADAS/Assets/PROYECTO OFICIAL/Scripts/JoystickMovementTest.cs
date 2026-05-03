using UnityEngine;

/// <summary>
/// Prueba básica de movimiento con joystick en la escena Test_Joystick.
/// 
/// Objetivo:
/// Verificar si el joystick también está enviando los ejes de movimiento
/// Horizontal y Vertical en Android.
/// 
/// Este script solo mueve un cubo de prueba.
/// No reemplaza MovimientoVR2 todavía.
/// </summary>
public class JoystickMovementTest : MonoBehaviour
{
    [Header("Velocidad de movimiento del cubo")]
    public float speed = 2f;

    void Update()
    {
        // Ejes clásicos del Input Manager antiguo de Unity.
        // En teclado suelen ser WASD/flechas.
        // En joystick Android deberían responder al stick.
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(horizontal, 0f, vertical);

        transform.Translate(movement * speed * Time.deltaTime, Space.World);
    }
}