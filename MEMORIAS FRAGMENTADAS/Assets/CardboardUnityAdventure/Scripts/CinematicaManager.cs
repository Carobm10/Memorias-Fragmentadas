using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class CinematicaManager : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public GameObject pantallaVideo;
    public GameObject pantallaInstrucciones;

    private bool mostrandoInstrucciones = false;

    void Start()
    {
        pantallaVideo.SetActive(true);
        pantallaInstrucciones.SetActive(false);

        videoPlayer.loopPointReached += AlTerminarVideo;
    }

    void Update()
    {
        if (mostrandoInstrucciones)
        {
            // Y → repetir video
            if (Input.GetKeyDown(KeyCode.JoystickButton4))
            {
                RepetirVideo();
            }

            // X → continuar
            if (Input.GetKeyDown(KeyCode.JoystickButton3))
            {
                Continuar();
            }
        }
    }

    void AlTerminarVideo(VideoPlayer vp)
    {
        pantallaVideo.SetActive(false);
        pantallaInstrucciones.SetActive(true);

        mostrandoInstrucciones = true;
    }

    public void RepetirVideo()
    {
        mostrandoInstrucciones = false;

        pantallaInstrucciones.SetActive(false);
        pantallaVideo.SetActive(true);

        videoPlayer.Stop();
        videoPlayer.Play();
    }

    public void Continuar()
    {
        SceneManager.LoadScene("Prototipo - Entrega");
    }
}