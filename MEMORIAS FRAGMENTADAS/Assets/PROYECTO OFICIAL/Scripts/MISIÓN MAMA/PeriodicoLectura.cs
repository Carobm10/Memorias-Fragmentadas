using UnityEngine;

/// <summary>
/// Controla la lectura del periódico.
///
/// FUNCIONAMIENTO:
/// 1. El jugador mira el periódico de la mesa.
/// 2. Presiona B.
/// 3. Se activa el periódico de lectura frente a la cámara.
/// 4. Se oculta el periódico visual de la mesa.
/// 5. Mientras lee, B pasa la hoja.
/// 6. X cierra la lectura.
/// 7. Al salir, se oculta el periódico de lectura y vuelve a aparecer el de la mesa.
///
/// IMPORTANTE:
/// - Este script NO mueve el periódico de la mesa.
/// - Este script NO usa mouse.
/// - La hoja que se anima debe ser una hoja del Periodico_Lectura.
/// </summary>
public class PeriodicoLectura : MonoBehaviour
{
    [Header("UI Lectura")]
    [Tooltip("Botón visual de salir con X.")]
    public GameObject botonSalirX;

    [Tooltip("Botón visual de pasar hoja con B.")]
    public GameObject botonPasarHojaB;

    [Header("Periódico de lectura")]
    [Tooltip("Periódico que aparece frente a la cámara. Debe estar apagado al inicio.")]
    public GameObject periodicoLectura;

    [Header("Periódico de mesa")]
    [Tooltip("Objeto visual del periódico que está en la mesa. Se oculta mientras se lee.")]
    public GameObject periodicoMesaVisual;

    [Header("Hoja animada")]
    [Tooltip("Hoja del Periodico_Lectura que debe girar al presionar B.")]
    public Transform hojaAnimada;

    [Header("Animación")]
    [Tooltip("Velocidad con la que gira la hoja.")]
    public float velocidad = 2f;

    [Tooltip("Rotación que hará la hoja al abrirse.")]
    public Vector3 rotacionAbierta = new Vector3(0f, 0f, -180f);

    [Header("Jugador")]
    [Tooltip("Script de movimiento del jugador.")]
    public MovimientoVR2 movimientoJugador;

    [Tooltip("Puntero 3D del jugador.")]
    public GameObject puntero3D;

    [Header("Debug")]
    public bool mostrarDebug = true;

    private Quaternion rotacionInicial;
    private Quaternion rotacionFinal;

    private bool abierto = false;
    private bool enLectura = false;

    private void Start()
    {
        if (hojaAnimada != null)
        {
            rotacionInicial = hojaAnimada.localRotation;
            rotacionFinal = rotacionInicial * Quaternion.Euler(rotacionAbierta);
        }
        else
        {
            Debug.LogWarning("[PeriodicoLectura] No hay hojaAnimada asignada.");
        }

        if (periodicoLectura != null)
            periodicoLectura.SetActive(false);

        if (botonSalirX != null)
            botonSalirX.SetActive(false);

        if (botonPasarHojaB != null)
            botonPasarHojaB.SetActive(false);
    }

    private void Update()
    {
        if (enLectura)
        {
            if (InputManagerCustom.PressB())
                PasarHoja();

            if (Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.JoystickButton3))
                SalirLectura();
        }

        AnimarHoja();
    }

    private void AnimarHoja()
    {
        if (hojaAnimada == null) return;

        Quaternion objetivo = abierto ? rotacionFinal : rotacionInicial;

        hojaAnimada.localRotation = Quaternion.Slerp(
            hojaAnimada.localRotation,
            objetivo,
            velocidad * Time.deltaTime
        );
    }

    public void InteractuarPeriodico()
    {
        if (!enLectura)
            EntrarModoLectura();
    }

    private void EntrarModoLectura()
    {
        enLectura = true;
        abierto = false;

        if (hojaAnimada != null)
            hojaAnimada.localRotation = rotacionInicial;

        if (periodicoLectura != null)
            periodicoLectura.SetActive(true);

        if (periodicoMesaVisual != null)
            periodicoMesaVisual.SetActive(false);

        if (movimientoJugador != null)
            movimientoJugador.puedeMoverse = false;

        if (puntero3D != null)
            puntero3D.SetActive(true);

        if (botonSalirX != null)
            botonSalirX.SetActive(true);

        if (botonPasarHojaB != null)
            botonPasarHojaB.SetActive(true);

        if (mostrarDebug)
            Debug.Log("[PeriodicoLectura] Entró a modo lectura.");
    }

    private void PasarHoja()
    {
        abierto = !abierto;

        if (mostrarDebug)
            Debug.Log("[PeriodicoLectura] Pasar hoja. Abierto: " + abierto);
    }

    public void SalirLectura()
    {
        if (!enLectura) return;

        enLectura = false;
        abierto = false;

        if (hojaAnimada != null)
            hojaAnimada.localRotation = rotacionInicial;

        if (periodicoLectura != null)
            periodicoLectura.SetActive(false);

        if (periodicoMesaVisual != null)
            periodicoMesaVisual.SetActive(true);

        if (movimientoJugador != null)
            movimientoJugador.puedeMoverse = true;

        if (puntero3D != null)
            puntero3D.SetActive(true);

        if (botonSalirX != null)
            botonSalirX.SetActive(false);

        if (botonPasarHojaB != null)
            botonPasarHojaB.SetActive(false);

        if (mostrarDebug)
            Debug.Log("[PeriodicoLectura] Salió de modo lectura.");
    }

    public bool EstaEnLectura()
    {
        return enLectura;
    }
}