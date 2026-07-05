using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneTransitionManager : MonoBehaviour
{
    [SerializeField] private Image fadeImage;
    [SerializeField, Min(0f)] private float fadeDuration = 0.5f;
    [SerializeField] private bool startTransparent = true;

    private static SceneTransitionManager instance;
    private bool isTransitioning;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        if (fadeImage == null)
        {
            fadeImage = CreateFadeImage();
        }

        fadeImage.raycastTarget = false;

        if (startTransparent)
        {
            SetAlpha(0f);
        }
    }

    public static void LoadScene(string sceneName)
    {
        if (instance == null || instance.fadeImage == null)
        {
            SceneManager.LoadScene(sceneName);
            return;
        }

        instance.StartCoroutine(instance.LoadSceneRoutine(sceneName));
    }

    public static void LoadScene(int buildIndex)
    {
        if (instance == null || instance.fadeImage == null)
        {
            SceneManager.LoadScene(buildIndex);
            return;
        }

        instance.StartCoroutine(instance.LoadSceneRoutine(buildIndex));
    }

    private IEnumerator LoadSceneRoutine(string sceneName)
    {
        if (isTransitioning)
        {
            yield break;
        }

        isTransitioning = true;

        yield return FadeTo(1f);
        SceneManager.LoadScene(sceneName);
        yield return null;
        yield return FadeTo(0f);

        isTransitioning = false;
    }

    private IEnumerator LoadSceneRoutine(int buildIndex)
    {
        if (isTransitioning)
        {
            yield break;
        }

        isTransitioning = true;

        yield return FadeTo(1f);
        SceneManager.LoadScene(buildIndex);
        yield return null;
        yield return FadeTo(0f);

        isTransitioning = false;
    }

    private IEnumerator FadeTo(float targetAlpha)
    {
        float startAlpha = fadeImage.color.a;
        float elapsed = 0f;

        if (fadeDuration <= 0f)
        {
            SetAlpha(targetAlpha);
            yield break;
        }

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            SetAlpha(Mathf.Lerp(startAlpha, targetAlpha, elapsed / fadeDuration));
            yield return null;
        }

        SetAlpha(targetAlpha);
    }

    private void SetAlpha(float alpha)
    {
        if (fadeImage == null)
        {
            return;
        }

        Color color = fadeImage.color;
        color.a = alpha;
        fadeImage.color = color;
    }

    private Image CreateFadeImage()
    {
        GameObject canvasObject = new GameObject("SceneTransitionCanvas");
        canvasObject.transform.SetParent(transform, false);

        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = short.MaxValue;

        CanvasScaler canvasScaler = canvasObject.AddComponent<CanvasScaler>();
        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasScaler.referenceResolution = new Vector2(1920f, 1080f);
        canvasScaler.matchWidthOrHeight = 0.5f;

        canvasObject.AddComponent<GraphicRaycaster>();

        GameObject imageObject = new GameObject("FadeImage");
        imageObject.transform.SetParent(canvasObject.transform, false);

        Image image = imageObject.AddComponent<Image>();
        image.color = Color.black;
        image.raycastTarget = false;

        RectTransform rectTransform = image.rectTransform;
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;

        return image;
    }
}
