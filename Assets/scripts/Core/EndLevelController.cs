// ========================================================================
// 文件功能：关卡结束控制器
// 负责关卡结束后的流程控制：冻结游戏、强制等待一段时间后允许玩家按任意键返回主菜单。
// 通过 Time.timeScale 暂停游戏逻辑，利用 Input System 检测按键，调用场景管理器加载主菜单。
// ========================================================================

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Collections;

public class EndLevelController : MonoBehaviour
{
    [Header("结束界面设置")]
    [Tooltip("加载后强制等待的时间（秒）")]
    [SerializeField] private float lockDuration = 5f;

    [Tooltip("按下任意键后跳转的场景名称（通常是主菜单）")]
    [SerializeField] private string mainMenuSceneName = "MainMenuScene";

    private bool canProceed = false;

    /// <summary>
    /// Unity 生命周期函数 Start，在脚本实例被启用时调用。
    /// 通过设置 Time.timeScale = 0 冻结游戏所有基于时间缩放的动作，
    /// 并查找场景中的 PauseManager 组件并禁用，防止暂停菜单干扰结束流程，
    /// 最后启动协程 UnlockAfterDelay 开始强制等待。
    /// 在组件面板中可配置的字段：lockDuration（强制等待时间）、mainMenuSceneName（目标主菜单场景名）。
    /// </summary>
    private void Start()
    {
        // 冻结游戏，禁用所有动作
        Time.timeScale = 0f;

        // 可选：禁用暂停管理器，防止按 ESC 干扰
        PauseManager pauseManager = FindObjectOfType<PauseManager>();
        if (pauseManager != null)
            pauseManager.enabled = false;

        // 启动等待协程
        StartCoroutine(UnlockAfterDelay());
    }

    /// <summary>
    /// 协程 UnlockAfterDelay，使用 WaitForSecondsRealtime 实现不受 timeScale 影响的实时等待。
    /// 等待 lockDuration 秒后，将 canProceed 设为 true，允许玩家按键跳转。
    /// </summary>
    private IEnumerator UnlockAfterDelay()
    {
        yield return new WaitForSecondsRealtime(lockDuration);
        canProceed = true;
    }

    /// <summary>
    /// Unity 生命周期函数 Update，每帧调用。
    /// 在 canProceed 为 true 后，通过 UnityEngine.InputSystem.Keyboard 检测任意键盘按键（anyKey.wasPressedThisFrame），
    /// 若按下则调用 ProceedToMainMenu 跳转场景。
    /// </summary>
    private void Update()
    {
        if (!canProceed)
            return;

        // 检测任意键盘按键
        if (Keyboard.current != null && (Keyboard.current.anyKey.wasPressedThisFrame
            ||Input.GetMouseButtonDown(0)|| Input.GetMouseButtonDown(1)|| Input.GetMouseButtonDown(2)))
        {
            ProceedToMainMenu();
        }
    }

    /// <summary>
    /// 跳转至主菜单的方法。
    /// 先将 Time.timeScale 恢复为 1f，再通过 SceneManager.LoadScene 加载在组件面板中配置的场景 mainMenuSceneName。
    /// </summary>
    private void ProceedToMainMenu()
    {
        // 恢复时间缩放
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }
}