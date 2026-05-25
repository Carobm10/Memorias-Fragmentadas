using UnityEngine;
using TMPro;
using System.Collections;

public class RosaFinalDialogue : MonoBehaviour
{
    [Header("Estado")]
    public bool dialogoDisponible = false;

    private bool mirando = false;
    private bool dialogoActivo = false;
    private bool escribiendo = false;
    private bool mostrandoOpciones = false;

    [Header("Máquina de escribir")]
    public float tiempoEntreLetras = 0.045f;

    [Header("Prompt mirar Rosa")]
    public GameObject promptPanel;
    public TMP_Text promptText;

    [Header("Panel diálogo")]
    public GameObject dialoguePanel;
    public TMP_Text npcNameText;
    public TMP_Text dialogueText;
    public string nombreNPC = "Rosa";

    [Header("Opciones")]
    public GameObject opcionesPanel;
    public TMP_Text opcionAText;
    public TMP_Text opcionBText;
    public TMP_Text opcionYText;

    [Header("Salir")]
    public GameObject salirPanel;
    public TMP_Text salirText;

    private int nodoActual = 0;
    private int fraseActual = 0;
    private string[] frasesActuales;
    private string textoCompletoActual = "";

    void Start()
    {
        OcultarTodo();
    }

    void Update()
    {
        if (!dialogoDisponible) return;

        if (mirando && !dialogoActivo)
        {
            MostrarPrompt("Presiona A para hablar con Rosa");

            if (InputManagerCustom.PressA())
                IniciarDialogo();
        }

        if (!dialogoActivo) return;

        if (InputManagerCustom.PressX())
        {
            SalirDialogo();
            return;
        }

        if (escribiendo && InputManagerCustom.PressB())
        {
            StopAllCoroutines();
            dialogueText.text = textoCompletoActual;
            escribiendo = false;
            return;
        }

        if (!escribiendo && !mostrandoOpciones && InputManagerCustom.PressB())
        {
            SiguienteFrase();
            return;
        }

        if (mostrandoOpciones)
        {
            if (InputManagerCustom.PressA()) ElegirOpcion(1);
            if (InputManagerCustom.PressB()) ElegirOpcion(2);
            if (InputManagerCustom.PressY()) ElegirOpcion(3);
        }
    }

    public void SetLookingAtMe(bool value)
    {
        mirando = value;

        if (!mirando && !dialogoActivo)
            OcultarPrompt();
    }

    public void ActivarDialogoFinal()
    {
        dialogoDisponible = true;
        Debug.Log("ROSA FINAL: diálogo final activado.");
    }

    void IniciarDialogo()
    {
        dialogoActivo = true;
        nodoActual = 0;

        OcultarPrompt();

        if (dialoguePanel != null) dialoguePanel.SetActive(true);
        if (npcNameText != null) npcNameText.text = nombreNPC;
        if (salirPanel != null) salirPanel.SetActive(true);
        if (salirText != null) salirText.text = "X Salir";

        MostrarBloque(
            new string[]
            {
                "Así sí, mijo...",
                "Con musiquita, la casa se siente menos sola.",
                "Hasta el oficio se hace distinto cuando suena una canción."
            },
            "A: ¿A usted le gusta esta canción?",
            "B: ¿Por qué la radio estaba sin pilas?",
            "Y: ¿Usted conoce mucho esta casa?"
        );
    }

    void MostrarBloque(string[] frases, string opA, string opB, string opY)
    {
        frasesActuales = frases;
        fraseActual = 0;
        mostrandoOpciones = false;

        if (opcionesPanel != null) opcionesPanel.SetActive(false);

        if (opcionAText != null) opcionAText.text = opA;
        if (opcionBText != null) opcionBText.text = opB;
        if (opcionYText != null) opcionYText.text = opY;

        MostrarFraseActual();
    }

    void MostrarFraseActual()
    {
        if (frasesActuales == null || frasesActuales.Length == 0) return;

        if (fraseActual >= frasesActuales.Length)
        {
            MostrarOpciones();
            return;
        }

        textoCompletoActual = frasesActuales[fraseActual];

        StopAllCoroutines();
        StartCoroutine(EscribirTexto(textoCompletoActual));
    }

    IEnumerator EscribirTexto(string texto)
    {
        escribiendo = true;

        if (dialogueText != null)
            dialogueText.text = "";

        for (int i = 0; i < texto.Length; i++)
        {
            if (dialogueText != null)
                dialogueText.text += texto[i];

            yield return new WaitForSeconds(tiempoEntreLetras);
        }

        escribiendo = false;
    }

    void SiguienteFrase()
    {
        fraseActual++;

        if (fraseActual >= frasesActuales.Length)
            MostrarOpciones();
        else
            MostrarFraseActual();
    }

    void MostrarOpciones()
    {
        mostrandoOpciones = true;

        if (opcionesPanel != null)
            opcionesPanel.SetActive(true);
    }

    void ElegirOpcion(int opcion)
    {
        mostrandoOpciones = false;

        if (opcionesPanel != null)
            opcionesPanel.SetActive(false);

        if (nodoActual == 0)
        {
            if (opcion == 1)
            {
                nodoActual = 1;
                MostrarBloque(
                    new string[]
                    {
                        "Claro que me gusta.",
                        "Esa la ponían mucho antes, cuando uno hacía oficio.",
                        "La mañana se iba rapidito entre escoba, trapo y canción."
                    },
                    "A: A mí me trae recuerdos.",
                    "B: Me parece triste.",
                    "Y: Quiero seguir escuchando."
                );
            }
            else if (opcion == 2)
            {
                nodoActual = 2;
                MostrarBloque(
                    new string[]
                    {
                        "Ay, eso pasa en esta casa...",
                        "Las cosas se pierden, se guardan, se olvidan.",
                        "Y después nadie sabe bien dónde quedaron."
                    },
                    "A: ¿Como si la casa también durmiera?",
                    "B: Yo pensé que estaba dañada.",
                    "Y: Menos mal funcionó."
                );
            }
            else if (opcion == 3)
            {
                nodoActual = 3;
                MostrarBloque(
                    new string[]
                    {
                        "Más de lo que parece, mijo.",
                        "Una casa también guarda secretos.",
                        "Uno aprende a caminar suave entre ellos."
                    },
                    "A: ¿Y usted recuerda a mi familia?",
                    "B: ¿La casa siempre fue así?",
                    "Y: ¿Qué debería mirar ahora?"
                );
            }

            return;
        }

        if (nodoActual == 1)
        {
            if (opcion == 1) MostrarFinal("Entonces escúchela bien. Los recuerdos a veces llegan bajito.");
            if (opcion == 2) MostrarFinal("No toda tristeza es mala. A veces solo avisa que algo fue importante.");
            if (opcion == 3) MostrarFinal("Haga eso. Quédese un ratico. No todo hay que resolverlo corriendo.");
            return;
        }

        if (nodoActual == 2)
        {
            if (opcion == 1) MostrarFinal("A veces sí. Y a veces uno despierta una parte con cosas pequeñas.");
            if (opcion == 2) MostrarFinal("No todo lo que no responde está dañado, mijo.");
            if (opcion == 3) MostrarFinal("Sí. A veces solo hace falta paciencia.");
            return;
        }

        if (nodoActual == 3)
        {
            if (opcion == 1) MostrarFinal("Algunas cosas sí. Otras es mejor que usted mismo las encuentre.");
            if (opcion == 2) MostrarFinal("No. Antes sonaba más. Había más pasos, más voces, más vida.");
            if (opcion == 3) MostrarFinal("Mire con calma. A veces los objetos hablan sin hacer ruido.");
            return;
        }

        if (nodoActual == 99)
        {
            if (opcion == 1)
                IniciarDialogo();
            else
                SalirDialogo();
        }
    }

    void MostrarFinal(string texto)
    {
        nodoActual = 99;

        MostrarBloque(
            new string[] { texto },
            "A: Volver a preguntar",
            "B: Seguir escuchando",
            "Y: Salir"
        );
    }

    void SalirDialogo()
    {
        dialogoActivo = false;
        escribiendo = false;
        mostrandoOpciones = false;
        nodoActual = 0;
        fraseActual = 0;

        StopAllCoroutines();
        OcultarTodo();
    }

    void MostrarPrompt(string texto)
    {
        if (promptPanel != null) promptPanel.SetActive(true);
        if (promptText != null) promptText.text = texto;
    }

    void OcultarPrompt()
    {
        if (promptPanel != null) promptPanel.SetActive(false);
    }

    void OcultarTodo()
    {
        OcultarPrompt();

        if (dialoguePanel != null) dialoguePanel.SetActive(false);
        if (opcionesPanel != null) opcionesPanel.SetActive(false);
        if (salirPanel != null) salirPanel.SetActive(false);
    }
}