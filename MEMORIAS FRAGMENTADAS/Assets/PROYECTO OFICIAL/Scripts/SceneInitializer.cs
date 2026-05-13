using UnityEngine;

/// <summary>
/// Inicializador de escenas que crea la UI de debug automáticamente.
/// Debe asignarse a un GameObject en cada escena.
/// </summary>
public class SceneInitializer : MonoBehaviour
{
    [SerializeField] private bool createDebugNavigator = true;
    [SerializeField] private bool debugMode = true;

    void Awake()
    {
        if (debugMode && createDebugNavigator)
        {
            // Crear la UI de navegación de debug
            SceneDebugNavigator.CreateDebugNavigatorUI();
            Debug.Log($"SceneInitializer: UI de debug creada en '{gameObject.scene.name}'");
        }
    }
}
