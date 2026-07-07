// ========================================================================
// 文件功能：游戏分辨率控制器
// 在构建版本中，于场景加载前自动设置游戏窗口分辨率（1920x1080 窗口化），
// 确保游戏运行在目标分辨率下。使用 Unity 的 RuntimeInitializeOnLoadMethod 特性
// 在运行时早期执行，仅在非编辑器环境中生效。
// 本类为静态类，无在组件面板中显示的字段。
// ========================================================================

using UnityEngine;

public static class GameResolutionController
{
    private const int TargetWidth = 1920;
    private const int TargetHeight = 1080;

    /// <summary>
    /// 运行时初始化方法，在场景加载前自动调用。
    /// 使用 [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)] 特性标记。
    /// 仅在非编辑器环境下，通过 Screen.SetResolution 将游戏分辨率设为 1920x1080 窗口化模式。
    /// 无在组件面板中显示的字段。
    /// </summary>
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void ApplyBuildResolution()
    {
#if !UNITY_EDITOR
        Screen.SetResolution(TargetWidth, TargetHeight, FullScreenMode.Windowed);
#endif
    }
}