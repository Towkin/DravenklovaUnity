using UnityEngine;
using System.Collections;

public class GameOverMenu : MonoBehaviour {

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
            Cursor.visible = value;
            GameOverMenuObject.SetActive(value);
        }
    }

    
    public void RestartButtonPressed()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("LoadingScene");
    }

    public void QuitButtonPressed()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
}
