using UnityEngine;

public class SystemDoor : MonoBehaviour
{
    public bool doorOpen = false;
    public float openOffsetY = 95f;
    public float smooth = 3f;

    private Quaternion closedRotation;
    private Quaternion openRotation;

    void Start()
    {
        closedRotation = transform.localRotation;
        openRotation = closedRotation * Quaternion.Euler(0f, openOffsetY, 0f);

        transform.localRotation = closedRotation; // fuerza que arranque cerrada
        doorOpen = false;
    }

    void Update()
    {
        Quaternion targetRotation = doorOpen ? openRotation : closedRotation;

        transform.localRotation = Quaternion.Slerp(
            transform.localRotation,
            targetRotation,
            smooth * Time.deltaTime
        );
    }

    public void ToggleDoor()
    {
        doorOpen = !doorOpen;
    }
}