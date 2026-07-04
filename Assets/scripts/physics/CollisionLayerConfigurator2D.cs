using UnityEngine;

public class CollisionLayerConfigurator2D : MonoBehaviour
{
    [SerializeField] private string playerLayerName = "Player";
    [SerializeField] private string mapSolidLayerName = "MapSolid";
    [SerializeField] private string fixableLayerName = "Fixable";

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