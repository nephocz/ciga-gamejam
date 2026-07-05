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

    private IEnumerator UnlockAfterDelay()
    {
        yield return new WaitForSecondsRealtime(lockDuration);
        canProceed = true;
    }

    private void Update()
    {
        if (!canProceed)
            return;

        // 检测任意键盘按键
        if (Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame)
        {
            ProceedToMainMenu();
        }
    }

    private void ProceedToMainMenu()
    {
        // 恢复时间缩放
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }
}

