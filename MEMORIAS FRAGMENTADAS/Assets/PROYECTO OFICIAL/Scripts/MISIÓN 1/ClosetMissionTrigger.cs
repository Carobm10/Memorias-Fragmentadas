using UnityEngine;
using System.Collections;

public class ClosetMissionTrigger : MonoBehaviour
{
    private MovimientoVR2 mov;

    [Header("Puertas del clóset")]
    public SystemDoor[] closetDoors;

    [Header("Estado misión")]
    public bool missionStarted = false;

    [Header("Delay")]
    public float delayAfterOpen = 1.5f;

    [Header("Posicionamiento del jugador")]
    public Transform cameraFocusPoint;
    public Transform playerRoot;
    public Transform cameraTransform;
    public float moveDuration = 1.8f;

    [Header("Bloqueo de movimiento")]
    public MonoBehaviour movementScript;

    public void StartClosetMission()
    {
        if (missionStarted) return;

        missionStarted = true;
        StartCoroutine(StartClosetMissionRoutine());
    }

    IEnumerator StartClosetMissionRoutine()
    {
        // 1. Abrir puertas
        for (int i = 0; i < closetDoors.Length; i++)
        {
            if (closetDoors[i] != null)
                closetDoors[i].ToggleDoor();
        }

        yield return new WaitForSeconds(delayAfterOpen);

        // 2. Mover jugador primero
        yield return StartCoroutine(MovePlayerToClosetView());

        // 3. Bloquear movimiento DESPUÉS
        mov = movementScript as MovimientoVR2;

        if (mov != null)
        {
            mov.puedeMoverse = false;
            mov.activarHeadBob = false;
        }

        Debug.Log("Jugador listo frente al clóset");
    }

    IEnumerator MovePlayerToClosetView()
    {
        if (playerRoot == null || cameraFocusPoint == null || cameraTransform == null)
            yield break;

        Vector3 startPos = playerRoot.position;
        Quaternion startRot = playerRoot.rotation;

        // Calculamos offset correcto de la cámara
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
}