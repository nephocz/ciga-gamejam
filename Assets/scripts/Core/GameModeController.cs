using UnityEngine;
using UnityEngine.InputSystem;

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

    public bool IsPlayerMoveMode => CurrentMode == ControlMode.PlayerMove;
    public bool IsMapMoveMode => CurrentMode == ControlMode.MapMove;

    private void Awake()
    {
        CurrentMode = startMode;
        PrintMode();
    }

    private void Update()
    {
        Keyboard keyboard = Keyboard.current;

        if (keyboard == null)
        {
            return;
        }

        if (keyboard.tabKey.wasPressedThisFrame)
        {
            ToggleMode();
        }
    }

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