using UnityEngine;

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