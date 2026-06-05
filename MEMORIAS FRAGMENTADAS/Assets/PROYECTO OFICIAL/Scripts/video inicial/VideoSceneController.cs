using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using UnityEngine.UI;
using System.Collections;

public class VideoSceneController : MonoBehaviour
{
    [Header("Referencias")]
    public VideoPlayer videoPlayer;
    public Camera targetCameraOverride;

    [Header("Video Background")]
    public RawImage videoBackground;
    public int renderTextureWidth = 1920;
    public int renderTextureHeight = 1080;

    [Header("VR / Cardboard")]
    public bool autoConfigureForVR = true;
    public VideoAspectRatio vrAspectRatio = VideoAspectRatio.FitOutside;
    public bool autoAddCardboardSimulatorInEditor = true;

    [Header("Escenas")]
    public string previousSceneName = "MenuInicial";
    public string nextSceneName = "BASE";

    [Header("Imágenes UI")]
    public Image imgAtras;
    public Image imgPlayPause;
    public Image imgContinuar;

    [Header("Sprites normales")]
    public Sprite atrasNormal;
    public Sprite playNormal;
    public Sprite continuarNormal;

    [Header("Sprites grises")]
    public Sprite atrasGris;
    public Sprite playGris;
    public Sprite continuarGris;

    [Header("Tiempo visual al presionar")]
    public float pressFeedbackTime = 0.15f;

    private bool isPaused = false;
    private bool changingScene = false;
    private bool inputLocked = false;

    private Camera resolvedTargetCamera;
    private RenderTexture runtimeVideoTexture;
    private SceneTransitionManager transitionManager;

    void Start()
    {
        Debug.Log("VideoSceneController iniciado");

        if (videoPlayer == null)
        {
            Debug.LogError("VideoPlayer NO asignado");
            return;
        }

        resolvedTargetCamera = ResolveTargetCamera();

        transitionManager = FindFirstObjectByType<SceneTransitionManager>();
        if (transitionManager == null)
        {
            Debug.LogWarning("No se encontró SceneTransitionManager. Creando uno nuevo.");
            GameObject managerGO = new GameObject("SceneTransitionManager");
            transitionManager = managerGO.AddComponent<SceneTransitionManager>();
        }

        if (autoConfigureForVR)
            ConfigureVideoForVR();

#if UNITY_EDITOR
        if (autoAddCardboardSimulatorInEditor)
            EnsureEditorCardboardPreview();
#endif

        ResetUI();

        // Iniciar reproducción con una corutina robusta:
        // espera a que el SceneTransitionManager termine su transición (si la hay)
        // y luego arranca el video de forma segura.
        StartCoroutine(StartVideoWhenReady());
    }

    /// <summary>
    /// Espera a que cualquier transición en curso termine y luego reproduce el video.
    /// Esto evita que PreloadMultimediaContent (u otro código post-carga) detenga el video.
    /// </summary>
    private IEnumerator StartVideoWhenReady()
    {
        // Si hay una transición activa, esperar a que termine (máx. 5 segundos)
        float timeout = 5f;
        float elapsed = 0f;
        while (transitionManager != null && transitionManager.IsTransitioning && elapsed < timeout)
        {
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        if (elapsed >= timeout)
            Debug.LogWarning("VideoSceneController: timeout esperando fin de transición.");

        // Un frame extra de seguridad
        yield return null;

        if (videoPlayer == null) yield break;

        // Asegurar que el clip esté asignado
        if (videoPlayer.clip == null)
        {
            Debug.LogError("VideoPlayer no tiene clip asignado.");
            yield break;
        }

        Debug.Log($"▶ Iniciando video: {videoPlayer.clip.name}");

        // Preparar el VideoPlayer para que el primer frame aparezca sin retraso
        videoPlayer.playOnAwake = false;
        videoPlayer.Stop();
        videoPlayer.Prepare();

        // Esperar a que esté preparado (máx. 3 segundos)
        float prepTimeout = 3f;
        float prepElapsed = 0f;
        while (!videoPlayer.isPrepared && prepElapsed < prepTimeout)
        {
            prepElapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        videoPlayer.Play();
        isPaused = false;
        Debug.Log("✓ Video reproduciéndose");
    }

    Camera ResolveTargetCamera()
    {
        if (targetCameraOverride != null) return targetCameraOverride;
        if (videoPlayer != null && videoPlayer.targetCamera != null) return videoPlayer.targetCamera;
        return Camera.main;
    }

    void ConfigureVideoForVR()
    {
        SetupVideoBackground();

        if (runtimeVideoTexture == null)
        {
            Debug.LogWarning("No se pudo crear la textura de video.");
            return;
        }

        videoPlayer.renderMode = VideoRenderMode.RenderTexture;
        videoPlayer.targetTexture = runtimeVideoTexture;
        videoPlayer.aspectRatio = vrAspectRatio;
        videoPlayer.waitForFirstFrame = true;
        videoPlayer.skipOnDrop = true;

        Debug.Log("Video configurado para RenderTexture.");
    }

    void SetupVideoBackground()
    {
        if (videoBackground == null)
        {
            Canvas canvas = (imgAtras != null) ? imgAtras.canvas : FindFirstObjectByType<Canvas>();

            if (canvas != null)
            {
                videoBackground = canvas.GetComponentInChildren<RawImage>(true);

                if (videoBackground == null)
                {
                    GameObject bgObj = new GameObject("VideoBackground",
                        typeof(RectTransform), typeof(CanvasRenderer), typeof(RawImage));
                    bgObj.transform.SetParent(canvas.transform, false);
                    bgObj.transform.SetAsFirstSibling();

                    RectTransform rt = bgObj.GetComponent<RectTransform>();
                    rt.anchorMin = Vector2.zero;
                    rt.anchorMax = Vector2.one;
                    rt.offsetMin = rt.offsetMax = Vector2.zero;

                    videoBackground = bgObj.GetComponent<RawImage>();
                }
            }
        }

        if (runtimeVideoTexture == null)
        {
            runtimeVideoTexture = new RenderTexture(renderTextureWidth, renderTextureHeight, 0, RenderTextureFormat.ARGB32);
            runtimeVideoTexture.Create();
        }

        if (videoBackground != null)
        {
            videoBackground.transform.SetAsFirstSibling();
            videoBackground.texture = runtimeVideoTexture;
        }
    }

    void OnDestroy()
    {
        if (runtimeVideoTexture != null)
        {
            runtimeVideoTexture.Release();
            Destroy(runtimeVideoTexture);
        }
    }

#if UNITY_EDITOR
    void EnsureEditorCardboardPreview()
    {
        if (resolvedTargetCamera == null) return;

        Transform root = resolvedTargotCamera_SafeRoot();
        if (root.GetComponent<CardboardSimulator>() == null)
        {
            root.gameObject.AddComponent<CardboardSimulator>();
            Debug.Log("CardboardSimulator añadido para previsualización.");
        }
    }

    private Transform resolvedTargotCamera_SafeRoot()
    {
        return resolvedTargetCamera.transform.parent != null
            ? resolvedTargetCamera.transform.parent
            : resolvedTargetCamera.transform;
    }
#endif

    void Update()
    {
        if (changingScene || inputLocked) return;

        if (Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.JoystickButton2))
        {
            Debug.Log("Input: X → Atrás");
            StartCoroutine(PressAtrasAndGoBack());
        }
        else if (Input.GetKeyDown(KeyCode.Y) || Input.GetKeyDown(KeyCode.JoystickButton3))
        {
            Debug.Log("Input: Y → Continuar");
            StartCoroutine(PressContinuar());
        }
    }

    void ResetUI()
    {
        if (imgAtras     != null && atrasNormal    != null) imgAtras.sprite     = atrasNormal;
        if (imgPlayPause != null && playNormal      != null) imgPlayPause.sprite = playNormal;
        if (imgContinuar != null && continuarNormal != null) imgContinuar.sprite = continuarNormal;
    }

    IEnumerator PressAtrasAndGoBack()
    {
        inputLocked = true;
        if (imgAtras != null && atrasGris != null) imgAtras.sprite = atrasGris;
        yield return new WaitForSeconds(pressFeedbackTime);
        if (imgAtras != null && atrasNormal != null) imgAtras.sprite = atrasNormal;

        changingScene = true;

        PlayerPrefs.SetInt("VolvioDelVideo", 1);
        PlayerPrefs.Save();

        Debug.Log("← Escena anterior: " + previousSceneName);

        if (transitionManager != null)
            transitionManager.LoadScene(previousSceneName);
        else
            SceneManager.LoadScene(previousSceneName);
    }

    IEnumerator PressPlayPause()
    {
        inputLocked = true;
        if (imgPlayPause != null && playGris != null) imgPlayPause.sprite = playGris;
        yield return new WaitForSeconds(pressFeedbackTime);
        if (imgPlayPause != null && playNormal != null) imgPlayPause.sprite = playNormal;

        if (videoPlayer != null)
        {
            if (isPaused) { videoPlayer.Play();  isPaused = false; Debug.Log("▶ Video reanudado"); }
            else          { videoPlayer.Pause(); isPaused = true;  Debug.Log("⏸ Video pausado");  }
        }

        inputLocked = false;
    }

    IEnumerator PressContinuar()
    {
        inputLocked = true;
        if (imgContinuar != null && continuarGris != null) imgContinuar.sprite = continuarGris;
        yield return new WaitForSeconds(pressFeedbackTime);
        if (imgContinuar != null && continuarNormal != null) imgContinuar.sprite = continuarNormal;

        changingScene = true;
        Debug.Log("→ Escena siguiente: " + nextSceneName);

        if (transitionManager != null)
            transitionManager.LoadScene(nextSceneName);
        else
            SceneManager.LoadScene(nextSceneName);
    }
}