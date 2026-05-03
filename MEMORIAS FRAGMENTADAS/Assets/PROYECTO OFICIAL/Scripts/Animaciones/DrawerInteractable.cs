using UnityEngine;

/// <summary>
/// DrawerInteractable controla la apertura y cierre de cajones.
/// 
/// Este script NO detecta botones directamente.
/// La interacción se hace desde Selected.cs:
/// - El jugador mira el cajón.
/// - Presiona B.
/// - Selected.cs llama ToggleDrawer().
/// 
/// Configuración recomendada:
/// - openDistance: entre 0.05 y 0.15 (depende del modelo).
/// - localOpenDirection: define hacia dónde se abre el cajón.
/// 
/// IMPORTANTE:
/// - No usar Rigidbody.
/// - Debe tener Collider.
/// - Debe estar en layer "RayCast Detect".
/// </summary>
public class DrawerInteractable : MonoBehaviour
{
    [Header("Configuración de apertura")]
    public bool startsOpen = false;
    public float openDistance = 0.1f;
    public float speed = 3f;

    [Header("Dirección local de apertura")]
    public Vector3 localOpenDirection = new Vector3(0f, 0f, -1f);

    [Header("Estado del cajón")]
    public bool isOpen = false;
    public bool isLocked = false;

    private Vector3 closedLocalPosition;
    private Vector3 openLocalPosition;

    void Start()
    {
        closedLocalPosition = transform.localPosition;
        openLocalPosition = closedLocalPosition + localOpenDirection.normalized * openDistance;

        isOpen = startsOpen;
        transform.localPosition = isOpen ? openLocalPosition : closedLocalPosition;
    }

    void Update()
    {
        Vector3 targetPosition = isOpen ? openLocalPosition : closedLocalPosition;

        transform.localPosition = Vector3.Lerp(
            transform.localPosition,
            targetPosition,
            speed * Time.deltaTime
        );
    }

    /// <summary>
    /// Alterna el estado del cajón (abrir/cerrar).
    /// </summary>
    public void ToggleDrawer()
    {
        if (isLocked)
        {
            Debug.Log("El cajón está bloqueado: " + gameObject.name);
            return;
        }

        isOpen = !isOpen;
    }

    /// <summary>
    /// Abre el cajón directamente.
    /// </summary>
    public void OpenDrawer()
    {
        if (isLocked) return;
        isOpen = true;
    }

    /// <summary>
    /// Cierra el cajón.
    /// </summary>
    public void CloseDrawer()
    {
        isOpen = false;
    }

    /// <summary>
    /// Bloquea el cajón.
    /// </summary>
    public void LockDrawer()
    {
        isLocked = true;
    }

    /// <summary>
    /// Desbloquea el cajón.
    /// </summary>
    public void UnlockDrawer()
    {
        isLocked = false;
    }
}