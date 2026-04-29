using UnityEngine;

public class Selected : MonoBehaviour
{
    [Header("Puntero 3D")]
    public Pointer3DController pointer3D;

    [Header("Raycast")]
    public float distancia = 3f;
    private LayerMask mask;

    [Header("Prompt general")]
    public GameObject TextDetect;

    [Header("Prompt puertas")]
    public GameObject DoorPromptPanel;

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

        ApagarPrompts();
    }

    void Update()
    {
        if (closetCanvasManager != null && closetCanvasManager.uiAbierta)
        {
            ApagarPrompts();
            if (pointer3D != null) pointer3D.SetDetected(false);
            return;
        }

        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.forward, out hit, distancia, mask))
        {
            if (pointer3D != null)
                pointer3D.SetDetected(true);

            GameObject objetoDetectado = hit.collider.gameObject;

            if (ultimoReconocido != objetoDetectado)
            {
                Deselect();
                SelectedObject(hit.collider);
            }

            // 1. NPC
            LogicaNPC npc = hit.collider.GetComponent<LogicaNPC>();
            if (npc == null)
                npc = hit.collider.GetComponentInParent<LogicaNPC>();

            if (npc != null)
            {
                ApagarPrompts();
                return;
            }

            // 2. Prendas del clóset
            ClosetClothingItem clothing = hit.collider.GetComponent<ClosetClothingItem>();
            if (clothing == null)
                clothing = hit.collider.GetComponentInParent<ClosetClothingItem>();

            if (clothing != null)
            {
                MostrarSolo(ClothingPromptPanel);

                ClosetMissionTrigger closetForClothing = hit.collider.GetComponent<ClosetMissionTrigger>();
                if (closetForClothing == null)
                    closetForClothing = hit.collider.GetComponentInParent<ClosetMissionTrigger>();

                if (InputManagerCustom.PressB())
                {
                    Debug.Log("Seleccionaste prenda: " + clothing.clothingName);

                    if (closetCanvasManager != null)
                    {
                        closetCanvasManager.AbrirCanvas(
                            clothing.clothingCanvas,
                            clothing.isCorrect,
                            closetForClothing
                        );
                    }

                    return;
                }

                return;
            }

            // 3. Clóset / misión
            ClosetMissionTrigger closet = hit.collider.GetComponent<ClosetMissionTrigger>();
            if (closet == null)
                closet = hit.collider.GetComponentInParent<ClosetMissionTrigger>();

            if (closet != null && !closet.missionStarted && !closet.missionCompleted)
            {
                MostrarSolo(MissionPromptPanel);

                if (InputManagerCustom.PressX())
                {
                    Debug.Log("Iniciando misión del clóset");
                    closet.StartClosetMission();
                    return;
                }

                return;
            }

            // 4. Puertas normales
            DoorInteractable door = hit.collider.GetComponent<DoorInteractable>();
            if (door == null)
                door = hit.collider.GetComponentInParent<DoorInteractable>();

            if (door != null)
            {
                MostrarSolo(DoorPromptPanel);

                if (InputManagerCustom.PressB())
                {
                    Debug.Log("Abriendo/cerrando puerta: " + door.gameObject.name);
                    door.ToggleDoor();
                    return;
                }

                return;
            }

            // 5. Cajones
            DrawerInteractable drawer = hit.collider.GetComponent<DrawerInteractable>();
            if (drawer == null)
                drawer = hit.collider.GetComponentInParent<DrawerInteractable>();

            if (drawer != null)
            {
                MostrarSolo(DoorPromptPanel);

                if (InputManagerCustom.PressB())
                {
                    Debug.Log("Abriendo/cerrando cajón: " + drawer.gameObject.name);
                    drawer.ToggleDrawer();
                    return;
                }

                return;
            }

            // 6. Objetos inspeccionables
            InspectableObject inspectable = hit.collider.GetComponent<InspectableObject>();
            if (inspectable == null)
                inspectable = hit.collider.GetComponentInParent<InspectableObject>();

            if (inspectable != null)
            {
                MostrarSolo(TextDetect);

                if (InputManagerCustom.PressX())
                {
                    currentInspectable = inspectable;
                    inspectable.ToggleInspect();
                    return;
                }

                return;
            }

            // 7. Objeto interactivo simple
            ObjetoInteractivo objeto = hit.collider.GetComponent<ObjetoInteractivo>();
            if (objeto == null)
                objeto = hit.collider.GetComponentInParent<ObjetoInteractivo>();

            if (objeto != null)
            {
                MostrarSolo(TextDetect);

                if (InputManagerCustom.PressX())
                {
                    objeto.ActivarObjeto();
                    return;
                }

                return;
            }

            ApagarPrompts();
            Debug.DrawRay(transform.position, transform.forward * distancia, Color.red);
        }
        else
        {
            if (pointer3D != null)
                pointer3D.SetDetected(false);

            ApagarPrompts();
            Deselect();
        }
    }

    void MostrarSolo(GameObject panel)
    {
        if (TextDetect != null)
            TextDetect.SetActive(panel == TextDetect);

        if (DoorPromptPanel != null)
            DoorPromptPanel.SetActive(panel == DoorPromptPanel);

        if (MissionPromptPanel != null)
            MissionPromptPanel.SetActive(panel == MissionPromptPanel);

        if (ClothingPromptPanel != null)
            ClothingPromptPanel.SetActive(panel == ClothingPromptPanel);
    }

    void ApagarPrompts()
    {
        if (TextDetect != null)
            TextDetect.SetActive(false);

        if (DoorPromptPanel != null)
            DoorPromptPanel.SetActive(false);

        if (MissionPromptPanel != null)
            MissionPromptPanel.SetActive(false);

        if (ClothingPromptPanel != null)
            ClothingPromptPanel.SetActive(false);
    }

    void SelectedObject(Collider col)
    {
        HighlightGroup highlightGroup = col.GetComponent<HighlightGroup>();
        if (highlightGroup == null)
            highlightGroup = col.GetComponentInParent<HighlightGroup>();

        if (highlightGroup != null)
        {
            Renderer[] renderers = highlightGroup.GetComponentsInChildren<Renderer>();
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

            ultimoReconocido = highlightGroup.gameObject;
            return;
        }

        Renderer renderer = col.GetComponent<Renderer>();
        if (renderer == null)
            renderer = col.GetComponentInParent<Renderer>();

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
}