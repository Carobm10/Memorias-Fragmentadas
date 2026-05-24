using UnityEngine;
using UnityEngine.SceneManagement;

public class Tecla : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private string letra;

    [Header("Opciones 3D")]
    [SerializeField] private GameObject opcionJugar;
    [SerializeField] private GameObject opcionAjustes;

    private Renderer rend;
    private Color colorOriginal;

    void Start()
    {
        rend = GetComponent<Renderer>();

        if (rend != null)
        {
            colorOriginal = rend.material.color;
        }

        // Ocultar opciones al iniciar
        if (opcionJugar != null)
            opcionJugar.SetActive(false);

        if (opcionAjustes != null)
            opcionAjustes.SetActive(false);
    }

    public void Presionar()
    {
        Debug.Log("Tecla presionada: " + letra);

        EscrituraPapel manager = FindObjectOfType<EscrituraPapel>();

        if (manager == null)
        {
            Debug.LogWarning("No se encontró EscrituraPapel");
            return;
        }

        // ========================================
        // BORRAR
        // ========================================

        if (letra == "BORRAR")
        {
            manager.NuevaHoja();

            if (rend != null)
                rend.material.color = Color.red;

            return;
        }

        // ========================================
        // ENVIAR
        // ========================================

        if (letra == "ENVIAR")
        {
            string texto = manager.ObtenerTexto().ToUpper();

            if (texto == "JUGAR")
            {
                SceneManager.LoadScene("EscenaJuego");
            }
            else if (texto == "AJUSTES")
            {
                Debug.Log("Abrir ajustes");
            }
            else
            {
                Debug.Log("Palabra no válida");
            }

            return;
        }

        // ========================================
        // OPCIONES
        // ========================================

        if (letra == "OPCIONES")
        {
            if (opcionJugar != null)
                opcionJugar.SetActive(true);

            if (opcionAjustes != null)
                opcionAjustes.SetActive(true);

            return;
        }

        // ========================================
        // TECLAS NORMALES
        // ========================================

        manager.Escribir(letra);
    }

    // ========================================
    // HOVER
    // ========================================

    public void Seleccionar()
    {
        if (rend != null)
        {
            rend.material.color = Color.green;
        }
    }

    public void Deseleccionar()
    {
        if (rend != null)
        {
            rend.material.color = colorOriginal;
        }
    }
}