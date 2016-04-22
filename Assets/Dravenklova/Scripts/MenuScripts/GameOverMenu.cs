using UnityEngine;
using System.Collections;

// Jens Bak
// Emanuel Strömgren

public class GameOverMenu : DravenklovaMenu {

    [SerializeField]
    private GameObject m_GameOverMenuObject;
    public GameObject GameOverMenuObject
    {
        get { return m_GameOverMenuObject; }
    }

    private bool m_Visible = false;
    public bool Visible
    {
        get { return m_Visible; }
        set
        {
            m_Visible = value;
            Cursor.lockState = value ? CursorLockMode.Confined : CursorLockMode.Locked;
            Cursor.visible = value;
            GameOverMenuObject.SetActive(value);
        }
    }

    
    public void RestartButtonPressed()
    {
        ButtonPressAudio.Play();
        UnityEngine.SceneManagement.SceneManager.LoadScene("LoadingScene");
    }

    public void QuitButtonPressed()
    {
        ButtonPressAudio.Play();
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
}
