using UnityEngine;

/// <summary>
/// DoorInteractable controla la apertura y cierre de una puerta.
/// 
/// Este script NO detecta botones directamente.
/// La interacción se hace desde Selected.cs:
/// - El jugador mira la puerta.
/// - Selected.cs detecta DoorInteractable.
/// - Si presiona B, llama ToggleDoor().
/// 
/// Configuración recomendada:
/// - Puertas grandes: Open Angle 90 o 95.
/// - Puertas pequeñas: Open Angle 45.
/// - Si abre al revés: usa ángulo negativo.
/// - Rotation Axis normalmente es Y = 1.
/// 
/// IMPORTANTE:
/// No usar Rigidbody en puertas.
/// La puerta debe tener Collider.
/// El objeto debe estar en la layer RayCast Detect.
/// </summary>
public class DoorInteractable : MonoBehaviour
{
    [Header("Configuración de apertura")]
    public bool startsOpen = false;
    public float openAngle = 95f;
    public float speed = 3f;
    public Vector3 rotationAxis = Vector3.up;

    [Header("Estado de la puerta")]
    public bool isOpen = false;
    public bool isLocked = false;

    private Quaternion closedRotation;
    private Quaternion openRotation;

    void Start()
    {
        closedRotation = transform.localRotation;
        openRotation = closedRotation * Quaternion.AngleAxis(openAngle, rotationAxis);

        isOpen = startsOpen;
        transform.localRotation = isOpen ? openRotation : closedRotation;
    }

    void Update()
    {
        Quaternion targetRotation = isOpen ? openRotation : closedRotation;

        transform.localRotation = Quaternion.Slerp(
            transform.localRotation,
            targetRotation,
            speed * Time.deltaTime
        );
    }

    /// <summary>
    /// Cambia el estado de la puerta:
    /// si está cerrada, la abre;
    /// si está abierta, la cierra.
    /// </summary>
    public void ToggleDoor()
    {
        if (isLocked)
        {
            Debug.Log("La puerta está bloqueada: " + gameObject.name);
            return;
        }

        isOpen = !isOpen;
    }

    /// <summary>
    /// Abre la puerta directamente.
    /// Se usa, por ejemplo, en la misión del clóset.
    /// </summary>
    public void OpenDoor()
    {
        if (isLocked)
        {
            Debug.Log("La puerta está bloqueada: " + gameObject.name);
            return;
        }

        isOpen = true;
    }

    /// <summary>
    /// Cierra la puerta directamente.
    /// </summary>
    public void CloseDoor()
    {
        isOpen = false;
    }

    /// <summary>
    /// Bloquea la puerta para que no pueda abrirse/cerrarse.
    /// </summary>
    public void LockDoor()
    {
        isLocked = true;
    }

    /// <summary>
    /// Desbloquea la puerta.
    /// </summary>
    public void UnlockDoor()
    {
        isLocked = false;
    }
}