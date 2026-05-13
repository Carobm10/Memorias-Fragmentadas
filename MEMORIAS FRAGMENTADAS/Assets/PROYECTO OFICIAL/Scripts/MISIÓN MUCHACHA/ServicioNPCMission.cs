using UnityEngine;

public class ServicioNPCMission : MonoBehaviour
{
    [Header("Misión")]
    public KitchenRadioMissionManager missionManager;

    [Header("Distancia")]
    public Transform player;
    public float interactionDistance = 3f;

    [Header("Debug")]
    public bool mostrarDebug = true;

    private bool playerLookingAtMe = false;

    void Update()
    {
        float distanciaActual = player != null
            ? Vector3.Distance(transform.position, player.position)
            : -1f;

        bool cerca = player != null && distanciaActual <= interactionDistance;

        if (mostrarDebug)
        {
            Debug.Log(
                "ROSA DEBUG | Mirando: " + playerLookingAtMe +
                " | Cerca: " + cerca +
                " | Distancia: " + distanciaActual +
                " | MissionManager: " + (missionManager != null)
            );
        }

        if (missionManager == null)
        {
            Debug.LogError("ROSA ERROR: No está asignado KitchenRadioMissionManager.");
            return;
        }

        if (playerLookingAtMe && cerca)
        {
            Debug.Log("ROSA OK: Se cumple condición para mostrar prompt.");
            missionManager.ShowMissionPrompt();

            if (InputManagerCustom.PressA())
            {
                Debug.Log("ROSA OK: Presionaste A.");
                missionManager.TryStartMission();
            }
        }
        else
        {
            missionManager.HideMissionPrompt();
        }
    }

    public void SetLookingAtMe(bool value)
    {
        playerLookingAtMe = value;
        Debug.Log("ROSA RAYCAST: SetLookingAtMe = " + value);
    }
}