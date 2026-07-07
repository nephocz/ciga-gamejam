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
        NormalizeCanvas();
        ConvertSpriteButtonsToUI();
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

    private void NormalizeCanvas()
    {
        Canvas canvas = FindCanvasInCurrentScene();
        CanvasScaler canvasScaler = canvas != null ? canvas.GetComponent<CanvasScaler>() : null;

        if (canvasScaler == null)
        {
            return;
        }

        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasScaler.referenceResolution = new Vector2(1920f, 1080f);
        canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        canvasScaler.matchWidthOrHeight = 0.5f;
    }

    private void ConvertSpriteButtonsToUI()
    {
        Canvas canvas = FindCanvasInCurrentScene();

        if (canvas == null)
        {
            return;
        }

        Button[] buttons = canvas.GetComponentsInChildren<Button>(true);

        foreach (Button button in buttons)
        {
            Image buttonImage = button.targetGraphic as Image;

            if (buttonImage == null)
            {
                buttonImage = button.GetComponent<Image>();
            }

            SpriteRenderer spriteRenderer = button.GetComponentInChildren<SpriteRenderer>(true);

            if (buttonImage == null || spriteRenderer == null || spriteRenderer.sprite == null)
            {
                continue;
            }

            buttonImage.sprite = spriteRenderer.sprite;
            buttonImage.color = spriteRenderer.color;
            buttonImage.type = Image.Type.Simple;
            buttonImage.preserveAspect = false;
            buttonImage.raycastTarget = true;
            button.targetGraphic = buttonImage;
            spriteRenderer.gameObject.SetActive(false);
        }
    }

    private Canvas FindCanvasInCurrentScene()
    {
        GameObject[] rootObjects = gameObject.scene.GetRootGameObjects();

        foreach (GameObject rootObject in rootObjects)
        {
            Canvas canvas = rootObject.GetComponentInChildren<Canvas>(true);

            if (canvas != null)
            {
                return canvas;
            }
        }

        return null;
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
