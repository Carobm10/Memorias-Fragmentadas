using UnityEngine;

/// <summary>
/// Reduce la calidad de renderizado de objetos lejanos desactivando sombras
/// y reduciendo la frecuencia de scripts en objetos distantes.
/// Agregar a la cámara del jugador o a un manager.
/// </summary>
public class LODOptimizer : MonoBehaviour
{
    [Header("Configuración")]
    public Transform player;
    public float shadowDistance = 10f;
    public float checkInterval = 1f;

    [Header("Objetos con sombras costosas")]
    public Renderer[] renderersConSombra;

    private float nextCheck;

    void Start()
    {
        // Reducir distancia de sombras globalmente
        QualitySettings.shadowDistance = shadowDistance;

        // Limitar framerate si no es VR
        if (!UnityEngine.XR.XRSettings.enabled)
        {
            Application.targetFrameRate = 60;
        }
    }

    void Update()
    {
        if (Time.time < nextCheck) return;
        nextCheck = Time.time + checkInterval;

        if (player == null) return;

        OptimizarSombras();
    }

    void OptimizarSombras()
    {
        Vector3 playerPos = player.position;

        for (int i = 0; i < renderersConSombra.Length; i++)
        {
            if (renderersConSombra[i] == null) continue;

            float dist = Vector3.Distance(playerPos, renderersConSombra[i].transform.position);

            // Objetos lejos no proyectan sombras
            renderersConSombra[i].shadowCastingMode = dist > shadowDistance
                ? UnityEngine.Rendering.ShadowCastingMode.Off
                : UnityEngine.Rendering.ShadowCastingMode.On;
        }
    }
}
