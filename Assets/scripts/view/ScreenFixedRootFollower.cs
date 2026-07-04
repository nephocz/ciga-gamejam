using UnityEngine;

public class ScreenFixedRootFollower : MonoBehaviour
{
    [SerializeField] private Camera targetCamera;
    [SerializeField] private float worldZ = 0f;
    [SerializeField] private bool followCameraRotation = true;

    private void Awake()
    {
        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }
    }

    private void LateUpdate()
    {
        if (targetCamera == null)
        {
            return;
        }

        Vector3 cameraPosition = targetCamera.transform.position;
        cameraPosition.z = worldZ;

        transform.position = cameraPosition;

        if (followCameraRotation)
        {
            transform.rotation = targetCamera.transform.rotation;
        }
    }
}