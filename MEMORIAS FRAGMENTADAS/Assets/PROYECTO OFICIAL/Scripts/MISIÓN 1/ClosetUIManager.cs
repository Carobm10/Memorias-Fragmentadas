using UnityEngine;

public class ClosetUIManager : MonoBehaviour
{
    [Header("Canvas principal del uniforme")]
    public GameObject canvasUniformeUI;

    public bool uiAbierta = false;

    public void AbrirUI()
    {
        if (canvasUniformeUI != null)
        {
            canvasUniformeUI.SetActive(true);
            uiAbierta = true;
        }
    }

    public void CerrarUI()
    {
        if (canvasUniformeUI != null)
        {
            canvasUniformeUI.SetActive(false);
            uiAbierta = false;
        }
    }
}