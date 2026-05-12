using UnityEngine;

public class InspectableObject360 : MonoBehaviour
{
    [Header("Rotación")]
    public float rotationSpeed = 120f;
    public float mouseRotationSpeed = 5f;

    [Header("Punto de inspección")]
    public Transform inspectPoint;

    [Header("Movimiento jugador")]
    public MovimientoVR2 playerMovement;

    [Header("Cámara")]
    public Transform cameraTransform;

    [Header("Canvas salir")]
    public GameObject exitCanvas;

    [Header("Puntero 3D")]
    public GameObject pointer3D;

    [Header("Ajuste visual")]
    public float inspectedScale = 0.5f;
    public Vector3 inspectedLocalPosition = Vector3.zero;
    public Vector3 inspectedLocalRotation = Vector3.zero;

    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private Vector3 originalScale;
    private Transform originalParent;

    private Quaternion lockedCameraRotation;

    private Collider[] colliders;
    private Rigidbody rb;

    private bool isInspecting = false;

    public bool IsInspecting()
    {
        return isInspecting;
    }

    public void StartInspection()
    {
        StartInspect();
    }

    public void StartInspect()
    {
        if (isInspecting) return;

        if (inspectPoint == null)
        {
            Debug.LogWarning("Falta InspectPoint en " + gameObject.name);
            return;
        }

        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;

        isInspecting = true;

        if (cameraTransform != null)
            lockedCameraRotation = cameraTransform.rotation;

        originalParent = transform.parent;
        originalPosition = transform.position;
        originalRotation = transform.rotation;
        originalScale = transform.localScale;

        colliders = GetComponentsInChildren<Collider>();
        rb = GetComponent<Rigidbody>();

        foreach (Collider col in colliders)
            col.enabled = false;

        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        transform.SetParent(inspectPoint);
        transform.localPosition = inspectedLocalPosition;
        transform.localRotation = Quaternion.Euler(inspectedLocalRotation);
        transform.localScale = Vector3.one * inspectedScale;

        if (playerMovement != null)
            playerMovement.puedeMoverse = false;

        if (exitCanvas != null)
            exitCanvas.SetActive(true);

        if (pointer3D != null)
            pointer3D.SetActive(false);
    }

    public void StopInspect()
    {
        if (!isInspecting) return;

        isInspecting = false;

        transform.SetParent(originalParent, true);
        transform.position = originalPosition;
        transform.rotation = originalRotation;
        transform.localScale = originalScale;

        if (colliders != null)
        {
            foreach (Collider col in colliders)
            {
                if (col != null)
                    col.enabled = true;
            }
        }

        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }

        if (playerMovement != null)
            playerMovement.puedeMoverse = true;

        if (exitCanvas != null)
            exitCanvas.SetActive(false);

        if (pointer3D != null)
            pointer3D.SetActive(true);
    }

    void LateUpdate()
    {
        if (!isInspecting) return;

        if (cameraTransform != null)
            cameraTransform.rotation = lockedCameraRotation;
    }

    void Update()
    {
        if (!isInspecting) return;

        if (InputManagerCustom.PressX())
        {
            StopInspect();
            return;
        }

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        transform.Rotate(Vector3.up, -horizontal * rotationSpeed * Time.deltaTime, Space.World);
        transform.Rotate(Vector3.right, vertical * rotationSpeed * Time.deltaTime, Space.World);

        if (Input.GetMouseButton(0))
        {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            transform.Rotate(Vector3.up, -mouseX * mouseRotationSpeed, Space.World);
            transform.Rotate(Vector3.right, mouseY * mouseRotationSpeed, Space.World);
        }
    }
}