using UnityEngine;
// ========================================================================
// 文件功能：视角固定物体影子控制器
// 负责控制关联影子物体（shadowObject）的显示与隐藏。
// 支持通过 SetVisible 接口由外部控制可见性，并在 Awake 时根据配置决定是否初始隐藏。
// 在组件面板中可配置影子物体引用和是否在唤醒时隐藏。
// ========================================================================
public class ViewFixableShadowController : MonoBehaviour
{
    [SerializeField] private GameObject shadowObject;
    [SerializeField] private bool hideOnAwake = true;

    private bool hasExternalVisibilityRequest;

    // ========================================================================
    /// <summary>
    /// Unity 生命周期函数 Awake，在脚本实例加载时调用。
    /// 若未在面板指定 shadowObject，则默认使用当前游戏对象（gameObject）。
    /// 当 hideOnAwake 为 true 且尚未接收到外部可见性请求时，调用 SetVisible(false)，
    /// 通过 GameObject.SetActive(false) 隐藏影子对象。
    /// 在组件面板中可显示的字段：shadowObject（影子物体）、hideOnAwake（唤醒时隐藏）。
    /// </summary>
    private void Awake()
    {
        if (shadowObject == null)
        {
            shadowObject = gameObject;
        }

        if (hideOnAwake && !hasExternalVisibilityRequest)
        {
            SetVisible(false);
        }
    }

    // ========================================================================
    /// <summary>
    /// 公开方法，设置影子物体的可见性。
    /// 标记已收到外部可见性请求，避免被 Awake 中的自动隐藏覆盖。
    /// 若 shadowObject 为空则回退到自身 gameObject，
    /// 然后调用 shadowObject.SetActive(visible) 实现显示或隐藏。
    /// 在组件面板中可显示的字段：shadowObject、hideOnAwake。
    /// </summary>
    public void SetVisible(bool visible)
    {
        hasExternalVisibilityRequest = true;

        if (shadowObject == null)
        {
            shadowObject = gameObject;
        }

        shadowObject.SetActive(visible);
    }
}
