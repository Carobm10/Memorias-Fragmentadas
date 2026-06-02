using UnityEngine;
using System.Collections;
using TMPro;

public class TypewriterInteractable : MonoBehaviour
{
    [Header("Tecla de prueba")] 
    public Transform testKey;
    public float keyPressDistance = 0.01f;
    public float keyPressSpeed = 0.05f;
    private bool ignoreNextB = false;
    private Vector3 originalKeyPosition;
    private bool keyAnimating = false;

    [Header("Sonidos")]
    public AudioSource audioSource;
    public AudioClip keyClickSound;

    [Header("UI salir")]
    public GameObject exitCanvas;

    [Header("UI propia de la máquina")]
    public GameObject promptCanvas;

    [Header("UI genérica que debe apagarse")]
    public GameObject genericInteractPrompt;

    [Header("Punto de vista de escritura")]
    public Transform typewriterViewPoint;

    [Header("Cámara del jugador")]
    public Transform playerCamera;

    [Header("Movimiento del jugador")]
    public MovimientoVR2 playerMovement;

    [Header("Configuración de cámara")]
    public float moveDuration = 1.2f;
    [Header("Carta")]
    public TextMeshProUGUI letterText;
    [TextArea(4, 8)]
    public string fullLetterText = "Querida tía María:\n\nTe invitamos a almorzar el jueves.\n\nCon cariño,\nJoselito";

    private int currentLetterIndex = 0;

    [Header("Estado")]
    public bool playerLookingAtMe = false;
    public bool isWritingMode = false;
    private bool keepCameraLocked = false;

    private Vector3 originalCameraPosition;
    private Quaternion originalCameraRotation;

    private void Start()
    {
        if (promptCanvas != null)
        {
            promptCanvas.SetActive(false);
        }

        Debug.Log("TypewriterInteractable iniciado en: " + gameObject.name);

        if (testKey != null)
        {
            originalKeyPosition = testKey.localPosition;
        }

        if (exitCanvas != null)
        {
            exitCanvas.SetActive(false);
        }
    }

    private void Update()
    {
        if (playerLookingAtMe && !isWritingMode)
        {
            if (Input.GetKeyDown(KeyCode.B) || Input.GetKeyDown(KeyCode.JoystickButton5))
            {
                StartWritingMode();
            }
        }

        if (isWritingMode)
        {
            if (Input.GetKeyDown(KeyCode.B) || Input.GetKeyDown(KeyCode.JoystickButton5))
            {
                if (ignoreNextB)
                {
                    ignoreNextB = false;
                    Debug.Log("Se ignoró la primera B usada para entrar a la máquina.");
                }
                else
                {
                    WriteNextLetter();
                }
            }

            if (Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.JoystickButton2))
            {
                ExitWritingMode();
            }
        }
    }

    public void ShowPrompt()
    {
        playerLookingAtMe = true;

        if (genericInteractPrompt != null)
        {
            genericInteractPrompt.SetActive(false);
        }

        if (promptCanvas != null)
        {
            promptCanvas.SetActive(true);
        }

        Debug.Log("Mirando máquina de escribir: mostrar prompt propio y apagar prompt genérico");
    }

    public void HidePrompt()
    {
        if (isWritingMode) return;

        playerLookingAtMe = false;

        if (promptCanvas != null)
        {
            promptCanvas.SetActive(false);
        }

        Debug.Log("Dejó de mirar máquina de escribir: ocultar prompt propio");
    }

    public void StartWritingMode()
    {
        if (typewriterViewPoint == null)
        {
            Debug.LogError("Falta asignar TypewriterViewPoint en el Inspector.");
            return;
        }

        if (playerCamera == null)
        {
            Debug.LogError("Falta asignar Player Camera en el Inspector.");
            return;
        }

        isWritingMode = true;
        ignoreNextB = true;
        Selected selectedSystem = FindObjectOfType<Selected>();

        if (selectedSystem != null)
        {
            selectedSystem.SendMessage("Deselect", SendMessageOptions.DontRequireReceiver);
            Debug.Log("Se apagó el highlight general de la máquina al entrar en modo escritura.");
        }

        if (promptCanvas != null)
        {
            promptCanvas.SetActive(false);
        }

        if (exitCanvas != null)
        {
            exitCanvas.SetActive(true);
        }

        if (genericInteractPrompt != null)
        {
            genericInteractPrompt.SetActive(false);
        }

        if (playerMovement != null)
        {
            playerMovement.puedeMoverse = false;
        }

        originalCameraPosition = playerCamera.position;
        originalCameraRotation = playerCamera.rotation;

        Debug.Log("Presionaste B: moviendo cámara hacia la máquina de escribir");

        StartCoroutine(MoveCameraToTypewriter());
    }

    private IEnumerator MoveCameraToTypewriter()
    {
        float elapsedTime = 0f;

        Vector3 startPosition = playerCamera.position;
        Quaternion startRotation = playerCamera.rotation;

        while (elapsedTime < moveDuration)
        {
            elapsedTime += Time.deltaTime;

            float t = elapsedTime / moveDuration;

            playerCamera.position = Vector3.Lerp(startPosition, typewriterViewPoint.position, t);
            playerCamera.rotation = Quaternion.Slerp(startRotation, typewriterViewPoint.rotation, t);

            yield return null;
        }

        playerCamera.position = typewriterViewPoint.position;
        playerCamera.rotation = typewriterViewPoint.rotation;

        keepCameraLocked = true;
        
        currentLetterIndex = 0;
        UpdateLetterVisual();

        Debug.Log("Cámara llegó al punto de escritura: posición bloqueada, mirada libre.");
    }

    private IEnumerator ReturnToPlayer()
    {
        keepCameraLocked = false;

        float elapsedTime = 0f;

        Vector3 startPosition = playerCamera.position;
        Quaternion startRotation = playerCamera.rotation;

        while (elapsedTime < moveDuration)
        {
            elapsedTime += Time.deltaTime;

            float t = elapsedTime / moveDuration;

            playerCamera.position = Vector3.Lerp(startPosition, originalCameraPosition, t);
            playerCamera.rotation = Quaternion.Slerp(startRotation, originalCameraRotation, t);

            yield return null;
        }

        playerCamera.position = originalCameraPosition;
        playerCamera.rotation = originalCameraRotation;

        isWritingMode = false;

        if (playerMovement != null)
        {
            playerMovement.puedeMoverse = true;
        }

        if (exitCanvas != null)
        {
            exitCanvas.SetActive(false);
        }

        Debug.Log("Jugador volvió a posición normal.");
    }

    private IEnumerator AnimateKey()
    {
        if (testKey == null)
            yield break;

        if (keyAnimating)
            yield break;

        keyAnimating = true;

        Vector3 pressedPosition =
            originalKeyPosition +
            Vector3.down * keyPressDistance;

        float t = 0f;

        while (t < keyPressSpeed)
        {
            t += Time.deltaTime;

            testKey.localPosition =
                Vector3.Lerp(
                    originalKeyPosition,
                    pressedPosition,
                    t / keyPressSpeed);

            yield return null;
        }

        t = 0f;

        while (t < keyPressSpeed)
        {
            t += Time.deltaTime;

            testKey.localPosition =
                Vector3.Lerp(
                    pressedPosition,
                    originalKeyPosition,
                    t / keyPressSpeed);

            yield return null;
        }

        testKey.localPosition = originalKeyPosition;

        keyAnimating = false;
    }
 
    private void LateUpdate()
    {
        if (keepCameraLocked && isWritingMode && playerCamera != null && typewriterViewPoint != null)
        {
            playerCamera.position = typewriterViewPoint.position;
        }
    }
    private void UpdateLetterVisual()
    {
        if (letterText == null)
        {
            Debug.LogWarning("Falta asignar Letter Text en el Inspector.");
            return;
        }

        string writtenPart = fullLetterText.Substring(0, currentLetterIndex);
        string pendingPart = fullLetterText.Substring(currentLetterIndex);

        letterText.text =
            "<color=#000000>" + writtenPart + "</color>" +
            "<color=#9B9B9B>" + pendingPart + "</color>";
    }

    public void ExitWritingMode()
    {
        Debug.Log("Saliendo de la máquina de escribir");

        StartCoroutine(ReturnToPlayer());
    }

    private void WriteNextLetter()
    {
        if (currentLetterIndex >= fullLetterText.Length)
        {
            Debug.Log("La carta ya está completa.");
            return;
        }

        currentLetterIndex++;

        if (audioSource != null && keyClickSound != null)
        {
            audioSource.PlayOneShot(keyClickSound);
            StartCoroutine(AnimateKey());
        }

        UpdateLetterVisual();

        Debug.Log("Letra escrita: " + currentLetterIndex + " / " + fullLetterText.Length);
    }
}
