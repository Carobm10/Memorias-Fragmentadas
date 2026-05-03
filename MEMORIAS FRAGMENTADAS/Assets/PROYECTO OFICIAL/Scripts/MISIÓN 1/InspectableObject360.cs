using UnityEngine;

public class InspectableObject360 : MonoBehaviour
{
    [Header("Referencias")]
    public Transform inspectPoint;
    public MovimientoVR2 movimientoPlayer;
    public Pointer3DController pointer3D;
    public GameObject canvasInspect;

    [Header("Opcional: script que rota la cámara")]
    public CameraLockController cameraLock;

    [Header("Configuración")]
    public float moveSpeed = 8f;
    public float rotationSpeed = 120f;
    public float inspectScale = 1f;

    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private Vector3 originalScale;
    private Transform originalParent;

    private bool isInspecting = false;
    private bool isMovingToInspect = false;
    private bool isReturning = false;

    void Start()
    {
        SaveOriginalTransform();

        if (canvasInspect != null)
            canvasInspect.SetActive(false);
    }

    void Update()
    {
        if (isMovingToInspect)
        {
            MoveToInspectPoint();
        }

        if (isInspecting && !isMovingToInspect && !isReturning)
        {
            RotateObject();

            if (InputManagerCustom.PressX())
            {
                ExitInspection();
            }
        }

        if (isReturning)
        {
            ReturnToOriginalPosition();
        }
    }

    public void StartInspection()
    {
        if (isInspecting || isMovingToInspect || isReturning) return;

        SaveOriginalTransform();

        isInspecting = true;
        isMovingToInspect = true;
        isReturning = false;

        if (movimientoPlayer != null)
        {
            movimientoPlayer.puedeMoverse = false;
            movimientoPlayer.activarHeadBob = false;
        }

        if (cameraLock != null)
            cameraLock.LockCamera();

        if (pointer3D != null)
            pointer3D.gameObject.SetActive(false);

        if (canvasInspect != null)
            canvasInspect.SetActive(true);

        transform.SetParent(null);
    }

    void MoveToInspectPoint()
    {
        if (inspectPoint == null)
        {
            Debug.LogWarning("Falta asignar InspectPoint en " + gameObject.name);
            return;
        }

        transform.position = Vector3.Lerp(
            transform.position,
            inspectPoint.position,
            Time.deltaTime * moveSpeed
        );

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            inspectPoint.rotation,
            Time.deltaTime * moveSpeed
        );

        transform.localScale = Vector3.Lerp(
            transform.localScale,
            originalScale * inspectScale,
            Time.deltaTime * moveSpeed
        );

        if (Vector3.Distance(transform.position, inspectPoint.position) < 0.02f)
        {
            transform.position = inspectPoint.position;
            transform.rotation = inspectPoint.rotation;
            transform.localScale = originalScale * inspectScale;

            isMovingToInspect = false;
        }
    }

    void RotateObject()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        transform.Rotate(Camera.main.transform.up, -horizontal * rotationSpeed * Time.deltaTime, Space.World);
        transform.Rotate(Camera.main.transform.right, vertical * rotationSpeed * Time.deltaTime, Space.World);
    }

    void ExitInspection()
    {
        isInspecting = false;
        isMovingToInspect = false;
        isReturning = true;

        if (canvasInspect != null)
            canvasInspect.SetActive(false);
    }

    void ReturnToOriginalPosition()
    {
        transform.position = Vector3.Lerp(
            transform.position,
            originalPosition,
            Time.deltaTime * moveSpeed
        );

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            originalRotation,
            Time.deltaTime * moveSpeed
        );

        transform.localScale = Vector3.Lerp(
            transform.localScale,
            originalScale,
            Time.deltaTime * moveSpeed
        );

        if (Vector3.Distance(transform.position, originalPosition) < 0.02f)
        {
            transform.position = originalPosition;
            transform.rotation = originalRotation;
            transform.localScale = originalScale;

            transform.SetParent(originalParent);

            isReturning = false;

            if (movimientoPlayer != null)
            {
                movimientoPlayer.puedeMoverse = true;
                movimientoPlayer.activarHeadBob = true;
            }

            if (cameraLock != null)
                cameraLock.UnlockCamera();

            if (pointer3D != null)
                pointer3D.gameObject.SetActive(true);
        }
    }

    void SaveOriginalTransform()
    {
        originalPosition = transform.position;
        originalRotation = transform.rotation;
        originalScale = transform.localScale;
        originalParent = transform.parent;
    }

    public bool IsInspecting()
    {
        return isInspecting || isMovingToInspect || isReturning;
    }
}