using UnityEngine;
using System.Collections;

public class TypewriterKey : MonoBehaviour
{
    [Header("Letra de esta tecla")]
    public string keyValue;

    [Header("Color")]
    public Color highlightColor = new Color(0.1f, 1f, 0.25f, 1f);

    private Renderer[] renderers;
    private Color[] originalColors;
    private Vector3 originalLocalPosition;

    private void Awake()
    {
        keyValue = gameObject.name.ToUpper();

        renderers = GetComponentsInChildren<Renderer>();
        originalColors = new Color[renderers.Length];

        for (int i = 0; i < renderers.Length; i++)
        {
            originalColors[i] = renderers[i].material.color;
        }

        originalLocalPosition = transform.localPosition;
    }

    public void SetHighlight(bool active)
    {
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].material.color = active ? highlightColor : originalColors[i];
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