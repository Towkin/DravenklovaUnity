using UnityEngine;
using System.Collections;

public class PauseMenu : MenuParent {

    private bool m_IsPaused = false;
    public bool IsPaused
    {
        get { return m_IsPaused; }
        set { m_IsPaused = value; }
    }

    public void Pause()
    {
        IsPaused = !IsPaused;
    }

    void Update()
    {
        if(IsPaused)
        {
            Cursor.visible = true;
            Time.timeScale = .0f;
        }
        else
        {
            Cursor.visible = false;
            Time.timeScale = 1f;
        }
    }

    void OnGUI()
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
    }
}
