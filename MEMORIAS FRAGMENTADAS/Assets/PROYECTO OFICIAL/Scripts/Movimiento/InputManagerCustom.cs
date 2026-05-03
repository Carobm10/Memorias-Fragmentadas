using UnityEngine;

/// <summary>
/// InputManagerCustom centraliza todos los controles del proyecto MEMORIAS FRAGMENTADAS.
/// 
/// Este script permite que el mismo juego funcione con:
/// - Teclado en Unity Editor.
/// - Joystick físico en Android/Cardboard.
/// 
/// IMPORTANTE:
/// Este script NO ejecuta acciones directamente.
/// Solo responde si un botón fue presionado.
/// 
/// Mapeo real probado en APK:
/// - X = JoystickButton2
/// - A = JoystickButton10
/// - Y = JoystickButton3
/// - B = JoystickButton5
/// - Gatillo/OK = JoystickButton0 o JoystickButton7
/// 
/// Regla oficial del proyecto:
/// - B = interactuar, abrir/cerrar puertas, cajones, seleccionar ropa.
/// - X = salir, cerrar canvas, cancelar.
/// - A = activar diálogo, iniciar misión.
/// - Y = opción adicional o tercera opción si se necesita.
/// </summary>
public static class InputManagerCustom
{
    /// <summary>
    /// Botón X.
    /// Uso oficial:
    /// - Salir de diálogos.
    /// - Cerrar canvas.
    /// - Cancelar acciones.
    /// 
    /// En teclado responde con X.
    /// En joystick responde con JoystickButton2.
    /// </summary>
    public static bool PressX()
    {
        return Input.GetKeyDown(KeyCode.X) ||
               Input.GetKeyDown(KeyCode.JoystickButton2);
    }

    /// <summary>
    /// Botón A.
    /// Uso oficial:
    /// - Activar diálogo con NPC.
    /// - Iniciar misión.
    /// - Opción 1 en diálogos si aplica.
    /// 
    /// En teclado responde con A.
    /// En joystick responde con JoystickButton10.
    /// </summary>
    public static bool PressA()
    {
        return Input.GetKeyDown(KeyCode.A) ||
               Input.GetKeyDown(KeyCode.JoystickButton10);
    }

    /// <summary>
    /// Botón Y.
    /// Uso oficial:
    /// - Opción adicional en diálogos.
    /// - Opción 2 o 3 según el caso.
    /// 
    /// En teclado responde con Y.
    /// En joystick responde con JoystickButton3.
    /// </summary>
    public static bool PressY()
    {
        return Input.GetKeyDown(KeyCode.Y) ||
               Input.GetKeyDown(KeyCode.JoystickButton3);
    }

    /// <summary>
    /// Botón B.
    /// Uso oficial:
    /// - Interactuar con objetos.
    /// - Abrir/cerrar puertas.
    /// - Abrir/cerrar cajones.
    /// - Seleccionar ropa.
    /// - Activar objetos simples.
    /// 
    /// En teclado responde con B.
    /// En joystick responde con JoystickButton5.
    /// </summary>
    public static bool PressB()
    {
        return Input.GetKeyDown(KeyCode.B) ||
               Input.GetKeyDown(KeyCode.JoystickButton5);
    }

    /// <summary>
    /// Gatillo frontal / botón OK del joystick.
    /// 
    /// En la prueba de APK apareció como:
    /// - JoystickButton0
    /// - JoystickButton7
    /// 
    /// Por ahora queda disponible como botón auxiliar.
    /// No lo usamos como botón principal para no mezclar acciones.
    /// </summary>
    public static bool PressTrigger()
    {
        return Input.GetKeyDown(KeyCode.JoystickButton0) ||
               Input.GetKeyDown(KeyCode.JoystickButton7);
    }
}