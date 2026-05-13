using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;
using System.Collections;

/// <summary>
/// Gestor centralizado de transiciones entre escenas.
/// Maneja precarga, indicadores de carga, desactivación de cámaras, y control de multimedia.
/// </summary>
public class SceneTransitionManager : MonoBehaviour
{
    private static SceneTransitionManager instance;

    [Header("Configuración de Carga")]
    [SerializeField] private bool useAsyncLoading = true;
    [SerializeField] private float minLoadingScreenTime = 1f;

    [Header("Canvas de Carga")]
    [SerializeField] private Canvas loadingCanvasPrefab;
    private Canvas currentLoadingCanvas;
    private LoadingScreenAnimator currentLoadingAnimator;

    private bool isTransitioning = false;

    // Diccionario de escenas en orden
    private string[] sceneSequence = { "Menu", "Escena_VideoIntro", "BASE" };

    void Awake()
    {
        // Implementar singleton
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Cambia a la siguiente escena en la secuencia
    /// </summary>
    public void LoadNextScene()
    {
        if (isTransitioning)
        {
            Debug.LogWarning("⚠ Ya hay una transición en proceso. Ignorando nuevo click.");
            return;
        }

        string currentScene = SceneManager.GetActiveScene().name;
        int currentIndex = System.Array.IndexOf(sceneSequence, currentScene);

        if (currentIndex >= 0 && currentIndex < sceneSequence.Length - 1)
        {
            LoadScene(sceneSequence[currentIndex + 1]);
        }
        else if (currentIndex == sceneSequence.Length - 1)
        {
            // Si es la última escena, vuelve al menú
            LoadScene(sceneSequence[0]);
        }
        else
        {
            Debug.LogWarning($"Escena actual '{currentScene}' no está en la secuencia");
            LoadScene("Menu");
        }
    }

    /// <summary>
    /// Cambia a la escena anterior en la secuencia
    /// </summary>
    public void LoadPreviousScene()
    {
        if (isTransitioning)
        {
            Debug.LogWarning("⚠ Ya hay una transición en proceso. Ignorando nuevo click.");
            return;
        }

        string currentScene = SceneManager.GetActiveScene().name;
        int currentIndex = System.Array.IndexOf(sceneSequence, currentScene);

        if (currentIndex > 0)
        {
            LoadScene(sceneSequence[currentIndex - 1]);
        }
        else if (currentIndex == 0)
        {
            // Si es el menú, va a la última escena
            LoadScene(sceneSequence[sceneSequence.Length - 1]);
        }
        else
        {
            Debug.LogWarning($"Escena actual '{currentScene}' no está en la secuencia");
            LoadScene("Menu");
        }
    }

    /// <summary>
    /// Carga una escena específica por nombre
    /// </summary>
    public void LoadScene(string sceneName)
    {
        if (isTransitioning)
        {
            Debug.LogWarning("⚠ Ya hay una transición en proceso. Ignorando nuevo click.");
            return;
        }

        Debug.Log($"→ Iniciando transición a: {sceneName}");
        StartCoroutine(TransitionToScene(sceneName));
    }

    private IEnumerator TransitionToScene(string sceneName)
    {
        isTransitioning = true;
        Debug.Log($"[TRANSICIÓN] Iniciando: {sceneName}");

        // Mostrar pantalla de carga
        ShowLoadingScreen();

        float loadStartTime = Time.time;

        if (useAsyncLoading)
        {
            // Precarga asincrónica de la escena
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
            asyncLoad.allowSceneActivation = false;

            // Esperar a que la escena esté precargada (pero no activada)
            while (!asyncLoad.isDone && asyncLoad.progress < 0.9f)
            {
                yield return new WaitForEndOfFrame();
            }

            // Permitir que pase el tiempo mínimo de carga
            while (Time.time - loadStartTime < minLoadingScreenTime)
            {
                yield return new WaitForEndOfFrame();
            }

            // Permitir la activación de la escena
            asyncLoad.allowSceneActivation = true;
        }
        else
        {
            // Carga sincrónica
            while (Time.time - loadStartTime < minLoadingScreenTime)
            {
                yield return new WaitForEndOfFrame();
            }

            SceneManager.LoadScene(sceneName);
        }

        // Esperar a que la escena esté completamente cargada y activa
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        // Precarga de contenido multimedia (pero no reproducción automática)
        PreloadMultimediaContent();

        // Crear UI de debug automáticamente
        SceneDebugNavigator.CreateDebugNavigatorUI();

        // Asegurar que haya cámaras activas en la nueva escena
        EnsureCameraActive();

        // Ocultar pantalla de carga
        HideLoadingScreen();

        // Permitir que el contenido comience cuando esté listo (sin reproducir automáticamente)
        yield return new WaitForEndOfFrame();

        Debug.Log($"[TRANSICIÓN] ✓ Completada: {sceneName}");
        isTransitioning = false;
        Debug.Log("[TRANSICIÓN] Flag reseteado - Listo para nueva transición");
    }

    private void EnsureCameraActive()
    {
        // Asegurar que la nueva escena tenga al menos una cámara activa
        Camera[] cameras = FindObjectsByType<Camera>(FindObjectsSortMode.None);
        
        bool cameraActive = false;
        foreach (Camera cam in cameras)
        {
            if (cam.gameObject.activeSelf)
            {
                cameraActive = true;
                break;
            }
        }

        // Si no hay cámara activa, activar la primera que encuentre
        if (!cameraActive && cameras.Length > 0)
        {
            cameras[0].gameObject.SetActive(true);
            Debug.Log($"✓ Cámara activada: {cameras[0].gameObject.name}");
        }
        else if (cameras.Length > 0)
        {
            Debug.Log($"✓ Cámara ya activa en la escena");
        }
        else
        {
            Debug.LogWarning("⚠ No hay cámaras en la escena");
        }
    }

    private void PreloadMultimediaContent()
    {
        // Buscar VideoPlayers y pausarlos
        VideoPlayer[] videoPlayers = FindObjectsByType<VideoPlayer>(FindObjectsSortMode.None);
        foreach (VideoPlayer player in videoPlayers)
        {
            player.Stop();
            player.playOnAwake = false;
        }

        // Buscar AudioSources y silenciarlos
        AudioSource[] audioSources = FindObjectsByType<AudioSource>(FindObjectsSortMode.None);
        foreach (AudioSource source in audioSources)
        {
            source.Stop();
            source.playOnAwake = false;
        }

        // Asegurar que hay un AudioListener (evita warnings de audio)
        if (FindFirstObjectByType<AudioListener>() == null)
        {
            Camera cam = FindFirstObjectByType<Camera>();
            if (cam != null && cam.GetComponent<AudioListener>() == null)
            {
                cam.gameObject.AddComponent<AudioListener>();
                Debug.Log("✓ AudioListener añadido a la cámara");
            }
        }

        Debug.Log("Contenido multimedia precargado (sin reproducción automática)");
    }

    private void ShowLoadingScreen()
    {
        if (currentLoadingCanvas != null)
        {
            Destroy(currentLoadingCanvas.gameObject);
        }

        // Crear la pantalla de carga visualmente mejorada
        currentLoadingCanvas = SceneDebugNavigator.CreateLoadingScreen();
        if (currentLoadingCanvas != null)
        {
            DontDestroyOnLoad(currentLoadingCanvas.gameObject);
            currentLoadingAnimator = currentLoadingCanvas.GetComponent<LoadingScreenAnimator>();
        }

        Debug.Log("Pantalla de carga mostrada");
    }

    private void HideLoadingScreen()
    {
        if (currentLoadingAnimator != null)
        {
            currentLoadingAnimator.Complete();
        }

        StartCoroutine(DestroyLoadingScreenDelayed());
    }

    private IEnumerator DestroyLoadingScreenDelayed()
    {
        // Esperar un poco para que se vea el "¡Listo!" y la barra completa
        yield return new WaitForSeconds(0.5f);

        if (currentLoadingCanvas != null)
        {
            Destroy(currentLoadingCanvas.gameObject);
            currentLoadingCanvas = null;
            currentLoadingAnimator = null;
        }

        Debug.Log("Pantalla de carga ocultada");
    }

    public bool IsTransitioning => isTransitioning;
}
