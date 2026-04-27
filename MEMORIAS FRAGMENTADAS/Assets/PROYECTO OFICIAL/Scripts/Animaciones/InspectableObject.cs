using UnityEngine;

public class InspectableObject : MonoBehaviour
{
    [Header("Velocidad de rotación")]
    public float rotationSpeed = 120f;

    [Header("Estado")]
    public bool isInspecting = false;

    [Header("Referencias")]
    public Transform inspectPoint;
    public MonoBehaviour playerMovementScript;

    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private Transform originalParent;

    public void ToggleInspect()
    {
        if (!isInspecting)
        {
            StartInspect();
        }
        else
        {
            StopInspect();
        }
    }

    void StartInspect()
    {
        if (inspectPoint == null)
        {
            Debug.LogWarning("No hay inspectPoint asignado en " + gameObject.name);
            return;
        }

        isInspecting = true;

        originalPosition = transform.position;
        originalRotation = transform.rotation;
        originalParent = transform.parent;

        transform.SetParent(inspectPoint);
        transform.position = inspectPoint.position;
        transform.rotation = inspectPoint.rotation;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = false;
        }

        if (playerMovementScript != null)
        {
            playerMovementScript.enabled = false;
        }
    }

    void StopInspect()
    {
        isInspecting = false;

        transform.SetParent(originalParent);
        transform.position = originalPosition;
        transform.rotation = originalRotation;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }

        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = true;
        }

        if (playerMovementScript != null)
        {
            playerMovementScript.enabled = true;
        }
    }

    void Update()
    {
        if (!isInspecting) return;

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        transform.Rotate(Vector3.up, -horizontal * rotationSpeed * Time.deltaTime, Space.World);
        transform.Rotate(Vector3.right, vertical * rotationSpeed * Time.deltaTime, Space.World);
    }
}