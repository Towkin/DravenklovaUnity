using UnityEngine;
using System.Collections;

public class LevelLoaderTrigger : MonoBehaviour {

    LevelDigger m_LevelGenerator;
    LevelDigger LevelGenerator
    {
        get { return m_LevelGenerator; }
        set { m_LevelGenerator = value; }
    }

    bool m_IsLoadingLevel = false;
    float m_LoadLevelStart = 0f;
	
	void Start () {
        LevelGenerator = FindObjectOfType<LevelDigger>();
	}
	
    public void OnTriggerEnter(Collider other)
    {
        if(other.GetComponentInChildren<Player>())
        {
            StartLoadProcess();
        }
    }

    public void StartLoadProcess()
    {
        m_IsLoadingLevel = true;
        m_LoadLevelStart = Time.realtimeSinceStartup;

        FindObjectOfType<IngameLoadingScript>().ShowLoadingUI();
    }
    public void EndLoadProcess()
    {
        LevelGenerator.LoadNextLevel();

        FindObjectOfType<IngameLoadingScript>().HideLoadingUI();

        Destroy(gameObject);
    }

    public void Update()
    {
        if(m_IsLoadingLevel && Time.realtimeSinceStartup - m_LoadLevelStart >= Time.deltaTime)
        {
            EndLoadProcess();
        }
    }
}
