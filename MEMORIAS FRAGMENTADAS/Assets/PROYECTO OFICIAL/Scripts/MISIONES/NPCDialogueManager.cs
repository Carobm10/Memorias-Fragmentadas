using System.Collections;
using UnityEngine;
using TMPro;

public class NPCDialogueManager : MonoBehaviour
{
    [Header("Botón siguiente")]
    public GameObject botonSiguienteY;
    public static NPCDialogueManager Instance;

    [Header("Panel principal")]
    public GameObject panelDialogoNPC;

    [Header("Textos")]
    public TextMeshProUGUI textoNombreNPC;
    public TextMeshProUGUI textoDialogo;

    [Header("Opciones")]
    public GameObject opcionY;
    public GameObject opcionA;
    public GameObject opcionB;

    public TextMeshProUGUI textoOpcionY;
    public TextMeshProUGUI textoOpcionA;
    public TextMeshProUGUI textoOpcionB;

    [Header("Botón salir")]
    public GameObject botonSalirX;

    [Header("Efecto escritura")]
    public float velocidadEscritura = 0.035f;

    [Header("Debug")]
    public bool mostrarDebug = true;

    private string[] lineasActuales;
    private int indiceLinea = 0;
    private bool dialogoActivo = false;
    private bool escribiendo = false;
    private Coroutine escrituraActual;

    private System.Action opcionYAction;
    private System.Action opcionAAction;
    private System.Action opcionBAction;

    private void Awake()
    {
        Instance = this;

        if (panelDialogoNPC != null)
            panelDialogoNPC.SetActive(false);

        OcultarOpciones();
        if (botonSiguienteY != null)
            botonSiguienteY.SetActive(false);
    }

    private void Update()
    {
        if (!dialogoActivo) return;

        if (Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.JoystickButton3))
        {
            CerrarDialogo();
            return;
        }

        if (opcionY.activeSelf || opcionA.activeSelf || opcionB.activeSelf)
        {
            RevisarOpciones();
            return;
        }

        if (Input.GetKeyDown(KeyCode.Y) || Input.GetKeyDown(KeyCode.JoystickButton4))
        {
            if (escribiendo)
            {
                CompletarLineaInstantanea();
            }
            else
            {
                SiguienteLinea();
            }
        }
    }

    public void MostrarDialogoSimple(string nombreNPC, string[] lineas)
    {
        if (lineas == null || lineas.Length == 0) return;

        dialogoActivo = true;
        lineasActuales = lineas;
        indiceLinea = 0;

        if (panelDialogoNPC != null)
            panelDialogoNPC.SetActive(true);

        if (textoNombreNPC != null)
            textoNombreNPC.text = nombreNPC;

        OcultarOpciones();
        if (botonSiguienteY != null)
            botonSiguienteY.SetActive(true);
        MostrarLineaActual();

        if (mostrarDebug)
            Debug.Log("[NPCDialogueManager] Iniciando diálogo simple con: " + nombreNPC);
    }

    public void MostrarDialogoConOpciones(
        string nombreNPC,
        string linea,
        string opcionYTexto,
        string opcionATexto,
        string opcionBTexto,
        System.Action accionY,
        System.Action accionA,
        System.Action accionB)
    {
        dialogoActivo = true;
        lineasActuales = new string[] { linea };
        indiceLinea = 0;

        opcionYAction = accionY;
        opcionAAction = accionA;
        opcionBAction = accionB;

        if (panelDialogoNPC != null)
            panelDialogoNPC.SetActive(true);

        if (textoNombreNPC != null)
            textoNombreNPC.text = nombreNPC;

        if (textoOpcionY != null) textoOpcionY.text = "Y - " + opcionYTexto;
        if (textoOpcionA != null) textoOpcionA.text = "A - " + opcionATexto;
        if (textoOpcionB != null) textoOpcionB.text = "B - " + opcionBTexto;
        if (botonSiguienteY != null)
            botonSiguienteY.SetActive(false);

        MostrarLineaActual();

        StartCoroutine(MostrarOpcionesCuandoTermineTexto());

        if (mostrarDebug)
            Debug.Log("[NPCDialogueManager] Iniciando diálogo con opciones: " + nombreNPC);
    }

    private void MostrarLineaActual()
    {
        if (escrituraActual != null)
            StopCoroutine(escrituraActual);

        escrituraActual = StartCoroutine(EscribirTexto(lineasActuales[indiceLinea]));
    }

    private IEnumerator EscribirTexto(string linea)
    {
        if (botonSiguienteY != null)
            botonSiguienteY.SetActive(false);
                
        escribiendo = true;

        if (textoDialogo != null)
            textoDialogo.text = "";

        foreach (char letra in linea)
        {
            if (textoDialogo != null)
                textoDialogo.text += letra;

            yield return new WaitForSeconds(velocidadEscritura);
        }

        escribiendo = false;
        if (!opcionY.activeSelf &&
            !opcionA.activeSelf &&
            !opcionB.activeSelf)
        {
            if (botonSiguienteY != null)
                botonSiguienteY.SetActive(true);
        }
    }

    private void CompletarLineaInstantanea()
    {
        if (escrituraActual != null)
            StopCoroutine(escrituraActual);

        if (textoDialogo != null)
            textoDialogo.text = lineasActuales[indiceLinea];

        escribiendo = false;

        if (!opcionY.activeSelf &&
            !opcionA.activeSelf &&
            !opcionB.activeSelf)
        {
            if (botonSiguienteY != null)
                botonSiguienteY.SetActive(true);
        }
    }

    private void SiguienteLinea()
    {
        indiceLinea++;

        if (indiceLinea >= lineasActuales.Length)
        {
            CerrarDialogo();
            if (botonSiguienteY != null)
                botonSiguienteY.SetActive(false);
            return;
        }

        MostrarLineaActual();
    }

    private IEnumerator MostrarOpcionesCuandoTermineTexto()
    {
        while (escribiendo)
            yield return null;

        MostrarOpciones();
    }

    private void MostrarOpciones()
    {
        if (botonSiguienteY != null)
            botonSiguienteY.SetActive(false);

        if (opcionY != null) opcionY.SetActive(true);
        if (opcionA != null) opcionA.SetActive(true);
        if (opcionB != null) opcionB.SetActive(true);
    }

    private void OcultarOpciones()
    {
        if (opcionY != null) opcionY.SetActive(false);
        if (opcionA != null) opcionA.SetActive(false);
        if (opcionB != null) opcionB.SetActive(false);
    }

    private void RevisarOpciones()
    {
        if (Input.GetKeyDown(KeyCode.Y) || Input.GetKeyDown(KeyCode.JoystickButton4))
        {
            opcionYAction?.Invoke();
        }

        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.JoystickButton11))
        {
            opcionAAction?.Invoke();
        }

        if (Input.GetKeyDown(KeyCode.B) || Input.GetKeyDown(KeyCode.JoystickButton7))
        {
            opcionBAction?.Invoke();
        }
    }

    public void CerrarDialogo()
    {
        dialogoActivo = false;
        escribiendo = false;

        if (escrituraActual != null)
            StopCoroutine(escrituraActual);

        OcultarOpciones();

        if (panelDialogoNPC != null)
            panelDialogoNPC.SetActive(false);
    }

    public bool EstaActivo()
    {
        return dialogoActivo;
    }
}