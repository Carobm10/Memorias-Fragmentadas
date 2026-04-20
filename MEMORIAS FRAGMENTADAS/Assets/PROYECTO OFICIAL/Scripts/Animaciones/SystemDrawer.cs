using UnityEngine;

public class SystemDrawer : MonoBehaviour
{
    public bool drawerOpen = false;
    public float openDistance = 0.3f;
    public float smooth = 3f;

    [Header("Dirección de apertura")]
    public Vector3 openDirection = new Vector3(0f, 0f, -1f);

    private Vector3 closedPosition;
    private Vector3 openPosition;

    void Start()
    {
        closedPosition = transform.localPosition;
        openPosition = closedPosition + openDirection.normalized * openDistance;

        transform.localPosition = closedPosition; // arranca cerrado
        drawerOpen = false;
    }

    void Update()
    {
        Vector3 targetPosition = drawerOpen ? openPosition : closedPosition;

        transform.localPosition = Vector3.Lerp(
            transform.localPosition,
            targetPosition,
            smooth * Time.deltaTime
        );
    }

    public void ToggleDrawer()
    {
        drawerOpen = !drawerOpen;
        Debug.Log("Cajón cambiado. Abierto: " + drawerOpen);
    }
}