using UnityEngine;

public class InteractionMegaDebugger : MonoBehaviour
{
    [Header("Raycast")]
    public float distancia = 4f;
    public LayerMask mask = ~0;

    [Header("Tecla debug")]
    public KeyCode teclaDebug = KeyCode.F9;

    void Update()
    {
        if (Input.GetKeyDown(teclaDebug))
            Revisar();
    }

    void Revisar()
    {
        RaycastHit hit;

        Debug.Log("========== INTERACTION MEGA DEBUG ==========");

        if (!Physics.Raycast(transform.position, transform.forward, out hit, distancia, mask))
        {
            Debug.Log("NO estoy tocando nada con el raycast.");
            Debug.Log("============================================");
            return;
        }

        GameObject obj = hit.collider.gameObject;

        Debug.Log("COLLIDER TOCADO: " + hit.collider.name);
        Debug.Log("OBJETO TOCADO: " + obj.name);
        Debug.Log("PADRE: " + (hit.collider.transform.parent != null ? hit.collider.transform.parent.name : "SIN PADRE"));
        Debug.Log("ROOT: " + hit.collider.transform.root.name);
        Debug.Log("LAYER: " + LayerMask.LayerToName(obj.layer));
        Debug.Log("TAG: " + obj.tag);
        Debug.Log("DISTANCIA HIT: " + hit.distance);

        Debug.Log("--- COMPONENTES EN OBJETO TOCADO ---");
        foreach (Component c in obj.GetComponents<Component>())
        {
            if (c == null)
                Debug.Log("Missing Script");
            else
                Debug.Log(c.GetType().Name);
        }

        Debug.Log("--- SCRIPTS EN PADRES ---");
        foreach (MonoBehaviour m in hit.collider.GetComponentsInParent<MonoBehaviour>(true))
        {
            if (m == null)
                Debug.Log("Missing Script en padre");
            else
                Debug.Log("PADRE SCRIPT: " + m.GetType().Name + " en " + m.gameObject.name);
        }

        Debug.Log("--- DETECCIONES IMPORTANTES ---");
        Debug.Log("Tiene DoorInteractable: " + (hit.collider.GetComponentInParent<DoorInteractable>() != null));
        Debug.Log("Tiene DrawerInteractable: " + (hit.collider.GetComponentInParent<DrawerInteractable>() != null));
        Debug.Log("Tiene RadioAnimacionesSimple: " + (hit.collider.GetComponentInParent<RadioAnimacionesSimple>() != null));
        Debug.Log("Tiene RadioKnobInteractable: " + (hit.collider.GetComponentInParent<RadioKnobInteractable>() != null));
        Debug.Log("Tiene RosaFinalDialogue: " + (hit.collider.GetComponentInParent<RosaFinalDialogue>() != null));

        Debug.Log("============================================");
    }
}