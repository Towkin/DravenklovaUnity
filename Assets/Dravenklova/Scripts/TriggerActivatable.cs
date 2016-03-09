using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class TriggerActivatable : MonoBehaviour
{
    [SerializeField]
    protected Activatable[] m_ActivatedObjects;
    public Activatable[] ActivatedObjects
    {
        get { return m_ActivatedObjects; }
    }

    [SerializeField]
    protected string m_CompareTag = "Player";
    public string CompareTag
    {
        get { return m_CompareTag; }
    }

    [SerializeField]
    protected bool m_TriggerOnce;
    public bool TriggerOnce
    {
        get { return m_TriggerOnce; }
    }

    void OnTriggerEnter (Collider other)
    {
        if (other.tag == CompareTag)
        {
            foreach(Activatable Object in ActivatedObjects)
            {
                Object.Activate(); 
            }
            if (TriggerOnce)
            {
                Destroy(this);
            }
        }
    }
	
}
