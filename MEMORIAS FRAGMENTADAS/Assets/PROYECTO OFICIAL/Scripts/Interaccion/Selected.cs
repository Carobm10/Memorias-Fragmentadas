using UnityEngine;

public class Selected : MonoBehaviour
{
    private LogicaNPC ultimoNPCMirado;
    private PhoneMissionController ultimoTelefonoMirado;

    [Header("Puntero 3D")]
    public Pointer3DController pointer3D;

    [Header("Raycast")]
    public float distancia = 3f;
    private LayerMask mask;

    [Header("Prompts")]
    public GameObject TextDetect;
    public GameObject DoorPromptPanel;
    public GameObject MissionPromptPanel;
    public GameObject ClothingPromptPanel;

    [Header("Manager de canvases")]
    public ClosetCanvasManager closetCanvasManager;

    private GameObject ultimoReconocido;
    private Renderer[] renderersActuales;
    private Color[] coloresOriginales;

    [Header("Color de selección")]
    public Color colorSeleccion = new Color(0.1f, 1f, 0.25f, 1f);

    void Awake()
    {
        ApagarTodosLosPrompts();
    }

    void Start()
    {
        mask = LayerMask.GetMask("Raycast Detect");
        ApagarTodosLosPrompts();
    }

    void Update()
    {
        if (closetCanvasManager != null && closetCanvasManager.uiAbierta)
        {
            LimpiarMiradas();
            ApagarTodosLosPrompts();

            if (pointer3D != null)
                pointer3D.SetDetected(false);

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
            LogicaNPC npc = hit.collider.GetComponentInParent<LogicaNPC>();

            if (npc != null)
            {
                ApagarTelefonoMirado();

                if (ultimoNPCMirado != null && ultimoNPCMirado != npc)
                    ultimoNPCMirado.SetMirandoNPC(false);

                ultimoNPCMirado = npc;
                ultimoNPCMirado.SetMirandoNPC(true);

                ApagarTodosLosPrompts();
                return;
            }
            else
            {
                ApagarNPCMirado();
            }

            // 2. TELÉFONO
            PhoneMissionController phone = hit.collider.GetComponentInParent<PhoneMissionController>();

            if (phone != null)
            {
                ApagarNPCMirado();

                if (ultimoTelefonoMirado != null && ultimoTelefonoMirado != phone)
                    ultimoTelefonoMirado.SetMirandoTelefono(false);

                ultimoTelefonoMirado = phone;
                ultimoTelefonoMirado.SetMirandoTelefono(true);

                return;
            }
            else
            {
                ApagarTelefonoMirado();
            }

            MemoryFrameInteractable frame =
                hit.collider.GetComponentInParent<MemoryFrameInteractable>();

            if (frame != null)
            {
                frame.SetMirando(true);

                if (Input.GetKeyDown(KeyCode.V))
                {
                    // el propio script ya lo maneja
                }

                return;
            }

            else
            {
                frame.SetMirando(false);
            }

            // 3. Prendas del clóset
            ClosetClothingItem clothing = hit.collider.GetComponentInParent<ClosetClothingItem>();

            if (clothing != null)
            {
                MostrarSolo(ClothingPromptPanel);

                ClosetMissionTrigger closetForClothing = hit.collider.GetComponentInParent<ClosetMissionTrigger>();

                if (InputManagerCustom.PressB())
                {
                    Debug.Log("Seleccionaste prenda con B: " + clothing.clothingName);

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

            // 4. Clóset / misión
            ClosetMissionTrigger closet = hit.collider.GetComponentInParent<ClosetMissionTrigger>();

            if (closet != null && !closet.missionStarted && !closet.missionCompleted)
            {
                MostrarSolo(MissionPromptPanel);

                if (InputManagerCustom.PressA())
                {
                    Debug.Log("Iniciando misión del clóset con A");
                    closet.StartClosetMission();
                    return;
                }

                return;
            }

            // 5. Puertas
            DoorInteractable door = hit.collider.GetComponentInParent<DoorInteractable>();

            if (door != null)
            {
                MostrarSolo(DoorPromptPanel);

                if (InputManagerCustom.PressB())
                {
                    Debug.Log("Abriendo/cerrando puerta con B: " + door.gameObject.name);
                    door.ToggleDoor();
                    return;
                }

                return;
            }

            // 6. Cajones
            DrawerInteractable drawer = hit.collider.GetComponentInParent<DrawerInteractable>();

            if (drawer != null)
            {
                MostrarSolo(DoorPromptPanel);

                if (InputManagerCustom.PressB())
                {
                    Debug.Log("Abriendo/cerrando cajón con B: " + drawer.gameObject.name);
                    drawer.ToggleDrawer();
                    return;
                }

                return;
            }

            // 7. Objetos inspeccionables 360
            InspectableObject360 inspectable = hit.collider.GetComponentInParent<InspectableObject360>();

            if (inspectable != null)
            {
                MostrarSolo(TextDetect);

                if (InputManagerCustom.PressB())
                {
                    ApagarTodosLosPrompts();
                    inspectable.StartInspection();
                    return;
                }

                return;
            }

            // 8. Objeto interactivo simple
            ObjetoInteractivo objeto = hit.collider.GetComponentInParent<ObjetoInteractivo>();

            if (objeto != null)
            {
                MostrarSolo(TextDetect);

                if (InputManagerCustom.PressB())
                {
                    objeto.ActivarObjeto();
                    return;
                }

                return;
            }

            ApagarTodosLosPrompts();
        }
        else
        {
            Deselect();
            LimpiarMiradas();
            ApagarTodosLosPrompts();

            if (pointer3D != null)
                pointer3D.SetDetected(false);
        }
    }

    void MostrarSolo(GameObject prompt)
    {
        ApagarTodosLosPrompts();

        if (prompt != null)
            prompt.SetActive(true);
    }

    void ApagarTodosLosPrompts()
    {
        if (TextDetect != null) TextDetect.SetActive(false);
        if (DoorPromptPanel != null) DoorPromptPanel.SetActive(false);
        if (MissionPromptPanel != null) MissionPromptPanel.SetActive(false);
        if (ClothingPromptPanel != null) ClothingPromptPanel.SetActive(false);
    }

    void LimpiarMiradas()
    {
        ApagarNPCMirado();
        ApagarTelefonoMirado();
    }

    void ApagarNPCMirado()
    {
        if (ultimoNPCMirado != null)
        {
            ultimoNPCMirado.SetMirandoNPC(false);
            ultimoNPCMirado = null;
        }
    }

    void ApagarTelefonoMirado()
    {
        if (ultimoTelefonoMirado != null)
        {
            ultimoTelefonoMirado.SetMirandoTelefono(false);
            ultimoTelefonoMirado = null;
        }
    }

    void SelectedObject(Collider colliderDetectado)
    {
        Transform raizSeleccion = colliderDetectado.transform;

        ClosetMissionTrigger closet = colliderDetectado.GetComponentInParent<ClosetMissionTrigger>();
        if (closet != null)
            raizSeleccion = closet.transform;

        DoorInteractable door = colliderDetectado.GetComponentInParent<DoorInteractable>();
        if (door != null)
            raizSeleccion = door.transform;

        DrawerInteractable drawer = colliderDetectado.GetComponentInParent<DrawerInteractable>();
        if (drawer != null)
            raizSeleccion = drawer.transform;

        ClosetClothingItem clothing = colliderDetectado.GetComponentInParent<ClosetClothingItem>();
        if (clothing != null)
            raizSeleccion = clothing.transform;

        PhoneMissionController phone = colliderDetectado.GetComponentInParent<PhoneMissionController>();
        if (phone != null)
            raizSeleccion = phone.transform;

        ultimoReconocido = raizSeleccion.gameObject;

        renderersActuales = raizSeleccion.GetComponentsInChildren<Renderer>();
        coloresOriginales = new Color[renderersActuales.Length];

        for (int i = 0; i < renderersActuales.Length; i++)
        {
            if (renderersActuales[i] != null && renderersActuales[i].material.HasProperty("_Color"))
            {
                coloresOriginales[i] = renderersActuales[i].material.color;
                renderersActuales[i].material.color = colorSeleccion;
            }
        }
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