using UnityEngine;

public class TextPromptEventTrigger : MonoBehaviour
{
    [SerializeField] private TextPromptEvent eventType = TextPromptEvent.Custom1;

    public void Show()
    {
        TextPromptManager.Show(eventType);
    }

    public void ShowMessage(string message)
    {
        TextPromptManager.ShowMessage(message);
    }
}
