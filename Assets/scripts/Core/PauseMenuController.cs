using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuController : MonoBehaviour
{
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Color backgroundColor = new Color(0f, 0f, 0f, 0.78431374f);

    private PauseManager pauseManager;

    private void Awake()
    {
        ApplyBackgroundColor();
    }

    private void Start()
    {
        pauseManager = FindFirstObjectByType<PauseManager>();
    }

    private void ApplyBackgroundColor()
    {
        if (backgroundImage == null)
        {
            GameObject panel = GameObject.Find("Panel");

            if (panel != null)
            {
                backgroundImage = panel.GetComponent<Image>();
            }
        }

        if (backgroundImage != null)
        {
            backgroundImage.color = backgroundColor;
        }
    }

    public void OnContinueButton()
    {
        SFXManager.Play(SFXType.ButtonClick);

        if (pauseManager != null)
        {
            pauseManager.ResumeGame();
        }
        else
        {
            Time.timeScale = 1f;
            SceneManager.UnloadSceneAsync(gameObject.scene);
        }
    }

    public void OnExitButton()
    {
        SFXManager.Play(SFXType.ButtonClick);
        SFXManager.Play(SFXType.SceneTransition);
        Time.timeScale = 1f;
        SceneTransitionManager.LoadScene("MainMenuScene");
    }

    public void OnRestartButton()
    {
        SFXManager.Play(SFXType.ButtonClick);
        SFXManager.Play(SFXType.SceneTransition);
        Time.timeScale = 1f;
        SceneTransitionManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
