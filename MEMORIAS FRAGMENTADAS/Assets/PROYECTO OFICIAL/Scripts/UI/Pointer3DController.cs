using UnityEngine;

public class Pointer3DController : MonoBehaviour
{
    [Header("Distancia fija frente a la cámara")]
    public float distanceFromCamera = 0.35f;

    [Header("Color")]
    public Renderer pointerRenderer;
    public Color normalColor = Color.white;
    public Color detectedColor = Color.green;

    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;

        if (pointerRenderer == null)
            pointerRenderer = GetComponent<Renderer>();

        SetDetected(false);
    }

    void LateUpdate()
    {
        if (mainCamera == null) return;

        transform.position = mainCamera.transform.position + mainCamera.transform.forward * distanceFromCamera;
        transform.rotation = mainCamera.transform.rotation;

        // IMPORTANTE:
        // Aquí NO tocamos la escala.
        // El tamaño queda manual desde el Inspector.
    }

    public void SetDetected(bool detected)
    {
        if (pointerRenderer != null)
            pointerRenderer.material.color = detected ? detectedColor : normalColor;
    }
}