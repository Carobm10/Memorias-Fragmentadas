using TMPro;
using UnityEngine;
using System.Collections;

public class EscrituraPapel : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private GameObject hojaActual;
    [SerializeField] private GameObject hojaPrefab; // 👈 IMPORTANTE asignar en inspector
    [SerializeField] private TextMeshPro textoHoja;

    private string textoActual = "";

    private Vector3 posicionInicial;
    private Vector3 escalaInicial;

    private Quaternion rotacionInicial;

    void Start()
    {
        posicionInicial = hojaActual.transform.localPosition;
        rotacionInicial = hojaActual.transform.localRotation;
        escalaInicial = hojaActual.transform.localScale;
    }

    // ✍️ Escribir en la hoja
    public void Escribir(string letra)
    {
        textoActual += letra;
        textoHoja.text = textoActual;
    }

    // 🧾 Activar cambio de hoja
    public void NuevaHoja()
    {
        StartCoroutine(AnimacionCambioHoja());
    }

    // 🎬 Animación de caída + fade
    IEnumerator AnimacionCambioHoja()
    {
        GameObject hojaVieja = hojaActual;

        // 1. Separar de la máquina
        hojaVieja.transform.parent = null;

        // 2. Agregar física tipo papel
        Rigidbody rb = hojaVieja.AddComponent<Rigidbody>();
        rb.mass = 0.05f;
        rb.linearDamping = 2f;
        rb.angularDamping = 2f;
        rb.useGravity = true;

        // 3. Empuje suave
        rb.AddForce(Vector3.down * 0.5f + Vector3.forward * 0.3f, ForceMode.Impulse);

        // 4. Fade + movimiento
        Renderer rend = hojaVieja.GetComponentInChildren<Renderer>();
        Material mat = rend.material;

        float t = 0;

        while (t < 1)
        {
            t += Time.deltaTime;

            // Fade
            Color c = mat.color;
            c.a = Mathf.Lerp(1, 0, t);
            mat.color = c;

            // Rotación
            hojaVieja.transform.Rotate(Vector3.right * 100 * Time.deltaTime);

            // Movimiento flotante (efecto papel)
            float movimiento = Mathf.Sin(Time.time * 5f) * 0.01f;
            hojaVieja.transform.position += new Vector3(movimiento, 0, 0);

            yield return null;
        }

        // 5. Eliminar hoja vieja
        Destroy(hojaVieja);

        // 6. Crear nueva hoja
        CrearNuevaHoja();
    }

    // 🆕 Instanciar nueva hoja limpia
    void CrearNuevaHoja()
    {
        GameObject nuevaHoja = Instantiate(hojaPrefab, transform);

        nuevaHoja.transform.localPosition = posicionInicial;
        nuevaHoja.transform.localRotation = rotacionInicial;
        nuevaHoja.transform.localScale = escalaInicial;

        hojaActual = nuevaHoja;

        textoHoja = nuevaHoja.GetComponentInChildren<TextMeshPro>();

        textoActual = "";
        textoHoja.text = "";
    }

    // 📄 Obtener texto actual
    public string ObtenerTexto()
    {
        return textoActual;
    }
}