using UnityEngine;

/// <summary>
/// ClosetClothingItem representa una prenda dentro del clóset.
/// 
/// Este script NO detecta botones.
/// La interacción se maneja desde Selected.cs:
/// - El jugador mira la prenda.
/// - Presiona B.
/// - Selected.cs llama a ClosetCanvasManager.
/// 
/// Este script SOLO contiene datos:
/// - Nombre de la prenda.
/// - Si es la correcta.
/// - Qué canvas mostrar.
/// 
/// IMPORTANTE:
/// - El objeto debe tener Collider.
/// - Debe estar en layer "RayCast Detect".
/// - El canvas debe estar asignado en el Inspector.
/// </summary>
public class ClosetClothingItem : MonoBehaviour
{
    [Header("Datos de la prenda")]
    public string clothingName = "Prenda";

    [Tooltip("Indica si esta prenda es la correcta dentro de la misión.")]
    public bool isCorrect = false;

    [Header("Canvas de esta prenda")]
    [Tooltip("Canvas que se abrirá cuando el jugador seleccione esta prenda.")]
    public GameObject clothingCanvas;

    /// <summary>
    /// Validación automática en Unity para evitar errores.
    /// </summary>
    void OnValidate()
    {
        if (clothingCanvas == null)
        {
            Debug.LogWarning("⚠ La prenda '" + clothingName + "' no tiene canvas asignado.");
        }
    }
}