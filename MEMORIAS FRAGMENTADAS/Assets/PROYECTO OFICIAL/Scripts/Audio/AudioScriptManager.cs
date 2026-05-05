using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioScriptManager : MonoBehaviour
{
    private class RuntimeAudioBinding
    {
        public AudioSource source;
        public AudioClipData audioData;
        public Vector3 fallbackPosition;
        public float volumenMultiplicador;
        public float volumenBase;
        public float duracionTotal;
        public float tiempoInicio;
    }

    public static AudioScriptManager Instance { get; private set; }

    [Header("Guion por Tiempo (segundos)")]
    [Tooltip("Lista de audios que se activan automáticamente según el tiempo transcurrido en la escena.")]
    [SerializeField]
    private List<AudioClipData> audiosScheduled = new List<AudioClipData>();

    [Header("Reproducción Central")]
    [Tooltip("Si está activo, los sonidos 3D usan la posición indicada al reproducirse.")]
    [SerializeField]
    private bool usarPosicionDeReproduccion = true;

    [Tooltip("Empty/Transform por defecto para origen espacial de audios 3D cuando el clip no tenga punto personalizado.")]
    [SerializeField]
    private Transform puntoEmision3DPorDefecto;

    [Header("Visualización Gizmo")]
    [Tooltip("Si está activo, el gizmo usa el centro visual del objeto (Renderers/Colliders) en lugar del pivot, cuando no hay punto de emisión asignado.")]
    [SerializeField]
    private bool usarCentroVisualParaGizmos = true;

    private float tiempoTranscurrido = 0f;
    private bool escenaIniciada = false;
    private readonly List<AudioSource> fuentesActivas = new List<AudioSource>();
    private readonly List<RuntimeAudioBinding> audiosRuntime = new List<RuntimeAudioBinding>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    void Start()
    {
        escenaIniciada = true;
        tiempoTranscurrido = 0f;

        // Resetear todos los audios
        foreach (var audio in audiosScheduled)
        {
            audio.yaReproducido = false;
        }

        // Reproducir audios configurados para reproducir al inicio
        foreach (var audio in audiosScheduled)
        {
            if (audio.reproducirAlInicio && audio.clip != null)
            {
                PlayAudio(audio);
                audio.yaReproducido = true;
            }
        }
    }

    void Update()
    {
        if (!escenaIniciada)
            return;

        tiempoTranscurrido += Time.deltaTime;

        // Revisar si algún audio debe reproducirse
        foreach (var audio in audiosScheduled)
        {
            if (!audio.yaReproducido && audio.clip != null && tiempoTranscurrido >= audio.delay)
            {
                PlayAudio(audio);
                audio.yaReproducido = true;
            }
        }

        ActualizarAudiosActivosEnTiempoReal();
    }

    /// <summary>
    /// Reproduce un audio con los parámetros configurados
    /// </summary>
    public void PlayAudio(AudioClipData audioData)
    {
        PlayAudioAtPosition(audioData, transform.position, 1f);
    }

    /// <summary>
    /// Reproduce un audio en una posición concreta con un multiplicador de volumen.
    /// </summary>
    public void PlayAudioAtPosition(AudioClipData audioData, Vector3 position, float volumenMultiplicador = 1f)
    {
        if (audioData == null || audioData.clip == null)
        {
            Debug.LogWarning("AudioScriptManager: Intento de reproducir audio nulo");
            return;
        }

        Vector3 posicionFinal = ResolverPosicionReproduccion(audioData, position);
        AudioSource runtimeSource = CrearFuenteTemporal(audioData, posicionFinal, volumenMultiplicador);
        IniciarReproduccion(audioData, runtimeSource);

        Debug.Log($"Reproduciendo audio: {audioData.nombre}");
    }

    private AudioSource CrearFuenteTemporal(AudioClipData audioData, Vector3 position, float volumenMultiplicador)
    {
        GameObject audioObject = new GameObject($"AudioRuntime_{audioData.nombre}");
        audioObject.transform.SetParent(transform);
        audioObject.transform.position = usarPosicionDeReproduccion ? position : transform.position;

        AudioSource runtimeSource = audioObject.AddComponent<AudioSource>();
        runtimeSource.playOnAwake = false;
        runtimeSource.clip = audioData.clip;
        float volumenBase = Mathf.Clamp01(audioData.volumen * Mathf.Clamp01(volumenMultiplicador));
        runtimeSource.volume = volumenBase;
        runtimeSource.pitch = audioData.pitch;
        runtimeSource.loop = audioData.loop;
        runtimeSource.spatialBlend = audioData.usar3D ? 1f : 0f;

        if (audioData.usar3D)
        {
            ConfigurarAtenuacion3D(runtimeSource, audioData);
        }

        fuentesActivas.Add(runtimeSource);
        audiosRuntime.Add(new RuntimeAudioBinding
        {
            source = runtimeSource,
            audioData = audioData,
            fallbackPosition = position,
            volumenMultiplicador = Mathf.Clamp01(volumenMultiplicador),
            volumenBase = volumenBase,
            duracionTotal = CalcularDuracionTotal(audioData),
            tiempoInicio = Time.time
        });

        return runtimeSource;
    }

    private void ConfigurarAtenuacion3D(AudioSource source, AudioClipData audioData)
    {
        float maxDistance = Mathf.Max(0.1f, audioData.distanciaMaxima);
        float intensidad = Mathf.Clamp(audioData.intensidadCercania, 0.25f, 4f);
        float fadeOutZona = Mathf.Clamp(audioData.fadeOutZona, 0.05f, 0.5f);

        source.rolloffMode = AudioRolloffMode.Custom;
        source.minDistance = 0.01f;
        source.maxDistance = maxDistance;

        AnimationCurve curve = new AnimationCurve();
        const int samples = 48;
        for (int i = 0; i <= samples; i++)
        {
            float t = i / (float)samples;
            float proximidad = Mathf.Clamp01(1f - t);
            float value = Mathf.Pow(proximidad, intensidad * 1.4f);

            if (fadeOutZona > 0f)
            {
                float inicioFade = 1f - fadeOutZona;
                if (t >= inicioFade)
                {
                    float fadeT = Mathf.InverseLerp(inicioFade, 1f, t);
                    float smoothFade = Mathf.SmoothStep(1f, 0f, fadeT);
                    value *= smoothFade;
                }
            }

            value = Mathf.SmoothStep(0f, 1f, value);
            curve.AddKey(new Keyframe(t, value));
        }

        curve.AddKey(new Keyframe(0f, 1f));
        curve.AddKey(new Keyframe(1f, 0f));

        source.SetCustomCurve(AudioSourceCurveType.CustomRolloff, curve);
    }

    private void ActualizarAudiosActivosEnTiempoReal()
    {
        for (int i = audiosRuntime.Count - 1; i >= 0; i--)
        {
            RuntimeAudioBinding binding = audiosRuntime[i];

            if (binding == null || binding.source == null)
            {
                audiosRuntime.RemoveAt(i);
                continue;
            }

            if (binding.audioData == null)
            {
                continue;
            }

            float volumenBase = Mathf.Clamp01(binding.audioData.volumen * binding.volumenMultiplicador);
            if (binding.audioData.animarVolumen)
            {
                float duracion = Mathf.Max(0.01f, binding.duracionTotal);
                float tiempoNormalizado = Mathf.Clamp01((Time.time - binding.tiempoInicio) / duracion);
                float factorCurva = binding.audioData.curvaVolumen != null
                    ? binding.audioData.curvaVolumen.Evaluate(tiempoNormalizado)
                    : tiempoNormalizado;
                binding.source.volume = Mathf.Clamp01(volumenBase * Mathf.Max(0f, factorCurva));
            }
            else
            {
                binding.source.volume = volumenBase;
            }

            if (binding.audioData.usar3D)
            {
                float factorDistancia = CalcularFactorDistancia3D(binding);
                binding.source.volume = Mathf.Clamp01(binding.source.volume * factorDistancia);
            }

            binding.source.pitch = binding.audioData.pitch;
            binding.source.spatialBlend = binding.audioData.usar3D ? 1f : 0f;
            binding.source.loop = binding.audioData.repeticiones > 1 && !binding.audioData.loop;

            Vector3 posicionFinal = ResolverPosicionReproduccion(binding.audioData, binding.fallbackPosition);
            binding.source.transform.position = posicionFinal;

            if (binding.audioData.usar3D)
            {
                ConfigurarAtenuacion3D(binding.source, binding.audioData);
            }
        }
    }

    private void IniciarReproduccion(AudioClipData audioData, AudioSource runtimeSource)
    {
        if (runtimeSource == null || audioData == null || audioData.clip == null)
        {
            return;
        }

        if (audioData.loop)
        {
            runtimeSource.loop = false;
            runtimeSource.Play();
            StartCoroutine(MantenerLoopInfinito(runtimeSource));
            return;
        }

        int repeticiones = Mathf.Max(1, audioData.repeticiones);
        float duracion = CalcularDuracionBase(audioData);
        float duracionTotal = audioData.loop ? duracion : duracion * repeticiones;

        if (repeticiones == 1)
        {
            runtimeSource.loop = false;
            runtimeSource.Play();
            StartCoroutine(DestruirFuenteCuandoTermine(runtimeSource, duracionTotal));
            return;
        }

        runtimeSource.loop = true;
        runtimeSource.Play();
        StartCoroutine(DetenerLoopFinito(runtimeSource, duracionTotal));
    }


    private IEnumerator DetenerLoopFinito(AudioSource source, float duracionTotal)
    {
        yield return new WaitForSeconds(duracionTotal + 0.02f);

        if (source == null)
        {
            yield break;
        }

        source.loop = false;
        source.Stop();
        fuentesActivas.Remove(source);
        RemoverBinding(source);
        Destroy(source.gameObject);
    }

    private IEnumerator MantenerLoopInfinito(AudioSource source)
    {
        if (source == null)
        {
            yield break;
        }

        int limiteSamples = source.clip != null ? Mathf.Max(1, source.clip.samples - 1) : 1;

        while (source != null)
        {
            if (source.clip != null)
            {
                if (!source.isPlaying || source.timeSamples >= limiteSamples)
                {
                    source.timeSamples = 0;
                    source.Play();
                }
            }

            yield return null;
        }
    }

    private float CalcularFactorDistancia3D(RuntimeAudioBinding binding)
    {
        if (binding == null || binding.source == null || binding.audioData == null)
        {
            return 1f;
        }

        AudioListener listener = FindFirstObjectByType<AudioListener>();
        if (listener == null)
        {
            return 1f;
        }

        float maxDistance = Mathf.Max(0.1f, binding.audioData.distanciaMaxima);
        float distancia = Vector3.Distance(binding.source.transform.position, listener.transform.position);
        float proximidad = Mathf.Clamp01(1f - (distancia / maxDistance));
        float intensidad = Mathf.Clamp(binding.audioData.intensidadCercania, 0.25f, 4f);
        float factor = Mathf.Pow(proximidad, 1f / intensidad);

        if (binding.audioData.fadeOutZona > 0f)
        {
            float inicioFade = 1f - Mathf.Clamp(binding.audioData.fadeOutZona, 0.05f, 0.5f);
            float distanciaNormalizada = Mathf.Clamp01(distancia / maxDistance);
            if (distanciaNormalizada >= inicioFade)
            {
                float fadeT = Mathf.InverseLerp(inicioFade, 1f, distanciaNormalizada);
                factor *= Mathf.SmoothStep(1f, 0f, fadeT);
            }
        }

        return factor;
    }

    private Vector3 ResolverPosicionReproduccion(AudioClipData audioData, Vector3 fallbackPosition)
    {
        if (!usarPosicionDeReproduccion)
        {
            return transform.position;
        }

        if (audioData != null && audioData.puntoEmisionPersonalizado != null)
        {
            return audioData.puntoEmisionPersonalizado.position;
        }

        if (puntoEmision3DPorDefecto != null)
        {
            return puntoEmision3DPorDefecto.position;
        }

        return fallbackPosition;
    }

    private void OnDrawGizmosSelected()
    {
        DibujarRangos3D(audiosScheduled);
    }

    private void DibujarRangos3D(List<AudioClipData> audios)
    {
        if (audios == null)
        {
            return;
        }

        for (int i = 0; i < audios.Count; i++)
        {
            AudioClipData audio = audios[i];
            if (audio == null || !audio.usar3D)
            {
                continue;
            }

            Vector3 centro = transform.position;
            if (audio.puntoEmisionPersonalizado != null)
            {
                centro = audio.puntoEmisionPersonalizado.position;
            }
            else if (puntoEmision3DPorDefecto != null)
            {
                centro = puntoEmision3DPorDefecto.position;
            }
            else if (usarCentroVisualParaGizmos)
            {
                centro = ObtenerCentroVisualDelObjeto();
            }

            float max = Mathf.Max(0.1f, audio.distanciaMaxima);

            Gizmos.color = new Color(0.2f, 0.5f, 1f, 0.7f);
            Gizmos.DrawWireSphere(centro, max);

            Gizmos.color = new Color(0.2f, 0.8f, 1f, 0.5f);
            Gizmos.DrawSphere(centro, 0.08f);
        }
    }

    private Vector3 ObtenerCentroVisualDelObjeto()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        if (renderers != null && renderers.Length > 0)
        {
            Bounds bounds = renderers[0].bounds;
            for (int i = 1; i < renderers.Length; i++)
            {
                bounds.Encapsulate(renderers[i].bounds);
            }

            return bounds.center;
        }

        Collider[] colliders = GetComponentsInChildren<Collider>();
        if (colliders != null && colliders.Length > 0)
        {
            Bounds bounds = colliders[0].bounds;
            for (int i = 1; i < colliders.Length; i++)
            {
                bounds.Encapsulate(colliders[i].bounds);
            }

            return bounds.center;
        }

        return transform.position;
    }

    private IEnumerator DestruirFuenteCuandoTermine(AudioSource source, float segundos)
    {
        yield return new WaitForSeconds(segundos + 0.1f);

        if (source == null)
        {
            yield break;
        }

        fuentesActivas.Remove(source);
        RemoverBinding(source);
        Destroy(source.gameObject);
    }

    private void RemoverBinding(AudioSource source)
    {
        for (int i = audiosRuntime.Count - 1; i >= 0; i--)
        {
            RuntimeAudioBinding binding = audiosRuntime[i];
            if (binding != null && binding.source == source)
            {
                audiosRuntime.RemoveAt(i);
            }
        }
    }

    /// <summary>
    /// Reproduce un audio inmediatamente
    /// </summary>
    public void PlayAudioImmediate(AudioClip clip, float volumen = 1f, float pitch = 1f)
    {
        if (clip == null)
            return;

        AudioClipData temporal = new AudioClipData
        {
            clip = clip,
            volumen = volumen,
            pitch = pitch,
            loop = false,
            usar3D = false,
            distanciaMaxima = 20f,
            intensidadCercania = 1f
        };

        PlayAudioAtPosition(temporal, transform.position, 1f);
    }

    /// <summary>
    /// Detiene el audio actual
    /// </summary>
    public void StopAudio()
    {
        StopAllAudio();
    }

    /// <summary>
    /// Pausa el audio
    /// </summary>
    public void PauseAudio()
    {
        foreach (var source in fuentesActivas)
        {
            if (source != null && source.isPlaying)
            {
                source.Pause();
            }
        }
    }

    /// <summary>
    /// Reanuda el audio pausado
    /// </summary>
    public void ResumeAudio()
    {
        foreach (var source in fuentesActivas)
        {
            if (source != null)
            {
                source.UnPause();
            }
        }
    }

    /// <summary>
    /// Reinicia el guion de audios desde el principio
    /// </summary>
    public void ResetScript()
    {
        tiempoTranscurrido = 0f;
        StopAudio();

        foreach (var audio in audiosScheduled)
        {
            audio.yaReproducido = false;
        }

        Debug.Log("AudioScriptManager: Guion reiniciado");
    }

    /// <summary>
    /// Detiene todos los audios que el manager haya creado.
    /// </summary>
    public void StopAllAudio()
    {
        for (int i = fuentesActivas.Count - 1; i >= 0; i--)
        {
            AudioSource source = fuentesActivas[i];
            if (source != null)
            {
                source.Stop();
                Destroy(source.gameObject);
            }
        }

        fuentesActivas.Clear();
        audiosRuntime.Clear();
    }

    /// <summary>
    /// Añade un nuevo audio al guion
    /// </summary>
    public void AddAudio(AudioClipData audioData)
    {
        if (audioData != null)
        {
            audiosScheduled.Add(audioData);
        }
    }

    /// <summary>
    /// Obtiene el tiempo transcurrido
    /// </summary>
    public float GetElapsedTime()
    {
        return tiempoTranscurrido;
    }

    /// <summary>
    /// Obtiene una estimación de la duración total del guion de audio.
    /// La visualización usa la duración base de cada clip en su punto de inicio,
    /// sin expandir repeticiones ni loops para que la línea de tiempo refleje
    /// la secuencia de audios y no el tiempo de reproducción acumulado.
    /// </summary>
    public float GetEstimatedTimelineDuration()
    {
        float maxEnd = 0f;

        for (int i = 0; i < audiosScheduled.Count; i++)
        {
            AudioClipData audio = audiosScheduled[i];
            if (audio == null || audio.clip == null)
            {
                continue;
            }

            float duracion = CalcularDuracionTotal(audio);
            float inicio = audio.reproducirAlInicio ? 0f : Mathf.Max(0f, audio.delay);
            float fin = inicio + duracion;

            if (fin > maxEnd)
            {
                maxEnd = fin;
            }
        }

        return maxEnd;
    }

    private float CalcularDuracionBase(AudioClipData audioData)
    {
        if (audioData == null || audioData.clip == null)
        {
            return 0f;
        }

        float pitch = Mathf.Max(0.01f, Mathf.Abs(audioData.pitch));
        return audioData.clip.length / pitch;
    }

    private float CalcularDuracionTotal(AudioClipData audioData)
    {
        float duracionBase = CalcularDuracionBase(audioData);
        if (audioData == null)
        {
            return duracionBase;
        }

        if (audioData.loop)
        {
            return duracionBase;
        }

        return duracionBase * Mathf.Max(1, audioData.repeticiones);
    }

    /// <summary>
    /// Retorna si un audio está siendo reproducido
    /// </summary>
    public bool IsPlaying()
    {
        for (int i = 0; i < fuentesActivas.Count; i++)
        {
            AudioSource source = fuentesActivas[i];
            if (source != null && source.isPlaying)
            {
                return true;
            }
        }

        return false;
    }
}
