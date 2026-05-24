using UnityEngine;
using TMPro;

public class RadioCoverTrigger : MonoBehaviour
{
    [Header("Modo pilas")]
    public RadioAnimacionesSimple radioAnimaciones;

    public bool usarModoPilas = false;

    [Header("Cerrar tapa")]
    public bool usarCerrarTapa = false;

    [Header("Prompt")]
    public GameObject promptPanel;
    public TMP_Text promptText;

    [Header("Visual")]
    public Renderer tapaRenderer;
    public Color colorSeleccion = new Color(0.1f, 1f, 0.25f, 1f);

    [Header("Estado")]
    public bool tapaAbierta = false;

    private Color colorOriginal;

    void Start()
    {
        if (tapaRenderer != null)
            colorOriginal = tapaRenderer.material.color;
    }

    public void MirarTapa()
    {
        if (tapaAbierta) return;

        // ======================================================
        // SOLO permite cerrar tapa cuando ya puede cerrarse
        // ======================================================

        if (usarCerrarTapa)
        {
            if (radioAnimaciones == null) return;

            if (!radioAnimaciones.PuedeCerrarTapa())
                return;
        }

        // ======================================================
        // HIGHLIGHT VERDE
        // ======================================================

        if (tapaRenderer != null)
            tapaRenderer.material.color = colorSeleccion;

        // ======================================================
        // PROMPT
        // ======================================================

        if (promptPanel != null)
            promptPanel.SetActive(true);

        if (promptText != null)
        {
            if (usarModoPilas)
            {
                promptText.text = "Presiona B para poner las pilas";
            }
            else if (usarCerrarTapa)
            {
                promptText.text = "Presiona B para cerrar la tapa";
            }
            else
            {
                promptText.text = "Presiona B para abrir la tapita";
            }
        }

        // ======================================================
        // INPUT
        // ======================================================

        if (InputManagerCustom.PressB())
        {
            // ==========================================
            // ABRIR TAPA PARA PILAS
            // ==========================================

            if (usarModoPilas)
            {
                if (radioAnimaciones != null)
                    radioAnimaciones.ActivarModoInsertarPilas();

                tapaAbierta = true;
                return;
            }

            // ==========================================
            // CERRAR TAPA
            // ==========================================

            if (usarCerrarTapa)
            {
                if (radioAnimaciones != null)
                    radioAnimaciones.CerrarTapaDesdeTrigger();

                tapaAbierta = true;
                return;
            }

            // ==========================================
            // TAPA NORMAL
            // ==========================================

            tapaAbierta = true;
        }
    }

    public void DejarMirarTapa()
    {
        if (tapaRenderer != null)
            tapaRenderer.material.color = colorOriginal;

        if (promptPanel != null)
            promptPanel.SetActive(false);
    }
}