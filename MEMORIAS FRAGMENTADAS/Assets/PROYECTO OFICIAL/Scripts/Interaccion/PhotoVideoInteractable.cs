using UnityEngine;
using TMPro;
using UnityEngine.Video;

public class PhotoVideoInteractable : MonoBehaviour
{
    [Header("Foto")]
    public GameObject fotoQuad;

    [Header("Video")]
    public GameObject videoQuad;
    public VideoPlayer videoPlayer;

    [Header("Prompt")]
    public GameObject promptPanel;
    public TMP_Text promptText;

    [Header("Textos")]
    public string textoInteractuar = "Presiona B para ver el recuerdo";
    public string textoCerrar = "Presiona X para cerrar";

    [Header("Highlight")]
    public Renderer rendererFoto;

    public Color colorNormal = Color.white;
    public Color colorMirando = new Color(0.1f, 1f, 0.25f, 1f);

    private bool mirando = false;
    private bool videoActivo = false;

    void Start()
    {
        if (fotoQuad != null)
            fotoQuad.SetActive(true);

        if (videoQuad != null)
            videoQuad.SetActive(false);

        if (promptPanel != null)
            promptPanel.SetActive(false);

        if (videoPlayer != null)
        {
            videoPlayer.playOnAwake = false;
            videoPlayer.isLooping = false;

            videoPlayer.loopPointReached += VideoTerminado;
        }

        Pintar(colorNormal);
    }

    void Update()
    {
        // =========================
        // MIRANDO FOTO
        // =========================

        if (mirando && !videoActivo)
        {
            MostrarPrompt(textoInteractuar);

            if (InputManagerCustom.PressB())
            {
                AbrirVideo();
            }
        }

        // =========================
        // VIDEO ABIERTO
        // =========================

        if (videoActivo)
        {
            MostrarPrompt(textoCerrar);

            if (InputManagerCustom.PressX())
            {
                CerrarVideo();
            }
        }
    }

    // ==================================================
    // MIRAR FOTO
    // ==================================================

    public void MirarFoto()
    {
        mirando = true;

        if (!videoActivo)
        {
            Pintar(colorMirando);
            MostrarPrompt(textoInteractuar);
        }
    }

    // ==================================================
    // DEJAR DE MIRAR
    // ==================================================

    public void DejarMirarFoto()
    {
        mirando = false;

        if (!videoActivo)
        {
            Pintar(colorNormal);
            OcultarPrompt();
        }
    }

    // ==================================================
    // ABRIR VIDEO
    // ==================================================

    private void AbrirVideo()
    {
        videoActivo = true;

        Pintar(colorNormal);

        if (fotoQuad != null)
            fotoQuad.SetActive(false);

        if (videoQuad != null)
            videoQuad.SetActive(true);

        if (videoPlayer != null)
        {
            videoPlayer.Stop();
            videoPlayer.frame = 0;
            videoPlayer.time = 0;
            videoPlayer.Play();
        }

        MostrarPrompt(textoCerrar);
    }

    // ==================================================
    // CERRAR VIDEO
    // ==================================================

    private void CerrarVideo()
    {
        videoActivo = false;

        if (videoPlayer != null)
        {
            videoPlayer.Stop();
            videoPlayer.frame = 0;
            videoPlayer.time = 0;
        }

        if (videoQuad != null)
            videoQuad.SetActive(false);

        if (fotoQuad != null)
            fotoQuad.SetActive(true);

        // VOLVER AL ESTADO CORRECTO

        if (mirando)
        {
            Pintar(colorMirando);
            MostrarPrompt(textoInteractuar);
        }
        else
        {
            Pintar(colorNormal);
            OcultarPrompt();
        }
    }

    // ==================================================
    // VIDEO TERMINADO AUTOMÁTICAMENTE
    // ==================================================

    private void VideoTerminado(VideoPlayer vp)
    {
        CerrarVideo();
    }

    // ==================================================
    // PROMPTS
    // ==================================================

    private void MostrarPrompt(string texto)
    {
        if (promptPanel != null)
            promptPanel.SetActive(true);

        if (promptText != null)
            promptText.text = texto;
    }

    private void OcultarPrompt()
    {
        if (promptPanel != null)
            promptPanel.SetActive(false);
    }

    // ==================================================
    // HIGHLIGHT
    // ==================================================

    private void Pintar(Color color)
    {
        if (rendererFoto != null)
            rendererFoto.material.color = color;
    }
}