using UnityEngine;

public class RaycastDebugRadio : MonoBehaviour
{
    [Header("Raycast")]
    public float distancia = 5f;
    public LayerMask mask;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            RevisarObjetoMirado();
        }
    }

    void RevisarObjetoMirado()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.forward, out hit, distancia, mask))
        {
            Debug.Log("===== RAYCAST DEBUG =====");
            Debug.Log("Objeto detectado: " + hit.collider.gameObject.name);
            Debug.Log("Padre: " + (hit.collider.transform.parent != null ? hit.collider.transform.parent.name : "Sin padre"));
            Debug.Log("Layer: " + LayerMask.LayerToName(hit.collider.gameObject.layer));
            Debug.Log("Tag: " + hit.collider.gameObject.tag);

            Debug.Log("Tiene RadioMissionInteractable: " + (hit.collider.GetComponentInParent<RadioMissionInteractable>() != null));
            Debug.Log("Tiene InspectableObject360: " + (hit.collider.GetComponentInParent<InspectableObject360>() != null));
            Debug.Log("Tiene RadioCoverTrigger: " + (hit.collider.GetComponentInParent<RadioCoverTrigger>() != null));
            Debug.Log("Tiene RadioFinalUse: " + (hit.collider.GetComponentInParent<RadioFinalUse>() != null));
            Debug.Log("Tiene BoxCollider: " + (hit.collider.GetComponent<BoxCollider>() != null));

            Debug.DrawRay(transform.position, transform.forward * distancia, Color.red, 2f);
        }
        else
        {
            Debug.Log("===== RAYCAST DEBUG =====");
            Debug.Log("No estoy detectando nada.");
            Debug.DrawRay(transform.position, transform.forward * distancia, Color.yellow, 2f);
        }
    }
}