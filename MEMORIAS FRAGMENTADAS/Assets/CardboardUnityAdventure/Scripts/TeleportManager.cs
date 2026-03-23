using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// GESTIONAR EL PROCESO DE TELETRANSPORTACIÓN
public class TeleportManager : MonoBehaviour
{
    public static TeleportManager Instance;
    public GameObject Player;
    private GameObject lastTeleportPoint;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    // Función que desactiva el punto de teletransportación al cual nos hemos teletransportado
    public void DisableTeleportPoint(GameObject teleportPoint)
    {
        if (lastTeleportPoint != null)
        {
            lastTeleportPoint.SetActive(true);
        }

        teleportPoint.SetActive(false);
        lastTeleportPoint = teleportPoint;

#if UNITY_EDITOR
        // Si luego necesitas algo del simulador, se agrega aquí
#endif
    }
}