using UnityEngine;
//using System.Collections.Generic;
using System;

public class ConnectionPoint : MonoBehaviour, IComparable<ConnectionPoint> {

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
    
    public int CompareTo(ConnectionPoint other)
    {
        return transform.position.y.CompareTo(other.transform.position.y);
    }
    public override string ToString()
    {
        return transform.name + " connection point; Linked " + Linked.ToString() + "; Exit only " + ExitOnly.ToString();
    }
}
