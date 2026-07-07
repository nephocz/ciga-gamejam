// ========================================================================
// 文件功能：场景加载器
// 提供一系列公开方法用于加载指定场景（主菜单、关卡选择、制作人员名单、各关卡等）
// 以及退出游戏。通过调用 SFXManager 播放音效，再调用 SceneTransitionManager 加载场景。
// 本类无在组件面板中可配置的字段，所有方法通过 UI 按钮事件调用。
// ========================================================================

using UnityEngine;

public class SceneLoader : MonoBehaviour
{
    /// <summary>
    /// 加载主菜单场景。
    /// 调用 LoadSceneWithSFX 传入场景名 "MainMenuScene" 进行加载。
    /// 无在组件面板中显示的字段。
    /// </summary>
    public void LoadMainMenu()
    {
        LoadSceneWithSFX("MainMenuScene");
    }

    /// <summary>
    /// 加载关卡选择场景。
    /// 调用 LoadSceneWithSFX 传入场景名 "LevelSelectScene" 进行加载。
    /// 无在组件面板中显示的字段。
    /// </summary>
    public void LoadLevelSelect()
    {
        LoadSceneWithSFX("LevelSelectScene");
    }

    /// <summary>
    /// 加载制作人员名单场景（EndLevel）。
    /// 调用 LoadSceneWithSFX 传入场景名 "EndLevel" 进行加载。
    /// 无在组件面板中显示的字段。
    /// </summary>
    public void LoadCredits()
    {
        LoadSceneWithSFX("EndLevel");
    }

    /// <summary>
    /// 加载关卡 Level_01。
    /// 调用 LoadSceneWithSFX 传入场景名 "Level_01" 进行加载。
    /// 无在组件面板中显示的字段。
    /// </summary>
    public void LoadLevel01()
    {
        LoadSceneWithSFX("Level_01");
    }

    /// <summary>
    /// 加载关卡 Level_001。
    /// 调用 LoadSceneWithSFX 传入场景名 "Level_001" 进行加载。
    /// 无在组件面板中显示的字段。
    /// </summary>
    public void LoadLevel001()
    {
        LoadSceneWithSFX("Level_001");
    }

    /// <summary>
    /// 加载关卡 Level_002。
    /// 调用 LoadSceneWithSFX 传入场景名 "Level_002" 进行加载。
    /// 无在组件面板中显示的字段。
    /// </summary>
    public void LoadLevel002()
    {
        LoadSceneWithSFX("Level_002");
    }

    /// <summary>
    /// 加载关卡 Level_003。
    /// 调用 LoadSceneWithSFX 传入场景名 "Level_003" 进行加载。
    /// 无在组件面板中显示的字段。
    /// </summary>
    public void LoadLevel003()
    {
        LoadSceneWithSFX("Level_003");
    }

    /// <summary>
    /// 加载关卡 Level_004。
    /// 调用 LoadSceneWithSFX 传入场景名 "Level_004" 进行加载。
    /// 无在组件面板中显示的字段。
    /// </summary>
    public void LoadLevel004()
    {
        LoadSceneWithSFX("Level_004");
    }

    /// <summary>
    /// 加载关卡 Level_005。
    /// 调用 LoadSceneWithSFX 传入场景名 "Level_005" 进行加载。
    /// 无在组件面板中显示的字段。
    /// </summary>
    public void LoadLevel005()
    {
        LoadSceneWithSFX("Level_005");
    }

    /// <summary>
    /// 加载关卡 Level_006。
    /// 调用 LoadSceneWithSFX 传入场景名 "Level_006" 进行加载。
    /// 无在组件面板中显示的字段。
    /// </summary>
    public void LoadLevel006()
    {
        LoadSceneWithSFX("Level_006");
    }

    /// <summary>
    /// 加载关卡 Level_02。
    /// 调用 LoadSceneWithSFX 传入场景名 "Level_02" 进行加载。
    /// 无在组件面板中显示的字段。
    /// </summary>
    public void LoadLevel02()
    {
        LoadSceneWithSFX("Level_02");
    }

    /// <summary>
    /// 根据传入的场景名称加载场景。
    /// 调用 LoadSceneWithSFX 传入参数 sceneName 进行加载。
    /// 无在组件面板中显示的字段。
    /// </summary>
    public void LoadSceneByName(string sceneName)
    {
        LoadSceneWithSFX(sceneName);
    }

    /// <summary>
    /// 内部通用加载方法，负责播放音效并加载场景。
    /// 先调用 SFXManager.Play 播放按钮点击和场景过渡音效，
    /// 再调用 SceneTransitionManager.LoadScene 加载指定场景。
    /// 无在组件面板中显示的字段。
    /// </summary>
    private void LoadSceneWithSFX(string sceneName)
    {
        SFXManager.Play(SFXType.ButtonClick);
        SFXManager.Play(SFXType.SceneTransition);
        SceneTransitionManager.LoadScene(sceneName);
    }

    /// <summary>
    /// 退出游戏。
    /// 在构建版本中调用 Application.Quit 退出程序；
    /// 在 Unity 编辑器中通过 UnityEditor.EditorApplication.isPlaying = false 停止播放模式。
    /// 无在组件面板中显示的字段。
    /// </summary>
    public void QuitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}