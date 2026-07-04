using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelGoal : MonoBehaviour
{
    [Header("通关设置")]
    [Tooltip("触碰后等待多长时间切换到下一关（秒）")]
    [SerializeField] private float completionDelay = 2f;
    [Tooltip("下一关的场景名称，例如 Level_02")]
    [SerializeField] private string nextLevelSceneName = "Level_02";

    private bool triggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 通过 Tag 判断是否为玩家，避免其他物体误触
        if (!triggered && other.CompareTag("Player"))
        {
            triggered = true;
            SFXManager.Play(SFXType.LevelComplete);
            TextPromptManager.Show(TextPromptEvent.LevelComplete);
            StartCoroutine(CompleteLevelRoutine());
        }
    }

    private IEnumerator CompleteLevelRoutine()
    {
        // 1. 冻结所有输入，但保持物理（timeScale 仍为 1）
        DisableAllInputComponents();

        // 2. 等待指定秒数（使用真实时间，不受 timeScale 影响）
        yield return new WaitForSecondsRealtime(completionDelay);

        // 3. 检查下一关卡场景是否存在
        if (!SceneExists(nextLevelSceneName))
        {
            Debug.LogWarning($"关卡场景 '{nextLevelSceneName}' 不存在于 Build Settings 中，无法加载！已恢复控制。");
            EnableAllInputComponents();
            triggered = false; // 允许再次尝试
            yield break;
        }

        // 4. 加载下一关（自动卸载当前场景）
        SFXManager.Play(SFXType.SceneTransition);
        SceneManager.LoadScene(nextLevelSceneName);
    }

    /// <summary>
    /// 禁用所有输入相关组件（玩家移动、地图移动、暂停、模式切换、点击固定物体）
    /// </summary>
    private void DisableAllInputComponents()
    {
        // 玩家移动
        PlayerMoveNoJump playerMove = FindObjectOfType<PlayerMoveNoJump>();
        if (playerMove != null) playerMove.enabled = false;

        // 地图移动/旋转
        MapTransformController mapCtrl = FindObjectOfType<MapTransformController>();
        if (mapCtrl != null) mapCtrl.enabled = false;

        // 暂停管理器
        PauseManager pauseMgr = FindObjectOfType<PauseManager>();
        if (pauseMgr != null) pauseMgr.enabled = false;

        // 模式切换（Tab键）
        GameModeController modeCtrl = FindObjectOfType<GameModeController>();
        if (modeCtrl != null) modeCtrl.enabled = false;

        // 点击固定物体
        ViewFixableClickController clickCtrl = FindObjectOfType<ViewFixableClickController>();
        if (clickCtrl != null) clickCtrl.enabled = false;
    }

    /// <summary>
    /// 重新启用所有被禁用的输入组件（当关卡不存在时恢复控制）
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
    /// 检查场景名称是否存在于 Build Settings 的场景列表中
    /// </summary>
    private bool SceneExists(string sceneName)
    {
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string path = SceneUtility.GetScenePathByBuildIndex(i);
            string name = System.IO.Path.GetFileNameWithoutExtension(path);
            if (name == sceneName)
                return true;
        }
        return false;
    }
}
