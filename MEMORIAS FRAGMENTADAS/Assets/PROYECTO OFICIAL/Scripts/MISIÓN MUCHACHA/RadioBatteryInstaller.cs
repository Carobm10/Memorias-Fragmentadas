using UnityEngine;
using System.Collections;

public class RadioBatteryInstaller : MonoBehaviour
{
    [Header("Misión")]
    public KitchenRadioMissionManager missionManager;

    [Header("Movimiento de pila")]
    public Transform targetSlot;
    public float moveSpeed = 3f;

    [Header("Prompt")]
    public GameObject promptPanel;
    public TMPro.TMP_Text promptText;

    [Header("Estado")]
    public bool installed = false;

    private bool isLooking = false;
    private bool isMoving = false;

    void Start()
    {
        HidePrompt();
    }

    void Update()
    {
        if (!isLooking) return;
        if (installed || isMoving) return;

        if (!missionManager.PlayerHasBatteries()) return;

        if (InputManagerCustom.PressB())
        {
            missionManager.StartInstallingBatteries();
            StartCoroutine(InstallBatteryRoutine());
        }
    }

    public void LookAtBattery()
    {
        if (!missionManager.PlayerHasBatteries()) return;
        if (installed) return;

        isLooking = true;
        ShowPrompt("Presiona B para poner la pila");
    }

    public void StopLookingAtBattery()
    {
        isLooking = false;
        HidePrompt();
    }

    IEnumerator InstallBatteryRoutine()
    {
        isMoving = true;
        HidePrompt();

        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;

        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * moveSpeed;

            transform.position = Vector3.Lerp(startPos, targetSlot.position, t);
            transform.rotation = Quaternion.Lerp(startRot, targetSlot.rotation, t);

            yield return null;
        }

        transform.position = targetSlot.position;
        transform.rotation = targetSlot.rotation;

        installed = true;
        isMoving = false;

        CheckAllBatteriesInstalled();
    }

    void CheckAllBatteriesInstalled()
    {
        RadioBatteryInstaller[] allBatteries = FindObjectsOfType<RadioBatteryInstaller>();

        foreach (RadioBatteryInstaller battery in allBatteries)
        {
            if (!battery.installed)
                return;
        }

        missionManager.BatteriesInstalled();
    }

    void ShowPrompt(string message)
    {
        if (promptPanel != null)
            promptPanel.SetActive(true);

        if (promptText != null)
            promptText.text = message;
    }

    void HidePrompt()
    {
        if (promptPanel != null)
            promptPanel.SetActive(false);
    }
}