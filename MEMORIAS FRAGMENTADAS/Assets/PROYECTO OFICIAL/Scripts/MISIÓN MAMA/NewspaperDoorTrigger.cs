using UnityEngine;

public class NewspaperDoorTrigger : MonoBehaviour
{
    [Header("Periódico")]
    public GameObject periodicoObjeto;
    [Header("Configuración")]
    public string missionID = "periodico";
    public string requiredItemID = "monedas";

    [Header("Opcional")]
    public DoorInteractable puertaPrincipal;

    [Header("Debug")]
    public bool mostrarDebug = true;

    public void InteractuarPuertaPeriodico()
    {
        if (!MissionManager.Instance.IsMissionActive(missionID))
        {
            if (mostrarDebug)
                Debug.Log("[NewspaperDoorTrigger] La misión periódico no está activa.");

            return;
        }

        if (!InventoryManager.Instance.HasItem(requiredItemID))
        {
            Debug.Log("NIÑO: Necesito las monedas primero.");
            return;
        }

        if (puertaPrincipal != null)
            puertaPrincipal.ToggleDoor();

        MissionManager.Instance.CompleteMission(missionID);

        if (periodicoObjeto != null)
        {
            periodicoObjeto.SetActive(true);

            if (mostrarDebug)
                Debug.Log("[NewspaperDoorTrigger] Periódico activado.");
        }

        Debug.Log("[NewspaperDoorTrigger] Misión periódico completada. Aquí luego va la cinemática.");
    }
}