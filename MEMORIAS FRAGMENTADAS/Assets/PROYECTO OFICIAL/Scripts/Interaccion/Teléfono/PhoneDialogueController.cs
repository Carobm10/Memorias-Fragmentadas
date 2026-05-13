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

    [Header("Audio")]
    public AudioSource audioSource;

    [Header("Audios INICIO")]
    public AudioClip audio100_Nino_Alo;
    public AudioClip audio101_Prima_SoyYo;
    public AudioClip audio102_Nino_EresTuMaria;
    public AudioClip audio103_Prima_Si_Y_Alunizaje;
    public AudioClip audio104_Prima_Television;

    [Header("Audios RAMA A")]
    public AudioClip audio200_Nino_SiNosVemos;
    public AudioClip audio201_Prima_PisarLuna;

    public AudioClip audio210_Nino_EnSerioLlegar;
    public AudioClip audio211_Prima_Cuentos;
    public AudioClip audio212_Prima_Historia;

    public AudioClip audio220_Nino_SuenaRaro;
    public AudioClip audio221_Prima_SiempreDicesEso;
    public AudioClip audio222_Prima_Distinto;
    public AudioClip audio223_Nino_TeEspero;

    public AudioClip audio230_Nino_ComoLoVamosAVer;
    public AudioClip audio231_Prima_TeleSala;
    public AudioClip audio232_Nino_Casa;
    public AudioClip audio233_Prima_VenTemprano;

    [Header("Audios RAMA B")]
    public AudioClip audio300_Nino_NoPuedo;
    public AudioClip audio301_Prima_QueTeVayaBien;

    public AudioClip audio310_Nino_MeEscapo;
    public AudioClip audio311_Prima_Momentico;

    public AudioClip audio320_Nino_AyudarCasa;
    public AudioClip audio321_Prima_Importante;
    public AudioClip audio322_Nino_LoSiento;

    public AudioClip audio330_Nino_NoMeInteresa;
    public AudioClip audio331_Prima_Ah;
    public AudioClip audio332_Prima_Bueno;

    [Header("Audios RAMA C")]
    public AudioClip audio400_Nino_QueEsEso;
    public AudioClip audio401_Prima_LlegarLuna;

    public AudioClip audio410_Nino_DeVerdad;
    public AudioClip audio411_Prima_MiPapaDice;
    public AudioClip audio412_Nino_LlegoATuCasa;

    public AudioClip audio420_Nino_ComoCuentos;
    public AudioClip audio421_Prima_EsReal;

    public AudioClip audio430_Nino_NoEntiendo;
    public AudioClip audio431_Prima_QuieroVerlo;
    public AudioClip audio432_Nino_NosVemos;

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
    public float pausaEntreLineas = 0.4f;

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
        {
            salirButton.gameObject.SetActive(false);
            salirButton.onClick.AddListener(CerrarDialogo);
        }

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
                    new string[]
                    {
                        "Joselito",
                        "María",
                        "Joselito",
                        "María",
                        "María"
                    },
                    new string[]
                    {
                        "¿Aló…?",
                        "¡Joselito! Soy yo… ¿me escuchas?",
                        "Sí… ¿eres tú, María?",
                        "¡Sí! Oye, llamaba para preguntarte si mañana nos reunimos a ver el alunizaje en vivo.",
                        "Mi papá dice que lo van a dar por la televisión…"
                    },
                    new AudioClip[]
                    {
                        audio100_Nino_Alo,
                        audio101_Prima_SoyYo,
                        audio102_Nino_EresTuMaria,
                        audio103_Prima_Si_Y_Alunizaje,
                        audio104_Prima_Television
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
                    new string[]
                    {
                        "Joselito",
                        "María"
                    },
                    new string[]
                    {
                        "Sí, de una… allá nos vemos.",
                        "¡Ay, sí! Dicen que van a pisar la luna…"
                    },
                    new AudioClip[]
                    {
                        audio200_Nino_SiNosVemos,
                        audio201_Prima_PisarLuna
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
                    new string[]
                    {
                        "Joselito",
                        "María",
                        "María"
                    },
                    new string[]
                    {
                        "¿En serio van a llegar?",
                        "¡Sí! Como en los cuentos… pero de verdad.",
                        "Wow, qué chévere. Eso va a ser historia."
                    },
                    new AudioClip[]
                    {
                        audio210_Nino_EnSerioLlegar,
                        audio211_Prima_Cuentos,
                        audio212_Prima_Historia
                    },
                    FinalizarConversacion
                ));
                break;

            case EstadoTelefono.RamaA2:
                rutinaDialogo = StartCoroutine(SecuenciaDialogo(
                    new string[]
                    {
                        "Joselito",
                        "María",
                        "María",
                        "Joselito"
                    },
                    new string[]
                    {
                        "Eso suena raro…",
                        "Tú siempre dices eso…",
                        "Pero esta vez es distinto.",
                        "Está bien, mañana te espero en mi casa."
                    },
                    new AudioClip[]
                    {
                        audio220_Nino_SuenaRaro,
                        audio221_Prima_SiempreDicesEso,
                        audio222_Prima_Distinto,
                        audio223_Nino_TeEspero
                    },
                    FinalizarConversacion
                ));
                break;

            case EstadoTelefono.RamaA3:
                rutinaDialogo = StartCoroutine(SecuenciaDialogo(
                    new string[]
                    {
                        "Joselito",
                        "María",
                        "Joselito",
                        "María"
                    },
                    new string[]
                    {
                        "¿Y cómo lo vamos a ver?",
                        "En la tele… todos juntos en la sala.",
                        "¿En tu casa o en la mía?",
                        "En la mía. Ven temprano mañana. Adiós."
                    },
                    new AudioClip[]
                    {
                        audio230_Nino_ComoLoVamosAVer,
                        audio231_Prima_TeleSala,
                        audio232_Nino_Casa,
                        audio233_Prima_VenTemprano
                    },
                    FinalizarConversacion
                ));
                break;

            case EstadoTelefono.RamaB:
                rutinaDialogo = StartCoroutine(SecuenciaDialogo(
                    new string[]
                    {
                        "Joselito",
                        "María"
                    },
                    new string[]
                    {
                        "No creo que pueda… voy a estar con mi familia.",
                        "Ah… bueno… entonces que te vaya bien. Adiós."
                    },
                    new AudioClip[]
                    {
                        audio300_Nino_NoPuedo,
                        audio301_Prima_QueTeVayaBien
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
                    new string[]
                    {
                        "Joselito",
                        "María"
                    },
                    new string[]
                    {
                        "De pronto me escapo un rato…",
                        "¡Sí! Aunque sea un momentico."
                    },
                    new AudioClip[]
                    {
                        audio310_Nino_MeEscapo,
                        audio311_Prima_Momentico
                    },
                    FinalizarConversacion
                ));
                break;

            case EstadoTelefono.RamaB2:
                rutinaDialogo = StartCoroutine(SecuenciaDialogo(
                    new string[]
                    {
                        "Joselito",
                        "María",
                        "Joselito"
                    },
                    new string[]
                    {
                        "Tengo que ayudar en la casa.",
                        "Bueno… eso también es importante.",
                        "Sí… lo siento. Adiós."
                    },
                    new AudioClip[]
                    {
                        audio320_Nino_AyudarCasa,
                        audio321_Prima_Importante,
                        audio322_Nino_LoSiento
                    },
                    FinalizarConversacion
                ));
                break;

            case EstadoTelefono.RamaB3:
                rutinaDialogo = StartCoroutine(SecuenciaDialogo(
                    new string[]
                    {
                        "Joselito",
                        "María",
                        "María"
                    },
                    new string[]
                    {
                        "No me interesa mucho eso.",
                        "…Ah.",
                        "Bueno…"
                    },
                    new AudioClip[]
                    {
                        audio330_Nino_NoMeInteresa,
                        audio331_Prima_Ah,
                        audio332_Prima_Bueno
                    },
                    FinalizarConversacion
                ));
                break;

            case EstadoTelefono.RamaC:
                rutinaDialogo = StartCoroutine(SecuenciaDialogo(
                    new string[]
                    {
                        "Joselito",
                        "María"
                    },
                    new string[]
                    {
                        "¿Eso qué es? ¿Qué van a hacer?",
                        "¡Van a llegar a la luna!"
                    },
                    new AudioClip[]
                    {
                        audio400_Nino_QueEsEso,
                        audio401_Prima_LlegarLuna
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
                    new string[]
                    {
                        "Joselito",
                        "María",
                        "Joselito"
                    },
                    new string[]
                    {
                        "¿De verdad se puede?",
                        "Sí… mi papá dice que sí.",
                        "Listo, entonces mañana llego a tu casa en la tarde. Adiós."
                    },
                    new AudioClip[]
                    {
                        audio410_Nino_DeVerdad,
                        audio411_Prima_MiPapaDice,
                        audio412_Nino_LlegoATuCasa
                    },
                    FinalizarConversacion
                ));
                break;

            case EstadoTelefono.RamaC2:
                rutinaDialogo = StartCoroutine(SecuenciaDialogo(
                    new string[]
                    {
                        "Joselito",
                        "María"
                    },
                    new string[]
                    {
                        "¿Como en los cuentos?",
                        "Sí… pero esta vez es real."
                    },
                    new AudioClip[]
                    {
                        audio420_Nino_ComoCuentos,
                        audio421_Prima_EsReal
                    },
                    FinalizarConversacion
                ));
                break;

            case EstadoTelefono.RamaC3:
                rutinaDialogo = StartCoroutine(SecuenciaDialogo(
                    new string[]
                    {
                        "Joselito",
                        "María",
                        "Joselito"
                    },
                    new string[]
                    {
                        "No entiendo mucho…",
                        "Yo tampoco mucho… pero quiero verlo contigo.",
                        "Está bien, mañana nos vemos en mi casa en la tarde. Adiós."
                    },
                    new AudioClip[]
                    {
                        audio430_Nino_NoEntiendo,
                        audio431_Prima_QuieroVerlo,
                        audio432_Nino_NosVemos
                    },
                    FinalizarConversacion
                ));
                break;
        }
    }

    IEnumerator SecuenciaDialogo(string[] personajes, string[] textos, AudioClip[] audios, System.Action alTerminar)
    {
        escribiendo = true;
        OcultarBotones();

        for (int i = 0; i < textos.Length; i++)
        {
            if (speakerText != null)
                speakerText.text = personajes[i];

            if (dialogueText != null)
                dialogueText.text = "";

            if (audioSource != null)
            {
                audioSource.Stop();

                if (audios != null && i < audios.Length && audios[i] != null)
                {
                    audioSource.clip = audios[i];
                    audioSource.Play();
                }
            }

            yield return StartCoroutine(EscribirTexto(textos[i]));

            if (audioSource != null && audios != null && i < audios.Length && audios[i] != null)
            {
                while (audioSource.isPlaying)
                    yield return null;
            }

            yield return new WaitForSeconds(pausaEntreLineas);
        }

        escribiendo = false;
        alTerminar?.Invoke();
    }

    IEnumerator EscribirTexto(string texto)
    {
        if (dialogueText == null)
            yield break;

        dialogueText.text = "";

        foreach (char letra in texto)
        {
            dialogueText.text += letra;
            yield return new WaitForSeconds(velocidadEscritura);
        }
    }

    private void RevisarInputOpciones()
    {
        if (optionYButton != null && optionYButton.gameObject.activeSelf && InputManagerCustom.PressY())
            SeleccionarOpcionY();

        if (optionAButton != null && optionAButton.gameObject.activeSelf && InputManagerCustom.PressA())
            SeleccionarOpcionA();

        if (optionBButton != null && optionBButton.gameObject.activeSelf && InputManagerCustom.PressB())
            SeleccionarOpcionB();
    }

    private void SeleccionarOpcionY()
    {
        if (estadoActual == EstadoTelefono.Inicio)
            estadoActual = EstadoTelefono.RamaA;
        else if (estadoActual == EstadoTelefono.RamaA)
            estadoActual = EstadoTelefono.RamaA1;
        else if (estadoActual == EstadoTelefono.RamaB)
            estadoActual = EstadoTelefono.RamaB1;
        else if (estadoActual == EstadoTelefono.RamaC)
            estadoActual = EstadoTelefono.RamaC1;

        MostrarEstadoActual();
    }

    private void SeleccionarOpcionA()
    {
        if (estadoActual == EstadoTelefono.Inicio)
            estadoActual = EstadoTelefono.RamaB;
        else if (estadoActual == EstadoTelefono.RamaA)
            estadoActual = EstadoTelefono.RamaA2;
        else if (estadoActual == EstadoTelefono.RamaB)
            estadoActual = EstadoTelefono.RamaB2;
        else if (estadoActual == EstadoTelefono.RamaC)
            estadoActual = EstadoTelefono.RamaC2;

        MostrarEstadoActual();
    }

    private void SeleccionarOpcionB()
    {
        if (estadoActual == EstadoTelefono.Inicio)
            estadoActual = EstadoTelefono.RamaC;
        else if (estadoActual == EstadoTelefono.RamaA)
            estadoActual = EstadoTelefono.RamaA3;
        else if (estadoActual == EstadoTelefono.RamaB)
            estadoActual = EstadoTelefono.RamaB3;
        else if (estadoActual == EstadoTelefono.RamaC)
            estadoActual = EstadoTelefono.RamaC3;

        MostrarEstadoActual();
    }

    private void MostrarOpcionY(string texto)
    {
        if (optionYButton != null)
            optionYButton.gameObject.SetActive(true);

        if (optionYText != null)
            optionYText.text = "Y - " + texto;
    }

    private void MostrarOpcionA(string texto)
    {
        if (optionAButton != null)
            optionAButton.gameObject.SetActive(true);

        if (optionAText != null)
            optionAText.text = "A - " + texto;
    }

    private void MostrarOpcionB(string texto)
    {
        if (optionBButton != null)
            optionBButton.gameObject.SetActive(true);

        if (optionBText != null)
            optionBText.text = "B - " + texto;
    }

    private void OcultarBotones()
    {
        if (optionYButton != null)
            optionYButton.gameObject.SetActive(false);

        if (optionAButton != null)
            optionAButton.gameObject.SetActive(false);

        if (optionBButton != null)
            optionBButton.gameObject.SetActive(false);
    }

    private void FinalizarConversacion()
    {
        if (salirButton != null)
            salirButton.gameObject.SetActive(true);
    }

    public void CerrarDialogo()
    {
        dialogoActivo = false;
        escribiendo = false;

        if (rutinaDialogo != null)
            StopCoroutine(rutinaDialogo);

        if (audioSource != null)
            audioSource.Stop();

        OcultarBotones();

        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        if (salirButton != null)
            salirButton.gameObject.SetActive(false);

        if (movimientoJugador != null)
            movimientoJugador.puedeMoverse = true;
    }
}