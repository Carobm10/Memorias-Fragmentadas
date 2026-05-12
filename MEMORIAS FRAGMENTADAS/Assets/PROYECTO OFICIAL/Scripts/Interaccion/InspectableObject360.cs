using UnityEngine;

public class InspectableObject360 : MonoBehaviour
{
    [Header("Cámara")]
    public Transform cameraTransform;

    private Quaternion lockedCameraRotation;
    [Header("Prefab visual")]
    public GameObject visualPrefab;

    [Header("Punto de inspección")]
    public Transform inspectPoint;

    [Header("Canvas salir")]
    public GameObject exitCanvas;

    [Header("Pointer")]
    public GameObject pointer3D;

    [Header("Movimiento jugador")]
    public MovimientoVR2 playerMovement;

    [Header("Rotación")]
    public float rotationSpeed = 120f;
    public float mouseRotationSpeed = 5f;

    [Header("Escala copia")]
    public float inspectScale = 1f;

    private GameObject currentClone;
    private bool inspecting = false;

    public bool IsInspecting()
    {
        return inspecting;
    }

    public void StartInspection()
    {
        if (inspecting) return;

        if (visualPrefab == null)
        {
            Debug.LogWarning("No hay visualPrefab en " + gameObject.name);
            return;
        }

        inspecting = true;

        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;

        if (cameraTransform != null)
            lockedCameraRotation = cameraTransform.rotation;

        currentClone = Instantiate(
            visualPrefab,
            inspectPoint.position,
            Quaternion.identity
        );

        currentClone.transform.SetParent(inspectPoint);

        currentClone.transform.localPosition = Vector3.zero;
        currentClone.transform.localRotation = Quaternion.identity;
        currentClone.transform.localScale = Vector3.one * inspectScale;

        Collider[] cloneCols = currentClone.GetComponentsInChildren<Collider>();

        foreach (Collider col in cloneCols)
            col.enabled = false;

        Rigidbody[] rbs = currentClone.GetComponentsInChildren<Rigidbody>();

        foreach (Rigidbody rb in rbs)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>();

        foreach (MeshRenderer r in renderers)
            r.enabled = false;

        if (playerMovement != null)
            playerMovement.puedeMoverse = false;

        if (pointer3D != null)
            pointer3D.SetActive(false);

        if (exitCanvas != null)
            exitCanvas.SetActive(true);
    }

    public void StopInspect()
    {
        if (!inspecting) return;

        inspecting = false;

        if (currentClone != null)
            Destroy(currentClone);

        MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>();

        foreach (MeshRenderer r in renderers)
            r.enabled = true;

        if (playerMovement != null)
            playerMovement.puedeMoverse = true;

        if (pointer3D != null)
            pointer3D.SetActive(true);

        if (exitCanvas != null)
            exitCanvas.SetActive(false);
    }

    void Update()
    {
        if (!inspecting) return;

        if (InputManagerCustom.PressX())
        {
            StopInspect();
            return;
        }

        if (currentClone == null) return;

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        currentClone.transform.Rotate(
            Vector3.up,
            -horizontal * rotationSpeed * Time.deltaTime,
            Space.World
        );

        currentClone.transform.Rotate(
            Vector3.right,
            vertical * rotationSpeed * Time.deltaTime,
            Space.World
        );

        if (Input.GetMouseButton(0))
        {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            currentClone.transform.Rotate(
                Vector3.up,
                -mouseX * mouseRotationSpeed,
                Space.World
            );

            currentClone.transform.Rotate(
                Vector3.right,
                mouseY * mouseRotationSpeed,
                Space.World
            );
        }
    }

    void LateUpdate()
    {
        if (!inspecting) return;

        if (cameraTransform != null)
            cameraTransform.rotation = lockedCameraRotation;
    }
}