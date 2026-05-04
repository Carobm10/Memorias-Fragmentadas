using UnityEngine;

public class TecladoRaycastInteractor : MonoBehaviour
{
    public float distanciaRaycast = 10f;
    public LayerMask layerTeclas;

    private Tecla teclaActual;

    void Update()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, distanciaRaycast, layerTeclas))
        {
            Tecla teclaDetectada = hit.collider.GetComponentInParent<Tecla>();

            if (teclaDetectada != null)
            {
                if (teclaActual != teclaDetectada)
                {
                    if (teclaActual != null)
                        teclaActual.Deseleccionar();

                    teclaActual = teclaDetectada;
                    teclaActual.Seleccionar();
                }

                if (InputManagerCustom.PressB())
                {
                    teclaActual.Presionar();
                }

                return;
            }
        }

        if (teclaActual != null)
        {
            teclaActual.Deseleccionar();
            teclaActual = null;
        }
    }
}