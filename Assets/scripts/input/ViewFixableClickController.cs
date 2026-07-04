using UnityEngine;
using UnityEngine.InputSystem;

public class ViewFixableClickController : MonoBehaviour
{
    [SerializeField] private GameModeController modeController;

    [Header("References")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Transform mapRoot;
    [SerializeField] private Transform screenFixedRoot;

    [Header("Click")]
    [SerializeField] private LayerMask clickableLayerMask;

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

    private void InitializeFixableObjects()
    {
        ViewFixableObject[] fixableObjects = FindObjectsByType<ViewFixableObject>(FindObjectsSortMode.None);

        foreach (ViewFixableObject fixableObject in fixableObjects)
        {
            fixableObject.Initialize(mapRoot, screenFixedRoot);
        }
    }
}