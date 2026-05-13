using UnityEngine;
using TMPro;
using System.Collections;

public class Radio360BackDetector : MonoBehaviour
{
    [Header("Misión")]
    public KitchenRadioMissionManager missionManager;

    [Header("Animator de la radio")]
    public Animator radioAnimator;

    [Header("Prompt")]
    public GameObject promptPanel;
    public TMP_Text promptText;

    [Header("Objeto tapa visual")]
    public Renderer tapaRenderer;

    [Header("Color tapa activa")]
    public Color colorSeleccion = new Color(0.1f, 1f, 0.25f, 1f);

    [Header("Ángulo trasero")]
    public float anguloMin = 150f;
    public float anguloMax = 210f;

    [Header("Debug")]
    public bool debugActivo = true;

    private Color colorOriginal;
    private bool puertaDetectada = false;
    private bool tapaAbierta = false;

    void Start()
    {
        AutoConfigurarReferenciasDelClon();

        if (tapaRenderer != null)
            colorOriginal = tapaRenderer.material.color;

        OcultarPrompt();

        if (debugActivo)
        {
            Debug.Log("RADIO360: iniciado en " + gameObject.name);
            Debug.Log("RADIO360: Animator usado = " + (radioAnimator != null ? radioAnimator.gameObject.name : "VACÍO"));
            Debug.Log("RADIO360: Tapa usada = " + (tapaRenderer != null ? tapaRenderer.gameObject.name : "VACÍA"));
        }
    }

    void AutoConfigurarReferenciasDelClon()
    {
        // Si el Animator asignado pertenece al radio original y no al clon, usamos uno del clon.
        if (radioAnimator == null || !radioAnimator.transform.IsChildOf(transform))
        {
            Animator[] animators = GetComponentsInChildren<Animator>(true);

            foreach (Animator anim in animators)
            {
                if (anim.runtimeAnimatorController != null &&
                    anim.runtimeAnimatorController.name == "AC_RadioMission")
                {
                    radioAnimator = anim;
                    break;
                }
            }

            if (radioAnimator == null && animators.Length > 0)
                radioAnimator = animators[0];
        }

        // Si la tapa asignada pertenece al radio original y no al clon, buscamos la tapa dentro del clon.
        if (tapaRenderer == null || !tapaRenderer.transform.IsChildOf(transform))
        {
            Renderer[] renderers = GetComponentsInChildren<Renderer>(true);

            foreach (Renderer r in renderers)
            {
                string nombre = r.gameObject.name.ToLower();

                if (nombre.Contains("tapa") || nombre.Contains("pilas"))
                {
                    tapaRenderer = r;
                    break;
                }
            }
        }
    }

    void Update()
    {
        if (tapaAbierta) return;

        float y = transform.eulerAngles.y;
        bool estaAtras = y >= anguloMin && y <= anguloMax;

        if (estaAtras)
        {
            ActivarTapa();

            if (InputManagerCustom.PressB())
            {
                AbrirTapa();
            }
        }
        else
        {
            DesactivarTapa();
        }
    }

    void ActivarTapa()
    {
        if (!puertaDetectada)
        {
            puertaDetectada = true;

            if (tapaRenderer != null)
                tapaRenderer.material.color = colorSeleccion;

            if (debugActivo)
                Debug.Log("RADIO360: tapa detectada. Puede abrirse.");
        }

        MostrarPrompt("Presiona B para abrir la tapa");
    }

    void DesactivarTapa()
    {
        if (!puertaDetectada) return;

        puertaDetectada = false;

        if (tapaRenderer != null)
            tapaRenderer.material.color = colorOriginal;

        OcultarPrompt();
    }

    void AbrirTapa()
    {
        if (tapaAbierta) return;

        tapaAbierta = true;
        OcultarPrompt();

        if (tapaRenderer != null)
            tapaRenderer.material.color = colorOriginal;

        if (radioAnimator != null)
        {
            Debug.Log("RADIO360: intentando reproducir AbrirTapa en " + radioAnimator.gameObject.name);

            radioAnimator.Play("AbrirTapa", 0, 0f);
            radioAnimator.Update(0f);
        }
        else
        {
            Debug.LogError("RADIO360: no hay Animator asignado para abrir tapa.");
        }

        StartCoroutine(AvisarMisionDespues());
    }

    IEnumerator AvisarMisionDespues()
    {
        yield return new WaitForSeconds(2.5f);

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