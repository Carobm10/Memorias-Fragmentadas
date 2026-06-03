using UnityEngine;

/// <summary>
/// Desactiva objetos que no están dentro del frustum de la cámara (lo que el jugador ve).
/// Más agresivo que el culling nativo de Unity para objetos con scripts pesados.
/// Solo afecta a los objetos asignados manualmente (objetos grandes/pesados).
/// </summary>
public class FrustumCullingManager : MonoBehaviour
{
    [Header("Cámara")]
    public Camera mainCamera;

    [Header("Objetos pesados para frustum culling manual")]
    public GameObject[] objetosPesados;

    [Header("Configuración")]
    [Tooltip("Cada cuántos frames se revisa (1 = cada frame, 3 = cada 3 frames)")]
    public int checkEveryNFrames = 3;

    [Tooltip("Margen extra alrededor del frustum para evitar pop-in")]
    public float boundsPadding = 2f;

    private Plane[] planes;
    private Bounds[] boundsCache;
    private int frameCount;

    void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        // Cachear bounds para no recalcular cada frame
        boundsCache = new Bounds[objetosPesados.Length];
        for (int i = 0; i < objetosPesados.Length; i++)
        {
            if (objetosPesados[i] == null) continue;

            Renderer r = objetosPesados[i].GetComponentInChildren<Renderer>();
            if (r != null)
            {
                boundsCache[i] = r.bounds;
                boundsCache[i].Expand(boundsPadding);
            }
            else
            {
                // Si no tiene renderer, usar un bounds basado en posición
                boundsCache[i] = new Bounds(objetosPesados[i].transform.position, Vector3.one * 3f);
            }
        }
    }

    void Update()
    {
        frameCount++;
        if (frameCount % checkEveryNFrames != 0) return;

        if (mainCamera == null) return;

        planes = GeometryUtility.CalculateFrustumPlanes(mainCamera);

        for (int i = 0; i < objetosPesados.Length; i++)
        {
            if (objetosPesados[i] == null) continue;

            // Actualizar posición del bounds
            Renderer r = objetosPesados[i].GetComponentInChildren<Renderer>();
            if (r != null)
            {
                boundsCache[i] = r.bounds;
                boundsCache[i].Expand(boundsPadding);
            }
            else
            {
                boundsCache[i] = new Bounds(objetosPesados[i].transform.position, Vector3.one * 3f);
            }

            bool visible = GeometryUtility.TestPlanesAABB(planes, boundsCache[i]);

            if (objetosPesados[i].activeSelf != visible)
                objetosPesados[i].SetActive(visible);
        }
    }
}
