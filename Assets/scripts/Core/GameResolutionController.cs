using UnityEngine;

public static class GameResolutionController
{
    private const int TargetWidth = 1920;
    private const int TargetHeight = 1080;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void ApplyBuildResolution()
    {
#if !UNITY_EDITOR
        Screen.SetResolution(TargetWidth, TargetHeight, FullScreenMode.Windowed);
#endif
    }
}
