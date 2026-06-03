using UnityEngine;

/// <summary>
/// Configuraciones globales de rendimiento.
/// Coloca este script en un GameObject vacío en la escena BASE.
/// Ajusta los parámetros desde el Inspector según el hardware objetivo.
/// </summary>
public class PerformanceManager : MonoBehaviour
{
    [Header("Frame Rate")]
    [Tooltip("Target FPS. Usa 30 para móviles/Cardboard, 60 para PC")]
    public int targetFrameRate = 60;

    [Header("Renderizado")]
    [Tooltip("Distancia máxima de sombras en tiempo real")]
    public float shadowDistance = 15f;

    [Tooltip("Resolución de sombras: 0=Low, 1=Medium, 2=High")]
    [Range(0, 2)]
    public int shadowResolution = 1;

    [Header("Física")]
    [Tooltip("Frecuencia de FixedUpdate (default 0.02 = 50fps)")]
    public float fixedTimestep = 0.02f;

    [Header("Cámara")]
    [Tooltip("Far clip plane de la cámara principal")]
    public float cameraFarClip = 50f;

    [Header("LOD")]
    [Tooltip("Bias de LOD - valores altos = LODs más bajos antes")]
    public float lodBias = 1f;

    void Awake()
    {
        // Frame rate
        Application.targetFrameRate = targetFrameRate;
        QualitySettings.vSyncCount = 0; // Desactivar VSync para respetar targetFrameRate

        // Sombras
        QualitySettings.shadowDistance = shadowDistance;
        QualitySettings.shadowResolution = (ShadowResolution)shadowResolution;

        // Física
        Time.fixedDeltaTime = fixedTimestep;

        // LOD
        QualitySettings.lodBias = lodBias;

        // Cámara
        if (Camera.main != null)
        {
            Camera.main.farClipPlane = cameraFarClip;
        }
    }

    void Start()
    {
        // Desactivar objetos con Animator que no estén visibles
        OptimizarAnimators();
    }

    void OptimizarAnimators()
    {
        // Configurar todos los Animators para que no se actualicen cuando no son visibles
        Animator[] animators = FindObjectsByType<Animator>(FindObjectsSortMode.None);
        foreach (Animator anim in animators)
        {
            if (anim == null) continue;
            anim.cullingMode = AnimatorCullingMode.CullUpdateTransforms;
        }
    }
}
