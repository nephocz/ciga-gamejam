using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class ViewFixableObject : MonoBehaviour
{
    public static ViewFixableObject CurrentFixedObject { get; private set; }

    [SerializeField] private Transform mapRoot;
    [SerializeField] private Transform screenFixedRoot;

    [Header("Visual")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite mapSprite;
    [SerializeField] private Sprite fixedSprite;

    [SerializeField] private bool isFixedToView;

    private Rigidbody2D rb;

    public bool IsFixedToView => isFixedToView;
    public Vector2 PivotPosition => transform.position;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        if (mapSprite == null && spriteRenderer != null)
        {
            mapSprite = spriteRenderer.sprite;
        }

        RefreshVisual();
    }

    public void Initialize(Transform mapRootTransform, Transform screenFixedRootTransform)
    {
        mapRoot = mapRootTransform;
        screenFixedRoot = screenFixedRootTransform;
    }

    public void ToggleFixedState()
    {
        if (isFixedToView)
        {
            UnfixFromView();
        }
        else
        {
            FixToView();
        }
    }

    public void FixToView()
    {
        if (screenFixedRoot == null)
        {
            Debug.LogWarning($"{name}: ScreenFixedRoot 没有设置。");
            return;
        }

        if (CurrentFixedObject != null && CurrentFixedObject != this)
        {
            CurrentFixedObject.UnfixFromView();
        }

        transform.SetParent(screenFixedRoot, true);
        isFixedToView = true;
        CurrentFixedObject = this;

        RefreshVisual();
        StopPhysicsMotion();
        Physics2D.SyncTransforms();

        Debug.Log($"{name} 已固定到视角。");
    }

    public void UnfixFromView()
    {
        if (mapRoot == null)
        {
            Debug.LogWarning($"{name}: MapRoot 没有设置。");
            return;
        }

        transform.SetParent(mapRoot, true);
        isFixedToView = false;

        if (CurrentFixedObject == this)
        {
            CurrentFixedObject = null;
        }

        RefreshVisual();
        StopPhysicsMotion();
        Physics2D.SyncTransforms();

        Debug.Log($"{name} 已取消固定，重新跟随地图。");
    }

    private void StopPhysicsMotion()
    {
        if (rb == null)
        {
            return;
        }

        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
    }

    private void RefreshVisual()
    {
        if (spriteRenderer == null)
        {
            return;
        }

        Sprite targetSprite = isFixedToView ? fixedSprite : mapSprite;

        if (targetSprite != null)
        {
            spriteRenderer.sprite = targetSprite;
        }
    }

    private void OnDisable()
    {
        if (CurrentFixedObject == this)
        {
            CurrentFixedObject = null;
        }
    }
}
