using UnityEngine;
using System.Collections;

// Samuel Einheri
// Emanuel Strömgren

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
    protected string m_TriggerTag = "Player";
    public string TriggerTag
    {
        get { return m_TriggerTag; }
    }

    [SerializeField]
    protected bool m_TriggerOnce;
    public bool TriggerOnce
    {
        get { return m_TriggerOnce; }
    }

    void OnTriggerEnter (Collider other)
    {
        if (other.tag == TriggerTag)
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
