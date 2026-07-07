using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
// ========================================================================
// 文件功能：暂停管理器
// 负责处理游戏的暂停与恢复：通过检测 ESC 按键，控制时间缩放并动态加载/卸载暂停场景。
// 在组件面板中可配置暂停场景的名称。
// ========================================================================
public class PauseManager : MonoBehaviour
{
    [SerializeField] private string pauseSceneName = "PauseMenuScene";

    private bool isPaused = false;
    // ========================================================================
    /// <summary>
    /// Unity 生命周期函数 Update，每帧调用。
    /// 通过 UnityEngine.InputSystem.Keyboard.current 检测 ESC 键（escapeKey.wasPressedThisFrame），
    /// 若未暂停则调用 PauseGame，已暂停则调用 ResumeGame，实现暂停状态切换。
    /// 在组件面板中可显示的字段：pauseSceneName（暂停场景名称）。
    /// </summary>
    private void Update()
    {
        if (Keyboard.current == null)
            return;

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (!isPaused)
                PauseGame();
            else
                ResumeGame();
        }
    }
    // ========================================================================
    /// <summary>
    /// 暂停游戏公开方法。
    /// 设置暂停标志，通过 Time.timeScale = 0 冻结所有基于时间缩放的逻辑；
    /// 使用 SceneManager.LoadScene(pauseSceneName, LoadSceneMode.Additive) 以叠加方式加载暂停界面场景。
    /// 在组件面板中可显示的字段：pauseSceneName。
    /// </summary>
    public void PauseGame()
    {
        if (isPaused) return;
        isPaused = true;
        Time.timeScale = 0f;
        SceneManager.LoadScene(pauseSceneName, LoadSceneMode.Additive);
    }
    // ========================================================================
    /// <summary>
    /// 恢复游戏公开方法。
    /// 清除暂停标志，将 Time.timeScale 恢复为 1f；
    /// 通过 SceneManager.UnloadSceneAsync(pauseSceneName) 异步卸载暂停界面场景，使游戏恢复正常运行。
    /// 在组件面板中可显示的字段：pauseSceneName。
    /// </summary>
    public void ResumeGame()
    {
        if (!isPaused) return;
        isPaused = false;
        Time.timeScale = 1f;
        SceneManager.UnloadSceneAsync(pauseSceneName);
    }
}