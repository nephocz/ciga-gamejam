// ========================================================================
// 文件功能：玩家死亡处理器
// 负责检测玩家是否掉落出边界（Y 坐标低于阈值），触发死亡流程：冻结游戏、
// 禁用暂停管理器、等待指定时间后调用场景过渡管理器重新加载当前关卡。
// 在组件面板中可配置掉落阈值和死亡等待时间。
// ========================================================================

using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerDeathHandler : MonoBehaviour
{
    [Header("死亡判定")]
    [SerializeField] private float fallThresholdY = -20f;   // 玩家掉落到此 Y 坐标以下判定死亡
    [SerializeField] private float deathPauseDuration = 1.5f; // 死亡后等待几秒再重载场景（对外接口）

    private bool isDead = false;                            // 防止重复触发

    /// <summary>
    /// Unity 生命周期函数 Update，每帧调用。
    /// 检查玩家是否死亡：通过比较 transform.position.y 与面板配置的 fallThresholdY，
    /// 若 Y 坐标低于阈值且未死亡，则调用 StartCoroutine 启动 OnPlayerDeath 协程。
    /// 在组件面板中可显示的字段：fallThresholdY（掉落阈值）、deathPauseDuration（死亡等待时间）。
    /// </summary>
    private void Update()
    {
        if (isDead)
            return;

        if (transform.position.y < fallThresholdY)
        {
            StartCoroutine(OnPlayerDeath());
        }
    }

    /// <summary>
    /// 玩家死亡流程协程 OnPlayerDeath。
    /// 设置死亡标志防止重复触发；通过 Time.timeScale = 0 冻结游戏逻辑；
    /// 使用 FindObjectOfType&lt;PauseManager&gt;() 查找并禁用暂停管理器，避免死亡期间弹出暂停菜单；
    /// 调用 WaitForSecondsRealtime 等待面板配置的 deathPauseDuration 秒（不受时间缩放影响）；
    /// 最后恢复时间缩放为 1f，并通过 SceneTransitionManager.LoadScene 传入当前场景构建索引重新加载关卡。
    /// 在组件面板中可显示的字段：deathPauseDuration（死亡等待时间）。
    /// </summary>
    private IEnumerator OnPlayerDeath()
    {
        isDead = true;

        // 1. 冻结游戏
        Time.timeScale = 0f;

        // 2. 禁用暂停管理器，避免死亡期间按 ESC 弹出暂停界面
        PauseManager pauseManager = FindObjectOfType<PauseManager>();
        if (pauseManager != null)
        {
            pauseManager.enabled = false;
        }

        // 3. 等待（使用 WaitForSecondsRealtime 因为 timeScale = 0 时 WaitForSeconds 会停止）
        yield return new WaitForSecondsRealtime(deathPauseDuration);

        // 4. 恢复时间并重新加载当前关卡
        Time.timeScale = 1f;
        SceneTransitionManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}