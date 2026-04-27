using UnityEngine;

/// <summary>
/// Script de ejemplo que muestra cómo utilizar el sistema de audio
/// Puedes adaptarlo según necesites
/// </summary>
public class AudioControlExample : MonoBehaviour
{
    [SerializeField]
    private AudioScriptManager audioScriptManager;

    void Start()
    {
        // Obtener referencias si no están asignadas
        if (audioScriptManager == null)
            audioScriptManager = FindFirstObjectByType<AudioScriptManager>();
    }

    void Update()
    {
        // PRUEBAS: Presiona teclas para probar el sistema
        
        // Tecla 1: Reproducir audio inmediatamente
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log("Tecla 1 presionada: Reproducir audio ejemplo");
            // audioScriptManager.PlayAudioImmediate(tuAudioClip, volumen: 0.8f);
        }

        // Tecla 2: Pausar
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Debug.Log("Tecla 2 presionada: Pausar audio");
            audioScriptManager.PauseAudio();
        }

        // Tecla 3: Reanudar
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Debug.Log("Tecla 3 presionada: Reanudar audio");
            audioScriptManager.ResumeAudio();
        }

        // Tecla 4: Detener
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            Debug.Log("Tecla 4 presionada: Detener audio");
            audioScriptManager.StopAudio();
        }

        // Tecla 5: Reiniciar guion
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            Debug.Log("Tecla 5 presionada: Reiniciar guion");
            audioScriptManager.ResetScript();
        }

        // Mostrar info en consola
        if (Input.GetKeyDown(KeyCode.I))
        {
            Debug.Log($"--- AUDIO INFO ---");
            Debug.Log($"Tiempo transcurrido: {audioScriptManager.GetElapsedTime():F2}s");
            Debug.Log($"¿Reproduciendo?: {audioScriptManager.IsPlaying()}");
        }
    }

    /// <summary>
    /// Ejemplo: Reproducir audio cuando se detecta un evento
    /// </summary>
    public void OnEventoJuego()
    {
        Debug.Log("Evento detectado - Reproduciendo audio");
        audioScriptManager.ResetScript();
        // El manager reproducirá los audios automáticamente según config
    }
}
