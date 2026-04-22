using UnityEngine;

public static class InputManagerCustom
{
    // X arriba
    public static bool PressX()
    {
        return Input.GetKeyDown(KeyCode.X) ||
               Input.GetKeyDown(KeyCode.JoystickButton3);
    }

    // Y derecha
    public static bool PressY()
    {
        return Input.GetKeyDown(KeyCode.Y) ||
               Input.GetKeyDown(KeyCode.JoystickButton4) ||
               Input.GetKeyDown(KeyCode.JoystickButton1) ||
               Input.GetKeyDown(KeyCode.JoystickButton2);
    }

    // B abajo
    public static bool PressB()
    {
        return Input.GetKeyDown(KeyCode.B) ||
               Input.GetKeyDown(KeyCode.JoystickButton7);
    }

    // A izquierda
    public static bool PressA()
    {
        return Input.GetKeyDown(KeyCode.A) ||
               Input.GetKeyDown(KeyCode.JoystickButton11);
    }

    // Gatillo
    public static bool PressTrigger()
    {
#if UNITY_ANDROID
        return Input.GetKeyDown(KeyCode.JoystickButton0) ||
               Input.GetKeyDown(KeyCode.JoystickButton9) ||
               Google.XR.Cardboard.Api.IsTriggerPressed;
#else
        return Input.GetKeyDown(KeyCode.JoystickButton0) ||
               Input.GetKeyDown(KeyCode.JoystickButton9);
#endif
    }
}