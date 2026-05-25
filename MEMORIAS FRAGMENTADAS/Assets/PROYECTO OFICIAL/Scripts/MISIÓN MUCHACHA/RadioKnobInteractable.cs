using UnityEngine;
using TMPro;

/// <summary>
/// Interacción para perillas de la radio.
/// 
/// TIPOS:
/// - Cambiar emisora
/// - Subir volumen
/// - Bajar volumen
/// </summary>
public class RadioKnobInteractable : MonoBehaviour
{
    public enum TipoPerilla
    {
        CambiarEmisora,
        Volumen
    }

    [Header("Tipo")]
    public TipoPerilla tipo;

    [Header("Radio")]
    public RadioMusicController radioMusic;

    [Header("Prompt")]
    public GameObject promptPanel;
    public TMP_Text promptText;

    [Header("Visual")]
    public Renderer knobRenderer;
    public Color colorSeleccion = new Color(0.1f, 1f, 0.25f, 1f);

    [Header("Rotación visual")]
    public Transform perillaVisual;
    public float rotacionPaso = 20f;

    private Color colorOriginal;

    void Start()
    {
        if (knobRenderer != null)
            colorOriginal = knobRenderer.material.color;
    }

    public void MirarPerilla()
    {
        // =========================
        // COLOR VERDE
        // =========================

        if (knobRenderer != null)
            knobRenderer.material.color = colorSeleccion;

        // =========================
        // PROMPTS
        // =========================

        if (promptPanel != null)
            promptPanel.SetActive(true);

        if (promptText != null)
        {
            if (tipo == TipoPerilla.CambiarEmisora)
            {
                promptText.text = "Presiona B para cambiar emisora";
            }
            else
            {
                promptText.text = "B = subir volumen\nX = bajar volumen";
            }
        }

        // =========================
        // CAMBIAR EMISORA
        // =========================

        if (tipo == TipoPerilla.CambiarEmisora)
        {
            if (InputManagerCustom.PressB())
            {
                if (radioMusic != null)
                    radioMusic.SiguienteEmisora();

                RotarPerillaDerecha();
            }
        }

        // =========================
        // VOLUMEN
        // =========================

        if (tipo == TipoPerilla.Volumen)
        {
            if (InputManagerCustom.PressB())
            {
                if (radioMusic != null)
                    radioMusic.SubirVolumen();

                RotarPerillaDerecha();
            }

            if (InputManagerCustom.PressX())
            {
                if (radioMusic != null)
                    radioMusic.BajarVolumen();

                RotarPerillaIzquierda();
            }
        }
    }

    public void DejarMirarPerilla()
    {
        if (knobRenderer != null)
            knobRenderer.material.color = colorOriginal;

        if (promptPanel != null)
            promptPanel.SetActive(false);
    }

    void RotarPerillaDerecha()
    {
        if (perillaVisual != null)
            perillaVisual.Rotate(Vector3.up * rotacionPaso);
    }

    void RotarPerillaIzquierda()
    {
        if (perillaVisual != null)
            perillaVisual.Rotate(Vector3.up * -rotacionPaso);
    }
}