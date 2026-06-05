using UnityEngine;

/// <summary>
/// InspectablePickup: Script para objetos que se pueden inspeccionar en 360.
/// 
/// Igual que DrawerItemPickup pero:
/// - No requiere estar dentro de un cajón
/// - Muestra "Presiona B para Inspeccionar"
/// - El objeto vuelve a su posición original al presionar X
/// 
/// Configuración:
/// - El objeto DEBE tener un Collider en layer "Raycast Detect" o "PickupItem"
/// </summary>
public class InspectablePickup : MonoBehaviour
{
    [Header("Punto de inspección (frente a la cámara)")]
    public Transform inspectPoint;

    [Header("Cámara")]
    public Transform cameraTransform;

    [Header("Movimiento jugador (se busca automáticamente si no se asigna)")]
    public MovimientoVR2 playerMovement;

    [Header("UI")]
    public GameObject promptPanel;
    public TMPro.TMP_Text promptText;
    public GameObject exitCanvas;

    [Header("Puntero")]
    public GameObject pointer3D;

    [Header("Rotación en inspección")]
    public float rotationSpeed = 120f;
    public float mouseRotationSpeed = 5f;

    [Header("Escala en inspección")]
    public float inspectScale = 1f;

    public enum ItemState
    {
        Idle,           // En su lugar, esperando interacción
        Inspecting,     // Inspeccionándolo en 360
    }

    [Header("Estado (solo lectura)")]
    public ItemState estado = ItemState.Idle;

    // Referencias internas
    private Transform originalParent;
    private Vector3 originalLocalPosition;
    private Quaternion originalLocalRotation;
    private Vector3 originalLocalScale;
    private Quaternion lockedCameraRotation;
    private GameObject inspectWrapper;

    void Start()
    {
        // Guardar posición original
        originalParent = transform.parent;
        originalLocalPosition = transform.localPosition;
        originalLocalRotation = transform.localRotation;
        originalLocalScale = transform.localScale;

        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;

        if (playerMovement == null)
            playerMovement = FindFirstObjectByType<MovimientoVR2>();

        if (inspectPoint == null && cameraTransform != null)
        {
            GameObject ip = new GameObject("InspectPoint_" + gameObject.name);
            ip.transform.SetParent(cameraTransform);
            ip.transform.localPosition = new Vector3(0f, 0f, 0.5f);
            ip.transform.localRotation = Quaternion.identity;
            inspectPoint = ip.transform;
        }

        if (pointer3D == null)
        {
            Pointer3DController pointerCtrl = FindFirstObjectByType<Pointer3DController>();
            if (pointerCtrl != null)
                pointer3D = pointerCtrl.gameObject;
        }

        if (promptPanel != null)
            promptPanel.SetActive(false);

        AsegurarCollider();
    }

    void AsegurarCollider()
    {
        int raycastLayer = LayerMask.NameToLayer("Raycast Detect");

        if (raycastLayer >= 0 && gameObject.layer == 0)
            gameObject.layer = raycastLayer;

        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            foreach (Transform child in GetComponentsInChildren<Transform>())
                if (child.gameObject.layer == 0 && child.GetComponent<Renderer>() != null)
                    child.gameObject.layer = raycastLayer >= 0 ? raycastLayer : gameObject.layer;
            return;
        }

        Collider childCol = GetComponentInChildren<Collider>();
        if (childCol != null)
        {
            if (raycastLayer >= 0 && childCol.gameObject.layer == 0)
                childCol.gameObject.layer = raycastLayer;
            return;
        }

        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0) return;

        Bounds bounds = renderers[0].bounds;
        for (int i = 1; i < renderers.Length; i++)
            bounds.Encapsulate(renderers[i].bounds);

        BoxCollider box = gameObject.AddComponent<BoxCollider>();
        box.center = transform.InverseTransformPoint(bounds.center);
        box.size = new Vector3(
            Mathf.Abs(transform.InverseTransformVector(bounds.size).x),
            Mathf.Abs(transform.InverseTransformVector(bounds.size).y),
            Mathf.Abs(transform.InverseTransformVector(bounds.size).z)
        );
    }

    void Update()
    {
        if (estado == ItemState.Inspecting)
            UpdateInspeccionando();
    }

    /// <summary>
    /// Retorna true si el objeto puede inspeccionarse.
    /// </summary>
    public bool PuedeInspeccionar()
    {
        return estado == ItemState.Idle;
    }

    /// <summary>
    /// Inicia la inspección 360 del objeto.
    /// </summary>
    public void IniciarInspeccion()
    {
        if (estado != ItemState.Idle) return;

        estado = ItemState.Inspecting;

        if (cameraTransform != null)
            lockedCameraRotation = cameraTransform.rotation;

        Vector3 inspectPosition;
        if (inspectPoint != null)
        {
            inspectPosition = inspectPoint.position;
        }
        else if (cameraTransform != null)
        {
            inspectPosition = cameraTransform.position + cameraTransform.forward * 0.5f;
        }
        else
        {
            inspectPosition = transform.position + Vector3.up * 0.3f;
        }

        transform.SetParent(null);

        inspectWrapper = new GameObject("InspectWrapper_" + gameObject.name);
        inspectWrapper.transform.position = inspectPosition;
        inspectWrapper.transform.rotation = Quaternion.identity;

        transform.SetParent(inspectWrapper.transform);
        transform.localScale = originalLocalScale * inspectScale;

        CentrarEnWrapper();

        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;
        foreach (Collider c in GetComponentsInChildren<Collider>())
            c.enabled = false;

        if (playerMovement != null)
            playerMovement.puedeMoverse = false;

        if (pointer3D != null)
            pointer3D.SetActive(false);

        if (exitCanvas != null)
            exitCanvas.SetActive(true);

        if (exitCanvas == null)
        {
            InspectableObject360 inspector = FindFirstObjectByType<InspectableObject360>();
            if (inspector != null && inspector.exitCanvas != null)
            {
                exitCanvas = inspector.exitCanvas;
                exitCanvas.SetActive(true);
            }
        }

        if (promptPanel != null)
            promptPanel.SetActive(false);
    }

    void CentrarEnWrapper()
    {
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>(true);
        SkinnedMeshRenderer[] skinnedMeshes = GetComponentsInChildren<SkinnedMeshRenderer>(true);

        if (meshFilters.Length == 0 && skinnedMeshes.Length == 0)
        {
            Renderer[] renderers = GetComponentsInChildren<Renderer>(true);
            if (renderers.Length == 0) return;

            Bounds bounds = renderers[0].bounds;
            for (int i = 1; i < renderers.Length; i++)
                bounds.Encapsulate(renderers[i].bounds);

            transform.position -= (bounds.center - inspectWrapper.transform.position);
            return;
        }

        Vector3 sumCenters = Vector3.zero;
        int count = 0;

        foreach (MeshFilter mf in meshFilters)
        {
            if (mf.sharedMesh == null) continue;
            Vector3 meshWorldCenter = mf.transform.TransformPoint(mf.sharedMesh.bounds.center);
            sumCenters += meshWorldCenter;
            count++;
        }

        foreach (SkinnedMeshRenderer smr in skinnedMeshes)
        {
            if (smr.sharedMesh == null) continue;
            Vector3 meshWorldCenter = smr.transform.TransformPoint(smr.sharedMesh.bounds.center);
            sumCenters += meshWorldCenter;
            count++;
        }

        if (count == 0) return;

        Vector3 realCenter = sumCenters / count;
        transform.position -= (realCenter - inspectWrapper.transform.position);
    }

    void UpdateInspeccionando()
    {
        if (InputManagerCustom.PressX())
        {
            DevolverALugar();
            return;
        }

        if (inspectWrapper == null) return;

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        inspectWrapper.transform.Rotate(Vector3.up, -horizontal * rotationSpeed * Time.deltaTime, Space.World);
        inspectWrapper.transform.Rotate(Vector3.right, vertical * rotationSpeed * Time.deltaTime, Space.World);

        if (Input.GetMouseButton(0))
        {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            inspectWrapper.transform.Rotate(Vector3.up, -mouseX * mouseRotationSpeed, Space.World);
            inspectWrapper.transform.Rotate(Vector3.right, mouseY * mouseRotationSpeed, Space.World);
        }
    }

    void LateUpdate()
    {
        if (estado == ItemState.Inspecting && cameraTransform != null)
            cameraTransform.rotation = lockedCameraRotation;
    }

    /// <summary>
    /// Devuelve el objeto a su posición original.
    /// </summary>
    public void DevolverALugar()
    {
        if (estado != ItemState.Inspecting) return;

        estado = ItemState.Idle;

        transform.SetParent(originalParent);
        transform.localPosition = originalLocalPosition;
        transform.localRotation = originalLocalRotation;
        transform.localScale = originalLocalScale;

        if (inspectWrapper != null)
            Destroy(inspectWrapper);
        inspectWrapper = null;

        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = true;
        foreach (Collider c in GetComponentsInChildren<Collider>())
            c.enabled = true;

        if (playerMovement != null)
            playerMovement.puedeMoverse = true;

        if (pointer3D != null)
            pointer3D.SetActive(true);

        if (exitCanvas != null)
            exitCanvas.SetActive(false);

        // Limpiar referencia en Selected
        Selected selected = FindFirstObjectByType<Selected>();
        if (selected != null)
            selected.ClearInspectablePickupActivo();
    }

    public bool EstaInspeccionando()
    {
        return estado == ItemState.Inspecting;
    }
}
