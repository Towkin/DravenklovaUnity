using UnityEngine;
using System.Collections;

public class ConnectionPoint : MonoBehaviour {

    private bool m_Linked = false;
    public bool Linked
    {
        get { return m_Linked; }
        set { m_Linked = true; }
    }
    [SerializeField]
    private bool m_ExitOnly = false;
    public bool ExitOnly
    {
        get { return m_ExitOnly; }
    }
    
    [SerializeField]
    private GameObject[] m_PreferedPrefabs;
    public GameObject[] PreferedPrefabs
    {
        get { return m_PreferedPrefabs; }
    }


	void Start () {
	    
	}

	void Update () {
	
	}
}
