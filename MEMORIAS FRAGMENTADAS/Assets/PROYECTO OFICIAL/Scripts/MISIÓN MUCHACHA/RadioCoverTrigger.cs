using UnityEngine;
using TMPro;

public class RadioCoverTrigger : MonoBehaviour
{
    [Header("Misión")]
    public KitchenRadioMissionManager missionManager;

    [Header("Animator")]
    public Animator radioAnimator;

    [Header("Prompt")]
    public GameObject promptPanel;
    public TMP_Text promptText;
    [Header("Modo pilas")]
    public RadioAnimacionesSimple radioAnimaciones;
    public bool usarModoPilas = false;

    [Header("Visual")]
    public Renderer tapaRenderer;
    public Color colorSeleccion = new Color(0.1f, 1f, 0.25f, 1f);

    private Color colorOriginal;

    [Header("Estado")]
    public bool tapaAbierta = false;

    private bool jugadorMirando = false;

    void Start()
    {
        OcultarPrompt();
         if (tapaRenderer != null)
            colorOriginal = tapaRenderer.material.color;
    }

    void Update()
    {
        if (!jugadorMirando) return;
        if (tapaAbierta) return;

        if (InputManagerCustom.PressB())
        {
            AbrirTapa();
        }
    }

    public void MirarTapa()
    {
        if (tapaRenderer != null)
            tapaRenderer.material.color = colorSeleccion;
        
        Debug.Log(
            "TAPA DEBUG | MirarTapa ejecutado" +
            " | usarModoPilas: " + usarModoPilas +
            " | promptPanel asignado: " + (promptPanel != null) +
            " | promptText asignado: " + (promptText != null) +
            " | promptPanel activo antes: " + (promptPanel != null && promptPanel.activeSelf)
        );

        if (promptPanel != null)
            promptPanel.SetActive(true);

        if (promptText != null)
        {
            if (usarModoPilas)
                promptText.text = "Presiona B para poner las pilas";
            else
                promptText.text = "Presiona B para abrir la tapa";
        }

        if (InputManagerCustom.PressB())
        {
            // =========================================
            // MODO PILAS
            // =========================================

            if (usarModoPilas)
            {
                if (radioAnimaciones != null)
                    radioAnimaciones.ActivarModoInsertarPilas();

                return;
            }

            // =========================================
            // MODO NORMAL
            // =========================================

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

    void AbrirTapa()
    {
        tapaAbierta = true;
        OcultarPrompt();

        if (radioAnimator != null)
            radioAnimator.SetTrigger("AbrirTapa");

        if (missionManager != null)
            missionManager.OnRadioBackCoverOpened();
    }

    void MostrarPrompt(string texto)
    {
        if (promptPanel != null)
            promptPanel.SetActive(true);

        if (promptText != null)
            promptText.text = texto;
    }

    void OcultarPrompt()
    {
        if (promptPanel != null)
            promptPanel.SetActive(false);
    }
}