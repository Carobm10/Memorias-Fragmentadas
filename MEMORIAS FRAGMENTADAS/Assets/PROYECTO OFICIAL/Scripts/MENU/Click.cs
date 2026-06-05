using UnityEngine;

public class Click : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // click izquierdo
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                // Detectar teclas de la máquina de escribir
                Tecla tecla = hit.collider.GetComponent<Tecla>();
                if (tecla == null)
                    tecla = hit.collider.GetComponentInParent<Tecla>();

                if (tecla != null)
                {
                    tecla.Presionar();
                    return;
                }

                // Detectar botones del panel de opciones
                BotonPanelOpciones opcion = hit.collider.GetComponent<BotonPanelOpciones>();
                if (opcion == null)
                    opcion = hit.collider.GetComponentInParent<BotonPanelOpciones>();

                if (opcion != null)
                {
                    opcion.Presionar();
                }
            }
        }
    }
}
