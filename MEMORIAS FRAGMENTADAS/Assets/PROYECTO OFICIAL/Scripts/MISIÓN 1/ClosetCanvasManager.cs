using UnityEngine;

/// <summary>
/// ClosetCanvasManager controla los canvas que aparecen al seleccionar una prenda.
/// 
/// Regla oficial de botones:
/// - X = salir/cerrar canvas.
/// 
/// Este script NO selecciona ropa.
/// La ropa se selecciona desde Selected.cs con el botón B.
/// 
/// Flujo:
/// 1. El jugador mira una prenda.
/// 2. Presiona B.
/// 3. Selected.cs abre el canvas de esa prenda.
/// 4. Si el canvas está abierto, X lo cierra.
/// 5. Si la prenda era correcta, vuelve al jugador a su posición original.
/// 6. Si era incorrecta, deja al jugador en la vista del clóset para probar otra.
/// </summary>
public class ClosetCanvasManager : MonoBehaviour
{
    [Header("Bloqueo de cámara")]
    public CameraLockController cameraLockController;

    [Header("Estado UI")]
    public bool uiAbierta = false;

    [Header("Puntero 3D")]
    public GameObject pointer3D;

    private GameObject currentCanvas;
    private ClosetMissionTrigger currentClosetMission;
    private bool currentChoiceIsCorrect = false;

    void Update()
    {
        // Si no hay canvas abierto, no hacemos nada.
        if (!uiAbierta) return;

        // X cierra cualquier canvas abierto.
        if (InputManagerCustom.PressX())
        {
            Debug.Log("Cerrando canvas de prenda con X");

            if (currentChoiceIsCorrect)
            {
                SalirCanvasCorrecto();
            }
            else
            {
                ProbarOtra();
            }
        }
    }

    /// <summary>
    /// Abre el canvas asociado a una prenda.
    /// </summary>
    public void AbrirCanvas(GameObject canvasToOpen, bool isCorrect, ClosetMissionTrigger closetMission)
    {
        if (canvasToOpen == null)
        {
            Debug.LogWarning("No hay canvas asignado a esta prenda.");
            return;
        }

        Debug.Log("Abriendo canvas de prenda: " + canvasToOpen.name);

        currentCanvas = canvasToOpen;
        currentClosetMission = closetMission;
        currentChoiceIsCorrect = isCorrect;

        currentCanvas.SetActive(false);
        currentCanvas.SetActive(true);

        uiAbierta = true;

        if (pointer3D != null)
            pointer3D.SetActive(false);

        if (cameraLockController != null)
            cameraLockController.LockCamera();
    }

    /// <summary>
    /// Se usa cuando la prenda seleccionada fue incorrecta.
    /// Cierra el canvas y mantiene al jugador en la vista del clóset.
    /// </summary>
    public void ProbarOtra()
    {
        CerrarCanvasActual();

        if (currentClosetMission != null)
        {
            currentClosetMission.ReactivarSeleccionSinMoverJugador();
        }
    }

    /// <summary>
    /// Se usa cuando la prenda seleccionada fue correcta.
    /// Cierra el canvas y devuelve al jugador a su posición original.
    /// </summary>
    public void SalirCanvasCorrecto()
    {
        CerrarCanvasActual();

        if (currentClosetMission != null)
        {
            currentClosetMission.ReturnPlayerToOriginalPosition();
        }
    }

    /// <summary>
    /// Cierra el canvas activo y reactiva puntero/cámara.
    /// </summary>
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