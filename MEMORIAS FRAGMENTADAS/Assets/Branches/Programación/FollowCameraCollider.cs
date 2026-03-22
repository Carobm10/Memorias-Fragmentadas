using UnityEngine;

public class MovimientoVR : MonoBehaviour
{
    [Header("Movimiento")]
    public float velocidad = 3f;
    public Transform camara;
    public float gravedad = -9.8f;

    [Header("Suavizado")]
    public float tiempoSuavizado = 0.15f;
    public float suavizadoInput = 5f;

    [Header("Head Bob")]
    public float bobFrecuencia = 6f;
    public float bobAltura = 0.05f;

    private CharacterController controller;

    private float velocidadY;
    private Vector3 velocidadActual;
    private Vector3 velocidadSuavizada;

    // 🔥 NUEVO: input suavizado
    private float inputH;
    private float inputV;

    private float bobTiempo;
    private Vector3 camaraPosInicial;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        camaraPosInicial = camara.localPosition;
    }

    void Update()
    {
        // INPUT RAW
        float hRaw = Input.GetAxisRaw("Horizontal");
        float vRaw = Input.GetAxisRaw("Vertical");

        // 🔥 SUAVIZAR INPUT (esto elimina el tirón)
        inputH = Mathf.Lerp(inputH, hRaw, suavizadoInput * Time.deltaTime);
        inputV = Mathf.Lerp(inputV, vRaw, suavizadoInput * Time.deltaTime);

        Vector3 forward = camara.forward;
        Vector3 right = camara.right;

        forward.y = 0;
        right.y = 0;

        Vector3 direccion = (forward * inputV + right * inputH).normalized;

        // MOVIMIENTO SUAVE
        Vector3 velocidadObjetivo = direccion * velocidad;

        velocidadActual = Vector3.SmoothDamp(
            velocidadActual,
            velocidadObjetivo,
            ref velocidadSuavizada,
            tiempoSuavizado
        );

        // GRAVEDAD
        if (controller.isGrounded && velocidadY < 0)
        {
            velocidadY = -2f;
        }

        velocidadY += gravedad * Time.deltaTime;

        Vector3 movimientoFinal = velocidadActual;
        movimientoFinal.y = velocidadY;

        controller.Move(movimientoFinal * Time.deltaTime);

        // HEAD BOB
        if (direccion.magnitude > 0.1f && controller.isGrounded)
        {
            bobTiempo += Time.deltaTime * bobFrecuencia;
            float bobOffset = Mathf.Sin(bobTiempo) * bobAltura;

            camara.localPosition = camaraPosInicial + new Vector3(0, bobOffset, 0);
        }
        else
        {
            bobTiempo = 0;
            camara.localPosition = Vector3.Lerp(
                camara.localPosition,
                camaraPosInicial,
                Time.deltaTime * 5f
            );
        }

        // AJUSTE VR
        Vector3 center = camara.localPosition;
        center.y = controller.height / 2;
        controller.center = center;
    }
}