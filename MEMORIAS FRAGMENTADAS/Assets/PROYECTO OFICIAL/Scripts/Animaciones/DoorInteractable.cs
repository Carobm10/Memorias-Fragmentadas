using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// DoorInteractable controla la apertura y cierre de una puerta.
/// 
/// Este script NO detecta botones directamente.
/// La interacción se hace desde Selected.cs:
/// - El jugador mira la puerta.
/// - Selected.cs detecta DoorInteractable.
/// - Si presiona B, llama ToggleDoor().
/// 
/// Configuración recomendada:
/// - Puertas grandes: Open Angle 90 o 95.
/// - Puertas pequeñas: Open Angle 45.
/// - Si abre al revés: usa ángulo negativo.
/// - Rotation Axis normalmente es Y = 1.
/// 
/// IMPORTANTE:
/// No usar Rigidbody en puertas.
/// La puerta debe tener Collider.
/// El objeto debe estar en la layer RayCast Detect.
/// </summary>
public class DoorInteractable : MonoBehaviour
{
    private const string OpenDoorClipPath = "Assets/PROYECTO OFICIAL/Scripts/Audio/PuertaCerrandose.mp3";
    private const string CloseDoorClipPath = "Assets/PROYECTO OFICIAL/Scripts/Audio/PuertaAbriendose.mp3";

    [Header("Configuración de apertura")]
    public bool startsOpen = false;
    public float openAngle = 95f;
    public float speed = 3f;
    public Vector3 rotationAxis = Vector3.up;

    [Header("Sonidos")]
    [SerializeField]
    private AudioClip openDoorClip;

    [SerializeField]
    private AudioClip closeDoorClip;

    [SerializeField]
    [Range(0f, 1f)]
    private float soundVolume = 1f;

    [SerializeField]
    [Range(0.5f, 2f)]
    private float soundPitch = 1f;

    [Header("Estado de la puerta")]
    public bool isOpen = false;
    public bool isLocked = false;

    private Quaternion closedRotation;
    private Quaternion openRotation;
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
        closedRotation = transform.localRotation;
        openRotation = closedRotation * Quaternion.AngleAxis(openAngle, rotationAxis);

        isOpen = startsOpen;
        transform.localRotation = isOpen ? openRotation : closedRotation;
    }

    void Update()
    {
        Quaternion targetRotation = isOpen ? openRotation : closedRotation;

        transform.localRotation = Quaternion.Slerp(
            transform.localRotation,
            targetRotation,
            speed * Time.deltaTime
        );
    }

    /// <summary>
    /// Cambia el estado de la puerta:
    /// si está cerrada, la abre;
    /// si está abierta, la cierra.
    /// </summary>
    public void ToggleDoor()
    {
        SetDoorState(!isOpen);
    }

    /// <summary>
    /// Abre la puerta directamente.
    /// Se usa, por ejemplo, en la misión del clóset.
    /// </summary>
    public void OpenDoor()
    {
        SetDoorState(true);
    }

    /// <summary>
    /// Cierra la puerta directamente.
    /// </summary>
    public void CloseDoor()
    {
        SetDoorState(false);
    }

    /// <summary>
    /// Bloquea la puerta para que no pueda abrirse/cerrarse.
    /// </summary>
    public void LockDoor()
    {
        isLocked = true;
    }

    /// <summary>
    /// Desbloquea la puerta.
    /// </summary>
    public void UnlockDoor()
    {
        isLocked = false;
    }

    private void SetDoorState(bool open)
    {
        if (isLocked)
        {
            Debug.Log("La puerta está bloqueada: " + gameObject.name);
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

        AudioClip clip = isOpen ? openDoorClip : closeDoorClip;
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
        if (openDoorClip == null)
        {
            openDoorClip = CargarClip(OpenDoorClipPath);
        }

        if (closeDoorClip == null)
        {
            closeDoorClip = CargarClip(CloseDoorClipPath);
        }
    }

    [ContextMenu("Intercambiar audios de puerta")]
    private void IntercambiarAudiosPuerta()
    {
        AudioClip clipTemporal = openDoorClip;
        openDoorClip = closeDoorClip;
        closeDoorClip = clipTemporal;
    }

#if UNITY_EDITOR
    private static AudioClip CargarClip(string assetPath)
    {
        return AssetDatabase.LoadAssetAtPath<AudioClip>(assetPath);
    }
#else
    private static AudioClip CargarClip(string assetPath)
    {
        return null;
    }
#endif
}