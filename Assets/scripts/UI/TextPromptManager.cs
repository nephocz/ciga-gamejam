// ========================================================================
// 文件功能：文本提示管理器
// 管理游戏中的全屏文本提示显示，支持根据预设事件类型展示单条或多条消息序列。
// 通过 CanvasGroup 控制淡入淡出，使用 TMP_Text 渲染文字，并可配置默认及每条消息的持续时间。
// 使用静态接口 Show(事件) 和 ShowMessage(自定义文本) 供其他脚本调用，并确保每个事件仅显示一次。
// 在组件面板中可配置提示文本组件、画布组、事件条目列表以及所有时间相关参数。
// ========================================================================

using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextPromptManager : MonoBehaviour
{
    [Serializable]
    private class PromptEntry
    {
        public TextPromptEvent eventType;
        [TextArea] public string message;
        public PromptMessage[] followingMessages;
        public float fadeInDuration = -1f;
        public float holdDuration = -1f;
        public float fadeOutDuration = -1f;
    }

    [Serializable]
    private class PromptMessage
    {
        [TextArea] public string message;
        public float fadeInDuration = -1f;
        public float holdDuration = -1f;
        public float fadeOutDuration = -1f;
    }

    [SerializeField] private TMP_Text promptText;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private PromptEntry[] promptEntries;
    [SerializeField, Min(0f)] private float defaultFadeInDuration = 0.35f;
    [SerializeField, Min(0f)] private float defaultHoldDuration = 1.6f;
    [SerializeField, Min(0f)] private float defaultFadeOutDuration = 0.45f;
    [SerializeField] private bool useUnscaledTime = true;
    [SerializeField] private bool showSceneStartOnStart;

    private static TextPromptManager current;
    private Coroutine showRoutine;
    private readonly HashSet<TextPromptEvent> shownEvents = new HashSet<TextPromptEvent>();

    /// <summary>
    /// Unity 生命周期函数 Awake，在脚本实例加载时调用。
    /// 将自身赋值给静态实例 current（单例模式）；若未在面板指定 promptText 则通过 GetComponentInChildren 查找；
    /// 若未指定 canvasGroup 则从 promptText 的父物体或自身获取；最后将 CanvasGroup 透明度初始化为 0。
    /// 在组件面板中可显示的字段：promptText、canvasGroup。
    /// </summary>
    private void Awake()
    {
        current = this;

        if (promptText == null)
        {
            promptText = GetComponentInChildren<TMP_Text>(true);
        }

        if (canvasGroup == null && promptText != null)
        {
            canvasGroup = promptText.GetComponentInParent<CanvasGroup>();
        }

        if (canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
        }
    }

    /// <summary>
    /// Unity 生命周期函数 Start，在首次 Update 前调用。
    /// 如果开启了 showSceneStartOnStart，则调用静态 Show 方法触发 SceneStart 事件提示。
    /// 在组件面板中可显示的字段：showSceneStartOnStart。
    /// </summary>
    private void Start()
    {
        if (showSceneStartOnStart)
        {
            Show(TextPromptEvent.SceneStart);
        }
    }

    /// <summary>
    /// Unity 生命周期函数 OnDestroy，当对象被销毁时调用。
    /// 如果当前静态实例指向自身，则清空引用，防止空指针。
    /// </summary>
    private void OnDestroy()
    {
        if (current == this)
        {
            current = null;
        }
    }

    /// <summary>
    /// 公开静态方法：按事件类型显示预设文本提示。
    /// 通过当前实例调用 ShowPrompt 方法，触发对应事件的消息序列。
    /// 无在组件面板中显示的字段（静态方法）。
    /// </summary>
    public static void Show(TextPromptEvent eventType)
    {
        if (current == null)
        {
            return;
        }

        current.ShowPrompt(eventType);
    }

    /// <summary>
    /// 公开静态方法：直接显示一条自定义文本提示。
    /// 调用 ShowMessageInstance 并传入默认持续时间（-1 表示使用默认值），不受事件单次限制。
    /// 无在组件面板中显示的字段。
    /// </summary>
    public static void ShowMessage(string message)
    {
        if (current == null)
        {
            return;
        }

        current.ShowMessageInstance(message, -1f, -1f, -1f);
    }

    /// <summary>
    /// 触发指定事件的提示序列（若该事件尚未显示过）。
    /// 首先检查事件是否在 shownEvents 集合中，防止重复显示；
    /// 通过 GetPromptEntry 获取面板配置的 PromptEntry，若有效且包含消息，则标记为已显示并调用 ShowPromptSequence 启动协程。
    /// 在组件面板中可显示的字段：promptEntries（通过 GetPromptEntry 查询）。
    /// </summary>
    private void ShowPrompt(TextPromptEvent eventType)
    {
        if (shownEvents.Contains(eventType))
        {
            return;
        }

        PromptEntry entry = GetPromptEntry(eventType);

        if (entry == null || !HasAnyMessage(entry))
        {
            return;
        }

        shownEvents.Add(eventType);
        ShowPromptSequence(entry);
    }

    /// <summary>
    /// 显示单条自定义消息，可指定各阶段时长（负数则使用默认值）。
    /// 停止当前正在运行的提示协程，重置透明度为 0，然后启动 ShowRoutine 协程执行淡入、保持、淡出。
    /// 在组件面板中可显示的字段：defaultFadeInDuration、defaultHoldDuration、defaultFadeOutDuration（通过 ResolveDuration 使用默认值）。
    /// </summary>
    private void ShowMessageInstance(string message, float fadeInDuration, float holdDuration, float fadeOutDuration)
    {
        if (promptText == null || canvasGroup == null)
        {
            return;
        }

        if (showRoutine != null)
        {
            StopCoroutine(showRoutine);
        }

        canvasGroup.alpha = 0f;

        showRoutine = StartCoroutine(ShowRoutine(
            message,
            ResolveDuration(fadeInDuration, defaultFadeInDuration),
            ResolveDuration(holdDuration, defaultHoldDuration),
            ResolveDuration(fadeOutDuration, defaultFadeOutDuration)));
    }

    /// <summary>
    /// 启动事件对应的完整提示序列（主消息 + 后续消息）。
    /// 停止当前协程，重置透明度，然后启动 ShowSequenceRoutine 协程遍历 entry 的所有消息。
    /// 在组件面板中可显示的字段：promptText、canvasGroup、defaultFadeInDuration、defaultHoldDuration、defaultFadeOutDuration、useUnscaledTime。
    /// </summary>
    private void ShowPromptSequence(PromptEntry entry)
    {
        if (promptText == null || canvasGroup == null)
        {
            return;
        }

        if (showRoutine != null)
        {
            StopCoroutine(showRoutine);
        }

        canvasGroup.alpha = 0f;

        showRoutine = StartCoroutine(ShowSequenceRoutine(entry));
    }

    /// <summary>
    /// 单条消息显示协程，包装 ShowSingleMessageRoutine，执行完毕后将 showRoutine 置空。
    /// 无额外面板字段影响。
    /// </summary>
    private IEnumerator ShowRoutine(string message, float fadeInDuration, float holdDuration, float fadeOutDuration)
    {
        yield return ShowSingleMessageRoutine(message, fadeInDuration, holdDuration, fadeOutDuration);
        showRoutine = null;
    }

    /// <summary>
    /// 消息序列协程：依次显示 entry 的主消息和所有 followingMessages。
    /// 每条消息都使用自己的持续时间设置（若为 -1 则回退到全局默认值）。
    /// 使用 FadeCanvasGroup 与 Wait 协程控制淡入淡出与保持，时间受 useUnscaledTime 控制。
    /// 在组件面板中可显示的字段：promptEntries（内容与时长）、defaultFadeInDuration、defaultHoldDuration、defaultFadeOutDuration、useUnscaledTime。
    /// </summary>
    private IEnumerator ShowSequenceRoutine(PromptEntry entry)
    {
        if (!string.IsNullOrWhiteSpace(entry.message))
        {
            yield return ShowSingleMessageRoutine(
                entry.message,
                ResolveDuration(entry.fadeInDuration, defaultFadeInDuration),
                ResolveDuration(entry.holdDuration, defaultHoldDuration),
                ResolveDuration(entry.fadeOutDuration, defaultFadeOutDuration));
        }

        if (entry.followingMessages != null)
        {
            foreach (PromptMessage promptMessage in entry.followingMessages)
            {
                if (promptMessage == null || string.IsNullOrWhiteSpace(promptMessage.message))
                {
                    continue;
                }

                yield return ShowSingleMessageRoutine(
                    promptMessage.message,
                    ResolveDuration(promptMessage.fadeInDuration, defaultFadeInDuration),
                    ResolveDuration(promptMessage.holdDuration, defaultHoldDuration),
                    ResolveDuration(promptMessage.fadeOutDuration, defaultFadeOutDuration));
            }
        }

        showRoutine = null;
    }

    /// <summary>
    /// 单条消息的完整显示流程：设置 TMP_Text 文本，依次执行淡入、保持、淡出。
    /// 使用 FadeCanvasGroup 控制 CanvasGroup.alpha，Wait 控制保持时间。
    /// </summary>
    private IEnumerator ShowSingleMessageRoutine(string message, float fadeInDuration, float holdDuration, float fadeOutDuration)
    {
        promptText.text = message;

        yield return FadeCanvasGroup(1f, fadeInDuration);
        yield return Wait(holdDuration);
        yield return FadeCanvasGroup(0f, fadeOutDuration);
    }

    /// <summary>
    /// 协程：将 CanvasGroup 的透明度在指定时间内渐变到 targetAlpha。
    /// 使用 Mathf.Lerp 进行线性插值，时间增量依据 useUnscaledTime 选择 Time.unscaledDeltaTime 或 Time.deltaTime。
    /// 若 duration <= 0 则直接设置透明度。
    /// </summary>
    private IEnumerator FadeCanvasGroup(float targetAlpha, float duration)
    {
        float startAlpha = canvasGroup.alpha;
        float elapsed = 0f;

        if (duration <= 0f)
        {
            canvasGroup.alpha = targetAlpha;
            yield break;
        }

        while (elapsed < duration)
        {
            elapsed += useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / duration);
            yield return null;
        }

        canvasGroup.alpha = targetAlpha;
    }

    /// <summary>
    /// 协程：等待指定时间，不受帧率影响。
    /// 时间增量同样受 useUnscaledTime 控制。
    /// </summary>
    private IEnumerator Wait(float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            yield return null;
        }
    }

    /// <summary>
    /// 从 promptEntries 数组中查找与给定事件类型匹配的 PromptEntry。
    /// 直接遍历数组进行比较。
    /// 在组件面板中可显示的字段：promptEntries。
    /// </summary>
    private PromptEntry GetPromptEntry(TextPromptEvent eventType)
    {
        foreach (PromptEntry promptEntry in promptEntries)
        {
            if (promptEntry.eventType == eventType)
            {
                return promptEntry;
            }
        }

        return null;
    }

    /// <summary>
    /// 检查 PromptEntry 是否包含至少一条有效消息（主消息或后续消息之一）。
    /// 返回布尔值。
    /// </summary>
    private bool HasAnyMessage(PromptEntry entry)
    {
        if (!string.IsNullOrWhiteSpace(entry.message))
        {
            return true;
        }

        if (entry.followingMessages == null)
        {
            return false;
        }

        foreach (PromptMessage promptMessage in entry.followingMessages)
        {
            if (promptMessage != null && !string.IsNullOrWhiteSpace(promptMessage.message))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 解析持续时间：若 entryDuration 非负则直接使用，否则使用默认值 defaultDuration。
    /// </summary>
    private float ResolveDuration(float entryDuration, float defaultDuration)
    {
        return entryDuration >= 0f ? entryDuration : defaultDuration;
    }
}