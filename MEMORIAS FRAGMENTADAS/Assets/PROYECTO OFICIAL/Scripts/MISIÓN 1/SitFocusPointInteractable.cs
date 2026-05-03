using UnityEngine;
using System.Collections;

public class SitFocusPointInteractable : MonoBehaviour
{
    [Header("Punto donde quedará la cámara sentada")]
    public Transform sitFocusPoint;

    [Header("Jugador")]
    public Transform playerRoot;
    public Transform cameraTransform;
    public MonoBehaviour movementScript;

    [Header("UI")]
    public Pointer3DController pointer3D;
    public GameObject canvasSalir;

    [Header("Cámara")]
    public CameraLockController cameraLock;

    [Header("Tiempos")]
    public float moveDuration = 1.2f;
    public float returnDuration = 1.0f;

    private MovimientoVR2 mov;

    private Vector3 originalPlayerPosition;
    private Quaternion originalPlayerRotation;
    private bool originalPoseSaved = false;

    private bool sitting = false;
    private bool moving = false;

    void Start()
    {
        if (canvasSalir != null)
            canvasSalir.SetActive(false);
    }

    void Update()
    {
        if (sitting && !moving && InputManagerCustom.PressX())
        {
            ReturnFromSeat();
        }
    }

    public void Sit()
    {
        if (sitting || moving) return;

        GuardarPoseOriginal();
        StartCoroutine(MovePlayerToSeatView());
    }

    void GuardarPoseOriginal()
    {
        if (playerRoot == null) return;

        originalPlayerPosition = playerRoot.position + Vector3.up * 0.15f;
        originalPlayerRotation = playerRoot.rotation;
        originalPoseSaved = true;
    }

    IEnumerator MovePlayerToSeatView()
    {
        if (playerRoot == null || sitFocusPoint == null || cameraTransform == null)
            yield break;

        moving = true;

        mov = movementScript as MovimientoVR2;

        if (mov != null)
        {
            mov.puedeMoverse = false;
            mov.activarHeadBob = false;
        }

        if (pointer3D != null)
            pointer3D.gameObject.SetActive(false);

        if (canvasSalir != null)
            canvasSalir.SetActive(true);

        Vector3 startPos = playerRoot.position;
        Quaternion startRot = playerRoot.rotation;

        Vector3 offset = playerRoot.position - cameraTransform.position;

        Vector3 targetPos = sitFocusPoint.position + offset;
        Quaternion targetRot = sitFocusPoint.rotation;

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

        if (cameraLock != null)
            cameraLock.LockCamera();

        sitting = true;
        moving = false;
    }

    public void ReturnFromSeat()
    {
        if (!sitting || moving || !originalPoseSaved) return;

        StartCoroutine(ReturnPlayerRoutine());
    }

    IEnumerator ReturnPlayerRoutine()
    {
        moving = true;

        if (cameraLock != null)
            cameraLock.UnlockCamera();

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

        if (pointer3D != null)
            pointer3D.gameObject.SetActive(true);

        if (canvasSalir != null)
            canvasSalir.SetActive(false);

        sitting = false;
        moving = false;
        originalPoseSaved = false;
    }
}