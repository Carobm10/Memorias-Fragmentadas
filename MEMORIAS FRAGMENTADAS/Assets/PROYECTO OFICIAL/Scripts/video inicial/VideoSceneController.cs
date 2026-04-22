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

    void Start()
    {
        Debug.Log("VideoSceneController iniciado");

        if (videoPlayer == null)
        {
            Debug.LogError("VideoPlayer NO asignado");
            return;
        }

        resolvedTargetCamera = ResolveTargetCamera();

        if (autoConfigureForVR)
        {
            ConfigureVideoForVR();
        }

#if UNITY_EDITOR
        if (autoAddCardboardSimulatorInEditor)
        {
            EnsureEditorCardboardPreview();
        }
#endif

        Debug.Log("Video clip asignado: " + (videoPlayer.clip != null ? videoPlayer.clip.name : "NINGUNO"));

        videoPlayer.Play();
        isPaused = false;

        ResetUI();
    }

    Camera ResolveTargetCamera()
    {
        if (targetCameraOverride != null)
            return targetCameraOverride;

        if (videoPlayer != null && videoPlayer.targetCamera != null)
            return videoPlayer.targetCamera;

        return Camera.main;
    }

    void ConfigureVideoForVR()
    {
        if (resolvedTargetCamera == null)
        {
            Debug.LogWarning("No se encontró cámara objetivo para el video. Asigna targetCameraOverride o etiqueta MainCamera.");
            return;
        }

        videoPlayer.renderMode = VideoRenderMode.CameraNearPlane;
        videoPlayer.targetCamera = resolvedTargetCamera;
        videoPlayer.targetCameraAlpha = 1f;
        videoPlayer.aspectRatio = vrAspectRatio;
        videoPlayer.waitForFirstFrame = true;
        videoPlayer.skipOnDrop = true;

        Debug.Log("Video configurado para VR en cámara: " + resolvedTargetCamera.name);
    }

#if UNITY_EDITOR
    void EnsureEditorCardboardPreview()
    {
        if (resolvedTargetCamera == null)
            return;

        Transform simulatorRoot = resolvedTargetCamera.transform.parent != null
            ? resolvedTargetCamera.transform.parent
            : resolvedTargetCamera.transform;

        if (simulatorRoot.GetComponent<CardboardSimulator>() == null)
        {
            simulatorRoot.gameObject.AddComponent<CardboardSimulator>();
            Debug.Log("Se añadió CardboardSimulator para previsualización en editor.");
        }
    }
#endif

    void Update()
    {
        if (changingScene || inputLocked)
            return;

        if (InputManagerCustom.PressA())
        {
            Debug.Log("Se detectó A");
            StartCoroutine(PressAtrasAndGoBack());
            return;
        }

        if (InputManagerCustom.PressX())
        {
            Debug.Log("Se detectó X");
            StartCoroutine(PressPlayPause());
            return;
        }

        if (InputManagerCustom.PressY())
        {
            Debug.Log("Se detectó Y");
            StartCoroutine(PressContinuar());
            return;
        }
    }

    void ResetUI()
    {
        if (imgAtras != null && atrasNormal != null)
            imgAtras.sprite = atrasNormal;

        if (imgPlayPause != null && playNormal != null)
            imgPlayPause.sprite = playNormal;

        if (imgContinuar != null && continuarNormal != null)
            imgContinuar.sprite = continuarNormal;
    }

    IEnumerator PressAtrasAndGoBack()
    {
        inputLocked = true;

        if (imgAtras != null && atrasGris != null)
            imgAtras.sprite = atrasGris;

        yield return new WaitForSeconds(pressFeedbackTime);

        if (imgAtras != null && atrasNormal != null)
            imgAtras.sprite = atrasNormal;

        Debug.Log("Cargando escena anterior: " + previousSceneName);
        changingScene = true;
        SceneManager.LoadScene(previousSceneName);
    }

    IEnumerator PressPlayPause()
    {
        inputLocked = true;

        if (imgPlayPause != null && playGris != null)
            imgPlayPause.sprite = playGris;

        yield return new WaitForSeconds(pressFeedbackTime);

        if (imgPlayPause != null && playNormal != null)
            imgPlayPause.sprite = playNormal;

        if (videoPlayer != null)
        {
            if (isPaused)
            {
                Debug.Log("Reanudando video");
                videoPlayer.Play();
                isPaused = false;
            }
            else
            {
                Debug.Log("Pausando video");
                videoPlayer.Pause();
                isPaused = true;
            }
        }

        inputLocked = false;
    }

    IEnumerator PressContinuar()
    {
        inputLocked = true;

        if (imgContinuar != null && continuarGris != null)
            imgContinuar.sprite = continuarGris;

        yield return new WaitForSeconds(pressFeedbackTime);

        if (imgContinuar != null && continuarNormal != null)
            imgContinuar.sprite = continuarNormal;

        Debug.Log("Cargando escena siguiente: " + nextSceneName);
        changingScene = true;
        SceneManager.LoadScene(nextSceneName);
    }
}