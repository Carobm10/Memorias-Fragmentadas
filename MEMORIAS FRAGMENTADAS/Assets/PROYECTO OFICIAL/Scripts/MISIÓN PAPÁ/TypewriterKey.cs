using UnityEngine;
using System.Collections;

public class TypewriterKey : MonoBehaviour
{
    [Header("Letra de esta tecla")]
    public string keyValue;

    [Header("Color")]
    public Color highlightColor = new Color(0.1f, 1f, 0.25f, 1f);

    private Renderer keyRenderer;
    private Color originalColor;
    private Vector3 originalLocalPosition;

    private void Awake()
    {
        if (string.IsNullOrEmpty(keyValue))
        {
            keyValue = gameObject.name.ToUpper();
        }

        keyRenderer = GetComponent<Renderer>();

        if (keyRenderer != null && keyRenderer.material.HasProperty("_Color"))
        {
            originalColor = keyRenderer.material.color;
        }

        originalLocalPosition = transform.localPosition;
    }

    public void SetHighlight(bool active)
    {
        if (keyRenderer == null) return;

        if (keyRenderer.material.HasProperty("_Color"))
        {
            keyRenderer.material.color = active ? highlightColor : originalColor;
        }
    }

    public IEnumerator Press(float distance, float speed)
    {
        Vector3 downPos = originalLocalPosition + Vector3.down * distance;

        float t = 0f;

        while (t < speed)
        {
            t += Time.deltaTime;
            transform.localPosition = Vector3.Lerp(originalLocalPosition, downPos, t / speed);
            yield return null;
        }

        t = 0f;

        while (t < speed)
        {
            t += Time.deltaTime;
            transform.localPosition = Vector3.Lerp(downPos, originalLocalPosition, t / speed);
            yield return null;
        }

        transform.localPosition = originalLocalPosition;
    }
}