using UnityEngine;

public class AntennaInteraction : MonoBehaviour, IInteractable
{
    [Header("Referencias")]
    public TVMissionController missionController;

    [Header("Antenas visuales reales")]
    public Transform antennaOne;
    public Transform antennaTwo;

    [Header("Estado")]
    public bool interactionEnabled = false;
    public bool isAdjustMode = false;
    public int selectedAntenna = 0; // 0 = ninguna, 1 = antennaOne, 2 = antennaTwo

    [Header("Movimiento")]
    public float moveStep = 5f;

    [Header("Ángulos actuales")]
    public float antennaOneAngle = 0f;
    public float antennaTwoAngle = 0f;

    [Header("Límites antena 1")]
    public float antennaOneMinAngle = -30f;
    public float antennaOneMaxAngle = 30f;

    [Header("Límites antena 2")]
    public float antennaTwoMinAngle = -30f;
    public float antennaTwoMaxAngle = 30f;

    [Header("Rango correcto antena 1")]
    public float antennaOneCorrectMin = 10f;
    public float antennaOneCorrectMax = 20f;

    [Header("Rango correcto antena 2")]
    public float antennaTwoCorrectMin = -15f;
    public float antennaTwoCorrectMax = -5f;

    [Header("Prompt")]
    [TextArea] public string normalPrompt = "Presiona B para ajustar antena";
    [TextArea] public string adjustPrompt = "1 antena izquierda / 2 antena derecha / X izquierda / Y derecha / B salir";

    [Header("Audio")]
    public AudioSource antennaAudioSource;
    public AudioClip moveClip;

    public void EnableInteraction(bool value)
    {
        interactionEnabled = value;

        if (!value)
        {
            isAdjustMode = false;
            selectedAntenna = 0;
        }
    }

    public void Interact()
    {
        if (!interactionEnabled) return;

        isAdjustMode = !isAdjustMode;

        if (!isAdjustMode)
        {
            selectedAntenna = 0;
            Debug.Log("Saliste del modo ajuste de antena.");
        }
        else
        {
            Debug.Log("Entraste al modo ajuste de antena.");
        }
    }

    public string GetPrompt()
    {
        if (!interactionEnabled) return "";

        if (isAdjustMode)
            return adjustPrompt;

        return normalPrompt;
    }

    private void Update()
    {
        if (!interactionEnabled || !isAdjustMode) return;

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            selectedAntenna = 1;
            Debug.Log("Seleccionaste antena 1.");
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            selectedAntenna = 2;
            Debug.Log("Seleccionaste antena 2.");
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            MoveSelectedAntenna(-moveStep);
        }

        if (Input.GetKeyDown(KeyCode.Y))
        {
            MoveSelectedAntenna(moveStep);
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            isAdjustMode = false;
            selectedAntenna = 0;
            Debug.Log("Saliste del modo ajuste de antena.");
        }
    }

    private void MoveSelectedAntenna(float delta)
    {
        if (selectedAntenna == 1)
        {
            antennaOneAngle += delta;
            antennaOneAngle = Mathf.Clamp(antennaOneAngle, antennaOneMinAngle, antennaOneMaxAngle);
            ApplyAntennaOneRotation();
            PlayMoveAudio();
            CheckSolution();
        }
        else if (selectedAntenna == 2)
        {
            antennaTwoAngle += delta;
            antennaTwoAngle = Mathf.Clamp(antennaTwoAngle, antennaTwoMinAngle, antennaTwoMaxAngle);
            ApplyAntennaTwoRotation();
            PlayMoveAudio();
            CheckSolution();
        }
        else
        {
            Debug.Log("Primero selecciona una antena con 1 o 2.");
        }
    }

    private void ApplyAntennaOneRotation()
    {
        if (antennaOne != null)
        {
            antennaOne.localRotation = Quaternion.Euler(antennaOneAngle, 0f, 0f);
        }
    }

    private void ApplyAntennaTwoRotation()
    {
        if (antennaTwo != null)
        {
            antennaTwo.localRotation = Quaternion.Euler(antennaTwoAngle, 0f, 0f);
        }
    }

    private void PlayMoveAudio()
    {
        if (antennaAudioSource != null && moveClip != null)
        {
            antennaAudioSource.PlayOneShot(moveClip);
        }
    }

    private void CheckSolution()
    {
        if (missionController != null)
        {
            missionController.CheckAntennaSolution();
        }
    }

    public bool IsAntennaOneCorrect()
    {
        return antennaOneAngle >= antennaOneCorrectMin && antennaOneAngle <= antennaOneCorrectMax;
    }

    public bool IsAntennaTwoCorrect()
    {
        return antennaTwoAngle >= antennaTwoCorrectMin && antennaTwoAngle <= antennaTwoCorrectMax;
    }
}