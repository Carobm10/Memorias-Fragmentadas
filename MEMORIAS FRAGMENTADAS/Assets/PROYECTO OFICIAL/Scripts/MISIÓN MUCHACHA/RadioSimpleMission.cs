using UnityEngine;
using TMPro;
using System.Collections;

public class RadioSimpleMission : MonoBehaviour
{
    [Header("Misión")]
    public KitchenRadioMissionManager missionManager;

    [Header("Objetos")]
    public GameObject radioNormal;
    public GameObject radioAnimacionAbrirTapa;

    [Header("Animación")]
    public Animator animatorAbrirTapa;
    public string nombreEstadoAbrirTapa = "AbrirTapa";
    public float duracionAnimacion = 2.5f;

    [Header("Jugador / cámara")]
    public Transform player;
    public MovimientoVR2 playerMovement;
    public Transform cameraTransform;
    public Transform puntoVistaRadio;

    [Header("Prompt")]
    public GameObject promptPanel;
    public TMP_Text promptText;

    [Header("Estado")]
    public bool mirandoRadio = false;
    public bool enInteraccion = false;
    public bool tapaAbierta = false;

    void Start()
    {
        if (radioAnimacionAbrirTapa != null)
            radioAnimacionAbrirTapa.SetActive(false);

        OcultarPrompt();
    }

    void Update()
    {
        if (!mirandoRadio) return;
        if (tapaAbierta) return;

        if (!enInteraccion)
        {
            MostrarPrompt("Presiona B para interactuar con la radio");

            if (InputManagerCustom.PressB())
            {
                StartCoroutine(IniciarInteraccionRadio());
            }
        }
        else
        {
            MostrarPrompt("Presiona B para abrir la tapa");

            if (InputManagerCustom.PressB())
            {
                StartCoroutine(AbrirTapa());
            }
        }
    }

    public void MirarRadio()
    {
        if (missionManager != null &&
            missionManager.currentState != KitchenRadioMissionManager.MissionState.NeedCheckRadio)
            return;

        mirandoRadio = true;
    }

    public void DejarMirarRadio()
    {
        mirandoRadio = false;

        if (!enInteraccion)
            OcultarPrompt();
    }

    IEnumerator IniciarInteraccionRadio()
    {
        enInteraccion = true;
        OcultarPrompt();

        if (playerMovement != null)
            playerMovement.puedeMoverse = false;

        if (puntoVistaRadio != null && player != null)
        {
            player.position = puntoVistaRadio.position;
            player.rotation = puntoVistaRadio.rotation;
        }

        yield return new WaitForSeconds(0.3f);

        MostrarPrompt("Presiona B para abrir la tapa");
    }

    IEnumerator AbrirTapa()
    {
        tapaAbierta = true;
        OcultarPrompt();

        if (radioNormal != null)
            radioNormal.SetActive(false);

        if (radioAnimacionAbrirTapa != null)
            radioAnimacionAbrirTapa.SetActive(true);

        if (animatorAbrirTapa != null)
        {
            animatorAbrirTapa.Play(nombreEstadoAbrirTapa, 0, 0f);
        }

        yield return new WaitForSeconds(duracionAnimacion);

        if (missionManager != null)
            missionManager.OnRadioBackCoverOpened();
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