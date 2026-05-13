using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

/// <summary>
/// Navegador de debug para transitar entre escenas.
/// Se añade a un Canvas pequeño en la esquina derecha de cada escena.
/// </summary>
public class SceneDebugNavigator : MonoBehaviour
{
    [Header("Configuración de UI")]
    [SerializeField] private Color buttonColor = new Color(0.2f, 0.2f, 0.2f, 0.8f);
    [SerializeField] private Color buttonTextColor = Color.white;
    [SerializeField] private Color hoverColor = new Color(0.4f, 0.4f, 0.4f, 0.9f);

    private SceneTransitionManager transitionManager;
    private bool isInitialized = false;

    void Awake()
    {
        // Buscar el manager de transiciones
        transitionManager = FindFirstObjectByType<SceneTransitionManager>();

        if (transitionManager == null)
        {
            Debug.LogWarning("No se encontró SceneTransitionManager en la escena. Creando uno nuevo...");
            GameObject managerGO = new GameObject("SceneTransitionManager");
            transitionManager = managerGO.AddComponent<SceneTransitionManager>();
        }

        isInitialized = true;
    }

    void Start()
    {
        if (!isInitialized)
        {
            Awake();
        }

        // El Canvas ya debería estar creado si este script está asignado al GameObject correcto
    }

    /// <summary>
    /// Crea el Canvas de debug con los botones de navegación
    /// Dos botones simples en esquinas superiores - sin texto
    /// </summary>
    public static void CreateDebugNavigatorUI()
    {
        // Buscar si ya existe
        SceneDebugNavigator existingNavigator = FindFirstObjectByType<SceneDebugNavigator>();
        if (existingNavigator != null)
        {
            Debug.LogWarning("SceneDebugNavigator ya existe en esta escena");
            return;
        }

        // IMPORTANTE: Asegurar que existe EventSystem (necesario para UI)
        if (FindFirstObjectByType<EventSystem>() == null)
        {
            GameObject eventSystemGO = new GameObject("EventSystem");
            eventSystemGO.AddComponent<EventSystem>();
            eventSystemGO.AddComponent<StandaloneInputModule>();
            Debug.Log("EventSystem creado para UI");
        }

        // Crear Canvas
        GameObject canvasGO = new GameObject("SceneDebugNavigator");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler canvasScaler = canvasGO.AddComponent<CanvasScaler>();
        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;

        GraphicRaycaster raycaster = canvasGO.AddComponent<GraphicRaycaster>();

        RectTransform rectTransform = canvasGO.GetComponent<RectTransform>();
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;

        // Botón "Anterior" - Esquina superior izquierda
        CreateSimpleButton(canvasGO, "Anterior", new Vector2(0, 1), new Vector2(0, 1), new Vector2(60, 60), new Vector2(35, -35), () =>
        {
            Debug.Log("→ Click en botón ANTERIOR");
            SceneTransitionManager manager = FindFirstObjectByType<SceneTransitionManager>();
            if (manager != null)
            {
                manager.LoadPreviousScene();
            }
            else
            {
                Debug.LogError("✗ No se encontró SceneTransitionManager");
            }
        });

        // Botón "Siguiente" - Esquina superior derecha
        CreateSimpleButton(canvasGO, "Siguiente", new Vector2(1, 1), new Vector2(1, 1), new Vector2(60, 60), new Vector2(-35, -35), () =>
        {
            Debug.Log("→ Click en botón SIGUIENTE");
            SceneTransitionManager manager = FindFirstObjectByType<SceneTransitionManager>();
            if (manager != null)
            {
                manager.LoadNextScene();
            }
            else
            {
                Debug.LogError("✗ No se encontró SceneTransitionManager");
            }
        });

        // Añadir el componente SceneDebugNavigator al Canvas
        SceneDebugNavigator navigator = canvasGO.AddComponent<SceneDebugNavigator>();
        canvas.sortingOrder = 10000;

        Debug.Log("SceneDebugNavigator UI creado - Botones en esquinas superiores");
    }

    private static void CreateSimpleButton(GameObject parent, string buttonName, Vector2 anchorMin, Vector2 anchorMax, Vector2 sizeDelta, Vector2 anchoredPosition, UnityEngine.Events.UnityAction onClick)
    {
        GameObject buttonGO = new GameObject("Button_" + buttonName);
        buttonGO.transform.SetParent(parent.transform, false);

        RectTransform buttonRect = buttonGO.AddComponent<RectTransform>();
        buttonRect.anchorMin = anchorMin;
        buttonRect.anchorMax = anchorMax;
        buttonRect.sizeDelta = sizeDelta;
        buttonRect.anchoredPosition = anchoredPosition;
        buttonRect.pivot = new Vector2(0.5f, 0.5f);

        // Image - Base del botón
        Image buttonImage = buttonGO.AddComponent<Image>();
        buttonImage.color = new Color(0.3f, 0.3f, 0.3f, 0.8f);
        buttonImage.raycastTarget = true;

        // Button - Componente interactivo
        Button button = buttonGO.AddComponent<Button>();
        button.targetGraphic = buttonImage;
        button.interactable = true;
        
        ColorBlock colors = ColorBlock.defaultColorBlock;
        colors.normalColor = new Color(0.3f, 0.3f, 0.3f, 0.8f);
        colors.highlightedColor = new Color(0.6f, 0.6f, 0.6f, 0.9f);
        colors.pressedColor = new Color(0.1f, 0.1f, 0.1f, 0.95f);
        colors.selectedColor = new Color(0.5f, 0.5f, 0.5f, 0.9f);
        colors.disabledColor = new Color(0.2f, 0.2f, 0.2f, 0.5f);
        colors.colorMultiplier = 1f;
        colors.fadeDuration = 0.1f;
        button.colors = colors;

        // Registrar click con Try-Catch para evitar errores
        try
        {
            button.onClick.AddListener(onClick);
            Debug.Log($"✓ Botón '{buttonName}' creado y listo para clicks");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"✗ Error al crear botón '{buttonName}': {e.Message}");
        }
    }

    /// <summary>
    /// Crea un canvas de carga con barra de progreso animada
    /// </summary>
    public static Canvas CreateLoadingScreen()
    {
        GameObject canvasGO = new GameObject("LoadingCanvas");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler canvasScaler = canvasGO.AddComponent<CanvasScaler>();
        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasScaler.referenceResolution = new Vector2(1080, 1920);
        canvasGO.AddComponent<GraphicRaycaster>();

        RectTransform rectTransform = canvasGO.GetComponent<RectTransform>();
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;

        // Fondo oscuro de pantalla completa
        GameObject bgGO = new GameObject("Background");
        bgGO.transform.SetParent(canvasGO.transform, false);

        RectTransform bgRect = bgGO.AddComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.offsetMin = Vector2.zero;
        bgRect.offsetMax = Vector2.zero;

        Image bgImage = bgGO.AddComponent<Image>();
        bgImage.color = new Color(0.05f, 0.05f, 0.08f, 0.95f); // Casi negro, muy opaco

        // Panel central con contenido
        GameObject panelGO = new GameObject("LoadingPanel");
        panelGO.transform.SetParent(canvasGO.transform, false);

        RectTransform panelRect = panelGO.AddComponent<RectTransform>();
        panelRect.anchoredPosition = Vector2.zero;
        panelRect.sizeDelta = new Vector2(600, 400);

        Image panelImage = panelGO.AddComponent<Image>();
        panelImage.color = new Color(0.1f, 0.1f, 0.15f, 0.8f); // Panel gris oscuro

        // Texto "Cargando" animado
        GameObject textGO = new GameObject("LoadingText");
        textGO.transform.SetParent(panelGO.transform, false);

        RectTransform textRect = textGO.AddComponent<RectTransform>();
        textRect.anchoredPosition = new Vector2(0, 80);
        textRect.sizeDelta = new Vector2(500, 100);

        Text text = textGO.AddComponent<Text>();
        text.text = "Cargando.";
        text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        text.fontSize = 48;
        text.fontStyle = FontStyle.Bold;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = Color.white;

        // Barra de progreso - Contenedor
        GameObject barContainerGO = new GameObject("ProgressBarContainer");
        barContainerGO.transform.SetParent(panelGO.transform, false);

        RectTransform barContainerRect = barContainerGO.AddComponent<RectTransform>();
        barContainerRect.anchoredPosition = new Vector2(0, -20);
        barContainerRect.sizeDelta = new Vector2(500, 40);

        Image barContainerImage = barContainerGO.AddComponent<Image>();
        barContainerImage.color = new Color(0.2f, 0.2f, 0.25f, 1f); // Gris claro para el fondo

        // Barra de progreso - Fill
        GameObject barFillGO = new GameObject("ProgressBarFill");
        barFillGO.transform.SetParent(barContainerGO.transform, false);

        RectTransform barFillRect = barFillGO.AddComponent<RectTransform>();
        barFillRect.anchorMin = Vector2.zero;
        barFillRect.anchorMax = new Vector2(1, 1);
        barFillRect.offsetMin = Vector2.zero;
        barFillRect.offsetMax = Vector2.zero;

        Image barFillImage = barFillGO.AddComponent<Image>();
        barFillImage.color = new Color(0.2f, 0.8f, 1f, 1f); // Azul claro - barra de progreso

        // Configurar la barra como Image con Fill
        barFillImage.type = Image.Type.Filled;
        barFillImage.fillMethod = Image.FillMethod.Horizontal;
        barFillImage.fillOrigin = (int)Image.OriginHorizontal.Left;
        barFillImage.fillAmount = 0.2f; // Iniciar en 20%

        // Texto de porcentaje
        GameObject percentGO = new GameObject("PercentText");
        percentGO.transform.SetParent(panelGO.transform, false);

        RectTransform percentRect = percentGO.AddComponent<RectTransform>();
        percentRect.anchoredPosition = new Vector2(0, -80);
        percentRect.sizeDelta = new Vector2(500, 60);

        Text percentText = percentGO.AddComponent<Text>();
        percentText.text = "0%";
        percentText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        percentText.fontSize = 32;
        percentText.fontStyle = FontStyle.Bold;
        percentText.alignment = TextAnchor.MiddleCenter;
        percentText.color = new Color(0.2f, 0.8f, 1f, 1f); // Azul claro

        // Animador
        LoadingScreenAnimator animator = canvasGO.AddComponent<LoadingScreenAnimator>();
        animator.Initialize(text, barFillImage);

        // Crear un MonoBehaviour helper para la corutina
        PercentageUpdater updater = canvasGO.AddComponent<PercentageUpdater>();
        updater.StartUpdating(percentText, barFillImage, animator);

        canvas.sortingOrder = 9999;

        return canvas;
    }

}

/// <summary>
/// Helper para actualizar el porcentaje en la pantalla de carga
/// </summary>
public class PercentageUpdater : MonoBehaviour
{
    private Text percentText;
    private Image progressBar;
    private LoadingScreenAnimator animator;

    public void StartUpdating(Text pText, Image pBar, LoadingScreenAnimator pAnimator)
    {
        percentText = pText;
        progressBar = pBar;
        animator = pAnimator;
        StartCoroutine(UpdatePercentageText());
    }

    private IEnumerator UpdatePercentageText()
    {
        while (percentText != null && progressBar != null)
        {
            int percentage = (int)(progressBar.fillAmount * 100f);
            percentText.text = percentage + "%";
            animator.SetProgress(progressBar.fillAmount + Random.Range(0.01f, 0.05f));
            yield return new WaitForSeconds(0.1f);
        }
    }
}
