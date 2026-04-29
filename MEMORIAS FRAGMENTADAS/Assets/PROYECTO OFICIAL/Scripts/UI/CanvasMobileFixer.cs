using UnityEngine;
using UnityEngine.UI;

public class CanvasMobileFixer : MonoBehaviour
{
    [Header("Configuración global UI móvil")]
    public Vector2 referenceResolution = new Vector2(1920, 1080);
    public bool aplicarEnStart = true;

    void Start()
    {
        if (aplicarEnStart)
            ArreglarTodosLosCanvas();
    }

    [ContextMenu("Arreglar todos los Canvas")]
    public void ArreglarTodosLosCanvas()
    {
        Canvas[] canvases = FindObjectsOfType<Canvas>(true);

        int order = 100;

        foreach (Canvas canvas in canvases)
        {
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = order;
            canvas.pixelPerfect = false;

            CanvasScaler scaler = canvas.GetComponent<CanvasScaler>();
            if (scaler == null)
                scaler = canvas.gameObject.AddComponent<CanvasScaler>();

            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = referenceResolution;
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;

            GraphicRaycaster raycaster = canvas.GetComponent<GraphicRaycaster>();
            if (raycaster == null)
                canvas.gameObject.AddComponent<GraphicRaycaster>();

            RectTransform rect = canvas.GetComponent<RectTransform>();
            if (rect != null)
            {
                rect.localScale = Vector3.one;
                rect.localPosition = Vector3.zero;
                rect.localRotation = Quaternion.identity;
            }

            CanvasGroup group = canvas.GetComponent<CanvasGroup>();
            if (group != null)
            {
                group.alpha = 1f;
                group.interactable = true;
                group.blocksRaycasts = true;
            }

            Debug.Log("CANVAS FIXED: " + canvas.name + " / SortOrder: " + order);

            order += 10;
        }
    }
}