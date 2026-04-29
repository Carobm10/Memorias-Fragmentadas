using UnityEngine;

public class CameraLockController : MonoBehaviour
{
    [Header("Cámara a bloquear")]
    public Transform cameraTransform;

    private bool isLocked = false;
    private Quaternion lockedRotation;

    void Start()
    {
        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;
    }

    void LateUpdate()
    {
        if (!isLocked || cameraTransform == null) return;

        cameraTransform.rotation = lockedRotation;
    }

    public void LockCamera()
    {
        if (cameraTransform == null) return;

        lockedRotation = cameraTransform.rotation;
        isLocked = true;
    }

    public void UnlockCamera()
    {
        isLocked = false;
    }
}