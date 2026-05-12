using UnityEngine;
using TMPro;

public class RadioBatteryTrigger : MonoBehaviour
{
    [Header("Misión")]
    public KitchenRadioMissionManager missionManager;
    [Header("Animator")]
    public Animator radioAnimator;

    [Header("Nombre del Trigger")]
    public string triggerName;

    [Header("Prompt")]
    public GameObject promptPanel;
    public TMP_Text promptText;

    [Header("Estado")]
    public bool yaUsado = false;

    private bool jugadorMirando = false;

    void Start()
    {
        OcultarPrompt();
    }

    void Update()
    {
        if (!jugadorMirando) return;

        if (yaUsado) return;

        if (missionManager == null || !missionManager.PlayerHasBatteries())
            return;

        if (InputManagerCustom.PressB())
        {
            ActivarPila();
        }
    }

    // ======================================================
    // CUANDO EL JUGADOR MIRA ESTA PILA
    // ======================================================

    public void MirarPila()
    {
        if (yaUsado) return;

        // Solo permite poner pilas si ya fueron tomadas del cajón
        if (missionManager == null || !missionManager.PlayerHasBatteries())
            return;

        jugadorMirando = true;

        MostrarPrompt("Presiona B para poner pila");
    }

    // ======================================================
    // CUANDO DEJA DE MIRAR
    // ======================================================

    public void DejarMirarPila()
    {
        jugadorMirando = false;

        OcultarPrompt();
    }

    // ======================================================
    // ACTIVA LA ANIMACIÓN
    // ======================================================

    void ActivarPila()
    {
        yaUsado = true;
        OcultarPrompt();

        if (radioAnimator != null)
        {
            radioAnimator.SetTrigger(triggerName);
            Debug.Log("Animación activada: " + triggerName);
        }

        // Si esta es la tercera pila, cerramos la tapa después de un momento
        if (triggerName == "TerceraPila")
        {
            Invoke(nameof(CerrarTapaDespues), 2.5f);
        }
    }

    void CerrarTapaDespues()
    {
        if (radioAnimator != null)
        {
            radioAnimator.SetTrigger("CerrarTapa");
            Debug.Log("Animación activada: CerrarTapa");
        }

        if (missionManager != null)
        {
            missionManager.BatteriesInstalled();
        }
    }

    // ======================================================
    // UI
    // ======================================================

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