using UnityEngine;
using System.Collections;
using System;

// Doesn't need to be in MonoBehavior, else it won't work the way we want it to do.
[Serializable]
public class StatScript 
{
    [SerializeField]
    private BarScript m_Bar;

    [SerializeField]
    private float m_MaxVal;

    [SerializeField]
    private float m_CurrentVal;

    public float CurrentVal
    {
        get
        {
            return m_CurrentVal;
        }

        set
        {
            this.m_CurrentVal = value;
            if (m_Bar != null)
            {
                m_Bar.BarValue = m_CurrentVal;
            }
        }
    }

    public float MaxVal
    {
        get
        {
            return m_MaxVal;
        }

        set
        {
            this.m_MaxVal = value;
            if (m_Bar != null)
            {
                m_Bar.MaxValue = m_MaxVal;
            }
        }
    }

    public void Initialize()
    {
        this.MaxVal = m_MaxVal;
        this.CurrentVal = m_CurrentVal;
    }
}

