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
        if(transform.childCount > 0)
        {
            transform.GetChild(0).gameObject.SetActive(true);
        }
        
    }
    public void EndLoadProcess()
    {
        LevelGenerator.LoadNextLevel();
        Destroy(gameObject);
    }

    public void Update()
    {
        if(m_IsLoadingLevel)
        {
            EndLoadProcess();
        }
    }
}
