using System.Collections;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class EfectoME : MonoBehaviour
{
    public float velocidad = 0.05f;

    [Header("Sonido")]
    public AudioClip sonidoTecla;
    public float volumen = 0.5f;

    private TextMeshPro texto;
    private string textoCompleto;
    private AudioSource audioSource;

    void Start()
    {
        texto = GetComponent<TextMeshPro>();
        audioSource = GetComponent<AudioSource>();

        textoCompleto = texto.text;
        texto.text = "";

        StartCoroutine(EscribirTexto());
    }

    IEnumerator EscribirTexto()
    {
        for (int i = 0; i <= textoCompleto.Length; i++)
        {
            texto.text = textoCompleto.Substring(0, i);

            // 🔊 SOLO reproduce si no hay otro sonido activo
            if (i > 0 && sonidoTecla != null && !audioSource.isPlaying)
            {
                audioSource.clip = sonidoTecla;
                audioSource.volume = volumen;
                audioSource.Play();
            }

            yield return new WaitForSeconds(velocidad);
        }
    }
}