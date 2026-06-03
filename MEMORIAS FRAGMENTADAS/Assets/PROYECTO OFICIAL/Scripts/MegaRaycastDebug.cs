using UnityEngine;

public class MegaRaycastDebug : MonoBehaviour
{
    [Header("Configuración Raycast")]
    public float distancia = 5f;
    public LayerMask capasDetectables;
    public bool dibujarRayo = true;

    [Header("Objetos a vigilar")]
    public GameObject radioAbiertoConTresPilas;
    public GameObject primeraPilaAnimacion;
    public GameObject segundaPilaAnimacion;
    public GameObject terceraPilaAnimacion;
    public GameObject radioAbiertoSinPilas;
    public GameObject tapaCerrarMusica;

    [Header("Control de consola")]
    public bool mostrarCadaFrame = false;
    public KeyCode teclaDebugManual = KeyCode.F9;

    void Update()
    {
        if (dibujarRayo)
            Debug.DrawRay(transform.position, transform.forward * distancia, Color.red);

        if (mostrarCadaFrame || Input.GetKeyDown(teclaDebugManual))
        {
            RevisarRaycast();
            RevisarEstadosRadio();
        }
    }

    void RevisarRaycast()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.forward, out hit, distancia, capasDetectables))
        {
            GameObject obj = hit.collider.gameObject;

            RadioCoverTrigger tapa = hit.collider.GetComponentInParent<RadioCoverTrigger>();
            RadioAnimacionesSimple radio = hit.collider.GetComponentInParent<RadioAnimacionesSimple>();
            BatteryPickup pilas = hit.collider.GetComponentInParent<BatteryPickup>();
            DrawerInteractable drawer = hit.collider.GetComponentInParent<DrawerInteractable>();
            RadioBatteryInsertTrigger pilaInsertar = hit.collider.GetComponentInParent<RadioBatteryInsertTrigger>();

            Debug.Log(
                "\n========== MEGA RAYCAST DEBUG ==========" +
                "\nCOLLIDER TOCADO: " + hit.collider.name +
                "\nOBJETO TOCADO: " + obj.name +
                "\nPADRE: " + (obj.transform.parent != null ? obj.transform.parent.name : "SIN PADRE") +
                "\nLAYER: " + LayerMask.LayerToName(obj.layer) +
                "\nTAG: " + obj.tag +
                "\nPOSICIÓN HIT: " + hit.point +
                "\nDISTANCIA HIT: " + hit.distance +
                "\nTIENE RadioCoverTrigger: " + (tapa != null) +
                "\nTIENE RadioAnimacionesSimple: " + (radio != null) +
                "\nTIENE BatteryPickup: " + (pilas != null) +
                "\nTIENE DrawerInteractable: " + (drawer != null) +
                "\nTIENE RadioBatteryInsertTrigger: " + (pilaInsertar != null) +
                "\nCOLLIDER ENABLED: " + hit.collider.enabled +
                "\nCOLLIDER IS TRIGGER: " + hit.collider.isTrigger +
                "\n========================================"
            );
        }
        else
        {
            Debug.Log("MEGA RAYCAST DEBUG: No estoy tocando nada.");
        }
    }

    void RevisarEstadosRadio()
    {
        Debug.Log(
            "\n========== DEBUG ESTADOS RADIO ==========" +
            "\nRadio abierto con 3 pilas ACTIVO: " + EstadoObjeto(radioAbiertoConTresPilas) +
            "\nAnimación pila 1 ACTIVA: " + EstadoObjeto(primeraPilaAnimacion) +
            "\nAnimación pila 2 ACTIVA: " + EstadoObjeto(segundaPilaAnimacion) +
            "\nAnimación pila 3 ACTIVA: " + EstadoObjeto(terceraPilaAnimacion) +
            "\nRadio abierto sin pilas ACTIVO: " + EstadoObjeto(radioAbiertoSinPilas) +
            "\nTapita cerrar música ACTIVA: " + EstadoObjeto(tapaCerrarMusica) +
            "\n========================================="
        );
    }

    string EstadoObjeto(GameObject obj)
    {
        if (obj == null)
            return "NO ASIGNADO EN INSPECTOR";

        return obj.activeInHierarchy ? "SÍ" : "NO";
    }
}