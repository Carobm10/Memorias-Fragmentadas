using UnityEngine;
using TMPro;

public class BatteryPickup : MonoBehaviour
{
    [Header("Referencia cajón")]
    public Transform cajon;
    public float distanciaParaActivar = 0.15f;

    [Header("Prompt")]
    public GameObject promptPanel;
    public TMP_Text promptText;

    [Header("Color")]
    public Color colorDisponible = new Color(0.1f, 1f, 0.25f, 1f);

    private Vector3 posicionInicialCajon;
    private bool cajonAbierto = false;
    private bool mirando = false;
    private bool tomadas = false;

    private Renderer[] renderers;
    private Color[] coloresOriginales;

    void Start()
    {
        if (cajon != null)
            posicionInicialCajon = cajon.position;

        renderers = GetComponentsInChildren<Renderer>();
        coloresOriginales = new Color[renderers.Length];

        for (int i = 0; i < renderers.Length; i++)
        {
            coloresOriginales[i] = renderers[i].material.color;
        }

        OcultarPrompt();
    }

    void Update()
    {
        if (tomadas) return;

        RevisarSiCajonSeAbrio();

        if (!cajonAbierto) return;

        PintarPilasVerdes();

        if (mirando)
        {
            MostrarPrompt("Presiona B para tomar las pilas");

            if (InputManagerCustom.PressB())
            {
                TomarPilas();
            }
        }
    }

    void RevisarSiCajonSeAbrio()
    {
        if (cajon == null) return;

        float distancia = Vector3.Distance(cajon.position, posicionInicialCajon);

        if (distancia >= distanciaParaActivar)
        {
            cajonAbierto = true;
        }
    }

    void TomarPilas()
    {
        tomadas = true;

        MostrarPrompt("Has recogido las pilas. Cierra el cajón y pon las pilas en la radio.");

        Debug.Log("PILAS: jugador tomó las pilas.");

        foreach (Renderer r in renderers)
        {
            if (r != null)
                r.enabled = false;
        }

        Collider col = GetComponent<Collider>();
        if (col != null)
            col.enabled = false;
    }

    public void LookAtBatteries()
    {
        mirando = true;
    }

    public void StopLookingAtBatteries()
    {
        mirando = false;

        if (!tomadas)
            OcultarPrompt();
    }

    void PintarPilasVerdes()
    {
        foreach (Renderer r in renderers)
        {
            if (r != null)
                r.material.color = colorDisponible;
        }
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