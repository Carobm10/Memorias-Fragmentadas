using System.Collections.Generic;
using UnityEngine;

public class MissionManager : MonoBehaviour
{
    public static MissionManager Instance;

    public enum MissionState
    {
        NotStarted,
        Active,
        Completed
    }

    [Header("Debug")]
    public bool mostrarDebug = true;

    private Dictionary<string, MissionState> missions = new Dictionary<string, MissionState>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        RegistrarMisionesIniciales();
    }

    private void RegistrarMisionesIniciales()
    {
        RegisterMission("periodico");
        RegisterMission("radio");
        RegisterMission("carta");
        RegisterMission("uniforme");
    }

    public void RegisterMission(string missionID)
    {
        if (!missions.ContainsKey(missionID))
        {
            missions.Add(missionID, MissionState.NotStarted);

            if (mostrarDebug)
                Debug.Log("[MissionManager] Misión registrada: " + missionID);
        }
    }

    public void StartMission(string missionID)
    {
        RegisterMission(missionID);

        if (missions[missionID] == MissionState.Completed)
        {
            if (mostrarDebug)
                Debug.Log("[MissionManager] La misión ya estaba completada: " + missionID);

            return;
        }

        missions[missionID] = MissionState.Active;

        if (mostrarDebug)
            Debug.Log("[MissionManager] Misión iniciada: " + missionID);
    }

    public void CompleteMission(string missionID)
    {
        RegisterMission(missionID);

        missions[missionID] = MissionState.Completed;

        if (mostrarDebug)
            Debug.Log("[MissionManager] Misión completada: " + missionID);
    }

    public bool IsMissionActive(string missionID)
    {
        return missions.ContainsKey(missionID) &&
               missions[missionID] == MissionState.Active;
    }

    public bool IsMissionCompleted(string missionID)
    {
        return missions.ContainsKey(missionID) &&
               missions[missionID] == MissionState.Completed;
    }

    public bool IsMissionNotStarted(string missionID)
    {
        return !missions.ContainsKey(missionID) ||
               missions[missionID] == MissionState.NotStarted;
    }

    public MissionState GetMissionState(string missionID)
    {
        RegisterMission(missionID);
        return missions[missionID];
    }

    [ContextMenu("DEBUG - Iniciar misión periódico")]
    public void DebugStartPeriodico()
    {
        StartMission("periodico");
    }

    [ContextMenu("DEBUG - Completar misión periódico")]
    public void DebugCompletePeriodico()
    {
        CompleteMission("periodico");
    }
}