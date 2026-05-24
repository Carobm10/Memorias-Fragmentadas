using UnityEngine;
using System.Collections;

public class PanelFlotando : MonoBehaviour
{
    [Header("Flotación")]
    public float velocidadFlotacion = 2f;
    public float alturaFlotacion = 0.05f;

    [Header("Rotación")]
    public bool rotar = true;
    public float velocidadRotacion = 5f;

    [Header("Tiempo visible")]
    public float tiempoVisible = 5f;

    [Header("Velocidad de desaparición")]
    public float velocidadDesaparicion = 3f;

    private Vector3 posicionInicial;

    void Start()
    {
        posicionInicial = transform.localPosition;

        StartCoroutine(Desaparecer());
    }

    void Update()
    {
        // =========================
        // FLOTACIÓN
        // =========================

        float nuevaY =
            posicionInicial.y +
            Mathf.Sin(Time.time * velocidadFlotacion) * alturaFlotacion;

        transform.localPosition = new Vector3(
            posicionInicial.x,
            nuevaY,
            posicionInicial.z
        );

        // =========================
        // ROTACIÓN
        // =========================

        if (rotar)
        {
            transform.Rotate(
                0,
                velocidadRotacion * Time.deltaTime,
                0
            );
        }
    }

    IEnumerator Desaparecer()
    {
        // Espera antes de desaparecer
        yield return new WaitForSeconds(tiempoVisible);

        float tiempo = 0f;

        Vector3 escalaInicial = transform.localScale;

        while (tiempo < 1f)
        {
            tiempo += Time.deltaTime * velocidadDesaparicion;

            transform.localScale = Vector3.Lerp(
                escalaInicial,
                Vector3.zero,
                tiempo
            );

            yield return null;
        }

        gameObject.SetActive(false);
    }
}
