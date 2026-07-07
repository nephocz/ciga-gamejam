// ========================================================================
// 文件功能：2D 碰撞层级配置器
// 负责在脚本唤醒时根据面板指定的层级名称，通过 Physics2D API 配置层级间碰撞忽略规则，
// 使玩家与地图实体、玩家与可修复物体之间允许碰撞，而地图实体与可修复物体之间忽略碰撞。
// 在组件面板中可配置玩家、地图实体、可修复物体的层级名称。
// ========================================================================

using UnityEngine;

public class CollisionLayerConfigurator2D : MonoBehaviour
{
    [SerializeField] private string playerLayerName = "Player";
    [SerializeField] private string mapSolidLayerName = "MapSolid";
    [SerializeField] private string fixableLayerName = "Fixable";

    /// <summary>
    /// Unity 生命周期函数 Awake，在脚本实例加载时调用。
    /// 通过 LayerMask.NameToLayer 将面板配置的层级名称转换为层级索引，并检查有效性；
    /// 使用 Physics2D.IgnoreLayerCollision 设置玩家与地图实体、玩家与可修复物体之间允许碰撞（false），
    /// 设置地图实体与可修复物体之间忽略碰撞（true），实现所需的碰撞过滤规则。
    /// 在组件面板中可显示的字段：playerLayerName（玩家层级名）、mapSolidLayerName（地图实体层级名）、
    /// fixableLayerName（可修复物体层级名）。
    /// </summary>
    private void Awake()
    {
        int playerLayer = LayerMask.NameToLayer(playerLayerName);
        int mapSolidLayer = LayerMask.NameToLayer(mapSolidLayerName);
        int fixableLayer = LayerMask.NameToLayer(fixableLayerName);

        if (playerLayer == -1)
        {
            Debug.LogError($"没有找到 Layer: {playerLayerName}");
            return;
        }

        if (mapSolidLayer == -1)
        {
            Debug.LogError($"没有找到 Layer: {mapSolidLayerName}");
            return;
        }

        if (fixableLayer == -1)
        {
            Debug.LogError($"没有找到 Layer: {fixableLayerName}");
            return;
        }

        Physics2D.IgnoreLayerCollision(playerLayer, mapSolidLayer, false);
        Physics2D.IgnoreLayerCollision(playerLayer, fixableLayer, false);

        Physics2D.IgnoreLayerCollision(mapSolidLayer, fixableLayer, true);
    }
}