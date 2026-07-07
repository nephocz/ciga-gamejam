using UnityEngine;
// ========================================================================
// 文件功能：视角固定物体（可修复物体）
// 负责管理场景中可被玩家固定到屏幕空间的交互物体。通过切换父物体实现物体
// 固定到屏幕（screenFixedRoot）或跟随地图（mapRoot），同步更新视觉精灵与影子，
// 并维护全局的当前固定物体引用。在组件面板中可配置地图/屏幕根节点、精灵、固定状态等。
// ========================================================================
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
<<<<<<< Updated upstream
=======
    [SerializeField] private ViewFixableShadowController shadowController;
>>>>>>> Stashed changes

    [SerializeField] private bool isFixedToView;

    private Rigidbody2D rb;

    public bool IsFixedToView => isFixedToView;
    public Vector2 PivotPosition => transform.position;

    // ========================================================================
    /// <summary>
    /// Unity 生命周期函数 Awake，在脚本实例加载时调用。
    /// 通过 GetComponent&lt;Rigidbody2D&gt;() 获取自身刚体用于后续物理状态控制；
    /// 若未在面板指定 SpriteRenderer，则通过 GetComponent&lt;SpriteRenderer&gt;() 获取；
    /// 若未指定 mapSprite 但已有 SpriteRenderer，则将其当前精灵作为地图精灵；
    /// 最后调用 RefreshVisual 和 RefreshShadow 同步视觉状态。
    /// 可在组件面板中显示的字段：spriteRenderer、mapSprite、fixedSprite、shadowController、isFixedToView。
    /// </summary>
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
<<<<<<< Updated upstream

        RefreshVisual();
    }
=======
>>>>>>> Stashed changes

        RefreshVisual();
        RefreshShadow();
    }

    // ========================================================================
    /// <summary>
    /// 公开初始化方法，由外部控制器调用以设置根节点引用。
    /// 接收地图根物体和屏幕固定根物体，保存到对应字段；
    /// 若当前未固定且地图根物体存在，则通过 Transform.SetParent(mapRoot, true) 将物体挂载到地图下，
    /// 并设置刚体为运动学模式（rb.isKinematic = true），使其跟随父物体移动而不受物理力影响。
    /// 可在组件面板中显示的字段：mapRoot、screenFixedRoot、isFixedToView。
    /// </summary>
    public void Initialize(Transform mapRootTransform, Transform screenFixedRootTransform)
    {
        mapRoot = mapRootTransform;
        screenFixedRoot = screenFixedRootTransform;
        // === 新增开始 ===
        // 确保初始时物体跟随地图移动，且刚体为运动学（不受物理力影响，由父物体带动）
        if (!isFixedToView && mapRoot != null)
        {
            transform.SetParent(mapRoot, true);
            if (rb != null)
                rb.isKinematic = true;   // 关闭物理模拟，完全由 Transform 控制位置
        }
        // === 新增结束 ===
    }

    // ========================================================================
    /// <summary>
    /// 切换固定状态的公开方法。
    /// 根据当前 isFixedToView 状态，若已固定则调用 UnfixFromView 取消固定，
    /// 否则调用 FixToView 执行固定。
    /// </summary>
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

    // ========================================================================
    /// <summary>
    /// 将物体固定到视角。
    /// 如果当前已有其他物体固定，则先调用其 UnfixFromView 解除；
    /// 使用 Transform.SetParent(screenFixedRoot, true) 将父物体设为屏幕固定根，
    /// 设置 isFixedToView = true 并更新全局当前固定物体；
    /// 调用 RefreshVisual 和 RefreshShadow 刷新视觉，通过 StopPhysicsMotion 清零物理速度，
    /// 使用 Physics2D.SyncTransforms 同步物理变换，并调用 SFXManager.Play 和 TextPromptManager.Show 播放音效与提示。
    /// 可在组件面板中显示的字段：screenFixedRoot、fixedSprite、shadowController。
    /// </summary>
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
<<<<<<< Updated upstream
        StopPhysicsMotion();
        Physics2D.SyncTransforms();
=======
        RefreshShadow();
        StopPhysicsMotion();
        Physics2D.SyncTransforms();
        SFXManager.Play(SFXType.AnchorObject);
        TextPromptManager.Show(TextPromptEvent.AnchorObject);
>>>>>>> Stashed changes

        Debug.Log($"{name} 已固定到视角。");
    }

    // ========================================================================
    /// <summary>
    /// 取消固定，使物体重新跟随地图移动。
    /// 通过 Transform.SetParent(mapRoot, true) 将父物体切回地图根；
    /// 清除固定标志，若当前全局固定物体为自身则置空；
    /// 更新视觉、停止物理速度、同步变换，并播放取消固定音效与提示。
    /// 可在组件面板中显示的字段：mapRoot、mapSprite、shadowController。
    /// </summary>
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
<<<<<<< Updated upstream
        StopPhysicsMotion();
        Physics2D.SyncTransforms();
=======
        RefreshShadow();
        StopPhysicsMotion();
        Physics2D.SyncTransforms();
        SFXManager.Play(SFXType.UnanchorObject);
        TextPromptManager.Show(TextPromptEvent.UnanchorObject);
>>>>>>> Stashed changes

        Debug.Log($"{name} 已取消固定，重新跟随地图。");
    }

    // ========================================================================    
    /// <summary>
    /// 停止物体所有物理运动。
    /// 通过 Rigidbody2D 的 linearVelocity 和 angularVelocity 属性将速度和角速度清零，
    /// 确保切换父物体时不会残留运动。
    /// </summary>
    private void StopPhysicsMotion()
    {
        if (rb == null)
        {
            return;
        }

        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
    }

<<<<<<< Updated upstream
=======
    // ========================================================================
    /// <summary>
    /// 刷新主精灵显示。
    /// 根据 isFixedToView 状态选择 mapSprite 或 fixedSprite 赋给 SpriteRenderer.sprite。
    /// 可在组件面板中显示的字段：spriteRenderer、mapSprite、fixedSprite。
    /// </summary>
>>>>>>> Stashed changes
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

<<<<<<< Updated upstream
=======
    // ========================================================================
    /// <summary>
    /// 刷新影子控制器的可见性。
    /// 若 shadowController 不为空，则调用其 SetVisible 方法，传入当前是否固定，
    /// 使得固定时显示影子，跟随地图时隐藏影子。
    /// 可在组件面板中显示的字段：shadowController。
    /// </summary>
    private void RefreshShadow()
    {
        if (shadowController == null)
        {
            return;
        }

        shadowController.SetVisible(isFixedToView);
    }

    // ========================================================================
    /// <summary>
    /// Unity 生命周期函数 OnDisable，当组件禁用或物体销毁时调用。
    /// 若当前全局固定物体为自身则置空，避免空引用；同时将影子控制器设为不可见。
    /// </summary>
>>>>>>> Stashed changes
    private void OnDisable()
    {
        if (CurrentFixedObject == this)
        {
            CurrentFixedObject = null;
        }
<<<<<<< Updated upstream
=======

        if (shadowController != null)
        {
            shadowController.SetVisible(false);
        }
>>>>>>> Stashed changes
    }
}
