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
                Tecla tecla = hit.collider.GetComponent<Tecla>();

                if (tecla != null)
                {
                    tecla.Presionar();
                }
            }
        }
    }
}
