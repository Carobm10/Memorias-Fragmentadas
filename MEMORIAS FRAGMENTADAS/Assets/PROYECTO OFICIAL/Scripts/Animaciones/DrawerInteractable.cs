using UnityEngine;

public class DrawerInteractable : MonoBehaviour
{
    [Header("Configuración")]
    public bool startsOpen = false;
    public float openDistance = 0.35f;
    public float speed = 3f;

    [Header("Dirección local de apertura")]
    public Vector3 localOpenDirection = new Vector3(0f, 0f, -1f);

    [Header("Estado")]
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

    public void ToggleDrawer()
    {
        if (isLocked) return;

        isOpen = !isOpen;
    }

    public void OpenDrawer()
    {
        if (isLocked) return;

        isOpen = true;
    }

    public void CloseDrawer()
    {
        isOpen = false;
    }

    public void LockDrawer()
    {
        isLocked = true;
    }

    public void UnlockDrawer()
    {
        isLocked = false;
    }
}