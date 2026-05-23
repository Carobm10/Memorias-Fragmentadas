using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class DiagnosticoAnimacionPila : MonoBehaviour
{
    [Header("Objeto de la animación")]
    public GameObject objetoPrimeraPila;

    [Header("Animator")]
    public Animator animatorPrimeraPila;

    [Header("Nombre exacto del estado naranja")]
    public string nombreEstado = "PrimeraPila";

    [Header("Objeto radio base que se debe apagar")]
    public GameObject radioAbierto3Pilas;

    [Header("Tecla de diagnóstico")]
    public KeyCode teclaDiagnostico = KeyCode.P;

    void Update()
    {
        if (Input.GetKeyDown(teclaDiagnostico))
        {
            StartCoroutine(DiagnosticarYProbar());
        }
    }

    IEnumerator DiagnosticarYProbar()
    {
        Debug.Log("========== DIAGNÓSTICO COMPLETO PRIMERA PILA ==========");

        if (objetoPrimeraPila == null)
        {
            Debug.LogError("ERROR: No asignaste objetoPrimeraPila.");
            yield break;
        }

        if (radioAbierto3Pilas != null)
        {
            radioAbierto3Pilas.SetActive(false);
            Debug.Log("OK: Apagué radio_abierto_3_pilas.");
        }
        else
        {
            Debug.LogWarning("AVISO: radioAbierto3Pilas está vacío. No apagué radio base.");
        }

        objetoPrimeraPila.SetActive(true);

        Debug.Log("Objeto pila: " + objetoPrimeraPila.name);
        Debug.Log("Activo Self: " + objetoPrimeraPila.activeSelf);
        Debug.Log("Activo Hierarchy: " + objetoPrimeraPila.activeInHierarchy);
        Debug.Log("Posición inicial: " + objetoPrimeraPila.transform.position);
        Debug.Log("Rotación inicial: " + objetoPrimeraPila.transform.eulerAngles);
        Debug.Log("Escala inicial: " + objetoPrimeraPila.transform.lossyScale);

        Renderer[] renderers = objetoPrimeraPila.GetComponentsInChildren<Renderer>(true);
        Debug.Log("Cantidad de Renderers en primera_pila: " + renderers.Length);

        foreach (Renderer r in renderers)
        {
            Debug.Log(
                "Renderer: " + r.gameObject.name +
                " | enabled: " + r.enabled +
                " | activeHierarchy: " + r.gameObject.activeInHierarchy +
                " | bounds center: " + r.bounds.center +
                " | bounds size: " + r.bounds.size
            );
        }

        if (animatorPrimeraPila == null)
            animatorPrimeraPila = objetoPrimeraPila.GetComponent<Animator>();

        if (animatorPrimeraPila == null)
        {
            Debug.LogError("ERROR: primera_pila NO tiene Animator.");
            yield break;
        }

        Debug.Log("Animator encontrado en: " + animatorPrimeraPila.gameObject.name);
        Debug.Log("Animator enabled: " + animatorPrimeraPila.enabled);
        Debug.Log("Apply Root Motion: " + animatorPrimeraPila.applyRootMotion);
        Debug.Log("Culling Mode: " + animatorPrimeraPila.cullingMode);
        Debug.Log("Controller: " + (animatorPrimeraPila.runtimeAnimatorController != null ? animatorPrimeraPila.runtimeAnimatorController.name : "SIN CONTROLLER"));

        if (animatorPrimeraPila.runtimeAnimatorController == null)
        {
            Debug.LogError("ERROR: El Animator no tiene Controller.");
            yield break;
        }

        bool tieneEstado = animatorPrimeraPila.HasState(0, Animator.StringToHash(nombreEstado));
        Debug.Log("Tiene estado '" + nombreEstado + "': " + tieneEstado);

        if (!tieneEstado)
        {
            Debug.LogError("ERROR: El estado naranja no se llama exactamente '" + nombreEstado + "'.");
            yield break;
        }

        RuntimeAnimatorController controller = animatorPrimeraPila.runtimeAnimatorController;
        AnimationClip[] clips = controller.animationClips;

        Debug.Log("Cantidad de clips en Controller: " + clips.Length);

        foreach (AnimationClip clip in clips)
        {
            Debug.Log("Clip encontrado: " + clip.name + " | Duración: " + clip.length);

#if UNITY_EDITOR
            EditorCurveBinding[] bindings = AnimationUtility.GetCurveBindings(clip);
            Debug.Log("Curvas del clip " + clip.name + ": " + bindings.Length);

            foreach (EditorCurveBinding b in bindings)
            {
                Debug.Log(
                    "CLIP BINDING | Path: [" + b.path +
                    "] | Property: [" + b.propertyName +
                    "] | Type: " + b.type
                );
            }
#endif
        }

        animatorPrimeraPila.enabled = true;
        animatorPrimeraPila.applyRootMotion = false;
        animatorPrimeraPila.cullingMode = AnimatorCullingMode.AlwaysAnimate;
        animatorPrimeraPila.speed = 1f;

        Vector3 posAntes = objetoPrimeraPila.transform.position;
        Quaternion rotAntes = objetoPrimeraPila.transform.rotation;
        Vector3 scaleAntes = objetoPrimeraPila.transform.lossyScale;

        Debug.Log("Voy a reproducir estado: " + nombreEstado);

        animatorPrimeraPila.Rebind();
        animatorPrimeraPila.Update(0f);
        animatorPrimeraPila.Play(nombreEstado, 0, 0f);
        animatorPrimeraPila.Update(0.1f);
        Debug.Break();

        yield return null;

        for (int i = 0; i < 10; i++)
        {
            yield return new WaitForSeconds(0.2f);

            AnimatorStateInfo info = animatorPrimeraPila.GetCurrentAnimatorStateInfo(0);

            Debug.Log(
                "FRAME TEST " + i +
                " | Estado hash: " + info.shortNameHash +
                " | NormalizedTime: " + info.normalizedTime +
                " | Posición: " + objetoPrimeraPila.transform.position +
                " | Rotación: " + objetoPrimeraPila.transform.eulerAngles +
                " | Escala: " + objetoPrimeraPila.transform.lossyScale
            );
        }

        Vector3 posDespues = objetoPrimeraPila.transform.position;
        Quaternion rotDespues = objetoPrimeraPila.transform.rotation;
        Vector3 scaleDespues = objetoPrimeraPila.transform.lossyScale;

        Debug.Log("¿Cambió posición?: " + (Vector3.Distance(posAntes, posDespues) > 0.001f));
        Debug.Log("¿Cambió rotación?: " + (Quaternion.Angle(rotAntes, rotDespues) > 0.001f));
        Debug.Log("¿Cambió escala?: " + (Vector3.Distance(scaleAntes, scaleDespues) > 0.001f));

        Debug.Log("========== FIN DIAGNÓSTICO PRIMERA PILA ==========");
    }
}