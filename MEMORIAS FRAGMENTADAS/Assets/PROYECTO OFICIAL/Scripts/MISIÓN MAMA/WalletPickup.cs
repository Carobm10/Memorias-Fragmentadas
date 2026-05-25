using UnityEngine;

public class WalletPickup : MonoBehaviour
{
    [Header("Configuración")]
    public string missionID = "periodico";
    public string itemID = "monedas";

    [Header("Opcional")]
    public GameObject objetoVisualParaOcultar;

    [Header("Debug")]
    public bool mostrarDebug = true;

    public void RecogerMonedero()
    {
        if (!MissionManager.Instance.IsMissionActive(missionID))
        {
            if (mostrarDebug)
                Debug.Log("[WalletPickup] No puedes tomar el monedero todavía. La misión no está activa.");

            return;
        }

        if (WalletSequenceController.Instance != null)
        {
            WalletSequenceController.Instance.IniciarSecuencia(() =>
            {
                InventoryManager.Instance.AddItem(itemID);

                if (mostrarDebug)
                    Debug.Log("[WalletPickup] Monedero recogido después de animación. Se agregaron monedas.");

                if (objetoVisualParaOcultar != null)
                    objetoVisualParaOcultar.SetActive(false);
                else
                    gameObject.SetActive(false);
            });
        }
        else
        {
            InventoryManager.Instance.AddItem(itemID);

            if (mostrarDebug)
                Debug.Log("[WalletPickup] No hay WalletSequenceController. Se agregaron monedas directo.");

            if (objetoVisualParaOcultar != null)
                objetoVisualParaOcultar.SetActive(false);
            else
                gameObject.SetActive(false);
        }
        
            
    }
}