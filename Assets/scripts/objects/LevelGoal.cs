using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelGoal : MonoBehaviour
{
    [Header("Level Complete")]
    [Tooltip("Delay before loading the next level, in seconds.")]
    [SerializeField] private float completionDelay = 2f;
    [Tooltip("Automatically load the next numbered level, for example Level_001 -> Level_002.")]
    [SerializeField] private bool autoLoadNextNumberedLevel = true;
    [Tooltip("Manual fallback scene name. Used when auto loading is disabled or current scene is not numbered.")]
    [SerializeField] private string nextLevelSceneName = "Level_002";
    [Tooltip("Auto loading stops after this level number.")]
    [SerializeField] private int maxAutoLevelNumber = 6;

    private bool triggered;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!triggered && other.CompareTag("Player"))
        {
            triggered = true;
            SFXManager.Play(SFXType.LevelComplete);
            TextPromptManager.Show(TextPromptEvent.LevelComplete);
            StartCoroutine(CompleteLevelRoutine());
        }
    }

    private IEnumerator CompleteLevelRoutine()
    {
        DisableAllInputComponents();

        yield return new WaitForSecondsRealtime(completionDelay);

        string targetSceneName = GetNextLevelSceneName();

        if (string.IsNullOrWhiteSpace(targetSceneName))
        {
            Debug.LogWarning("No next level scene is available.");
            EnableAllInputComponents();
            triggered = false;
            yield break;
        }

        if (!SceneExists(targetSceneName))
        {
            Debug.LogWarning($"Level scene '{targetSceneName}' is not in Build Settings.");
            EnableAllInputComponents();
            triggered = false;
            yield break;
        }

        SFXManager.Play(SFXType.SceneTransition);
        SceneTransitionManager.LoadScene(targetSceneName);
    }

    private string GetNextLevelSceneName()
    {
        if (!autoLoadNextNumberedLevel)
        {
            return nextLevelSceneName;
        }

        string currentSceneName = SceneManager.GetActiveScene().name;
        Match match = Regex.Match(currentSceneName, @"^(.*?)(\d+)$");

        if (!match.Success)
        {
            return nextLevelSceneName;
        }

        string prefix = match.Groups[1].Value;
        string numberText = match.Groups[2].Value;
        int currentLevelNumber = int.Parse(numberText);
        int nextLevelNumber = currentLevelNumber + 1;

        if (nextLevelNumber > maxAutoLevelNumber)
        {
            return string.Empty;
        }

        return $"{prefix}{nextLevelNumber.ToString().PadLeft(numberText.Length, '0')}";
    }

    private void DisableAllInputComponents()
    {
        PlayerMoveNoJump playerMove = FindObjectOfType<PlayerMoveNoJump>();
        if (playerMove != null) playerMove.enabled = false;

        MapTransformController mapCtrl = FindObjectOfType<MapTransformController>();
        if (mapCtrl != null) mapCtrl.enabled = false;

        PauseManager pauseMgr = FindObjectOfType<PauseManager>();
        if (pauseMgr != null) pauseMgr.enabled = false;

        GameModeController modeCtrl = FindObjectOfType<GameModeController>();
        if (modeCtrl != null) modeCtrl.enabled = false;

        ViewFixableClickController clickCtrl = FindObjectOfType<ViewFixableClickController>();
        if (clickCtrl != null) clickCtrl.enabled = false;
    }

    private void EnableAllInputComponents()
    {
        PlayerMoveNoJump playerMove = FindObjectOfType<PlayerMoveNoJump>();
        if (playerMove != null) playerMove.enabled = true;

        MapTransformController mapCtrl = FindObjectOfType<MapTransformController>();
        if (mapCtrl != null) mapCtrl.enabled = true;

        PauseManager pauseMgr = FindObjectOfType<PauseManager>();
        if (pauseMgr != null) pauseMgr.enabled = true;

        GameModeController modeCtrl = FindObjectOfType<GameModeController>();
        if (modeCtrl != null) modeCtrl.enabled = true;

        ViewFixableClickController clickCtrl = FindObjectOfType<ViewFixableClickController>();
        if (clickCtrl != null) clickCtrl.enabled = true;
    }

    private bool SceneExists(string sceneName)
    {
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string path = SceneUtility.GetScenePathByBuildIndex(i);
            string name = System.IO.Path.GetFileNameWithoutExtension(path);

            if (name == sceneName)
            {
                return true;
            }
        }

        return false;
    }
}
