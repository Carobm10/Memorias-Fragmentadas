using System.Collections;
using UnityEngine;

public class ControlHUDFeedback : MonoBehaviour
{
    [Header("Luces del HUD")]
    [SerializeField] private GameObject lightX;
    [SerializeField] private GameObject lightY;
    [SerializeField] private GameObject lightA;
    [SerializeField] private GameObject lightB;

    [Header("Configuración")]
    [SerializeField] private float tiempoEncendido = 0.18f;

    private Coroutine coroutineX;
    private Coroutine coroutineY;
    private Coroutine coroutineA;
    private Coroutine coroutineB;

    private void Start()
    {
        ApagarTodasLasLuces();
    }

    private void Update()
    {
        if (InputManagerCustom.PressX())
        {
            EncenderLuz(ref coroutineX, lightX);
        }

        if (InputManagerCustom.PressY())
        {
            EncenderLuz(ref coroutineY, lightY);
        }

        if (InputManagerCustom.PressA())
        {
            EncenderLuz(ref coroutineA, lightA);
        }

        if (InputManagerCustom.PressB())
        {
            EncenderLuz(ref coroutineB, lightB);
        }
    }

    private void EncenderLuz(ref Coroutine coroutineActual, GameObject luz)
    {
        if (luz == null)
        {
            Debug.LogWarning("ControlHUDFeedback: falta asignar una luz en el Inspector.");
            return;
        }

        if (coroutineActual != null)
        {
            StopCoroutine(coroutineActual);
        }

        coroutineActual = StartCoroutine(ParpadearLuz(luz));
    }

    private IEnumerator ParpadearLuz(GameObject luz)
    {
        luz.SetActive(true);

        yield return new WaitForSeconds(tiempoEncendido);

        luz.SetActive(false);
    }

    private void ApagarTodasLasLuces()
    {
        if (lightX != null) lightX.SetActive(false);
        if (lightY != null) lightY.SetActive(false);
        if (lightA != null) lightA.SetActive(false);
        if (lightB != null) lightB.SetActive(false);
    }
}