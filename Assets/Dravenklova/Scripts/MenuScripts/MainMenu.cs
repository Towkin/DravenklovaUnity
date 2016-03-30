using UnityEngine;
using System.Collections;

public class MainMenu : MenuParent
{
    

    void OnGUI()
    {
        if (GUI.Button(new Rect(Screen.width / 2 - m_ButtonWidth / 2, Screen.height / 2 - m_ButtonHeight / 2 - m_ButtonSpacing, m_ButtonWidth, m_ButtonHeight), "Click Bait"))
        {
            Application.LoadLevel("MainScene");
        }

        if (GUI.Button(new Rect(Screen.width / 2 - m_ButtonWidth / 2, Screen.height / 2 + m_ButtonHeight / 2 + m_ButtonSpacing, m_ButtonWidth, m_ButtonHeight), "Quit Bait"))
        {
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
    }


}
