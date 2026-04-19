using UnityEngine;

public class Selected : MonoBehaviour
{
    LayerMask mask;
    public float distancia =1.5f; 
    void Start()
    {
        //Rayo imaginario que se lanza del punto A al punto B, choca con colaiders y se obtiene información 
        // Estructura base para declarar: Raycast(origen(De dónde se dispara), dirección, out hit(Lo que almacena), distancia, máscara)
        mask= LayerMask.GetMask("Raycast Detect"); 
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;

        if(Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, distancia, mask))
        {
            if (hit.collider.tag == "Objeto Interactivo")
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    //hit.collider.transform.GetComponent<SCRIPT>().FUNCION();
                    hit.collider.transform.GetComponent<ObjetoInteractivo>().ActivarObjeto();

                }
            }
        }
    }
}
