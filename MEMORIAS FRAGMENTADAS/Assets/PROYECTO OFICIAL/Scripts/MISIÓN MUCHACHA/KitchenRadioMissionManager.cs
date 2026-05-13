using UnityEngine;
using TMPro;
using System.Collections;

public class KitchenRadioMissionManager : MonoBehaviour
{
    [Header("Radio animaciones simple")]
    public RadioAnimacionesSimple radioAnimacionesSimple;
    [Header("UI diálogo NPC")]
    public GameObject dialoguePanel;
    public TMPro.TMP_Text npcNameText;
    [Header("Datos NPC")]
    public string npcDisplayName = "Rosa [Servicio]";
    public enum MissionState
    {
        NotStarted,
        Started,
        NeedCheckRadio,
        RadioOpenedNoBatteries,
        NeedFindBatteries,
        HasBatteries,
        InstallingBatteries,
        BatteriesInstalled,
        Completed
    }

    [Header("Estado")]
    public MissionState currentState = MissionState.NotStarted;

    [Header("UI existente")]
    public GameObject missionPromptPanel;
    public TMP_Text missionPromptText;
    public TMP_Text dialogueText;

    public GameObject notificationPanel;
    public TMP_Text notificationText;

    [Header("Radio")]
    public RadioMissionInteractable radio;

    [Header("Pilas")]
    public GameObject batteriesInDrawer;
    public GameObject batteriesVisualNearRadio;

    [Header("Audio opcional")]
    public AudioSource radioAudioSource;
    public AudioClip[] radioChannels;

    private int currentChannelIndex = 0;

    void Start()
    {
        HideMissionPrompt();
        HideDialogue();
        HideNotification();

        if (batteriesVisualNearRadio != null)
            batteriesVisualNearRadio.SetActive(false);
    }

    public void ShowMissionPrompt()
    {
        if (currentState != MissionState.NotStarted) return;

        if (missionPromptPanel != null)
            missionPromptPanel.SetActive(true);

        if (missionPromptText != null)
            missionPromptText.text = "Presiona A para misión";
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
            Debug.LogError("No asignaste RadioAnimacionesSimple en el KitchenRadioMissionManager.");
        }
    }

    public void OnRadioBackCoverOpened()
    {
        if (currentState != MissionState.NeedCheckRadio) return;

        currentState = MissionState.RadioOpenedNoBatteries;

        StartCoroutine(NoBatteriesDialogue());
    }

    IEnumerator NoBatteriesDialogue()
    {
        ShowDialogue("La radio no tiene pilas... creo que las pilas están en el cajón.");
        yield return new WaitForSeconds(4f);

        HideDialogue();

        currentState = MissionState.NeedFindBatteries;

        ShowNotification("Busca las pilas en el cajón.");
    }

    public void PickBatteries()
    {
        if (currentState != MissionState.NeedFindBatteries) return;

        currentState = MissionState.HasBatteries;

        if (batteriesInDrawer != null)
            batteriesInDrawer.SetActive(false);

        if (batteriesVisualNearRadio != null)
            batteriesVisualNearRadio.SetActive(true);

        ShowNotification("Has cogido las pilas. Vuelve a la radio.");
    }

    public bool PlayerHasBatteries()
    {
        return currentState == MissionState.HasBatteries || 
               currentState == MissionState.InstallingBatteries ||
               currentState == MissionState.BatteriesInstalled ||
               currentState == MissionState.Completed;
    }

    public void StartInstallingBatteries()
    {
        if (currentState == MissionState.HasBatteries)
            currentState = MissionState.InstallingBatteries;
    }

    public void BatteriesInstalled()
    {
        currentState = MissionState.BatteriesInstalled;
        ShowNotification("Las pilas están puestas. Ahora puedes prender la radio.");
    }

    public bool CanUseRadio()
    {
        return currentState == MissionState.BatteriesInstalled || currentState == MissionState.Completed;
    }

    public void ChangeRadioChannel()
    {
        if (!CanUseRadio()) return;

        currentState = MissionState.Completed;

        if (radioAudioSource == null || radioChannels == null || radioChannels.Length == 0)
        {
            ShowNotification("La radio está encendida.");
            return;
        }

        currentChannelIndex++;

        if (currentChannelIndex >= radioChannels.Length)
            currentChannelIndex = 0;

        radioAudioSource.clip = radioChannels[currentChannelIndex];
        radioAudioSource.Play();

        ShowNotification("Cambiando canal...");
    }

   public void ShowDialogue(string message)
    {
        if (dialoguePanel != null)
            dialoguePanel.SetActive(true);

        if (npcNameText != null)
        npcNameText.text = npcDisplayName;

        if (dialogueText != null)
            dialogueText.text = message;
    }

    public void HideDialogue()
    {
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);
    }

    public void ShowNotification(string message)
    {
        StartCoroutine(NotificationRoutine(message));
    }

    IEnumerator NotificationRoutine(string message)
    {
        if (notificationPanel != null)
            notificationPanel.SetActive(true);

        if (notificationText != null)
            notificationText.text = message;

        yield return new WaitForSeconds(5f);

        HideNotification();
    }

    public void HideNotification()
    {
        if (notificationPanel != null)
            notificationPanel.SetActive(false);
    }
}