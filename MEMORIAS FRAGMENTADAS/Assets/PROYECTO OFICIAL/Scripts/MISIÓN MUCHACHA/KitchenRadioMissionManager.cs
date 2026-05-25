using UnityEngine;
using TMPro;
using System.Collections;

/// <summary>
/// KITCHEN RADIO MISSION MANAGER
/// Controla el inicio narrativo de la misión de la cocina:
/// 1. Rosa muestra el prompt de misión.
/// 2. El jugador presiona A.
/// 3. Rosa pide poner música.
/// 4. Se desbloquea la radio.
/// 
/// IMPORTANTE:
/// Este script NO maneja animaciones de radio.
/// Eso lo maneja RadioAnimacionesSimple.
/// </summary>
public class KitchenRadioMissionManager : MonoBehaviour
{
    public enum MissionState
    {
        NotStarted,
        Started,
        NeedCheckRadio,
        NeedFindBatteries,
        HasBatteries,
        BatteriesInstalled,
        Completed
    }

    [Header("Estado")]
    public MissionState currentState = MissionState.NotStarted;

    [Header("Radio principal")]
    public RadioAnimacionesSimple radioAnimacionesSimple;

    [Header("UI misión")]
    public GameObject missionPromptPanel;
    public TMP_Text missionPromptText;

    [Header("UI diálogo")]
    public GameObject dialoguePanel;
    public TMP_Text npcNameText;
    public TMP_Text dialogueText;
    public string npcDisplayName = "Rosa";

    [Header("UI notificación opcional")]
    public GameObject notificationPanel;
    public TMP_Text notificationText;

    void Start()
    {
        HideMissionPrompt();
        HideDialogue();
        HideNotification();
    }

    public void ShowMissionPrompt()
    {
        if (currentState != MissionState.NotStarted) return;

        if (missionPromptPanel != null)
            missionPromptPanel.SetActive(true);

        if (missionPromptText != null)
            missionPromptText.text = "Presiona A para hablar con Rosa";
    }

    public void HideMissionPrompt()
    {
        if (missionPromptPanel != null)
            missionPromptPanel.SetActive(false);
    }

    public void TryStartMission()
    {
        if (currentState != MissionState.NotStarted) return;

        currentState = MissionState.Started;
        HideMissionPrompt();

        StartCoroutine(StartMissionDialogue());
    }

    IEnumerator StartMissionDialogue()
    {
        ShowDialogue("Ay, llegó el niño Cenito. ¿Me haces un favor? ¿Podrías poner música en la radio?");
        yield return new WaitForSeconds(4f);

        HideDialogue();

        currentState = MissionState.NeedCheckRadio;
        ShowNotification("Busca la radio en la cocina.");

        if (radioAnimacionesSimple != null)
        {
            radioAnimacionesSimple.DesbloquearRadio();
        }
        else
        {
            Debug.LogError("KITCHEN RADIO: Falta asignar RadioAnimacionesSimple en el Inspector.");
        }
    }

    public void PickBatteries()
    {
        currentState = MissionState.HasBatteries;
        ShowNotification("Has cogido las pilas. Vuelve a la radio.");
    }

    public bool PlayerHasBatteries()
    {
        return currentState == MissionState.HasBatteries ||
               currentState == MissionState.BatteriesInstalled ||
               currentState == MissionState.Completed;
    }

    public void BatteriesInstalled()
    {
        currentState = MissionState.BatteriesInstalled;
        ShowNotification("Las pilas están puestas.");
    }

    public bool CanUseRadio()
    {
        return currentState == MissionState.BatteriesInstalled ||
               currentState == MissionState.Completed;
    }

    public void CompleteMission()
    {
        currentState = MissionState.Completed;
        ShowNotification("La radio ya está funcionando.");
    }

    void ShowDialogue(string text)
    {
        if (dialoguePanel != null)
            dialoguePanel.SetActive(true);

        if (npcNameText != null)
            npcNameText.text = npcDisplayName;

        if (dialogueText != null)
            dialogueText.text = text;
    }

    void HideDialogue()
    {
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);
    }

    void ShowNotification(string text)
    {
        if (notificationPanel != null)
            notificationPanel.SetActive(true);

        if (notificationText != null)
            notificationText.text = text;

        Debug.Log("KITCHEN RADIO: " + text);
    }

    void HideNotification()
    {
        if (notificationPanel != null)
            notificationPanel.SetActive(false);
    }
}