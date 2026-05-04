using UnityEngine;

/// <summary>
/// Selected controla la detección por mirada/raycast del jugador.
/// 
/// Regla oficial de botones:
/// - B = interactuar con objetos, puertas, cajones, ropa e inspecciones.
/// - A = iniciar misión o diálogo con NPC.
/// - X = salir/cerrar/cancelar. NO se usa aquí para interactuar.
/// - Y = queda disponible para opciones adicionales.
/// </summary>
public class Selected : MonoBehaviour
{
    private LogicaNPC ultimoNPCMirado;

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
            LogicaNPC npc = hit.collider.GetComponentInParent<LogicaNPC>();

            if (npc != null)
            {
                // Si cambió el NPC que estás mirando
                if (ultimoNPCMirado != npc)
                {
                    // Apagar el anterior
                    if (ultimoNPCMirado != null)
                    {
                        ultimoNPCMirado.SetMirandoNPC(false);
                    }

                    // Guardar el nuevo
                    ultimoNPCMirado = npc;
                }

                // Activar el actual
                ultimoNPCMirado.SetMirandoNPC(true);

                // 🔴 IMPORTANTE: apagar otros prompts
                ApagarTodosLosPrompts();

                return;
            }
            // Si ya no estás mirando un NPC
            if (ultimoNPCMirado != null)
            {
                ultimoNPCMirado.SetMirandoNPC(false);
                ultimoNPCMirado = null;
            }

            // 2. Prendas del clóset: B selecciona ropa
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

            // 3. Clóset / misión: A inicia misión
            ClosetMissionTrigger closet = hit.collider.GetComponent<ClosetMissionTrigger>();
            if (closet == null)
                closet = hit.collider.GetComponentInParent<ClosetMissionTrigger>();

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

            // 4. Puertas: B abre/cierra
            DoorInteractable door = hit.collider.GetComponent<DoorInteractable>();
            if (door == null)
                door = hit.collider.GetComponentInParent<DoorInteractable>();

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

            // 5. Cajones: B abre/cierra
            DrawerInteractable drawer = hit.collider.GetComponent<DrawerInteractable>();
            if (drawer == null)
                drawer = hit.collider.GetComponentInParent<DrawerInteractable>();

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

            // 6. Objetos inspeccionables 360: B inspecciona
            InspectableObject360 inspectable = hit.collider.GetComponentInParent<InspectableObject360>();

            if (inspectable != null)
            {
                MostrarSolo(TextDetect);

                if (InputManagerCustom.PressB())
                {
                    ApagarPrompts();
                    inspectable.StartInspection();
                    return;
                }

                return;
            }

            // 7. Objeto interactivo simple: B activa
            ObjetoInteractivo objeto = hit.collider.GetComponent<ObjetoInteractivo>();
            if (objeto == null)
                objeto = hit.collider.GetComponentInParent<ObjetoInteractivo>();

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

            // 8. Sentarse / Focus Point: B activa
            SitFocusPointInteractable sit = hit.collider.GetComponentInParent<SitFocusPointInteractable>();

            if (sit != null)
            {
                MostrarSolo(TextDetect);

                if (InputManagerCustom.PressB())
                {
                    ApagarPrompts();
                    sit.Sit();
                    return;
                }

                return;
            }

            ApagarPrompts();
        }
        else
        {
            Deselect();
            ApagarPrompts();

            if (pointer3D != null)
                pointer3D.SetDetected(false);

            if (ultimoNPCMirado != null)
            {
                ultimoNPCMirado.SetMirandoNPC(false);
                ultimoNPCMirado = null;
            }
        }
    }

    /// <summary>
    /// Muestra solo un prompt y apaga los demás.
    /// </summary>
    void MostrarSolo(GameObject prompt)
    {
        ApagarTodosLosPrompts();

        if (prompt != null)
            prompt.SetActive(true);
    }

    /// <summary>
    /// Apaga todos los prompts visibles.
    /// </summary>
    void ApagarPrompts()
    {
        ApagarTodosLosPrompts();
    }

    /// <summary>
    /// Apaga todos los paneles de ayuda/interacción.
    /// </summary>
    void ApagarTodosLosPrompts()
    {
        if (TextDetect != null) TextDetect.SetActive(false);
        if (DoorPromptPanel != null) DoorPromptPanel.SetActive(false);
        if (MissionPromptPanel != null) MissionPromptPanel.SetActive(false);
        if (ClothingPromptPanel != null) ClothingPromptPanel.SetActive(false);
    }

    /// <summary>
    /// Marca visualmente el objeto que el jugador está mirando.
    /// Si el objeto pertenece a un interactuable grande, como el clóset,
    /// intenta seleccionar el objeto padre completo.
    /// </summary>
    void SelectedObject(Collider colliderDetectado)
    {
        Transform raizSeleccion = colliderDetectado.transform;

        // Si pertenece a una misión de clóset, selecciona todo el clóset.
        ClosetMissionTrigger closet = colliderDetectado.GetComponentInParent<ClosetMissionTrigger>();
        if (closet != null)
        {
            raizSeleccion = closet.transform;
        }

        // Si pertenece a una puerta, selecciona la puerta completa.
        DoorInteractable door = colliderDetectado.GetComponentInParent<DoorInteractable>();
        if (door != null)
        {
            raizSeleccion = door.transform;
        }

        // Si pertenece a un cajón, selecciona el cajón completo.
        DrawerInteractable drawer = colliderDetectado.GetComponentInParent<DrawerInteractable>();
        if (drawer != null)
        {
            raizSeleccion = drawer.transform;
        }

        // Si pertenece a una prenda, selecciona la prenda completa.
        ClosetClothingItem clothing = colliderDetectado.GetComponentInParent<ClosetClothingItem>();
        if (clothing != null)
        {
            raizSeleccion = clothing.transform;
        }

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

    /// <summary>
    /// Quita la selección visual y devuelve los colores originales.
    /// </summary>
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