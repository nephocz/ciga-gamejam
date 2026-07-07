// ========================================================================
// 文件功能：屏幕固定根跟随器
// 使挂载物体始终与指定相机保持相同位置（并可选择相同旋转），常用于将 UI 或特效固定在屏幕空间原点。
// 在组件面板中可配置目标相机、世界 Z 轴偏移、是否跟随相机旋转。
// ========================================================================

using UnityEngine;

public class ScreenFixedRootFollower : MonoBehaviour
{
    [SerializeField] private Camera targetCamera;
    [SerializeField] private float worldZ = 0f;
    [SerializeField] private bool followCameraRotation = true;

    /// <summary>
    /// Unity 生命周期函数 Awake，在脚本实例加载时调用。
    /// 若 targetCamera 未在面板中指定，则通过 Camera.main 自动获取主摄像机，
    /// 确保后续跟随逻辑有合法的相机引用。
    /// 在组件面板中可显示的字段：targetCamera（目标相机）、worldZ（世界 Z 坐标）、followCameraRotation（跟随旋转）。
    /// </summary>
    private void Awake()
    {
        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }
    }

    /// <summary>
    /// Unity 生命周期函数 LateUpdate，在 Update 之后每帧调用，保证在相机移动完成后更新。
    /// 获取 targetCamera.transform.position 作为自身位置，将 Z 轴强制设为 worldZ；
    /// 若 followCameraRotation 为 true，则同时将自身旋转同步为相机旋转，实现物体固定在屏幕空间。
    /// 在组件面板中可显示的字段：targetCamera、worldZ、followCameraRotation。
    /// </summary>
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