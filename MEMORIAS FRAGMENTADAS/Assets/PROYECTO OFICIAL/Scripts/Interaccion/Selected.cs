using UnityEngine;

public class Selected : MonoBehaviour
{
    private LayerMask mask;

    public float distancia = 2f;
    public Texture2D puntero;

    [Header("Prompt general")]
    public GameObject TextDetect;

    [Header("Prompt clóset")]
    public GameObject MissionPromptPanel;

    [Header("Prompt prendas")]
    public GameObject ClothingPromptPanel;

    [Header("Manager de canvases")]
    public ClosetCanvasManager closetCanvasManager;

    private GameObject ultimoReconocido;
    private Renderer[] renderersActuales;
    private Color[] coloresOriginales;
    private InspectableObject currentInspectable;

    void Start()
    {
        mask = LayerMask.GetMask("Raycast Detect");

        if (MissionPromptPanel != null)
            MissionPromptPanel.SetActive(false);

        if (ClothingPromptPanel != null)
            ClothingPromptPanel.SetActive(false);

        if (TextDetect != null)
            TextDetect.SetActive(false);
    }

    void Update()
    {
        if (closetCanvasManager != null && closetCanvasManager.uiAbierta)
        {
            if (TextDetect != null)
                TextDetect.SetActive(false);

            if (MissionPromptPanel != null)
                MissionPromptPanel.SetActive(false);

            if (ClothingPromptPanel != null)
                ClothingPromptPanel.SetActive(false);

            return;
        }

        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.forward, out hit, distancia, mask))
        {
            GameObject objetoDetectado = hit.collider.gameObject;

            if (ultimoReconocido != objetoDetectado)
            {
                Deselect();
                SelectedObject(hit.collider);
            }

            LogicaNPC npc = hit.collider.GetComponent<LogicaNPC>();
            if (npc == null)
                npc = hit.collider.GetComponentInParent<LogicaNPC>();

            // Si mira un NPC, no mostrar prompt general ni tratarlo como objeto normal
            if (npc != null)
            {
                if (TextDetect != null)
                    TextDetect.SetActive(false);

                if (MissionPromptPanel != null)
                    MissionPromptPanel.SetActive(false);

                if (ClothingPromptPanel != null)
                    ClothingPromptPanel.SetActive(false);

                return;
            }

            ClosetMissionTrigger closet = hit.collider.GetComponent<ClosetMissionTrigger>();
            if (closet == null)
                closet = hit.collider.GetComponentInParent<ClosetMissionTrigger>();

            ClosetClothingItem clothing = hit.collider.GetComponent<ClosetClothingItem>();
            if (clothing == null)
                clothing = hit.collider.GetComponentInParent<ClosetClothingItem>();

            if (clothing != null)
            {
                if (ClothingPromptPanel != null)
                    ClothingPromptPanel.SetActive(true);

                if (MissionPromptPanel != null)
                    MissionPromptPanel.SetActive(false);

                if (TextDetect != null)
                    TextDetect.SetActive(false);

                if (InputManagerCustom.PressB())
                {
                    Debug.Log("Seleccionaste prenda: " + clothing.clothingName);

                    if (closetCanvasManager != null)
                    {
                        closetCanvasManager.AbrirCanvas(
                            clothing.clothingCanvas,
                            clothing.isCorrect,
                            closet
                        );
                    }

                    return;
                }
            }
            else
            {
                if (ClothingPromptPanel != null)
                    ClothingPromptPanel.SetActive(false);
            }

            if (clothing == null && closet != null && !closet.missionStarted)
            {
                if (MissionPromptPanel != null)
                    MissionPromptPanel.SetActive(true);

                if (TextDetect != null)
                    TextDetect.SetActive(false);

                if (InputManagerCustom.PressX())
                {
                    Debug.Log("Iniciando misión del clóset");
                    closet.StartClosetMission();
                    return;
                }
            }
            else
            {
                if (MissionPromptPanel != null && clothing == null)
                    MissionPromptPanel.SetActive(false);
            }

            if (clothing == null && closet == null)
            {
                if (InputManagerCustom.PressB())
                {
                    Debug.Log("Interactuando con objeto normal");

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
                    return;
                }
            }

            Debug.DrawRay(transform.position, transform.forward * distancia, Color.red);
        }
        else
        {
            if (MissionPromptPanel != null)
                MissionPromptPanel.SetActive(false);

            if (ClothingPromptPanel != null)
                ClothingPromptPanel.SetActive(false);

            if (TextDetect != null)
                TextDetect.SetActive(false);

            Deselect();
        }
    }

    void SelectedObject(Collider col)
    {
        HighlightGroup highlightGroup = col.GetComponent<HighlightGroup>();
        if (highlightGroup == null)
            highlightGroup = col.GetComponentInParent<HighlightGroup>();

        if (highlightGroup != null)
        {
            Renderer[] renderers = highlightGroup.GetComponentsInChildren<Renderer>();

            if (renderers != null && renderers.Length > 0)
            {
                renderersActuales = renderers;
                coloresOriginales = new Color[renderers.Length];

                for (int i = 0; i < renderers.Length; i++)
                {
                    if (renderers[i] != null && renderers[i].material.HasProperty("_Color"))
                    {
                        coloresOriginales[i] = renderers[i].material.color;
                        renderers[i].material.color = Color.green;
                    }
                }
            }

            ultimoReconocido = highlightGroup.gameObject;
            return;
        }

        Renderer renderer = col.GetComponent<Renderer>();
        if (renderer == null)
            renderer = col.GetComponentInParent<Renderer>();
        if (renderer == null)
            renderer = col.GetComponentInChildren<Renderer>();

        if (renderer != null && renderer.material.HasProperty("_Color"))
        {
            renderersActuales = new Renderer[] { renderer };
            coloresOriginales = new Color[] { renderer.material.color };
            renderer.material.color = Color.green;
        }

        ultimoReconocido = col.gameObject;
    }

    void Deselect()
    {
        if (renderersActuales != null && coloresOriginales != null)
        {
            for (int i = 0; i < renderersActuales.Length; i++)
            {
                if (renderersActuales[i] != null && renderersActuales[i].material.HasProperty("_Color"))
                {
                    renderersActuales[i].material.color = coloresOriginales[i];
                }
            }
        }

        ultimoReconocido = null;
        renderersActuales = null;
        coloresOriginales = null;
    }

    void OnGUI()
    {
        if (closetCanvasManager != null && closetCanvasManager.uiAbierta)
        {
            if (TextDetect != null)
                TextDetect.SetActive(false);
            return;
        }

        if (puntero != null)
        {
            float size = 12f;
            Rect rect = new Rect(
                (Screen.width - size) / 2,
                (Screen.height - size) / 2,
                size,
                size
            );

            GUI.DrawTexture(rect, puntero);
        }

        if (TextDetect != null)
        {
            bool mirandoCloset = false;
            bool mirandoPrenda = false;
            bool mirandoNPC = false;

            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, distancia, mask))
            {
                ClosetMissionTrigger closet = hit.collider.GetComponent<ClosetMissionTrigger>();
                if (closet == null)
                    closet = hit.collider.GetComponentInParent<ClosetMissionTrigger>();

                mirandoCloset = closet != null && !closet.missionStarted;

                ClosetClothingItem clothing = hit.collider.GetComponent<ClosetClothingItem>();
                if (clothing == null)
                    clothing = hit.collider.GetComponentInParent<ClosetClothingItem>();

                mirandoPrenda = clothing != null;

                LogicaNPC npc = hit.collider.GetComponent<LogicaNPC>();
                if (npc == null)
                    npc = hit.collider.GetComponentInParent<LogicaNPC>();

                mirandoNPC = npc != null;
            }

            TextDetect.SetActive(
                (ultimoReconocido != null || currentInspectable != null)
                && !mirandoCloset
                && !mirandoPrenda
                && !mirandoNPC
            );
        }
    }
}