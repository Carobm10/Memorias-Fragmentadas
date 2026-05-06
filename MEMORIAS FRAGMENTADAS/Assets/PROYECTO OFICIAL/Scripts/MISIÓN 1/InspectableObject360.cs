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
    public float rotationSpeed = 200f;
    public float inspectScale = 1f;
    [Range(0.02f, 0.5f)]
    public float moveSmoothTime = 0.12f;
    [Range(0.02f, 0.5f)]
    public float rotationSmoothTime = 0.08f;

    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private Vector3 originalScale;
    private Transform originalParent;

    private Transform rotationPivot;
    private Vector3 pivotVelocity;
    private Quaternion targetPivotRotation;

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
        CrearPivotDeInspeccion();

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
    }

    void MoveToInspectPoint()
    {
        if (inspectPoint == null || rotationPivot == null)
        {
            Debug.LogWarning("Falta asignar InspectPoint en " + gameObject.name);
            return;
        }

        rotationPivot.position = Vector3.SmoothDamp(
            rotationPivot.position,
            inspectPoint.position,
            ref pivotVelocity,
            moveSmoothTime,
            Mathf.Infinity,
            Time.deltaTime
        );

        float giroSuave = 1f - Mathf.Exp(-moveSpeed * Time.deltaTime);
        rotationPivot.rotation = Quaternion.Slerp(rotationPivot.rotation, targetPivotRotation, giroSuave);

        transform.localScale = Vector3.Lerp(
            transform.localScale,
            originalScale * inspectScale,
            Time.deltaTime * moveSpeed
        );

        if (Vector3.Distance(rotationPivot.position, inspectPoint.position) < 0.02f)
        {
            rotationPivot.position = inspectPoint.position;
            rotationPivot.rotation = targetPivotRotation;
            transform.localScale = originalScale * inspectScale;
            AjustarCentroAlPivot();

            isMovingToInspect = false;
        }

        AjustarCentroAlPivot();
    }

    void RotateObject()
    {
        if (rotationPivot == null)
        {
            return;
        }

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        if (Mathf.Abs(horizontal) < 0.001f && Mathf.Abs(vertical) < 0.001f)
        {
            return;
        }

        Vector3 up = Camera.main != null ? Camera.main.transform.up : Vector3.up;
        Vector3 right = Camera.main != null ? Camera.main.transform.right : Vector3.right;

        Quaternion giroHorizontal = Quaternion.AngleAxis(-horizontal * rotationSpeed * Time.deltaTime, up);
        Quaternion giroVertical = Quaternion.AngleAxis(vertical * rotationSpeed * Time.deltaTime, right);

        targetPivotRotation = giroHorizontal * giroVertical * targetPivotRotation;

        float giroFollow = 1f - Mathf.Exp(-rotationSmoothTime * 20f * Time.deltaTime);
        rotationPivot.rotation = Quaternion.Slerp(rotationPivot.rotation, targetPivotRotation, giroFollow);

        AjustarCentroAlPivot();
    }

    void ExitInspection()
    {
        isInspecting = false;
        isMovingToInspect = false;
        isReturning = true;

        DestruirPivotDeInspeccion();

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

    void CrearPivotDeInspeccion()
    {
        if (rotationPivot != null)
        {
            Destroy(rotationPivot.gameObject);
        }

        GameObject pivotObject = new GameObject(gameObject.name + "_InspectionPivot");
        rotationPivot = pivotObject.transform;
        rotationPivot.position = ObtenerCentroMundoObjetos();
        rotationPivot.rotation = transform.rotation;

        transform.SetParent(rotationPivot, true);
        transform.localRotation = Quaternion.identity;
        transform.localPosition = Vector3.zero;
        targetPivotRotation = rotationPivot.rotation;
        pivotVelocity = Vector3.zero;

        AjustarCentroAlPivot();
    }

    void DestruirPivotDeInspeccion()
    {
        if (rotationPivot == null)
        {
            return;
        }

        transform.SetParent(originalParent, true);

        Destroy(rotationPivot.gameObject);
        rotationPivot = null;
    }

    Vector3 ObtenerCentroMundoObjetos()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>(true);

        if (renderers == null || renderers.Length == 0)
        {
            return transform.position;
        }

        Bounds bounds = renderers[0].bounds;

        for (int i = 1; i < renderers.Length; i++)
        {
            bounds.Encapsulate(renderers[i].bounds);
        }

        return bounds.center;
    }

    Vector3 ObtenerCentroLocalObjetos()
    {
        return transform.InverseTransformPoint(ObtenerCentroMundoObjetos());
    }

    void AjustarCentroAlPivot()
    {
        if (rotationPivot == null)
        {
            return;
        }

        Vector3 centroActual = ObtenerCentroMundoObjetos();
        Vector3 delta = rotationPivot.position - centroActual;

        if (delta.sqrMagnitude > 0.000001f)
        {
            transform.position += delta;
        }
    }

    public bool IsInspecting()
    {
        return isInspecting || isMovingToInspect || isReturning;
    }
}