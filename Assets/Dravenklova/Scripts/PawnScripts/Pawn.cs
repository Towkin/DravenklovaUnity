using UnityEngine;
using System.Collections;

public abstract class Pawn : MonoBehaviour {

    #region Stats
    [Header ("Character Attributes")]

    [SerializeField]
    protected float m_Health = 1f;
    [SerializeField]
    protected float m_HealthMax = 1f;
    public float Health
    {
        get { return m_Health; }
        set
        {
            m_Health = Mathf.Clamp(value, 0f, m_HealthMax);
            if(m_Health == 0f)
            {
                // TODO: Death
            }
        }
    }

    [SerializeField]
    [Range(0f, 1f)]
    protected float m_Sanity;
    //[SerializeField]
    //protected float m_SanityMax = 1;
    public float Sanity
    {
        get { return m_Sanity; }
        set
        {
            m_Sanity = Mathf.Clamp01(value);//Mathf.Clamp(value, 0f, m_SanityMax);
        }
    }

    [SerializeField]
    [Range(0f, 1f)]
    protected float m_Holy;
    //[SerializeField]
    protected float m_HolyMax = 1;
    public float Holy
    {
        get { return m_Holy; }
        set
        {
            m_Holy = Mathf.Clamp01(value);// Mathf.Clamp(value, 0f, m_HolyMax);
        }
    }

    [SerializeField]
    [Range(0f, 1f)]
    protected float m_Oil;
    //[SerializeField]
    //protected float m_OilMax = 1;
    //public float GetOilMax
    //{
    //    get { return m_OilMax; }
    //}

    public float Oil
    {
        get { return m_Oil; }
        set
        {
            m_Oil = Mathf.Clamp01(value);//Mathf.Clamp(value, 0f, m_OilMax);
        }
    }

    public void SetCharacterAttributes(float a_Health, float a_Sanity, float a_Holy, float a_Oil)
    {
        Health = a_Health;
        Sanity = a_Sanity;
        Holy = a_Holy;
        Oil = a_Oil;
    }

    public void SetCharacterAttributes()
    {
        Health = 1;
        Sanity = 1;
        Holy = 1;
        Oil = 1;
    }
    
    #endregion

    void Start ()
    {
	    
	}


	void Update ()
    {
        
	}
}
