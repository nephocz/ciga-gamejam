// ========================================================================
// 文件功能：地图变换控制器
// 负责控制地图的移动与旋转（平移功能暂时禁用），并在平移时同步移动玩家，
// 旋转时若存在固定物体则以其为轴心进行旋转。通过 Input System 读取键盘 Q/E 键控制旋转，
// 在 FixedUpdate 中调用 Rigidbody2D 的 MovePosition 和 MoveRotation 实现物理驱动的变换。
// 在组件面板中可配置模式控制器引用、玩家刚体引用、地图移动速度、地图旋转速度。
// ========================================================================

using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class MapTransformController : MonoBehaviour
{
    [SerializeField] private GameModeController modeController;
    [SerializeField] private Rigidbody2D playerRigidbody;

    [Header("Map Move")]
    [SerializeField] private float moveSpeed = 3f;

    [Header("Map Rotate")]
    [SerializeField] private float rotateSpeed = 90f;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private float rotateInput;

    /// <summary>
    /// Unity 生命周期函数 Awake，在脚本实例加载时调用。
    /// 通过 GetComponent&lt;Rigidbody2D&gt;() 获取自身刚体用于移动和旋转；
    /// 若未在面板指定 modeController，则使用 FindAnyObjectByType&lt;GameModeController&gt;() 自动查找；
    /// 若未指定 playerRigidbody，则通过 FindAnyObjectByType&lt;PlayerMoveNoJump&gt;() 查找玩家并获取其 Rigidbody2D 组件。
    /// 本类在组件面板中可显示的字段：modeController（模式控制器）、playerRigidbody（玩家刚体）、
    /// moveSpeed（移动速度）、rotateSpeed（旋转速度）。
    /// </summary>
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (modeController == null)
        {
            modeController = FindAnyObjectByType<GameModeController>();
        }

        if (playerRigidbody == null)
        {
            PlayerMoveNoJump player = FindAnyObjectByType<PlayerMoveNoJump>();

            if (player != null)
            {
                playerRigidbody = player.GetComponent<Rigidbody2D>();
            }
        }
    }

    /// <summary>
    /// Unity 生命周期函数 Update，每帧调用。
    /// 根据当前模式允许时，使用 UnityEngine.InputSystem.Keyboard.current 读取键盘 Q/E 键状态以设置旋转输入值，
    /// 平移输入暂时被注释禁用。若 modeController 不存在或非地图移动模式（IsMapMoveMode 为 false），则清零所有输入。
    /// 本类在组件面板中可显示的字段：modeController。
    /// </summary>
    private void Update()
    {
        moveInput = Vector2.zero;
        rotateInput = 0f;

        if (modeController == null || !modeController.IsMapMoveMode)
        {
            return;
        }

        Keyboard keyboard = Keyboard.current;

        if (keyboard == null)
        {
            return;
        }

        // Map translation input is temporarily disabled.
        // if (keyboard.wKey.isPressed)
        // {
        //     moveInput.y += 1f;
        // }
        //
        // if (keyboard.sKey.isPressed)
        // {
        //     moveInput.y -= 1f;
        // }
        //
        // if (keyboard.aKey.isPressed)
        // {
        //     moveInput.x -= 1f;
        // }
        //
        // if (keyboard.dKey.isPressed)
        // {
        //     moveInput.x += 1f;
        // }

        if (keyboard.qKey.isPressed)
        {
            rotateInput += 1f;
        }

        if (keyboard.eKey.isPressed)
        {
            rotateInput -= 1f;
        }
    }

    /// <summary>
    /// Unity 固定物理更新回调 FixedUpdate，按固定时间间隔执行。
    /// 根据 moveInput 和 moveSpeed、rotateInput 和 rotateSpeed，计算本帧的位置移动量和旋转量；
    /// 调用 Rigidbody2D.MovePosition 应用平移，并通过 MovePlayerWithHorizontalMapPan 同步移动玩家；
    /// 若旋转量不为零，则调用 RotateMap 处理旋转（可能绕固定物体轴心）。
    /// 本类在组件面板中可显示的字段：moveSpeed、rotateSpeed、playerRigidbody。
    /// </summary>
    private void FixedUpdate()
    {
        Vector2 movement = moveInput.normalized * moveSpeed * Time.fixedDeltaTime;
        Vector2 nextPosition = rb.position + movement;

        float rotationAmount = rotateInput * rotateSpeed * Time.fixedDeltaTime;

        rb.MovePosition(nextPosition);
        MovePlayerWithHorizontalMapPan(movement.x);

        if (!Mathf.Approximately(rotationAmount, 0f))
        {
            RotateMap(nextPosition, rotationAmount);
        }
    }

    /// <summary>
    /// 执行地图旋转，可选择绕当前固定物体的轴心旋转。
    /// 若 ViewFixableObject.CurrentFixedObject 不为空，通过 RotatePointAroundPivot 计算绕轴心旋转后的位置，
    /// 并再次调用 Rigidbody2D.MovePosition 修正位置，使旋转围绕固定点进行；
    /// 最后调用 Rigidbody2D.MoveRotation 应用角度增量。
    /// </summary>
    private void RotateMap(Vector2 mapPositionAfterPan, float rotationAmount)
    {
        ViewFixableObject fixedObject = ViewFixableObject.CurrentFixedObject;

        if (fixedObject != null)
        {
            Vector2 nextPosition = RotatePointAroundPivot(mapPositionAfterPan, fixedObject.PivotPosition, rotationAmount);
            rb.MovePosition(nextPosition);
        }

        rb.MoveRotation(rb.rotation + rotationAmount);
    }

    /// <summary>
    /// 工具方法：计算一个点绕另一个点旋转指定角度后的新位置。
    /// 使用 Mathf.Deg2Rad 将角度转为弧度，利用三角函数计算旋转后的偏移向量，返回绕 pivot 旋转后的坐标点。
    /// </summary>
    private Vector2 RotatePointAroundPivot(Vector2 point, Vector2 pivot, float angleDegrees)
    {
        float radians = angleDegrees * Mathf.Deg2Rad;
        Vector2 offset = point - pivot;
        float sin = Mathf.Sin(radians);
        float cos = Mathf.Cos(radians);

        Vector2 rotatedOffset = new Vector2(
            offset.x * cos - offset.y * sin,
            offset.x * sin + offset.y * cos
        );

        return pivot + rotatedOffset;
    }

    /// <summary>
    /// 在地图水平平移时同步移动玩家，使玩家相对于地图保持位置不变。
    /// 若 playerRigidbody 存在且水平移动量不为零，则通过 Rigidbody2D.MovePosition 将玩家向右平移相同水平距离。
    /// 本类在组件面板中可显示的字段：playerRigidbody。
    /// </summary>
    private void MovePlayerWithHorizontalMapPan(float horizontalMovement)
    {
        if (playerRigidbody == null || Mathf.Approximately(horizontalMovement, 0f))
        {
            return;
        }

        Vector2 playerNextPosition = playerRigidbody.position + Vector2.right * horizontalMovement;
        playerRigidbody.MovePosition(playerNextPosition);
    }
}