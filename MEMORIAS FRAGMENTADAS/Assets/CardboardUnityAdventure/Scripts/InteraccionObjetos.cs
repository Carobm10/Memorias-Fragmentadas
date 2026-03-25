using UnityEngine;

public class InteraccionObjetos : MonoBehaviour
{
    public float distancia = 3f;
    public GameObject textDetect;

    private GameObject ultimoReconocido = null;
    private Renderer ultimoRenderer = null;
    private Color colorOriginal;
    private LayerMask mask;

    void Start()
    {
        mask = LayerMask.GetMask("RayCast Detect");

        if (textDetect != null)
            textDetect.SetActive(false);
        else
            Debug.LogWarning("No asignaste TextDetect en InteraccionObjetos.");
    }

    void Update()
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position, transform.forward);

        if (Physics.Raycast(ray, out hit, distancia, mask))
        {
            if (ultimoReconocido != hit.collider.gameObject)
            {
                Deselect();
                SelectObject(hit.collider.gameObject);
            }

            if (textDetect != null)
                textDetect.SetActive(true);

            if (Input.GetKeyDown(KeyCode.I))
            {
                // Busca el script en el objeto golpeado o en su padre
                ObjetoInteractivo interactivo = hit.collider.GetComponentInParent<ObjetoInteractivo>();

                if (interactivo != null)
                {
                    Debug.Log("Interactuando con: " + interactivo.gameObject.name);
                    interactivo.ActivarObjeto();
                    Deselect();

                    if (textDetect != null)
                        textDetect.SetActive(false);
                }
                else
                {
                    Debug.LogWarning("El objeto mirado no tiene ObjetoInteractivo en él ni en su padre.");
                }
            }
        }
        else
        {
            Deselect();

            if (textDetect != null)
                textDetect.SetActive(false);
        }
    }

    void SelectObject(GameObject obj)
    {
        Renderer renderer = obj.GetComponent<Renderer>();

        if (renderer != null)
        {
            ultimoRenderer = renderer;
            colorOriginal = renderer.material.color;
            renderer.material.color = Color.green;
        }

        ultimoReconocido = obj;
    }

    void Deselect()
    {
        if (ultimoReconocido != null && ultimoRenderer != null)
        {
            ultimoRenderer.material.color = colorOriginal;
        }

        ultimoReconocido = null;
        ultimoRenderer = null;
    }
}