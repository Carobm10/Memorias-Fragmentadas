using UnityEngine;

public class MovimientoVR2 : MonoBehaviour
{
    public float velocidad = 3f;
    public Transform camara;
    public float gravedad = -9.8f;

    [Header("Control de movimiento")]
    public bool puedeMoverse = true;

    [Header("Head Bob (caminar)")]
    public bool activarHeadBob = true;
    public float bobFrecuencia = 6f;
    public float bobAltura = 0.05f;

    [Header("Sonido de pasos")]
    public bool activarSonidoPasos = true;
    public bool usarVolumenCaminar = true;
    [Range(0f, 1f)]
    public float volumenCaminar = 1f;
    [Range(0.1f, 5f)]
    public float pasosPorSegundoMin = 1f;
    [Range(0.1f, 8f)]
    public float pasosPorSegundoMax = 3f;
    [Range(0.8f, 1.2f)]
    public float pitchPasoMin = 0.95f;
    [Range(0.8f, 1.2f)]
    public float pitchPasoMax = 1.05f;
    [Range(0f, 1f)]
    public float volumenPasoMin = 0.85f;
    [Range(0f, 1f)]
    public float volumenPasoMax = 1f;
    [Range(0f, 0.5f)]
    public float umbralMovimientoPasos = 0.03f;
    [Range(0.1f, 6f)]
    public float pasosPorSegundoBase = 1.2f;
    public AudioClip paso_madera_1;
    public AudioClip paso_madera_2;
    public AudioClip paso_madera_3;
    public AudioClip paso_piso_1;
    public AudioClip paso_piso_2;

    [Header("Detección de superficie")]
    [Tooltip("Punto opcional desde el que se hace el raycast hacia abajo para detectar el suelo. Si está vacío, se usa el CharacterController.")]
    public Transform origenDeteccionSuelo;
    [Tooltip("Si está activo, la detección parte del CharacterController del jugador antes que del Transform manual.")]
    public bool usarCharacterControllerComoOrigen = true;
    [Tooltip("Nombres o fragmentos que identifican madera. Ejemplo: piso_completo.001, wood")]
    public string[] nombresSuperficieMadera = new string[] { "piso_completo.001", "wood" };
    [Tooltip("Nombres o fragmentos que identifican piso/losa. Ejemplo: piso_completo.002")]
    public string[] nombresSuperficiePiso = new string[] { "piso_completo.002", "decorated tile", "tiles detailed", "tile" };

    [Header("Debug")]
    [Tooltip("Muestra en consola el nombre del objeto detectado bajo el jugador")]
    public bool mostrarDebugSuperficie = false;

    [Header("Pruebas")]
    public bool activarModeloYAnimacion = false;

    [Header("Animación")]
    public Animator animator;

    [Header("Rotación visual")]
    public Transform modelo;
    public float velocidadRotacion = 10f;

    [Header("Suavizado")]
    public float suavizado = 5f;

    private CharacterController controller;
    private float velocidadY;
    private Vector3 velocidadActual;
    private AudioSource audioPasosSource;

    private float bobTiempo;
    private Vector3 camaraPosInicial;
    private float pasoAcumulado;
    private int ultimoIndicePaso = -1;

    private enum TipoSuperficiePaso
    {
        Madera,
        Piso
    }

    void Start()
    {
        controller = GetComponent<CharacterController>();

        audioPasosSource = GetComponent<AudioSource>();
        if (audioPasosSource == null)
        {
            audioPasosSource = gameObject.AddComponent<AudioSource>();
        }

        audioPasosSource.playOnAwake = false;
        audioPasosSource.loop = false;
        audioPasosSource.spatialBlend = 0f;

        if (camara == null && Camera.main != null)
        {
            camara = Camera.main.transform;
        }

        if (camara != null)
        {
            camaraPosInicial = camara.localPosition;
        }
    }

    void Update()
    {
        if (controller == null || camara == null)
            return;

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 forward = camara.forward;
        Vector3 right = camara.right;

        forward.y = 0;
        right.y = 0;

        Vector3 direccion = (forward * v + right * h).normalized;

        if (!puedeMoverse)
        {
            direccion = Vector3.zero;
        }

        if (activarModeloYAnimacion)
        {
            if (animator != null)
            {
                float movimientoInput = direccion.magnitude;
                animator.SetFloat("movement", movimientoInput);
            }

            if (modelo != null)
            {
                Vector3 direccionCamara = camara.forward;
                direccionCamara.y = 0;

                if (direccionCamara.sqrMagnitude > 0.01f)
                {
                    Quaternion rotacionObjetivo = Quaternion.LookRotation(direccionCamara);
                    modelo.rotation = Quaternion.Slerp(
                        modelo.rotation,
                        rotacionObjetivo,
                        velocidadRotacion * Time.deltaTime
                    );
                }
            }
        }

        Vector3 velocidadObjetivo = direccion * velocidad;
        float factorSuavizado = 1f - Mathf.Exp(-Mathf.Max(0.01f, suavizado) * Time.deltaTime);
        velocidadActual = Vector3.Lerp(velocidadActual, velocidadObjetivo, factorSuavizado);

        if (controller.isGrounded && velocidadY < 0)
        {
            velocidadY = -2f;
        }

        velocidadY += gravedad * Time.deltaTime;

        Vector3 movimientoFinal = velocidadActual;
        movimientoFinal.y = velocidadY;

        controller.Move(movimientoFinal * Time.deltaTime);

        float velocidadHorizontal = new Vector3(velocidadActual.x, 0f, velocidadActual.z).magnitude;
        float movimientoNormalizado = Mathf.InverseLerp(umbralMovimientoPasos, Mathf.Max(umbralMovimientoPasos + 0.01f, velocidad), velocidadHorizontal);
        bool estaMoviendose = direccion.magnitude > umbralMovimientoPasos && controller.isGrounded;

        ActualizarPasos(estaMoviendose, movimientoNormalizado);

        if (activarHeadBob && estaMoviendose)
        {
            float bobFrecuenciaActual = Mathf.Lerp(bobFrecuencia * 0.75f, bobFrecuencia * 1.25f, movimientoNormalizado);
            float bobAlturaActual = Mathf.Lerp(bobAltura * 0.5f, bobAltura, movimientoNormalizado);

            bobTiempo += Time.deltaTime * bobFrecuenciaActual;
            float bobOffset = Mathf.Sin(bobTiempo) * bobAlturaActual;

            camara.localPosition = camaraPosInicial + new Vector3(0, bobOffset, 0);
        }
        else
        {
            bobTiempo = 0;
            pasoAcumulado = 0f;
            camara.localPosition = Vector3.Lerp(
                camara.localPosition,
                camaraPosInicial,
                Time.deltaTime * 5f
            );
        }

        //controller.center = new Vector3(0, controller.height / 2, 0);
    }

    private void ActualizarPasos(bool estaMoviendose, float movimientoNormalizado)
    {
        if (!activarSonidoPasos || audioPasosSource == null)
        {
            return;
        }

        if (!estaMoviendose)
        {
            pasoAcumulado = 0f;
            return;
        }

        float pasosPorSegundoActual = Mathf.Max(
            pasosPorSegundoBase,
            Mathf.Lerp(pasosPorSegundoMin, pasosPorSegundoMax, movimientoNormalizado)
        );
        pasoAcumulado += Time.deltaTime * pasosPorSegundoActual;

        while (pasoAcumulado >= 1f)
        {
            pasoAcumulado -= 1f;
            ReproducirPaso(movimientoNormalizado);
        }
    }

    private void ReproducirPaso(float movimientoNormalizado)
    {
        TipoSuperficiePaso tipoSuperficie = DetectarSuperficiePaso();
        AudioClip clipPaso = ObtenerClipPasoAleatorio(tipoSuperficie);
        if (clipPaso == null || audioPasosSource == null)
        {
            return;
        }

        float volumenPaso = Mathf.Clamp01(volumenCaminar);
        if (usarVolumenCaminar)
        {
            float factorMovimiento = Mathf.Lerp(volumenPasoMin, volumenPasoMax, movimientoNormalizado);
            volumenPaso *= Mathf.Max(0.9f, factorMovimiento);
        }

        if (volumenPaso <= 0f)
        {
            return;
        }

        audioPasosSource.pitch = Random.Range(pitchPasoMin, pitchPasoMax);
        audioPasosSource.PlayOneShot(clipPaso, Mathf.Clamp01(volumenPaso));
    }

    private AudioClip ObtenerClipPasoAleatorio(TipoSuperficiePaso tipoSuperficie)
    {
        AudioClip[] clips = tipoSuperficie == TipoSuperficiePaso.Piso
            ? new AudioClip[] { paso_piso_1, paso_piso_2 }
            : new AudioClip[] { paso_madera_1, paso_madera_2, paso_madera_3 };

        int cantidadValidos = 0;

        for (int i = 0; i < clips.Length; i++)
        {
            if (clips[i] != null)
            {
                cantidadValidos++;
            }
        }

        if (cantidadValidos == 0)
        {
            return null;
        }

        if (cantidadValidos == 1)
        {
            for (int i = 0; i < clips.Length; i++)
            {
                if (clips[i] != null)
                {
                    ultimoIndicePaso = i;
                    return clips[i];
                }
            }
        }

        int indice = ultimoIndicePaso;
        for (int intentos = 0; intentos < 8; intentos++)
        {
            indice = Random.Range(0, clips.Length);
            if (clips[indice] != null && indice != ultimoIndicePaso)
            {
                ultimoIndicePaso = indice;
                return clips[indice];
            }
        }

        for (int i = 0; i < clips.Length; i++)
        {
            if (clips[i] != null)
            {
                ultimoIndicePaso = i;
                return clips[i];
            }
        }

        return null;
    }

    private TipoSuperficiePaso DetectarSuperficiePaso()
    {
        if (controller == null)
        {
            return TipoSuperficiePaso.Madera;
        }

        // CORRECCIÓN: origen justo en el borde inferior del CharacterController + pequeño offset hacia arriba
        Vector3 origen;
        if (origenDeteccionSuelo != null)
        {
            origen = origenDeteccionSuelo.position + Vector3.up * 0.05f;
        }
        else if (usarCharacterControllerComoOrigen)
        {
            // Pie del controller + pequeño offset para no empezar dentro del suelo
            origen = new Vector3(controller.bounds.center.x,
                                 controller.bounds.min.y + 0.05f,
                                 controller.bounds.center.z);
        }
        else
        {
            origen = transform.position + Vector3.up * 0.05f;
        }

        // Distancia corta: solo necesitamos detectar el suelo inmediato
        float distancia = 0.3f;

        if (!Physics.Raycast(origen, Vector3.down, out RaycastHit hit, distancia))
        {
            // Si no impacta con distancia corta, intentar con un poco más de margen
            distancia = 0.6f;
            if (!Physics.Raycast(origen, Vector3.down, out hit, distancia))
            {
                return TipoSuperficiePaso.Madera;
            }
        }

        // CORRECCIÓN PRINCIPAL: buscar el nombre en el collider golpeado y su jerarquía,
        // pero priorizando el objeto más específico (el propio collider) antes de subir a padres.
        string nombreDetectado = ObtenerNombreSuperficie(hit.collider);

        if (mostrarDebugSuperficie)
        {
            Debug.Log($"[Pasos] Superficie detectada: '{nombreDetectado}' | Objeto del collider: '{hit.collider.gameObject.name}' | Distancia: {hit.distance:F3}");
        }

        if (CoincideConLista(nombreDetectado, nombresSuperficiePiso))
        {
            return TipoSuperficiePaso.Piso;
        }

        if (CoincideConLista(nombreDetectado, nombresSuperficieMadera))
        {
            return TipoSuperficiePaso.Madera;
        }

        // Fallback: madera por defecto
        return TipoSuperficiePaso.Madera;
    }

    private string ObtenerNombreSuperficie(Collider collider)
    {
        if (collider == null)
        {
            return string.Empty;
        }

        // CORRECCIÓN: primero revisar el nombre del propio GameObject del collider.
        // Solo subimos por la jerarquía si el nombre directo no coincide con nada.
        // Esto evita que siempre se retorne el nombre del objeto raíz padre.
        string nombreDirecto = collider.gameObject.name;
        if (!string.IsNullOrEmpty(nombreDirecto))
        {
            // Si el nombre directo coincide con alguna lista, retornarlo de inmediato
            if (CoincideConLista(nombreDirecto, nombresSuperficiePiso) ||
                CoincideConLista(nombreDirecto, nombresSuperficieMadera))
            {
                return nombreDirecto;
            }
        }

        // Si el nombre directo no coincide, buscar en la jerarquía de padres
        // (por si el collider está en un hijo y el nombre está en el padre inmediato)
        Transform actual = collider.transform.parent;
        int nivelMax = 4; // Limitar cuántos niveles subimos para evitar llegar al root

        while (actual != null && nivelMax > 0)
        {
            string nombre = actual.gameObject.name;
            if (!string.IsNullOrEmpty(nombre))
            {
                if (CoincideConLista(nombre, nombresSuperficiePiso) ||
                    CoincideConLista(nombre, nombresSuperficieMadera))
                {
                    return nombre;
                }
            }
            actual = actual.parent;
            nivelMax--;
        }

        // Si no encontramos nada en la jerarquía, retornar el nombre directo del collider
        return nombreDirecto;
    }

    private bool CoincideConLista(string nombreSuperficie, string[] candidatos)
    {
        if (string.IsNullOrEmpty(nombreSuperficie) || candidatos == null)
        {
            return false;
        }

        string nombre = nombreSuperficie.ToLowerInvariant();

        for (int i = 0; i < candidatos.Length; i++)
        {
            string candidato = candidatos[i];
            if (string.IsNullOrWhiteSpace(candidato))
            {
                continue;
            }

            if (nombre.Contains(candidato.ToLowerInvariant()))
            {
                return true;
            }
        }

        return false;
    }
}