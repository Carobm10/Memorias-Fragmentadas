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

    [Header("Estado")]
    public bool tapaAbierta = false;

    private bool jugadorMirando = false;

    void Start()
    {
        OcultarPrompt();
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
        if (tapaAbierta) return;

        jugadorMirando = true;
        MostrarPrompt("Presiona B para abrir la tapa");
    }

    public void DejarMirarTapa()
    {
        jugadorMirando = false;
        OcultarPrompt();
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