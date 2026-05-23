using UnityEngine;

public class TestCambioRadioAPrimeraPila : MonoBehaviour
{
    [Header("Estado inicial")]
    public GameObject radioAbierto3Pilas;

    [Header("Animación primera pila")]
    public GameObject primeraPilaObjeto;
    public Animator animatorPrimeraPila;
    public string estadoPrimeraPila = "PrimeraPila";

    [Header("Tecla prueba")]
    public KeyCode teclaPrueba = KeyCode.P;

    void Start()
    {
        if (radioAbierto3Pilas != null)
            radioAbierto3Pilas.SetActive(true);

        if (primeraPilaObjeto != null)
            primeraPilaObjeto.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(teclaPrueba))
            ProbarPrimeraPila();
    }

    public void ProbarPrimeraPila()
    {
        Debug.Log("========== TEST CAMBIO A PRIMERA PILA ==========");

        if (radioAbierto3Pilas != null)
        {
            radioAbierto3Pilas.SetActive(false);
            Debug.Log("Apagué radio_abierto_3_pilas");
        }

        if (primeraPilaObjeto == null)
        {
            Debug.LogError("No asignaste primeraPilaObjeto.");
            return;
        }

        primeraPilaObjeto.SetActive(true);
        Debug.Log("Activé objeto: " + primeraPilaObjeto.name);

        if (animatorPrimeraPila == null)
            animatorPrimeraPila = primeraPilaObjeto.GetComponent<Animator>();

        if (animatorPrimeraPila == null)
        {
            Debug.LogError("primera_pila no tiene Animator.");
            return;
        }

        Debug.Log("Controller: " + 
            (animatorPrimeraPila.runtimeAnimatorController != null 
            ? animatorPrimeraPila.runtimeAnimatorController.name 
            : "SIN CONTROLLER"));

        bool tieneEstado = animatorPrimeraPila.HasState(0, Animator.StringToHash(estadoPrimeraPila));
        Debug.Log("Tiene estado " + estadoPrimeraPila + ": " + tieneEstado);

        if (!tieneEstado)
            return;

        animatorPrimeraPila.enabled = true;
        animatorPrimeraPila.speed = 1f;
        animatorPrimeraPila.Rebind();
        animatorPrimeraPila.Update(0f);
        animatorPrimeraPila.Play(estadoPrimeraPila, 0, 0f);

        Debug.Log("Play ejecutado: " + estadoPrimeraPila);
    }
}