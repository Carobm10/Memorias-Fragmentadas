using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LogicaNPC : MonoBehaviour
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
        RamaA2,
        RamaB,
        RamaC,
        RamaC1,
        RamaC2,
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
    }

    void Start()
    {
        if (botonOpcion1 != null)
            botonOpcion1.onClick.AddListener(Opcion1);

        if (botonOpcion2 != null)
            botonOpcion2.onClick.AddListener(Opcion2);

        if (botonOpcion3 != null)
            botonOpcion3.onClick.AddListener(Opcion3);

        if (botonSalirDialogo != null)
            botonSalirDialogo.onClick.AddListener(CerrarDialogo);
    }

    void Update()
    {
        ActualizarCercaniaPorDistancia();

        bool puedeHablar = jugadorCerca && jugadorMirando && !dialogoActivo;

        if (panelInteraccionNPC != null)
            panelInteraccionNPC.SetActive(puedeHablar);

        // 🔹 HABLAR con Y
        if (puedeHablar && InputManagerCustom.PressY())
        {
            IniciarDialogo();
            return;
        }

        // 🔹 SALIR con X
        if (dialogoActivo && InputManagerCustom.PressX())
        {
            CerrarDialogo();
            return;
        }

        if (dialogoActivo && !escribiendo)
        {
            if (botonOpcion1 != null && botonOpcion1.gameObject.activeSelf && InputManagerCustom.PressY())
                Opcion1();

            if (botonOpcion2 != null && botonOpcion2.gameObject.activeSelf && InputManagerCustom.PressA())
                Opcion2();

            if (botonOpcion3 != null && botonOpcion3.gameObject.activeSelf && InputManagerCustom.PressB())
                Opcion3();
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
                    "¡Ey! ¿Quieres jugar conmigo?\nEstoy armando algo… pero me falta una pieza.\n\n¿Me ayudas o estás ocupado?",
                    () =>
                    {
                        textoOpcion1.text = "Sí, ¿a qué estás jugando?  Y";
                        textoOpcion2.text = "No puedo ahora, tengo que alistarme.  A";
                        textoOpcion3.text = "¿Qué estás haciendo?  B";

                        botonOpcion1.gameObject.SetActive(true);
                        botonOpcion2.gameObject.SetActive(true);
                        botonOpcion3.gameObject.SetActive(true);
                    }
                );
                break;

            case EstadoDialogo.RamaA:
                ReproducirAudioDialogo(audioRamaA);
                IniciarEscritura(
                    "¡Estoy armando una casita!\nPero siempre se me cae…\n\nTú eres mejor que yo… ¿cierto?",
                    () =>
                    {
                        textoOpcion1.text = "Déjame ayudarte.  Y";
                        textoOpcion2.text = "Está bien así.  A";
                        textoOpcion3.text = "";

                        botonOpcion1.gameObject.SetActive(true);
                        botonOpcion2.gameObject.SetActive(true);
                        botonOpcion3.gameObject.SetActive(false);
                    }
                );
                break;

            case EstadoDialogo.RamaA1:
                ReproducirAudioDialogo(audioRamaA1);
                IniciarEscritura(
                    "¡Sabía que sí!\nCuando seas grande, seguro vas a hacer cosas increíbles.",
                    MostrarSoloBotonContinuar
                );
                break;

            case EstadoDialogo.RamaA2:
                ReproducirAudioDialogo(audioRamaA2);
                IniciarEscritura(
                    "Bueno… igual me gusta como queda.\nNo todo tiene que ser perfecto.",
                    MostrarSoloBotonContinuar
                );
                break;

            case EstadoDialogo.RamaB:
                ReproducirAudioDialogo(audioRamaB);
                IniciarEscritura(
                    "Ahh…\nSiempre estás ocupado…\n\nBueno… pero no te demores mucho.\nLuego jugamos, ¿sí?",
                    MostrarSoloBotonContinuar
                );
                break;

            case EstadoDialogo.RamaC:
                ReproducirAudioDialogo(audioRamaC);
                IniciarEscritura(
                    "Es una casa…\nPara que vivamos todos…\n\nAsí no se nos olvida nada.\n¿Tú crees que las cosas se olvidan?",
                    () =>
                    {
                        textoOpcion1.text = "A veces…  Y";
                        textoOpcion2.text = "No, si las recuerdas.  A";
                        textoOpcion3.text = "";

                        botonOpcion1.gameObject.SetActive(true);
                        botonOpcion2.gameObject.SetActive(true);
                        botonOpcion3.gameObject.SetActive(false);
                    }
                );
                break;

            case EstadoDialogo.RamaC1:
                ReproducirAudioDialogo(audioRamaC1);
                IniciarEscritura(
                    "Sí… a veces las cosas se van poquito a poquito.",
                    MostrarSoloBotonContinuar
                );
                break;

            case EstadoDialogo.RamaC2:
                ReproducirAudioDialogo(audioRamaC2);
                IniciarEscritura(
                    "Entonces acuérdate de mí siempre, ¿sí?",
                    MostrarSoloBotonContinuar
                );
                break;

            case EstadoDialogo.Cierre:
                ReproducirAudioDialogo(audioCierre);
                IniciarEscritura(
                    "Bueno… ve pues.\nPero vuelve a jugar conmigo.",
                    MostrarSoloBotonCerrar
                );
                break;
        }
    }

    private void Opcion1()
    {
        if (estadoActual == EstadoDialogo.Inicio)
            estadoActual = EstadoDialogo.RamaA;
        else if (estadoActual == EstadoDialogo.RamaA)
            estadoActual = EstadoDialogo.RamaA1;
        else if (estadoActual == EstadoDialogo.RamaC)
            estadoActual = EstadoDialogo.RamaC1;
        else
            estadoActual = EstadoDialogo.Cierre;

        MostrarEstadoActual();
    }

    private void Opcion2()
    {
        if (estadoActual == EstadoDialogo.Inicio)
            estadoActual = EstadoDialogo.RamaB;
        else if (estadoActual == EstadoDialogo.RamaA)
            estadoActual = EstadoDialogo.RamaA2;
        else if (estadoActual == EstadoDialogo.RamaC)
            estadoActual = EstadoDialogo.RamaC2;
        else
            estadoActual = EstadoDialogo.Cierre;

        MostrarEstadoActual();
    }

    private void Opcion3()
    {
        if (estadoActual == EstadoDialogo.Inicio)
            estadoActual = EstadoDialogo.RamaC;
        else
            estadoActual = EstadoDialogo.Cierre;

        MostrarEstadoActual();
    }

    private void MostrarSoloBotonContinuar()
    {
        OcultarBotones();

        textoOpcion1.text = "Continuar  Y";
        botonOpcion1.gameObject.SetActive(true);
    }

    private void MostrarSoloBotonCerrar()
    {
        OcultarBotones();

        // Ya no mostramos opción "Cerrar".
        // El jugador debe usar el botón visual "Salir X".
    }

    private void OcultarBotones()
    {
        if (botonOpcion1 != null)
            botonOpcion1.gameObject.SetActive(false);

        if (botonOpcion2 != null)
            botonOpcion2.gameObject.SetActive(false);

        if (botonOpcion3 != null)
            botonOpcion3.gameObject.SetActive(false);
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