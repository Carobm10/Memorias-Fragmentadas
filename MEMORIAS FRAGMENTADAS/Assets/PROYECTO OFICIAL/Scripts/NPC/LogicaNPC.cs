using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LogicaNPC : MonoBehaviour
{
    // =========================================================
    // DATOS GENERALES DEL NPC
    // =========================================================

    [Header("Datos del NPC")]
    public string nombreNPC = "Aurora [Hermana]";

    // =========================================================
    // PANELES DEL CANVAS
    // =========================================================

    [Header("Paneles")]
    public GameObject panelInteraccionNPC;
    public GameObject panelDialogoNPC;

    // =========================================================
    // TEXTOS DEL DIÁLOGO
    // =========================================================

    [Header("Textos")]
    public TMP_Text textoNombreNPC;
    public TMP_Text textoDialogo;
    public TMP_Text textoOpcion1;
    public TMP_Text textoOpcion2;
    public TMP_Text textoOpcion3;

    // =========================================================
    // BOTONES VISUALES
    // =========================================================

    [Header("Botones")]
    public Button botonOpcion1;
    public Button botonOpcion2;
    public Button botonOpcion3;
    public Button botonSalirDialogo;

    // =========================================================
    // AUDIO
    // =========================================================

    [Header("Audio")]
    public AudioSource fuenteAudio;
    public AudioClip audioInicio;
    public AudioClip audioRamaA;
    public AudioClip audioRamaA1;
    public AudioClip audioCierre;
    public AudioClip audioMamaRegano;

    // =========================================================
    // DETECCIÓN DEL PLAYER
    // =========================================================

    [Header("Detección")]
    public Transform player;
    public float distanciaParaHablar = 2f;
    public bool jugadorCerca = false;
    public bool jugadorMirando = false;

    // =========================================================
    // CONFIGURACIÓN
    // =========================================================

    [Header("Configuración")]
    public float velocidadEscritura = 0.03f;

    // =========================================================
    // ESTADO INTERNO
    // =========================================================

    private bool dialogoActivo = false;
    private bool escribiendo = false;
    private bool reproducirMamaReganoAlCerrar = false;
    private bool mamaReganoYaReproducido = false;
    private bool opcionAUsada = false;
    private bool opcionBUsada = false;
    private bool opcionYUsada = false;
    private bool seleccionDeshabilitada = false;
    private bool ramaPrincipalCompletada = false;
    private Coroutine rutinaEscritura;

    private enum EstadoDialogo
    {
        Inicio,
        RamaA,
        RamaA1,
        Cierre
    }

    private EstadoDialogo estadoActual;

    // =========================================================
    // INICIO
    // =========================================================

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

    void Update()
    {
        ActualizarCercaniaPorDistancia();

        bool puedeHablar = !seleccionDeshabilitada && jugadorCerca && jugadorMirando && !dialogoActivo;

        if (panelInteraccionNPC != null)
            panelInteraccionNPC.SetActive(puedeHablar);

        // A = iniciar diálogo
        if (puedeHablar && InputManagerCustom.PressA())
        {
            IniciarDialogo();
            return;
        }

        // X = salir del diálogo
        if (dialogoActivo && InputManagerCustom.PressX())
        {
            CerrarDialogo();
            return;
        }

        // Mientras escribe, no recibe opciones
        if (!dialogoActivo || escribiendo)
            return;

        // A / B / Y = elegir respuestas
        RevisarInputOpciones();
    }

    // =========================================================
    // DETECCIÓN
    // =========================================================

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

    public bool PuedeSerSeleccionado()
    {
        return !seleccionDeshabilitada;
    }

    // =========================================================
    // DIÁLOGO
    // =========================================================

    private void IniciarDialogo()
    {
        if (seleccionDeshabilitada)
        {
            return;
        }

        dialogoActivo = true;
        estadoActual = EstadoDialogo.Inicio;

        if (panelInteraccionNPC != null)
            panelInteraccionNPC.SetActive(false);

        if (panelDialogoNPC != null)
            panelDialogoNPC.SetActive(true);

        if (botonSalirDialogo != null)
            botonSalirDialogo.gameObject.SetActive(true);

        if (textoNombreNPC != null)
            textoNombreNPC.text = nombreNPC;

        MostrarEstadoActual();
    }

    private void MostrarEstadoActual()
    {
        OcultarBotones();

        switch (estadoActual)
        {
            case EstadoDialogo.Inicio:
                reproducirMamaReganoAlCerrar = false;
                ReproducirAudioDialogo(audioInicio);
                IniciarEscritura(
                    "¡Ey! ¿Quieres jugar conmigo?\nEstoy armando algo… pero me falta una pieza.\n¿Me ayudas?",
                    () =>
                    {
                        MostrarOpcionesInicialesDisponibles();
                    }
                );
                break;

            case EstadoDialogo.RamaA:
                reproducirMamaReganoAlCerrar = false;
                ReproducirAudioDialogo(audioRamaA);
                IniciarEscritura(
                    "¡Estoy armando una casita!\nPero siempre se me cae…\nTú eres mejor que yo… ¿cierto?",
                    () =>
                    {
                        MostrarOpcion1("Déjame ayudarte  A");
                    }
                );
                break;

            case EstadoDialogo.RamaA1:
                reproducirMamaReganoAlCerrar = true;
                ramaPrincipalCompletada = true;
                ReproducirAudioDialogo(audioRamaA1);
                IniciarEscritura(
                    "¡Sabía que sí!\nCuando seas grande, seguro vas a hacer cosas increíbles.\nVe que mamá te está llamando.",
                    () =>
                    {
                        //MostrarOpcion1("Continuar  A");
                    }
                );
                break;

            case EstadoDialogo.Cierre:
                reproducirMamaReganoAlCerrar = true;
                ReproducirAudioDialogo(audioCierre);
                IniciarEscritura(
                    "Bueno… ve pues.\nPero vuelve a jugar conmigo.",
                    () =>
                    {
                       // MostrarOpcion1("Salir  A");
                    }
                );
                break;
        }
    }

    // =========================================================
    // INPUT DE OPCIONES
    // =========================================================

    private void RevisarInputOpciones()
    {
        if (InputManagerCustom.PressA())
        {
            OpcionA();
            return;
        }

        if (InputManagerCustom.PressB())
        {
            OpcionB();
            return;
        }

        if (InputManagerCustom.PressY())
        {
            OpcionY();
            return;
        }
    }

    private void OpcionA()
    {
        if (estadoActual == EstadoDialogo.Inicio)
        {
            if (opcionAUsada)
                return;

            ConsumirRutaPrincipal();
            estadoActual = EstadoDialogo.RamaA;
        }
        else if (estadoActual == EstadoDialogo.RamaA)
        {
            estadoActual = EstadoDialogo.RamaA1;
        }
        else if (estadoActual == EstadoDialogo.RamaA1)
        {
            CerrarDialogo();
            return;
        }
        else if (estadoActual == EstadoDialogo.Cierre)
        {
            CerrarDialogo();
            return;
        }

        MostrarEstadoActual();
    }

    private void OpcionB()
    {
        if (estadoActual == EstadoDialogo.Inicio)
        {
            if (opcionBUsada)
                return;

            opcionBUsada = true;
            estadoActual = EstadoDialogo.Cierre;
            MostrarEstadoActual();
        }
    }

    private void OpcionY()
    {
        if (estadoActual == EstadoDialogo.Inicio)
        {
            if (opcionYUsada)
                return;

            ConsumirRutaPrincipal();
            estadoActual = EstadoDialogo.RamaA;
            MostrarEstadoActual();
        }
    }

    // =========================================================
    // BOTONES VISUALES
    // =========================================================

    private void MostrarOpcion1(string texto)
    {
        if (textoOpcion1 != null)
            textoOpcion1.text = texto;

        if (botonOpcion1 != null)
            botonOpcion1.gameObject.SetActive(true);
    }

    private void MostrarOpcion2(string texto)
    {
        if (textoOpcion2 != null)
            textoOpcion2.text = texto;

        if (botonOpcion2 != null)
            botonOpcion2.gameObject.SetActive(true);
    }

    private void MostrarOpcion3(string texto)
    {
        if (textoOpcion3 != null)
            textoOpcion3.text = texto;

        if (botonOpcion3 != null)
            botonOpcion3.gameObject.SetActive(true);
    }

    private void OcultarBotones()
    {
        if (botonOpcion1 != null)
            botonOpcion1.gameObject.SetActive(false);

        if (botonOpcion2 != null)
            botonOpcion2.gameObject.SetActive(false);

        if (botonOpcion3 != null)
            botonOpcion3.gameObject.SetActive(false);

        if (textoOpcion1 != null)
            textoOpcion1.text = "";

        if (textoOpcion2 != null)
            textoOpcion2.text = "";

        if (textoOpcion3 != null)
            textoOpcion3.text = "";
    }

    private void MostrarOpcionesInicialesDisponibles()
    {
        bool algunaDisponible = false;

        if (!opcionAUsada)
        {
            MostrarOpcion1("Sí, quiero ayudarte  A");
            algunaDisponible = true;
        }

        if (!opcionBUsada)
        {
            MostrarOpcion2("No puedo ahora  B");
            algunaDisponible = true;
        }

        if (!opcionYUsada)
        {
            MostrarOpcion3("¿Qué estás haciendo?  Y");
            algunaDisponible = true;
        }

        if (opcionAUsada && opcionYUsada && !opcionBUsada)
        {
            MostrarOpcion2("No puedo ahora  B");
            algunaDisponible = true;
        }

        if (!algunaDisponible)
        {
            IniciarEscritura(
                "Ya jugamos bastante por ahora.",
                () =>
                {
                    CerrarDialogo();
                }
            );
        }
    }

    // =========================================================
    // EFECTO DE ESCRITURA
    // =========================================================

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

    // =========================================================
    // CERRAR DIÁLOGO
    // =========================================================

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

        if (reproducirMamaReganoAlCerrar)
        {
            if (!mamaReganoYaReproducido)
            {
                ReproducirAudioDialogo(audioMamaRegano);
                mamaReganoYaReproducido = true;
            }

            reproducirMamaReganoAlCerrar = false;
        }

        if (!seleccionDeshabilitada && ramaPrincipalCompletada)
        {
            seleccionDeshabilitada = true;

            if (panelInteraccionNPC != null)
                panelInteraccionNPC.SetActive(false);
        }
    }

    // =========================================================
    // AUDIO
    // =========================================================

    private void ReproducirAudioDialogo(AudioClip clip)
    {
        if (fuenteAudio == null || clip == null)
            return;

        fuenteAudio.Stop();
        fuenteAudio.clip = clip;
        fuenteAudio.Play();
    }

    private void ConsumirRutaPrincipal()
    {
        opcionAUsada = true;
        opcionYUsada = true;
    }
}