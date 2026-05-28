using UnityEngine;

/// <summary>
/// Sistema de lectura del periódico:
/// 
/// 1. El jugador mira el periódico de la mesa.
/// 2. Presiona B.
/// 3. Aparece un periódico especial frente a cámara.
/// 4. B pasa hoja.
/// 5. X cierra lectura.
/// 
/// Pensado para VR/Cardboard.
/// </summary>
public class AbrirPeriodico : MonoBehaviour
{
    [Header("Periódico de lectura")]
    [Tooltip("Periódico que aparece frente a la cámara.")]
    public GameObject periodicoLectura;

    [Header("Hoja animada")]
    [Tooltip("Hoja que rota al pasar página.")]
    public Transform hojaAnimada;

    [Header("Animación")]
    public float velocidad = 2f;

    [Tooltip("Rotación de la hoja al abrir.")]
    public Vector3 rotacionAbierta = new Vector3(0f, 0f, -180f);

    [Header("Jugador")]
    public MovimientoVR2 movimientoJugador;
    public GameObject puntero3D;

    [Header("Debug")]
    public bool mostrarDebug = true;

    private Quaternion rotacionInicial;
    private Quaternion rotacionFinal;

    private bool abierto = false;
    private bool enLectura = false;

    private void Start()
    {
        // Guardar rotación inicial de la hoja
        if (hojaAnimada != null)
        {
            rotacionInicial = hojaAnimada.localRotation;
            rotacionFinal = rotacionInicial * Quaternion.Euler(rotacionAbierta);
        }

        // El periódico de lectura empieza apagado
        if (periodicoLectura != null)
            periodicoLectura.SetActive(false);
    }

    private void Update()
    {
        // Animación suave de hoja
        if (hojaAnimada != null)
        {
            Quaternion objetivo = abierto ? rotacionFinal : rotacionInicial;

            hojaAnimada.localRotation = Quaternion.Slerp(
                hojaAnimada.localRotation,
                objetivo,
                velocidad * Time.deltaTime
            );
        }
    }

    // ======================================================
    // INTERACCIÓN PRINCIPAL
    // ======================================================
    public void InteractuarPeriodico()
    {
        // Primera interacción
        if (!enLectura)
        {
            EntrarModoLectura();
            return;
        }

        // Ya leyendo → pasar hoja
        PasarHoja();
    }

    // ======================================================
    // ENTRAR A LECTURA
    // ======================================================
    private void EntrarModoLectura()
    {
        enLectura = true;

        if (periodicoLectura != null)
            periodicoLectura.SetActive(true);

        if (movimientoJugador != null)
            movimientoJugador.puedeMoverse = false;

        if (puntero3D != null)
            puntero3D.SetActive(false);

        if (mostrarDebug)
            Debug.Log("[AbrirPeriodico] Entró a modo lectura.");
    }

    // ======================================================
    // PASAR HOJA
    // ======================================================
    private void PasarHoja()
    {
        abierto = !abierto;

        if (mostrarDebug)
            Debug.Log("[AbrirPeriodico] Pasar hoja. Estado abierto: " + abierto);
    }

    // ======================================================
    // SALIR DE LECTURA
    // ======================================================
    public void SalirLectura()
    {
        if (!enLectura)
            return;

        enLectura = false;
        abierto = false;

        // Reiniciar hoja
        if (hojaAnimada != null)
            hojaAnimada.localRotation = rotacionInicial;

        // Ocultar periódico lectura
        if (periodicoLectura != null)
            periodicoLectura.SetActive(false);

        // Devolver control
        if (movimientoJugador != null)
            movimientoJugador.puedeMoverse = true;

        if (puntero3D != null)
            puntero3D.SetActive(true);

        if (mostrarDebug)
            Debug.Log("[AbrirPeriodico] Salió de modo lectura.");
    }

    // ======================================================
    // ESTADO ACTUAL
    // ======================================================
    public bool EstaEnLectura()
    {
        return enLectura;
    }
}