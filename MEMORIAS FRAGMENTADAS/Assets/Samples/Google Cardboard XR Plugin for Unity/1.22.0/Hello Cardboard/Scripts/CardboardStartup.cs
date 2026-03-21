//-----------------------------------------------------------------------
// CardboardStartup FIXED (XR Initialization added)
//-----------------------------------------------------------------------

using Google.XR.Cardboard;
using UnityEngine;
using UnityEngine.XR.Management;
using System.Collections;

public class CardboardStartup : MonoBehaviour
{
    IEnumerator Start()
    {
        // 🔥 Inicializar XR correctamente
        yield return XRGeneralSettings.Instance.Manager.InitializeLoader();

        if (XRGeneralSettings.Instance.Manager.activeLoader == null)
        {
            Debug.LogError("XR Loader failed to initialize");
            yield break;
        }

        XRGeneralSettings.Instance.Manager.StartSubsystems();

        // Configuración de pantalla
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Screen.brightness = 1.0f;

        // Verificar parámetros del dispositivo
        if (!Api.HasDeviceParams())
        {
            Api.ScanDeviceParams();
        }
    }

    void Update()
    {
        // 🚨 Evita errores si XR aún no está listo
        if (!XRGeneralSettings.Instance.Manager.isInitializationComplete)
            return;

        if (Api.IsGearButtonPressed)
        {
            Api.ScanDeviceParams();
        }

        if (Api.IsCloseButtonPressed)
        {
            Application.Quit();
        }

        if (Api.IsTriggerHeldPressed)
        {
            Api.Recenter();
        }

        if (Api.HasNewDeviceParams())
        {
            Api.ReloadDeviceParams();
        }

        Api.UpdateScreenParams();
    }

    void OnDisable()
    {
        // 🔻 Detener XR cuando se desactiva
        if (XRGeneralSettings.Instance.Manager.isInitializationComplete)
        {
            XRGeneralSettings.Instance.Manager.StopSubsystems();
            XRGeneralSettings.Instance.Manager.DeinitializeLoader();
        }
    }
}