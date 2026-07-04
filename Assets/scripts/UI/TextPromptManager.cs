using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class TextPromptManager : MonoBehaviour
{
    [Serializable]
    private class PromptEntry
    {
        public TextPromptEvent eventType;
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

    private void Start()
    {
        if (showSceneStartOnStart)
        {
            Show(TextPromptEvent.SceneStart);
        }
    }

    private void OnDestroy()
    {
        if (current == this)
        {
            current = null;
        }
    }

    public static void Show(TextPromptEvent eventType)
    {
        if (current == null)
        {
            return;
        }

        current.ShowPrompt(eventType);
    }

    public static void ShowMessage(string message)
    {
        if (current == null)
        {
            return;
        }

        current.ShowMessageInstance(message, -1f, -1f, -1f);
    }

    private void ShowPrompt(TextPromptEvent eventType)
    {
        PromptEntry entry = GetPromptEntry(eventType);

        if (entry == null || string.IsNullOrWhiteSpace(entry.message))
        {
            return;
        }

        ShowMessageInstance(entry.message, entry.fadeInDuration, entry.holdDuration, entry.fadeOutDuration);
    }

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

        showRoutine = StartCoroutine(ShowRoutine(
            message,
            ResolveDuration(fadeInDuration, defaultFadeInDuration),
            ResolveDuration(holdDuration, defaultHoldDuration),
            ResolveDuration(fadeOutDuration, defaultFadeOutDuration)));
    }

    private IEnumerator ShowRoutine(string message, float fadeInDuration, float holdDuration, float fadeOutDuration)
    {
        promptText.text = message;

        yield return FadeCanvasGroup(1f, fadeInDuration);
        yield return Wait(holdDuration);
        yield return FadeCanvasGroup(0f, fadeOutDuration);

        showRoutine = null;
    }

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

    private IEnumerator Wait(float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            yield return null;
        }
    }

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

    private float ResolveDuration(float entryDuration, float defaultDuration)
    {
        return entryDuration >= 0f ? entryDuration : defaultDuration;
    }
}
