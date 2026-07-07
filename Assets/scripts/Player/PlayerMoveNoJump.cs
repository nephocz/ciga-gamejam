using UnityEngine;
using UnityEngine.InputSystem;
// ========================================================================
// 文件功能：玩家移动控制器（无跳跃版）
// 负责根据玩家键盘输入控制角色的水平移动，结合 GameModeController 判断是否允许移动，
// 并通过 SFXManager 管理移动音效的循环播放与停止。
// 在组件面板中可配置移动速度、跳跃力（暂未启用）及游戏模式控制器引用。
// ========================================================================
public class PlayerMoveNoJump : MonoBehaviour
{
    [SerializeField] private GameModeController modeController;

    public float moveSpeed = 5f;
    public float jumpForce = 8f;

    private Rigidbody2D rb;
    private bool isGrounded = true;
    private float moveInput;

    // ========================================================================
    /// <summary>
    /// Unity 生命周期函数 Start，在对象启用时调用。
    /// 通过 GetComponent&lt;Rigidbody2D&gt;() 获取自身刚体组件用于物理移动；
    /// 若 modeController 未在面板指定，则通过 FindAnyObjectByType&lt;GameModeController&gt;() 在场景中查找。
    /// 本类在组件面板中可显示的字段：modeController（模式控制器）、moveSpeed（移动速度）、jumpForce（跳跃力，当前未使用）。
    /// </summary>
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (modeController == null)
        {
            modeController = FindAnyObjectByType<GameModeController>();
        }
    }

    // ========================================================================
    /// <summary>
    /// Unity 生命周期函数 Update，每帧调用。
    /// 首先检查 modeController 的 IsPlayerMoveMode 属性，若当前非移动模式或键盘不可用，则将 moveInput 清零并停止移动音效；
    /// 若允许移动，使用 UnityEngine.InputSystem.Keyboard.current 检测 A/左箭头（向左）和 D/右箭头（向右）的按键状态，
    /// 设置 moveInput 为 -1 或 1，实现无跳跃的左右输入处理，并调用 RefreshMoveSFX 更新移动音效。
    /// 本类在组件面板中可显示的字段：modeController、moveSpeed、jumpForce。
    /// </summary>
    void Update()
    {
        if (modeController != null && !modeController.IsPlayerMoveMode)
        {
            moveInput = 0f;
            return;
        }

        Keyboard keyboard = Keyboard.current;

        if (keyboard == null)
        {
            moveInput = 0f;
            return;
        }

        moveInput = 0f;

        if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed)
        {
            moveInput = -1f;
        }

        if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed)
        {
            moveInput = 1f;
        }

        if ((keyboard.spaceKey.wasPressedThisFrame || keyboard.wKey.wasPressedThisFrame || keyboard.upArrowKey.wasPressedThisFrame) && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            isGrounded = false;
        }
    }

    // ========================================================================
    /// <summary>
    /// Unity 固定物理更新回调 FixedUpdate，按固定时间间隔调用。
    /// 通过直接设置 Rigidbody2D.linearVelocity，将水平速度设为 moveInput * moveSpeed，垂直速度保持不变，实现物理驱动的水平移动。
    /// 本类在组件面板中可显示的字段：moveSpeed。
    /// </summary>
    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
    }

    // ========================================================================
    /// <summary>
    /// Unity 碰撞开始回调 OnCollisionEnter2D，当与其他 2D 碰撞体发生碰撞时调用。
    /// 将 isGrounded 设为 true 表示玩家着地，并调用 RefreshMoveSFX 尝试恢复移动音效。
    /// 本类在组件面板中可显示的字段：无直接使用，但 isGrounded 状态受碰撞影响。
    /// </summary>
    void OnCollisionEnter2D(Collision2D collision)
    {
        isGrounded = true;
<<<<<<< Updated upstream
=======
        RefreshMoveSFX();
    }

    // ========================================================================
    /// <summary>
    /// 刷新移动音效状态（私有方法）。
    /// 通过检查 isGrounded 与 moveInput 的绝对值是否大于 0.01 判断是否应播放移动音效；
    /// 若应播放且当前未播放，则调用 SFXManager.StartLoop(SFXType.PlayerMove) 开始循环音效；
    /// 若不应播放且正在播放，则调用 StopMoveSFX 停止音效。
    /// </summary>
    private void RefreshMoveSFX()
    {
        bool shouldPlay = isGrounded && Mathf.Abs(moveInput) > 0.01f;

        if (shouldPlay && !isPlayingMoveSFX)
        {
            SFXManager.StartLoop(SFXType.PlayerMove);
            isPlayingMoveSFX = true;
        }

        if (!shouldPlay && isPlayingMoveSFX)
        {
            StopMoveSFX();
        }
    }

    // ========================================================================
    /// <summary>
    /// 停止移动音效（私有方法）。
    /// 若当前正在播放移动音效，则调用 SFXManager.StopLoop(SFXType.PlayerMove) 停止循环音效，
    /// 并将 isPlayingMoveSFX 标志复位。
    /// </summary>
    private void StopMoveSFX()
    {
        if (!isPlayingMoveSFX)
        {
            return;
        }

        SFXManager.StopLoop(SFXType.PlayerMove);
        isPlayingMoveSFX = false;
    }

    // ========================================================================
    /// <summary>
    /// Unity 生命周期函数 OnDisable，当组件被禁用或对象被销毁时调用。
    /// 确保停止可能正在播放的移动循环音效，避免音效泄露。
    /// </summary>
    private void OnDisable()
    {
        StopMoveSFX();
>>>>>>> Stashed changes
    }
}
