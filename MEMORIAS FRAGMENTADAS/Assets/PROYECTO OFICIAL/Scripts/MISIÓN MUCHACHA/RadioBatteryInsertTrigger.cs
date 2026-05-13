using UnityEngine;
using TMPro;

public class RadioBatteryInsertTrigger : MonoBehaviour
{
    [Header("Configuración")]
    public int numeroPila = 1; // 1, 2 o 3

    [Header("Radio")]
    public RadioAnimacionesSimple radioAnimaciones;

    [Header("Prompt")]
    public GameObject promptPanel;
    public TMP_Text promptText;

    [Header("Visual")]
    public Renderer pilaRenderer;
    public Color colorSeleccion = new Color(0.1f, 1f, 0.25f, 1f);

    private Color colorOriginal;
    private bool yaUsada = false;

    void Start()
    {
        if (pilaRenderer != null)
            colorOriginal = pilaRenderer.material.color;
    }

    public void MirarPila()
    {
        if (yaUsada) return;

        if (pilaRenderer != null)
            pilaRenderer.material.color = colorSeleccion;

        if (promptPanel != null)
            promptPanel.SetActive(true);

        if (promptText != null)
            promptText.text = "Presiona B para poner pila " + numeroPila;

        if (InputManagerCustom.PressB())
        {
            yaUsada = true;

            if (promptPanel != null)
                promptPanel.SetActive(false);

            if (radioAnimaciones != null)
                radioAnimaciones.PonerPila(numeroPila);
        }
    }

    public void DejarMirarPila()
    {
        if (yaUsada) return;

        if (pilaRenderer != null)
            pilaRenderer.material.color = colorOriginal;

        if (promptPanel != null)
            promptPanel.SetActive(false);
    }
}