using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenuScene");
    }

    public void LoadLevelSelect()
    {
        SceneManager.LoadScene("LevelSelectScene");
    }

    public void LoadLevel01()
    {
        SceneManager.LoadScene("Level_01");
    }

    public void LoadLevel001()
    {
        SceneManager.LoadScene("Level_001");
    }

    public void LoadLevel02()
    {
        SceneManager.LoadScene("Level_02");
    }

    public void LoadSceneByName(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void QuitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
