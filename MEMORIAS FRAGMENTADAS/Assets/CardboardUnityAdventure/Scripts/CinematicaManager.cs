using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class CinematicaManager : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public GameObject pantallaVideo;
    public GameObject pantallaInstrucciones;

    void Start()
    {
        pantallaVideo.SetActive(true);
        pantallaInstrucciones.SetActive(false);

        videoPlayer.loopPointReached += AlTerminarVideo;
    }

    void AlTerminarVideo(VideoPlayer vp)
    {
        pantallaVideo.SetActive(false);
        pantallaInstrucciones.SetActive(true);
    }

    public void RepetirVideo()
    {
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