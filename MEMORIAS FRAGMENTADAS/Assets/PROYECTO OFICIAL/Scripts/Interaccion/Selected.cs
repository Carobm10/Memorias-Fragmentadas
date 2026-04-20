using UnityEngine;

public class Selected : MonoBehaviour
{
    private LayerMask mask;

    public float distancia = 2f;
    public Texture2D puntero;
    public GameObject TextDetect;

    private GameObject ultimoReconocido;
    private Renderer ultimoRenderer;
    private Color colorOriginal;

    private InspectableObject currentInspectable;

    void Start()
    {
        mask = LayerMask.GetMask("Raycast Detect");

        if (TextDetect != null)
        {
            TextDetect.SetActive(false);
        }
    }

    void Update()
    {
        // Si ya estás inspeccionando algo, E lo suelta
        if (currentInspectable != null && currentInspectable.isInspecting)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                currentInspectable.ToggleInspect();
                currentInspectable = null;
            }

            return;
        }

        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, distancia, mask))
        {
            GameObject objetoDetectado = hit.collider.gameObject;

            if (ultimoReconocido != objetoDetectado)
            {
                Deselect();
                SelectedObject(hit.collider);
            }

            if (Input.GetKeyDown(KeyCode.B))
            {
                SystemDoor door = hit.collider.GetComponent<SystemDoor>();
                if (door == null)
                    door = hit.collider.GetComponentInParent<SystemDoor>();

                if (door != null)
                {
                    door.ToggleDoor();
                    return;
                }

                SystemDrawer drawer = hit.collider.GetComponent<SystemDrawer>();
                if (drawer == null)
                    drawer = hit.collider.GetComponentInParent<SystemDrawer>();

                if (drawer != null)
                {
                    drawer.ToggleDrawer();
                    return;
                }
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                InspectableObject inspectable = hit.collider.GetComponent<InspectableObject>();
                if (inspectable == null)
                    inspectable = hit.collider.GetComponentInParent<InspectableObject>();

                if (inspectable != null)
                {
                    currentInspectable = inspectable;
                    inspectable.ToggleInspect();
                    return;
                }

                ObjetoInteractivo objeto = hit.collider.GetComponent<ObjetoInteractivo>();
                if (objeto == null)
                    objeto = hit.collider.GetComponentInParent<ObjetoInteractivo>();

                if (objeto != null)
                {
                    objeto.ActivarObjeto();
                }
            }

            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * distancia, Color.red);
        }
        else
        {
            Deselect();
        }
    }

    void SelectedObject(Collider col)
    {
        Renderer renderer = col.GetComponent<Renderer>();

        if (renderer == null)
            renderer = col.GetComponentInParent<Renderer>();

        if (renderer != null)
        {
            ultimoRenderer = renderer;
            colorOriginal = renderer.material.color;
            renderer.material.color = Color.green;
        }

        ultimoReconocido = col.gameObject;
    }

    void Deselect()
    {
        if (ultimoRenderer != null)
        {
            ultimoRenderer.material.color = colorOriginal;
        }

        ultimoReconocido = null;
        ultimoRenderer = null;
    }

    private void OnGUI()
    {
        if (puntero != null)
        {
            Rect rect = new Rect(
                (Screen.width - puntero.width) / 2,
                (Screen.height - puntero.height) / 2,
                puntero.width,
                puntero.height
            );

            GUI.DrawTexture(rect, puntero);
        }

        if (TextDetect != null)
        {
            TextDetect.SetActive(ultimoReconocido != null || currentInspectable != null);
        }
    }
}