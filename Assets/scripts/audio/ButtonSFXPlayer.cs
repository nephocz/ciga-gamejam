using UnityEngine;

public class ButtonSFXPlayer : MonoBehaviour
{
    public void PlayButtonClick()
    {
        SFXManager.Play(SFXType.ButtonClick);
    }
}
