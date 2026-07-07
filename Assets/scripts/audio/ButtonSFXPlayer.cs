// ========================================================================
// 文件功能：按钮音效播放器
// 提供可通过 UI 按钮事件调用的公开方法，用于播放按钮点击音效。
// 内部调用 SFXManager.Play 静态方法播放预设的 ButtonClick 音效。
// 本类无在组件面板中可配置的字段。
// ========================================================================

using UnityEngine;

public class ButtonSFXPlayer : MonoBehaviour
{
    /// <summary>
    /// 公开方法：播放按钮点击音效。
    /// 通过调用 SFXManager.Play(SFXType.ButtonClick) 静态方法触发音效。
    /// 无在组件面板中显示的字段。
    /// </summary>
    public void PlayButtonClick()
    {
        SFXManager.Play(SFXType.ButtonClick);
    }
}