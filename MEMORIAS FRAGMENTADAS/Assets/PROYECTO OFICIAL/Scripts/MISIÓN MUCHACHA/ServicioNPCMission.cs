using UnityEngine;

public class ServicioNPCMission : MonoBehaviour
{
    [Header("Misión")]
    public KitchenRadioMissionManager missionManager;

    [Header("Distancia")]
    public Transform player;
    public float interactionDistance = 3f;

    [Header("Debug")]
    public bool mostrarDebug = false;

    private bool playerLookingAtMe = false;
    private bool lastCerca = false;
    private bool lastMirando = false;

    void Update()
    {
        float distanciaActual = player != null
            ? Vector3.Distance(transform.position, player.position)
            : -1f;

        bool cerca = player != null && distanciaActual <= interactionDistance;

        // Solo loguear cuando cambia el estado, no cada frame
        if (mostrarDebug && (cerca != lastCerca || playerLookingAtMe != lastMirando))
        {
            Debug.Log(
                "ROSA DEBUG | Mirando: " + playerLookingAtMe +
                " | Cerca: " + cerca +
                " | Distancia: " + distanciaActual +
                " | MissionManager: " + (missionManager != null)
            );
            lastCerca = cerca;
            lastMirando = playerLookingAtMe;
        }

        if (missionManager == null) return;

        if (playerLookingAtMe && cerca)
        {
            missionManager.ShowMissionPrompt();

            if (InputManagerCustom.PressA())
            {
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
    }
}