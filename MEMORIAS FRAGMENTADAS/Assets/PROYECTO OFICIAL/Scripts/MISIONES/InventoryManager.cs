using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    [Header("Debug")]
    public bool mostrarDebug = true;

    private HashSet<string> inventory = new HashSet<string>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    public void AddItem(string itemID)
    {
        if (!inventory.Contains(itemID))
        {
            inventory.Add(itemID);

            if (mostrarDebug)
                Debug.Log("[InventoryManager] Item agregado: " + itemID);
        }
    }

    public void RemoveItem(string itemID)
    {
        if (inventory.Contains(itemID))
        {
            inventory.Remove(itemID);

            if (mostrarDebug)
                Debug.Log("[InventoryManager] Item eliminado: " + itemID);
        }
    }

    public bool HasItem(string itemID)
    {
        return inventory.Contains(itemID);
    }

    [ContextMenu("DEBUG - Agregar monedas")]
    public void DebugAddMonedas()
    {
        AddItem("monedas");
    }

    [ContextMenu("DEBUG - Quitar monedas")]
    public void DebugRemoveMonedas()
    {
        RemoveItem("monedas");
    }

    [ContextMenu("DEBUG - Revisar monedas")]
    public void DebugCheckMonedas()
    {
        bool tiene = HasItem("monedas");

        Debug.Log("[InventoryManager] ¿Tiene monedas?: " + tiene);
    }
}