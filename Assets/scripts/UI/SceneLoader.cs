using UnityEngine;

public class SceneLoader : MonoBehaviour
{
    public void LoadMainMenu()
    {
        LoadSceneWithSFX("MainMenuScene");
    }

    public void LoadLevelSelect()
    {
        LoadSceneWithSFX("LevelSelectScene");
    }

    public void LoadLevel01()
    {
        LoadSceneWithSFX("Level_01");
    }

    public void LoadLevel001()
    {
        LoadSceneWithSFX("Level_001");
    }

    public void LoadLevel002()
    {
        LoadSceneWithSFX("Level_002");
    }

    public void LoadLevel003()
    {
        LoadSceneWithSFX("Level_003");
    }

    public void LoadLevel004()
    {
        LoadSceneWithSFX("Level_004");
    }

    public void LoadLevel005()
    {
        LoadSceneWithSFX("Level_005");
    }

    public void LoadLevel006()
    {
        LoadSceneWithSFX("Level_006");
    }

    public void LoadLevel02()
    {
        LoadSceneWithSFX("Level_02");
    }

    public void LoadSceneByName(string sceneName)
    {
        LoadSceneWithSFX(sceneName);
    }

    private void LoadSceneWithSFX(string sceneName)
    {
        SFXManager.Play(SFXType.ButtonClick);
        SFXManager.Play(SFXType.SceneTransition);
        SceneTransitionManager.LoadScene(sceneName);
    }

    public void QuitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
