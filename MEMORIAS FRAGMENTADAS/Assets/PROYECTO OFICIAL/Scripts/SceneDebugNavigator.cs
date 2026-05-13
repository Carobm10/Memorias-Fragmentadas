using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

/// <summary>
/// Navegador de debug para transitar entre escenas.
/// </summary>
public class SceneDebugNavigator : MonoBehaviour
{
    /// <summary>
    /// Devuelve la fuente built-in correcta según la versión de Unity.
    /// Unity 2023+ eliminó "Arial.ttf" y lo reemplazó por "LegacyRuntime.ttf".
    /// </summary>
    private static Font GetBuiltinFont()
    {
        // Intentar primero LegacyRuntime (Unity 2023+)
        Font font = null;
        try { font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf"); } catch { }
        if (font != null) return font;

        // Fallback para versiones anteriores
        try { font = Resources.GetBuiltinResource<Font>("Arial.ttf"); } catch { }
        if (font != null) return font;

        // Último recurso: cualquier fuente disponible en el proyecto
        Debug.LogWarning("No se encontró fuente built-in. Usando Font por defecto.");
        return new Font();
    }

    void Awake()
    {
        // Buscar o crear el SceneTransitionManager
        if (FindFirstObjectByType<SceneTransitionManager>() == null)
        {
            GameObject managerGO = new GameObject("SceneTransitionManager");
            managerGO.AddComponent<SceneTransitionManager>();
            Debug.Log("SceneDebugNavigator: SceneTransitionManager creado automáticamente");
        }
    }

    /// <summary>
    /// Crea el Canvas de debug con botones de navegación en las esquinas superiores.
    /// </summary>
    public static void CreateDebugNavigatorUI()
    {
        // Evitar duplicados
        if (FindFirstObjectByType<SceneDebugNavigator>() != null)
        {
            Debug.LogWarning("SceneDebugNavigator ya existe en esta escena");
            return;
        }

        // EventSystem necesario para que los botones respondan
        if (FindFirstObjectByType<EventSystem>() == null)
        {
            GameObject esGO = new GameObject("EventSystem");
            esGO.AddComponent<EventSystem>();
            esGO.AddComponent<StandaloneInputModule>();
            Debug.Log("EventSystem creado para UI");
        }

        // Canvas
        GameObject canvasGO = new GameObject("SceneDebugNavigator");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 10000;

        CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080, 1920);

        canvasGO.AddComponent<GraphicRaycaster>();

        RectTransform rt = canvasGO.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        // Botón ANTERIOR — esquina superior izquierda
        // pivot (0,1): la esquina superior-izquierda del botón se ancla al borde
        CreateNavButton(canvasGO, "Anterior",
            anchor: new Vector2(0, 1),
            pivot:  new Vector2(0, 1),
            size: new Vector2(90, 90),
            position: new Vector2(10, -10),
            label: "◀",
            onClick: () =>
            {
                SceneTransitionManager manager = FindFirstObjectByType<SceneTransitionManager>();
                if (manager != null)
                    manager.LoadPreviousScene();
                else
                    Debug.LogError("✗ No se encontró SceneTransitionManager");
            });

        // Botón SIGUIENTE — esquina superior derecha
        // pivot (1,1): la esquina superior-derecha del botón se ancla al borde → nunca se corta
        CreateNavButton(canvasGO, "Siguiente",
            anchor: new Vector2(1, 1),
            pivot:  new Vector2(1, 1),
            size: new Vector2(90, 90),
            position: new Vector2(-10, -10),
            label: "▶",
            onClick: () =>
            {
                SceneTransitionManager manager = FindFirstObjectByType<SceneTransitionManager>();
                if (manager != null)
                    manager.LoadNextScene();
                else
                    Debug.LogError("✗ No se encontró SceneTransitionManager");
            });

        canvasGO.AddComponent<SceneDebugNavigator>();

        Debug.Log("SceneDebugNavigator creado con botones ◀ ▶");
    }

    private static void CreateNavButton(
        GameObject parent,
        string name,
        Vector2 anchor,
        Vector2 pivot,
        Vector2 size, Vector2 position,
        string label,
        UnityEngine.Events.UnityAction onClick)
    {
        GameObject go = new GameObject("Btn_" + name);
        go.transform.SetParent(parent.transform, false);

        RectTransform rect = go.AddComponent<RectTransform>();
        // Mismo valor en anchorMin y anchorMax = tamaño fijo (no se estira)
        rect.anchorMin = anchor;
        rect.anchorMax = anchor;
        rect.pivot = pivot;
        rect.sizeDelta = size;
        rect.anchoredPosition = position;

        Image img = go.AddComponent<Image>();
        img.color = new Color(0.15f, 0.15f, 0.15f, 0.85f);
        img.raycastTarget = true;

        Button btn = go.AddComponent<Button>();
        btn.targetGraphic = img;

        ColorBlock cb = ColorBlock.defaultColorBlock;
        cb.normalColor    = new Color(0.15f, 0.15f, 0.15f, 0.85f);
        cb.highlightedColor = new Color(0.5f, 0.5f, 0.5f, 0.95f);
        cb.pressedColor   = new Color(0.05f, 0.05f, 0.05f, 1f);
        cb.selectedColor  = cb.highlightedColor;
        cb.colorMultiplier = 1f;
        cb.fadeDuration   = 0.1f;
        btn.colors = cb;

        btn.onClick.AddListener(onClick);

        // Texto del botón
        GameObject textGO = new GameObject("Label");
        textGO.transform.SetParent(go.transform, false);

        RectTransform textRect = textGO.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        Text text = textGO.AddComponent<Text>();
        text.text = label;
        text.font = GetBuiltinFont();
        text.fontSize = 36;
        text.fontStyle = FontStyle.Bold;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = Color.white;
        text.raycastTarget = false; // El raycaster va en el Image padre

        Debug.Log($"✓ Botón '{name}' creado");
    }

    /// <summary>
    /// Crea la pantalla de carga animada.
    /// </summary>
    public static Canvas CreateLoadingScreen()
    {
        // EventSystem necesario para evitar warnings de UI
        if (FindFirstObjectByType<EventSystem>() == null)
        {
            GameObject esGO = new GameObject("EventSystem");
            esGO.AddComponent<EventSystem>();
            esGO.AddComponent<StandaloneInputModule>();
        }

        GameObject canvasGO = new GameObject("LoadingCanvas");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 9999;

        CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080, 1920);

        canvasGO.AddComponent<GraphicRaycaster>();

        // Fondo oscuro
        GameObject bg = new GameObject("Background");
        bg.transform.SetParent(canvasGO.transform, false);
        RectTransform bgRect = bg.AddComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.offsetMin = bgRect.offsetMax = Vector2.zero;
        Image bgImg = bg.AddComponent<Image>();
        bgImg.color = new Color(0.04f, 0.04f, 0.07f, 0.97f);

        // Panel central
        GameObject panel = new GameObject("Panel");
        panel.transform.SetParent(canvasGO.transform, false);
        RectTransform panelRect = panel.AddComponent<RectTransform>();
        panelRect.anchoredPosition = Vector2.zero;
        panelRect.sizeDelta = new Vector2(700, 300);
        Image panelImg = panel.AddComponent<Image>();
        panelImg.color = new Color(0.1f, 0.1f, 0.15f, 0.9f);

        // Texto "Cargando..."
        GameObject textGO = new GameObject("LoadingText");
        textGO.transform.SetParent(panel.transform, false);
        RectTransform textRect = textGO.AddComponent<RectTransform>();
        textRect.anchoredPosition = new Vector2(0, 70);
        textRect.sizeDelta = new Vector2(600, 80);
        Text text = textGO.AddComponent<Text>();
        text.text = "Cargando.";
        text.font = GetBuiltinFont();
        text.fontSize = 52;
        text.fontStyle = FontStyle.Bold;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = Color.white;

        // Contenedor barra de progreso
        GameObject barBG = new GameObject("BarBackground");
        barBG.transform.SetParent(panel.transform, false);
        RectTransform barBGRect = barBG.AddComponent<RectTransform>();
        barBGRect.anchoredPosition = new Vector2(0, -10);
        barBGRect.sizeDelta = new Vector2(580, 36);
        Image barBGImg = barBG.AddComponent<Image>();
        barBGImg.color = new Color(0.2f, 0.2f, 0.25f, 1f);

        // Fill de la barra
        GameObject barFill = new GameObject("BarFill");
        barFill.transform.SetParent(barBG.transform, false);
        RectTransform barFillRect = barFill.AddComponent<RectTransform>();
        barFillRect.anchorMin = Vector2.zero;
        barFillRect.anchorMax = Vector2.one;
        barFillRect.offsetMin = barFillRect.offsetMax = Vector2.zero;
        Image barFillImg = barFill.AddComponent<Image>();
        barFillImg.color = new Color(0.2f, 0.8f, 1f, 1f);
        barFillImg.type = Image.Type.Filled;
        barFillImg.fillMethod = Image.FillMethod.Horizontal;
        barFillImg.fillOrigin = (int)Image.OriginHorizontal.Left;
        barFillImg.fillAmount = 0f;

        // Texto de porcentaje
        GameObject pctGO = new GameObject("PercentText");
        pctGO.transform.SetParent(panel.transform, false);
        RectTransform pctRect = pctGO.AddComponent<RectTransform>();
        pctRect.anchoredPosition = new Vector2(0, -75);
        pctRect.sizeDelta = new Vector2(580, 50);
        Text pctText = pctGO.AddComponent<Text>();
        pctText.text = "0%";
        pctText.font = GetBuiltinFont();
        pctText.fontSize = 30;
        pctText.fontStyle = FontStyle.Bold;
        pctText.alignment = TextAnchor.MiddleCenter;
        pctText.color = new Color(0.2f, 0.8f, 1f, 1f);

        // Animador: único responsable de barra y texto
        LoadingScreenAnimator animator = canvasGO.AddComponent<LoadingScreenAnimator>();
        animator.Initialize(text, barFillImg);

        // Actualizador de porcentaje (separado para no contaminar el animador)
        PercentageUpdater updater = canvasGO.AddComponent<PercentageUpdater>();
        updater.StartUpdating(pctText, barFillImg);

        return canvas;
    }
}

/// <summary>
/// Actualiza el texto de porcentaje según el fillAmount de la barra.
/// </summary>
public class PercentageUpdater : MonoBehaviour
{
    private Text percentText;
    private Image progressBar;

    public void StartUpdating(Text pText, Image pBar)
    {
        percentText = pText;
        progressBar = pBar;
        StartCoroutine(UpdateLoop());
    }

    private IEnumerator UpdateLoop()
    {
        while (this != null && percentText != null && progressBar != null)
        {
            int pct = Mathf.RoundToInt(progressBar.fillAmount * 100f);
            percentText.text = pct + "%";

            // Parar cuando llegue al 100%
            if (pct >= 100)
                yield break;

            yield return new WaitForSecondsRealtime(0.1f);
        }
    }
}