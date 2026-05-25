using System.Collections;
using UnityEngine;

public class WalletSequenceController : MonoBehaviour
{
    public static WalletSequenceController Instance;

    [Header("Objetos de animación del monedero")]
    public GameObject animAbrirMonedero;
    public GameObject animSacar1Moneda;
    public GameObject animSacar2Monedas;
    public GameObject animSacar3Monedas;
    public GameObject animSacar4Monedas;
    public GameObject animCerrarMonedero;

    [Header("Jugador")]
    public MovimientoVR2 movimientoJugador;
    public GameObject puntero3D;

    [Header("Tiempos por animación")]
    public float tiempoAbrir = 1.5f;
    public float tiempoSacarMonedas = 1.5f;
    public float tiempoCerrar = 1.5f;

    [Header("Configuración")]
    public bool elegirCantidadAleatoria = true;
    public int cantidadMonedasManual = 3;

    [Header("Debug")]
    public bool mostrarDebug = true;

    private bool secuenciaActiva = false;

    private void Awake()
    {
        Instance = this;
        ApagarTodasLasAnimaciones();
    }

    public void IniciarSecuencia(System.Action alTerminar)
    {
        if (secuenciaActiva)
            return;

        StartCoroutine(SecuenciaMonedero(alTerminar));
    }

    private IEnumerator SecuenciaMonedero(System.Action alTerminar)
    {
        secuenciaActiva = true;

        if (mostrarDebug)
            Debug.Log("[WalletSequence] Iniciando secuencia del monedero.");

        BloquearJugador();

        ApagarTodasLasAnimaciones();

        yield return ReproducirObjetoAnimado(animAbrirMonedero, tiempoAbrir, "Abrir monedero");

        yield return ReproducirObjetoAnimado(animSacar1Moneda, tiempoSacarMonedas, "Sacar moneda 1");

        yield return ReproducirObjetoAnimado(animSacar2Monedas, tiempoSacarMonedas, "Sacar moneda 2");

        yield return ReproducirObjetoAnimado(animSacar3Monedas, tiempoSacarMonedas, "Sacar moneda 3");
        yield return ReproducirObjetoAnimado(animSacar4Monedas, tiempoSacarMonedas, "Sacar moneda 4");

        yield return ReproducirObjetoAnimado(animCerrarMonedero, tiempoCerrar, "Cerrar monedero");

        ApagarTodasLasAnimaciones();

        DesbloquearJugador();

        if (mostrarDebug)
            Debug.Log("[WalletSequence] Secuencia terminada.");

        alTerminar?.Invoke();

        secuenciaActiva = false;
    }

    private IEnumerator ReproducirObjetoAnimado(GameObject objetoAnimado, float tiempo, string nombreDebug)
    {
        if (objetoAnimado == null)
        {
            Debug.LogWarning("[WalletSequence] Falta asignar animación: " + nombreDebug);
            yield return new WaitForSeconds(0.5f);
            yield break;
        }

        ApagarTodasLasAnimaciones();

        objetoAnimado.SetActive(true);

        Animator anim = objetoAnimado.GetComponent<Animator>();

        if (anim != null)
        {
            anim.Play(0, 0, 0f);
        }

        if (mostrarDebug)
            Debug.Log("[WalletSequence] Reproduciendo: " + nombreDebug);

        yield return new WaitForSeconds(tiempo);

        objetoAnimado.SetActive(false);
    }

    private GameObject ObtenerAnimacionSacarMonedas(int cantidad)
    {
        if (cantidad == 1) return animSacar1Moneda;
        if (cantidad == 2) return animSacar2Monedas;
        if (cantidad == 3) return animSacar3Monedas;
        if (cantidad == 4) return animSacar4Monedas;

        return animSacar3Monedas;
    }

    private void ApagarTodasLasAnimaciones()
    {
        if (animAbrirMonedero != null) animAbrirMonedero.SetActive(false);
        if (animSacar1Moneda != null) animSacar1Moneda.SetActive(false);
        if (animSacar2Monedas != null) animSacar2Monedas.SetActive(false);
        if (animSacar3Monedas != null) animSacar3Monedas.SetActive(false);
        if (animSacar4Monedas != null) animSacar4Monedas.SetActive(false);
        if (animCerrarMonedero != null) animCerrarMonedero.SetActive(false);
    }

    private void BloquearJugador()
    {
        if (movimientoJugador != null)
        {
            movimientoJugador.puedeMoverse = false;
            movimientoJugador.activarHeadBob = false;
        }

        if (puntero3D != null)
            puntero3D.SetActive(false);
    }

    private void DesbloquearJugador()
    {
        if (movimientoJugador != null)
        {
            movimientoJugador.puedeMoverse = true;
            movimientoJugador.activarHeadBob = true;
        }

        if (puntero3D != null)
            puntero3D.SetActive(true);
    }
}