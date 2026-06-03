using UnityEngine;

/// <summary>
/// Desactiva renderers y objetos pesados que están fuera de la habitación actual del jugador.
/// Coloca este script en un GameObject vacío y asigna las zonas.
/// Cada zona es un trigger collider que representa una habitación.
/// Los objetos de esa zona se activan cuando el jugador entra y se desactivan cuando sale.
/// </summary>
public class RoomBasedCulling : MonoBehaviour
{
    [System.Serializable]
    public class Room
    {
        public string nombre;
        public GameObject[] objetos;
        [HideInInspector] public bool jugadorDentro;
    }

    [Header("Configuración")]
    public Transform player;
    public float checkInterval = 0.5f;
    public float maxRenderDistance = 20f;

    [Header("Objetos pesados para desactivar por distancia")]
    public GameObject[] objetosLejanos;

    private float nextCheck;

    void Update()
    {
        if (Time.time < nextCheck) return;
        nextCheck = Time.time + checkInterval;

        if (player == null) return;

        CullingPorDistancia();
    }

    void CullingPorDistancia()
    {
        Vector3 playerPos = player.position;

        for (int i = 0; i < objetosLejanos.Length; i++)
        {
            if (objetosLejanos[i] == null) continue;

            float dist = Vector3.Distance(playerPos, objetosLejanos[i].transform.position);
            bool debeEstarActivo = dist <= maxRenderDistance;

            if (objetosLejanos[i].activeSelf != debeEstarActivo)
                objetosLejanos[i].SetActive(debeEstarActivo);
        }
    }
}
