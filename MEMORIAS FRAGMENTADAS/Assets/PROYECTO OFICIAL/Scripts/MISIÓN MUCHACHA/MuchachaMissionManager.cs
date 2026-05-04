using UnityEngine;
using TMPro;

public class MuchachaMissionManager : MonoBehaviour
{
    [Header("UI Diálogo")]
    public GameObject panelDialogo;
    public TextMeshProUGUI textoNombre;
    public TextMeshProUGUI textoDialogo;

    [Header("UI Objetivo")]
    public GameObject panelObjetivo;
    public TextMeshProUGUI textoObjetivo;

    [Header("Diálogo inicial")]
    public string nombreNPC = "Muchacha";

    [TextArea(2, 4)]
    public string[] dialogoInicial;

    [Header("Estado")]
    public bool misionIniciada = false;
    public bool dialogoActivo = false;
    public bool dialogoCompletado = false;

    private int indiceDialogo = 0;

    private void Start()
    {
        if (panelDialogo != null)
            panelDialogo.SetActive(false);

        if (panelObjetivo != null)
            panelObjetivo.SetActive(false);
    }

    public void ActivarMision()
    {
        if (misionIniciada) return;

        misionIniciada = true;
        IniciarDialogo();
    }

    public void AvanzarDialogo()
    {
        if (!dialogoActivo) return;

        indiceDialogo++;

        if (indiceDialogo >= dialogoInicial.Length)
        {
            FinalizarDialogoInicial();
            return;
        }

        MostrarLinea();
    }

    private void IniciarDialogo()
    {
        dialogoActivo = true;
        indiceDialogo = 0;

        if (panelDialogo != null)
            panelDialogo.SetActive(true);

        if (textoNombre != null)
            textoNombre.text = nombreNPC;

        MostrarLinea();
    }

    private void MostrarLinea()
    {
        if (textoDialogo != null && dialogoInicial.Length > 0)
            textoDialogo.text = dialogoInicial[indiceDialogo];
    }

    private void FinalizarDialogoInicial()
    {
        dialogoActivo = false;
        dialogoCompletado = true;

        if (panelDialogo != null)
            panelDialogo.SetActive(false);

        MostrarObjetivo("Busca el radio en la cocina.");
    }

    public void CerrarDialogo()
    {
        if (!dialogoActivo) return;

        dialogoActivo = false;

        if (panelDialogo != null)
            panelDialogo.SetActive(false);
    }

    public void MostrarObjetivo(string mensaje)
    {
        if (panelObjetivo != null)
            panelObjetivo.SetActive(true);

        if (textoObjetivo != null)
            textoObjetivo.text = mensaje;
    }
}