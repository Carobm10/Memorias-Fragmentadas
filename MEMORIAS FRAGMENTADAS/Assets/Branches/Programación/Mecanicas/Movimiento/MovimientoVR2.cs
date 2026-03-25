using UnityEngine;

public class MovimientoVR2 : MonoBehaviour
{
    public float velocidad = 3f;
    public Transform camara;
    public float gravedad = -9.8f;

    [Header("Animación")]
    public Animator animator;

    [Header("Suavizado")]
    public float suavizado = 5f;

    [Header("Head Bob (caminar)")]
    public float bobFrecuencia = 6f;
    public float bobAltura = 0.05f;

    private CharacterController controller;
    private float velocidadY;
    private Vector3 velocidadActual;

    private float bobTiempo;
    private Vector3 camaraPosInicial;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        camaraPosInicial = camara.localPosition;
    }

    void Update()
    {
        // INPUT (WASD)
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 forward = camara.forward;
        Vector3 right = camara.right;

        forward.y = 0;
        right.y = 0;

        Vector3 direccion = (forward * v + right * h).normalized;

        // 🔥 ANIMACIÓN
        float movimientoInput = direccion.magnitude; // 0 o 1
        animator.SetFloat("movement", movimientoInput);

        // SUAVIZADO
        Vector3 velocidadObjetivo = direccion * velocidad;
        velocidadActual = Vector3.Lerp(velocidadActual, velocidadObjetivo, suavizado * Time.deltaTime);

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

        controller.center = new Vector3(0, controller.height / 2, 0);
    }
}