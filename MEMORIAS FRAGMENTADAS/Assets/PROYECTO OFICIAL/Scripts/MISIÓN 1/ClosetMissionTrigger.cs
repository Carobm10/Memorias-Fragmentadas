using UnityEngine;
using System.Collections;

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

    [Header("Raycast / Interacción")]
    [Tooltip("Colliders grandes del clóset que bloquean la detección de la ropa.")]
    public Collider[] collidersClosetParaDesactivar;

    private Vector3 originalPlayerPosition;
    private Quaternion originalPlayerRotation;
    private bool originalPoseSaved = false;

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

    private IEnumerator StartClosetMissionRoutine()
    {
        GuardarPoseOriginal();

        AbrirPuertasCloset();

        yield return new WaitForSeconds(delayAfterOpen);

        yield return StartCoroutine(MovePlayerToClosetView());

        // IMPORTANTE:
        // Después de mover al jugador, apagamos los colliders grandes del clóset
        // para que el raycast pueda detectar la ropa dentro.
        DesactivarCollidersCloset();

        BloquearMovimientoJugador();

        Debug.Log("Misión clóset: jugador en vista de selección. Ropa lista para interactuar.");
    }

    private void GuardarPoseOriginal()
    {
        if (playerRoot == null || originalPoseSaved) return;

        originalPlayerPosition = playerRoot.position;
        originalPlayerRotation = playerRoot.rotation;
        originalPoseSaved = true;
    }

    private void AbrirPuertasCloset()
    {
        for (int i = 0; i < closetDoors.Length; i++)
        {
            if (closetDoors[i] != null)
            {
                closetDoors[i].OpenDoor();
            }
        }
    }

    private void CerrarPuertasCloset()
    {
        for (int i = 0; i < closetDoors.Length; i++)
        {
            if (closetDoors[i] != null)
            {
                closetDoors[i].CloseDoor();
            }
        }
    }

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

    private void BloquearMovimientoJugador()
    {
        mov = movementScript as MovimientoVR2;

        if (mov != null)
        {
            mov.puedeMoverse = false;
            mov.activarHeadBob = false;
        }
    }

    private void DesactivarCollidersCloset()
    {
        for (int i = 0; i < collidersClosetParaDesactivar.Length; i++)
        {
            if (collidersClosetParaDesactivar[i] != null)
            {
                collidersClosetParaDesactivar[i].enabled = false;
            }
        }
    }

    private void ActivarCollidersCloset()
    {
        for (int i = 0; i < collidersClosetParaDesactivar.Length; i++)
        {
            if (collidersClosetParaDesactivar[i] != null)
            {
                collidersClosetParaDesactivar[i].enabled = true;
            }
        }
    }

    public void ReturnPlayerToOriginalPosition()
    {
        StartCoroutine(ReturnPlayerRoutine());
    }

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

        ActivarCollidersCloset();
        CerrarPuertasCloset();

        Debug.Log("Misión clóset completada. Jugador regresó a la posición original.");
    }

    private void ReactivarMovimientoJugador()
    {
        if (mov == null)
        {
            mov = movementScript as MovimientoVR2;
        }

        if (mov != null)
        {
            mov.puedeMoverse = true;
            mov.activarHeadBob = true;
        }
    }

    public void ReactivarSeleccionSinMoverJugador()
    {
        BloquearMovimientoJugador();
    }
}