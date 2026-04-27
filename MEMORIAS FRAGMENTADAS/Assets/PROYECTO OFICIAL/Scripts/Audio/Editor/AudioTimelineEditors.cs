#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

internal struct TimelineEntry
{
    public string label;
    public float start;
    public float end;
    public bool overlap;
    public bool infiniteLoop;
}

[CustomEditor(typeof(AudioScriptManager))]
public class AudioScriptManagerEditor : Editor
{
    private static float managerZoom = 2f;
    private static float managerOffset = 0f;

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        DrawDefaultInspector();
        DrawTimelineForManager();
        serializedObject.ApplyModifiedProperties();

        if (Application.isPlaying)
        {
            Repaint();
        }
    }

    private void DrawTimelineForManager()
    {
        SerializedProperty audiosProp = serializedObject.FindProperty("audiosScheduled");
        if (audiosProp == null)
        {
            return;
        }

        List<TimelineEntry> entries = new List<TimelineEntry>();

        for (int i = 0; i < audiosProp.arraySize; i++)
        {
            SerializedProperty audioProp = audiosProp.GetArrayElementAtIndex(i);
            SerializedProperty nombreProp = audioProp.FindPropertyRelative("nombre");
            SerializedProperty delayProp = audioProp.FindPropertyRelative("delay");
            SerializedProperty pitchProp = audioProp.FindPropertyRelative("pitch");
            SerializedProperty inicioProp = audioProp.FindPropertyRelative("reproducirAlInicio");
            SerializedProperty clipProp = audioProp.FindPropertyRelative("clip");
            SerializedProperty loopProp = audioProp.FindPropertyRelative("loop");
            SerializedProperty repeticionesProp = audioProp.FindPropertyRelative("repeticiones");

            string nombre = string.IsNullOrWhiteSpace(nombreProp.stringValue) ? $"Audio {i + 1}" : nombreProp.stringValue;
            AudioClip clip = clipProp.objectReferenceValue as AudioClip;
            float pitch = Mathf.Max(0.01f, Mathf.Abs(pitchProp.floatValue));
            bool loopInfinito = loopProp.boolValue;
            int repeticiones = Mathf.Max(1, repeticionesProp.intValue);
            float duracionBase = clip != null ? clip.length / pitch : 0f;
            float duracion = loopInfinito ? duracionBase * 4f : duracionBase * repeticiones;
            float inicio = inicioProp.boolValue ? 0f : Mathf.Max(0f, delayProp.floatValue);
            float fin = inicio + duracion;

            entries.Add(new TimelineEntry
            {
                label = nombre,
                start = inicio,
                end = fin,
                overlap = false,
                infiniteLoop = loopInfinito
            });
        }

        entries.Sort((a, b) => a.start.CompareTo(b.start));

        float previousEnd = -1f;
        for (int i = 0; i < entries.Count; i++)
        {
            TimelineEntry entry = entries[i];
            if (entry.start < previousEnd)
            {
                entry.overlap = true;
                entries[i] = entry;
            }

            if (entry.end > previousEnd)
            {
                previousEnd = entry.end;
            }
        }

        DrawTimelineSection("Vista de Línea de Tiempo - Manager", entries,
            "Start/End en segundos. Si aparece OVERLAP, ese audio se superpone con otro.");
    }

    private void DrawTimelineSection(string titulo, List<TimelineEntry> entries, string descripcion)
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField(titulo, EditorStyles.boldLabel);
        EditorGUILayout.HelpBox(descripcion, MessageType.Info);

        if (entries.Count == 0)
        {
            EditorGUILayout.HelpBox("No hay audios configurados para mostrar en la línea de tiempo.", MessageType.None);
            return;
        }

        float total = 0.1f;
        for (int i = 0; i < entries.Count; i++)
        {
            if (entries[i].end > total)
            {
                total = entries[i].end;
            }
        }

        managerZoom = EditorGUILayout.Slider("Zoom Timeline", managerZoom, 1f, 8f);
        float visibleWindow = Mathf.Max(0.5f, total / Mathf.Max(1f, managerZoom));
        float maxOffset = Mathf.Max(0f, total - visibleWindow);
        managerOffset = Mathf.Clamp(managerOffset, 0f, maxOffset);

        using (new EditorGUI.DisabledScope(maxOffset <= 0f))
        {
            managerOffset = EditorGUILayout.Slider("Desplazamiento (s)", managerOffset, 0f, maxOffset);
        }

        if (maxOffset <= 0f)
        {
            EditorGUILayout.HelpBox("Aumenta el Zoom Timeline para habilitar el desplazamiento horizontal.", MessageType.None);
        }

        Rect rulerRect = GUILayoutUtility.GetRect(EditorGUIUtility.currentViewWidth - 40f, 18f);
        Rect rect = GUILayoutUtility.GetRect(EditorGUIUtility.currentViewWidth - 40f, 24f);

        DibujarReglaSegundos(rulerRect, managerOffset, visibleWindow);
        EditorGUI.DrawRect(rect, new Color(0.18f, 0.18f, 0.18f, 1f));

        if (Application.isPlaying)
        {
            AudioScriptManager manager = target as AudioScriptManager;
            if (manager != null)
            {
                float playheadTime = manager.GetElapsedTime();
                EditorGUILayout.LabelField($"Tiempo actual: {FormatSeconds(playheadTime)}");

                if (playheadTime < managerOffset || playheadTime > managerOffset + visibleWindow)
                {
                    managerOffset = Mathf.Clamp(playheadTime - (visibleWindow * 0.5f), 0f, maxOffset);
                }

                float playheadX = rect.x + Mathf.Clamp01((playheadTime - managerOffset) / visibleWindow) * rect.width;
                EditorGUI.DrawRect(new Rect(playheadX - 1f, rect.y - 6f, 2f, rect.height + 12f), new Color(1f, 0.2f, 0.2f, 1f));
                GUI.Label(new Rect(playheadX + 4f, rect.y - 18f, 90f, 18f), $"▶ {FormatSeconds(playheadTime)}");
            }
        }

        for (int i = 0; i < entries.Count; i++)
        {
            TimelineEntry entry = entries[i];
            float xMin = rect.x + Mathf.Clamp01((entry.start - managerOffset) / visibleWindow) * rect.width;
            float xMax = rect.x + Mathf.Clamp01((entry.end - managerOffset) / visibleWindow) * rect.width;
            float width = Mathf.Max(2f, xMax - xMin);

            if (entry.end < managerOffset || entry.start > managerOffset + visibleWindow)
            {
                continue;
            }

            Rect segment = new Rect(xMin, rect.y + 3f, width, rect.height - 6f);

            Color color = entry.overlap
                ? new Color(1f, 0.45f, 0.2f, 1f)
                : new Color(0.2f, 0.75f, 0.9f, 1f);

            if (entry.infiniteLoop)
            {
                color = new Color(0.9f, 0.2f, 0.9f, 1f);
            }

            EditorGUI.DrawRect(segment, color);
        }

        EditorGUILayout.Space(2f);
        EditorGUILayout.LabelField($"Duración total estimada: {FormatSeconds(total)}");

        for (int i = 0; i < entries.Count; i++)
        {
            TimelineEntry entry = entries[i];
            string estado = entry.overlap ? "  [OVERLAP]" : string.Empty;
            string loop = entry.infiniteLoop ? "  [LOOP INFINITO]" : string.Empty;
            EditorGUILayout.LabelField($"{i + 1}. {entry.label}  |  {FormatSeconds(entry.start)} -> {FormatSeconds(entry.end)}{estado}{loop}");
        }
    }

    private void DibujarReglaSegundos(Rect rect, float offset, float visibleWindow)
    {
        EditorGUI.DrawRect(rect, new Color(0.14f, 0.14f, 0.14f, 1f));

        float step = ObtenerPasoAgradable(visibleWindow / 8f);
        float first = Mathf.Floor(offset / step) * step;

        for (float t = first; t <= offset + visibleWindow; t += step)
        {
            float normalized = Mathf.Clamp01((t - offset) / visibleWindow);
            float x = rect.x + normalized * rect.width;

            Rect tick = new Rect(x, rect.y + 8f, 1f, rect.height - 8f);
            EditorGUI.DrawRect(tick, new Color(0.7f, 0.7f, 0.7f, 0.8f));
            GUI.Label(new Rect(x + 2f, rect.y, 60f, rect.height), $"{t:0.0}s");
        }
    }

    private float ObtenerPasoAgradable(float value)
    {
        if (value <= 0f)
        {
            return 1f;
        }

        float exponent = Mathf.Floor(Mathf.Log10(value));
        float fraction = value / Mathf.Pow(10f, exponent);
        float niceFraction;

        if (fraction <= 1f)
        {
            niceFraction = 1f;
        }
        else if (fraction <= 2f)
        {
            niceFraction = 2f;
        }
        else if (fraction <= 5f)
        {
            niceFraction = 5f;
        }
        else
        {
            niceFraction = 10f;
        }

        return niceFraction * Mathf.Pow(10f, exponent);
    }

    private string FormatSeconds(float seconds)
    {
        return $"{Mathf.Max(0f, seconds):0.00}s";
    }
}
#endif
