using System.Collections;
using UnityEngine;

public class PhoneMissionController : MonoBehaviour
{
    public enum PhoneState
    {
        Locked,
        Unlocking,
        ReadyToTalk,
        InCall,
        Finished
    }

    [Header("Modelos del teléfono")]
    public GameObject telefonoCerrado;
    public GameObject telefonoAnimacionCandado;
    public GameObject telefonoAbierto;

    [Header("Puntos jugador")]
    public Transform playerRoot;
    public Transform phoneSitPoint;
    public Transform phoneLookTarget;

    [Header("Scripts")]
    public MovimientoVR2 movimientoJugador;
    public PhoneDialogueController phoneDialogueController;

    [Header("UI de interacción")]
    public GameObject promptPanel;
    public TMPro.TMP_Text promptText;

    [Header("Configuración")]
    public float tiempoMovimientoSentarse = 1.5f;
    public float duracionAnimacionCandado = 4f;

    private bool jugadorMirando = false;
    private bool ocupado = false;
    private PhoneState estado = PhoneState.Locked;

    void Start()
    {
        estado = PhoneState.Locked;

        if (telefonoCerrado != null)
            telefonoCerrado.SetActive(true);

        if (telefonoAnimacionCandado != null)
            telefonoAnimacionCandado.SetActive(false);

        if (telefonoAbierto != null)
            telefonoAbierto.SetActive(false);

        if (promptPanel != null)
            promptPanel.SetActive(false);
    }

    void Update()
    {
        bool puedeInteractuar = jugadorMirando && !ocupado;

        if (promptPanel != null)
            promptPanel.SetActive(puedeInteractuar);

        if (puedeInteractuar)
            ActualizarTextoPrompt();

        if (puedeInteractuar && InputManagerCustom.PressB())
        {
            if (estado == PhoneState.Locked)
            {
                StartCoroutine(SecuenciaAbrirCandado());
            }
            else if (estado == PhoneState.ReadyToTalk)
            {
                StartCoroutine(SecuenciaContestarTelefono());
            }
        }
    }

    void ActualizarTextoPrompt()
    {
        if (promptText == null)
            return;

        if (estado == PhoneState.Locked)
            promptText.text = "Presiona B para abrir el candado";
        else if (estado == PhoneState.ReadyToTalk)
            promptText.text = "Presiona B para contestar el teléfono";
        else
            promptText.text = "";
    }

    public void SetMirandoTelefono(bool mirando)
    {
        jugadorMirando = mirando;

        if (!jugadorMirando && promptPanel != null && !ocupado)
            promptPanel.SetActive(false);
    }

    IEnumerator SecuenciaAbrirCandado()
    {
        ocupado = true;
        estado = PhoneState.Unlocking;

        if (promptPanel != null)
            promptPanel.SetActive(false);

        if (telefonoCerrado != null)
            telefonoCerrado.SetActive(false);

        if (telefonoAnimacionCandado != null)
            telefonoAnimacionCandado.SetActive(true);

        yield return new WaitForSeconds(duracionAnimacionCandado);

        if (telefonoAnimacionCandado != null)
            telefonoAnimacionCandado.SetActive(false);

        if (telefonoAbierto != null)
            telefonoAbierto.SetActive(true);

        estado = PhoneState.ReadyToTalk;
        ocupado = false;
    }

    IEnumerator SecuenciaContestarTelefono()
    {
        ocupado = true;
        estado = PhoneState.InCall;

        if (promptPanel != null)
            promptPanel.SetActive(false);

        if (movimientoJugador != null)
            movimientoJugador.puedeMoverse = false;

        yield return StartCoroutine(MoverJugadorA(phoneSitPoint, true));

        if (phoneDialogueController != null)
            phoneDialogueController.StartPhoneDialogue();

        ocupado = false;
    }

    IEnumerator MoverJugadorA(Transform destino, bool mirarAlTelefono)
    {
        if (playerRoot == null || destino == null)
            yield break;

        Vector3 posicionInicial = playerRoot.position;
        Quaternion rotacionInicial = playerRoot.rotation;

        Vector3 posicionFinal = destino.position;
        Quaternion rotacionFinal = destino.rotation;

        if (mirarAlTelefono && phoneLookTarget != null)
        {
            Vector3 direccion = phoneLookTarget.position - destino.position;
            direccion.y = 0;

            if (direccion != Vector3.zero)
                rotacionFinal = Quaternion.LookRotation(direccion);
        }

        float tiempo = 0f;

        while (tiempo < tiempoMovimientoSentarse)
        {
            tiempo += Time.deltaTime;
            float t = tiempo / tiempoMovimientoSentarse;

            playerRoot.position = Vector3.Lerp(posicionInicial, posicionFinal, t);
            playerRoot.rotation = Quaternion.Slerp(rotacionInicial, rotacionFinal, t);

            yield return null;
        }

        playerRoot.position = posicionFinal;
        playerRoot.rotation = rotacionFinal;
    }
}