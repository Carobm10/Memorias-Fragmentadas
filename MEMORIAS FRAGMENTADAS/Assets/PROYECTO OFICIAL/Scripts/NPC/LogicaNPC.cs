using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LogicaNPC : MonoBehaviour
{
    [Header("Paneles")]
    public GameObject panelInteraccionNPC;
    public GameObject panelDialogoNPC;

    [Header("Botón salir (imagen)")]
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

    [Header("Configuración")]
    public KeyCode teclaHablarTeclado = KeyCode.X;
    public KeyCode teclaHablarJoystick = KeyCode.JoystickButton3;   // X
    public KeyCode teclaOpcion1 = KeyCode.JoystickButton11;         // A
    public KeyCode teclaOpcion2 = KeyCode.JoystickButton7;          // B
    public KeyCode teclaOpcion3 = KeyCode.JoystickButton4;          // Y
    public float velocidadEscritura = 0.03f;

    [Header("Debug")]
    public bool jugadorCerca = false;

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

        // La imagen de salir actúa como botón
        if (botonSalirDialogo != null)
            botonSalirDialogo.onClick.AddListener(CerrarDialogo);
    }

    void Update()
    {
        // Mostrar prompt solo si está cerca y no hay diálogo
        if (panelInteraccionNPC != null)
            panelInteraccionNPC.SetActive(jugadorCerca && !dialogoActivo);

        // Abrir diálogo con X si está cerca
        if (jugadorCerca && !dialogoActivo &&
            (Input.GetKeyDown(teclaHablarTeclado) || Input.GetKeyDown(teclaHablarJoystick)))
        {
            IniciarDialogo();
        }

        // Salir del diálogo en cualquier momento con B
        if (dialogoActivo && InputManagerCustom.PressB())
        {
            Debug.Log("Salir del diálogo manualmente");
            CerrarDialogo();
            return;
        }

        // Elegir opciones cuando el diálogo está activo
        if (dialogoActivo && !escribiendo)
        {
            if (botonOpcion1 != null && botonOpcion1.gameObject.activeSelf && Input.GetKeyDown(teclaOpcion1))
                Opcion1();

            if (botonOpcion2 != null && botonOpcion2.gameObject.activeSelf && Input.GetKeyDown(teclaOpcion2))
                Opcion2();

            if (botonOpcion3 != null && botonOpcion3.gameObject.activeSelf && Input.GetKeyDown(teclaOpcion3))
                Opcion3();
        }
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
                        if (textoOpcion1 != null) textoOpcion1.text = "Sí, ¿a qué estás jugando?  Y";
                        if (textoOpcion2 != null) textoOpcion2.text = "No puedo ahora, tengo que alistarme.  A";
                        if (textoOpcion3 != null) textoOpcion3.text = "¿Qué estás haciendo?  B";

                        if (botonOpcion1 != null) botonOpcion1.gameObject.SetActive(true);
                        if (botonOpcion2 != null) botonOpcion2.gameObject.SetActive(true);
                        if (botonOpcion3 != null) botonOpcion3.gameObject.SetActive(true);
                    }
                );
                break;

            case EstadoDialogo.RamaA:
                ReproducirAudioDialogo(audioRamaA);
                IniciarEscritura(
                    "¡Estoy armando una casita!\nPero siempre se me cae…\n\nTú eres mejor que yo… ¿cierto?",
                    () =>
                    {
                        if (textoOpcion1 != null) textoOpcion1.text = "Déjame ayudarte.  A";
                        if (textoOpcion2 != null) textoOpcion2.text = "Está bien así.  B";
                        if (textoOpcion3 != null) textoOpcion3.text = "";

                        if (botonOpcion1 != null) botonOpcion1.gameObject.SetActive(true);
                        if (botonOpcion2 != null) botonOpcion2.gameObject.SetActive(true);
                        if (botonOpcion3 != null) botonOpcion3.gameObject.SetActive(false);
                    }
                );
                break;

            case EstadoDialogo.RamaA1:
                ReproducirAudioDialogo(audioRamaA1);
                IniciarEscritura(
                    "¡Sabía que sí!\nCuando seas grande, seguro vas a hacer cosas increíbles.",
                    () => { MostrarSoloBotonContinuar(); }
                );
                break;

            case EstadoDialogo.RamaA2:
                ReproducirAudioDialogo(audioRamaA2);
                IniciarEscritura(
                    "Bueno… igual me gusta como queda.\nNo todo tiene que ser perfecto.",
                    () => { MostrarSoloBotonContinuar(); }
                );
                break;

            case EstadoDialogo.RamaB:
                ReproducirAudioDialogo(audioRamaB);
                IniciarEscritura(
                    "Ahh…\nSiempre estás ocupado…\n\nBueno… pero no te demores mucho.\nLuego jugamos, ¿sí?",
                    () => { MostrarSoloBotonContinuar(); }
                );
                break;

            case EstadoDialogo.RamaC:
                ReproducirAudioDialogo(audioRamaC);
                IniciarEscritura(
                    "Es una casa…\nPara que vivamos todos…\n\nAsí no se nos olvida nada.\n¿Tú crees que las cosas se olvidan?",
                    () =>
                    {
                        if (textoOpcion1 != null) textoOpcion1.text = "A veces…  A";
                        if (textoOpcion2 != null) textoOpcion2.text = "No, si las recuerdas.  B";
                        if (textoOpcion3 != null) textoOpcion3.text = "";

                        if (botonOpcion1 != null) botonOpcion1.gameObject.SetActive(true);
                        if (botonOpcion2 != null) botonOpcion2.gameObject.SetActive(true);
                        if (botonOpcion3 != null) botonOpcion3.gameObject.SetActive(false);
                    }
                );
                break;

            case EstadoDialogo.RamaC1:
                ReproducirAudioDialogo(audioRamaC1);
                IniciarEscritura(
                    "Entonces hay que cuidarlas…\nPara que no desaparezcan.",
                    () => { MostrarSoloBotonContinuar(); }
                );
                break;

            case EstadoDialogo.RamaC2:
                ReproducirAudioDialogo(audioRamaC2);
                IniciarEscritura(
                    "Entonces está bien…\nPorque yo no quiero olvidar esto.",
                    () => { MostrarSoloBotonContinuar(); }
                );
                break;

            case EstadoDialogo.Cierre:
                ReproducirAudioDialogo(audioCierre);
                IniciarEscritura(
                    "Bueno… ve, que mamá te está llamando.",
                    () =>
                    {
                        if (textoOpcion1 != null) textoOpcion1.text = "Cerrar  A";
                        if (textoOpcion2 != null) textoOpcion2.text = "";
                        if (textoOpcion3 != null) textoOpcion3.text = "";

                        if (botonOpcion1 != null) botonOpcion1.gameObject.SetActive(true);
                        if (botonOpcion2 != null) botonOpcion2.gameObject.SetActive(false);
                        if (botonOpcion3 != null) botonOpcion3.gameObject.SetActive(false);
                    }
                );
                break;
        }
    }

    void IniciarEscritura(string mensaje, System.Action alTerminar)
    {
        if (rutinaEscritura != null)
            StopCoroutine(rutinaEscritura);

        rutinaEscritura = StartCoroutine(EscribirTexto(mensaje, alTerminar));
    }

    IEnumerator EscribirTexto(string mensaje, System.Action alTerminar)
    {
        escribiendo = true;

        if (textoDialogo != null)
            textoDialogo.text = "";

        foreach (char letra in mensaje)
        {
            if (textoDialogo != null)
                textoDialogo.text += letra;

            yield return new WaitForSeconds(velocidadEscritura);
        }

        escribiendo = false;
        alTerminar?.Invoke();
    }

    void OcultarBotones()
    {
        if (botonOpcion1 != null) botonOpcion1.gameObject.SetActive(false);
        if (botonOpcion2 != null) botonOpcion2.gameObject.SetActive(false);
        if (botonOpcion3 != null) botonOpcion3.gameObject.SetActive(false);

        if (textoOpcion1 != null) textoOpcion1.text = "";
        if (textoOpcion2 != null) textoOpcion2.text = "";
        if (textoOpcion3 != null) textoOpcion3.text = "";
    }

    void MostrarSoloBotonContinuar()
    {
        if (textoOpcion1 != null) textoOpcion1.text = "Continuar  A";
        if (textoOpcion2 != null) textoOpcion2.text = "";
        if (textoOpcion3 != null) textoOpcion3.text = "";

        if (botonOpcion1 != null) botonOpcion1.gameObject.SetActive(true);
        if (botonOpcion2 != null) botonOpcion2.gameObject.SetActive(false);
        if (botonOpcion3 != null) botonOpcion3.gameObject.SetActive(false);
    }

    public void Opcion1()
    {
        switch (estadoActual)
        {
            case EstadoDialogo.Inicio:
                estadoActual = EstadoDialogo.RamaA;
                break;
            case EstadoDialogo.RamaA:
                estadoActual = EstadoDialogo.RamaA1;
                break;
            case EstadoDialogo.RamaC:
                estadoActual = EstadoDialogo.RamaC1;
                break;
            case EstadoDialogo.RamaA1:
            case EstadoDialogo.RamaA2:
            case EstadoDialogo.RamaB:
            case EstadoDialogo.RamaC1:
            case EstadoDialogo.RamaC2:
                estadoActual = EstadoDialogo.Cierre;
                break;
            case EstadoDialogo.Cierre:
                CerrarDialogo();
                return;
        }

        MostrarEstadoActual();
    }

    public void Opcion2()
    {
        switch (estadoActual)
        {
            case EstadoDialogo.Inicio:
                estadoActual = EstadoDialogo.RamaB;
                break;
            case EstadoDialogo.RamaA:
                estadoActual = EstadoDialogo.RamaA2;
                break;
            case EstadoDialogo.RamaC:
                estadoActual = EstadoDialogo.RamaC2;
                break;
            default:
                return;
        }

        MostrarEstadoActual();
    }

    public void Opcion3()
    {
        switch (estadoActual)
        {
            case EstadoDialogo.Inicio:
                estadoActual = EstadoDialogo.RamaC;
                break;
            default:
                return;
        }

        MostrarEstadoActual();
    }

    public void CerrarDialogo()
    {
        dialogoActivo = false;
        escribiendo = false;

        if (rutinaEscritura != null)
        {
            StopCoroutine(rutinaEscritura);
            rutinaEscritura = null;
        }

        if (panelDialogoNPC != null)
            panelDialogoNPC.SetActive(false);

        if (panelInteraccionNPC != null)
            panelInteraccionNPC.SetActive(false);

        if (botonSalirDialogo != null)
            botonSalirDialogo.gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = true;
            Debug.Log("Jugador entró al área del NPC");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = false;

            if (panelInteraccionNPC != null)
                panelInteraccionNPC.SetActive(false);

            Debug.Log("Jugador salió del área del NPC");
        }
    }
}