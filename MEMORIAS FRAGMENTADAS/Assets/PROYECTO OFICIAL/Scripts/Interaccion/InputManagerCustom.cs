using UnityEngine;

public static class InputManagerCustom
{
    // X arriba (en Android puede ser JoystickButton2 o JoystickButton3)
    public static bool PressX()
    {
        return Input.GetKeyDown(KeyCode.X) ||
               Input.GetKeyDown(KeyCode.JoystickButton3) ||
               Input.GetKeyDown(KeyCode.JoystickButton2) ||  // Alternativa para Android
               CheckAndroidButton("x");
    }

    // Y derecha (en Android puede ser JoystickButton1 o JoystickButton4)
    public static bool PressY()
    {
        return Input.GetKeyDown(KeyCode.Y) ||
               Input.GetKeyDown(KeyCode.JoystickButton4) ||
               Input.GetKeyDown(KeyCode.JoystickButton1) ||  // Alternativa para Android
               CheckAndroidButton("y");
    }

    // B abajo (en Android puede ser JoystickButton0 o JoystickButton7)
    public static bool PressB()
    {
        return Input.GetKeyDown(KeyCode.B) ||
               Input.GetKeyDown(KeyCode.JoystickButton7) ||
               Input.GetKeyDown(KeyCode.JoystickButton0) ||  // Alternativa para Android
               CheckAndroidButton("b");
    }

    // A izquierda (en Android puede ser JoystickButton11)
    public static bool PressA()
    {
        return Input.GetKeyDown(KeyCode.A) ||
               Input.GetKeyDown(KeyCode.JoystickButton11) ||
               CheckAndroidButton("a");
    }

    // Gatillo
    public static bool PressTrigger()
    {
        return Input.GetKeyDown(KeyCode.JoystickButton0) ||
               Input.GetKeyDown(KeyCode.JoystickButton9) ||
#if UNITY_ANDROID
               Google.XR.Cardboard.Api.IsTriggerPressed ||
#endif
               false;
    }

    // Helper para detectar botones con axis en Android (alternativo)
    private static bool CheckAndroidButton(string buttonName)
    {
#if UNITY_ANDROID
        // En algunos controles, los botones pueden venir por axis
        try
        {
            switch (buttonName.ToLower())
            {
                case "x": return Input.GetButtonDown("x_android");
                case "y": return Input.GetButtonDown("y_android");
                case "a": return Input.GetButtonDown("a_android");
                case "b": return Input.GetButtonDown("b_android");
            }
        }
        catch { }
#endif
        return false;
    }
}