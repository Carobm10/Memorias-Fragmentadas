using UnityEngine;
using UnityEngine.SceneManagement;

public class Tecla : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private string letra;

    [Header("Opciones 3D")]
    [SerializeField] private GameObject opcionJugar;
    [SerializeField] private GameObject opcionAjustes;

    [Header("Prompt (se muestra al mirar la tecla OPCIONES)")]
    [SerializeField] private GameObject promptPanel;

    private Renderer rend;
    private Color colorOriginal;
    private bool opcionesVisibles = false;

    void Start()
    {
        rend = GetComponent<Renderer>();

        if (rend != null)
        {
            colorOriginal = rend.material.color;
        }

        if (opcionJugar != null)
            opcionJugar.SetActive(false);

        if (opcionAjustes != null)
            opcionAjustes.SetActive(false);

        if (promptPanel != null)
            promptPanel.SetActive(false);
    }

    public void Presionar()
    {
        EscrituraPapel manager = FindFirstObjectByType<EscrituraPapel>();

        if (letra == "BORRAR")
        {
            if (manager != null)
                manager.NuevaHoja();

            if (rend != null)
                rend.material.color = Color.red;

            return;
        }

        if (letra == "ENVIAR")
        {
            if (manager == null) return;

            string texto = manager.ObtenerTexto().ToUpper();

            if (texto == "JUGAR")
            {
                SceneManager.LoadScene("TutorialJoystick");
            }
            else if (texto == "AJUSTES")
            {
                Debug.Log("Abrir ajustes");
            }

            return;
        }

        if (letra == "OPCIONES")
        {
            opcionesVisibles = !opcionesVisibles;

            if (opcionJugar != null)
                opcionJugar.SetActive(opcionesVisibles);

            if (opcionAjustes != null)
                opcionAjustes.SetActive(opcionesVisibles);

            // Ocultar prompt al presionar
            if (promptPanel != null)
                promptPanel.SetActive(false);

            return;
        }

        // Teclas normales
        if (manager != null)
            manager.Escribir(letra);
    }

    public void Seleccionar()
    {
        if (rend != null)
            rend.material.color = Color.green;

        // Mostrar prompt solo para la tecla OPCIONES
        if (letra == "OPCIONES" && !opcionesVisibles && promptPanel != null)
            promptPanel.SetActive(true);
    }

    public void Deseleccionar()
    {
        if (rend != null)
            rend.material.color = colorOriginal;

        // Ocultar prompt al dejar de mirar
        if (letra == "OPCIONES" && promptPanel != null)
            promptPanel.SetActive(false);
    }
}