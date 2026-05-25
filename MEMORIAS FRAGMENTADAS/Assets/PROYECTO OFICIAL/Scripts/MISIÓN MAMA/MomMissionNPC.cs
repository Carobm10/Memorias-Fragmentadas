using UnityEngine;

public class MomMissionNPC : MonoBehaviour
{
    [Header("ID de misión")]
    public string missionID = "periodico";

    [Header("Debug")]
    public bool mostrarDebug = true;

    public void Interactuar()
    {
        if (MissionManager.Instance.IsMissionNotStarted(missionID))
        {
            DialogoInicio();
            MissionManager.Instance.StartMission(missionID);
            return;
        }

        if (MissionManager.Instance.IsMissionActive(missionID))
        {
            DialogoMisionActiva();
            return;
        }

        if (MissionManager.Instance.IsMissionCompleted(missionID))
        {
            DialogoPostMision();
            return;
        }
    }

    void DialogoInicio()
    {
        string[] lineas = new string[]
        {
            "¡Ay, llegó el del periódico!",
            "Mijo, tráigame por favor el monedero que lo dejé por ahí.",
            "¡Rápido!, antes de que se vaya el señor."
        };

        NPCDialogueManager.Instance.MostrarDialogoSimple("Mamá", lineas);

        if (mostrarDebug)
            Debug.Log("[MomMissionNPC] Misión periódico iniciada.");
    }

    void DialogoMisionActiva()
    {
        string[] lineas = new string[]
        {
            "¡Rápido mijo!",
            "El monedero debe estar cerca de la mesita del televisor."
        };

        NPCDialogueManager.Instance.MostrarDialogoSimple("Mamá", lineas);
    }

    void DialogoPostMision()
    {
        NPCDialogueManager.Instance.MostrarDialogoConOpciones(
            "Mamá",
            "Gracias mijo. ¿Sí alcanzó al señor del periódico?",
            "Sí mamá, aquí está.",
            "Casi se me va.",
            "¿Siempre compra el periódico?",
            RespuestaSiAlcanzo,
            RespuestaCasiSeVa,
            RespuestaSiempreCompra
        );
    }

    void RespuestaSiAlcanzo()
    {
        string[] lineas = new string[]
        {
            "Ay, qué juicioso.",
            "Su papá siempre lee el periódico después del café."
        };

        NPCDialogueManager.Instance.MostrarDialogoSimple("Mamá", lineas);
    }

    void RespuestaCasiSeVa()
    {
        string[] lineas = new string[]
        {
            "Eso pensé, mijo.",
            "Ese señor pasa rápido porque todavía le falta toda la cuadra."
        };

        NPCDialogueManager.Instance.MostrarDialogoSimple("Mamá", lineas);
    }

    void RespuestaSiempreCompra()
    {
        string[] lineas = new string[]
        {
            "Sí, todos los días.",
            "Así nos enteramos de lo que pasa en el país y en el barrio."
        };

        NPCDialogueManager.Instance.MostrarDialogoSimple("Mamá", lineas);
    }
}