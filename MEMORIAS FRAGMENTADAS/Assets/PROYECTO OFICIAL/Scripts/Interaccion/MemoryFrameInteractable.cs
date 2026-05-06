using System.Collections;
using UnityEngine;
using UnityEngine.Video;

public class MemoryFrameInteractable : MonoBehaviour
{
    [Header("Video")]
    public GameObject videoQuad;
    public VideoPlayer videoPlayer;

    [Header("Movimiento")]
    public Transform inspectPoint;
    public float moveSpeed = 3f;
    public float rotateSpeed = 3f;

    [Header("Jugador")]
    public MovimientoVR2 movimientoJugador;

    private Vector3 posicionOriginal;
    private Quaternion rotacionOriginal;

    private bool interactuando = false;
    private bool mirando = false;

    void Start()
    {
        posicionOriginal = transform.position;
        rotacionOriginal = transform.rotation;

        if (videoQuad != null)
            videoQuad.SetActive(false);
    }

    void Update()
    {
        // INTERACTUAR
        if (mirando && !interactuando && InputManagerCustom.PressB())
        {
            StartCoroutine(MoverFrenteCamara());
        }

        // CERRAR
        if (interactuando && InputManagerCustom.PressX())
        {
            StartCoroutine(VolverAPosicion());
        }
    }

    public void SetMirando(bool estado)
    {
        mirando = estado;
    }

    IEnumerator MoverFrenteCamara()
    {
        interactuando = true;

        // bloquear movimiento
        if (movimientoJugador != null)
            movimientoJugador.puedeMoverse = false;

        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;

        float t = 0;

        while (t < 1)
        {
            t += Time.deltaTime * moveSpeed;

            transform.position = Vector3.Lerp(
                startPos,
                inspectPoint.position,
                t
            );

            transform.rotation = Quaternion.Slerp(
                startRot,
                inspectPoint.rotation,
                t
            );

            yield return null;
        }

        // ACTIVAR VIDEO
        if (videoQuad != null)
            videoQuad.SetActive(true);

        if (videoPlayer != null)
            videoPlayer.Play();
    }

    IEnumerator VolverAPosicion()
    {
        // apagar video
        if (videoPlayer != null)
            videoPlayer.Stop();

        if (videoQuad != null)
            videoQuad.SetActive(false);

        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;

        float t = 0;

        while (t < 1)
        {
            t += Time.deltaTime * moveSpeed;

            transform.position = Vector3.Lerp(
                startPos,
                posicionOriginal,
                t
            );

            transform.rotation = Quaternion.Slerp(
                startRot,
                rotacionOriginal,
                t
            );

            yield return null;
        }

        // desbloquear movimiento
        if (movimientoJugador != null)
            movimientoJugador.puedeMoverse = true;

        interactuando = false;
    }
}