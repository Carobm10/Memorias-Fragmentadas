using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Animador visual de la pantalla de carga.
/// </summary>
public class LoadingScreenAnimator : MonoBehaviour
{
    private Text loadingText;
    private Image progressBar;

    private float currentProgress = 0f;
    private float targetProgress = 0.1f;  // Arranca visible desde el principio
    private bool isComplete = false;

    public void Initialize(Text textComponent, Image progressBarImage)
    {
        loadingText = textComponent;
        progressBar = progressBarImage;
        currentProgress = 0f;
        targetProgress = 0.1f;
        isComplete = false;

        if (progressBar != null)
            progressBar.fillAmount = 0f;

        StartCoroutine(AnimateLoadingText());
        StartCoroutine(AnimateProgressBar());
    }

    public void SetProgress(float progress)
    {
        if (!isComplete)
            targetProgress = Mathf.Clamp01(progress);
    }

    public void Complete()
    {
        isComplete = true;
        targetProgress = 1f;
        currentProgress = 1f;

        if (progressBar != null)
            progressBar.fillAmount = 1f;

        if (loadingText != null)
            loadingText.text = "¡Listo!";
    }

    private IEnumerator AnimateLoadingText()
    {
        string[] frames = { "Cargando.", "Cargando..", "Cargando..." };
        int i = 0;

        while (!isComplete)
        {
            if (loadingText != null)
            {
                loadingText.text = frames[i];
                i = (i + 1) % frames.Length;
            }
            yield return new WaitForSecondsRealtime(0.4f);
        }

        // El texto "¡Listo!" lo pone Complete(), no hace falta repetirlo aquí
    }

    private IEnumerator AnimateProgressBar()
    {
        while (!isComplete)
        {
            // Lerp suave hacia el objetivo
            currentProgress = Mathf.Lerp(currentProgress, targetProgress, Time.unscaledDeltaTime * 3f);

            if (progressBar != null)
                progressBar.fillAmount = currentProgress;

            // Avance automático aleatorio mientras no se haya completado
            if (targetProgress < 0.85f)
            {
                targetProgress += Random.Range(0.005f, 0.02f);
                targetProgress = Mathf.Clamp01(targetProgress);
            }

            yield return null;
        }

        // Asegurar barra llena al completar
        if (progressBar != null)
            progressBar.fillAmount = 1f;
    }
}