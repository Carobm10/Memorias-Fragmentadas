using UnityEngine;

/// <summary>
/// Script de inicialización global del proyecto.
/// Se debe colocar en un GameObject en la escena Menu.unity
/// </summary>
public class ProjectInitializer : MonoBehaviour
{
    [SerializeField] private bool createTransitionManager = true;

    void Awake()
    {
        if (createTransitionManager)
        {
            // Buscar si ya existe
            SceneTransitionManager existingManager = FindFirstObjectByType<SceneTransitionManager>();
            if (existingManager == null)
            {
                GameObject managerGO = new GameObject("SceneTransitionManager");
                SceneTransitionManager manager = managerGO.AddComponent<SceneTransitionManager>();
                DontDestroyOnLoad(managerGO);
                Debug.Log("ProjectInitializer: SceneTransitionManager creado");
            }
        }

        // Crear UI de debug para la escena actual
        SceneDebugNavigator.CreateDebugNavigatorUI();
        Debug.Log("ProjectInitializer: UI de debug creado para la escena inicial");

        // Este GameObject puede auto-destruirse después de inicializar
        // Ya no es necesario mantenerlo en memoria
        Destroy(gameObject);
    }
}
