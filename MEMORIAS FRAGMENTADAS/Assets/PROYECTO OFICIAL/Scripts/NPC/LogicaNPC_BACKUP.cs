using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LogicaNPC_BACKUP : MonoBehaviour
{
    [Header("Paneles")]
    public GameObject panelInteraccionNPC;
    public GameObject panelDialogoNPC;

    [Header("Botón salir")]
    public Button botonSalirDialogo;

    [Header("Textos")]
    public TMP_Text textoDialogo;
    public TMP_Text textoOpcion1;
    public TMP_Text textoOpcion2;
    public TMP_Text textoOpcion3;

    [Header("Botones de opciones")]
    public Button botonOpcion1;
    public Button botonOpcion2;
    public Button botonOpcion3;

    [Header("Audio")]
    public AudioSource fuenteAudio;
    public AudioClip audioInicio;
    public AudioClip audioRamaA;
    public AudioClip audioRamaA1;
    public AudioClip audioRamaA2;
    public AudioClip audioRamaB;
    public AudioClip audioRamaC;
    public AudioClip audioRamaC1;
    public AudioClip audioRamaC2;
    public AudioClip audioCierre;

    [Header("Detección por distancia")]
    public Transform player;
    public float distanciaParaHablar = 2f;

    [Header("Detección por mirada")]
    public bool jugadorCerca = false;
    public bool jugadorMirando = false;

    [Header("Configuración")]
    public float velocidadEscritura = 0.03f;

    private bool dialogoActivo = false;
    private bool escribiendo = false;
    private Coroutine rutinaEscritura;

    private enum EstadoDialogo
    {
        Inicio,
        RamaA,
        RamaA1,
        Cierre
    }

    private EstadoDialogo estadoActual;

    void Awake()
    {
        jugadorCerca = false;
        jugadorMirando = false;
        dialogoActivo = false;
        escribiendo = false;

        if (panelInteraccionNPC != null)
            panelInteraccionNPC.SetActive(false);

        if (panelDialogoNPC != null)
            panelDialogoNPC.SetActive(false);

        if (botonSalirDialogo != null)
            botonSalirDialogo.gameObject.SetActive(false);

        OcultarBotones();
    }

    void Start()
    {
        if (botonOpcion1 != null)
            botonOpcion1.onClick.AddListener(OpcionPrincipal);

        if (botonSalirDialogo != null)
            botonSalirDialogo.onClick.AddListener(CerrarDialogo);
    }

    void Update()
    {
        ActualizarCercaniaPorDistancia();

        bool puedeHablar = jugadorCerca && jugadorMirando && !dialogoActivo;

        if (panelInteraccionNPC != null)
            panelInteraccionNPC.SetActive(puedeHablar);

        // 🔹 INICIAR DIÁLOGO → BOTÓN A
        if (puedeHablar && InputManagerCustom.PressA())
        {
            IniciarDialogo();
            return;
        }

        // 🔹 AVANZAR DIÁLOGO PRINCIPAL → BOTÓN A
        if (dialogoActivo && !escribiendo && InputManagerCustom.PressA())
        {
            OpcionPrincipal();
            return;
        }

        // 🔹 SALIR DEL DIÁLOGO → BOTÓN X
        if (dialogoActivo && InputManagerCustom.PressX())
        {
            Debug.Log("Cerrando diálogo con X");
            CerrarDialogo();
            return;
        }
    }

    void ActualizarCercaniaPorDistancia()
    {
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

            if (playerObj != null)
                player = playerObj.transform;
        }

        if (player == null)
        {
            jugadorCerca = false;
            return;
        }

        float distancia = Vector3.Distance(transform.position, player.position);
        jugadorCerca = distancia <= distanciaParaHablar;
    }

    public void SetMirandoNPC(bool mirando)
    {
        jugadorMirando = mirando;
    }

    private void IniciarDialogo()
    {
        dialogoActivo = true;

        if (panelInteraccionNPC != null)
            panelInteraccionNPC.SetActive(false);

        if (panelDialogoNPC != null)
            panelDialogoNPC.SetActive(true);

        if (botonSalirDialogo != null)
            botonSalirDialogo.gameObject.SetActive(true);

        estadoActual = EstadoDialogo.Inicio;
        MostrarEstadoActual();
    }

    private void MostrarEstadoActual()
    {
        OcultarBotones();

        switch (estadoActual)
        {
            case EstadoDialogo.Inicio:
                ReproducirAudioDialogo(audioInicio);
                IniciarEscritura(
                    "¡Ey! ¿Quieres jugar conmigo?\nEstoy armando algo… pero me falta una pieza.\n\n¿Me ayudas?",
                    () =>
                    {
                        textoOpcion1.text = "Sí, quiero ayudarte  X";
                        botonOpcion1.gameObject.SetActive(true);
                    }
                );
                break;

            case EstadoDialogo.RamaA:
                ReproducirAudioDialogo(audioRamaA);
                IniciarEscritura(
                    "¡Estoy armando una casita!\nPero siempre se me cae…\n\nTú eres mejor que yo… ¿cierto?",
                    () =>
                    {
                        textoOpcion1.text = "Déjame ayudarte  X";
                        botonOpcion1.gameObject.SetActive(true);
                    }
                );
                break;

            case EstadoDialogo.RamaA1:
                ReproducirAudioDialogo(audioRamaA1);
                IniciarEscritura(
                    "¡Sabía que sí!\nCuando seas grande, seguro vas a hacer cosas increíbles.\n\nVe que mamá te está llamando.",
                    () =>
                    {
                        textoOpcion1.text = "Continuar  X";
                        botonOpcion1.gameObject.SetActive(true);
                    }
                );
                break;

            case EstadoDialogo.Cierre:
                ReproducirAudioDialogo(audioCierre);
                IniciarEscritura(
                    "Bueno… ve pues.\nPero vuelve a jugar conmigo.",
                    () =>
                    {
                        CerrarDialogo();
                    }
                );
                break;
        }
    }

    private void OpcionPrincipal()
    {
        if (estadoActual == EstadoDialogo.Inicio)
            estadoActual = EstadoDialogo.RamaA;
        else if (estadoActual == EstadoDialogo.RamaA)
            estadoActual = EstadoDialogo.RamaA1;
        else
            estadoActual = EstadoDialogo.Cierre;

        MostrarEstadoActual();
    }

    private void OcultarBotones()
    {
        if (botonOpcion1 != null)
            botonOpcion1.gameObject.SetActive(false);

        if (botonOpcion2 != null)
            botonOpcion2.gameObject.SetActive(false);

        if (botonOpcion3 != null)
            botonOpcion3.gameObject.SetActive(false);

        if (textoOpcion2 != null)
            textoOpcion2.text = "";

        if (textoOpcion3 != null)
            textoOpcion3.text = "";
    }

    private void IniciarEscritura(string texto, System.Action alTerminar)
    {
        if (rutinaEscritura != null)
            StopCoroutine(rutinaEscritura);

        rutinaEscritura = StartCoroutine(EscribirTexto(texto, alTerminar));
    }

    private IEnumerator EscribirTexto(string texto, System.Action alTerminar)
    {
        escribiendo = true;

        if (textoDialogo != null)
            textoDialogo.text = "";

        foreach (char letra in texto)
        {
            if (textoDialogo != null)
                textoDialogo.text += letra;

            yield return new WaitForSeconds(velocidadEscritura);
        }

        escribiendo = false;
        alTerminar?.Invoke();
    }

    private void CerrarDialogo()
    {
        dialogoActivo = false;
        escribiendo = false;

        if (rutinaEscritura != null)
            StopCoroutine(rutinaEscritura);

        if (fuenteAudio != null)
            fuenteAudio.Stop();

        if (panelDialogoNPC != null)
            panelDialogoNPC.SetActive(false);

        if (botonSalirDialogo != null)
            botonSalirDialogo.gameObject.SetActive(false);

        OcultarBotones();
    }

    private void ReproducirAudioDialogo(AudioClip clip)
    {
        if (fuenteAudio == null || clip == null)
            return;

        fuenteAudio.Stop();
        fuenteAudio.clip = clip;
        fuenteAudio.Play();
    }
}