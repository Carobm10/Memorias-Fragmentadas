using UnityEngine;

public class ClosetClothingItem : MonoBehaviour
{
    [Header("Datos de la prenda")]
    public string clothingName = "Prenda";
    public bool isCorrect = false;

    [Header("Canvas de esta prenda")]
    public GameObject clothingCanvas;
}