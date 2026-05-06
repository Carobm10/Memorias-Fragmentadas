using UnityEngine;
using UnityEngine.Rendering;

public class Pointer3DController : MonoBehaviour
{
    [Header("Distancia fija frente a la cámara")]
    public float distanceFromCamera = 0.35f;

    [Header("Overlay (sobre todo)")]
    [Tooltip("Si está activo, el puntero se renderiza encima de todo con una cámara overlay.")]
    public bool sobreponerPuntero = true;

    [Tooltip("Layer usada por el puntero overlay (0-31).")]
    [Range(0, 31)]
    public int overlayLayer = 2;

    [Tooltip("Depth de la cámara overlay. Debe ser mayor que la cámara principal.")]
    public int overlayCameraDepth = 100;

    [Header("Escala")]
    [Tooltip("Si está activo, fuerza escala uniforme para que la esfera no se deforme.")]
    public bool mantenerEscalaUniforme = true;

    [Header("Color")]
    public Renderer pointerRenderer;
    public Color normalColor = Color.white;
    public Color detectedColor = Color.green;

    private Camera mainCamera;
    private Camera overlayCamera;
    private int? cullingMaskOriginal;
    private float escalaUniforme = 1f;
    private float escalaMundo = 1f;
    private Material pointerMaterial;
    private bool detectedActual;
    private float alphaActual = 1f;
    private bool materialPreparadoParaFade;

    void Start()
    {
        mainCamera = Camera.main;

        if (pointerRenderer == null)
            pointerRenderer = GetComponent<Renderer>();

        if (pointerRenderer != null)
        {
            pointerMaterial = pointerRenderer.material;
        }

        escalaUniforme = (transform.localScale.x + transform.localScale.y + transform.localScale.z) / 3f;
        escalaMundo = (transform.lossyScale.x + transform.lossyScale.y + transform.lossyScale.z) / 3f;

        if (sobreponerPuntero)
            ConfigurarOverlayCamera();

        SetDetected(false);
    }

    void LateUpdate()
    {
        if (mainCamera == null) return;

        transform.position = mainCamera.transform.position + mainCamera.transform.forward * distanceFromCamera;
        transform.rotation = mainCamera.transform.rotation;

        if (mantenerEscalaUniforme)
            AjustarEscalaUniforme();

        // IMPORTANTE:
        // Aquí NO tocamos la escala.
        // El tamaño queda manual desde el Inspector.
    }

    public void SetDetected(bool detected)
    {
        detectedActual = detected;
        AplicarColorActual();
    }

    public void SetAlpha(float alpha)
    {
        alphaActual = Mathf.Clamp01(alpha);
        AplicarColorActual();
    }

    private void ConfigurarOverlayCamera()
    {
        if (mainCamera == null)
        {
            return;
        }

        if (!cullingMaskOriginal.HasValue)
        {
            cullingMaskOriginal = mainCamera.cullingMask;
        }

        gameObject.layer = overlayLayer;
        mainCamera.cullingMask &= ~(1 << overlayLayer);

        if (overlayCamera == null)
        {
            GameObject cameraObject = new GameObject("PointerOverlayCamera");
            cameraObject.transform.SetParent(mainCamera.transform, false);
            overlayCamera = cameraObject.AddComponent<Camera>();
        }

        overlayCamera.CopyFrom(mainCamera);
        overlayCamera.clearFlags = CameraClearFlags.Depth;
        overlayCamera.cullingMask = 1 << overlayLayer;
        overlayCamera.depth = Mathf.Max(mainCamera.depth + 1f, overlayCameraDepth);
        overlayCamera.useOcclusionCulling = false;
    }

    private void AplicarColorActual()
    {
        if (pointerRenderer == null)
        {
            return;
        }

        if (pointerMaterial == null)
        {
            pointerMaterial = pointerRenderer.material;
        }

        if (pointerMaterial == null)
        {
            return;
        }

        if (alphaActual < 1f)
        {
            PrepararMaterialParaFade(pointerMaterial);
        }

        Color color = detectedActual ? detectedColor : normalColor;
        color.a *= alphaActual;
        pointerMaterial.color = color;

        if (!pointerRenderer.enabled)
        {
            pointerRenderer.enabled = true;
        }
    }

    private void PrepararMaterialParaFade(Material material)
    {
        if (materialPreparadoParaFade || material == null)
        {
            return;
        }

        if (material.HasProperty("_Surface"))
        {
            material.SetFloat("_Surface", 1f);
            material.SetOverrideTag("RenderType", "Transparent");
            material.SetInt("_SrcBlend", (int)BlendMode.SrcAlpha);
            material.SetInt("_DstBlend", (int)BlendMode.OneMinusSrcAlpha);
            material.SetInt("_ZWrite", 0);
            material.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
            material.DisableKeyword("_SURFACE_TYPE_OPAQUE");
            material.renderQueue = 3000;
            materialPreparadoParaFade = true;
            return;
        }

        if (material.HasProperty("_Mode"))
        {
            material.SetFloat("_Mode", 3f);
            material.SetOverrideTag("RenderType", "Transparent");
            material.SetInt("_SrcBlend", (int)BlendMode.SrcAlpha);
            material.SetInt("_DstBlend", (int)BlendMode.OneMinusSrcAlpha);
            material.SetInt("_ZWrite", 0);
            material.DisableKeyword("_ALPHATEST_ON");
            material.EnableKeyword("_ALPHABLEND_ON");
            material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            material.renderQueue = 3000;
            materialPreparadoParaFade = true;
        }
    }

    private void AjustarEscalaUniforme()
    {
        Vector3 parentScale = transform.parent != null ? transform.parent.lossyScale : Vector3.one;
        float scaleX = Mathf.Max(0.0001f, Mathf.Abs(parentScale.x));
        float scaleY = Mathf.Max(0.0001f, Mathf.Abs(parentScale.y));
        float scaleZ = Mathf.Max(0.0001f, Mathf.Abs(parentScale.z));

        transform.localScale = new Vector3(
            escalaMundo / scaleX,
            escalaMundo / scaleY,
            escalaMundo / scaleZ
        );
    }

    private void OnDisable()
    {
        if (mainCamera != null && cullingMaskOriginal.HasValue)
        {
            mainCamera.cullingMask = cullingMaskOriginal.Value;
        }
    }
}