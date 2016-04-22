using UnityEngine;
using System.Collections;

// Jens Bak
// Emanuel Strömgren

public class MainMenu : DravenklovaMenu
{
    public void PlayButtonPressed()
    {
        ButtonPressAudio.Play();
        UnityEngine.SceneManagement.SceneManager.LoadScene("LoadingScene");
    }
    public void ExitButtonPressed()
    {
        ButtonPressAudio.Play();
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
