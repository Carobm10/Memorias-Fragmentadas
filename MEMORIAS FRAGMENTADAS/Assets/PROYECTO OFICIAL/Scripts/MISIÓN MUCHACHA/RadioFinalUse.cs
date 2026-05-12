using UnityEngine;
using TMPro;

public class RadioFinalUse : MonoBehaviour
{
    [Header("Misión")]
    public KitchenRadioMissionManager missionManager;

    [Header("Prompt")]
    public GameObject promptPanel;
    public TMP_Text promptText;

    private bool mirandoRadio = false;

    void Start()
    {
        OcultarPrompt();
    }

    void Update()
    {
        if (!mirandoRadio) return;

        if (missionManager == null) return;

        if (!missionManager.CanUseRadio()) return;

        if (InputManagerCustom.PressB())
        {
            missionManager.ChangeRadioChannel();
        }
    }

    public void MirarRadioFinal()
    {
        if (missionManager == null) return;

        if (!missionManager.CanUseRadio()) return;

        mirandoRadio = true;
        MostrarPrompt("Presiona B para cambiar canales");
    }

    public void DejarMirarRadioFinal()
    {
        mirandoRadio = false;
        OcultarPrompt();
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