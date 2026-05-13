using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Animador visual de la pantalla de carga.
/// Maneja animaciones de progreso y texto dinámico.
/// </summary>
public class LoadingScreenAnimator : MonoBehaviour
{
    private Text loadingText;
    private Image progressBar;
    private float currentProgress = 0f;
    private float targetProgress = 0f;
    private bool isAnimating = true;

    public void Initialize(Text textComponent, Image progressBarImage)
    {
        loadingText = textComponent;
        progressBar = progressBarImage;
        currentProgress = 0f;
        targetProgress = 0f;
        isAnimating = true;
        StartCoroutine(AnimateLoadingText());
        StartCoroutine(AnimateProgressBar());
    }

    public void SetProgress(float progress)
    {
        targetProgress = Mathf.Clamp01(progress);
    }

    public void Complete()
    {
        isAnimating = false;
        targetProgress = 1f;
        currentProgress = 1f;
        if (progressBar != null)
        {
            progressBar.fillAmount = 1f;
        }
    }

    private IEnumerator AnimateLoadingText()
    {
        string[] frames = new string[] { "Cargando.", "Cargando..", "Cargando..." };
        int frameIndex = 0;

        while (isAnimating)
        {
            if (loadingText != null)
            {
                loadingText.text = frames[frameIndex];
                frameIndex = (frameIndex + 1) % frames.Length;
            }
            yield return new WaitForSeconds(0.4f);
        }

        if (loadingText != null)
        {
            loadingText.text = "¡Listo!";
        }
    }

    private IEnumerator AnimateProgressBar()
    {
        while (isAnimating)
        {
            // Animar suavemente hacia el progreso objetivo
            currentProgress = Mathf.Lerp(currentProgress, targetProgress, Time.deltaTime * 2f);

            if (progressBar != null)
            {
                progressBar.fillAmount = currentProgress;
            }

            // Si está muy cerca del objetivo, avanzar automáticamente un poco
            if (isAnimating && targetProgress < 0.9f && currentProgress > targetProgress - 0.05f)
            {
                targetProgress += Random.Range(0.05f, 0.15f);
                targetProgress = Mathf.Clamp01(targetProgress);
            }

            yield return new WaitForEndOfFrame();
        }
    }
}
