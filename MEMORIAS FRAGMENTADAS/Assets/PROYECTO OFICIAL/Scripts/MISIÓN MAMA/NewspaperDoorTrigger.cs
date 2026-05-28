using UnityEngine;

public class NewspaperDoorTrigger : MonoBehaviour
{
    [Header("Configuración de misión")]
    [Tooltip("ID de la misión que controla esta puerta")]
    public string missionID = "periodico";

    [Tooltip("Item necesario para completar la misión")]
    public string requiredItemID = "monedas";

    [Header("Puerta")]
    [Tooltip("Referencia a la puerta principal que usa DoorInteractable")]
    public DoorInteractable puertaPrincipal;

    [Header("Periódico")]
    [Tooltip("Objeto del periódico que aparecerá al terminar la misión")]
    public GameObject periodicoObjeto;

    [Header("Debug")]
    public bool mostrarDebug = true;

    // ======================================================
    // MÉTODO PRINCIPAL DE INTERACCIÓN
    // ======================================================
    public void InteractuarPuertaPeriodico()
    {
        // --------------------------------------------------
        // 1. Revisar si la misión está activa
        // --------------------------------------------------
        if (!MissionManager.Instance.IsMissionActive(missionID))
        {
            if (mostrarDebug)
                Debug.Log("[NewspaperDoorTrigger] La misión periódico no está activa.");

            return;
        }

        // --------------------------------------------------
        // 2. Revisar si el jugador tiene monedas
        // --------------------------------------------------
        if (!InventoryManager.Instance.HasItem(requiredItemID))
        {
            Debug.Log("NIÑO: Necesito las monedas primero.");

            return;
        }

        // --------------------------------------------------
        // 3. Abrir puerta usando DoorInteractable
        // --------------------------------------------------
        if (puertaPrincipal != null)
        {
            puertaPrincipal.ToggleDoor();

            if (mostrarDebug)
                Debug.Log("[NewspaperDoorTrigger] Puerta principal abierta.");
        }
        else
        {
            Debug.LogWarning("[NewspaperDoorTrigger] No hay DoorInteractable asignado.");
        }

        // --------------------------------------------------
        // 4. Completar misión del periódico
        // --------------------------------------------------
        MissionManager.Instance.CompleteMission(missionID);

        if (mostrarDebug)
            Debug.Log("[NewspaperDoorTrigger] Misión periódico completada.");

        // --------------------------------------------------
        // 5. Activar periódico después de la cinemática
        // --------------------------------------------------
        if (periodicoObjeto != null)
        {
            periodicoObjeto.SetActive(true);

            if (mostrarDebug)
                Debug.Log("[NewspaperDoorTrigger] Periódico activado.");
        }
        else
        {
            Debug.LogWarning("[NewspaperDoorTrigger] No hay periódico asignado.");
        }

        // --------------------------------------------------
        // 6. Aquí después conectaremos:
        // - sonido vendedor
        // - pasos
        // - bicicletas
        // - cinemática
        // --------------------------------------------------
        if (mostrarDebug)
            Debug.Log("[NewspaperDoorTrigger] Aquí luego irá la cinemática del periódico.");
    }
}