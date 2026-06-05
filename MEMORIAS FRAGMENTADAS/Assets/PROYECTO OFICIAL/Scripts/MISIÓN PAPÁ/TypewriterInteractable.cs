using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement;

public class TypewriterInteractable : MonoBehaviour
{
    [Header("Salto automático de línea")]
    public float paperLineUpY = 0.015f;
    public float lineReturnSpeed = 0.3f;

    [Header("Movimiento del carro / papel")]
    public Transform paperSupport;
    public float paperStepX = -0.003f;
    public float paperMoveSpeed = 0.05f;

    [Header("UI instrucción")]
    public GameObject instructionCanvas;
    public float instructionTime = 4f;

    [Header("UI enviar")]
    public GameObject sendCanvas;
    public string finalSceneName = "FINAL";

    [Header("Detección de teclas")]
    public float keyRayDistance = 2f;
    public Color keyHighlightColor = new Color(0.1f, 1f, 0.25f, 1f);

    [Header("Animación de teclas")]
    public float keyPressDistance = 0.01f;
    public float keyPressSpeed = 0.05f;

    [Header("Sonidos")]
    public AudioSource audioSource;
    public AudioClip keyClickSound;

    [Header("UI salir")]
    public GameObject exitCanvas;

    [Header("UI inicial máquina")]
    public GameObject promptCanvas;
    public GameObject genericInteractPrompt;

    [Header("Cámara y jugador")]
    public Transform typewriterViewPoint;
    public Transform playerCamera;
    public MovimientoVR2 playerMovement;
    public float moveDuration = 1.2f;

    [Header("Carta")]
    public TextMeshProUGUI letterText;

    [TextArea(4, 8)]
    public string fullLetterText =
        "Querida tía María:\n\nTe invitamos a almorzar el jueves.\n\nCon cariño,\nJoselito";

    [Header("Estado")]
    public bool playerLookingAtMe = false;
    public bool isWritingMode = false;

    private int currentLetterIndex = 0;
    private bool ignoreNextB = false;
    private bool keepCameraLocked = false;
    private bool paperMoving = false;
    private bool letterFinished = false;

    private Vector3 originalCameraPosition;
    private Quaternion originalCameraRotation;
    private Vector3 originalPaperPosition;

    private TypewriterKey currentLookedKey;

    private void Start()
    {
        if (promptCanvas != null) promptCanvas.SetActive(false);
        if (instructionCanvas != null) instructionCanvas.SetActive(false);
        if (exitCanvas != null) exitCanvas.SetActive(false);
        if (sendCanvas != null) sendCanvas.SetActive(false);

        if (paperSupport != null)
            originalPaperPosition = paperSupport.localPosition;

        Debug.Log("TypewriterInteractable iniciado en: " + gameObject.name);
    }

    private void Update()
    {
        if (playerLookingAtMe && !isWritingMode)
        {
            if (Input.GetKeyDown(KeyCode.B) || Input.GetKeyDown(KeyCode.JoystickButton5))
                StartWritingMode();
        }

        if (!isWritingMode) return;

        DetectLookedKey();

        if (letterFinished)
        {
            if (Input.GetKeyDown(KeyCode.B) || Input.GetKeyDown(KeyCode.JoystickButton5))
                SendLetter();

            if (Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.JoystickButton2))
                ExitWritingMode();

            return;
        }

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
            ExitWritingMode();
    }

    private void LateUpdate()
    {
        if (keepCameraLocked && isWritingMode && playerCamera != null && typewriterViewPoint != null)
            playerCamera.position = typewriterViewPoint.position;
    }

    public void ShowPrompt()
    {
        if (isWritingMode)
        {
            HideAllEntryPrompts();
            return;
        }

        playerLookingAtMe = true;

        if (genericInteractPrompt != null)
            genericInteractPrompt.SetActive(false);

        if (promptCanvas != null)
            promptCanvas.SetActive(true);
    }

    public void HidePrompt()
    {
        if (isWritingMode) return;

        playerLookingAtMe = false;

        if (promptCanvas != null)
            promptCanvas.SetActive(false);
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
        letterFinished = false;
        currentLetterIndex = 0;

        HideAllEntryPrompts();

        if (instructionCanvas != null) instructionCanvas.SetActive(false);
        if (sendCanvas != null) sendCanvas.SetActive(false);
        if (exitCanvas != null) exitCanvas.SetActive(true);

        if (playerMovement != null)
            playerMovement.puedeMoverse = false;

        Selected selectedSystem = FindFirstObjectByType<Selected>();

        if (selectedSystem != null)
            selectedSystem.SendMessage("Deselect", SendMessageOptions.DontRequireReceiver);

        originalCameraPosition = playerCamera.position;
        originalCameraRotation = playerCamera.rotation;

        if (paperSupport != null)
            paperSupport.localPosition = originalPaperPosition;

        StartCoroutine(MoveCameraToTypewriter());
    }

    public void ExitWritingMode()
    {
        StartCoroutine(ReturnToPlayer());
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

        UpdateLetterVisual();

        if (instructionCanvas != null)
        {
            instructionCanvas.SetActive(true);
            StartCoroutine(HideInstructionAfterSeconds());
        }

        if (exitCanvas != null)
            exitCanvas.SetActive(true);

        Debug.Log("Cámara llegó al punto de escritura.");
    }

    private IEnumerator ReturnToPlayer()
    {
        keepCameraLocked = false;

        if (currentLookedKey != null)
        {
            currentLookedKey.SetHighlight(false);
            currentLookedKey = null;
        }

        if (instructionCanvas != null) instructionCanvas.SetActive(false);
        if (exitCanvas != null) exitCanvas.SetActive(false);
        if (sendCanvas != null) sendCanvas.SetActive(false);

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
        playerLookingAtMe = false;

        if (playerMovement != null)
            playerMovement.puedeMoverse = true;

        Debug.Log("Jugador volvió a posición normal.");
    }

    private void DetectLookedKey()
    {
        RaycastHit[] hits = Physics.RaycastAll(
            playerCamera.position,
            playerCamera.forward,
            keyRayDistance,
            ~0,
            QueryTriggerInteraction.Collide
        );

        TypewriterKey foundKey = null;

        foreach (RaycastHit hit in hits)
        {
            TypewriterKey key = hit.collider.GetComponentInParent<TypewriterKey>();

            if (key != null)
            {
                foundKey = key;
                break;
            }
        }

        if (foundKey != null)
        {
            if (currentLookedKey != foundKey)
            {
                if (currentLookedKey != null)
                    currentLookedKey.SetHighlight(false);

                currentLookedKey = foundKey;
                currentLookedKey.SetHighlight(true);
            }

            return;
        }

        if (currentLookedKey != null)
        {
            currentLookedKey.SetHighlight(false);
            currentLookedKey = null;
        }
    }

    private void WriteNextLetter()
    {
        if (currentLetterIndex >= fullLetterText.Length)
        {
            FinishLetter();
            return;
        }

        char currentChar = fullLetterText[currentLetterIndex];

        if (currentChar == ' ')
        {
            currentLetterIndex++;
            PlayKeySound();
            StartCoroutine(MovePaperOneStep());
            UpdateLetterVisual();
            CheckLetterFinished();
            return;
        }

        if (currentChar == '\n')
        {
            currentLetterIndex++;
            UpdateLetterVisual();
            StartCoroutine(ReturnCarriageAndMovePaperUp());
            CheckLetterFinished();
            return;
        }

        if (currentLookedKey == null)
        {
            Debug.Log("No estás mirando ninguna tecla.");
            return;
        }

        string expectedKey = GetExpectedKey();

        if (currentLookedKey.keyValue.ToUpper() != expectedKey)
        {
            Debug.Log("Tecla incorrecta. Esperada: " + expectedKey + " / Mirada: " + currentLookedKey.keyValue);
            return;
        }

        currentLetterIndex++;

        PlayKeySound();

        StartCoroutine(currentLookedKey.Press(keyPressDistance, keyPressSpeed));
        StartCoroutine(MovePaperOneStep());

        UpdateLetterVisual();
        CheckLetterFinished();
    }

    private void PlayKeySound()
    {
        if (audioSource != null && keyClickSound != null)
            audioSource.PlayOneShot(keyClickSound);
    }

    private IEnumerator MovePaperOneStep()
    {
        if (paperSupport == null || paperMoving)
            yield break;

        paperMoving = true;

        Vector3 startPos = paperSupport.localPosition;
        Vector3 endPos = startPos + new Vector3(paperStepX, 0f, 0f);

        float t = 0f;

        while (t < paperMoveSpeed)
        {
            t += Time.deltaTime;
            paperSupport.localPosition = Vector3.Lerp(startPos, endPos, t / paperMoveSpeed);
            yield return null;
        }

        paperSupport.localPosition = endPos;
        paperMoving = false;
    }

    private IEnumerator ReturnCarriageAndMovePaperUp()
    {
        if (paperSupport == null)
            yield break;

        paperMoving = true;

        Vector3 startPos = paperSupport.localPosition;

        Vector3 endPos = new Vector3(
            originalPaperPosition.x,
            startPos.y + paperLineUpY,
            startPos.z
        );

        float t = 0f;

        while (t < lineReturnSpeed)
        {
            t += Time.deltaTime;
            paperSupport.localPosition = Vector3.Lerp(startPos, endPos, t / lineReturnSpeed);
            yield return null;
        }

        paperSupport.localPosition = endPos;
        paperMoving = false;

        Debug.Log("Salto automático de línea: carro vuelve y hoja sube.");
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

    private void CheckLetterFinished()
    {
        if (currentLetterIndex >= fullLetterText.Length)
            FinishLetter();
    }

    private void FinishLetter()
    {
        if (letterFinished) return;

        letterFinished = true;

        if (currentLookedKey != null)
        {
            currentLookedKey.SetHighlight(false);
            currentLookedKey = null;
        }

        if (instructionCanvas != null)
            instructionCanvas.SetActive(false);

        if (sendCanvas != null)
            sendCanvas.SetActive(true);

        Debug.Log("Carta terminada. Mostrar botón enviar.");
    }

    private void SendLetter()
    {
        Debug.Log("Enviando carta. Cargando escena final: " + finalSceneName);

        if (sendCanvas != null)
            sendCanvas.SetActive(false);

        SceneManager.LoadScene(finalSceneName);
    }

    private IEnumerator HideInstructionAfterSeconds()
    {
        yield return new WaitForSeconds(instructionTime);

        if (instructionCanvas != null && isWritingMode && !letterFinished)
            instructionCanvas.SetActive(false);
    }

    private string GetExpectedKey()
    {
        if (currentLetterIndex >= fullLetterText.Length)
            return "";

        char expectedChar = fullLetterText[currentLetterIndex];

        string letra = expectedChar.ToString().ToUpper();

        letra = letra
            .Replace("Á", "A")
            .Replace("É", "E")
            .Replace("Í", "I")
            .Replace("Ó", "O")
            .Replace("Ú", "U")
            .Replace("Ñ", "N");

        return letra;
    }

    private void HideAllEntryPrompts()
    {
        if (promptCanvas != null)
            promptCanvas.SetActive(false);

        if (genericInteractPrompt != null)
            genericInteractPrompt.SetActive(false);

        GameObject[] todos = FindObjectsByType<GameObject>(FindObjectsSortMode.None);

        foreach (GameObject obj in todos)
        {
            if (obj.name == "Texto-Detectar" ||
                obj.name == "TextDetect" ||
                obj.name == "Canvas_Detectar")
            {
                obj.SetActive(false);
            }
        }
    }
}