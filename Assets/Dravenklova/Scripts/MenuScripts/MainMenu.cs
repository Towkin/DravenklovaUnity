using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour
{
    private int m_ButtonWidth = 200;
    private int m_ButtonHeight = 50;
    private int m_ButtonSpacing = 50;

    void OnGUI()
    {
        if (GUI.Button(new Rect(Screen.width / 2 - m_ButtonWidth / 2, Screen.height / 2 - m_ButtonHeight / 2 - m_ButtonSpacing, m_ButtonWidth, m_ButtonHeight), "Click Bait"))
        {
            //Application.LoadLevel("MainScene");
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainScene");
        }

        if (GUI.Button(new Rect(Screen.width / 2 - m_ButtonWidth / 2, Screen.height / 2 + m_ButtonHeight / 2 + m_ButtonSpacing, m_ButtonWidth, m_ButtonHeight), "Quit Bait"))
        {
            Application.Quit();

        }
    }


}
