using UnityEngine;

public class ViewFixableShadowController : MonoBehaviour
{
    [SerializeField] private GameObject shadowObject;
    [SerializeField] private bool hideOnAwake = true;

    private void Awake()
    {
        if (shadowObject == null)
        {
            shadowObject = gameObject;
        }

        if (hideOnAwake)
        {
            SetVisible(false);
        }
    }

    public void SetVisible(bool visible)
    {
        if (shadowObject == null)
        {
            shadowObject = gameObject;
        }

        shadowObject.SetActive(visible);
    }
}
