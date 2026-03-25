using UnityEngine;

public class Selected : MonoBehaviour
{
    public float distancia = 3f;
    public Texture2D puntero;
    public GameObject TextDetect;

    private GameObject ultimoReconocido = null;
    private LayerMask mask;

    void Start()
    {
        mask = LayerMask.GetMask("RayCast Detect");
        TextDetect.SetActive(false);
    }

    void Update()
    {
        RaycastHit hit;

        // Rayo desde el centro de la cámara (como Minecraft)
        Ray ray = Camera.main.ScreenPointToRay(
            new Vector3(Screen.width / 2, Screen.height / 2, 0)
        );

        if (Physics.Raycast(ray, out hit, distancia, mask))
        {
            Deselect();
            SelectedObject(hit.transform);

            if (hit.collider.CompareTag("Interactivo"))
            {
                if (Input.GetKeyDown(KeyCode.I) || Input.GetKeyDown(KeyCode.JoystickButton7))
                {
                    hit.collider.GetComponent<ObjetoInteractivo>().ActivarObjeto();
                }
            }
        }
        else
        {
            Deselect();
        }
    }

    void SelectedObject(Transform obj)
    {
        Renderer renderer = obj.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = Color.green;
        }

        ultimoReconocido = obj.gameObject;
    }

    void Deselect()
    {
        if (ultimoReconocido != null)
        {
            Renderer renderer = ultimoReconocido.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = Color.white;
            }

            ultimoReconocido = null;
        }
    }

    void OnGUI()
    {
        Rect rect = new Rect(
            (Screen.width - puntero.width) / 2,
            (Screen.height - puntero.height) / 2,
            puntero.width,
            puntero.height
        );

        GUI.DrawTexture(rect, puntero);

        TextDetect.SetActive(ultimoReconocido != null);
    }
}