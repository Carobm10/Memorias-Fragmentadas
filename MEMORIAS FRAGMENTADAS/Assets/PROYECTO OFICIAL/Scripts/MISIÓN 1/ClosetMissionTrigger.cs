using UnityEngine;
using System.Collections;

/// <summary>
/// ClosetMissionTrigger controla la misión del clóset.
/// 
/// Este script NO lee botones directamente.
/// La activación de la misión se hace desde Selected.cs con el botón A.
/// 
/// Flujo:
/// 1. El jugador mira el clóset.
/// 2. Selected.cs detecta ClosetMissionTrigger.
/// 3. El jugador presiona A.
/// 4. Se llama StartClosetMission().
/// 5. Se abren las puertas del clóset.
/// 6. El jugador se mueve hacia el punto cameraFocusPoint.
/// 7. Se bloquea el movimiento para elegir ropa.
/// 8. Al elegir la prenda correcta, ClosetCanvasManager llama ReturnPlayerToOriginalPosition().
/// </summary>
public class ClosetMissionTrigger : MonoBehaviour
{
    private MovimientoVR2 mov;

    [Header("Puertas del clóset")]
    public DoorInteractable[] closetDoors;

    [Header("Estado misión")]
    public bool missionStarted = false;
    public bool missionCompleted = false;

    [Header("Tiempo antes de mover al jugador")]
    public float delayAfterOpen = 1.5f;

    [Header("Posicionamiento del jugador")]
    public Transform cameraFocusPoint;
    public Transform playerRoot;
    public Transform cameraTransform;
    public float moveDuration = 1.8f;
    public float returnDuration = 1.5f;

    [Header("Bloqueo de movimiento")]
    public MonoBehaviour movementScript;

    /// <summary>
    /// Inicia la misión del clóset.
    /// Se llama desde Selected.cs cuando el jugador presiona A mirando el clóset.
    /// </summary>
    public void StartClosetMission()
    {
        if (missionStarted || missionCompleted)
        {
            Debug.Log("La misión del clóset ya inició o ya fue completada.");
            return;
        }

        missionStarted = true;
        StartCoroutine(StartClosetMissionRoutine());
    }

    /// <summary>
    /// Rutina principal de la misión.
    /// Abre puertas, espera, mueve jugador y bloquea movimiento.
    /// </summary>
    private IEnumerator StartClosetMissionRoutine()
    {
        GuardarPoseOriginal();

        AbrirPuertasCloset();

        yield return new WaitForSeconds(delayAfterOpen);

        yield return StartCoroutine(MovePlayerToClosetView());

        BloquearMovimientoJugador();
    }

    /// <summary>
    /// Guarda la posición original del jugador antes de moverlo al clóset.
    /// </summary>
    private Vector3 originalPlayerPosition;
    private Quaternion originalPlayerRotation;
    private bool originalPoseSaved = false;

    private void GuardarPoseOriginal()
    {
        if (playerRoot == null || originalPoseSaved) return;

        originalPlayerPosition = playerRoot.position;
        originalPlayerRotation = playerRoot.rotation;
        originalPoseSaved = true;
    }

    /// <summary>
    /// Abre todas las puertas configuradas del clóset.
    /// </summary>
    private void AbrirPuertasCloset()
    {
        for (int i = 0; i < closetDoors.Length; i++)
        {
            if (closetDoors[i] != null)
                closetDoors[i].OpenDoor();
        }
    }

    /// <summary>
    /// Cierra todas las puertas configuradas del clóset.
    /// </summary>
    private void CerrarPuertasCloset()
    {
        for (int i = 0; i < closetDoors.Length; i++)
        {
            if (closetDoors[i] != null)
                closetDoors[i].CloseDoor();
        }
    }

    /// <summary>
    /// Mueve el Player hasta la vista del clóset.
    /// Usa el offset entre Player y Camera para no romper la altura de VR.
    /// </summary>
    private IEnumerator MovePlayerToClosetView()
    {
        if (playerRoot == null || cameraFocusPoint == null || cameraTransform == null)
        {
            Debug.LogWarning("Faltan referencias para mover al jugador al clóset.");
            yield break;
        }

        Vector3 startPos = playerRoot.position;
        Quaternion startRot = playerRoot.rotation;

        Vector3 offset = playerRoot.position - cameraTransform.position;

        Vector3 targetPos = cameraFocusPoint.position + offset;
        Quaternion targetRot = cameraFocusPoint.rotation;

        float elapsed = 0f;

        while (elapsed < moveDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / moveDuration;

            playerRoot.position = Vector3.Lerp(startPos, targetPos, t);
            playerRoot.rotation = Quaternion.Slerp(startRot, targetRot, t);

            yield return null;
        }

        playerRoot.position = targetPos;
        playerRoot.rotation = targetRot;
    }

    /// <summary>
    /// Bloquea movimiento y head bob mientras el jugador elige ropa.
    /// </summary>
    private void BloquearMovimientoJugador()
    {
        mov = movementScript as MovimientoVR2;

        if (mov != null)
        {
            mov.puedeMoverse = false;
            mov.activarHeadBob = false;
        }
    }

    /// <summary>
    /// Devuelve al jugador a la posición original.
    /// Se llama cuando el jugador eligió la prenda correcta y cierra el canvas con X.
    /// </summary>
    public void ReturnPlayerToOriginalPosition()
    {
        StartCoroutine(ReturnPlayerRoutine());
    }

    /// <summary>
    /// Rutina que regresa al jugador, reactiva movimiento y cierra puertas.
    /// </summary>
    private IEnumerator ReturnPlayerRoutine()
    {
        if (playerRoot == null || !originalPoseSaved)
        {
            Debug.LogWarning("No hay pose original guardada para regresar al jugador.");
            yield break;
        }

        Vector3 startPos = playerRoot.position;
        Quaternion startRot = playerRoot.rotation;

        float elapsed = 0f;

        while (elapsed < returnDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / returnDuration;

            playerRoot.position = Vector3.Lerp(startPos, originalPlayerPosition, t);
            playerRoot.rotation = Quaternion.Slerp(startRot, originalPlayerRotation, t);

            yield return null;
        }

        playerRoot.position = originalPlayerPosition;
        playerRoot.rotation = originalPlayerRotation;

        ReactivarMovimientoJugador();

        missionCompleted = true;
        missionStarted = false;

        CerrarPuertasCloset();
    }

    /// <summary>
    /// Reactiva movimiento normal del jugador.
    /// </summary>
    private void ReactivarMovimientoJugador()
    {
        if (mov == null)
            mov = movementScript as MovimientoVR2;

        if (mov != null)
        {
            mov.puedeMoverse = true;
            mov.activarHeadBob = true;
        }
    }

    /// <summary>
    /// Mantiene al jugador quieto en la vista del clóset después de elegir una prenda incorrecta.
    /// </summary>
    public void ReactivarSeleccionSinMoverJugador()
    {
        BloquearMovimientoJugador();
    }
}