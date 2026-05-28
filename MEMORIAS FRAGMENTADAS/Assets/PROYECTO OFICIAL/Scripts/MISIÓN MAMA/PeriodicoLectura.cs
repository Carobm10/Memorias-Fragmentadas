using UnityEngine;

public class PeriodicoLectura : MonoBehaviour
{
    [Header("UI Lectura")]
    public GameObject botonSalirX;
    public GameObject botonPasarHojaB;
    [Header("Periódico de lectura")]
    public GameObject periodicoLectura;

    [Header("Hoja animada")]
    public Transform hojaAnimada;

    [Header("Animación")]
    public float velocidad = 2f;
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
        if (hojaAnimada != null)
        {
            rotacionInicial = hojaAnimada.localRotation;
            rotacionFinal = rotacionInicial * Quaternion.Euler(rotacionAbierta);
        }

        if (periodicoLectura != null)
            periodicoLectura.SetActive(false);
    }

    private void Update()
    {
        if (enLectura)
        {
            if (InputManagerCustom.PressB())
            {
                PasarHoja();
            }

            if (Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.JoystickButton3))
            {
                SalirLectura();
            }
        }

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
        {
            EntrarModoLectura();
            return;
        }

        PasarHoja();
    }

    private void EntrarModoLectura()
    {
        enLectura = true;

        if (periodicoLectura != null)
            periodicoLectura.SetActive(true);

        if (movimientoJugador != null)
            movimientoJugador.puedeMoverse = false;

        if (puntero3D != null)
            puntero3D.SetActive(true);

        // Mostrar botones UI de lectura
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

        if (movimientoJugador != null)
            movimientoJugador.puedeMoverse = true;

        if (puntero3D != null)
            puntero3D.SetActive(true);
        
        // Ocultar botones UI de lectura
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