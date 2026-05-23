using UnityEngine;
using TMPro;
using System.Collections;

public class RadioAnimacionesSimple : MonoBehaviour
{
    [Header("Tiempos pilas")]
    public float tiempoAnimacionPila = 6f;

    [Header("Velocidad animaciones pilas")]
    public float velocidadAnimacionPilas = 0.35f;

    [Header("Control pilas")]
    private bool animandoPila = false;
    private bool pila1Puesta = false;
    private bool pila2Puesta = false;
    private bool pila3Puesta = false;

    [Header("Animators pilas")]
    public Animator animatorPrimeraPila;
    public Animator animatorSegundaPila;
    public Animator animatorTerceraPila;

    [Header("Estados visuales del radio abierto")]
    public GameObject radioAbiertoConTresPilas;
    public GameObject radioAbiertoConDosPilas;
    public GameObject radioAbiertoConUnaPila;
    public GameObject radioAbiertoSinPilas;

    [Header("Final")]
    public GameObject tapaCerrarMusica;
    public GameObject radioNormalFinal;
    public GameObject primeraPilaAnimacion;

    [Header("Control de misión")]
    public bool radioDesbloqueada = false;

    [Header("Sistema Raycast")]
    public Selected selectedRaycast;

    [Header("Radio - Visuales")]
    public GameObject radioNormalVisual;
    public GameObject radioVolteadoVisual;
    public GameObject abrirTapa;
    public GameObject radioSinPilasVisual;
    public GameObject radioAbiertoConPilas;

    [Header("Objetos de animación de pilas")]
    public GameObject primeraPila;
    public GameObject segundaPila;
    public GameObject terceraPila;
    public GameObject cerrarTapa;

    [Header("Tapita")]
    public Renderer tapaRenderer;
    public Color colorTapaSeleccionada = new Color(0.1f, 1f, 0.25f, 1f);

    [Header("Prompt")]
    public GameObject promptPanel;
    public TMP_Text promptText;

    [Header("Diálogo Rosa")]
    public GameObject dialogoPanel;
    public TMP_Text nombreDialogoText;
    public TMP_Text dialogoText;
    public GameObject botonSalirX;
    public string nombreNPC = "Rosa";

    [Header("Tiempos")]
    public float velocidadAnimacion = 0.45f;
    public float tiempoDialogoRosa = 4f;

    [Header("Animación tapa")]
    public string nombreAnimacionAbrir = "AbrirTapa";

    private bool mirandoRadio = false;
    private bool secuenciaIniciada = false;
    private bool esperandoSalir = false;
    private bool modoInsertarPilas = false;
    private int pilaActual = 0;
    private Color colorOriginalTapa;

    void Start()
    {
        OcultarPrompt();
        OcultarDialogo();

        if (radioNormalVisual != null)
            radioNormalVisual.SetActive(true);

        if (radioVolteadoVisual != null)
            radioVolteadoVisual.SetActive(false);

        if (abrirTapa != null)
            abrirTapa.SetActive(false);

        if (radioSinPilasVisual != null)
            radioSinPilasVisual.SetActive(false);

        if (tapaRenderer != null)
            colorOriginalTapa = tapaRenderer.material.color;
    }

    void Update()
    {
        Debug.Log(
            "Radio DEBUG | mirandoRadio: " + mirandoRadio +
            " | radioDesbloqueada: " + radioDesbloqueada +
            " | modoInsertarPilas: " + modoInsertarPilas +
            " | pila1Puesta: " + pila1Puesta
        );

        if (esperandoSalir)
        {
            if (InputManagerCustom.PressX())
                SalirABuscarPilas();

            return;
        }

        if (!mirandoRadio) return;
        if (!radioDesbloqueada) return;

        // ======================================================
        // 1. PRIMERA VEZ: revisar radio y abrir tapa
        // ======================================================
        if (!modoInsertarPilas && !secuenciaIniciada)
        {
            MostrarPrompt("Presiona B para revisar el radio");

            if (InputManagerCustom.PressB())
                StartCoroutine(SecuenciaRadio());

            return;
        }

        // ======================================================
        // 2. CUANDO YA TIENE PILAS: poner SOLO la primera pila
        // ======================================================
        if (modoInsertarPilas && !animandoPila && !pila1Puesta)
        {
            MostrarPrompt("Presiona B para poner la primera pila");

            if (InputManagerCustom.PressB())
            {
                StartCoroutine(AnimarPrimeraPila());
            }

            return;
        }

        // ======================================================
        // 3. DESPUÉS DE PONER LA PRIMERA PILA: poner segunda pila
        // ======================================================
        if (modoInsertarPilas && pila1Puesta && !animandoPila && !pila2Puesta)
        {
            MostrarPrompt("Presiona B para poner la segunda pila");

            if (InputManagerCustom.PressB())
            {
                StartCoroutine(AnimarSegundaPila());
            }

            return;
        }

        // ======================================================
        // 4. Después de poner la segunda pila
        // ======================================================
        if (modoInsertarPilas && pila2Puesta)
        {
            MostrarPrompt("Segunda pila puesta");
            return;
        }
    }

    IEnumerator AnimarPrimeraPila()
    {
        animandoPila = true;
        OcultarPrompt();

        Debug.Log("========== INICIO PILA 1 ==========");

        if (radioAbiertoSinPilas != null)
        {
            Debug.Log("Radio base antes: " + radioAbiertoSinPilas.name);
            Debug.Log("Radio base posición: " + radioAbiertoSinPilas.transform.position);
            radioAbiertoSinPilas.SetActive(false);
        }
        else
        {
            Debug.LogError("ERROR: Radio Abierto Sin Pilas está vacío en el Inspector.");
        }

        if (primeraPilaAnimacion == null)
        {
            Debug.LogError("ERROR: Primera Pila Animacion está vacío.");
            animandoPila = false;
            yield break;
        }

        primeraPilaAnimacion.SetActive(true);

        Debug.Log("Primera pila objeto: " + primeraPilaAnimacion.name);
        Debug.Log("Primera pila posición ANTES: " + primeraPilaAnimacion.transform.position);

        if (animatorPrimeraPila == null)
            animatorPrimeraPila = primeraPilaAnimacion.GetComponent<Animator>();

        if (animatorPrimeraPila == null)
        {
            Debug.LogError("ERROR: primera_pila no tiene Animator.");
            animandoPila = false;
            yield break;
        }

        animatorPrimeraPila.enabled = true;
        animatorPrimeraPila.speed = velocidadAnimacionPilas;
        animatorPrimeraPila.Rebind();
        animatorPrimeraPila.Update(0f);

        bool tieneEstado = animatorPrimeraPila.HasState(0, Animator.StringToHash("PrimeraPila"));
        Debug.Log("Tiene estado PrimeraPila: " + tieneEstado);

        if (!tieneEstado)
        {
            Debug.LogError("ERROR: El estado no se llama PrimeraPila.");
            animandoPila = false;
            yield break;
        }

        animatorPrimeraPila.Play("PrimeraPila", 0, 0f);

        yield return new WaitForSeconds(0.5f);

        Debug.Log("Primera pila posición DESPUÉS 0.5s: " + primeraPilaAnimacion.transform.position);

        yield return new WaitForSeconds(tiempoAnimacionPila);

        Debug.Log("Primera pila posición FINAL: " + primeraPilaAnimacion.transform.position);

        pila1Puesta = true;
        pilaActual = 1;
        animandoPila = false;

        MostrarPrompt("Primera pila puesta");

        Debug.Log("========== FIN PILA 1 ==========");
    }

    IEnumerator AnimarSegundaPila()
    {
        animandoPila = true;
        OcultarPrompt();

        Debug.Log("RADIO PILAS: Iniciando animación de segunda pila.");

        // Apagamos el estado anterior para que no tape la segunda animación.
        if (primeraPilaAnimacion != null)
            primeraPilaAnimacion.SetActive(false);

        if (primeraPila != null)
            primeraPila.SetActive(false);

        // Activamos la segunda animación.
        if (segundaPila != null)
            segundaPila.SetActive(true);

        if (animatorSegundaPila != null)
        {
            animatorSegundaPila.gameObject.SetActive(true);
            animatorSegundaPila.enabled = true;
            animatorSegundaPila.applyRootMotion = false;
            animatorSegundaPila.cullingMode = AnimatorCullingMode.AlwaysAnimate;
            animatorSegundaPila.speed = velocidadAnimacionPilas;

            animatorSegundaPila.Rebind();
            animatorSegundaPila.Update(0f);

            // IMPORTANTE: el estado naranja debe llamarse exactamente SegundaPila.
            animatorSegundaPila.Play("SegundaPila", 0, 0f);

            Debug.Log("RADIO PILAS: Animación SegundaPila reproducida.");
        }
        else
        {
            Debug.LogError("RADIO PILAS ERROR: No asignaste animatorSegundaPila en el Inspector.");
        }

        yield return new WaitForSeconds(tiempoAnimacionPila);

        pila2Puesta = true;
        pilaActual = 2;
        animandoPila = false;

        MostrarPrompt("Segunda pila puesta");

        Debug.Log("RADIO PILAS: Segunda pila quedó puesta.");
    }

    IEnumerator AnimarTerceraPila()
    {
        animandoPila = true;
        OcultarPrompt();

        Debug.Log("RADIO PILAS: Iniciando animación de tercera pila.");

        // Apagamos el estado anterior para que no tape la tercera animación.
        if (segundaPila != null)
            segundaPila.SetActive(false);

        // Activamos la tercera animación.
        if (terceraPila != null)
            terceraPila.SetActive(true);

        if (animatorTerceraPila != null)
        {
            animatorTerceraPila.gameObject.SetActive(true);
            animatorTerceraPila.enabled = true;
            animatorTerceraPila.applyRootMotion = false;
            animatorTerceraPila.cullingMode = AnimatorCullingMode.AlwaysAnimate;
            animatorTerceraPila.speed = velocidadAnimacionPilas;

            animatorTerceraPila.Rebind();
            animatorTerceraPila.Update(0f);

            // IMPORTANTE: el estado naranja debe llamarse exactamente TerceraPila.
            animatorTerceraPila.Play("TerceraPila", 0, 0f);

            Debug.Log("RADIO PILAS: Animación TerceraPila reproducida.");
        }
        else
        {
            Debug.LogError("RADIO PILAS ERROR: No asignaste animatorTerceraPila en el Inspector.");
        }

        yield return new WaitForSeconds(tiempoAnimacionPila);

        pila3Puesta = true;
        pilaActual = 3;
        animandoPila = false;

        MostrarPrompt("Tercera pila puesta");

        Debug.Log("RADIO PILAS: Tercera pila quedó puesta.");
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

        float tiempoBloqueo = 0.5f;
        float contador = 0f;

        while (contador < tiempoBloqueo)
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

    void SalirABuscarPilas()
    {
        esperandoSalir = false;

        OcultarDialogo();

        if (selectedRaycast != null)
            selectedRaycast.enabled = true;

        mirandoRadio = false;

        Debug.Log("Ahora el jugador puede buscar las pilas en los cajones.");
    }

    void ReproducirAbrirTapa()
    {
        if (abrirTapa == null)
        {
            Debug.LogError("No asignaste abrirTapa en el Inspector.");
            return;
        }

        abrirTapa.SetActive(false);
        abrirTapa.SetActive(true);

        Animator anim = abrirTapa.GetComponent<Animator>();

        if (anim == null)
        {
            Debug.LogError("abrirTapa no tiene Animator.");
            return;
        }

        anim.enabled = true;
        anim.applyRootMotion = false;
        anim.speed = velocidadAnimacion;

        anim.Rebind();
        anim.Update(0f);
        anim.Play(nombreAnimacionAbrir, 0, 0f);
    }

    public void MirarRadio()
    {
        if (!radioDesbloqueada)
        {
            mirandoRadio = false;
            OcultarPrompt();
            return;
        }

        if (secuenciaIniciada && !modoInsertarPilas) return;

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

        Debug.Log("RADIO PILAS: Modo insertar pilas activado.");
    }

    public void ActivarModoVolverARadio()
    {
        radioDesbloqueada = true;

        if (radioNormalVisual != null)
            radioNormalVisual.SetActive(true);

        Debug.Log("Ahora el jugador debe volver al radio.");
    }

    public void PonerPrimeraPila()
    {
        if (!pila1Puesta && !animandoPila)
            StartCoroutine(AnimarPrimeraPila());
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

    public bool PuedePonerPila(int numeroPila)
    {
        if (!modoInsertarPilas) return false;
        if (animandoPila) return false;

        if (numeroPila == 1)
            return !pila1Puesta;

        if (numeroPila == 2)
            return pila1Puesta && !pila2Puesta;

        if (numeroPila == 3)
            return pila1Puesta && pila2Puesta && !pila3Puesta;

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
}