using UnityEngine;

public class RadioMusicController : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource audioSource;

    [Header("Canciones / emisoras")]
    public AudioClip[] canciones;

    [Header("Estática")]
    public AudioClip sonidoEstatica;

    [Header("Volumen")]
    [Range(0f, 1f)]
    public float volumenActual = 0.6f;

    private int emisoraActual = -1;

    void Start()
    {
        if (audioSource != null)
        {
            audioSource.volume = volumenActual;
            audioSource.loop = true;
        }
    }

    public void SiguienteEmisora()
    {
        if (audioSource == null)
        {
            Debug.LogError("RADIO MUSIC: No hay AudioSource asignado.");
            return;
        }

        if (canciones == null || canciones.Length == 0)
        {
            Debug.LogWarning("RADIO MUSIC: No hay canciones asignadas.");
            return;
        }

        emisoraActual++;

        if (emisoraActual >= canciones.Length)
            emisoraActual = 0;

        if (sonidoEstatica != null)
        {
            audioSource.loop = false;
            audioSource.clip = sonidoEstatica;
            audioSource.Play();

            Invoke(nameof(ReproducirCancionActual), 0.7f);
        }
        else
        {
            ReproducirCancionActual();
        }

        Debug.Log("RADIO MUSIC: Cambiando a emisora " + (emisoraActual + 1));
    }

    void ReproducirCancionActual()
    {
        if (audioSource == null) return;
        if (canciones == null || canciones.Length == 0) return;

        audioSource.clip = canciones[emisoraActual];
        audioSource.loop = true;
        audioSource.volume = volumenActual;
        audioSource.Play();

        if (canciones[emisoraActual] != null)
        Debug.Log("RADIO MUSIC: Reproduciendo " + canciones[emisoraActual].name);
    else
        Debug.LogWarning("RADIO MUSIC: La emisora " + (emisoraActual + 1) + " no tiene canción asignada.");
    }

    public void SubirVolumen()
    {
        volumenActual += 0.1f;
        volumenActual = Mathf.Clamp01(volumenActual);

        if (audioSource != null)
            audioSource.volume = volumenActual;

        Debug.Log("RADIO MUSIC: Volumen subió a " + volumenActual);
    }

    public void BajarVolumen()
    {
        volumenActual -= 0.1f;
        volumenActual = Mathf.Clamp01(volumenActual);

        if (audioSource != null)
            audioSource.volume = volumenActual;

        Debug.Log("RADIO MUSIC: Volumen bajó a " + volumenActual);
    }
}