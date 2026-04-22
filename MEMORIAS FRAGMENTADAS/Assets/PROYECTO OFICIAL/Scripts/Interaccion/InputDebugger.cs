using UnityEngine;
using UnityEngine.UI;

public class InputDebugger : MonoBehaviour
{
    public Text debugText;
    private string debugInfo = "";

    void Update()
    {
        debugInfo = "=== INPUT DEBUG ===\n";
        
        // Detectar todos los joystick buttons
        for (int i = 0; i < 20; i++)
        {
            if (Input.GetKeyDown(KeyCode.JoystickButton0 + i))
            {
                debugInfo += $"JoystickButton{i} PRESSED\n";
            }
        }

        // Verificar ejes de entrada
        debugInfo += "\nAxes:\n";
        debugInfo += $"Horizontal: {Input.GetAxis("Horizontal")}\n";
        debugInfo += $"Vertical: {Input.GetAxis("Vertical")}\n";
        debugInfo += $"Submit: {Input.GetAxis("Submit")}\n";
        debugInfo += $"Cancel: {Input.GetAxis("Cancel")}\n";

        // Verificar teclas
        debugInfo += "\nKeys:\n";
        if (Input.GetKeyDown(KeyCode.A)) debugInfo += "A KEY\n";
        if (Input.GetKeyDown(KeyCode.X)) debugInfo += "X KEY\n";
        if (Input.GetKeyDown(KeyCode.Y)) debugInfo += "Y KEY\n";
        if (Input.GetKeyDown(KeyCode.B)) debugInfo += "B KEY\n";

#if UNITY_ANDROID
        // Verificar Cardboard
        try
        {
            if (Google.XR.Cardboard.Api.IsTriggerPressed)
                debugInfo += "CARDBOARD TRIGGER PRESSED\n";
        }
        catch { }
#endif

        if (debugText != null)
            debugText.text = debugInfo;
        else
            Debug.Log(debugInfo);
    }
}
