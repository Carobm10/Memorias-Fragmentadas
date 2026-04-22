using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class ClosetUIManager : MonoBehaviour
{
    [Header("Canvas principal")]
    public GameObject canvasUniformeUI;

    [Header("Imágenes")]
    public Image characterImage;
    public Image clothingImage;

    [Header("Sprites personaje")]
    public Sprite[] characterSprites;

    [Header("Sprites prenda")]
    public Sprite[] clothingSprites;

    [Header("Texto feedback")]
    public TMP_Text feedbackText;

    [Header("Indicadores visuales")]
    public GameObject btnProbarOtra;
    public GameObject btnSalir;

    [Header("Control")]
    public bool uiAbierta = false;
    public float autoCloseDelay = 1.5f;

    private ClosetMissionTrigger currentClosetMission;
    private bool currentChoiceIsCorrect = false;

    public void AbrirUI(int clothingIndex, bool isCorrect, ClosetMissionTrigger closetMission)
    {
        currentClosetMission = closetMission;
        currentChoiceIsCorrect = isCorrect;

        if (canvasUniformeUI != null)
            canvasUniformeUI.SetActive(true);

        uiAbierta = true;

        if (characterImage != null && clothingIndex >= 0 && clothingIndex < characterSprites.Length)
            characterImage.sprite = characterSprites[clothingIndex];

        if (clothingImage != null && clothingIndex >= 0 && clothingIndex < clothingSprites.Length)
            clothingImage.sprite = clothingSprites[clothingIndex];

        if (feedbackText != null)
        {
            feedbackText.text = isCorrect
                ? "¡Este es el uniforme correcto!"
                : "¡Has escogido la prenda incorrecta, escoge otra!";
        }

        if (btnProbarOtra != null)
            btnProbarOtra.SetActive(!isCorrect);

        if (btnSalir != null)
            btnSalir.SetActive(!isCorrect);

        if (isCorrect)
        {
            StartCoroutine(CerrarAutomaticamenteYVolver());
        }
    }

    void Update()
    {
        if (!uiAbierta)
            return;

        if (!currentChoiceIsCorrect)
        {
            if (InputManagerCustom.PressY())
            {
                ProbarOtra();
                return;
            }

            if (InputManagerCustom.PressX())
            {
                CerrarUI();
                return;
            }
        }
    }

    IEnumerator CerrarAutomaticamenteYVolver()
    {
        yield return new WaitForSeconds(autoCloseDelay);

        CerrarUI();

        if (currentClosetMission != null)
        {
            currentClosetMission.ReturnPlayerToOriginalPosition();
        }
    }

    public void CerrarUI()
    {
        if (canvasUniformeUI != null)
            canvasUniformeUI.SetActive(false);

        uiAbierta = false;
    }

    public void ProbarOtra()
    {
        CerrarUI();

        if (currentClosetMission != null)
        {
            currentClosetMission.ReactivarSeleccionSinMoverJugador();
        }
    }
}