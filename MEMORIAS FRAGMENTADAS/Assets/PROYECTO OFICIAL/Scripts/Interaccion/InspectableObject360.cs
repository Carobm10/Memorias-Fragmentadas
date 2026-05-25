using UnityEngine;
using System.Collections;

public class InspectableObject360 : MonoBehaviour
{
    [Header("Cámara")]
    public Transform cameraTransform;
    private Quaternion lockedCameraRotation;

    [Header("Prefab visual")]
    public GameObject visualPrefab;

    [Header("Punto de inspección")]
    public Transform inspectPoint;

    [Header("Canvas salir")]
    public GameObject exitCanvas;

    [Header("Pointer")]
    public GameObject pointer3D;

    [Header("Movimiento jugador")]
    public MovimientoVR2 playerMovement;

    [Header("Rotación")]
    public float rotationSpeed = 120f;
    public float mouseRotationSpeed = 5f;

    [Header("Escala copia")]
    public float inspectScale = 1f;

    [Header("Debug")]
    public bool showDebug = true;

    private GameObject currentClone;
    private GameObject visualWrapper;
    private bool inspecting = false;

    public bool IsInspecting()
    {
        return inspecting;
    }

    public void StartInspection()
    {
        if (inspecting) return;

        if (visualPrefab == null)
        {
            Debug.LogWarning("No hay visualPrefab en " + gameObject.name);
            return;
        }

        if (inspectPoint == null)
        {
            Debug.LogError("No hay InspectPoint asignado en " + gameObject.name);
            return;
        }

        inspecting = true;

        // ======================================================
        // BLOQUEAR ROTACIÓN CÁMARA
        // ======================================================

        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;

        if (cameraTransform != null)
            lockedCameraRotation = cameraTransform.rotation;

        // ======================================================
        // CREAR CONTENEDOR VISUAL
        // ======================================================

        visualWrapper = new GameObject("INSPECCION_360_" + visualPrefab.name);

        visualWrapper.transform.SetParent(inspectPoint);

        visualWrapper.transform.localPosition = Vector3.zero;
        visualWrapper.transform.localRotation = Quaternion.identity;
        visualWrapper.transform.localScale = Vector3.one;

        // ======================================================
        // CREAR CLON
        // ======================================================

        currentClone = Instantiate(visualPrefab, visualWrapper.transform);

        Debug.Log("===== DEBUG COMPONENTES DEL CLON 360 =====");
        Debug.Log("Clon creado: " + currentClone.name);


        Animator[] animators = currentClone.GetComponentsInChildren<Animator>(true);

        Debug.Log("Animators encontrados en clon: " + animators.Length);

        foreach (Animator anim in animators)
        {
            string controllerName = anim.runtimeAnimatorController != null
                ? anim.runtimeAnimatorController.name
                : "SIN CONTROLLER";

            Debug.Log("Animator en: " + anim.gameObject.name + " | Controller: " + controllerName);
        }
        
        currentClone.name = visualPrefab.name + "_CLON_360";

        currentClone.transform.localPosition = Vector3.zero;
        currentClone.transform.localRotation = Quaternion.identity;
        currentClone.transform.localScale = Vector3.one * inspectScale;

        // ======================================================
        // DESACTIVAR COLLIDERS DEL CLON
        // ======================================================

        Collider[] cloneCols = currentClone.GetComponentsInChildren<Collider>(true);

        foreach (Collider col in cloneCols)
            col.enabled = false;

        // ======================================================
        // DESACTIVAR RIGIDBODIES
        // ======================================================

        Rigidbody[] rbs = currentClone.GetComponentsInChildren<Rigidbody>(true);

        foreach (Rigidbody rb in rbs)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        // ======================================================
        // CENTRAR CLON
        // ======================================================

        CenterCloneByRenderers();

        // ======================================================
        // OCULTAR ORIGINAL
        // ======================================================

        SetOriginalVisible(false);

        // ======================================================
        // BLOQUEAR MOVIMIENTO
        // ======================================================

        if (playerMovement != null)
            playerMovement.puedeMoverse = false;

        // ======================================================
        // OCULTAR PUNTERO Y VOLVERLO A MOSTRAR
        // ======================================================

        //if (pointer3D != null)
        //{
        //    pointer3D.SetActive(false);
        //    StartCoroutine(ShowPointerAfterSeconds());
        //}

        if (pointer3D != null)
        {
            pointer3D.SetActive(false);
        }

        // ======================================================
        // MOSTRAR CANVAS SALIR
        // ======================================================

        if (exitCanvas != null)
            exitCanvas.SetActive(true);

        // ======================================================
        // DEBUG
        // ======================================================

        if (showDebug)
        {
            Debug.Log("===== DEBUG 360 =====");
            Debug.Log("Original: " + gameObject.name);
            Debug.Log("Visual Prefab: " + visualPrefab.name);
            Debug.Log("InspectPoint: " + inspectPoint.name);
            Debug.Log("InspectPoint mundo: " + inspectPoint.position);
            Debug.Log("InspectPoint local: " + inspectPoint.localPosition);
            Debug.Log("Wrapper mundo: " + visualWrapper.transform.position);
            Debug.Log("Clon localPosition final: " + currentClone.transform.localPosition);
            Debug.Log("Clon escala: " + currentClone.transform.localScale);
        }
    }

    /*
    IEnumerator ShowPointerAfterSeconds()
    {
        yield return new WaitForSeconds(2f);

        if (inspecting && pointer3D != null)
        {
            pointer3D.SetActive(true);
            Debug.Log("Puntero 3D volvió a aparecer en modo inspección 360");
        }
    }
    */

    void CenterCloneByRenderers()
    {
        if (currentClone == null) return;

        Renderer[] renderers = currentClone.GetComponentsInChildren<Renderer>(true);

        if (renderers == null || renderers.Length == 0)
        {
            Debug.LogWarning("El clon no tiene renderers para centrar: " + currentClone.name);
            return;
        }

        Bounds bounds = renderers[0].bounds;

        for (int i = 1; i < renderers.Length; i++)
        {
            bounds.Encapsulate(renderers[i].bounds);
        }

        Vector3 worldCenter = bounds.center;

        Vector3 localCenter =
            currentClone.transform.InverseTransformPoint(worldCenter);

        currentClone.transform.localPosition -= localCenter;

        if (showDebug)
        {
            Debug.Log("Centro visual mundo: " + worldCenter);
            Debug.Log("Centro visual local: " + localCenter);
        }
    }

    void SetOriginalVisible(bool visible)
    {
        Renderer[] renderers =
            GetComponentsInChildren<Renderer>(true);

        foreach (Renderer r in renderers)
            r.enabled = visible;

        Collider[] colliders =
            GetComponentsInChildren<Collider>(true);

        foreach (Collider c in colliders)
            c.enabled = visible;
    }

    public void StopInspect()
    {
        if (!inspecting) return;

        inspecting = false;

        // ======================================================
        // ELIMINAR CLON
        // ======================================================

        if (visualWrapper != null)
            Destroy(visualWrapper);
        else if (currentClone != null)
            Destroy(currentClone);

        // ======================================================
        // VOLVER A MOSTRAR ORIGINAL
        // ======================================================

        SetOriginalVisible(true);

        // ======================================================
        // DEVOLVER MOVIMIENTO
        // ======================================================

        if (playerMovement != null)
            playerMovement.puedeMoverse = true;

        // ======================================================
        // MOSTRAR PUNTERO
        // ======================================================

        if (pointer3D != null)
            pointer3D.SetActive(true);

        // ======================================================
        // OCULTAR CANVAS SALIR
        // ======================================================

        if (exitCanvas != null)
            exitCanvas.SetActive(false);
    }

    void Update()
    {
        if (!inspecting) return;

        // ======================================================
        // SALIR
        // ======================================================

        if (InputManagerCustom.PressX())
        {
            StopInspect();
            return;
        }

        if (visualWrapper == null) return;

        // ======================================================
        // INPUT JOYSTICK
        // ======================================================

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        visualWrapper.transform.Rotate(
            Vector3.up,
            -horizontal * rotationSpeed * Time.deltaTime,
            Space.World
        );

        visualWrapper.transform.Rotate(
            Vector3.right,
            vertical * rotationSpeed * Time.deltaTime,
            Space.World
        );

        // ======================================================
        // INPUT MOUSE
        // ======================================================

        if (Input.GetMouseButton(0))
        {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            visualWrapper.transform.Rotate(
                Vector3.up,
                -mouseX * mouseRotationSpeed,
                Space.World
            );

            visualWrapper.transform.Rotate(
                Vector3.right,
                mouseY * mouseRotationSpeed,
                Space.World
            );
        }
    }

    void LateUpdate()
    {
        if (!inspecting) return;

        if (cameraTransform != null)
            cameraTransform.rotation = lockedCameraRotation;
    }
}