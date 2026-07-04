using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerDeathHandler : MonoBehaviour
{
    [Header("死亡判定")]
    [SerializeField] private float fallThresholdY = -20f;   // 玩家掉落到此 Y 坐标以下判定死亡
    [SerializeField] private float deathPauseDuration = 1.5f; // 死亡后等待几秒再重载场景（对外接口）

    private bool isDead = false;                            // 防止重复触发

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
    /// 玩家死亡流程：冻结游戏、禁用暂停管理器、等待指定时间后重新加载当前关卡。
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
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}