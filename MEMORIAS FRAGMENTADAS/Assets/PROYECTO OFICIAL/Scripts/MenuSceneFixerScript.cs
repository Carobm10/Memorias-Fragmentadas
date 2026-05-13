using UnityEngine;
using TMPro;

/// <summary>
/// Script para reparar errores de TextMeshPro en Menu.unity
/// Ejecuta esto una sola vez, luego puedes eliminarlo
/// </summary>
public class MenuSceneFixerScript : MonoBehaviour
{
    [ContextMenu("Arreglar todos los TextMeshPro")]
    public void FixAllTextMeshPro()
    {
        // Buscar todos los TextMeshPro en la escena
        TextMeshPro[] tmpTexts = FindObjectsByType<TextMeshPro>(FindObjectsSortMode.None);
        
        int fixed_count = 0;
        
        foreach (TextMeshPro tmpText in tmpTexts)
        {
            // Asignar una fuente por defecto si no tiene
            if (tmpText.font == null)
            {
                // Intenta cargar LiberationSans SDF
                TMP_FontAsset liberationFont = Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");
                
                if (liberationFont != null)
                {
                    tmpText.font = liberationFont;
                    Debug.Log($"✓ Fuente asignada a: {tmpText.gameObject.name}");
                    fixed_count++;
                }
                else
                {
                    Debug.LogWarning($"✗ No se encontró LiberationSans SDF para: {tmpText.gameObject.name}");
                }
            }
            
            // Asegurarse de que tiene RectTransform (para UI)
            if (tmpText.gameObject.GetComponent<RectTransform>() == null)
            {
                tmpText.gameObject.AddComponent<RectTransform>();
                Debug.Log($"✓ RectTransform añadido a: {tmpText.gameObject.name}");
            }
        }
        
        Debug.Log($"\n=== REPARACIÓN COMPLETADA ===\nTextMeshPro solucionados: {fixed_count}");
    }
    
    [ContextMenu("Limpiar Renderer Abstracto")]
    public void CleanupAbstractRenderer()
    {
        // Buscar todos los TextMeshPro y remover Renderer si lo hay
        TextMeshPro[] tmpTexts = FindObjectsByType<TextMeshPro>(FindObjectsSortMode.None);
        
        foreach (TextMeshPro tmpText in tmpTexts)
        {
            // Los Renderer abstractos causen problemas
            // Solo necesitamos MeshRenderer o CanvasRenderer
            Renderer[] renderers = tmpText.GetComponents<Renderer>();
            
            foreach (Renderer renderer in renderers)
            {
                if (renderer.GetType().Name == "Renderer")
                {
                    DestroyImmediate(renderer);
                    Debug.Log($"✓ Renderer abstracto removido de: {tmpText.gameObject.name}");
                }
            }
        }
    }
}
