// ========================================================================
// 文件功能：文本提示事件触发器
// 提供可通过 UnityEvent 等调用的公开方法，用于触发指定类型的文本提示事件
// 或直接显示自定义文本信息。内部调用 TextPromptManager 的静态方法实现。
// 在组件面板中可配置触发的事件类型。
// ========================================================================

using UnityEngine;

public class TextPromptEventTrigger : MonoBehaviour
{
    [SerializeField] private TextPromptEvent eventType = TextPromptEvent.Custom1;

    /// <summary>
    /// 触发在组件面板中配置的事件类型对应的文本提示。
    /// 通过调用 TextPromptManager.Show(eventType) 静态方法，显示与该事件关联的预设文本。
    /// 在组件面板中可显示的字段：eventType（事件类型）。
    /// </summary>
    public void Show()
    {
        TextPromptManager.Show(eventType);
    }

    /// <summary>
    /// 显示自定义文本提示。
    /// 通过调用 TextPromptManager.ShowMessage(message) 静态方法，直接传入字符串显示。
    /// 无在组件面板中额外显示的字段。
    /// </summary>
    public void ShowMessage(string message)
    {
        TextPromptManager.ShowMessage(message);
    }
}