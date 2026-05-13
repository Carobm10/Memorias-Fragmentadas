using UnityEngine;

public class RadioMissionInteractable : MonoBehaviour
{
    [Header("Misión")]
    public KitchenRadioMissionManager missionManager;

    [Header("Inspección 360")]
    public InspectableObject360 inspectable360;

    [Header("Prompts")]
    public GameObject promptPanel;
    public TMPro.TMP_Text promptText;

    public bool isBeingInspected = false;

    void Start()
    {
        HidePrompt();
    }

    public void LookAtRadio()
    {
        if (missionManager == null) return;

        if (missionManager.CanUseRadio())
        {
            ShowPrompt("Presiona B para cambiar canales");

            if (InputManagerCustom.PressB())
            {
                missionManager.ChangeRadioChannel();
            }

            return;
        }

        if (missionManager.currentState == KitchenRadioMissionManager.MissionState.NeedCheckRadio ||
            missionManager.currentState == KitchenRadioMissionManager.MissionState.HasBatteries)
        {
            ShowPrompt("Presiona B para observar la radio");

            if (InputManagerCustom.PressB())
            {
                EnterInspection();
            }
        }
    }

    public void StopLookingAtRadio()
    {
        HidePrompt();
    }

    public void EnterInspection()
    {
        HidePrompt();

        if (inspectable360 != null)
        {
            inspectable360.StartInspection();
            isBeingInspected = true;
        }
    }

    public void ExitInspection()
    {
        isBeingInspected = false;
    }

    void ShowPrompt(string message)
    {
        if (promptPanel != null)
            promptPanel.SetActive(true);

        if (promptText != null)
            promptText.text = message;
    }

    void HidePrompt()
    {
        if (promptPanel != null)
            promptPanel.SetActive(false);
    }
}