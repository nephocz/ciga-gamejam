using UnityEngine;
using UnityEngine.InputSystem;
// ========================================================================
// 文件功能：游戏模式控制器
// 管理游戏当前的操控模式（玩家移动模式 / 地图移动模式），通过 Tab 键切换模式，
// 并提供属性供其他脚本查询当前模式。支持在面板中设置初始模式和调试日志开关。
// ========================================================================
public enum ControlMode
{
    PlayerMove,
    MapMove
}

public class GameModeController : MonoBehaviour
{
    [SerializeField] private ControlMode startMode = ControlMode.PlayerMove;
    [SerializeField] private bool showDebugLog = true;

    public ControlMode CurrentMode { get; private set; }

    // ——修改开始—— 始终允许玩家移动，不再受模式限制
    public bool IsPlayerMoveMode => true;
    // ——修改结束——
    // ——修改开始—— 始终允许地图移动，不再受模式限制
    public bool IsMapMoveMode => true;
    // ——修改结束——

    /// <summary>
    /// Unity 生命周期函数 Awake，在脚本实例加载时调用。
    /// 将当前模式 CurrentMode 设为面板配置的 startMode 初始模式，
    /// 并调用 PrintMode 输出初始模式的调试日志。
    /// 在组件面板中可显示的字段：startMode（初始模式）、showDebugLog（显示调试日志）。
    /// </summary>
    private void Awake()
    {
        CurrentMode = startMode;
        PrintMode();
    }

    /// <summary>
    /// Unity 生命周期函数 Update，每帧调用。
    /// 原用于检测 Tab 键切换模式，现因需要同时操控玩家和地图，已移除切换逻辑。
    /// </summary>
    private void Update()
    {
        // ——修改开始—— 取消 Tab 键切换模式，玩家和地图现在同时操控，故注释掉切换逻辑
        // Keyboard keyboard = Keyboard.current;
        // if (keyboard == null)
        // {
        //     return;
        // }
        // if (keyboard.tabKey.wasPressedThisFrame)
        // {
        //     ToggleMode();
        // }
        // ——修改结束——
    }

    /// <summary>
    /// 切换当前模式（公开方法）。
    /// 保留方法但不再被 Update 调用，调用后不影响实际操控（IsPlayerMoveMode 和 IsMapMoveMode 永远为 true）。
    /// </summary>
    public void ToggleMode()
    {
        if (CurrentMode == ControlMode.PlayerMove)
        {
            CurrentMode = ControlMode.MapMove;
        }
        else
        {
            CurrentMode = ControlMode.PlayerMove;
        }

        PrintMode();
    }

    /// <summary>
    /// 输出当前模式的调试日志（私有方法）。
    /// 当面板中 showDebugLog 为 false 时直接返回，不输出任何内容；
    /// 否则通过 Debug.Log 在控制台打印当前模式及操作提示信息。
    /// 在组件面板中可显示的字段：showDebugLog。
    /// </summary>
    private void PrintMode()
    {
        if (!showDebugLog)
        {
            return;
        }

        if (CurrentMode == ControlMode.PlayerMove)
        {
            Debug.Log("当前模式：人物移动模式。A / D 控制玩家。按 Tab 切换到地图移动模式。");
        }
        else
        {
            Debug.Log("当前模式：地图移动模式。WASD 控制地图平移，Q / E 控制地图旋转，鼠标点击固定物体。");
        }
    }
}