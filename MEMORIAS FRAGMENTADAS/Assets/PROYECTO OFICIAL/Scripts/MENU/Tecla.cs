using UnityEngine;
using UnityEngine.SceneManagement;

public class Tecla : MonoBehaviour
{
    [SerializeField] private string letra;

    private Renderer rend;
    private Color colorOriginal;
    private SceneTransitionManager transitionManager;

    void Start()
    {
        rend = GetComponent<Renderer>();

        if (rend != null)
        {
            colorOriginal = rend.material.color;
        }

        // Obtener el SceneTransitionManager
        transitionManager = FindFirstObjectByType<SceneTransitionManager>();
        if (transitionManager == null)
        {
            Debug.LogWarning("No se encontró SceneTransitionManager. Se creará uno automáticamente.");
            GameObject managerGO = new GameObject("SceneTransitionManager");
            transitionManager = managerGO.AddComponent<SceneTransitionManager>();
        }
    }

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

    public void Presionar()
    {
        Debug.Log("Tecla presionada: " + letra);

        EscrituraPapel manager = Object.FindFirstObjectByType<EscrituraPapel>();

        if (manager == null)
        {
            Debug.LogWarning("No se encontró EscrituraPapel en la escena");
            return;
        }

        if (letra == "BORRAR")
        {
            manager.NuevaHoja();
            return;
        }

        if (letra == "ENVIAR")
        {
            string texto = manager.ObtenerTexto().ToUpper();

            if (texto == "JUGAR")
            {
                // Usar el SceneTransitionManager para cargar la siguiente escena
                if (transitionManager != null)
                {
                    transitionManager.LoadNextScene();
                }
                else
                {
                    SceneManager.LoadScene("Escena_VideoIntro");
                }
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

        manager.Escribir(letra);
    }
}