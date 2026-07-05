using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMoveNoJump : MonoBehaviour
{
    [SerializeField] private GameModeController modeController;
    public float moveSpeed = 5f;
    public float jumpForce = 8f;

    private Rigidbody2D rb;
    private bool isGrounded = true;
    private float moveInput;
    private bool isPlayingMoveSFX;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (modeController == null)
        {
            modeController = FindAnyObjectByType<GameModeController>();
        }
    }

    void Update()
    {
        if (modeController != null && !modeController.IsPlayerMoveMode)
        {
            moveInput = 0f;
            StopMoveSFX();
            return;
        }

        Keyboard keyboard = Keyboard.current;

        if (keyboard == null)
        {
            moveInput = 0f;
            StopMoveSFX();
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

        // Jump input is temporarily disabled.
        // if ((keyboard.spaceKey.wasPressedThisFrame || keyboard.wKey.wasPressedThisFrame || keyboard.upArrowKey.wasPressedThisFrame) && isGrounded)
        // {
        //     rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        //     isGrounded = false;
        //     StopMoveSFX();
        // }

        RefreshMoveSFX();
    }

    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        isGrounded = true;
        RefreshMoveSFX();
    }

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

    private void StopMoveSFX()
    {
        if (!isPlayingMoveSFX)
        {
            return;
        }

        SFXManager.StopLoop(SFXType.PlayerMove);
        isPlayingMoveSFX = false;
    }

    private void OnDisable()
    {
        StopMoveSFX();
    }
}
