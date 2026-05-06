using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PhoneDialogueController : MonoBehaviour
{
    [Header("Panel diálogo")]
    public GameObject dialoguePanel;

    [Header("Textos principales")]
    public TMP_Text speakerText;
    public TMP_Text dialogueText;

    [Header("Botones de opciones")]
    public Button optionYButton;
    public Button optionAButton;
    public Button optionBButton;
    public Button salirButton;

    [Header("Textos dentro de botones")]
    public TMP_Text optionYText;
    public TMP_Text optionAText;
    public TMP_Text optionBText;

    [Header("Movimiento jugador")]
    public MovimientoVR2 movimientoJugador;

    [Header("Configuración")]
    public float velocidadEscritura = 0.035f;
    public float pausaEntreLineas = 1.2f;

    private bool dialogoActivo = false;
    private bool escribiendo = false;
    private Coroutine rutinaDialogo;

    private enum EstadoTelefono
    {
        Inicio,
        RamaA,
        RamaA1,
        RamaA2,
        RamaA3,
        RamaB,
        RamaB1,
        RamaB2,
        RamaB3,
        RamaC,
        RamaC1,
        RamaC2,
        RamaC3
    }

    private EstadoTelefono estadoActual;

    void Awake()
    {
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        if (salirButton != null)
            salirButton.gameObject.SetActive(false);

        OcultarBotones();
    }

    void Update()
    {
        if (!dialogoActivo)
            return;

        if (InputManagerCustom.PressX())
        {
            CerrarDialogo();
            return;
        }

        if (escribiendo)
            return;

        RevisarInputOpciones();
    }

    public void StartPhoneDialogue()
    {
        dialogoActivo = true;
        estadoActual = EstadoTelefono.Inicio;

        if (movimientoJugador != null)
            movimientoJugador.puedeMoverse = false;

        if (dialoguePanel != null)
            dialoguePanel.SetActive(true);

        if (salirButton != null)
            salirButton.gameObject.SetActive(true);

        MostrarEstadoActual();
    }

    private void MostrarEstadoActual()
    {
        OcultarBotones();

        if (rutinaDialogo != null)
            StopCoroutine(rutinaDialogo);

        switch (estadoActual)
        {
            case EstadoTelefono.Inicio:
                rutinaDialogo = StartCoroutine(SecuenciaDialogo(
                    new string[] { "Joselito", "Prima", "Joselito", "Prima", "Prima" },
                    new string[]
                    {
                        "¿Aló…?",
                        "¡Joselito! Soy yo… ¿me escuchas?",
                        "Sí… ¿eres tú, María?",
                        "¡Sí! Oye, llamaba para preguntarte si mañana nos reunimos a ver el alunizaje en vivo.",
                        "Mi papá dice que lo van a dar por la televisión…"
                    },
                    () =>
                    {
                        MostrarOpcionY("Sí, allá nos vemos.");
                        MostrarOpcionA("No creo que pueda… voy a estar con mi familia.");
                        MostrarOpcionB("¿Eso qué es? ¿Qué van a hacer?");
                    }
                ));
                break;

            case EstadoTelefono.RamaA:
                rutinaDialogo = StartCoroutine(SecuenciaDialogo(
                    new string[] { "Joselito", "Prima" },
                    new string[]
                    {
                        "Sí, de una… allá nos vemos.",
                        "¡Ay, sí! Dicen que van a pisar la luna…"
                    },
                    () =>
                    {
                        MostrarOpcionY("¿En serio van a llegar?");
                        MostrarOpcionA("Eso suena raro…");
                        MostrarOpcionB("¿Y cómo lo vamos a ver?");
                    }
                ));
                break;

            case EstadoTelefono.RamaA1:
                rutinaDialogo = StartCoroutine(SecuenciaDialogo(
                    new string[] { "Joselito", "Prima", "Prima" },
                    new string[]
                    {
                        "¿En serio van a llegar?",
                        "¡Sí! Como en los cuentos… pero de verdad.",
                        "Wow, qué chévere… eso va a ser historia."
                    },
                    () => { }
                ));
                break;

            case EstadoTelefono.RamaA2:
                rutinaDialogo = StartCoroutine(SecuenciaDialogo(
                    new string[] { "Joselito", "Prima", "Prima", "Prima" },
                    new string[]
                    {
                        "Eso suena raro…",
                        "Tú siempre dices eso…",
                        "Pero esta vez es distinto.",
                        "Está bien, mañana te espero en mi casa."
                    },
                    () => { }
                ));
                break;

            case EstadoTelefono.RamaA3:
                rutinaDialogo = StartCoroutine(SecuenciaDialogo(
                    new string[] { "Joselito", "Prima", "Joselito", "Prima" },
                    new string[]
                    {
                        "¿Y cómo lo vamos a ver?",
                        "En la tele… todos juntos en la sala.",
                        "¿En tu casa o en la mía?",
                        "En la mía. Ven temprano mañana. Adiós."
                    },
                    () => { }
                ));
                break;

            case EstadoTelefono.RamaB:
                rutinaDialogo = StartCoroutine(SecuenciaDialogo(
                    new string[] { "Joselito", "Prima" },
                    new string[]
                    {
                        "No creo que pueda… voy a estar con mi familia.",
                        "Ah… bueno… entonces que te vaya bien. Adiós."
                    },
                    () =>
                    {
                        MostrarOpcionY("De pronto me escapo un rato.");
                        MostrarOpcionA("Tengo que ayudar en la casa.");
                        MostrarOpcionB("No me interesa mucho eso.");
                    }
                ));
                break;

            case EstadoTelefono.RamaB1:
                rutinaDialogo = StartCoroutine(SecuenciaDialogo(
                    new string[] { "Joselito", "Prima" },
                    new string[]
                    {
                        "De pronto me escapo un rato…",
                        "¡Sí! Aunque sea un momentico."
                    },
                    () => { }
                ));
                break;

            case EstadoTelefono.RamaB2:
                rutinaDialogo = StartCoroutine(SecuenciaDialogo(
                    new string[] { "Joselito", "Prima", "Joselito" },
                    new string[]
                    {
                        "Tengo que ayudar en la casa.",
                        "Bueno… eso también es importante.",
                        "Sí… lo siento. Adiós."
                    },
                    () => { }
                ));
                break;

            case EstadoTelefono.RamaB3:
                rutinaDialogo = StartCoroutine(SecuenciaDialogo(
                    new string[] { "Joselito", "Prima", "Prima" },
                    new string[]
                    {
                        "No me interesa mucho eso.",
                        "…Ah.",
                        "Bueno…"
                    },
                    () => { }
                ));
                break;

            case EstadoTelefono.RamaC:
                rutinaDialogo = StartCoroutine(SecuenciaDialogo(
                    new string[] { "Joselito", "Prima" },
                    new string[]
                    {
                        "¿Eso qué es? ¿Qué van a hacer?",
                        "¡Van a llegar a la luna!"
                    },
                    () =>
                    {
                        MostrarOpcionY("¿De verdad se puede?");
                        MostrarOpcionA("¿Como en los cuentos?");
                        MostrarOpcionB("No entiendo mucho…");
                    }
                ));
                break;

            case EstadoTelefono.RamaC1:
                rutinaDialogo = StartCoroutine(SecuenciaDialogo(
                    new string[] { "Joselito", "Prima", "Joselito" },
                    new string[]
                    {
                        "¿De verdad se puede?",
                        "Sí… mi papá dice que sí.",
                        "Listo, entonces mañana llego a tu casa en la tarde. Adiós."
                    },
                    () => { }
                ));
                break;

            case EstadoTelefono.RamaC2:
                rutinaDialogo = StartCoroutine(SecuenciaDialogo(
                    new string[] { "Joselito", "Prima" },
                    new string[]
                    {
                        "¿Como en los cuentos?",
                        "Sí… pero esta vez es real."
                    },
                    () => { }
                ));
                break;

            case EstadoTelefono.RamaC3:
                rutinaDialogo = StartCoroutine(SecuenciaDialogo(
                    new string[] { "Joselito", "Prima", "Prima" },
                    new string[]
                    {
                        "No entiendo mucho…",
                        "Yo tampoco mucho… pero quiero verlo contigo.",
                        "Está bien, mañana nos vemos en mi casa en la tarde. Adiós."
                    },
                    () => { }
                ));
                break;
        }
    }

    private IEnumerator SecuenciaDialogo(string[] nombres, string[] textos, System.Action alTerminar)
    {
        escribiendo = true;
        OcultarBotones();

        for (int i = 0; i < textos.Length; i++)
        {
            if (speakerText != null)
                speakerText.text = nombres[i];

            if (dialogueText != null)
                dialogueText.text = "";

            foreach (char letra in textos[i])
            {
                if (dialogueText != null)
                    dialogueText.text += letra;

                yield return new WaitForSeconds(velocidadEscritura);
            }

            yield return new WaitForSeconds(pausaEntreLineas);
        }

        escribiendo = false;
        alTerminar?.Invoke();
    }

    private void RevisarInputOpciones()
    {
        if (InputManagerCustom.PressY())
        {
            OpcionY();
            return;
        }

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
    }

    private void OpcionY()
    {
        if (estadoActual == EstadoTelefono.Inicio)
            estadoActual = EstadoTelefono.RamaA;
        else if (estadoActual == EstadoTelefono.RamaA)
            estadoActual = EstadoTelefono.RamaA1;
        else if (estadoActual == EstadoTelefono.RamaB)
            estadoActual = EstadoTelefono.RamaB1;
        else if (estadoActual == EstadoTelefono.RamaC)
            estadoActual = EstadoTelefono.RamaC1;
        else
        {
            CerrarDialogo();
            return;
        }

        MostrarEstadoActual();
    }

    private void OpcionA()
    {
        if (estadoActual == EstadoTelefono.Inicio)
            estadoActual = EstadoTelefono.RamaB;
        else if (estadoActual == EstadoTelefono.RamaA)
            estadoActual = EstadoTelefono.RamaA2;
        else if (estadoActual == EstadoTelefono.RamaB)
            estadoActual = EstadoTelefono.RamaB2;
        else if (estadoActual == EstadoTelefono.RamaC)
            estadoActual = EstadoTelefono.RamaC2;
        else
        {
            CerrarDialogo();
            return;
        }

        MostrarEstadoActual();
    }

    private void OpcionB()
    {
        if (estadoActual == EstadoTelefono.Inicio)
            estadoActual = EstadoTelefono.RamaC;
        else if (estadoActual == EstadoTelefono.RamaA)
            estadoActual = EstadoTelefono.RamaA3;
        else if (estadoActual == EstadoTelefono.RamaB)
            estadoActual = EstadoTelefono.RamaB3;
        else if (estadoActual == EstadoTelefono.RamaC)
            estadoActual = EstadoTelefono.RamaC3;
        else
        {
            CerrarDialogo();
            return;
        }

        MostrarEstadoActual();
    }

    private void MostrarOpcionY(string texto)
    {
        if (optionYText != null)
            optionYText.text = texto;

        if (optionYButton != null)
            optionYButton.gameObject.SetActive(true);
    }

    private void MostrarOpcionA(string texto)
    {
        if (optionAText != null)
            optionAText.text = texto;

        if (optionAButton != null)
            optionAButton.gameObject.SetActive(true);
    }

    private void MostrarOpcionB(string texto)
    {
        if (optionBText != null)
            optionBText.text = texto;

        if (optionBButton != null)
            optionBButton.gameObject.SetActive(true);
    }

    private void OcultarBotones()
    {
        if (optionYButton != null)
            optionYButton.gameObject.SetActive(false);

        if (optionAButton != null)
            optionAButton.gameObject.SetActive(false);

        if (optionBButton != null)
            optionBButton.gameObject.SetActive(false);

        if (optionYText != null)
            optionYText.text = "";

        if (optionAText != null)
            optionAText.text = "";

        if (optionBText != null)
            optionBText.text = "";
    }

    private void CerrarDialogo()
    {
        dialogoActivo = false;
        escribiendo = false;

        if (rutinaDialogo != null)
            StopCoroutine(rutinaDialogo);

        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        if (salirButton != null)
            salirButton.gameObject.SetActive(false);

        OcultarBotones();

        if (movimientoJugador != null)
            movimientoJugador.puedeMoverse = true;
    }
}