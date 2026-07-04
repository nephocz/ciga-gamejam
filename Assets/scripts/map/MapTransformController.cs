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

        if (keyboard.wKey.isPressed)
        {
            moveInput.y += 1f;
        }

        if (keyboard.sKey.isPressed)
        {
            moveInput.y -= 1f;
        }

        if (keyboard.aKey.isPressed)
        {
            moveInput.x -= 1f;
        }

        if (keyboard.dKey.isPressed)
        {
            moveInput.x += 1f;
        }

        if (keyboard.qKey.isPressed)
        {
            rotateInput += 1f;
        }

        if (keyboard.eKey.isPressed)
        {
            rotateInput -= 1f;
        }
    }

    private void FixedUpdate()
    {
        Vector2 movement = moveInput.normalized * moveSpeed * Time.fixedDeltaTime;
        Vector2 nextPosition = rb.position + movement;

        float rotationAmount = rotateInput * rotateSpeed * Time.fixedDeltaTime;
        float nextRotation = rb.rotation + rotationAmount;

        rb.MovePosition(nextPosition);
        MovePlayerWithHorizontalMapPan(movement.x);
        rb.MoveRotation(nextRotation);
    }

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
