using UnityEngine;
using System.Collections;

public class TypewriterLever : MonoBehaviour
{
    [Header("Renderers")]
    public Renderer[] renderers;

    [Header("Color")]
    public Color highlightColor = new Color(0.1f, 1f, 0.25f, 1f);

    private Color[] originalColors;
    private Quaternion originalRotation;

    private void Awake()
    {
        if (renderers == null || renderers.Length == 0)
        {
            renderers = GetComponentsInChildren<Renderer>();
        }

        originalColors = new Color[renderers.Length];

        for (int i = 0; i < renderers.Length; i++)
        {
            originalColors[i] = renderers[i].material.color;
        }

        originalRotation = transform.localRotation;
    }

    public void SetHighlight(bool active)
    {
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].material.color = active ? highlightColor : originalColors[i];
        }
    }

    public IEnumerator PullLever(float angle = 25f, float speed = 0.15f)
    {
        Quaternion pulledRotation = originalRotation * Quaternion.Euler(angle, 0f, 0f);

        float t = 0f;

        while (t < speed)
        {
            t += Time.deltaTime;
            transform.localRotation = Quaternion.Slerp(originalRotation, pulledRotation, t / speed);
            yield return null;
        }

        t = 0f;

        while (t < speed)
        {
            t += Time.deltaTime;
            transform.localRotation = Quaternion.Slerp(pulledRotation, originalRotation, t / speed);
            yield return null;
        }

        transform.localRotation = originalRotation;
    }
}