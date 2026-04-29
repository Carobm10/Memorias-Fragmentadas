using UnityEngine;

public class ClosetCanvasManager : MonoBehaviour
{
    [Header("Bloqueo de cámara")]
    public CameraLockController cameraLockController;
    public bool uiAbierta = false;

    [Header("Puntero 3D")]
    public GameObject pointer3D;

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

        if (pointer3D != null)
            pointer3D.SetActive(false);

        if (cameraLockController != null)
            cameraLockController.LockCamera();

        Debug.Log("Canvas abierto: " + currentCanvas.name);
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
            currentCanvas.SetActive(false);

        currentCanvas = null;
        uiAbierta = false;

        if (pointer3D != null)
            pointer3D.SetActive(true);

        if (cameraLockController != null)
            cameraLockController.UnlockCamera();
    }
}