using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;
using System.Collections;

/// <summary>
/// Gestor centralizado de transiciones entre escenas.
/// </summary>
public class SceneTransitionManager : MonoBehaviour
{
    private static SceneTransitionManager instance;

    [Header("Configuración de Carga")]
    [SerializeField] private bool useAsyncLoading = true;
    [SerializeField] private float minLoadingScreenTime = 1.5f;

    private Canvas currentLoadingCanvas;
    private LoadingScreenAnimator currentLoadingAnimator;
    private bool isTransitioning = false;

    private string[] sceneSequence = { "Menu", "Escena_VideoIntro", "BASE" };

    void Awake()
    {
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

    public void LoadNextScene()
    {
        if (isTransitioning)
        {
            Debug.LogWarning("⚠ Ya hay una transición en proceso.");
            return;
        }

        string currentScene = SceneManager.GetActiveScene().name;
        int currentIndex = System.Array.IndexOf(sceneSequence, currentScene);

        if (currentIndex >= 0 && currentIndex < sceneSequence.Length - 1)
            LoadScene(sceneSequence[currentIndex + 1]);
        else if (currentIndex == sceneSequence.Length - 1)
            LoadScene(sceneSequence[0]);
        else
            LoadScene("Menu");
    }

    public void LoadPreviousScene()
    {
        if (isTransitioning)
        {
            Debug.LogWarning("⚠ Ya hay una transición en proceso.");
            return;
        }

        string currentScene = SceneManager.GetActiveScene().name;
        int currentIndex = System.Array.IndexOf(sceneSequence, currentScene);

        if (currentIndex > 0)
            LoadScene(sceneSequence[currentIndex - 1]);
        else if (currentIndex == 0)
            LoadScene(sceneSequence[sceneSequence.Length - 1]);
        else
            LoadScene("Menu");
    }

    public void LoadScene(string sceneName)
    {
        if (isTransitioning)
        {
            Debug.LogWarning("⚠ Ya hay una transición en proceso.");
            return;
        }

        Debug.Log($"→ Iniciando transición a: {sceneName}");
        StartCoroutine(TransitionToScene(sceneName));
    }

    private IEnumerator TransitionToScene(string sceneName)
    {
        isTransitioning = true;
        Debug.Log($"[TRANSICIÓN] Iniciando: {sceneName}");

        // --- 1. Mostrar pantalla de carga ANTES de cargar la escena ---
        ShowLoadingScreen();

        // Esperar un frame para asegurar que el canvas se renderizó
        yield return null;
        yield return null;

        float loadStartTime = Time.realtimeSinceStartup;

        if (useAsyncLoading)
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
            // No activar la escena todavía: mantener la pantalla de carga visible
            asyncLoad.allowSceneActivation = false;

            // Esperar a que la precarga termine (progress llega a 0.9 = 90%)
            while (asyncLoad.progress < 0.9f)
            {
                yield return null;
            }

            // Esperar el tiempo mínimo de pantalla de carga
            while (Time.realtimeSinceStartup - loadStartTime < minLoadingScreenTime)
            {
                yield return null;
            }

            // Ahora sí activar la escena
            asyncLoad.allowSceneActivation = true;

            // Esperar a que la escena esté completamente activa
            while (!asyncLoad.isDone)
            {
                yield return null;
            }
        }
        else
        {
            while (Time.realtimeSinceStartup - loadStartTime < minLoadingScreenTime)
            {
                yield return null;
            }
            SceneManager.LoadScene(sceneName);
            yield return null;
        }

        // --- 2. La escena ya está cargada y activa ---
        // Esperar dos frames para que Awake/Start de los nuevos objetos se ejecuten
        yield return null;
        yield return null;

        // Ejecutar tareas post-carga en try/catch: un error aquí no debe
        // impedir que la pantalla de carga se destruya.
        try
        {
            EnsureAudioListener();
            EnsureCameraActive();
            SceneDebugNavigator.CreateDebugNavigatorUI();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[TRANSICIÓN] Error en post-carga (no crítico): {e.Message}");
        }

        // --- 3. Ocultar pantalla de carga y finalizar ---
        // Se ejecuta SIEMPRE, incluso si hubo error arriba.
        yield return StartCoroutine(HideLoadingScreenAndFinish());

        Debug.Log($"[TRANSICIÓN] ✓ Completada: {sceneName}");
        isTransitioning = false;
    }

    private void EnsureCameraActive()
    {
        Camera[] cameras = FindObjectsByType<Camera>(FindObjectsSortMode.None);
        bool anyActive = false;

        foreach (Camera cam in cameras)
        {
            if (cam.gameObject.activeSelf)
            {
                anyActive = true;
                break;
            }
        }

        if (!anyActive && cameras.Length > 0)
        {
            cameras[0].gameObject.SetActive(true);
            Debug.Log($"✓ Cámara activada: {cameras[0].gameObject.name}");
        }
    }

    /// <summary>
    /// Solo garantiza que haya un AudioListener en la escena para evitar warnings.
    /// No toca VideoPlayers ni AudioSources: eso es responsabilidad de cada escena.
    /// </summary>
    private void EnsureAudioListener()
    {
        if (FindFirstObjectByType<AudioListener>() == null)
        {
            Camera cam = FindFirstObjectByType<Camera>();
            if (cam != null && cam.GetComponent<AudioListener>() == null)
            {
                cam.gameObject.AddComponent<AudioListener>();
                Debug.Log("✓ AudioListener añadido a la cámara");
            }
        }
    }

    private void ShowLoadingScreen()
    {
        // Destruir pantalla anterior si existe
        if (currentLoadingCanvas != null)
        {
            Destroy(currentLoadingCanvas.gameObject);
            currentLoadingCanvas = null;
        }

        currentLoadingCanvas = SceneDebugNavigator.CreateLoadingScreen();

        if (currentLoadingCanvas != null)
        {
            DontDestroyOnLoad(currentLoadingCanvas.gameObject);
            currentLoadingAnimator = currentLoadingCanvas.GetComponent<LoadingScreenAnimator>();
            Debug.Log("✓ Pantalla de carga mostrada");
        }
        else
        {
            Debug.LogError("✗ No se pudo crear la pantalla de carga");
        }
    }

    /// <summary>
    /// Completa la animación, espera y destruye la pantalla de carga.
    /// Garantizado: el canvas se destruye SIEMPRE, incluso si hay errores previos.
    /// </summary>
    private IEnumerator HideLoadingScreenAndFinish()
    {
        // Completar animación (barra al 100%, texto "¡Listo!")
        try
        {
            if (currentLoadingAnimator != null)
                currentLoadingAnimator.Complete();
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"[CARGA] Error al completar animación: {e.Message}");
        }

        // Breve pausa para que el jugador vea el 100%
        yield return new WaitForSecondsRealtime(0.5f);

        // Destruir el canvas de carga — esto DEBE ocurrir pase lo que pase
        ForceDestroyLoadingCanvas();
    }

    /// <summary>
    /// Destruye el canvas de carga de forma incondicional.
    /// Llamado también desde OnDestroy como seguro extra.
    /// </summary>
    private void ForceDestroyLoadingCanvas()
    {
        if (currentLoadingCanvas != null)
        {
            try   { Destroy(currentLoadingCanvas.gameObject); }
            catch { /* el objeto ya fue destruido externamente */ }
            currentLoadingCanvas = null;
            currentLoadingAnimator = null;
            Debug.Log("✓ Pantalla de carga destruida");
        }
    }

    void OnDestroy()
    {
        // Si el manager se destruye por alguna razón, limpiar el canvas huérfano
        ForceDestroyLoadingCanvas();
    }

    public bool IsTransitioning => isTransitioning;
}