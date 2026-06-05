using UnityEngine;
using TMPro;

public class TecladoRaycastInteractor : MonoBehaviour
{
    public float distanciaRaycast = 10f;
    public LayerMask layerTeclas = ~0;

    [Header("Panel Prompt (igual que DoorPromptPanel en Selected)")]
    [Tooltip("Asigna un panel con TMP_Text hijo. Se activa al mirar una tecla.")]
    public GameObject promptPanel;

    private Tecla teclaActual;
    private BotonPanelOpciones opcionActual;

    void Start()
    {
        if (layerTeclas == 0)
            layerTeclas = ~0;

        // Si no tiene prompt asignado, buscar el DoorPromptPanel del Selected en la misma cámara
        if (promptPanel == null)
        {
            Selected selected = GetComponent<Selected>();
            if (selected != null && selected.DoorPromptPanel != null)
            {
                promptPanel = selected.DoorPromptPanel;
            }
        }

        // Si todavía no hay, crear uno por código
        if (promptPanel == null)
            CrearPromptPanel();

        if (promptPanel != null)
            promptPanel.SetActive(false);
    }

    void CrearPromptPanel()
    {
        // Crear un Canvas world space como prompt
        GameObject canvasObj = new GameObject("PromptPanel_Menu");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;
        canvasObj.AddComponent<UnityEngine.UI.CanvasScaler>();

        // Crear panel de fondo
        GameObject panelObj = new GameObject("Panel");
        panelObj.transform.SetParent(canvasObj.transform, false);
        UnityEngine.UI.Image img = panelObj.AddComponent<UnityEngine.UI.Image>();
        img.color = new Color(0, 0, 0, 0.7f);

        RectTransform panelRect = panelObj.GetComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.5f, 0.75f);
        panelRect.anchorMax = new Vector2(0.5f, 0.75f);
        panelRect.pivot = new Vector2(0.5f, 0.5f);
        panelRect.sizeDelta = new Vector2(300, 60);

        // Crear texto
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(panelObj.transform, false);
        TextMeshProUGUI tmpText = textObj.AddComponent<TextMeshProUGUI>();
        tmpText.text = "Presiona B";
        tmpText.fontSize = 28;
        tmpText.alignment = TextAlignmentOptions.Center;
        tmpText.color = Color.white;

        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        promptPanel = canvasObj;
    }

    void Update()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, distanciaRaycast, layerTeclas))
        {
            // ========================================
            // DETECTAR TECLAS
            // ========================================

            Tecla teclaDetectada = hit.collider.GetComponent<Tecla>();
            if (teclaDetectada == null)
                teclaDetectada = hit.collider.GetComponentInParent<Tecla>();

            if (teclaDetectada != null)
            {
                LimpiarOpcion();

                if (teclaActual != teclaDetectada)
                {
                    if (teclaActual != null)
                        teclaActual.Deseleccionar();

                    teclaActual = teclaDetectada;
                    teclaActual.Seleccionar();
                }

                MostrarPrompt("Presiona B");

                if (InputManagerCustom.PressB())
                {
                    teclaActual.Presionar();
                    OcultarPrompt();
                }

                return;
            }

            // ========================================
            // DETECTAR BOTONES OPCIONES (JUGAR / AJUSTES)
            // ========================================

            BotonPanelOpciones opcion = hit.collider.GetComponent<BotonPanelOpciones>();
            if (opcion == null)
                opcion = hit.collider.GetComponentInParent<BotonPanelOpciones>();

            if (opcion != null)
            {
                LimpiarTecla();
                opcionActual = opcion;

                MostrarPrompt("Presiona B");

                if (InputManagerCustom.PressB())
                {
                    opcion.Presionar();
                    OcultarPrompt();
                }

                return;
            }
        }

        LimpiarTecla();
        LimpiarOpcion();
        OcultarPrompt();
    }

    void MostrarPrompt(string mensaje)
    {
        if (promptPanel == null) return;

        promptPanel.SetActive(true);

        TMP_Text texto = promptPanel.GetComponentInChildren<TMP_Text>(true);
        if (texto != null)
            texto.text = mensaje;
    }

    void OcultarPrompt()
    {
        if (promptPanel != null)
            promptPanel.SetActive(false);
    }

    void LimpiarTecla()
    {
        if (teclaActual != null)
        {
            teclaActual.Deseleccionar();
            teclaActual = null;
        }
    }

    void LimpiarOpcion()
    {
        opcionActual = null;
    }
}