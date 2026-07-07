using UnityEngine;
using UnityEngine.SceneManagement;
<<<<<<< Updated upstream

=======
using UnityEngine.UI;
// ========================================================================
// 文件功能：暂停菜单控制器
// 负责暂停菜单场景的初始化与交互：规范化 Canvas 缩放、转换旧版 Sprite 按钮为 UI 按钮、
// 应用背景颜色，并响应继续、退出、重新开始按钮的点击事件。
// 在组件面板中可配置背景图片引用及其颜色。
// ========================================================================
>>>>>>> Stashed changes
public class PauseMenuController : MonoBehaviour
{
    private PauseManager pauseManager;
<<<<<<< Updated upstream

    private void Start()
    {
        // 找到关卡中的 PauseManager（因为叠加场景后，原场景对象仍在）
        pauseManager = FindObjectOfType<PauseManager>();
    }

    /// <summary> 继续游戏按钮回调 </summary>
=======
    // ========================================================================
    /// <summary>
    /// Unity 生命周期函数 Awake，在脚本实例被加载时调用。
    /// 依次执行：NormalizeCanvas（规范画布缩放设置）、ConvertSpriteButtonsToUI（将旧版 Sprite 按钮转换为 UI 按钮）、
    /// ApplyBackgroundColor（应用背景颜色），完成暂停菜单界面的初始化设置。
    /// 可在组件面板中显示的字段：backgroundImage（背景图片）、backgroundColor（背景颜色）。
    /// </summary>
    private void Awake()
    {
        NormalizeCanvas();
        ConvertSpriteButtonsToUI();
        ApplyBackgroundColor();
    }
    // ========================================================================
    /// <summary>
    /// Unity 生命周期函数 Start，在首次 Update 之前调用。
    /// 通过 FindFirstObjectByType&lt;PauseManager&gt;() 查找场景中的暂停管理器并缓存，供按钮回调使用。
    /// </summary>
    private void Start()
    {
        pauseManager = FindFirstObjectByType<PauseManager>();
    }
    // ========================================================================
    /// <summary>
    /// 应用背景颜色。
    /// 若面板未指定 backgroundImage，则使用 GameObject.Find("Panel") 查找名为 Panel 的对象并获取其 Image 组件；
    /// 最终将 backgroundImage.color 设为面板配置的 backgroundColor。
    /// 可在组件面板中显示的字段：backgroundImage、backgroundColor。
    /// </summary>
    private void ApplyBackgroundColor()
    {
        if (backgroundImage == null)
        {
            GameObject panel = GameObject.Find("Panel");

            if (panel != null)
            {
                backgroundImage = panel.GetComponent<Image>();
            }
        }

        if (backgroundImage != null)
        {
            backgroundImage.color = backgroundColor;
        }
    }
    // ========================================================================
    /// <summary>
    /// 规范 Canvas 缩放设置。
    /// 调用 FindCanvasInCurrentScene 查找当前场景中的 Canvas，并获取其 CanvasScaler 组件；
    /// 使用 CanvasScaler 的 API 将 UI 缩放模式设为 ScaleWithScreenSize，参考分辨率设为 1920x1080，
    /// 匹配模式设为 MatchWidthOrHeight 且宽高匹配权重设为 0.5，确保 UI 在不同分辨率下合理缩放。
    /// </summary>
    private void NormalizeCanvas()
    {
        Canvas canvas = FindCanvasInCurrentScene();
        CanvasScaler canvasScaler = canvas != null ? canvas.GetComponent<CanvasScaler>() : null;

        if (canvasScaler == null)
        {
            return;
        }

        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasScaler.referenceResolution = new Vector2(1920f, 1080f);
        canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        canvasScaler.matchWidthOrHeight = 0.5f;
    }
    // ========================================================================
    /// <summary>
    /// 将场景中使用 SpriteRenderer 的旧版按钮转换为标准 UI 按钮。
    /// 通过 FindCanvasInCurrentScene 获取 Canvas，遍历其下的所有 Button 组件；
    /// 对每个 Button 查找子物体上的 SpriteRenderer，将其精灵、颜色复制到按钮关联的 Image 上，
    /// 并禁用原 SpriteRenderer，使按钮以 UI Image 方式正常渲染和响应射线。
    /// </summary>
    private void ConvertSpriteButtonsToUI()
    {
        Canvas canvas = FindCanvasInCurrentScene();

        if (canvas == null)
        {
            return;
        }

        Button[] buttons = canvas.GetComponentsInChildren<Button>(true);

        foreach (Button button in buttons)
        {
            Image buttonImage = button.targetGraphic as Image;

            if (buttonImage == null)
            {
                buttonImage = button.GetComponent<Image>();
            }

            SpriteRenderer spriteRenderer = button.GetComponentInChildren<SpriteRenderer>(true);

            if (buttonImage == null || spriteRenderer == null || spriteRenderer.sprite == null)
            {
                continue;
            }

            buttonImage.sprite = spriteRenderer.sprite;
            buttonImage.color = spriteRenderer.color;
            buttonImage.type = Image.Type.Simple;
            buttonImage.preserveAspect = false;
            buttonImage.raycastTarget = true;
            button.targetGraphic = buttonImage;
            spriteRenderer.gameObject.SetActive(false);
        }
    }
    // ========================================================================
    /// <summary>
    /// 在当前场景中查找 Canvas 组件。
    /// 使用 gameObject.scene.GetRootGameObjects() 获取当前场景的所有根物体，
    /// 再通过 GetComponentInChildren&lt;Canvas&gt;(true) 递归查找 Canvas 并返回。
    /// </summary>
    private Canvas FindCanvasInCurrentScene()
    {
        GameObject[] rootObjects = gameObject.scene.GetRootGameObjects();

        foreach (GameObject rootObject in rootObjects)
        {
            Canvas canvas = rootObject.GetComponentInChildren<Canvas>(true);

            if (canvas != null)
            {
                return canvas;
            }
        }

        return null;
    }
    // ========================================================================
    /// <summary>
    /// “继续”按钮点击回调。
    /// 调用 SFXManager.Play(SFXType.ButtonClick) 播放按钮音效；
    /// 若持有 pauseManager 则调用其 ResumeGame 方法恢复游戏，
    /// 否则直接设置 Time.timeScale = 1f 并通过 SceneManager.UnloadSceneAsync 卸载自身场景。
    /// </summary>
>>>>>>> Stashed changes
    public void OnContinueButton()
    {
        if (pauseManager != null)
            pauseManager.ResumeGame();
        else
        {
            // 保底逻辑
            Time.timeScale = 1f;
            SceneManager.UnloadSceneAsync(gameObject.scene);
        }
    }
<<<<<<< Updated upstream

    /// <summary> 退出游戏按钮回调 </summary>
=======
    // ========================================================================
    /// <summary>
    /// “退出”按钮点击回调。
    /// 播放按钮音效和场景过渡音效，将 Time.timeScale 恢复为 1f，
    /// 通过 SceneTransitionManager.LoadScene 加载主菜单场景 "MainMenuScene"。
    /// </summary>
>>>>>>> Stashed changes
    public void OnExitButton()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenuScene");
    }
<<<<<<< Updated upstream

    /// <summary> 重新开始按钮回调 </summary>
=======
    // ========================================================================
    /// <summary>
    /// “重新开始”按钮点击回调。
    /// 播放按钮音效和场景过渡音效，将 Time.timeScale 恢复为 1f，
    /// 通过 SceneTransitionManager.LoadScene 传入当前活动场景的 buildIndex 重新加载关卡。
    /// </summary>
>>>>>>> Stashed changes
    public void OnRestartButton()
    {
        Time.timeScale = 1f;
        // 重新加载当前活动场景（即关卡本身）
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
