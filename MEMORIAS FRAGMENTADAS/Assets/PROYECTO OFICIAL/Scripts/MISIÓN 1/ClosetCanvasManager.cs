using UnityEngine;

public class ClosetCanvasManager : MonoBehaviour
{
    public bool uiAbierta = false;

    private GameObject currentCanvas;
    private ClosetMissionTrigger currentClosetMission;
    private bool currentChoiceIsCorrect = false;

    public void AbrirCanvas(GameObject canvasToOpen, bool isCorrect, ClosetMissionTrigger closetMission)
    {
        if (canvasToOpen == null)
        {
            Debug.LogWarning("No hay canvas asignado a esta prenda.");
            return;
        }

        currentCanvas = canvasToOpen;
        currentClosetMission = closetMission;
        currentChoiceIsCorrect = isCorrect;

        currentCanvas.SetActive(true);
        uiAbierta = true;

        Debug.Log("Canvas abierto: " + currentCanvas.name);
    }

    void Update()
    {
        if (!uiAbierta) return;

        // Si es incorrecta
        if (!currentChoiceIsCorrect)
        {
            if (InputManagerCustom.PressY())
            {
                Debug.Log("Probar otra prenda");
                ProbarOtra();
                return;
            }

            if (InputManagerCustom.PressX())
            {
                Debug.Log("Cerrar canvas incorrecto");
                CerrarCanvasActual();
                return;
            }
        }
        else
        {
            // Si es correcta: X cierra y vuelve a cámara original
            if (InputManagerCustom.PressX())
            {
                Debug.Log("Cerrar canvas correcto y volver");
                SalirCanvasCorrecto();
                return;
            }
        }
    }

    public void ProbarOtra()
    {
        CerrarCanvasActual();

        if (currentClosetMission != null)
        {
            currentClosetMission.ReactivarSeleccionSinMoverJugador();
        }
    }

    public void SalirCanvasCorrecto()
    {
        CerrarCanvasActual();

        if (currentClosetMission != null)
        {
            currentClosetMission.ReturnPlayerToOriginalPosition();
        }
    }

    public void CerrarCanvasActual()
    {
        if (currentCanvas != null)
        {
            currentCanvas.SetActive(false);
        }

        currentCanvas = null;
        uiAbierta = false;
    }
}