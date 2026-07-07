using UnityEngine;
// ========================================================================
// 文件功能：玩家摩擦力控制器
// 根据当前地图（地板）的倾斜角度动态切换玩家的物理材质，以模拟在陡坡上打滑的效果。
// 在组件面板中可配置倾斜阈值、正常/低摩擦材质，并支持手动指定地图刚体或自动查找。
// ========================================================================
public class PlayerFrictionController : MonoBehaviour
{
    [Header("倾斜阈值")]
    [Tooltip("当地板倾斜角度超过此值时，切换为低摩擦材质")]
    [SerializeField] private float slideAngleThreshold = 30f;

    [Header("物理材质")]
    [Tooltip("正常摩擦材质（倾斜角小于等于阈值时使用）")]
    [SerializeField] private PhysicsMaterial2D normalFrictionMaterial;
    [Tooltip("低摩擦/无摩擦材质（倾斜角大于阈值时使用）")]
    [SerializeField] private PhysicsMaterial2D lowFrictionMaterial;

    [Header("引用")]
    [Tooltip("地图的刚体，用于获取旋转角度。如果留空则自动从 MapTransformController 获取")]
    [SerializeField] private Rigidbody2D mapRigidbody;

    private Collider2D playerCollider;
    // ========================================================================
    /// <summary>
    /// Unity 生命周期函数 Start，在脚本实例启用时调用。
    /// 通过 GetComponent&lt;Collider2D&gt;() 获取玩家自身的碰撞体，用于后续修改物理材质。
    /// 若 mapRigidbody 未在面板指定，则使用 FindObjectOfType&lt;MapTransformController&gt;() 查找场景中的地图控制器并获取其 Rigidbody2D。
    /// 若未在面板指定 normalFrictionMaterial，则保留玩家碰撞体当前的 sharedMaterial 作为正常材质。
    /// 可在组件面板中显示的字段：slideAngleThreshold（倾斜阈值）、normalFrictionMaterial（正常材质）、
    /// lowFrictionMaterial（低摩擦材质）、mapRigidbody（地图刚体引用）。
    /// </summary>
    private void Start()
    {
        playerCollider = GetComponent<Collider2D>();
        if (playerCollider == null)
        {
            Debug.LogError("PlayerFrictionController 必须挂载在带有 Collider2D 的对象上");
            return;
        }

        // 自动获取地图刚体
        if (mapRigidbody == null)
        {
            MapTransformController mapCtrl = FindObjectOfType<MapTransformController>();
            if (mapCtrl != null)
                mapRigidbody = mapCtrl.GetComponent<Rigidbody2D>();
        }

        // 如果正常材质未指定，则保留玩家当前的物理材质
        if (normalFrictionMaterial == null)
            normalFrictionMaterial = playerCollider.sharedMaterial;
    }
    // ========================================================================
    /// <summary>
    /// Unity 生命周期函数 Update，每帧调用。
    /// 通过 mapRigidbody.rotation 获取地图刚体的旋转角度，计算 0～90 度的有效倾斜角。
    /// 利用 Mathf.Abs 和取模运算将角度标准化；若倾斜角大于 slideAngleThreshold，则使用 playerCollider.sharedMaterial 切换到 lowFrictionMaterial；
    /// 否则切换回 normalFrictionMaterial。材质仅在需要改变时才重新赋值。
    /// 可在组件面板中显示的字段：slideAngleThreshold、normalFrictionMaterial、lowFrictionMaterial、mapRigidbody。
    /// </summary>
    private void Update()
    {
        if (playerCollider == null || mapRigidbody == null)
            return;

        // 计算地板的实际倾斜角（0°～90°）
        float rawAngle = mapRigidbody.rotation % 180f;
        float tiltAngle = Mathf.Abs(rawAngle);
        if (tiltAngle > 90f)
            tiltAngle = 180f - tiltAngle;

        // 根据倾斜角切换碰撞材质
        if (tiltAngle > slideAngleThreshold)
        {
            if (lowFrictionMaterial != null && playerCollider.sharedMaterial != lowFrictionMaterial)
                playerCollider.sharedMaterial = lowFrictionMaterial;
        }
        else
        {
            if (normalFrictionMaterial != null && playerCollider.sharedMaterial != normalFrictionMaterial)
                playerCollider.sharedMaterial = normalFrictionMaterial;
        }
    }
}