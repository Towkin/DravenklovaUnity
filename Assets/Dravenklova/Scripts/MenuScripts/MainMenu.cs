using UnityEngine;
using System.Collections;

public class MainMenu : MenuParent
{
    public void PlayButtonPressed()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("LoadingScene");
    }
    public void ExitButtonPressed()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
