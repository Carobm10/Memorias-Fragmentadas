using UnityEngine;

/// <summary>
/// SELECTED.CS
/// Script principal de selección por mirada/raycast.
/// Va en la Main Camera.
/// 
/// Este script detecta objetos en la capa "Raycast Detect" y decide qué interacción ejecutar:
/// - NPC normal
/// - NPC de misión de la muchacha del servicio
/// - Teléfono
/// - Radio
/// - Tapa de radio
/// - Pilas del cajón
/// - Pilas para insertar en radio
/// - Prendas del clóset
/// - Misión del clóset
/// - Puertas
/// - Cajones
/// - Objetos 360
/// - Objetos simples
/// </summary>
public class Selected : MonoBehaviour
{
    // =========================
    // ÚLTIMOS OBJETOS MIRADOS
    // =========================

    private LogicaNPC ultimoNPCMirado;
    private RadioFinalUse ultimaRadioFinalMirada;
    private PhoneMissionController ultimoTelefonoMirado;
    private ServicioNPCMission ultimaMuchachaMirada;
    private RadioMissionInteractable ultimaRadioMirada;
    private RadioBackCover ultimaTapaRadioMirada;
    private BatteryPickup ultimasPilasPickupMiradas;
    private RadioBatteryInstaller ultimaPilaInstallerMirada;
    private RadioBatteryTrigger ultimaPilaRadioMirada;
    private RadioCoverTrigger ultimaTapaRadioTriggerMirada;
    private RadioAnimacionesSimple ultimaRadioAnimacionSimpleMirada;

    // =========================
    // PUNTERO 3D
    // =========================
    //[Header("Sistema de selección")]
    //public Selected selectedRaycast;

    [Header("Puntero 3D")]
    public Pointer3DController pointer3D;

    // =========================
    // RAYCAST
    // =========================

    [Header("Raycast")]
    public float distancia = 3f;
    private LayerMask mask;

    // =========================
    // PROMPTS / CANVASES
    // =========================

    [Header("Prompts")]
    public GameObject TextDetect;
    public GameObject DoorPromptPanel;
    public GameObject MissionPromptPanel;
    public GameObject ClothingPromptPanel;

    // =========================
    // MANAGER MISIÓN ARMARIO
    // =========================

    [Header("Manager de canvases")]
    public ClosetCanvasManager closetCanvasManager;

    // =========================
    // HIGHLIGHT
    // =========================

    private GameObject ultimoReconocido;
    private Renderer[] renderersActuales;
    private Color[] coloresOriginales;

    [Header("Color de selección")]
    public Color colorSeleccion = new Color(0.1f, 1f, 0.25f, 1f);

    // =========================
    // INICIO
    // =========================

    void Awake()
    {
        ApagarPrompts();
    }

    void Start()
    {
        mask = LayerMask.GetMask("Raycast Detect");
        ApagarTodosLosPrompts();
    }

    // =========================
    // UPDATE PRINCIPAL
    // =========================

    void Update()
    {
        // Si hay UI del armario abierta, no dejamos seleccionar nada del mundo.
        if (closetCanvasManager != null && closetCanvasManager.uiAbierta)
        {
            LimpiarMiradas();
            ApagarTodosLosPrompts();
            Deselect();

            if (pointer3D != null)
                pointer3D.SetDetected(false);

            return;
        }

        RaycastHit hit;

        // Lanzamos raycast desde la cámara hacia adelante.
        if (Physics.Raycast(transform.position, transform.forward, out hit, distancia, mask))
        {
            if (pointer3D != null)
                pointer3D.SetDetected(true);

            GameObject objetoDetectado = hit.collider.gameObject;

            // Detectamos componentes generales.
            LogicaNPC npc = hit.collider.GetComponentInParent<LogicaNPC>();
            InspectableObject360 inspectable = hit.collider.GetComponentInParent<InspectableObject360>();

            // Si el NPC existe pero no puede seleccionarse, limpiamos todo.
            bool npcBloqueado = npc != null && !npc.PuedeSerSeleccionado();

            if (npcBloqueado)
            {
                LimpiarMiradas();
                Deselect();
                ApagarTodosLosPrompts();

                if (pointer3D != null)
                    pointer3D.SetDetected(false);

                return;
            }

            // Si un objeto 360 ya está en inspección, no seleccionamos nada más.
            if (inspectable != null && inspectable.IsInspecting())
            {
                LimpiarMiradas();
                Deselect();

                if (pointer3D != null)
                    pointer3D.SetDetected(false);

                return;
            }

            // Aplicamos highlight si cambió el objeto mirado.
            if (ultimoReconocido != objetoDetectado)
            {
                Deselect();
                SelectedObject(hit.collider);
            }

            // ======================================================
            // 1. MUCHACHA DEL SERVICIO - MISIÓN RADIO
            // ======================================================

            ServicioNPCMission muchacha = hit.collider.GetComponentInParent<ServicioNPCMission>();

            if (muchacha != null)
            {
                LimpiarTodoMenosMuchacha(muchacha);

                ultimaMuchachaMirada = muchacha;
                ultimaMuchachaMirada.SetLookingAtMe(true);

                //ApagarTodosLosPrompts();
                return;
            }
            else
            {
                ApagarMuchachaMirada();
            }

            // ======================================================
            // 2. RADIO - MISIÓN RADIO
            // ======================================================

            RadioAnimacionesSimple radioAnimSimple = hit.collider.GetComponentInParent<RadioAnimacionesSimple>();

            if (radioAnimSimple != null)
            {
                if (ultimaRadioAnimacionSimpleMirada != null && ultimaRadioAnimacionSimpleMirada != radioAnimSimple)
                    ultimaRadioAnimacionSimpleMirada.DejarMirarRadio();

                ultimaRadioAnimacionSimpleMirada = radioAnimSimple;
                radioAnimSimple.MirarRadio();

                return;
            }
            else
            {
                ApagarRadioAnimacionSimpleMirada();
            }

            RadioMissionInteractable radio = hit.collider.GetComponentInParent<RadioMissionInteractable>();

            if (radio != null)
            {
                LimpiarTodoMenosRadio(radio);

                ultimaRadioMirada = radio;
                ultimaRadioMirada.LookAtRadio();

                return;
            }
            else
            {
                ApagarRadioMirada();
            }

            

            // ======================================================
            // 3. TAPA TRASERA DE LA RADIO
            // ======================================================

            RadioBackCover tapaRadio = hit.collider.GetComponentInParent<RadioBackCover>();

            if (tapaRadio != null)
            {
                LimpiarTodoMenosTapaRadio(tapaRadio);

                ultimaTapaRadioMirada = tapaRadio;
                ultimaTapaRadioMirada.LookAtCover();

                return;
            }
            else
            {
                ApagarTapaRadioMirada();
            }

            // ======================================================
            // 4. PILAS DENTRO DEL CAJÓN
            // ======================================================

            BatteryPickup pilasPickup = hit.collider.GetComponentInParent<BatteryPickup>();

            if (pilasPickup != null)
            {
                LimpiarTodoMenosPilasPickup(pilasPickup);

                ultimasPilasPickupMiradas = pilasPickup;
                ultimasPilasPickupMiradas.LookAtBatteries();

                return;
            }
            else
            {
                ApagarPilasPickupMiradas();
            }

            // ======================================================
            // 5. PILAS PARA INSERTAR EN RADIO
            // ======================================================

            RadioBatteryInstaller pilaInstaller = hit.collider.GetComponentInParent<RadioBatteryInstaller>();

            if (pilaInstaller != null)
            {
                LimpiarTodoMenosPilaInstaller(pilaInstaller);

                ultimaPilaInstallerMirada = pilaInstaller;
                ultimaPilaInstallerMirada.LookAtBattery();

                return;
            }
            else
            {
                ApagarPilaInstallerMirada();
            }

            // ======================================================
            // 6. NPC NORMAL
            // ======================================================

            if (npc != null)
            {
                LimpiarTodoMenosNPC(npc);

                ultimoNPCMirado = npc;
                ultimoNPCMirado.SetMirandoNPC(true);

                ApagarTodosLosPrompts();
                return;
            }
            else
            {
                ApagarNPCMirado();
            }

            // ======================================================
            // 7. TELÉFONO
            // ======================================================

            PhoneMissionController phone = hit.collider.GetComponentInParent<PhoneMissionController>();

            if (phone != null)
            {
                LimpiarTodoMenosTelefono(phone);

                ultimoTelefonoMirado = phone;
                ultimoTelefonoMirado.SetMirandoTelefono(true);

                return;
            }
            else
            {
                ApagarTelefonoMirado();
            }

            // ======================================================
            // 8. PRENDAS DEL CLÓSET
            // ======================================================

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

            // ======================================================
            // 9. CLÓSET / MISIÓN ARMARIO
            // ======================================================

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

            // ======================================================
            // 10. PUERTAS
            // ======================================================

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

            // ======================================================
            // 11. CAJONES
            // ======================================================

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
            // ======================================================
            // TAPA INTERACTIVA DE LA RADIO
            // ======================================================

            RadioCoverTrigger tapaRadioTrigger = hit.collider.GetComponentInParent<RadioCoverTrigger>();

            if (tapaRadioTrigger != null)
            {
                if (ultimaTapaRadioTriggerMirada != null && ultimaTapaRadioTriggerMirada != tapaRadioTrigger)
                {
                    ultimaTapaRadioTriggerMirada.DejarMirarTapa();
                }

                ultimaTapaRadioTriggerMirada = tapaRadioTrigger;
                tapaRadioTrigger.MirarTapa();

                return;
            }
            else
            {
                ApagarTapaRadioTriggerMirada();
            }

            RadioFinalUse radioFinal = hit.collider.GetComponentInParent<RadioFinalUse>();

            if (radioFinal != null)
            {
                if (ultimaRadioFinalMirada != null && ultimaRadioFinalMirada != radioFinal)
                    ultimaRadioFinalMirada.DejarMirarRadioFinal();

                ultimaRadioFinalMirada = radioFinal;
                radioFinal.MirarRadioFinal();

                return;
            }
            else
            {
                ApagarRadioFinalMirada();
            }

            

            // ======================================================
            // 12. OBJETOS INSPECCIONABLES 360
            // ======================================================

            if (inspectable != null)
            {
                MostrarSolo(TextDetect);

                if (InputManagerCustom.PressB())
                {
                    ApagarTodosLosPrompts();
                    inspectable.StartInspection();
                    Deselect();
                    return;
                }

                return;
            }
            // ======================================================
            // 13. PILAS INTERACTIVAS RADIO
            // ======================================================

            RadioBatteryTrigger pilaRadio = hit.collider.GetComponentInParent<RadioBatteryTrigger>();

            if (pilaRadio != null)
            {
                if (ultimaPilaRadioMirada != null && ultimaPilaRadioMirada != pilaRadio)
                {
                    ultimaPilaRadioMirada.DejarMirarPila();
                }

                ultimaPilaRadioMirada = pilaRadio;

                pilaRadio.MirarPila();

                return;
            }
            else
            {
                ApagarPilaRadioMirada();
            }

            // ======================================================
            // 13. OBJETO INTERACTIVO SIMPLE
            // ======================================================

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

            // Si no es ningún objeto válido, limpiamos.
            ApagarTodosLosPrompts();
        }
        else
        {
            // Si el raycast no toca nada:
            LimpiarMiradas();
            Deselect();
            ApagarTodosLosPrompts();

            if (pointer3D != null)
                pointer3D.SetDetected(false);
        }
    }

    // ======================================================
    // HIGHLIGHT DEL OBJETO MIRADO
    // ======================================================

    void SelectedObject(Collider col)
    {
        ultimoReconocido = col.gameObject;

        HighlightGroup group = col.GetComponentInParent<HighlightGroup>();

        if (group != null)
        {
            renderersActuales = group.GetComponentsInChildren<Renderer>();
        }
        else
        {
            renderersActuales = col.GetComponentsInChildren<Renderer>();
        }

        if (renderersActuales == null || renderersActuales.Length == 0)
            return;

        coloresOriginales = new Color[renderersActuales.Length];

        for (int i = 0; i < renderersActuales.Length; i++)
        {
            if (renderersActuales[i] != null && renderersActuales[i].material != null)
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
                if (renderersActuales[i] != null && renderersActuales[i].material != null)
                {
                    renderersActuales[i].material.color = coloresOriginales[i];
                }
            }
        }

        renderersActuales = null;
        coloresOriginales = null;
        ultimoReconocido = null;
    }

    // ======================================================
    // PROMPTS
    // ======================================================

    void MostrarSolo(GameObject panel)
    {
        ApagarTodosLosPrompts();

        if (panel != null)
            panel.SetActive(true);
    }

    void ApagarPrompts()
    {
        ApagarTodosLosPrompts();
    }

    void ApagarTodosLosPrompts()
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

    // ======================================================
    // LIMPIEZA GENERAL DE MIRADAS
    // ======================================================

    void LimpiarMiradas()
    {
        ApagarNPCMirado();
        ApagarTelefonoMirado();
        ApagarMuchachaMirada();
        ApagarRadioMirada();
        ApagarTapaRadioMirada();
        ApagarPilasPickupMiradas();
        ApagarPilaInstallerMirada();
        ApagarPilaRadioMirada();
        ApagarTapaRadioTriggerMirada();
        ApagarRadioFinalMirada();
        ApagarRadioAnimacionSimpleMirada();
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

    void ApagarMuchachaMirada()
    {
        if (ultimaMuchachaMirada != null)
        {
            ultimaMuchachaMirada.SetLookingAtMe(false);
            ultimaMuchachaMirada = null;
        }
    }

    void ApagarRadioMirada()
    {
        if (ultimaRadioMirada != null)
        {
            ultimaRadioMirada.StopLookingAtRadio();
            ultimaRadioMirada = null;
        }
    }

    void ApagarTapaRadioMirada()
    {
        if (ultimaTapaRadioMirada != null)
        {
            ultimaTapaRadioMirada.StopLookingAtCover();
            ultimaTapaRadioMirada = null;
        }
    }

    void ApagarPilasPickupMiradas()
    {
        if (ultimasPilasPickupMiradas != null)
        {
            ultimasPilasPickupMiradas.StopLookingAtBatteries();
            ultimasPilasPickupMiradas = null;
        }
    }

    void ApagarPilaInstallerMirada()
    {
        if (ultimaPilaInstallerMirada != null)
        {
            ultimaPilaInstallerMirada.StopLookingAtBattery();
            ultimaPilaInstallerMirada = null;
        }
    }

    void ApagarPilaRadioMirada()
    {
        if (ultimaPilaRadioMirada != null)
        {
            ultimaPilaRadioMirada.DejarMirarPila();
            ultimaPilaRadioMirada = null;
        }
    }

    // ======================================================
    // LIMPIEZAS ESPECÍFICAS
    // Estas evitan que dos objetos crean que los estás mirando al mismo tiempo.
    // ======================================================

    void LimpiarTodoMenosMuchacha(ServicioNPCMission actual)
    {
        ApagarNPCMirado();
        ApagarTelefonoMirado();
        ApagarRadioMirada();
        ApagarTapaRadioMirada();
        ApagarPilasPickupMiradas();
        ApagarPilaInstallerMirada();
        ApagarTapaRadioTriggerMirada();

        if (ultimaMuchachaMirada != null && ultimaMuchachaMirada != actual)
            ultimaMuchachaMirada.SetLookingAtMe(false);
    }

    void LimpiarTodoMenosRadio(RadioMissionInteractable actual)
    {
        ApagarNPCMirado();
        ApagarTelefonoMirado();
        ApagarMuchachaMirada();
        ApagarTapaRadioMirada();
        ApagarPilasPickupMiradas();
        ApagarPilaInstallerMirada();

        if (ultimaRadioMirada != null && ultimaRadioMirada != actual)
            ultimaRadioMirada.StopLookingAtRadio();
    }

    void LimpiarTodoMenosTapaRadio(RadioBackCover actual)
    {
        ApagarNPCMirado();
        ApagarTelefonoMirado();
        ApagarMuchachaMirada();
        ApagarRadioMirada();
        ApagarPilasPickupMiradas();
        ApagarPilaInstallerMirada();
        ApagarTapaRadioTriggerMirada();

        if (ultimaTapaRadioMirada != null && ultimaTapaRadioMirada != actual)
            ultimaTapaRadioMirada.StopLookingAtCover();
    }

    void LimpiarTodoMenosPilasPickup(BatteryPickup actual)
    {
        ApagarNPCMirado();
        ApagarTelefonoMirado();
        ApagarMuchachaMirada();
        ApagarRadioMirada();
        ApagarTapaRadioMirada();
        ApagarPilaInstallerMirada();
        ApagarTapaRadioTriggerMirada();

        if (ultimasPilasPickupMiradas != null && ultimasPilasPickupMiradas != actual)
            ultimasPilasPickupMiradas.StopLookingAtBatteries();
    }

    void LimpiarTodoMenosPilaInstaller(RadioBatteryInstaller actual)
    {
        ApagarNPCMirado();
        ApagarTelefonoMirado();
        ApagarMuchachaMirada();
        ApagarRadioMirada();
        ApagarTapaRadioMirada();
        ApagarPilasPickupMiradas();
        ApagarTapaRadioTriggerMirada();

        if (ultimaPilaInstallerMirada != null && ultimaPilaInstallerMirada != actual)
            ultimaPilaInstallerMirada.StopLookingAtBattery();
    }

    void LimpiarTodoMenosNPC(LogicaNPC actual)
    {
        ApagarTelefonoMirado();
        ApagarMuchachaMirada();
        ApagarRadioMirada();
        ApagarTapaRadioMirada();
        ApagarPilasPickupMiradas();
        ApagarPilaInstallerMirada();
        ApagarTapaRadioTriggerMirada();

        if (ultimoNPCMirado != null && ultimoNPCMirado != actual)
            ultimoNPCMirado.SetMirandoNPC(false);
    }

    void LimpiarTodoMenosTelefono(PhoneMissionController actual)
    {
        ApagarNPCMirado();
        ApagarMuchachaMirada();
        ApagarRadioMirada();
        ApagarTapaRadioMirada();
        ApagarPilasPickupMiradas();
        ApagarPilaInstallerMirada();
        ApagarTapaRadioTriggerMirada();

        if (ultimoTelefonoMirado != null && ultimoTelefonoMirado != actual)
            ultimoTelefonoMirado.SetMirandoTelefono(false);
    }
    void ApagarTapaRadioTriggerMirada()
    {
        if (ultimaTapaRadioTriggerMirada != null)
        {
            ultimaTapaRadioTriggerMirada.DejarMirarTapa();
            ultimaTapaRadioTriggerMirada = null;
        }
    }
    void ApagarRadioFinalMirada()
    {
        if (ultimaRadioFinalMirada != null)
        {
            ultimaRadioFinalMirada.DejarMirarRadioFinal();
            ultimaRadioFinalMirada = null;
        }
    }

    void ApagarRadioAnimacionSimpleMirada()
    {
        if (ultimaRadioAnimacionSimpleMirada != null)
        {
            ultimaRadioAnimacionSimpleMirada.DejarMirarRadio();
            ultimaRadioAnimacionSimpleMirada = null;
        }
    }
}