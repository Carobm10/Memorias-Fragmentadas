using UnityEngine;

public class AudioTrigger : MonoBehaviour
{
    [Header("Audio a Reproducir")]
    [Tooltip("Datos del audio que se activará al entrar al trigger.")]
    [SerializeField]
    private AudioClipData audioData;
    
    [Header("Activación")]
    [Tooltip("Si está activo, el audio se dispara cuando el jugador entra en el área.")]
    [SerializeField]
    private bool reproducirAlEntrar = true;
    
    [Tooltip("Si está activo, el audio solo se reproduce una vez.")]
    [SerializeField]
    private bool reproducirUnaVezSolo = true;
    
    [Tooltip("Tag que debe tener el jugador para activar el trigger.")]
    [SerializeField]
    private string tagJugador = "Player";
    
    [Header("Volumen Dinámico")]
    [Tooltip("Si está activo, el volumen baja o sube según la distancia al jugador. Se calcula en unidades de Unity.")]
    [SerializeField]
    private bool usarVolumenDinamico = false;
    
    [Tooltip("Distancia máxima en unidades de Unity usada para ajustar el volumen dinámico.")]
    [SerializeField]
    [Min(0f)]
    private float distanciaMaxima = 20f;

    private AudioScriptManager audioManager;
    private bool yaReproducido = false;
    private Collider triggerCollider;

    void Start()
    {
        audioManager = AudioScriptManager.Instance != null ? AudioScriptManager.Instance : FindFirstObjectByType<AudioScriptManager>();

        if (audioManager == null)
        {
            Debug.LogError($"AudioTrigger en {gameObject.name}: No se encontró AudioScriptManager en la escena.");
            enabled = false;
            return;
        }

        // Configurar el collider como trigger
        triggerCollider = GetComponent<Collider>();
        if (triggerCollider == null)
        {
            Debug.LogError($"AudioTrigger en {gameObject.name}: No hay Collider. Añade uno y marca 'Is Trigger'");
            enabled = false;
            return;
        }

        if (!triggerCollider.isTrigger)
        {
            triggerCollider.isTrigger = true;
            Debug.LogWarning($"AudioTrigger en {gameObject.name}: Collider no era trigger. Se cambió automáticamente.");
        }

        // Resetear estado
        yaReproducido = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(tagJugador) && reproducirAlEntrar)
        {
            if (reproducirUnaVezSolo && yaReproducido)
            {
                return;
            }

            PlayAudio(other.transform);
            yaReproducido = true;
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(tagJugador) && usarVolumenDinamico)
        {
            // El volumen se calcula al disparar el audio, no mientras suena, para evitar cambios bruscos.
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(tagJugador))
        {
            // No se necesita estado extra; el trigger se evalúa al entrar.
        }
    }

    private void PlayAudio(Transform player)
    {
        if (audioData == null || audioData.clip == null)
        {
            Debug.LogWarning($"AudioTrigger en {gameObject.name}: AudioClipData o clip nulo");
            return;
        }

        float volumenMultiplicador = 1f;
        if (usarVolumenDinamico && player != null)
        {
            float distancia = Vector3.Distance(transform.position, player.position);
            float distanciaClampeada = Mathf.Max(0.01f, distanciaMaxima);
            volumenMultiplicador = Mathf.Clamp01(1f - (distancia / distanciaClampeada));
        }

        audioManager.PlayAudioAtPosition(audioData, transform.position, volumenMultiplicador);
        Debug.Log($"AudioTrigger: Reproduciendo '{audioData.nombre}'");
    }

    /// <summary>
    /// Reinicia el trigger para poder reproducir el audio nuevamente
    /// </summary>
    public void ResetTrigger()
    {
        yaReproducido = false;
        Debug.Log($"AudioTrigger en {gameObject.name}: Reiniciado");
    }

    /// <summary>
    /// Configura el audio para este trigger
    /// </summary>
    public void SetAudioData(AudioClipData newAudioData)
    {
        audioData = newAudioData;
    }

    /// <summary>
    /// Detiene la reproducción del audio
    /// </summary>
    public void StopAudio()
    {
        if (audioManager != null)
        {
            audioManager.StopAudio();
        }
    }
}
