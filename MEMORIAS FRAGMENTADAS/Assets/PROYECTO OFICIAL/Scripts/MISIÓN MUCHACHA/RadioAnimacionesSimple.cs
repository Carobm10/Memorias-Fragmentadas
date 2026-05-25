using UnityEngine;
using TMPro;
using System.Collections;

/// <summary>
/// Controla toda la misión de la radio:
/// 1. La radio se desbloquea después de hablar con Rosa.
/// 2. El jugador revisa la radio.
/// 3. Se abre la tapa.
/// 4. Rosa dice que faltan pilas.
/// 5. El jugador recoge pilas.
/// 6. Se insertan pila 1, pila 2 y pila 3.
/// 7. Se cierra la tapa.
/// 8. Se activa la radio final para música.
/// </summary>
public class RadioAnimacionesSimple : MonoBehaviour
{
    [Header("Tiempos")]
    public float tiempoAnimacionPila = 2.5f;
    public float velocidadAnimacionPilas = 1f;
    public float velocidadAnimacion = 0.45f;
    public float tiempoDialogoRosa = 4f;

    [Header("Control de misión")]
    public bool radioDesbloqueada = false;
    public Selected selectedRaycast;

    [Header("Visuales principales")]
    public GameObject radioNormalVisual;
    public GameObject radioVolteadoVisual;
    public GameObject abrirTapa;
    public GameObject radioSinPilasVisual;
    public GameObject radioAbiertoSinPilas;

    [Header("Animaciones de pilas")]
    public GameObject primeraPila;
    public GameObject segundaPila;
    public GameObject terceraPila;
    public GameObject cerrarTapa;

    [Header("Animators pilas")]
    public Animator animatorPrimeraPila;
    public Animator animatorSegundaPila;
    public Animator animatorTerceraPila;

    [Header("Radio final para música")]
    public GameObject radioParaMusica;

    [Header("Tapita / highlight")]
    public Renderer tapaRenderer;
    public Color colorTapaSeleccionada = new Color(0.1f, 1f, 0.25f, 1f);
    private Color colorOriginalTapa;

    [Header("Prompt")]
    public GameObject promptPanel;
    public TMP_Text promptText;

    [Header("Diálogo Rosa")]
    public GameObject dialogoPanel;
    public TMP_Text nombreDialogoText;
    public TMP_Text dialogoText;
    public GameObject botonSalirX;
    public string nombreNPC = "Rosa";

    [Header("Nombres de estados Animator")]
    public string nombreAnimacionAbrir = "AbrirTapa";
    public string nombreAnimacionPrimeraPila = "PrimeraPila";
    public string nombreAnimacionSegundaPila = "SegundaPila";
    public string nombreAnimacionTerceraPila = "TerceraPila";
    public string nombreAnimacionCerrarTapa = "CerrarTapa";

    private bool mirandoRadio = false;
    private bool secuenciaIniciada = false;
    private bool esperandoSalir = false;
    private bool modoInsertarPilas = false;
    private bool animandoPila = false;

    private bool pila1Puesta = false;
    private bool pila2Puesta = false;
    private bool pila3Puesta = false;

    private bool puedeCerrarTapa = false;
    private bool tapaCerrada = false;

    void Start()
    {
        OcultarPrompt();
        OcultarDialogo();

        if (tapaRenderer != null)
            colorOriginalTapa = tapaRenderer.material.color;

        ActivarSolo(radioNormalVisual);

        if (radioParaMusica != null)
            radioParaMusica.SetActive(false);

        if (primeraPila != null) primeraPila.SetActive(false);
        if (segundaPila != null) segundaPila.SetActive(false);
        if (terceraPila != null) terceraPila.SetActive(false);
        if (cerrarTapa != null) cerrarTapa.SetActive(false);
    }

    void Update()
    {
        if (esperandoSalir)
        {
            if (InputManagerCustom.PressX())
                SalirABuscarPilas();

            return;
        }

        if (!mirandoRadio) return;
        if (!radioDesbloqueada) return;

        if (!modoInsertarPilas && !secuenciaIniciada)
        {
            MostrarPrompt("Presiona B para revisar el radio");

            if (InputManagerCustom.PressB())
                StartCoroutine(SecuenciaRadio());

            return;
        }

        if (PuedeCerrarTapa())
        {
            MostrarPrompt("Presiona B para cerrar la tapa");

            if (tapaRenderer != null)
                tapaRenderer.material.color = colorTapaSeleccionada;

            if (InputManagerCustom.PressB())
                CerrarTapaDesdeTrigger();

            return;
        }
    }

    IEnumerator SecuenciaRadio()
    {
        secuenciaIniciada = true;

        if (selectedRaycast != null)
            selectedRaycast.enabled = false;

        if (radioNormalVisual != null)
            radioNormalVisual.SetActive(false);

        if (radioVolteadoVisual != null)
            radioVolteadoVisual.SetActive(true);

        if (tapaRenderer != null)
            tapaRenderer.material.color = colorTapaSeleccionada;

        float contador = 0f;

        while (contador < 0.5f)
        {
            contador += Time.deltaTime;
            MostrarPrompt("Presiona B para abrir la tapita");
            yield return null;
        }

        while (!InputManagerCustom.PressB())
        {
            MostrarPrompt("Presiona B para abrir la tapita");
            yield return null;
        }

        MostrarPrompt("Abriendo tapa...");

        if (tapaRenderer != null)
            tapaRenderer.material.color = colorOriginalTapa;

        if (radioVolteadoVisual != null)
            radioVolteadoVisual.SetActive(false);

        ReproducirAbrirTapa();

        yield return new WaitForSeconds(3f);

        if (abrirTapa != null)
            abrirTapa.SetActive(false);

        if (radioSinPilasVisual != null)
            radioSinPilasVisual.SetActive(true);

        OcultarPrompt();

        MostrarDialogo(
            "Ay, verdad que no tiene pilas.\n" +
            "Las pilas creo que están en alguno de esos cajones de ahí de abajo del radio."
        );

        yield return new WaitForSeconds(tiempoDialogoRosa);

        MostrarDialogo("Busca las pilas dentro de los cajones.");

        esperandoSalir = true;

        if (botonSalirX != null)
            botonSalirX.SetActive(true);
    }

    void ReproducirAbrirTapa()
    {
        if (abrirTapa == null) return;

        abrirTapa.SetActive(true);

        Animator anim = abrirTapa.GetComponent<Animator>();
        if (anim == null) return;

        anim.enabled = true;
        anim.applyRootMotion = false;
        anim.cullingMode = AnimatorCullingMode.AlwaysAnimate;
        anim.speed = velocidadAnimacion;
        anim.Rebind();
        anim.Update(0f);
        anim.Play(nombreAnimacionAbrir, 0, 0f);
    }

    public void ActivarModoInsertarPilas()
    {
        modoInsertarPilas = true;
        secuenciaIniciada = false;
        esperandoSalir = false;
        mirandoRadio = true;

        if (radioSinPilasVisual != null)
            radioSinPilasVisual.SetActive(false);

        if (radioAbiertoSinPilas != null)
            radioAbiertoSinPilas.SetActive(true);

        MostrarPrompt("Presiona B para poner la primera pila");
    }

    public bool PuedePonerPila(int numeroPila)
    {
        if (!modoInsertarPilas) return false;
        if (animandoPila) return false;

        if (numeroPila == 1) return !pila1Puesta;
        if (numeroPila == 2) return pila1Puesta && !pila2Puesta;
        if (numeroPila == 3) return pila1Puesta && pila2Puesta && !pila3Puesta;

        return false;
    }

    public void PonerPila(int numeroPila)
    {
        if (numeroPila == 1 && PuedePonerPila(1))
            StartCoroutine(AnimarPrimeraPila());

        if (numeroPila == 2 && PuedePonerPila(2))
            StartCoroutine(AnimarSegundaPila());

        if (numeroPila == 3 && PuedePonerPila(3))
            StartCoroutine(AnimarTerceraPila());
    }

    IEnumerator AnimarPrimeraPila()
    {
        animandoPila = true;
        OcultarPrompt();

        if (radioAbiertoSinPilas != null)
            radioAbiertoSinPilas.SetActive(false);

        if (primeraPila != null)
            primeraPila.SetActive(true);

        ReproducirAnimator(animatorPrimeraPila, primeraPila, nombreAnimacionPrimeraPila);

        yield return new WaitForSeconds(tiempoAnimacionPila);

        pila1Puesta = true;
        animandoPila = false;

        MostrarPrompt("Presiona B para poner la segunda pila");
    }

    IEnumerator AnimarSegundaPila()
    {
        animandoPila = true;
        OcultarPrompt();

        if (primeraPila != null)
            primeraPila.SetActive(false);

        if (segundaPila != null)
            segundaPila.SetActive(true);

        ReproducirAnimator(animatorSegundaPila, segundaPila, nombreAnimacionSegundaPila);

        yield return new WaitForSeconds(tiempoAnimacionPila);

        pila2Puesta = true;
        animandoPila = false;

        MostrarPrompt("Presiona B para poner la tercera pila");
    }

    IEnumerator AnimarTerceraPila()
    {
        animandoPila = true;
        OcultarPrompt();

        if (segundaPila != null)
            segundaPila.SetActive(false);

        if (terceraPila != null)
            terceraPila.SetActive(true);

        ReproducirAnimator(animatorTerceraPila, terceraPila, nombreAnimacionTerceraPila);

        yield return new WaitForSeconds(tiempoAnimacionPila);

        pila3Puesta = true;
        puedeCerrarTapa = true;
        animandoPila = false;

        MostrarPrompt("Presiona B para cerrar la tapa");
    }

    public bool PuedeCerrarTapa()
    {
        return modoInsertarPilas && pila3Puesta && puedeCerrarTapa && !tapaCerrada && !animandoPila;
    }

    public void CerrarTapaDesdeTrigger()
    {
        if (PuedeCerrarTapa())
            StartCoroutine(AnimarCerrarTapa());
    }

    IEnumerator AnimarCerrarTapa()
    {
        animandoPila = true;
        puedeCerrarTapa = false;
        OcultarPrompt();

        if (tapaRenderer != null)
            tapaRenderer.material.color = colorOriginalTapa;

        if (terceraPila != null)
            terceraPila.SetActive(false);

        if (cerrarTapa != null)
            cerrarTapa.SetActive(true);

        Animator animCerrar = null;

        if (cerrarTapa != null)
            animCerrar = cerrarTapa.GetComponent<Animator>();

        if (animCerrar != null)
        {
            animCerrar.enabled = true;
            animCerrar.applyRootMotion = false;
            animCerrar.cullingMode = AnimatorCullingMode.AlwaysAnimate;
            animCerrar.speed = velocidadAnimacionPilas;
            animCerrar.Rebind();
            animCerrar.Update(0f);
            animCerrar.Play(nombreAnimacionCerrarTapa, 0, 0f);
        }

        yield return new WaitForSeconds(tiempoAnimacionPila);

        tapaCerrada = true;
        animandoPila = false;

        yield return new WaitForSeconds(0.3f);

        ActivarRadioParaMusica();
    }

    void ActivarRadioParaMusica()
    {
        if (primeraPila != null) primeraPila.SetActive(false);
        if (segundaPila != null) segundaPila.SetActive(false);
        if (terceraPila != null) terceraPila.SetActive(false);
        if (cerrarTapa != null) cerrarTapa.SetActive(false);

        if (radioNormalVisual != null) radioNormalVisual.SetActive(false);
        if (radioVolteadoVisual != null) radioVolteadoVisual.SetActive(false);
        if (radioSinPilasVisual != null) radioSinPilasVisual.SetActive(false);
        if (radioAbiertoSinPilas != null) radioAbiertoSinPilas.SetActive(false);

        if (radioParaMusica != null)
            radioParaMusica.SetActive(true);

        modoInsertarPilas = false;
        mirandoRadio = false;
        secuenciaIniciada = true;

        OcultarPrompt();

        Debug.Log("RADIO FINAL: Se activó radioParaMusica.");
    }

    void ReproducirAnimator(Animator animator, GameObject objeto, string nombreEstado)
    {
        if (objeto != null)
            objeto.SetActive(true);

        if (animator == null && objeto != null)
            animator = objeto.GetComponent<Animator>();

        if (animator == null)
        {
            Debug.LogError("No hay Animator para: " + nombreEstado);
            return;
        }

        animator.enabled = true;
        animator.applyRootMotion = false;
        animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
        animator.speed = velocidadAnimacionPilas;
        animator.Rebind();
        animator.Update(0f);
        animator.Play(nombreEstado, 0, 0f);
    }

    void ActivarSolo(GameObject objetoActivo)
    {
        if (radioNormalVisual != null)
            radioNormalVisual.SetActive(radioNormalVisual == objetoActivo);

        if (radioVolteadoVisual != null)
            radioVolteadoVisual.SetActive(radioVolteadoVisual == objetoActivo);

        if (abrirTapa != null)
            abrirTapa.SetActive(abrirTapa == objetoActivo);

        if (radioSinPilasVisual != null)
            radioSinPilasVisual.SetActive(radioSinPilasVisual == objetoActivo);

        if (radioAbiertoSinPilas != null)
            radioAbiertoSinPilas.SetActive(radioAbiertoSinPilas == objetoActivo);
    }

    void SalirABuscarPilas()
    {
        esperandoSalir = false;
        mirandoRadio = false;

        OcultarDialogo();

        if (selectedRaycast != null)
            selectedRaycast.enabled = true;
    }

    public void MirarRadio()
    {
        if (!radioDesbloqueada)
        {
            mirandoRadio = false;
            OcultarPrompt();
            return;
        }

        if (secuenciaIniciada && !modoInsertarPilas)
            return;

        mirandoRadio = true;
    }

    public void DejarMirarRadio()
    {
        if (secuenciaIniciada || esperandoSalir) return;

        mirandoRadio = false;
        OcultarPrompt();
    }

    public void DesbloquearRadio()
    {
        radioDesbloqueada = true;
        Debug.Log("Radio desbloqueada después de hablar con Rosa.");
    }

    public void ActivarModoVolverARadio()
    {
        radioDesbloqueada = true;

        if (radioNormalVisual != null)
            radioNormalVisual.SetActive(true);
    }

    void MostrarPrompt(string texto)
    {
        if (promptPanel != null)
            promptPanel.SetActive(true);

        if (promptText != null)
            promptText.text = texto;
    }

    void OcultarPrompt()
    {
        if (promptPanel != null)
            promptPanel.SetActive(false);
    }

    void MostrarDialogo(string texto)
    {
        if (dialogoPanel != null)
            dialogoPanel.SetActive(true);

        if (nombreDialogoText != null)
            nombreDialogoText.text = nombreNPC;

        if (dialogoText != null)
            dialogoText.text = texto;

        if (botonSalirX != null)
            botonSalirX.SetActive(false);
    }

    void OcultarDialogo()
    {
        if (dialogoPanel != null)
            dialogoPanel.SetActive(false);

        if (botonSalirX != null)
            botonSalirX.SetActive(false);
    }
}