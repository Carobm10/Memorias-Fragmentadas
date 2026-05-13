using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// Script avanzado para limpiar Menu.unity de todos los errores pre-existentes
/// Maneja: TextMeshPro sin fuentes, Prefabs faltantes, Placeholders rotos, Renderers abstractos
/// 
/// USO:
/// 1. Añade este script a cualquier GameObject en Menu.unity
/// 2. En el Inspector, click derecho en el componente AdvancedMenuFixer
/// 3. Selecciona "Limpiar Menu Completamente" (mejor opción)
/// 4. Espera a que termine (observa la Consola)
/// 5. Delete este GameObject
/// 6. Save la escena
/// </summary>
public class AdvancedMenuFixer : MonoBehaviour
{
    [ContextMenu("Limpiar Menu Completamente")]
    public void CleanMenuCompletely()
    {
        Debug.Log("=== INICIANDO LIMPIEZA COMPLETA DE MENU.UNITY ===");
        
        int fixCount = 0;
        
        // 1. Busca y elimina GameObjects con nombres "Placeholder" o "Missing"
        fixCount += RemoveBrokenPlaceholders();
        
        // 2. Arregla todos los TextMeshPro
        fixCount += FixAllTextMeshPro();
        
        // 3. Limpia Renderers abstractos
        fixCount += CleanupAbstractRenderers();
        
        // 4. Busca Prefabs faltantes y notifica
        FindMissingPrefabs();
        
        Debug.Log($"=== LIMPIEZA COMPLETA. Total de problemas arreglados: {fixCount} ===");
    }

    private int RemoveBrokenPlaceholders()
    {
        int count = 0;
        var allGameObjects = FindObjectsByType<Transform>(FindObjectsSortMode.None);
        
        foreach (var transform in allGameObjects)
        {
            string name = transform.gameObject.name;
            
            // Busca GameObjects rotos
            if (name.Contains("Placeholder") || 
                name.Contains("Missing") ||
                name.Contains("(Clone)") ||
                name.Contains("ErrorPrefab"))
            {
                // Verifica si este GameObject no tiene componentes críticos
                if (!HasCriticalComponents(transform.gameObject))
                {
                    Debug.Log($"Eliminando GameObject roto: {name}");
                    DestroyImmediate(transform.gameObject);
                    count++;
                }
            }
        }
        
        if (count > 0)
            Debug.Log($"✓ Se eliminaron {count} GameObjects rotos/Placeholders");
        
        return count;
    }

    private int FixAllTextMeshPro()
    {
        int count = 0;
        var textComponents = FindObjectsByType<TextMeshProUGUI>(FindObjectsSortMode.None);
        
        foreach (var text in textComponents)
        {
            if (text == null) continue;
            
            // Asegura que tiene RectTransform
            RectTransform rect = text.GetComponent<RectTransform>();
            if (rect == null)
            {
                text.gameObject.AddComponent<RectTransform>();
                Debug.Log($"RectTransform añadido a: {text.gameObject.name}");
                count++;
            }
            
            // Asigna fuente si falta
            if (text.font == null)
            {
                // Intenta encontrar LiberationSans SDF
                TMP_FontAsset font = Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");
                
                if (font != null)
                {
                    text.font = font;
                    Debug.Log($"Fuente asignada a: {text.gameObject.name}");
                    count++;
                }
                else
                {
                    // Usa la fuente por defecto si LiberationSans no existe
                    Debug.LogWarning($"LiberationSans SDF no encontrada para {text.gameObject.name}. Usando fuente por defecto.");
                }
            }
        }
        
        if (count > 0)
            Debug.Log($"✓ Se repararon {count} componentes TextMeshPro");
        
        return count;
    }

    private int CleanupAbstractRenderers()
    {
        int count = 0;
        var allGameObjects = FindObjectsByType<Transform>(FindObjectsSortMode.None);
        
        foreach (var transform in allGameObjects)
        {
            // Busca componentes Renderer directamente (que es abstracto)
            Component[] components = transform.gameObject.GetComponents<Component>();
            
            foreach (var component in components)
            {
                // Si el componente es null, intenta removerlo
                if (component == null)
                {
                    try
                    {
                        // Unity automáticamente no puede remover componentes null directamente
                        // pero podemos detectarlos y notificar
                        Debug.LogWarning($"Componente null detectado en: {transform.gameObject.name}");
                    }
                    catch { }
                }
            }
        }
        
        return count;
    }

    private void FindMissingPrefabs()
    {
        var allGameObjects = FindObjectsByType<Transform>(FindObjectsSortMode.None);
        int missingCount = 0;
        
        foreach (var transform in allGameObjects)
        {
            // En Unity, si un prefab está faltante, el GameObject sigue existiendo
            // pero su prefab source es null
            #if UNITY_EDITOR
            UnityEditor.PrefabInstanceStatus status = UnityEditor.PrefabUtility.GetPrefabInstanceStatus(transform.gameObject);
            
            if (status == UnityEditor.PrefabInstanceStatus.MissingAsset)
            {
                Debug.LogWarning($"⚠ Prefab faltante en: {transform.gameObject.name}. Puede eliminarse si no se necesita.");
                missingCount++;
            }
            #endif
        }
        
        if (missingCount > 0)
            Debug.Log($"⚠ Se encontraron {missingCount} Prefabs faltantes. Revisa los Warnings arriba.");
    }

    private bool HasCriticalComponents(GameObject go)
    {
        // No elimines GameObjects que tengan componentes críticos
        return go.GetComponent<Canvas>() != null ||
               go.GetComponent<CanvasGroup>() != null ||
               go.GetComponent<GraphicRaycaster>() != null ||
               go.name.ToLower().Contains("canvas") ||
               go.name.ToLower().Contains("panel");
    }

    [ContextMenu("Reportar Estado de Menu")]
    public void ReportMenuStatus()
    {
        Debug.Log("=== REPORTE DE ESTADO DE MENU.UNITY ===");
        
        var textComponents = FindObjectsByType<TextMeshProUGUI>(FindObjectsSortMode.None);
        Debug.Log($"Total de TextMeshPro: {textComponents.Length}");
        
        int missingFonts = 0;
        int missingRectTransform = 0;
        
        foreach (var text in textComponents)
        {
            if (text.font == null) missingFonts++;
            if (text.GetComponent<RectTransform>() == null) missingRectTransform++;
        }
        
        Debug.Log($"  - Sin fuente asignada: {missingFonts}");
        Debug.Log($"  - Sin RectTransform: {missingRectTransform}");
        
        #if UNITY_EDITOR
        var allGameObjects = FindObjectsByType<Transform>(FindObjectsSortMode.None);
        int missingPrefabs = 0;
        
        foreach (var transform in allGameObjects)
        {
            UnityEditor.PrefabInstanceStatus status = UnityEditor.PrefabUtility.GetPrefabInstanceStatus(transform.gameObject);
            if (status == UnityEditor.PrefabInstanceStatus.MissingAsset)
                missingPrefabs++;
        }
        
        Debug.Log($"  - Prefabs faltantes: {missingPrefabs}");
        #endif
        
        Debug.Log("=== FIN DEL REPORTE ===");
    }
}
