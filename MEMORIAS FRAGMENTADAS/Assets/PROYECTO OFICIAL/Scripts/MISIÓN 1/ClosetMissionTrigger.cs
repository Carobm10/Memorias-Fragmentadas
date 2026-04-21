using System.Collections;
using UnityEngine;

public class ClosetMissionTrigger : MonoBehaviour
{
    [Header("Puertas del clóset")]
    public SystemDoor leftDoor;
    public SystemDoor rightDoor;

    [Header("Canvas de misión")]
    public GameObject missionCanvas;

    [Header("Movimiento del jugador")]
    public MonoBehaviour playerMovementScript;

    [Header("Configuración")]
    public float delayBeforeMission = 0.6f;

    private bool missionStarted = false;

    public void StartClosetMission()
    {
        if (missionStarted) return;

        missionStarted = true;
        StartCoroutine(OpenClosetSequence());
    }

    IEnumerator OpenClosetSequence()
    {
        if (leftDoor != null)
            leftDoor.OpenDoor();

        if (rightDoor != null)
            rightDoor.OpenDoor();

        yield return new WaitForSeconds(delayBeforeMission);

        if (playerMovementScript != null)
            playerMovementScript.enabled = false;

        if (missionCanvas != null)
            missionCanvas.SetActive(true);
    }
}