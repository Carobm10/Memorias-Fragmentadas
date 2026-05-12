using UnityEngine;
using TMPro;

public class BatteryPickup : MonoBehaviour
{
    [Header("Misión")]
    public KitchenRadioMissionManager missionManager;

    [Header("Prompt")]
    public GameObject promptPanel;
    public TMP_Text promptText;

    private bool isLooking = false;

    void Start()
    {
        HidePrompt();
    }

    void Update()
    {
        if (!isLooking) return;

        if (missionManager.currentState != KitchenRadioMissionManager.MissionState.NeedFindBatteries)
            return;

        if (InputManagerCustom.PressB())
        {
            TakeBatteries();
        }
    }

    public void LookAtBatteries()
    {
        if (missionManager.currentState != KitchenRadioMissionManager.MissionState.NeedFindBatteries)
            return;

        isLooking = true;
        ShowPrompt("Presiona B para tomar las pilas");
    }

    public void StopLookingAtBatteries()
    {
        isLooking = false;
        HidePrompt();
    }

    void TakeBatteries()
    {
        HidePrompt();
        missionManager.PickBatteries();
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