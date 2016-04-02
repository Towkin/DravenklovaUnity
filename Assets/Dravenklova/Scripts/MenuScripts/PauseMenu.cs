using UnityEngine;
using System.Collections;

public class PauseMenu : DravenklovaMenu {

    [SerializeField]
    private GameObject m_PauseMenuObject;
    public GameObject PauseMenuObject
    {
        get { return m_PauseMenuObject; }
    }
    
    private bool m_IsPaused = false;
    public bool IsPaused
    {
        get { return m_IsPaused; }
        set
        {
            m_IsPaused = value;
            Cursor.lockState = value ? CursorLockMode.Confined : CursorLockMode.Locked;
            Cursor.visible = value;
            Time.timeScale = value? 0f : 1f;
            PauseMenuObject.SetActive(value);
        }
    }

    public void Pause()
    {
        IsPaused = !IsPaused;
    }
    
    public void ResumeButtonPressed()
    {
        ButtonPressAudio.Play();
        IsPaused = false;
    }

    public void QuitButtonPressed()
    {
        ButtonPressAudio.Play();
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }

    /*void OnGUI()
    {
        if(IsPaused)
        {
            if (GUI.Button(new Rect(Screen.width / 2 - m_ButtonWidth / 2, Screen.height / 2 - m_ButtonHeight / 2 - m_ButtonSpacing, m_ButtonWidth, m_ButtonHeight), "Resume"))
            {
                IsPaused = !IsPaused;
            }

            if (GUI.Button(new Rect(Screen.width / 2 - m_ButtonWidth / 2, Screen.height / 2 + m_ButtonHeight / 2 + m_ButtonSpacing, m_ButtonWidth, m_ButtonHeight), "Quit"))
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
            }
        }
    }*/


}
