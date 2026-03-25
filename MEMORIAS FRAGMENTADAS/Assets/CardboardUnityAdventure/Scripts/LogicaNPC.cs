using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LogicaNPC : MonoBehaviour
{
    [Header("Paneles")]
    public GameObject panelInteraccionNPC;
    public GameObject panelDialogoNPC;

    [Header("Textos")]
    public TMP_Text textoDialogo;
    public TMP_Text textoOpcion1;
    public TMP_Text textoOpcion2;
    public TMP_Text textoOpcion3;

    [Header("Botones")]
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

    private bool jugadorCerca = false;
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

    void Start()
    {
        panelInteraccionNPC.SetActive(false);
        panelDialogoNPC.SetActive(false);

        botonOpcion1.onClick.AddListener(Opcion1);
        botonOpcion2.onClick.AddListener(Opcion2);
        botonOpcion3.onClick.AddListener(Opcion3);
    }

    void Update()
    {
        // Mostrar diálogo al estar cerca y oprimir X
        if (jugadorCerca && !dialogoActivo &&
            (Input.GetKeyDown(teclaHablarTeclado) || Input.GetKeyDown(teclaHablarJoystick)))
        {
            IniciarDialogo();
        }

        // Elegir opciones con joystick cuando el diálogo está activo
        if (dialogoActivo && !escribiendo)
        {
            if (botonOpcion1.gameObject.activeSelf && Input.GetKeyDown(teclaOpcion1))
            {
                Opcion1();
            }

            if (botonOpcion2.gameObject.activeSelf && Input.GetKeyDown(teclaOpcion2))
            {
                Opcion2();
            }

            if (botonOpcion3.gameObject.activeSelf && Input.GetKeyDown(teclaOpcion3))
            {
                Opcion3();
            }
        }
    }

    private void IniciarDialogo()
    {
        dialogoActivo = true;
        panelInteraccionNPC.SetActive(false);
        panelDialogoNPC.SetActive(true);

        estadoActual = EstadoDialogo.Inicio;
        MostrarEstadoActual();
    }

    private void MostrarEstadoActual()
    {
        OcultarBotones();

        switch (estadoActual)
        {
            case EstadoDialogo.Inicio:
                IniciarEscritura(
                    "¡Ey! ¿Quieres jugar conmigo?\nEstoy armando algo… pero me falta una pieza.\n\n¿Me ayudas o estás ocupado?",
                    () =>
                    {
                        textoOpcion1.text = "A. Sí, ¿a qué estás jugando?";
                        textoOpcion2.text = "B. No puedo ahora, tengo que alistarme.";
                        textoOpcion3.text = "Y. ¿Qué estás haciendo?";

                        botonOpcion1.gameObject.SetActive(true);
                        botonOpcion2.gameObject.SetActive(true);
                        botonOpcion3.gameObject.SetActive(true);
                    }
                );
                break;

            case EstadoDialogo.RamaA:
                IniciarEscritura(
                    "¡Estoy armando una casita!\nPero siempre se me cae…\n\nTú eres mejor que yo… ¿cierto?",
                    () =>
                    {
                        textoOpcion1.text = "A. Déjame ayudarte.";
                        textoOpcion2.text = "B. Está bien así.";
                        textoOpcion3.text = "";

                        botonOpcion1.gameObject.SetActive(true);
                        botonOpcion2.gameObject.SetActive(true);
                        botonOpcion3.gameObject.SetActive(false);
                    }
                );
                break;

            case EstadoDialogo.RamaA1:
                IniciarEscritura(
                    "¡Sabía que sí!\nCuando seas grande, seguro vas a hacer cosas increíbles.",
                    () =>
                    {
                        MostrarSoloBotonContinuar();
                    }
                );
                break;

            case EstadoDialogo.RamaA2:
                IniciarEscritura(
                    "Bueno… igual me gusta como queda.\nNo todo tiene que ser perfecto.",
                    () =>
                    {
                        MostrarSoloBotonContinuar();
                    }
                );
                break;

            case EstadoDialogo.RamaB:
                IniciarEscritura(
                    "Ahh…\nSiempre estás ocupado…\n\nBueno… pero no te demores mucho.\nLuego jugamos, ¿sí?",
                    () =>
                    {
                        MostrarSoloBotonContinuar();
                    }
                );
                break;

            case EstadoDialogo.RamaC:
                IniciarEscritura(
                    "Es una casa…\nPara que vivamos todos…\n\nAsí no se nos olvida nada.\n¿Tú crees que las cosas se olvidan?",
                    () =>
                    {
                        textoOpcion1.text = "A. A veces…";
                        textoOpcion2.text = "B. No, si las recuerdas.";
                        textoOpcion3.text = "";

                        botonOpcion1.gameObject.SetActive(true);
                        botonOpcion2.gameObject.SetActive(true);
                        botonOpcion3.gameObject.SetActive(false);
                    }
                );
                break;

            case EstadoDialogo.RamaC1:
                IniciarEscritura(
                    "Entonces hay que cuidarlas…\nPara que no desaparezcan.",
                    () =>
                    {
                        MostrarSoloBotonContinuar();
                    }
                );
                break;

            case EstadoDialogo.RamaC2:
                IniciarEscritura(
                    "Entonces está bien…\nPorque yo no quiero olvidar esto.",
                    () =>
                    {
                        MostrarSoloBotonContinuar();
                    }
                );
                break;

            case EstadoDialogo.Cierre:
                IniciarEscritura(
                    "Bueno… ve, que mamá te está llamando.",
                    () =>
                    {
                        textoOpcion1.text = "A. Cerrar";
                        textoOpcion2.text = "";
                        textoOpcion3.text = "";

                        botonOpcion1.gameObject.SetActive(true);
                        botonOpcion2.gameObject.SetActive(false);
                        botonOpcion3.gameObject.SetActive(false);
                    }
                );
                break;
        }
    }

    private void IniciarEscritura(string mensaje, System.Action alTerminar)
    {
        if (rutinaEscritura != null)
        {
            StopCoroutine(rutinaEscritura);
        }

        rutinaEscritura = StartCoroutine(EfectoEscritura(mensaje, alTerminar));
    }

    private IEnumerator EfectoEscritura(string mensaje, System.Action alTerminar)
    {
        escribiendo = true;
        textoDialogo.text = "";

        foreach (char letra in mensaje)
        {
            textoDialogo.text += letra;
            yield return new WaitForSeconds(velocidadEscritura);
        }

        escribiendo = false;
        alTerminar?.Invoke();
    }

    private void OcultarBotones()
    {
        botonOpcion1.gameObject.SetActive(false);
        botonOpcion2.gameObject.SetActive(false);
        botonOpcion3.gameObject.SetActive(false);
    }

    private void MostrarSoloBotonContinuar()
    {
        textoOpcion1.text = "A. Continuar";
        textoOpcion2.text = "";
        textoOpcion3.text = "";

        botonOpcion1.gameObject.SetActive(true);
        botonOpcion2.gameObject.SetActive(false);
        botonOpcion3.gameObject.SetActive(false);
    }

    public void Opcion1()
    {
        if (escribiendo) return;

        switch (estadoActual)
        {
            case EstadoDialogo.Inicio:
                estadoActual = EstadoDialogo.RamaA;
                break;

            case EstadoDialogo.RamaA:
                estadoActual = EstadoDialogo.RamaA1;
                break;

            case EstadoDialogo.RamaA1:
            case EstadoDialogo.RamaA2:
            case EstadoDialogo.RamaB:
            case EstadoDialogo.RamaC1:
            case EstadoDialogo.RamaC2:
                estadoActual = EstadoDialogo.Cierre;
                break;

            case EstadoDialogo.RamaC:
                estadoActual = EstadoDialogo.RamaC1;
                break;

            case EstadoDialogo.Cierre:
                CerrarDialogo();
                return;
        }

        MostrarEstadoActual();
    }

    public void Opcion2()
    {
        if (escribiendo) return;

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
        if (escribiendo) return;

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

    private void CerrarDialogo()
    {
        dialogoActivo = false;
        panelDialogoNPC.SetActive(false);

        if (jugadorCerca)
        {
            panelInteraccionNPC.SetActive(true);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = true;

            if (!dialogoActivo)
            {
                panelInteraccionNPC.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = false;
            panelInteraccionNPC.SetActive(false);
            panelDialogoNPC.SetActive(false);
            dialogoActivo = false;
        }
    }
}