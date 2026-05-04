using UnityEngine;

public class MuchachaMissionTrigger : MonoBehaviour
{
    [Header("Manager de misión")]
    public MuchachaMissionManager missionManager;

    public void ActivarConA()
    {
        if (missionManager == null) return;

        if (!missionManager.misionIniciada)
        {
            missionManager.ActivarMision();
        }
        else if (missionManager.dialogoActivo)
        {
            missionManager.AvanzarDialogo();
        }
    }

    public void CerrarConX()
    {
        if (missionManager == null) return;

        missionManager.CerrarDialogo();
    }
}