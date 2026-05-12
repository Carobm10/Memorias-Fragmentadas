using UnityEngine;
using System.Collections;

public class RadioBackCover : MonoBehaviour
{
    [Header("Misión")]
    public KitchenRadioMissionManager missionManager;

    [Header("Animación")]
    public Animator animator;
    public string openAnimationTrigger = "Open";

    [Header("Estado")]
    public bool isOpen = false;

    [Header("Prompt")]
    public GameObject promptPanel;
    public TMPro.TMP_Text promptText;

    void Start()
    {
        HidePrompt();
    }

    public void LookAtCover()
    {
        if (isOpen) return;

        if (missionManager.currentState == KitchenRadioMissionManager.MissionState.NeedCheckRadio)
        {
            ShowPrompt("Presiona B para abrir la tapa");

            if (InputManagerCustom.PressB())
            {
                OpenCover();
            }
        }
    }

    public void StopLookingAtCover()
    {
        HidePrompt();
    }

    public void OpenCover()
    {
        if (isOpen) return;

        isOpen = true;
        HidePrompt();

        if (animator != null)
            animator.SetTrigger(openAnimationTrigger);

        missionManager.OnRadioBackCoverOpened();
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