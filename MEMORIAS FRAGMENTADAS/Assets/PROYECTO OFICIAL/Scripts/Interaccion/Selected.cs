using UnityEngine;

/// <summary>
/// SELECTED.CS LIMPIO
/// Va en la Main Camera.
/// 
/// Este script lanza un raycast desde la cámara y detecta objetos en:
/// - Layer "Raycast Detect"
/// - Layer "PickupItem"
///
/// Mantiene:
/// - Muchacha/Rosa
/// - Radio actual de la misión
/// - Tapa de radio
/// - Pilas del cajón
/// - Pilas de inserción
/// - Fotos/video
/// - Teléfono
/// - Puertas/cajones/objetos mediante SendMessage seguro
///
/// Elimina sistemas viejos de radio:
/// - RadioBackCover
/// - RadioBatteryInstaller
/// - RadioBatteryTrigger
/// - RadioMissionInteractable
/// - RadioFinalUse
/// </summary>
public class Selected : MonoBehaviour
{
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

    [Header("Manager de UI armario")]
    public ClosetCanvasManager closetCanvasManager;

    [Header("Color selección")]
    public Color colorSeleccion = new Color(0.1f, 1f, 0.25f, 1f);

    private GameObject ultimoReconocido;
    private Renderer[] renderersActuales;
    private Color[] coloresOriginales;

    // Últimos objetos mirados
    private ServicioNPCMission ultimaMuchachaMirada;
    private RadioKnobInteractable ultimaPerillaMirada;
    private PhotoVideoInteractable ultimaFotoVideoMirada;
    private RadioAnimacionesSimple ultimaRadioMirada;
    private RadioCoverTrigger ultimaTapaMirada;
    private BatteryPickup ultimasPilasMiradas;
    private RadioBatteryInsertTrigger ultimaPilaInsertMirada;
    private GameObject ultimoObjetoConSendMessage;

    void Awake()
    {
        ApagarTodosLosPrompts();
    }

    void Start()
    {
        mask = LayerMask.GetMask("Raycast Detect", "PickupItem");
        ApagarTodosLosPrompts();
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

            if (ultimoReconocido != objetoDetectado)
            {
                Deselect();
                SelectedObject(hit.collider);
            }

            // 1. Muchacha / Rosa
            ServicioNPCMission muchacha = hit.collider.GetComponentInParent<ServicioNPCMission>();
            if (muchacha != null)
            {
                LimpiarTodoMenos(muchacha.gameObject);

                if (ultimaMuchachaMirada != null && ultimaMuchachaMirada != muchacha)
                    ultimaMuchachaMirada.SetLookingAtMe(false);

                ultimaMuchachaMirada = muchacha;
                muchacha.SetLookingAtMe(true);
                return;
            }

            ApagarMuchacha();

            // 2. Foto / video
            PhotoVideoInteractable fotoVideo = hit.collider.GetComponentInParent<PhotoVideoInteractable>();
            if (fotoVideo != null)
            {
                LimpiarTodoMenos(fotoVideo.gameObject);

                if (ultimaFotoVideoMirada != null && ultimaFotoVideoMirada != fotoVideo)
                    ultimaFotoVideoMirada.DejarMirarFoto();

                ultimaFotoVideoMirada = fotoVideo;
                fotoVideo.MirarFoto();
                return;
            }

            ApagarFotoVideo();

            // 3. Tapa de radio actual
            RadioCoverTrigger tapa = hit.collider.GetComponentInParent<RadioCoverTrigger>();
            if (tapa != null)
            {
                LimpiarTodoMenos(tapa.gameObject);

                if (ultimaTapaMirada != null && ultimaTapaMirada != tapa)
                    ultimaTapaMirada.DejarMirarTapa();

                ultimaTapaMirada = tapa;
                tapa.MirarTapa();
                return;
            }

            ApagarTapa();

            // 4. Pilas para insertar
            RadioBatteryInsertTrigger pilaInsert = hit.collider.GetComponentInParent<RadioBatteryInsertTrigger>();
            if (pilaInsert != null)
            {
                LimpiarTodoMenos(pilaInsert.gameObject);

                if (ultimaPilaInsertMirada != null && ultimaPilaInsertMirada != pilaInsert)
                    ultimaPilaInsertMirada.DejarMirarPila();

                ultimaPilaInsertMirada = pilaInsert;
                pilaInsert.MirarPila();
                return;
            }

            ApagarPilaInsert();

            // 5. Pilas del cajón
            BatteryPickup pilas = hit.collider.GetComponentInParent<BatteryPickup>();
            if (pilas != null)
            {
                LimpiarTodoMenos(pilas.gameObject);

                if (ultimasPilasMiradas != null && ultimasPilasMiradas != pilas)
                    ultimasPilasMiradas.StopLookingAtBatteries();

                ultimasPilasMiradas = pilas;
                pilas.LookAtBatteries();
                return;
            }

            ApagarPilasPickup();

            // 6. Radio principal de la misión
            RadioAnimacionesSimple radio = hit.collider.GetComponentInParent<RadioAnimacionesSimple>();
            if (radio != null)
            {
                LimpiarTodoMenos(radio.gameObject);

                if (ultimaRadioMirada != null && ultimaRadioMirada != radio)
                    ultimaRadioMirada.DejarMirarRadio();

                ultimaRadioMirada = radio;
                radio.MirarRadio();
                return;
            }

            ApagarRadio();

            // 7. Perillas de radio
            RadioKnobInteractable perilla = hit.collider.GetComponentInParent<RadioKnobInteractable>();

            if (perilla != null)
            {
                LimpiarTodoMenos(perilla.gameObject);

                if (ultimaPerillaMirada != null && ultimaPerillaMirada != perilla)
                    ultimaPerillaMirada.DejarMirarPerilla();

                ultimaPerillaMirada = perilla;
                perilla.MirarPerilla();
                return;
            }

            ApagarPerilla();

            // 7. Interacciones generales conservadas por SendMessage seguro
            // Esto permite que puertas, cajones, objetos 360, ropa, teléfono, etc.
            // sigan funcionando si tienen sus métodos propios.
            GameObject root = hit.collider.transform.root.gameObject;
            GameObject parent = hit.collider.GetComponentInParent<Transform>().gameObject;

            if (ultimoObjetoConSendMessage != null && ultimoObjetoConSendMessage != hit.collider.gameObject)
            {
                EnviarSalidaSegura(ultimoObjetoConSendMessage);
            }

            ultimoObjetoConSendMessage = hit.collider.gameObject;

            hit.collider.SendMessageUpwards("MirarPuerta", SendMessageOptions.DontRequireReceiver);
            hit.collider.SendMessageUpwards("LookAtDoor", SendMessageOptions.DontRequireReceiver);

            hit.collider.SendMessageUpwards("MirarCajon", SendMessageOptions.DontRequireReceiver);
            hit.collider.SendMessageUpwards("LookAtDrawer", SendMessageOptions.DontRequireReceiver);

            hit.collider.SendMessageUpwards("MirarObjeto", SendMessageOptions.DontRequireReceiver);
            hit.collider.SendMessageUpwards("LookAtObject", SendMessageOptions.DontRequireReceiver);

            hit.collider.SendMessageUpwards("MirarTelefono", SendMessageOptions.DontRequireReceiver);
            hit.collider.SendMessageUpwards("LookAtPhone", SendMessageOptions.DontRequireReceiver);

            hit.collider.SendMessageUpwards("MirarPrenda", SendMessageOptions.DontRequireReceiver);
            hit.collider.SendMessageUpwards("LookAtClothing", SendMessageOptions.DontRequireReceiver);
        }
        else
        {
            LimpiarMiradas();
            Deselect();

            if (pointer3D != null)
                pointer3D.SetDetected(false);
        }
    }

    void SelectedObject(Collider collider)
    {
        ultimoReconocido = collider.gameObject;

        renderersActuales = collider.GetComponentsInChildren<Renderer>();

        if (renderersActuales == null || renderersActuales.Length == 0)
            return;

        coloresOriginales = new Color[renderersActuales.Length];

        for (int i = 0; i < renderersActuales.Length; i++)
        {
            if (renderersActuales[i] == null) continue;

            coloresOriginales[i] = renderersActuales[i].material.color;
            renderersActuales[i].material.color = colorSeleccion;
        }
    }

    void Deselect()
    {
        if (renderersActuales != null && coloresOriginales != null)
        {
            for (int i = 0; i < renderersActuales.Length; i++)
            {
                if (renderersActuales[i] == null) continue;
                if (i >= coloresOriginales.Length) continue;

                renderersActuales[i].material.color = coloresOriginales[i];
            }
        }

        renderersActuales = null;
        coloresOriginales = null;
        ultimoReconocido = null;
    }

    void LimpiarMiradas()
    {
        ApagarMuchacha();
        ApagarFotoVideo();
        ApagarTapa();
        ApagarPilaInsert();
        ApagarPilasPickup();
        ApagarRadio();
        ApagarPerilla();

        if (ultimoObjetoConSendMessage != null)
        {
            EnviarSalidaSegura(ultimoObjetoConSendMessage);
            ultimoObjetoConSendMessage = null;
        }

        ApagarTodosLosPrompts();
    }

    void LimpiarTodoMenos(GameObject objetoActivo)
    {
        if (ultimaMuchachaMirada != null && ultimaMuchachaMirada.gameObject != objetoActivo)
            ApagarMuchacha();

        if (ultimaFotoVideoMirada != null && ultimaFotoVideoMirada.gameObject != objetoActivo)
            ApagarFotoVideo();

        if (ultimaTapaMirada != null && ultimaTapaMirada.gameObject != objetoActivo)
            ApagarTapa();

        if (ultimaPilaInsertMirada != null && ultimaPilaInsertMirada.gameObject != objetoActivo)
            ApagarPilaInsert();

        if (ultimasPilasMiradas != null && ultimasPilasMiradas.gameObject != objetoActivo)
            ApagarPilasPickup();

        if (ultimaRadioMirada != null && ultimaRadioMirada.gameObject != objetoActivo)
            ApagarRadio();
        
        if (ultimaPerillaMirada != null && ultimaPerillaMirada.gameObject != objetoActivo)
            ApagarPerilla();
    }

    void ApagarMuchacha()
    {
        if (ultimaMuchachaMirada != null)
        {
            ultimaMuchachaMirada.SetLookingAtMe(false);
            ultimaMuchachaMirada = null;
        }
    }

    void ApagarFotoVideo()
    {
        if (ultimaFotoVideoMirada != null)
        {
            ultimaFotoVideoMirada.DejarMirarFoto();
            ultimaFotoVideoMirada = null;
        }
    }

    void ApagarTapa()
    {
        if (ultimaTapaMirada != null)
        {
            ultimaTapaMirada.DejarMirarTapa();
            ultimaTapaMirada = null;
        }
    }

    void ApagarPilaInsert()
    {
        if (ultimaPilaInsertMirada != null)
        {
            ultimaPilaInsertMirada.DejarMirarPila();
            ultimaPilaInsertMirada = null;
        }
    }

    void ApagarPilasPickup()
    {
        if (ultimasPilasMiradas != null)
        {
            ultimasPilasMiradas.StopLookingAtBatteries();
            ultimasPilasMiradas = null;
        }
    }

    void ApagarRadio()
    {
        if (ultimaRadioMirada != null)
        {
            ultimaRadioMirada.DejarMirarRadio();
            ultimaRadioMirada = null;
        }
    }
    void ApagarPerilla()
    {
        if (ultimaPerillaMirada != null)
        {
            ultimaPerillaMirada.DejarMirarPerilla();
            ultimaPerillaMirada = null;
        }
    }

    void EnviarSalidaSegura(GameObject obj)
    {
        if (obj == null) return;

        obj.SendMessageUpwards("DejarMirarPuerta", SendMessageOptions.DontRequireReceiver);
        obj.SendMessageUpwards("StopLookingAtDoor", SendMessageOptions.DontRequireReceiver);

        obj.SendMessageUpwards("DejarMirarCajon", SendMessageOptions.DontRequireReceiver);
        obj.SendMessageUpwards("StopLookingAtDrawer", SendMessageOptions.DontRequireReceiver);

        obj.SendMessageUpwards("DejarMirarObjeto", SendMessageOptions.DontRequireReceiver);
        obj.SendMessageUpwards("StopLookingAtObject", SendMessageOptions.DontRequireReceiver);

        obj.SendMessageUpwards("DejarMirarTelefono", SendMessageOptions.DontRequireReceiver);
        obj.SendMessageUpwards("StopLookingAtPhone", SendMessageOptions.DontRequireReceiver);

        obj.SendMessageUpwards("DejarMirarPrenda", SendMessageOptions.DontRequireReceiver);
        obj.SendMessageUpwards("StopLookingAtClothing", SendMessageOptions.DontRequireReceiver);
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
}