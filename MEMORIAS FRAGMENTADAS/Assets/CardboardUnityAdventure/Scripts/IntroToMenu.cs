using UnityEngine;
using UnityEngine.Video;
using System.Collections;

public class IntroToMenu : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public GameObject pantallaVideo;
    public GameObject pantallaMenu;

    IEnumerator Start()
    {
        pantallaVideo.SetActive(true);
        pantallaMenu.SetActive(false);

        videoPlayer.playOnAwake = false;
        videoPlayer.isLooping = false;

        videoPlayer.loopPointReached += AlTerminarVideo;

        videoPlayer.Prepare();

        while (!videoPlayer.isPrepared)
        {
            yield return null;
        }

        videoPlayer.Play();
    }

    void AlTerminarVideo(VideoPlayer vp)
    {
        Debug.Log("El video terminó");

        pantallaVideo.SetActive(false);
        pantallaMenu.SetActive(true);
    }
}