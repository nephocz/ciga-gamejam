using UnityEngine;

public class ViewFixableShadowController : MonoBehaviour
{
    [SerializeField] private GameObject shadowObject;
    [SerializeField] private bool hideOnAwake = true;

    private bool hasExternalVisibilityRequest;

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
