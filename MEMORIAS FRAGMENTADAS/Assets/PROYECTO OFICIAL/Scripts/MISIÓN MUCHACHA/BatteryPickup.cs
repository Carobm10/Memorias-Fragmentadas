using UnityEngine;
using TMPro;
using System.Collections;

public class BatteryPickup : MonoBehaviour
{
    [Header("Radio")]
    public RadioAnimacionesSimple radio;

    [Header("Cajón")]
    public DrawerInteractable drawer;

    [Header("Prompt")]
    public GameObject promptPanel;
    public TMP_Text promptText;

    [Header("Color")]
    public Color colorDisponible = new Color(0.1f, 1f, 0.25f, 1f);

    private bool mirando = false;
    private bool tomadas = false;

    private Renderer[] renderers;
    private Collider[] colliders;

    void Start()
    {
        renderers = GetComponentsInChildren<Renderer>();
        colliders = GetComponentsInChildren<Collider>();

        foreach (Renderer r in renderers)
        {
            if (r != null)
                r.material.color = colorDisponible;
        }

        OcultarPrompt();
    }

    void Update()
    {
        if (tomadas) return;

        if (mirando)
        {
            MostrarPrompt("Presiona B para recoger las pilas");

            if (InputManagerCustom.PressB())
            {
                StartCoroutine(TomarPilasRoutine());
            }
        }
    }

    IEnumerator TomarPilasRoutine()
    {
        tomadas = true;
        mirando = false;

        foreach (Renderer r in renderers)
        {
            if (r != null)
                r.enabled = false;
        }

        foreach (Collider c in colliders)
        {
            if (c != null)
                c.enabled = false;
        }

        OcultarPrompt();

        if (drawer != null)
            drawer.ToggleDrawer();

        yield return new WaitForSeconds(5f);

        if (radio != null)
            radio.ActivarModoInsertarPilas();
    }

    public void LookAtBatteries()
    {
        if (tomadas) return;

        mirando = true;
    }

    public void StopLookingAtBatteries()
    {
        mirando = false;

        if (!tomadas)
            OcultarPrompt();
    }

    public bool YaFueronTomadas()
    {
        return tomadas;
    }

    void MostrarPrompt(string texto)
    {
        if (promptPanel != null)
            promptPanel.SetActive(true);

        if (promptText != null)
            promptText.text = texto;
    }

    void OcultarPrompt()
    {
        if (promptPanel != null)
            promptPanel.SetActive(false);
    }
}