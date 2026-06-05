using UnityEngine;

/// <summary>
/// DrawerItemPickup: Script para objetos dentro de cajones.
/// 
/// Permite al jugador:
/// 1. Mirar el objeto dentro del cajón abierto (se muestra prompt)
/// 2. Presionar B para sacar el objeto y verlo en 360
/// 3. Rotar el objeto con joystick/mouse
/// 4. Presionar X para devolver el objeto al cajón
/// 
/// Mientras el objeto está fuera, el cajón se bloquea y no se puede cerrar.
/// 
/// Configuración:
/// - El objeto DEBE estar como hijo del cajón (DrawerInteractable)
/// - El objeto DEBE tener un Collider en layer "Raycast Detect" o "PickupItem"
/// - Asignar inspectPoint (punto frente a la cámara donde se muestra el objeto)
/// - Asignar playerMovement para bloquear movimiento durante inspección
/// </summary>
public class DrawerItemPickup : MonoBehaviour
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
        InDrawer,       // Dentro del cajón, se mueve con él
        Inspecting,     // Sacado, viéndolo en 360
    }

    [Header("Estado (solo lectura)")]
    public ItemState estado = ItemState.InDrawer;

    // Referencias internas
    private DrawerInteractable parentDrawer;
    private Transform originalParent;
    private Vector3 originalLocalPosition;
    private Quaternion originalLocalRotation;
    private Vector3 originalLocalScale;
    private Quaternion lockedCameraRotation;
    private bool jugadorMirando = false;

    void Start()
    {
        // Buscar el cajón padre
        parentDrawer = GetComponentInParent<DrawerInteractable>();

        // Guardar posición original dentro del cajón
        originalParent = transform.parent;
        originalLocalPosition = transform.localPosition;
        originalLocalRotation = transform.localRotation;
        originalLocalScale = transform.localScale;

        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;

        // Buscar MovimientoVR2 automáticamente si no está asignado
        if (playerMovement == null)
            playerMovement = FindFirstObjectByType<MovimientoVR2>();

        // Buscar inspectPoint automáticamente si no está asignado
        if (inspectPoint == null && cameraTransform != null)
        {
            GameObject ip = new GameObject("InspectPoint_" + gameObject.name);
            ip.transform.SetParent(cameraTransform);
            ip.transform.localPosition = new Vector3(0f, 0f, 0.5f);
            ip.transform.localRotation = Quaternion.identity;
            inspectPoint = ip.transform;
        }

        // Buscar pointer3D automáticamente si no está asignado
        if (pointer3D == null)
        {
            Pointer3DController pointerCtrl = FindFirstObjectByType<Pointer3DController>();
            if (pointerCtrl != null)
                pointer3D = pointerCtrl.gameObject;
        }

        if (promptPanel != null)
            promptPanel.SetActive(false);
    }

    void Update()
    {
        switch (estado)
        {
            case ItemState.InDrawer:
                UpdateEnCajon();
                break;
            case ItemState.Inspecting:
                UpdateInspeccionando();
                break;
        }
    }

    // =============================================
    // ESTADO: Dentro del cajón
    // =============================================

    void UpdateEnCajon()
    {
        // El prompt se maneja desde Selected.cs
        // Aquí solo nos aseguramos de que siga en su lugar
    }

    /// <summary>
    /// Llamado por Selected.cs cuando el jugador mira este objeto.
    /// </summary>
    public void SetMirando(bool mirando)
    {
        jugadorMirando = mirando;
    }

    /// <summary>
    /// Llamado por Selected.cs cuando el jugador mira y está en el cajón.
    /// Retorna true si el cajón está abierto y el objeto puede sacarse.
    /// </summary>
    public bool PuedeSacarse()
    {
        if (estado != ItemState.InDrawer) return false;
        if (parentDrawer == null) return true; // Si no tiene cajón padre, siempre se puede
        return parentDrawer.EstaAbierto();
    }

    /// <summary>
    /// Saca el objeto del cajón y lo pone frente al jugador para inspección 360.
    /// </summary>
    public void SacarParaInspeccion()
    {
        if (estado != ItemState.InDrawer) return;
        if (!PuedeSacarse()) return;

        estado = ItemState.Inspecting;

        // Bloquear cajón
        if (parentDrawer != null)
            parentDrawer.LockDrawer();

        // Bloquear cámara
        if (cameraTransform != null)
            lockedCameraRotation = cameraTransform.rotation;

        // Desparentar del cajón y mover al punto de inspección
        transform.SetParent(null);

        if (inspectPoint != null)
        {
            transform.position = inspectPoint.position;
            transform.rotation = Quaternion.identity;
        }

        transform.localScale = originalLocalScale * inspectScale;

        // Desactivar collider para que el raycast no interfiera
        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        // Bloquear movimiento
        if (playerMovement != null)
            playerMovement.puedeMoverse = false;

        // Ocultar pointer pero NO destruirlo
        if (pointer3D != null)
            pointer3D.SetActive(false);

        if (exitCanvas != null)
            exitCanvas.SetActive(true);

        if (promptPanel != null)
            promptPanel.SetActive(false);
    }

    // =============================================
    // ESTADO: Inspeccionando (fuera del cajón, rotando 360)
    // =============================================

    void UpdateInspeccionando()
    {
        // Mantener en el punto de inspección (por si la cámara se movió)
        if (inspectPoint != null)
            transform.position = inspectPoint.position;

        // Salir con X
        if (InputManagerCustom.PressX())
        {
            DevolverACajon();
            return;
        }

        // Rotación con joystick
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        transform.Rotate(Vector3.up, -horizontal * rotationSpeed * Time.deltaTime, Space.World);
        transform.Rotate(Vector3.right, vertical * rotationSpeed * Time.deltaTime, Space.World);

        // Rotación con mouse
        if (Input.GetMouseButton(0))
        {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            transform.Rotate(Vector3.up, -mouseX * mouseRotationSpeed, Space.World);
            transform.Rotate(Vector3.right, mouseY * mouseRotationSpeed, Space.World);
        }
    }

    void LateUpdate()
    {
        // Bloquear rotación de cámara mientras inspecciona
        if (estado == ItemState.Inspecting && cameraTransform != null)
            cameraTransform.rotation = lockedCameraRotation;
    }

    /// <summary>
    /// Devuelve el objeto a su posición original dentro del cajón.
    /// </summary>
    public void DevolverACajon()
    {
        if (estado != ItemState.Inspecting) return;

        estado = ItemState.InDrawer;

        // Reparentar al cajón
        transform.SetParent(originalParent);
        transform.localPosition = originalLocalPosition;
        transform.localRotation = originalLocalRotation;
        transform.localScale = originalLocalScale;

        // Reactivar collider
        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = true;

        // Desbloquear cajón
        if (parentDrawer != null)
            parentDrawer.UnlockDrawer();

        // Devolver movimiento
        if (playerMovement != null)
            playerMovement.puedeMoverse = true;

        // Mostrar pointer
        if (pointer3D != null)
            pointer3D.SetActive(true);

        if (exitCanvas != null)
            exitCanvas.SetActive(false);
    }

    /// <summary>
    /// Retorna si está en modo inspección.
    /// </summary>
    public bool EstaInspeccionando()
    {
        return estado == ItemState.Inspecting;
    }
}
