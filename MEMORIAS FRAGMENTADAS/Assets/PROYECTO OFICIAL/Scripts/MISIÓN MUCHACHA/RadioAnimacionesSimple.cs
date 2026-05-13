using UnityEngine;
using TMPro;
using System.Collections;

public class RadioAnimacionesSimple : MonoBehaviour
{
    [Header("Control de misión")]
    public bool radioDesbloqueada = false;

    [Header("Sistema Raycast")]
    public Selected selectedRaycast;

    [Header("Radio - Visuales")]
    public GameObject radioNormalVisual;
    public GameObject radioVolteadoVisual;
    public GameObject abrirTapa;
    public GameObject radioSinPilasVisual;

    [Header("Tapita")]
    public Renderer tapaRenderer;
    public Color colorTapaSeleccionada = new Color(0.1f, 1f, 0.25f, 1f);

    [Header("Prompt")]
    public GameObject promptPanel;
    public TMP_Text promptText;

    [Header("Diálogo Rosa")]
    public GameObject dialogoPanel;
    public TMP_Text nombreDialogoText;
    public TMP_Text dialogoText;
    public GameObject botonSalirX;
    public string nombreNPC = "Rosa";

    [Header("Tiempos")]
    public float velocidadAnimacion = 0.45f;
    public float tiempoDialogoRosa = 4f;

    [Header("Animación")]
    public string nombreAnimacionAbrir = "AbrirTapa";

    private bool mirandoRadio = false;
    private bool secuenciaIniciada = false;
    private bool esperandoSalir = false;
    private Color colorOriginalTapa;

    void Start()
    {
        OcultarPrompt();
        OcultarDialogo();

        if (radioNormalVisual != null)
            radioNormalVisual.SetActive(true);

        if (radioVolteadoVisual != null)
            radioVolteadoVisual.SetActive(false);

        if (abrirTapa != null)
            abrirTapa.SetActive(false);

        if (radioSinPilasVisual != null)
            radioSinPilasVisual.SetActive(false);

        if (tapaRenderer != null)
            colorOriginalTapa = tapaRenderer.material.color;
    }

    void Update()
    {
        if (esperandoSalir)
        {
            if (InputManagerCustom.PressX())
                SalirABuscarPilas();

            return;
        }

        if (!mirandoRadio) return;
        if (secuenciaIniciada) return;
        if (!radioDesbloqueada) return;

        MostrarPrompt("Presiona B para revisar el radio");

        if (InputManagerCustom.PressB())
            StartCoroutine(SecuenciaRadio());
    }

    IEnumerator SecuenciaRadio()
    {
        secuenciaIniciada = true;

        if (selectedRaycast != null)
            selectedRaycast.enabled = false;

        if (radioNormalVisual != null)
            radioNormalVisual.SetActive(false);

        if (radioVolteadoVisual != null)
            radioVolteadoVisual.SetActive(true);

        if (tapaRenderer != null)
            tapaRenderer.material.color = colorTapaSeleccionada;

        // Mantener el prompt visible mientras espera el segundo B
        float tiempoBloqueo = 0.5f;
        float contador = 0f;

        while (contador < tiempoBloqueo)
        {
            contador += Time.deltaTime;
            MostrarPrompt("Presiona B para abrir la tapita");
            yield return null;
        }

        while (!InputManagerCustom.PressB())
        {
            MostrarPrompt("Presiona B para abrir la tapita");
            yield return null;
        }

        MostrarPrompt("Abriendo tapita...");

        if (tapaRenderer != null)
            tapaRenderer.material.color = colorOriginalTapa;

        if (radioVolteadoVisual != null)
            radioVolteadoVisual.SetActive(false);

        ReproducirAbrirTapa();

        yield return new WaitForSeconds(3f);

        if (abrirTapa != null)
            abrirTapa.SetActive(false);

        if (radioSinPilasVisual != null)
            radioSinPilasVisual.SetActive(true);

        OcultarPrompt();

        MostrarDialogo(
            "Ay, verdad que no tiene pilas.\n" +
            "Las pilas creo que están en alguno de esos cajones de ahí de abajo del radio."
        );

        yield return new WaitForSeconds(tiempoDialogoRosa);

        MostrarDialogo("Busca las pilas dentro de los cajones.");

        esperandoSalir = true;

        if (botonSalirX != null)
            botonSalirX.SetActive(true);
    }

    void SalirABuscarPilas()
    {
        esperandoSalir = false;

        OcultarDialogo();

        if (selectedRaycast != null)
            selectedRaycast.enabled = true;

        mirandoRadio = false;

        Debug.Log("Ahora el jugador puede buscar las pilas en los cajones.");
    }

    void ReproducirAbrirTapa()
    {
        if (abrirTapa == null)
        {
            Debug.LogError("No asignaste abrirTapa en el Inspector.");
            return;
        }

        abrirTapa.SetActive(false);
        abrirTapa.SetActive(true);

        Animator anim = abrirTapa.GetComponent<Animator>();

        if (anim == null)
        {
            Debug.LogError("abrirTapa no tiene Animator.");
            return;
        }

        anim.enabled = true;
        anim.applyRootMotion = false;
        anim.speed = velocidadAnimacion;

        anim.Rebind();
        anim.Update(0f);
        anim.Play(nombreAnimacionAbrir, 0, 0f);
    }

    public void MirarRadio()
    {
        if (!radioDesbloqueada)
        {
            mirandoRadio = false;
            OcultarPrompt();
            return;
        }

        if (secuenciaIniciada) return;

        mirandoRadio = true;
    }

    public void DejarMirarRadio()
    {
        if (secuenciaIniciada || esperandoSalir) return;

        mirandoRadio = false;
        OcultarPrompt();
    }

    public void DesbloquearRadio()
    {
        radioDesbloqueada = true;
        Debug.Log("Radio desbloqueada después de hablar con Rosa.");
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

    void MostrarDialogo(string texto)
    {
        if (dialogoPanel != null)
            dialogoPanel.SetActive(true);

        if (nombreDialogoText != null)
            nombreDialogoText.text = nombreNPC;

        if (dialogoText != null)
            dialogoText.text = texto;

        if (botonSalirX != null)
            botonSalirX.SetActive(false);
    }

    void OcultarDialogo()
    {
        if (dialogoPanel != null)
            dialogoPanel.SetActive(false);

        if (botonSalirX != null)
            botonSalirX.SetActive(false);
    }
}