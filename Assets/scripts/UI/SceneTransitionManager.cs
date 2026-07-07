// ========================================================================
// 文件功能：场景过渡管理器
// 作为单例工具，提供带淡入淡出效果的多场景加载功能。
// 通过协程控制 Image 的透明度实现黑场过渡，在切换场景前后执行淡入和淡出，
// 支持按场景名称或构建索引加载。首次调用时若不存在实例会自动创建，并保持跨场景存活。
// 在组件面板中可配置过渡图片（Image）、过渡时长、是否初始透明。
// ========================================================================

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

    /// <summary>
    /// Unity 生命周期函数 Awake，在实例加载时调用。
    /// 实现单例模式：若已有实例则销毁自身；否则将自身设为持久化对象（DontDestroyOnLoad），
    /// 若未在面板指定 fadeImage，则调用 CreateFadeImage 动态创建全屏黑场 Image；
    /// 并根据 startTransparent 决定初始透明度。
    /// 在组件面板中可显示的字段：fadeImage（过渡图片）、fadeDuration（过渡时长）、startTransparent（初始透明）。
    /// </summary>
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

    /// <summary>
    /// 公开静态方法：按场景名称加载场景（带淡入淡出效果）。
    /// 通过 GetOrCreateInstance 获取或创建单例实例，若有效则启动 LoadSceneRoutine 协程，
    /// 否则直接调用 SceneManager.LoadScene 进行无过渡加载。
    /// 无在组件面板中显示的字段（静态方法，操作实例的 fadeDuration 等）。
    /// </summary>
    public static void LoadScene(string sceneName)
    {
        SceneTransitionManager manager = GetOrCreateInstance();

        if (manager.fadeImage == null)
        {
            SceneManager.LoadScene(sceneName);
            return;
        }

        manager.StartCoroutine(manager.LoadSceneRoutine(sceneName));
    }

    /// <summary>
    /// 公开静态方法：按构建索引加载场景（带淡入淡出效果）。
    /// 与 LoadScene(string) 类似，传递场景的构建索引，启动 LoadSceneRoutine(int) 协程。
    /// 无在组件面板中显示的字段。
    /// </summary>
    public static void LoadScene(int buildIndex)
    {
        SceneTransitionManager manager = GetOrCreateInstance();

        if (manager.fadeImage == null)
        {
            SceneManager.LoadScene(buildIndex);
            return;
        }

        manager.StartCoroutine(manager.LoadSceneRoutine(buildIndex));
    }

    /// <summary>
    /// 私有静态方法：获取或创建 SceneTransitionManager 单例实例。
    /// 若 instance 为空，则创建一个名为 "SceneTransitionManager" 的新 GameObject 并挂载该组件。
    /// 返回实例引用。
    /// 无在组件面板中显示的字段。
    /// </summary>
    private static SceneTransitionManager GetOrCreateInstance()
    {
        if (instance != null)
        {
            return instance;
        }

        GameObject managerObject = new GameObject("SceneTransitionManager");
        return managerObject.AddComponent<SceneTransitionManager>();
    }

    /// <summary>
    /// 协程：按场景名称执行过渡加载。
    /// 若正在过渡中（isTransitioning）则直接退出；
    /// 先调用 FadeTo(1f) 将屏幕淡入黑屏，然后使用 SceneManager.LoadScene(sceneName) 加载新场景；
    /// 等待一帧后调用 FadeTo(0f) 淡出黑屏，完成过渡。
    /// 在组件面板中可显示的字段：fadeDuration（影响 FadeTo 时长）。
    /// </summary>
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

    /// <summary>
    /// 协程：按构建索引执行过渡加载。
    /// 逻辑与 LoadSceneRoutine(string) 相同，使用 SceneManager.LoadScene(buildIndex)。
    /// 在组件面板中可显示的字段：fadeDuration。
    /// </summary>
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

    /// <summary>
    /// 协程：在给定时间内将 fadeImage 的透明度渐变至目标值。
    /// 利用 Time.unscaledDeltaTime 保证不受时间缩放影响。
    /// 若 fadeDuration <= 0 则直接设置透明度；否则每帧通过 Mathf.Lerp 插值更新 Image.color.a。
    /// 在组件面板中可显示的字段：fadeImage、fadeDuration。
    /// </summary>
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

    /// <summary>
    /// 私有方法：设置 fadeImage 的透明度。
    /// 通过修改 Image.color 的 alpha 通道实现。
    /// </summary>
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

    /// <summary>
    /// 私有方法：动态创建全屏黑场遮罩 Image。
    /// 创建 Canvas（ScreenSpaceOverlay，最高渲染顺序）和 CanvasScaler（1920x1080 参考分辨率），
    /// 生成一个拉伸至全屏的黑色 Image 并关闭射线检测，作为过渡遮罩。
    /// 无在组件面板中显示的字段（内部创建）。
    /// </summary>
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