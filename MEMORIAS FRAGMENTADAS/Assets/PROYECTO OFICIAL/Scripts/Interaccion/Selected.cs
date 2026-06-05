using UnityEngine;
using TMPro;

/// <summary>
/// SELECTED.CS
/// Va en la Main Camera.
/// Detecta objetos con Raycast y llama las interacciones correspondientes.
/// </summary>
public class Selected : MonoBehaviour
{
    private TypewriterInteractable currentTypewriter;
    [Header("Raycast")]
    public float distancia = 3f;
    private LayerMask mask;

    [Header("Puntero 3D")]
    public Pointer3DController pointer3D;

    [Header("Prompts generales")]
    public GameObject TextDetect;
    public GameObject DoorPromptPanel;
    public GameObject MissionPromptPanel;
    public GameObject ClothingPromptPanel;

    [Header("Manager armario")]
    public ClosetCanvasManager closetCanvasManager;

    [Header("Color selección")]
    public Color colorSeleccion = new Color(0.1f, 1f, 0.25f, 1f);

    private GameObject ultimoReconocido;
    private Renderer[] renderersActuales;
    private Color[] coloresOriginales;
    private string[] propiedadesColorOriginal;

    private ServicioNPCMission ultimaMuchachaMirada;
    private RosaFinalDialogue ultimaRosaFinalMirada;
    private PhotoVideoInteractable ultimaFotoVideoMirada;
    private RadioAnimacionesSimple ultimaRadioMirada;
    private RadioCoverTrigger ultimaTapaMirada;
    private BatteryPickup ultimasPilasMiradas;
    private RadioBatteryInsertTrigger ultimaPilaInsertMirada;
    private RadioKnobInteractable ultimaPerillaMirada;
    private PhoneMissionController ultimaTelefonoMirada;
    private ClosetMissionTrigger ultimaClosetMirada;

    private GameObject ultimoGenericoMirado;
    private Collider lastHitCollider;
    private Component[] lastHitCache;

    void Awake()
    {
        ApagarPromptsIniciales();
    }

    void Start()
    {
        mask = LayerMask.GetMask("Raycast Detect", "PickupItem");
        ApagarPromptsIniciales();
    }

    void Update()
    {
        if (closetCanvasManager != null && closetCanvasManager.uiAbierta)
        {
            LimpiarMiradas();
            Deselect();

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
            Debug.Log(
                "[SELECTED DEBUG] Estoy mirando: " + hit.collider.name +
                " | Padre: " + hit.collider.transform.parent?.name +
                " | Root: " + hit.collider.transform.root.name +
                " | Tiene TypewriterInteractable: " + (hit.collider.GetComponentInParent<TypewriterInteractable>() != null)
            );

            // if (ultimoReconocido != objetoDetectado)
            // {
            //    Deselect();
            //     SelectedObject(hit.collider);
            // }
            
            // ======================================================
            // MISIÓN PAPÁ / MÁQUINA DE ESCRIBIR
            // ======================================================
            TypewriterInteractable typewriter = hit.collider.GetComponentInParent<TypewriterInteractable>();

            if (typewriter != null)
            {
                if (TextDetect != null)
                {
                    TextDetect.SetActive(false);
                }
                if (typewriter.isWritingMode)
                {
                    Deselect();
                    LimpiarGenerico();
                    OcultarPromptPuertaOCajon();
                    ApagarMissionPrompt();
                    return;
                }

                GameObject maquinaCompleta = typewriter.gameObject;

                if (ultimoReconocido != maquinaCompleta)
                {
                    Deselect();
                    SelectedObject(maquinaCompleta.GetComponent<Collider>());
                }

                LimpiarGenerico();
                OcultarPromptPuertaOCajon();

                if (currentTypewriter != typewriter)
                {
                    if (currentTypewriter != null)
                    {
                        currentTypewriter.HidePrompt();
                    }

                    currentTypewriter = typewriter;
                    currentTypewriter.ShowPrompt();
                }

                return;
            }

            if (ultimoReconocido != objetoDetectado)
            {
                Deselect();
                SelectedObject(hit.collider);
            }

            // ======================================================
            // MISIÓN MAMÁ / MONEDERO
            // ======================================================
            WalletPickup wallet = hit.collider.GetComponentInParent<WalletPickup>();

            if (wallet != null)
            {
                LimpiarGenerico();
                ApagarMuchacha();
                ApagarFotoVideo();
                ApagarTapa();
                ApagarPilaInsert();
                ApagarPilasPickup();
                ApagarPerilla();
                ApagarRadio();
                ApagarRosaFinal();
                ApagarTelefono();
                ApagarClosetMission();

                MostrarPromptMision("Presiona B para tomar monedero");

                if (InputManagerCustom.PressB())
                {
                    wallet.RecogerMonedero();
                }

                return;
            }

            // ======================================================
            // MISIÓN MAMÁ / PUERTA PERIÓDICO
            // ======================================================
            NewspaperDoorTrigger puertaPeriodico = hit.collider.GetComponentInParent<NewspaperDoorTrigger>();

            if (puertaPeriodico != null)
            {
                LimpiarGenerico();
                ApagarMuchacha();
                ApagarFotoVideo();
                ApagarTapa();
                ApagarPilaInsert();
                ApagarPilasPickup();
                ApagarPerilla();
                ApagarRadio();
                ApagarRosaFinal();
                ApagarTelefono();
                ApagarClosetMission();

                MostrarPromptMision("Presiona B para salir por el periódico");

                if (InputManagerCustom.PressB())
                {
                    puertaPeriodico.InteractuarPuertaPeriodico();
                }

                return;
            }

            // ======================================================
            // PERIÓDICO / LECTURA
            // ======================================================
            PeriodicoLectura periodico = hit.collider.GetComponentInParent<PeriodicoLectura>();

            if (periodico != null)
            {
                LimpiarGenerico();
                ApagarMuchacha();
                ApagarFotoVideo();
                ApagarTapa();
                ApagarPilaInsert();
                ApagarPilasPickup();
                ApagarPerilla();
                ApagarRadio();
                ApagarRosaFinal();
                ApagarTelefono();
                ApagarClosetMission();

                MostrarPromptMision(
                    periodico.EstaEnLectura()
                    ? "Presiona B para pasar hoja / X para salir"
                    : "Presiona B para ver periódico"
                );

                if (InputManagerCustom.PressB())
                {
                    periodico.InteractuarPeriodico();
                }

                if (InputManagerCustom.PressX())
                {
                    periodico.SalirLectura();
                }

                return;
            }

            // ======================================================
            // PUERTAS
            // ======================================================
            DoorInteractable puerta = hit.collider.GetComponentInParent<DoorInteractable>();

            if (puerta != null)
            {
                MostrarPromptPuertaOCajon(puerta.isOpen ? "Presiona B para cerrar puerta" : "Presiona B para abrir puerta");

                if (InputManagerCustom.PressB())
                {
                    puerta.ToggleDoor();
                }

                return;
            }

            // ======================================================
            // OBJETOS DENTRO DE CAJONES
            // ======================================================
            DrawerItemPickup itemCajon = hit.collider.GetComponent<DrawerItemPickup>();
            if (itemCajon == null)
                itemCajon = hit.collider.GetComponentInParent<DrawerItemPickup>();

            if (itemCajon != null && itemCajon.PuedeSacarse())
            {
                OcultarPromptPuertaOCajon();
                LimpiarGenerico();

                MostrarPromptPuertaOCajon("Presiona B para sacar objeto");

                if (InputManagerCustom.PressB())
                {
                    itemCajon.SacarParaInspeccion();
                }

                return;
            }

            // ======================================================
            // CAJONES
            // ======================================================
            DrawerInteractable cajon = hit.collider.GetComponentInParent<DrawerInteractable>();

            if (cajon != null)
            {
                MostrarPromptPuertaOCajon(cajon.isOpen ? "Presiona B para cerrar cajón" : "Presiona B para abrir cajón");

                if (InputManagerCustom.PressB())
                {
                    cajon.ToggleDrawer();
                }

                return;
            }

            // ======================================================
            // MISIÓN MAMÁ / NPC MAMÁ
            // ======================================================
            MomMissionNPC momNPC = hit.collider.GetComponentInParent<MomMissionNPC>();

            if (momNPC != null)
            {
                LimpiarGenerico();
                ApagarMuchacha();
                ApagarFotoVideo();
                ApagarTapa();
                ApagarPilaInsert();
                ApagarPilasPickup();
                ApagarPerilla();
                ApagarRadio();
                ApagarRosaFinal();
                ApagarTelefono();
                ApagarClosetMission();

                MostrarPromptMision("Presiona A para hablar con mamá");

                if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.JoystickButton10))
                {
                    momNPC.Interactuar();
                }

                return;
            }

            // 1. ROSA FINAL
            RosaFinalDialogue rosaFinal = hit.collider.GetComponentInParent<RosaFinalDialogue>();

            if (rosaFinal != null && rosaFinal.dialogoDisponible)
            {
                LimpiarGenerico();
                ApagarMuchacha();
                ApagarFotoVideo();
                ApagarTapa();
                ApagarPilaInsert();
                ApagarPilasPickup();
                ApagarPerilla();
                ApagarRadio();

                if (ultimaRosaFinalMirada != null && ultimaRosaFinalMirada != rosaFinal)
                    ultimaRosaFinalMirada.SetLookingAtMe(false);

                ultimaRosaFinalMirada = rosaFinal;
                rosaFinal.SetLookingAtMe(true);
                return;
            }

            ApagarRosaFinal();

            // 2. ROSA MISIÓN INICIAL
            ServicioNPCMission muchacha = hit.collider.GetComponentInParent<ServicioNPCMission>();

            if (muchacha != null)
            {
                LimpiarGenerico();
                ApagarFotoVideo();
                ApagarTapa();
                ApagarPilaInsert();
                ApagarPilasPickup();
                ApagarPerilla();
                ApagarRadio();

                if (ultimaMuchachaMirada != null && ultimaMuchachaMirada != muchacha)
                    ultimaMuchachaMirada.SetLookingAtMe(false);

                ultimaMuchachaMirada = muchacha;
                muchacha.SetLookingAtMe(true);
                return;
            }

            ApagarMuchacha();

            // 3. FOTO / VIDEO
            PhotoVideoInteractable fotoVideo = hit.collider.GetComponentInParent<PhotoVideoInteractable>();

            if (fotoVideo != null)
            {
                LimpiarGenerico();
                ApagarTapa();
                ApagarPilaInsert();
                ApagarPilasPickup();
                ApagarPerilla();
                ApagarRadio();

                if (ultimaFotoVideoMirada != null && ultimaFotoVideoMirada != fotoVideo)
                    ultimaFotoVideoMirada.DejarMirarFoto();

                ultimaFotoVideoMirada = fotoVideo;
                fotoVideo.MirarFoto();
                return;
            }

            ApagarFotoVideo();

            // 4. TAPA RADIO
            RadioCoverTrigger tapa = hit.collider.GetComponentInParent<RadioCoverTrigger>();

            if (tapa != null)
            {
                if (tapa.radioAnimaciones != null && tapa.radioAnimaciones.RadioFinalActivo)
                {
                    // No interactuar con la tapa cuando ya estamos en la radio final para música.
                    ApagarTapa();
                    return;
                }

                LimpiarGenerico();
                ApagarPilaInsert();
                ApagarPilasPickup();
                ApagarPerilla();
                ApagarRadio();

                if (ultimaTapaMirada != null && ultimaTapaMirada != tapa)
                    ultimaTapaMirada.DejarMirarTapa();

                ultimaTapaMirada = tapa;
                tapa.MirarTapa();
                return;
            }

            ApagarTapa();

            // 5. PILAS PARA INSERTAR
            RadioBatteryInsertTrigger pilaInsert = hit.collider.GetComponentInParent<RadioBatteryInsertTrigger>();

            if (pilaInsert != null)
            {
                LimpiarGenerico();
                ApagarPilasPickup();
                ApagarPerilla();
                ApagarRadio();

                if (ultimaPilaInsertMirada != null && ultimaPilaInsertMirada != pilaInsert)
                    ultimaPilaInsertMirada.DejarMirarPila();

                ultimaPilaInsertMirada = pilaInsert;
                pilaInsert.MirarPila();
                return;
            }

            ApagarPilaInsert();

            // 6. PILAS DEL CAJÓN
            BatteryPickup pilas = hit.collider.GetComponentInParent<BatteryPickup>();

            if (pilas != null)
            {
                LimpiarGenerico();
                ApagarPerilla();
                ApagarRadio();

                if (ultimasPilasMiradas != null && ultimasPilasMiradas != pilas)
                    ultimasPilasMiradas.StopLookingAtBatteries();

                ultimasPilasMiradas = pilas;
                pilas.LookAtBatteries();
                return;
            }

            ApagarPilasPickup();

            // 7. PERILLAS RADIO MÚSICA
            RadioKnobInteractable perilla = hit.collider.GetComponentInParent<RadioKnobInteractable>();

            if (perilla != null)
            {
                LimpiarGenerico();
                ApagarRadio();

                if (ultimaPerillaMirada != null && ultimaPerillaMirada != perilla)
                    ultimaPerillaMirada.DejarMirarPerilla();

                ultimaPerillaMirada = perilla;
                perilla.MirarPerilla();
                return;
            }

            ApagarPerilla();

            // 8. MISIÓN DEL CLOSET
            ClosetMissionTrigger closetMission = hit.collider.GetComponentInParent<ClosetMissionTrigger>();

            if (closetMission != null)
            {
                LimpiarGenerico();
                ApagarMuchacha();
                ApagarFotoVideo();
                ApagarTapa();
                ApagarPilaInsert();
                ApagarPilasPickup();
                ApagarPerilla();
                ApagarRadio();
                ApagarRosaFinal();
                ApagarTelefono();

                if (ultimaClosetMirada != null && ultimaClosetMirada != closetMission)
                    ultimaClosetMirada = null;

                ultimaClosetMirada = closetMission;

                if (InputManagerCustom.PressA())
                {
                    closetMission.StartClosetMission();
                }
                else
                {
                    MostrarPromptMision("Presiona A para iniciar la misión del clóset");
                }

                return;
            }

            ApagarMissionPrompt();

            // 9. TELÉFONO
            PhoneMissionController telefono = hit.collider.GetComponentInParent<PhoneMissionController>();

            if (telefono != null)
            {
                LimpiarGenerico();
                ApagarMuchacha();
                ApagarFotoVideo();
                ApagarTapa();
                ApagarPilaInsert();
                ApagarPilasPickup();
                ApagarPerilla();
                ApagarRadio();
                ApagarRosaFinal();
                ApagarClosetMission();

                if (ultimaTelefonoMirada != null && ultimaTelefonoMirada != telefono)
                    ultimaTelefonoMirada.SetMirandoTelefono(false);

                ultimaTelefonoMirada = telefono;
                telefono.SetMirandoTelefono(true);
                return;
            }

            ApagarTelefono();

            // 9. RADIO PRINCIPAL
            RadioAnimacionesSimple radio = hit.collider.GetComponentInParent<RadioAnimacionesSimple>();

            if (radio != null)
            {
                OcultarPromptPuertaOCajon();
                LimpiarGenerico();

                if (ultimaRadioMirada != null && ultimaRadioMirada != radio)
                    ultimaRadioMirada.DejarMirarRadio();

                ultimaRadioMirada = radio;
                radio.MirarRadio();
                return;
            }

            ApagarRadio();

            // 10. OBJETOS INSPECCIONABLES 360
            InspectableObject360 inspectable360 = hit.collider.GetComponentInParent<InspectableObject360>();

            if (inspectable360 != null && !inspectable360.IsInspecting())
            {
                OcultarPromptPuertaOCajon();
                LimpiarGenerico();

                MostrarPromptPuertaOCajon("Presiona B para inspeccionar");

                if (InputManagerCustom.PressB())
                {
                    inspectable360.StartInspection();
                }

                return;
            }

            // OBJETOS GENERALES: puertas, cajones, teléfono, ropa.
            ManejarGenerico(hit.collider);
        }
        else
        {
            LimpiarMiradas();
            Deselect();

            OcultarPromptPuertaOCajon();

            if (pointer3D != null)
                pointer3D.SetDetected(false);
        }
    }

    void ManejarGenerico(Collider col)
    {
        if (ultimoGenericoMirado != null && ultimoGenericoMirado != col.gameObject)
            EnviarSalidaGenerica(ultimoGenericoMirado);

        ultimoGenericoMirado = col.gameObject;

        col.SendMessageUpwards("MirarPuerta", SendMessageOptions.DontRequireReceiver);
        col.SendMessageUpwards("LookAtDoor", SendMessageOptions.DontRequireReceiver);
        col.SendMessageUpwards("LookAtPuerta", SendMessageOptions.DontRequireReceiver);

        col.SendMessageUpwards("MirarCajon", SendMessageOptions.DontRequireReceiver);
        col.SendMessageUpwards("LookAtDrawer", SendMessageOptions.DontRequireReceiver);
        col.SendMessageUpwards("LookAtCajon", SendMessageOptions.DontRequireReceiver);

        col.SendMessageUpwards("MirarObjeto", SendMessageOptions.DontRequireReceiver);
        col.SendMessageUpwards("LookAtObject", SendMessageOptions.DontRequireReceiver);
        col.SendMessageUpwards("LookAtInspectable", SendMessageOptions.DontRequireReceiver);

        col.SendMessageUpwards("MirarTelefono", SendMessageOptions.DontRequireReceiver);
        col.SendMessageUpwards("LookAtPhone", SendMessageOptions.DontRequireReceiver);

        col.SendMessageUpwards("MirarPrenda", SendMessageOptions.DontRequireReceiver);
        col.SendMessageUpwards("LookAtClothing", SendMessageOptions.DontRequireReceiver);
        col.SendMessageUpwards("MirarRopa", SendMessageOptions.DontRequireReceiver);
    }

    void EnviarSalidaGenerica(GameObject obj)
    {
        if (obj == null) return;

        obj.SendMessageUpwards("DejarMirarPuerta", SendMessageOptions.DontRequireReceiver);
        obj.SendMessageUpwards("StopLookingAtDoor", SendMessageOptions.DontRequireReceiver);
        obj.SendMessageUpwards("StopLookAtDoor", SendMessageOptions.DontRequireReceiver);

        obj.SendMessageUpwards("DejarMirarCajon", SendMessageOptions.DontRequireReceiver);
        obj.SendMessageUpwards("StopLookingAtDrawer", SendMessageOptions.DontRequireReceiver);
        obj.SendMessageUpwards("StopLookAtDrawer", SendMessageOptions.DontRequireReceiver);

        obj.SendMessageUpwards("DejarMirarObjeto", SendMessageOptions.DontRequireReceiver);
        obj.SendMessageUpwards("StopLookingAtObject", SendMessageOptions.DontRequireReceiver);
        obj.SendMessageUpwards("StopLookAtObject", SendMessageOptions.DontRequireReceiver);

        obj.SendMessageUpwards("DejarMirarTelefono", SendMessageOptions.DontRequireReceiver);
        obj.SendMessageUpwards("StopLookingAtPhone", SendMessageOptions.DontRequireReceiver);

        obj.SendMessageUpwards("DejarMirarPrenda", SendMessageOptions.DontRequireReceiver);
        obj.SendMessageUpwards("StopLookingAtClothing", SendMessageOptions.DontRequireReceiver);
        obj.SendMessageUpwards("DejarMirarRopa", SendMessageOptions.DontRequireReceiver);

        obj.SendMessageUpwards("DejarMirarNPC", SendMessageOptions.DontRequireReceiver);
        obj.SendMessageUpwards("StopLookingAtNPC", SendMessageOptions.DontRequireReceiver);
    }

    void ApagarPromptsIniciales()
    {
        if (TextDetect != null) TextDetect.SetActive(false);
        if (DoorPromptPanel != null) DoorPromptPanel.SetActive(false);
        if (MissionPromptPanel != null) MissionPromptPanel.SetActive(false);
        if (ClothingPromptPanel != null) ClothingPromptPanel.SetActive(false);
    }
    void MostrarPromptPuertaOCajon(string mensaje)
    {
        if (DoorPromptPanel != null)
        {
            DoorPromptPanel.SetActive(true);

            TMP_Text texto = DoorPromptPanel.GetComponentInChildren<TMP_Text>(true);
            if (texto != null)
                texto.text = mensaje;
        }
    }

    void OcultarPromptPuertaOCajon()
    {
        if (DoorPromptPanel != null)
            DoorPromptPanel.SetActive(false);
    }

    void MostrarPromptMision(string mensaje)
    {
        if (MissionPromptPanel != null)
        {
            MissionPromptPanel.SetActive(true);
            TMP_Text texto = MissionPromptPanel.GetComponentInChildren<TMP_Text>(true);
            if (texto != null)
                texto.text = mensaje;
        }
    }

    void ApagarMissionPrompt()
    {
        if (MissionPromptPanel != null)
            MissionPromptPanel.SetActive(false);
    }

    void ApagarTelefono()
    {
        if (ultimaTelefonoMirada != null)
        {
            try { ultimaTelefonoMirada.SetMirandoTelefono(false); } catch { }
            ultimaTelefonoMirada = null;
        }
    }

    void ApagarClosetMission()
    {
        if (ultimaClosetMirada != null)
        {
            ultimaClosetMirada = null;
        }
    }

    // --- Helper methods missing earlier (added to fix compilation) ---
    void SelectedObject(Collider col)
    {
        if (col == null) return;

        ultimoReconocido = col.gameObject;

        // store renderers and original colors (only for materials that expose a color property)
        renderersActuales = ultimoReconocido.GetComponentsInChildren<Renderer>(true);
        if (renderersActuales != null && renderersActuales.Length > 0)
        {
            coloresOriginales = new Color[renderersActuales.Length];
            propiedadesColorOriginal = new string[renderersActuales.Length];

            for (int i = 0; i < renderersActuales.Length; i++)
            {
                var mat = renderersActuales[i].material;
                string prop = null;
                if (mat.HasProperty("_Color")) prop = "_Color";
                else if (mat.HasProperty("_BaseColor")) prop = "_BaseColor";

                propiedadesColorOriginal[i] = prop;

                if (prop != null)
                {
                    try { coloresOriginales[i] = mat.GetColor(prop); }
                    catch { coloresOriginales[i] = Color.white; }

                    try { mat.SetColor(prop, colorSeleccion); } catch { }
                }
                else
                {
                    coloresOriginales[i] = Color.white;
                }
            }
        }

        if (TextDetect != null)
            TextDetect.SetActive(DeberiaMostrarPromptGenerico(col));
    }

    bool DeberiaMostrarPromptGenerico(Collider col)
    {
        if (col == null) return false;

        if (col.GetComponentInParent<DoorInteractable>() != null) return false;
        if (col.GetComponentInParent<DrawerInteractable>() != null) return false;
        if (col.GetComponentInParent<DrawerItemPickup>() != null) return false;
        if (col.GetComponentInParent<RosaFinalDialogue>() != null) return false;
        if (col.GetComponentInParent<ServicioNPCMission>() != null) return false;
        if (col.GetComponentInParent<PhotoVideoInteractable>() != null) return false;
        if (col.GetComponentInParent<PhoneMissionController>() != null) return false;
        if (col.GetComponentInParent<ClosetMissionTrigger>() != null) return false;
        if (col.GetComponentInParent<RadioCoverTrigger>() != null) return false;
        if (col.GetComponentInParent<RadioBatteryInsertTrigger>() != null) return false;
        if (col.GetComponentInParent<BatteryPickup>() != null) return false;
        if (col.GetComponentInParent<RadioKnobInteractable>() != null) return false;
        if (col.GetComponentInParent<RadioAnimacionesSimple>() != null) return false;
        if (col.GetComponentInParent<WalletPickup>() != null) return false;

        return true;
    }

    void Deselect()
    {
        if (renderersActuales != null && coloresOriginales != null)
        {
            for (int i = 0; i < renderersActuales.Length; i++)
            {
                if (renderersActuales[i] == null) continue;
                var mat = renderersActuales[i].material;
                string prop = (propiedadesColorOriginal != null && i < propiedadesColorOriginal.Length) ? propiedadesColorOriginal[i] : null;
                if (!string.IsNullOrEmpty(prop))
                {
                    try { mat.SetColor(prop, coloresOriginales[i]); } catch { }
                }
            }
        }

        renderersActuales = null;
        coloresOriginales = null;
        propiedadesColorOriginal = null;
        ultimoReconocido = null;

        if (TextDetect != null) TextDetect.SetActive(false);
    }

    void LimpiarMiradas()
    {
        LimpiarGenerico();
        ApagarMuchacha();
        ApagarFotoVideo();
        ApagarTapa();
        ApagarPilaInsert();
        ApagarPilasPickup();
        ApagarPerilla();
        ApagarRadio();
        ApagarRosaFinal();
        ApagarTelefono();
        ApagarMissionPrompt();
        ApagarTypewriter();
    }

    void LimpiarGenerico()
    {
        if (ultimoGenericoMirado != null)
        {
            EnviarSalidaGenerica(ultimoGenericoMirado);
            ultimoGenericoMirado = null;
        }
    }

    void ApagarMuchacha()
    {
        if (ultimaMuchachaMirada != null)
        {
            try { ultimaMuchachaMirada.SetLookingAtMe(false); } catch { }
            ultimaMuchachaMirada = null;
        }
    }

    void ApagarFotoVideo()
    {
        if (ultimaFotoVideoMirada != null)
        {
            try { ultimaFotoVideoMirada.DejarMirarFoto(); } catch { }
            ultimaFotoVideoMirada = null;
        }
    }

    void ApagarTapa()
    {
        if (ultimaTapaMirada != null)
        {
            try { ultimaTapaMirada.DejarMirarTapa(); } catch { }
            ultimaTapaMirada = null;
        }
    }

    void ApagarPilaInsert()
    {
        if (ultimaPilaInsertMirada != null)
        {
            try { ultimaPilaInsertMirada.DejarMirarPila(); } catch { }
            ultimaPilaInsertMirada = null;
        }
    }

    void ApagarPilasPickup()
    {
        if (ultimasPilasMiradas != null)
        {
            try { ultimasPilasMiradas.StopLookingAtBatteries(); } catch { }
            ultimasPilasMiradas = null;
        }
    }

    void ApagarPerilla()
    {
        if (ultimaPerillaMirada != null)
        {
            try { ultimaPerillaMirada.DejarMirarPerilla(); } catch { }
            ultimaPerillaMirada = null;
        }
    }

    void ApagarRadio()
    {
        if (ultimaRadioMirada != null)
        {
            try { ultimaRadioMirada.DejarMirarRadio(); } catch { }
            ultimaRadioMirada = null;
        }
    }

    void ApagarRosaFinal()
    {
        if (ultimaRosaFinalMirada != null)
        {
            try { ultimaRosaFinalMirada.SetLookingAtMe(false); } catch { }
            ultimaRosaFinalMirada = null;
        }
    }

    void ApagarTypewriter()
    {
        if (currentTypewriter != null)
        {
            currentTypewriter.HidePrompt();
            currentTypewriter = null;
        }
    }
}
