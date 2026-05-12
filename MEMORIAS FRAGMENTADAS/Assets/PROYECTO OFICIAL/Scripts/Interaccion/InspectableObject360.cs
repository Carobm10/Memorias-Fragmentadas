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

    [Header("Debug")]
    public bool showDebug = true;

    private GameObject currentClone;
    private GameObject visualWrapper;
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

        if (inspectPoint == null)
        {
            Debug.LogError("No hay InspectPoint asignado en " + gameObject.name);
            return;
        }

        inspecting = true;

        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;

        if (cameraTransform != null)
            lockedCameraRotation = cameraTransform.rotation;

        // Creamos un contenedor limpio en el punto de inspección.
        visualWrapper = new GameObject("INSPECCION_360_" + visualPrefab.name);
        visualWrapper.transform.SetParent(inspectPoint);
        visualWrapper.transform.localPosition = Vector3.zero;
        visualWrapper.transform.localRotation = Quaternion.identity;
        visualWrapper.transform.localScale = Vector3.one;

        // Creamos la copia visual dentro del contenedor.
        currentClone = Instantiate(visualPrefab, visualWrapper.transform);
        currentClone.name = visualPrefab.name + "_CLON_360";
        currentClone.transform.localPosition = Vector3.zero;
        currentClone.transform.localRotation = Quaternion.identity;
        currentClone.transform.localScale = Vector3.one * inspectScale;

        // Desactivamos física/colliders del clon para que no empuje nada.
        Collider[] cloneCols = currentClone.GetComponentsInChildren<Collider>(true);
        foreach (Collider col in cloneCols)
            col.enabled = false;

        Rigidbody[] rbs = currentClone.GetComponentsInChildren<Rigidbody>(true);
        foreach (Rigidbody rb in rbs)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        // Centramos el clon usando sus renderers.
        CenterCloneByRenderers();

        // Ocultamos el objeto original.
        SetOriginalVisible(false);

        if (playerMovement != null)
            playerMovement.puedeMoverse = false;

        if (pointer3D != null)
            pointer3D.SetActive(false);

        if (exitCanvas != null)
            exitCanvas.SetActive(true);

        if (showDebug)
        {
            Debug.Log("===== DEBUG 360 =====");
            Debug.Log("Original: " + gameObject.name);
            Debug.Log("Visual Prefab: " + visualPrefab.name);
            Debug.Log("InspectPoint: " + inspectPoint.name);
            Debug.Log("InspectPoint mundo: " + inspectPoint.position);
            Debug.Log("InspectPoint local: " + inspectPoint.localPosition);
            Debug.Log("Wrapper mundo: " + visualWrapper.transform.position);
            Debug.Log("Clon localPosition final: " + currentClone.transform.localPosition);
            Debug.Log("Clon escala: " + currentClone.transform.localScale);
        }
    }

    void CenterCloneByRenderers()
    {
        if (currentClone == null) return;

        Renderer[] renderers = currentClone.GetComponentsInChildren<Renderer>(true);

        if (renderers == null || renderers.Length == 0)
        {
            Debug.LogWarning("El clon no tiene renderers para centrar: " + currentClone.name);
            return;
        }

        Bounds bounds = renderers[0].bounds;

        for (int i = 1; i < renderers.Length; i++)
        {
            bounds.Encapsulate(renderers[i].bounds);
        }

        Vector3 worldCenter = bounds.center;
        Vector3 localCenter = currentClone.transform.InverseTransformPoint(worldCenter);

        currentClone.transform.localPosition -= localCenter;

        if (showDebug)
        {
            Debug.Log("Centro visual mundo: " + worldCenter);
            Debug.Log("Centro visual local: " + localCenter);
        }
    }

    void SetOriginalVisible(bool visible)
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>(true);

        foreach (Renderer r in renderers)
            r.enabled = visible;
    }

    public void StopInspect()
    {
        if (!inspecting) return;

        inspecting = false;

        if (visualWrapper != null)
            Destroy(visualWrapper);
        else if (currentClone != null)
            Destroy(currentClone);

        SetOriginalVisible(true);

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

        if (visualWrapper == null) return;

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        visualWrapper.transform.Rotate(
            Vector3.up,
            -horizontal * rotationSpeed * Time.deltaTime,
            Space.World
        );

        visualWrapper.transform.Rotate(
            Vector3.right,
            vertical * rotationSpeed * Time.deltaTime,
            Space.World
        );

        if (Input.GetMouseButton(0))
        {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            visualWrapper.transform.Rotate(
                Vector3.up,
                -mouseX * mouseRotationSpeed,
                Space.World
            );

            visualWrapper.transform.Rotate(
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
