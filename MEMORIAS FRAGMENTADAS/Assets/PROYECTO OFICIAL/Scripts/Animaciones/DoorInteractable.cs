using UnityEngine;

public class DoorInteractable : MonoBehaviour
{
    [Header("Configuración")]
    public bool startsOpen = false;
    public float openAngle = 95f;
    public float speed = 3f;
    public Vector3 rotationAxis = Vector3.up;

    [Header("Estado")]
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
        Quaternion target = isOpen ? openRotation : closedRotation;

        transform.localRotation = Quaternion.Slerp(
            transform.localRotation,
            target,
            speed * Time.deltaTime
        );
    }

    public void ToggleDoor()
    {
        if (isLocked) return;

        isOpen = !isOpen;
    }

    public void OpenDoor()
    {
        if (isLocked) return;

        isOpen = true;
    }

    public void CloseDoor()
    {
        isOpen = false;
    }

    public void LockDoor()
    {
        isLocked = true;
    }

    public void UnlockDoor()
    {
        isLocked = false;
    }
}