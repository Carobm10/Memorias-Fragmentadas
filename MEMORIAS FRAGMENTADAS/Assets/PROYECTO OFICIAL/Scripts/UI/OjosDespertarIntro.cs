using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class OjosDespertarIntro : MonoBehaviour
{
    [Header("Secuencia")]
    [Tooltip("Tiempo de espera antes de comenzar la animacion, en segundos.")]
    [Min(0f)]
    [SerializeField] private float esperaInicial = 0.15f;

    [Tooltip("Cantidad de parpadeos cortos antes de abrir completamente los ojos.")]
    [Min(0)]
    [SerializeField] private int parpadeosPrevios = 2;

    [Tooltip("Tiempo que tarda cada parpadeo en cerrarse o abrirse, en segundos.")]
    [Min(0.01f)]
    [SerializeField] private float duracionParpadeo = 0.09f;

    [Tooltip("Tiempo que tarda la apertura final al despertar, en segundos.")]
    [Min(0.01f)]
    [SerializeField] private float duracionAperturaFinal = 0.95f;

    [Tooltip("Pausa breve entre parpadeos previos, en segundos.")]
    [Min(0f)]
    [SerializeField] private float pausaEntreParpadeos = 0.05f;

    [Tooltip("Tiempo adicional en el que el ojo sigue abriendose un poco mas mientras la pantalla se desvanece.")]
    [Min(0.01f)]
    [SerializeField] private float duracionExpansionFinal = 0.55f;

    [Header("Rendimiento")]
    [Tooltip("Frecuencia de la animacion de apertura/cierre. 24 FPS suele sentirse fluido y reduce el costo de redibujado.")]
    [Range(12, 60)]
    [SerializeField] private int fpsAnimacion = 24;

    [Header("Forma Ocular")]
    [Tooltip("Alto del ojo cerrado, como fraccion de la pantalla. Valores bajos dejan una ranura muy fina.")]
    [Range(0.02f, 0.35f)]
    [SerializeField] private float altoOjoCerrado = 0.06f;

    [Tooltip("Alto del ojo abierto, como fraccion de la pantalla. Este valor define el tamano ocular final.")]
    [Range(0.15f, 0.75f)]
    [SerializeField] private float altoOjoAbierto = 0.42f;

    [Tooltip("Ancho del ojo respecto a la pantalla. 0.90 = casi toda la pantalla, 0.70 = mas estrecho.")]
    [Range(0.55f, 1f)]
    [SerializeField] private float anchoOjo = 0.94f;

    [Tooltip("Curvatura del borde ocular. Valores altos hacen la forma mas redonda y organica.")]
    [Range(0.5f, 3f)]
    [SerializeField] private float curvaturaOcular = 1.85f;

    [Tooltip("Suavizado del borde del parpado para evitar cortes bruscos.")]
    [Range(0.01f, 0.25f)]
    [SerializeField] private float suavizadoBorde = 0.06f;

    [Tooltip("Alto del ojo cuando termina de expandirse al despertar.")]
    [Range(0.45f, 0.9f)]
    [SerializeField] private float altoOjoFinalExtra = 0.68f;

    [Tooltip("Ancho del ojo cuando termina de expandirse al despertar.")]
    [Range(0.8f, 1.1f)]
    [SerializeField] private float anchoOjoFinalExtra = 1.0f;

    [Tooltip("En que momento de la apertura final empieza la expansion extra del ojo. 0.6 = empieza al 60%.")]
    [Range(0.1f, 0.9f)]
    [SerializeField] private float inicioExpansionFinal = 0.6f;

    [Tooltip("Intensidad del micro-movimiento durante la apertura final. 0 = sin movimiento, 1 = movimiento sutil.")]
    [Range(0f, 1f)]
    [SerializeField] private float microMovimientoApertura = 0.35f;

    [Tooltip("Porcentaje del final de la apertura en el que la pantalla completa se va desvaneciendo.")]
    [Range(0.1f, 1f)]
    [SerializeField] private float inicioFadePantalla = 0.45f;

    [Header("Apariencia")]
    [Tooltip("Color del parpado/overlay. Normalmente negro.")]
    [SerializeField] private Color colorParpado = Color.black;

    [Tooltip("Si esta activo, el efecto crea su propio overlay de pantalla completa.")]
    [SerializeField] private bool crearOverlayAutomatico = true;

    [Tooltip("Si esta activo, el efecto se reproduce automaticamente al iniciar.")]
    [SerializeField] private bool iniciarAutomaticamente = true;

    [Header("Opcional")]
    [Tooltip("Componentes que se desactivan mientras dura la intro.")]
    [SerializeField] private MonoBehaviour[] scriptsParaDesactivar;

    private GameObject overlayRoot;
    private RawImage overlayImage;
    private RectTransform overlayRect;
    private Coroutine secuenciaCoroutine;

    private Texture2D mascaraTexture;
    private Color32[] bufferPixels;
    private int texturaAncho = 512;
    private int texturaAlto = 256;
    private float coberturaActual = 1f;
    private float alphaPantallaActual = 1f;
    private float expansionFinalActual = 0f;

    private void Start()
    {
        if (iniciarAutomaticamente)
        {
            IniciarIntro();
        }
    }

    [ContextMenu("Iniciar Intro")]
    public void IniciarIntro()
    {
        if (secuenciaCoroutine != null)
        {
            StopCoroutine(secuenciaCoroutine);
        }

        if (crearOverlayAutomatico)
        {
            CrearOverlay();
        }

        secuenciaCoroutine = StartCoroutine(SeccionDespertar());
    }

    private void CrearOverlay()
    {
        if (overlayRoot != null)
        {
            Destroy(overlayRoot);
        }

        overlayRoot = new GameObject("Intro_Ojos_Overlay");
        overlayRoot.transform.SetParent(transform, false);

        Canvas canvas = overlayRoot.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = short.MaxValue;

        CanvasScaler scaler = overlayRoot.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;

        overlayRoot.AddComponent<GraphicRaycaster>();

        GameObject overlayGo = new GameObject("Mascara_Ocular", typeof(RectTransform), typeof(RawImage));
        overlayGo.transform.SetParent(overlayRoot.transform, false);

        overlayRect = overlayGo.GetComponent<RectTransform>();
        overlayRect.anchorMin = Vector2.zero;
        overlayRect.anchorMax = Vector2.one;
        overlayRect.offsetMin = Vector2.zero;
        overlayRect.offsetMax = Vector2.zero;

        overlayImage = overlayGo.GetComponent<RawImage>();
        overlayImage.raycastTarget = false;

        if (mascaraTexture == null)
        {
            mascaraTexture = new Texture2D(texturaAncho, texturaAlto, TextureFormat.RGBA32, false, true);
            mascaraTexture.wrapMode = TextureWrapMode.Clamp;
            mascaraTexture.filterMode = FilterMode.Bilinear;
            bufferPixels = new Color32[texturaAncho * texturaAlto];
        }

        overlayImage.texture = mascaraTexture;
        overlayImage.color = Color.white;
        alphaPantallaActual = 1f;
        DibujarMascara(1f);
    }

    private IEnumerator SeccionDespertar()
    {
        DesactivarScriptsOpcionales(true);

        yield return new WaitForSecondsRealtime(esperaInicial);

        for (int i = 0; i < parpadeosPrevios; i++)
        {
            yield return AnimarCobertura(1f, 0.18f, duracionParpadeo);
            yield return new WaitForSecondsRealtime(pausaEntreParpadeos);
            yield return AnimarCobertura(0.18f, 1f, duracionParpadeo);
            yield return new WaitForSecondsRealtime(pausaEntreParpadeos);
        }

        yield return AnimarAperturaFinalConFade();

        DesactivarScriptsOpcionales(false);

        if (overlayRoot != null)
        {
            Destroy(overlayRoot);
        }

        secuenciaCoroutine = null;
    }

    private IEnumerator AnimarCobertura(float coberturaInicio, float coberturaFin, float duracion)
    {
        int fps = Mathf.Clamp(fpsAnimacion, 12, 60);
        float paso = 1f / fps;
        int pasos = Mathf.Max(1, Mathf.CeilToInt(duracion * fps));

        for (int i = 0; i < pasos; i++)
        {
            float t = (i + 1) / (float)pasos;
            float suavizado = Mathf.SmoothStep(0f, 1f, t);
            coberturaActual = Mathf.Lerp(coberturaInicio, coberturaFin, suavizado);
            DibujarMascara(coberturaActual);
            yield return new WaitForSecondsRealtime(paso);
        }

        coberturaActual = coberturaFin;
        DibujarMascara(coberturaActual);
    }

    private IEnumerator AnimarAperturaFinalConFade()
    {
        int fps = Mathf.Clamp(fpsAnimacion, 12, 60);
        float paso = 1f / fps;
        int pasos = Mathf.Max(1, Mathf.CeilToInt(duracionAperturaFinal * fps));

        for (int i = 0; i < pasos; i++)
        {
            float t = (i + 1) / (float)pasos;
            float suave = Mathf.SmoothStep(0f, 1f, t);

            coberturaActual = Mathf.Lerp(1f, 0f, suave);
            expansionFinalActual = Mathf.SmoothStep(0f, 1f, Mathf.InverseLerp(inicioExpansionFinal, 1f, t));

            float fadeStart = Mathf.Clamp01(inicioFadePantalla);
            float fadeT = Mathf.InverseLerp(fadeStart, 1f, t);
            alphaPantallaActual = Mathf.Lerp(1f, 0f, Mathf.SmoothStep(0f, 1f, fadeT));

            AplicarMicroMovimiento(t, suave);
            DibujarMascara(coberturaActual);
            AplicarAlphaPantalla();

            yield return new WaitForSecondsRealtime(paso);
        }

        coberturaActual = 0f;
        alphaPantallaActual = 0f;
        expansionFinalActual = 1f;
        DibujarMascara(coberturaActual);
        AplicarAlphaPantalla();
        RestablecerTransformacionOverlay();
    }

    private void AplicarMicroMovimiento(float progreso, float suavizado)
    {
        if (overlayRect == null)
        {
            return;
        }

        float intensidad = Mathf.Clamp01(microMovimientoApertura);
        float temblor = Mathf.Sin(progreso * Mathf.PI * 6f) * (1f - suavizado) * intensidad;
        float escala = 1f + (temblor * 0.02f);

        overlayRect.localScale = new Vector3(escala, escala, 1f);
    }

    private void RestablecerTransformacionOverlay()
    {
        if (overlayRect != null)
        {
            overlayRect.localScale = Vector3.one;
        }
    }

    private void AplicarAlphaPantalla()
    {
        if (overlayImage == null)
        {
            return;
        }

        Color color = colorParpado;
        color.a = alphaPantallaActual;
        overlayImage.color = color;
    }

    private void DibujarMascara(float cobertura)
    {
        if (mascaraTexture == null || bufferPixels == null)
        {
            return;
        }

        Color32 colorPrincipal = colorParpado;
        float centroX = texturaAncho * 0.5f;
        float centroY = texturaAlto * 0.5f;
        float radioX = (texturaAncho * 0.5f) * Mathf.Lerp(anchoOjo, anchoOjoFinalExtra, expansionFinalActual);
        float altoFraccion = Mathf.Lerp(altoOjoCerrado, altoOjoAbierto, Mathf.Clamp01(1f - cobertura));
        altoFraccion = Mathf.Lerp(altoFraccion, altoOjoFinalExtra, expansionFinalActual);
        float radioY = (texturaAlto * 0.5f) * altoFraccion;
        float feather = Mathf.Max(1f, texturaAlto * suavizadoBorde);
        for (int y = 0; y < texturaAlto; y++)
        {
            float ny = (y - centroY) / Mathf.Max(1f, radioY);
            for (int x = 0; x < texturaAncho; x++)
            {
                float nx = (x - centroX) / Mathf.Max(1f, radioX);
                float distancia = Mathf.Pow(Mathf.Abs(nx), curvaturaOcular) + Mathf.Pow(Mathf.Abs(ny), curvaturaOcular);

                Color32 pixel;
                if (distancia <= 1f)
                {
                    pixel = new Color32(0, 0, 0, 0);
                }
                else
                {
                    float borde = Mathf.InverseLerp(1f, 1f + (feather / Mathf.Max(1f, texturaAlto * 0.5f)), distancia);
                    byte a = (byte)(Mathf.Clamp01(borde) * 255f);
                    pixel = new Color32(colorPrincipal.r, colorPrincipal.g, colorPrincipal.b, a);
                }

                int index = y * texturaAncho + x;
                bufferPixels[index] = pixel;
            }
        }

        mascaraTexture.SetPixels32(bufferPixels);
        mascaraTexture.Apply(false, false);
        AplicarAlphaPantalla();
    }

    private void DesactivarScriptsOpcionales(bool desactivar)
    {
        if (scriptsParaDesactivar == null)
        {
            return;
        }

        for (int i = 0; i < scriptsParaDesactivar.Length; i++)
        {
            MonoBehaviour script = scriptsParaDesactivar[i];
            if (script != null)
            {
                script.enabled = !desactivar;
            }
        }
    }
}
