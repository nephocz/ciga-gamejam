using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    private PauseManager pauseManager;

    private void Start()
    {
        // 找到关卡中的 PauseManager（因为叠加场景后，原场景对象仍在）
        pauseManager = FindObjectOfType<PauseManager>();
    }

    /// <summary> 继续游戏按钮回调 </summary>
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

    /// <summary> 退出游戏按钮回调 </summary>
    public void OnExitButton()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenuScene");
    }

    /// <summary> 重新开始按钮回调 </summary>
    public void OnRestartButton()
    {
        Time.timeScale = 1f;
        // 重新加载当前活动场景（即关卡本身）
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
