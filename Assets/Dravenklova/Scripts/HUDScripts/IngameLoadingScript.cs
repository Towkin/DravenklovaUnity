using UnityEngine;
using System.Collections;

public class IngameLoadingScript : MonoBehaviour {
    [SerializeField]
    private GameObject m_LoadingUI;
    public GameObject LoadingUI
    {
        get { return m_LoadingUI; }
    }
    [SerializeField]
    private UnityEngine.UI.Text m_LoadingTextUI;
    public UnityEngine.UI.Text LoadingTextUI
    {
        get { return m_LoadingTextUI; }
    }
    [SerializeField]
    private string[] m_LoadingTextList;
    public string[] LoadingTextList
    {
        get { return m_LoadingTextList; }
    }

    public void ShowLoadingUI()
    {
        if (LoadingUI)
        {
            LoadingUI.SetActive(true);
            if (LoadingTextUI)
            {
                LoadingTextUI.text = "Loading...";
                if (LoadingTextList.Length > 0)
                {
                    LoadingTextUI.text = LoadingTextList[Random.Range(0, LoadingTextList.Length)];
                }
            }
        }
    }
    public void HideLoadingUI()
    {
        if (LoadingUI)
        {
            LoadingUI.SetActive(false);
        }
    }
}
