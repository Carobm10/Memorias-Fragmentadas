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

    private float bobTiempo;
    private Vector3 camaraPosInicial;

    void Start()
    {
        controller = GetComponent<CharacterController>();

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

        if (activarHeadBob && direccion.magnitude > 0.1f && controller.isGrounded)
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