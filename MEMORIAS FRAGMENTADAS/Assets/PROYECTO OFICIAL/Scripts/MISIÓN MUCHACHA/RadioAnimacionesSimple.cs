using UnityEngine;
using TMPro;
using System.Collections;

public class RadioAnimacionesSimple : MonoBehaviour
{
    [Header("Sistema Raycast")]
    public Selected selectedRaycast;

    [Header("Radio - Visuales")]
    public GameObject radioNormalVisual;    // group4
    public GameObject radioVolteadoVisual;  // visual del radio volteado
    public GameObject abrirTapa;            // objeto abrir_tapa

    [Header("Tapita")]
    public Renderer tapaRenderer;
    public Color colorTapaSeleccionada = new Color(0.1f, 1f, 0.25f, 1f);

    [Header("Prompt")]
    public GameObject promptPanel;
    public TMP_Text promptText;

    [Header("Tiempos")]
    public float esperaAntesDeAbrir = 2f;
    public float velocidadAnimacion = 0.45f;

    [Header("Animación")]
    public string nombreAnimacionAbrir = "AbrirTapa";

    private bool mirandoRadio = false;
    private bool secuenciaIniciada = false;
    private Color colorOriginalTapa;

    void Start()
    {
        OcultarPrompt();

        if (radioNormalVisual != null)
            radioNormalVisual.SetActive(true);

        if (radioVolteadoVisual != null)
            radioVolteadoVisual.SetActive(false);

        if (abrirTapa != null)
            abrirTapa.SetActive(false);

        if (tapaRenderer != null)
            colorOriginalTapa = tapaRenderer.material.color;
    }

    void Update()
    {
        if (!mirandoRadio) return;
        if (secuenciaIniciada) return;

        MostrarPrompt("Presiona B para revisar el radio");

        if (InputManagerCustom.PressB())
            StartCoroutine(SecuenciaAutomaticaRadio());
    }

    IEnumerator SecuenciaAutomaticaRadio()
    {
        secuenciaIniciada = true;

        if (selectedRaycast != null)
            selectedRaycast.enabled = false;

        MostrarPrompt("Revisando parte trasera...");

        if (radioNormalVisual != null)
            radioNormalVisual.SetActive(false);

        if (radioVolteadoVisual != null)
            radioVolteadoVisual.SetActive(true);

        if (tapaRenderer != null)
            tapaRenderer.material.color = colorTapaSeleccionada;

        yield return new WaitForSeconds(esperaAntesDeAbrir);

        MostrarPrompt("Abriendo tapa...");

        if (tapaRenderer != null)
            tapaRenderer.material.color = colorOriginalTapa;

        if (radioVolteadoVisual != null)
            radioVolteadoVisual.SetActive(false);

        ReproducirAbrirTapa();

        yield return new WaitForSeconds(3f);

        OcultarPrompt();

        Debug.Log("Secuencia automática de radio terminada.");
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

        Debug.Log("Reproduciendo animación: " + nombreAnimacionAbrir);
    }

    public void MirarRadio()
    {
        mirandoRadio = true;
    }

    public void DejarMirarRadio()
    {
        if (secuenciaIniciada) return;

        mirandoRadio = false;
        OcultarPrompt();
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