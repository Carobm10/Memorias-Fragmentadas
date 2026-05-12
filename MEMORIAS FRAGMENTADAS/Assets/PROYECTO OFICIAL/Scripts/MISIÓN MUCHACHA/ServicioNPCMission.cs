using UnityEngine;

public class ServicioNPCMission : MonoBehaviour
{
    [Header("Misión")]
    public KitchenRadioMissionManager missionManager;

    [Header("Distancia")]
    public Transform player;
    public float interactionDistance = 3f;

    private bool playerLookingAtMe = false;

    void Update()
    {
        if (playerLookingAtMe && IsPlayerNear())
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

    bool IsPlayerNear()
    {
        if (player == null) return false;

        float distance = Vector3.Distance(transform.position, player.position);
        return distance <= interactionDistance;
    }
}