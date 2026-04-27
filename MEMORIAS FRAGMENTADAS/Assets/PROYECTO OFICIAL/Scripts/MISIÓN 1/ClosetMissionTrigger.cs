using UnityEngine;
using System.Collections;

public class ClosetMissionTrigger : MonoBehaviour
{
    private MovimientoVR2 mov;

    [Header("Puertas del clóset")]
    public SystemDoor[] closetDoors;

    [Header("Estado misión")]
    public bool missionStarted = false;
    public bool missionCompleted = false;

    [Header("Delay")]
    public float delayAfterOpen = 1.5f;

    [Header("Posicionamiento del jugador")]
    public Transform cameraFocusPoint;
    public Transform playerRoot;
    public Transform cameraTransform;
    public float moveDuration = 1.8f;
    public float returnDuration = 1.5f;

    [Header("Bloqueo de movimiento")]
    public MonoBehaviour movementScript;

    private Vector3 originalPlayerPosition;
    private Quaternion originalPlayerRotation;
    private bool originalPoseSaved = false;

    public void StartClosetMission()
    {
        if (missionStarted || missionCompleted) return;

        missionStarted = true;
        StartCoroutine(StartClosetMissionRoutine());
    }

    IEnumerator StartClosetMissionRoutine()
    {
        GuardarPoseOriginal();

        for (int i = 0; i < closetDoors.Length; i++)
        {
            if (closetDoors[i] != null)
                closetDoors[i].ToggleDoor();
        }

        yield return new WaitForSeconds(delayAfterOpen);
        yield return StartCoroutine(MovePlayerToClosetView());

        mov = movementScript as MovimientoVR2;

        if (mov != null)
        {
            mov.puedeMoverse = false;
            mov.activarHeadBob = false;
        }
    }

    void GuardarPoseOriginal()
    {
        if (playerRoot == null || originalPoseSaved) return;

        originalPlayerPosition = playerRoot.position;
        originalPlayerRotation = playerRoot.rotation;
        originalPoseSaved = true;
    }

    IEnumerator MovePlayerToClosetView()
    {
        if (playerRoot == null || cameraFocusPoint == null || cameraTransform == null)
            yield break;

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

    public void ReturnPlayerToOriginalPosition()
    {
        StartCoroutine(ReturnPlayerRoutine());
    }

    IEnumerator ReturnPlayerRoutine()
    {
        if (playerRoot == null || !originalPoseSaved)
            yield break;

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

        if (mov == null)
            mov = movementScript as MovimientoVR2;

        if (mov != null)
        {
            mov.puedeMoverse = true;
            mov.activarHeadBob = true;
        }

        missionCompleted = true;
        missionStarted = false;
    }

    public void ReactivarSeleccionSinMoverJugador()
    {
        if (mov == null)
            mov = movementScript as MovimientoVR2;

        if (mov != null)
        {
            mov.puedeMoverse = false;
            mov.activarHeadBob = false;
        }
    }
}