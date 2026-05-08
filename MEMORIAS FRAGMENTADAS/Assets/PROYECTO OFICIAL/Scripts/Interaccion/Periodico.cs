using UnityEngine;

public class AbrirPeriodico : MonoBehaviour
{
    [Header("Configuración")]
    public float velocidad = 2f;

    [Tooltip("Rotación que hará la hoja al abrirse")]
    public Vector3 rotacionAbierta = new Vector3(0f, 0f, -180f);

    private Quaternion rotacionInicial;
    private Quaternion rotacionFinal;

    private bool abierto = false;

    void Start()
    {
        // Guarda la rotación inicial EXACTA
        rotacionInicial = transform.localRotation;

        // Calcula la rotación abierta relativa a la inicial
        rotacionFinal = rotacionInicial * Quaternion.Euler(rotacionAbierta);
    }

    void Update()
    {
        Quaternion objetivo = abierto ? rotacionFinal : rotacionInicial;

        transform.localRotation = Quaternion.Slerp(
            transform.localRotation,
            objetivo,
            velocidad * Time.deltaTime
        );
    }

    void OnMouseDown()
    {
        // Cambia entre abierto y cerrado
        abierto = !abierto;
    }
}