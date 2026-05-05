using UnityEngine;

/// <summary>
/// DrawerInteractable controla la apertura y cierre de cajones.
/// 
/// Este script NO detecta botones directamente.
/// La interacción se hace desde Selected.cs:
/// - El jugador mira el cajón.
/// - Presiona B.
/// - Selected.cs llama ToggleDrawer().
/// 
/// Configuración recomendada:
/// - openDistance: entre 0.05 y 0.15 (depende del modelo).
/// - localOpenDirection: define hacia dónde se abre el cajón.
/// 
/// IMPORTANTE:
/// - No usar Rigidbody.
/// - Debe tener Collider.
/// - Debe estar en layer "RayCast Detect".
/// </summary>
public class DrawerInteractable : MonoBehaviour
{
    private const string OpenDrawerClipPath = "Assets/PROYECTO OFICIAL/Scripts/Audio/CajonAbriendose.mp3";
    private const string CloseDrawerClipPath = "Assets/PROYECTO OFICIAL/Scripts/Audio/CajonCerrandose.mp3";

    [Header("Configuración de apertura")]
    public bool startsOpen = false;
    public float openDistance = 0.1f;
    public float speed = 3f;

    [Header("Dirección local de apertura")]
    public Vector3 localOpenDirection = new Vector3(0f, 0f, -1f);

    [Header("Estado del cajón")]
    public bool isOpen = false;
    public bool isLocked = false;

    [Header("Sonidos")]
    [SerializeField]
    private AudioClip openDrawerClip;

    [SerializeField]
    private AudioClip closeDrawerClip;

    [SerializeField]
    [Range(0f, 1f)]
    private float soundVolume = 1f;

    [SerializeField]
    [Range(0.5f, 2f)]
    private float soundPitch = 1f;

    private Vector3 closedLocalPosition;
    private Vector3 openLocalPosition;
    private AudioSource audioSource;

    private void Reset()
    {
        CargarSonidosPredeterminados();
    }

    private void OnValidate()
    {
        CargarSonidosPredeterminados();
    }

    private void Awake()
    {
        PrepararAudio();
    }

    void Start()
    {
        PrepararAudio();
        closedLocalPosition = transform.localPosition;
        openLocalPosition = closedLocalPosition + localOpenDirection.normalized * openDistance;

        isOpen = startsOpen;
        transform.localPosition = isOpen ? openLocalPosition : closedLocalPosition;
    }

    void Update()
    {
        Vector3 targetPosition = isOpen ? openLocalPosition : closedLocalPosition;

        transform.localPosition = Vector3.Lerp(
            transform.localPosition,
            targetPosition,
            speed * Time.deltaTime
        );
    }

    /// <summary>
    /// Alterna el estado del cajón (abrir/cerrar).
    /// </summary>
    public void ToggleDrawer()
    {
        SetDrawerState(!isOpen);
    }

    /// <summary>
    /// Abre el cajón directamente.
    /// </summary>
    public void OpenDrawer()
    {
        SetDrawerState(true);
    }

    /// <summary>
    /// Cierra el cajón.
    /// </summary>
    public void CloseDrawer()
    {
        SetDrawerState(false);
    }

    /// <summary>
    /// Bloquea el cajón.
    /// </summary>
    public void LockDrawer()
    {
        isLocked = true;
    }

    /// <summary>
    /// Desbloquea el cajón.
    /// </summary>
    public void UnlockDrawer()
    {
        isLocked = false;
    }

    private void SetDrawerState(bool open)
    {
        if (isLocked)
        {
            Debug.Log("El cajón está bloqueado: " + gameObject.name);
            return;
        }

        if (isOpen == open)
        {
            return;
        }

        isOpen = open;
        ReproducirSonidoDeEstado();
    }

    private void ReproducirSonidoDeEstado()
    {
        if (audioSource == null)
        {
            return;
        }

        AudioClip clip = isOpen ? openDrawerClip : closeDrawerClip;
        if (clip == null)
        {
            return;
        }

        audioSource.pitch = soundPitch;
        audioSource.PlayOneShot(clip, soundVolume);
    }

    private void PrepararAudio()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        if (audioSource == null && Application.isPlaying)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        if (audioSource == null)
        {
            return;
        }

        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1f;
        audioSource.dopplerLevel = 0f;
    }

    private void CargarSonidosPredeterminados()
    {
        if (openDrawerClip == null)
        {
            openDrawerClip = CargarClip(OpenDrawerClipPath);
        }

        if (closeDrawerClip == null)
        {
            closeDrawerClip = CargarClip(CloseDrawerClipPath);
        }
    }

#if UNITY_EDITOR
    private static AudioClip CargarClip(string assetPath)
    {
        return UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>(assetPath);
    }
#else
    private static AudioClip CargarClip(string assetPath)
    {
        return null;
    }
#endif
}