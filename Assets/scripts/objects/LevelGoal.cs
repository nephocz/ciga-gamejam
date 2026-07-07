// ========================================================================
// 文件功能：关卡终点控制器
// 负责玩家到达关卡终点后的通关流程：检测触发器碰撞、播放通关音效与提示、
// 禁用所有输入组件、根据配置自动或手动获取下一关场景名并加载。
// 在组件面板中可配置延迟时间、是否自动递增关卡编号、手动场景名及最大关卡号。
// ========================================================================

using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelGoal : MonoBehaviour
{
    [Header("Level Complete")]
    [Tooltip("Delay before loading the next level, in seconds.")]
    [SerializeField] private float completionDelay = 2f;
    [Tooltip("Automatically load the next numbered level, for example Level_001 -> Level_002.")]
    [SerializeField] private bool autoLoadNextNumberedLevel = true;
    [Tooltip("Manual fallback scene name. Used when auto loading is disabled or current scene is not numbered.")]
    [SerializeField] private string nextLevelSceneName = "Level_002";
    [Tooltip("Auto loading stops after this level number.")]
    [SerializeField] private int maxAutoLevelNumber = 6;

    private bool triggered;

    /// <summary>
    /// Unity 触发器进入回调 OnTriggerEnter2D，当其他 2D 碰撞体进入触发器时调用。
    /// 通过 Collider2D.CompareTag("Player") 检测是否玩家进入，若未触发则设置 triggered 标志，
    /// 调用 SFXManager.Play 播放关卡完成音效、TextPromptManager.Show 显示通关提示，
    /// 并使用 StartCoroutine 启动 CompleteLevelRoutine 协程。
    /// 本类在组件面板中可配置的字段：completionDelay（完成延迟）、autoLoadNextNumberedLevel（是否自动编号）、
    /// nextLevelSceneName（手动下一关场景名）、maxAutoLevelNumber（最大自动编号关卡数）。
    /// </summary>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!triggered && other.CompareTag("Player"))
        {
            triggered = true;
            SFXManager.Play(SFXType.LevelComplete);
            TextPromptManager.Show(TextPromptEvent.LevelComplete);
            StartCoroutine(CompleteLevelRoutine());
        }
    }

    /// <summary>
    /// 关卡完成协程 CompleteLevelRoutine。
    /// 首先调用 DisableAllInputComponents 禁用玩家与输入相关的所有组件；
    /// 然后使用 WaitForSecondsRealtime 等待 completionDelay 秒不受时间缩放影响的真实时间；
    /// 通过 GetNextLevelSceneName 获取下一个关卡场景名，若无效则调用 EnableAllInputComponents 恢复并退出；
    /// 调用 SceneExists 检查场景是否在 Build Settings 中，不存在则同样恢复；
    /// 最后通过 SFXManager.Play 播放场景过渡音效，调用 SceneTransitionManager.LoadScene 加载目标场景。
    /// 本类在组件面板中可配置的字段同上。
    /// </summary>
    private IEnumerator CompleteLevelRoutine()
    {
        DisableAllInputComponents();

        yield return new WaitForSecondsRealtime(completionDelay);

        string targetSceneName = GetNextLevelSceneName();

        if (string.IsNullOrWhiteSpace(targetSceneName))
        {
            Debug.LogWarning("No next level scene is available.");
            EnableAllInputComponents();
            triggered = false;
            yield break;
        }

        if (!SceneExists(targetSceneName))
        {
            Debug.LogWarning($"Level scene '{targetSceneName}' is not in Build Settings.");
            EnableAllInputComponents();
            triggered = false;
            yield break;
        }

        SFXManager.Play(SFXType.SceneTransition);
        SceneTransitionManager.LoadScene(targetSceneName);
    }

    /// <summary>
    /// 获取下一关场景名的方法。
    /// 若不使用自动编号则直接返回面板配置的 nextLevelSceneName；
    /// 否则通过 SceneManager.GetActiveScene().name 获取当前场景名，
    /// 使用 Regex.Match 和正则表达式 @"^(.*?)(\d+)$" 分离前缀与末尾数字，
    /// 解析当前关卡编号并 +1，判断是否超过 maxAutoLevelNumber 决定返回空或带前缀的下一关名称。
    /// 本类在组件面板中可配置的字段：autoLoadNextNumberedLevel、nextLevelSceneName、maxAutoLevelNumber。
    /// </summary>
    private string GetNextLevelSceneName()
    {
        if (!autoLoadNextNumberedLevel)
        {
            return nextLevelSceneName;
        }

        string currentSceneName = SceneManager.GetActiveScene().name;
        Match match = Regex.Match(currentSceneName, @"^(.*?)(\d+)$");

        if (!match.Success)
        {
            return nextLevelSceneName;
        }

        string prefix = match.Groups[1].Value;
        string numberText = match.Groups[2].Value;
        int currentLevelNumber = int.Parse(numberText);
        int nextLevelNumber = currentLevelNumber + 1;

        if (nextLevelNumber > maxAutoLevelNumber)
        {
            return string.Empty;
        }

        return $"{prefix}{nextLevelNumber.ToString().PadLeft(numberText.Length, '0')}";
    }

    /// <summary>
    /// 禁用所有输入相关组件。
    /// 通过 FindObjectOfType 查找场景中的 PlayerMoveNoJump、MapTransformController、
    /// PauseManager、GameModeController、ViewFixableClickController 实例，
    /// 并将它们的 enabled 属性设为 false，使玩家在通关等待期间无法操作。
    /// 本类在组件面板中可配置的字段：completionDelay、autoLoadNextNumberedLevel、nextLevelSceneName、maxAutoLevelNumber。
    /// </summary>
    private void DisableAllInputComponents()
    {
        PlayerMoveNoJump playerMove = FindObjectOfType<PlayerMoveNoJump>();
        if (playerMove != null) playerMove.enabled = false;

        MapTransformController mapCtrl = FindObjectOfType<MapTransformController>();
        if (mapCtrl != null) mapCtrl.enabled = false;

        PauseManager pauseMgr = FindObjectOfType<PauseManager>();
        if (pauseMgr != null) pauseMgr.enabled = false;

        GameModeController modeCtrl = FindObjectOfType<GameModeController>();
        if (modeCtrl != null) modeCtrl.enabled = false;

        ViewFixableClickController clickCtrl = FindObjectOfType<ViewFixableClickController>();
        if (clickCtrl != null) clickCtrl.enabled = false;
    }

    /// <summary>
    /// 启用所有输入相关组件。
    /// 同样通过 FindObjectOfType 查找上述组件，将 enabled 设为 true，
    /// 用于加载下一关失败或目标场景不存在时恢复玩家输入。
    /// 本类在组件面板中可配置的字段同上。
    /// </summary>
    private void EnableAllInputComponents()
    {
        PlayerMoveNoJump playerMove = FindObjectOfType<PlayerMoveNoJump>();
        if (playerMove != null) playerMove.enabled = true;

        MapTransformController mapCtrl = FindObjectOfType<MapTransformController>();
        if (mapCtrl != null) mapCtrl.enabled = true;

        PauseManager pauseMgr = FindObjectOfType<PauseManager>();
        if (pauseMgr != null) pauseMgr.enabled = true;

        GameModeController modeCtrl = FindObjectOfType<GameModeController>();
        if (modeCtrl != null) modeCtrl.enabled = true;

        ViewFixableClickController clickCtrl = FindObjectOfType<ViewFixableClickController>();
        if (clickCtrl != null) clickCtrl.enabled = true;
    }

    /// <summary>
    /// 检查场景是否存在于 Build Settings 中。
    /// 通过 SceneManager.sceneCountInBuildSettings 获取构建场景总数，
    /// 遍历使用 SceneUtility.GetScenePathByBuildIndex 获取每个构建索引的场景路径，
    /// 并利用 System.IO.Path.GetFileNameWithoutExtension 提取场景文件名与传入名称比较。
    /// 本类在组件面板中可配置的字段同上。
    /// </summary>
    private bool SceneExists(string sceneName)
    {
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string path = SceneUtility.GetScenePathByBuildIndex(i);
            string name = System.IO.Path.GetFileNameWithoutExtension(path);

            if (name == sceneName)
            {
                return true;
            }
        }

        return false;
    }
}