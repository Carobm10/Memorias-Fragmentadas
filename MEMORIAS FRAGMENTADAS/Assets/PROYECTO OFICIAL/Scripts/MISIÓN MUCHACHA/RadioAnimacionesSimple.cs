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

    
    [Header("Estados después de poner pilas")]
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

    [Header("Animación")]
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
            " | modoInsertarPilas: " + modoInsertarPilas
        );
        if (esperandoSalir)
        {
            if (InputManagerCustom.PressX())
                SalirABuscarPilas();

            return;
        }

        if (!mirandoRadio) return;
        if (secuenciaIniciada) return;
        if (!radioDesbloqueada) return;

        // ==========================================
        // RADIO NORMAL
        // ==========================================

        if (!modoInsertarPilas && !secuenciaIniciada)
        {
            MostrarPrompt("Presiona B para revisar el radio");

            if (InputManagerCustom.PressB())
                StartCoroutine(SecuenciaRadio());

            return;
        }

        // ==========================================
        // VOLVER CON PILAS
        // ==========================================

        if (!modoInsertarPilas && !secuenciaIniciada)
        {
            MostrarPrompt("Presiona B para poner las pilas");

            if (InputManagerCustom.PressB())
            {
                modoInsertarPilas = true;

                if (radioSinPilasVisual != null)
                    radioSinPilasVisual.SetActive(false);

                if (radioAbiertoConPilas != null)
                    radioAbiertoConPilas.SetActive(true);
            }

            return;
        }

        // ==========================================
        // PILA 1
        // ==========================================

        if (modoInsertarPilas)
        {
            if (pilaActual == 0)
            {
                MostrarPrompt("Presiona B para poner la primera pila");

                if (InputManagerCustom.PressB())
                {
                    if (primeraPila != null)
                        primeraPila.SetActive(true);

                    pilaActual = 1;
                }

                return;
            }

            if (pilaActual == 1)
            {
                MostrarPrompt("Presiona B para poner la segunda pila");

                if (InputManagerCustom.PressB())
                {
                    if (segundaPila != null)
                        segundaPila.SetActive(true);

                    pilaActual = 2;
                }

                return;
            }

            if (pilaActual == 2)
            {
                MostrarPrompt("Presiona B para poner la tercera pila");

                if (InputManagerCustom.PressB())
                {
                    if (terceraPila != null)
                        terceraPila.SetActive(true);

                    pilaActual = 3;
                }

                return;
            }

            if (pilaActual == 3)
            {
                MostrarPrompt("Presiona B para cerrar la tapa");

                if (InputManagerCustom.PressB())
                {
                    if (cerrarTapa != null)
                        cerrarTapa.SetActive(true);

                    MostrarPrompt("Presiona X para salir");
                }

                return;
            }
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

        // Mantener el prompt visible mientras espera el segundo B
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

        if (secuenciaIniciada) return;

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
    
    
    public void ActivarModoVolverARadio()
    {
        radioDesbloqueada = true;

        if (radioNormalVisual != null)
        {
            radioNormalVisual.SetActive(true);
        }

        Debug.Log("Ahora el jugador debe volver al radio.");
    }
    public void ActivarModoInsertarPilas()
    {
        if (radioSinPilasVisual != null)
            radioSinPilasVisual.SetActive(false);

        if (radioAbiertoConPilas != null)
            radioAbiertoConPilas.SetActive(true);

        modoInsertarPilas = true;

        Debug.Log("Modo insertar pilas ACTIVADO");
    }
    public void PonerPrimeraPila()
    {
        if (primeraPila != null)
            primeraPila.SetActive(true);

        Debug.Log("Animación primera pila activada.");
    }
    public void PonerPila(int numeroPila)
    {
        if (animandoPila) return;

        StartCoroutine(SecuenciaPonerPilaSimple(numeroPila));
    }

    IEnumerator SecuenciaPonerPilaSimple(int numeroPila)
    {
        animandoPila = true;

        // Apagar radio base para que NO tape la animación
        if (radioAbiertoConTresPilas != null)
            radioAbiertoConTresPilas.SetActive(false);

        if (radioAbiertoSinPilas != null)
            radioAbiertoSinPilas.SetActive(false);

        if (tapaCerrarMusica != null)
            tapaCerrarMusica.SetActive(false);

        GameObject animacionActual = null;

        if (numeroPila == 1)
            animacionActual = primeraPila;
        else if (numeroPila == 2)
            animacionActual = segundaPila;
        else if (numeroPila == 3)
            animacionActual = terceraPila;

        if (animacionActual != null)
        {
            animacionActual.SetActive(true);

            Animator anim = animacionActual.GetComponent<Animator>();

            if (anim != null)
            {
                anim.speed = 0.35f;
                anim.Play(0, 0, 0f);
            }

            Debug.Log("Reproduciendo animación pila: " + numeroPila);
        }
        else
        {
            Debug.LogError("No asignaste la animación de la pila " + numeroPila);
        }

        yield return new WaitForSeconds(tiempoAnimacionPila);

        // Después de pila 1 y 2, vuelve el radio base para seleccionar la siguiente pila
        if (numeroPila == 1 || numeroPila == 2)
        {
            if (animacionActual != null)
                animacionActual.SetActive(false);

            if (radioAbiertoConTresPilas != null)
                radioAbiertoConTresPilas.SetActive(true);
        }

        // Después de pila 3, ya NO vuelve al radio con tres pilas
        if (numeroPila == 3)
        {
            if (animacionActual != null)
                animacionActual.SetActive(false);

            if (radioAbiertoSinPilas != null)
                radioAbiertoSinPilas.SetActive(true);

            if (tapaCerrarMusica != null)
                tapaCerrarMusica.SetActive(true);
        }

        animandoPila = false;
    }
    void OcultarRenderers(GameObject obj)
    {
        if (obj == null) return;

        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();

        foreach (Renderer r in renderers)
        {
            if (r != null)
                r.enabled = false;
        }
    }
    void OcultarTodosEstadosPilas()
    {
        if (radioAbiertoConTresPilas != null) radioAbiertoConTresPilas.SetActive(false);
        if (radioAbiertoConDosPilas != null) radioAbiertoConDosPilas.SetActive(false);
        if (radioAbiertoConUnaPila != null) radioAbiertoConUnaPila.SetActive(false);

        if (primeraPila != null) primeraPila.SetActive(false);
        if (segundaPila != null) segundaPila.SetActive(false);
        if (terceraPila != null) terceraPila.SetActive(false);
        if (radioAbiertoSinPilas != null) radioAbiertoSinPilas.SetActive(false);
    }

    void OcultarTodosLosEstadosDelRadio()
    {
        if (radioNormalVisual != null) radioNormalVisual.SetActive(false);
        if (radioVolteadoVisual != null) radioVolteadoVisual.SetActive(false);
        if (radioSinPilasVisual != null) radioSinPilasVisual.SetActive(false);

        if (radioAbiertoConTresPilas != null) radioAbiertoConTresPilas.SetActive(false);
        if (radioAbiertoConDosPilas != null) radioAbiertoConDosPilas.SetActive(false);
        if (radioAbiertoConUnaPila != null) radioAbiertoConUnaPila.SetActive(false);
        if (radioAbiertoSinPilas != null) radioAbiertoSinPilas.SetActive(false);

        if (primeraPila != null) primeraPila.SetActive(false);
        if (segundaPila != null) segundaPila.SetActive(false);
        if (terceraPila != null) terceraPila.SetActive(false);
    }
}