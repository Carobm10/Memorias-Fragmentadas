using UnityEngine;

public class Titulo : MonoBehaviour
{
    public float minY = 153f;     // límite derecha
    public float maxY = 198.5f;   // límite izquierda

    void Update()
    {
        if (Camera.main == null) return;

        Vector3 direccion = Camera.main.transform.position - transform.position;
        direccion.y = 0;

        if (direccion.sqrMagnitude < 0.001f) return;

        // Ángulo hacia la cámara
        float angulo = Mathf.Atan2(direccion.x, direccion.z) * Mathf.Rad2Deg;

        // Ajuste para que coincida con tu 180 inicial
        float anguloFinal = angulo + 180f;

        // Normalizar a 0–360
        if (anguloFinal < 0) anguloFinal += 360f;
        if (anguloFinal > 360) anguloFinal -= 360f;

        // Clamp dentro de TU rango real
        float anguloLimitado = Mathf.Clamp(anguloFinal, minY, maxY);

        transform.rotation = Quaternion.Euler(0, anguloLimitado, 0);
    }
}