using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class UIButtonSpriteFeedback : MonoBehaviour
{
    public enum ButtonAction
    {
        X,
        Y,
        B,
        A
    }

    [Header("Imagen del botón")]
    public Image buttonImage;

    [Header("Sprites")]
    public Sprite normalSprite;
    public Sprite pressedSprite;

    [Header("Botón que activa esta imagen")]
    public ButtonAction actionKey = ButtonAction.X;

    [Header("Tiempo visible del cambio")]
    public float feedbackTime = 0.5f;

    [Header("Acción después del feedback")]
    public UnityEvent onPressed;

    private bool isRunning = false;

    void Awake()
    {
        if (buttonImage == null)
            buttonImage = GetComponent<Image>();

        if (buttonImage != null && normalSprite != null)
            buttonImage.sprite = normalSprite;
    }

    void Update()
    {
        if (isRunning) return;

        if (PressedConfiguredKey())
        {
            StartCoroutine(PressRoutine());
        }
    }

    bool PressedConfiguredKey()
    {
        switch (actionKey)
        {
            case ButtonAction.X:
                return InputManagerCustom.PressX();

            case ButtonAction.Y:
                return InputManagerCustom.PressY();

            case ButtonAction.B:
                return InputManagerCustom.PressB();

            case ButtonAction.A:
                return InputManagerCustom.PressA();
        }

        return false;
    }

    IEnumerator PressRoutine()
    {
        isRunning = true;

        if (buttonImage != null && pressedSprite != null)
            buttonImage.sprite = pressedSprite;

        yield return new WaitForSeconds(feedbackTime);

        if (buttonImage != null && normalSprite != null)
            buttonImage.sprite = normalSprite;

        onPressed.Invoke();

        isRunning = false;
    }
}