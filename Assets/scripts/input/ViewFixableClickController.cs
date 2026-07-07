using UnityEngine;
using UnityEngine.InputSystem;
// ========================================================================
// 文件功能：视角固定物体点击控制器
// 负责在"地图移动模式"下处理鼠标点击，检测是否点击了可修复的视角固定物体，
// 并切换其修复状态。同时负责初始化场景中所有 ViewFixableObject 的根节点引用。
// 通过 Input System 检测鼠标点击，使用物理射线检测点击位置的可修复物体，
// 并调用对应物体的 ToggleFixedState 方法。
// 在组件面板中可配置的字段：modeController（游戏模式控制器）、mainCamera（主摄像机）、
// mapRoot（地图根节点）、screenFixedRoot（屏幕固定根节点）、clickableLayerMask（可点击层遮罩）。
// ========================================================================
public class ViewFixableClickController : MonoBehaviour
{
    [SerializeField] private GameModeController modeController;

    [Header("References")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Transform mapRoot;
    [SerializeField] private Transform screenFixedRoot;

    [Header("Click")]
    [SerializeField] private LayerMask clickableLayerMask;
    // ========================================================================
    /// <summary>
    /// Unity 生命周期函数 Awake，在脚本实例加载时调用。
    /// 若未在面板指定 modeController，则通过 FindFirstObjectByType&lt;GameModeController&gt;() 自动查找；
    /// 若未指定 mainCamera，则使用 Camera.main 获取主摄像机；
    /// 最后调用 InitializeFixableObjects 初始化场景中所有可修复物体。
    /// 本类在组件面板中可显示的字段：modeController、mainCamera、mapRoot、screenFixedRoot、clickableLayerMask。
    /// </summary>
    private void Awake()
    {
        if (modeController == null)
        {
            modeController = FindFirstObjectByType<GameModeController>();
        }

        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        InitializeFixableObjects();
    }
    // ========================================================================
    /// <summary>
    /// Unity 生命周期函数 Update，每帧调用。
    /// 首先检查 modeController 的 IsMapMoveMode 属性，确保当前处于地图移动模式；
    /// 若条件满足，使用 UnityEngine.InputSystem.Mouse.current 获取鼠标设备，
    /// 通过 leftButton.wasPressedThisFrame 检测左键是否在本帧按下；
    /// 若按下则调用 TryToggleFixableObjectAtMousePosition 尝试切换鼠标位置的可修复物体。
    /// 本类在组件面板中可显示的字段：modeController。
    /// </summary>
    private void Update()
    {
        if (modeController == null || !modeController.IsMapMoveMode)
        {
            return;
        }

        Mouse mouse = Mouse.current;

        if (mouse == null)
        {
            return;
        }

        if (!mouse.leftButton.wasPressedThisFrame)
        {
            return;
        }

        TryToggleFixableObjectAtMousePosition();
    }
    // ========================================================================
    /// <summary>
    /// 尝试在鼠标位置切换可修复物体的固定状态。
    /// 使用 Mouse.current.position.ReadValue() 获取屏幕坐标，
    /// 通过 mainCamera.ScreenToWorldPoint 将屏幕坐标转换为世界坐标；
    /// 调用 Physics2D.OverlapPointAll 结合面板配置的 clickableLayerMask 进行物理检测，
    /// 在命中的碰撞体中查找 ViewFixableObject 组件并调用其 ToggleFixedState 方法。
    /// 本类在组件面板中可显示的字段：mainCamera、clickableLayerMask。
    /// </summary>
    private void TryToggleFixableObjectAtMousePosition()
    {
        if (mainCamera == null)
        {
            return;
        }

        Vector2 screenPosition = Mouse.current.position.ReadValue();

        Vector3 worldPosition3D = mainCamera.ScreenToWorldPoint(screenPosition);
        Vector2 worldPosition2D = new Vector2(worldPosition3D.x, worldPosition3D.y);

        Collider2D[] hits = Physics2D.OverlapPointAll(worldPosition2D, clickableLayerMask);

        if (hits == null || hits.Length == 0)
        {
            return;
        }

        foreach (Collider2D hit in hits)
        {
            ViewFixableObject fixableObject = hit.GetComponentInParent<ViewFixableObject>();

            if (fixableObject == null)
            {
                continue;
            }

            fixableObject.ToggleFixedState();
            return;
        }
    }
    // ========================================================================
    /// <summary>
    /// 初始化场景中所有的 ViewFixableObject 组件。
    /// 通过 FindObjectsByType&lt;ViewFixableObject&gt;(FindObjectsSortMode.None) 获取全部可修复物体实例，
    /// 并依次调用它们的 Initialize 方法，传入面板配置的 mapRoot（地图根节点）和 screenFixedRoot（屏幕固定根节点），
    /// 使每个可修复物体知道自己在两种坐标系下的父物体。
    /// 本类在组件面板中可显示的字段：mapRoot、screenFixedRoot。
    /// </summary>
    private void InitializeFixableObjects()
    {
        ViewFixableObject[] fixableObjects = FindObjectsByType<ViewFixableObject>(FindObjectsSortMode.None);

        foreach (ViewFixableObject fixableObject in fixableObjects)
        {
            fixableObject.Initialize(mapRoot, screenFixedRoot);
        }
    }
}