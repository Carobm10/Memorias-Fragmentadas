using UnityEngine;

public class TVMissionController : MonoBehaviour
{
    [Header("Estado de misión")]
    public bool missionStarted = false;
    public bool interferenceTriggered = false;
    public bool missionCompleted = false;

    [Header("Referencias")]
    public TVInteraction tvInteraction;
    public AntennaInteraction antennaInteraction;

    [Header("Audio")]
    public AudioSource missionAudioSource;
    public AudioClip fatherRequestClip;
    public AudioClip fatherSuccessClip;

    [Header("Configuración")]
    public bool startMissionOnPlay = true;

    private void Start()
    {
        if (startMissionOnPlay)
        {
            StartMission();
        }
    }

    public void StartMission()
    {
        if (missionStarted) return;

        missionStarted = true;

        if (missionAudioSource != null && fatherRequestClip != null)
        {
            missionAudioSource.clip = fatherRequestClip;
            missionAudioSource.loop = false;
            missionAudioSource.Play();
        }

        if (tvInteraction != null)
        {
            tvInteraction.EnableInteraction(true);
        }

        if (antennaInteraction != null)
        {
            antennaInteraction.EnableInteraction(false);
        }

        Debug.Log("Misión del televisor iniciada.");
    }

    public void TriggerInterference()
    {
        if (interferenceTriggered || missionCompleted) return;

        interferenceTriggered = true;

        if (tvInteraction != null)
        {
            tvInteraction.SetInterferenceState(true);
        }

        if (antennaInteraction != null)
        {
            antennaInteraction.EnableInteraction(true);
        }

        Debug.Log("Interferencia activada. Ajusta la antena.");
    }

    public void CheckAntennaSolution()
    {
        if (!interferenceTriggered || missionCompleted || antennaInteraction == null) return;

        bool antennaOneReady = antennaInteraction.IsAntennaOneCorrect();
        bool antennaTwoReady = antennaInteraction.IsAntennaTwoCorrect();

        if (antennaOneReady && antennaTwoReady)
        {
            CompleteMission();
        }
    }

    public void CompleteMission()
    {
        if (missionCompleted) return;

        missionCompleted = true;

        if (tvInteraction != null)
        {
            tvInteraction.ResolveInterference();
        }

        if (antennaInteraction != null)
        {
            antennaInteraction.EnableInteraction(false);
        }

        if (missionAudioSource != null && fatherSuccessClip != null)
        {
            missionAudioSource.Stop();
            missionAudioSource.clip = fatherSuccessClip;
            missionAudioSource.loop = false;
            missionAudioSource.Play();
        }

        Debug.Log("Misión del televisor completada.");
    }
}