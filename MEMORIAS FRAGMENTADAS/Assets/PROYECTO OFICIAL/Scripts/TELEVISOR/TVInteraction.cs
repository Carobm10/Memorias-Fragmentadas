using UnityEngine;

public class TVInteraction : MonoBehaviour, IInteractable
{
    [Header("Referencias")]
    public TVMissionController missionController;
    public MeshRenderer tvScreenRenderer;

    [Header("Materiales")]
    public Material channel1Material;
    public Material channel2Material;
    public Material channel3Material;
    public Material interferenceMaterial;

    [Header("Audio")]
    public AudioSource tvAudioSource;
    public AudioClip channel1Clip;
    public AudioClip channel2Clip;
    public AudioClip channel3Clip;
    public AudioClip interferenceClip;

    [Header("Configuración")]
    public bool interactionEnabled = false;
    public bool isInInterference = false;
    public int currentChannel = 0;
    public int channelThatTriggersInterference = 2;

    [Header("Prompts")]
    public string normalPrompt = "Presiona B para cambiar canal";
    public string interferencePrompt = "La señal está fallando. Revisa la antena";

    private void Start()
    {
        ApplyChannelVisual(currentChannel);
        PlayChannelAudio(currentChannel);
    }

    public void EnableInteraction(bool value)
    {
        interactionEnabled = value;
    }

    public void Interact()
    {
        if (!interactionEnabled) return;

        if (isInInterference)
        {
            Debug.Log("No puedes seguir cambiando. Debes ajustar la antena.");
            return;
        }

        ChangeChannel();
    }

    public string GetPrompt()
    {
        if (!interactionEnabled) return "";

        if (isInInterference)
            return interferencePrompt;

        return normalPrompt;
    }

    private void ChangeChannel()
    {
        currentChannel++;

        if (currentChannel > 2)
            currentChannel = 0;

        ApplyChannelVisual(currentChannel);
        PlayChannelAudio(currentChannel);

        Debug.Log("Canal actual: " + currentChannel);

        if (currentChannel == channelThatTriggersInterference && missionController != null)
        {
            missionController.TriggerInterference();
        }
    }

    private void ApplyChannelVisual(int channelIndex)
    {
        if (tvScreenRenderer == null) return;

        switch (channelIndex)
        {
            case 0:
                if (channel1Material != null) tvScreenRenderer.material = channel1Material;
                break;
            case 1:
                if (channel2Material != null) tvScreenRenderer.material = channel2Material;
                break;
            case 2:
                if (channel3Material != null) tvScreenRenderer.material = channel3Material;
                break;
        }
    }

    private void PlayChannelAudio(int channelIndex)
    {
        if (tvAudioSource == null) return;

        AudioClip selectedClip = null;

        switch (channelIndex)
        {
            case 0:
                selectedClip = channel1Clip;
                break;
            case 1:
                selectedClip = channel2Clip;
                break;
            case 2:
                selectedClip = channel3Clip;
                break;
        }

        if (selectedClip != null)
        {
            tvAudioSource.clip = selectedClip;
            tvAudioSource.loop = true;
            tvAudioSource.Play();
        }
    }

    public void SetInterferenceState(bool active)
    {
        isInInterference = active;

        if (active)
        {
            if (tvScreenRenderer != null && interferenceMaterial != null)
            {
                tvScreenRenderer.material = interferenceMaterial;
            }

            if (tvAudioSource != null && interferenceClip != null)
            {
                tvAudioSource.clip = interferenceClip;
                tvAudioSource.loop = true;
                tvAudioSource.Play();
            }
        }
    }

    public void ResolveInterference()
    {
        isInInterference = false;
        ApplyChannelVisual(currentChannel);
        PlayChannelAudio(currentChannel);
        Debug.Log("Se resolvió la interferencia.");
    }
}