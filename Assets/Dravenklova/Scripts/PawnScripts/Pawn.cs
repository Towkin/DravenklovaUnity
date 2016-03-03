using UnityEngine;
using System.Collections;

public abstract class Pawn : MonoBehaviour {

    [SerializeField]
    protected float m_Health;
    [SerializeField]
    protected float m_HealthMax;
    public float Health
    {
        get { return m_Health; }
        set
        {
            m_Health = Mathf.Clamp(value, 0f, m_HealthMax);
        }
    }

    [SerializeField]
    protected float m_Sanity;
    [SerializeField]
    protected float m_SanityMax;
    public float Sanity
    {
        get { return m_Sanity; }
        set
        {
            m_Sanity = Mathf.Clamp(value, 0f, m_SanityMax);
        }
    }

    [SerializeField]
    protected float m_Holy;
    [SerializeField]
    protected float m_HolyMax;
    public float Holy
    {
        get { return m_Holy; }
        set
        {
            m_Holy = Mathf.Clamp(value, 0f, m_HolyMax);
        }
    }

    [SerializeField]
    protected float m_Oil;
    [SerializeField]
    protected float m_OilMax;
    public float GetOilMax
    {
        get { return m_OilMax; }
    }

    public float Oil
    {
        get { return m_Oil; }
        set
        {
            m_Oil = Mathf.Clamp(value, 0f, m_OilMax);
        }
    }

    void Start ()
    {
	    
	}


	void Update ()
    {
        

        if (Health <= 0)
        {
            // TODO: Death functionality
        }
	}
}
