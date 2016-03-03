using UnityEngine;
using System.Collections;

// public abstract class Pawn : MonoBehaviour <- Use this one later
public class Pawn : MonoBehaviour
{

    #region All move attributes
    #region Basic move attributes
    [Header("Basic move attributes")]

    [SerializeField]
    [Range(0f, 50f)]
    protected float m_MaxSpeed = 5f;
    public float MaxSpeed
    {
        get { return m_MaxSpeed; }
    }

    [SerializeField]
    [Range(0f, 10f)]
    protected float m_Acceleration = 1f;
    public float Acceleration
    {
        get { return m_Acceleration; }
    }

    [SerializeField]
    [Range(0f, 10f)]
    protected float m_Deacceleration = 1f;
    public float Deacceleration
    {
        get { return m_Deacceleration; }
    }

    [SerializeField]
    [Range(0f, 1f)]
    protected float m_Decay = 0.05f;
    public float Decay
    {
        get { return m_Decay; }
    }
    protected Vector3 m_Velocity = Vector3.zero;
    protected Vector3 m_LastDirection = Vector3.forward;
    public Vector3 Velocity
    {
        get { return m_Velocity; }
        protected set
        {
            m_Velocity = value;
            if (m_Velocity != Vector3.zero)
            {
                m_LastDirection = m_Velocity.normalized;
            }
        }
    }
    public float Speed
    {
        get { return Velocity.magnitude; }
        // TODO: Change m_Velocity.normalized to something better
        protected set { Velocity = Velocity.normalized * value; }
    }

    public Vector3 VelocityDirection
    {
        get { return m_LastDirection; }
        protected set { Velocity = value.normalized * Speed; }
    }

    public Vector3 ForwardVelocity
    {
        get { return transform.rotation * Velocity; }
    }

    #endregion

    #region Jump and gravity attributes
    [Header("Jump and gravity attributes")]
    [SerializeField]
    protected float m_JumpForce = 10f;
    public float JumpForce
    {
        get { return m_JumpForce; }
    }
    protected float m_JumpTime = 0f;
    public float JumpTime
    {
        get { return m_JumpTime; }
        protected set { m_JumpTime = value; }
    }
    [SerializeField]
    protected float m_JumpTimeMax = 0.2f;
    public float JumpTimeMax
    {
        get { return m_JumpTimeMax; }
    }
    protected bool m_IsGrounded = true;
    public bool IsGrounded
    {
        get { return m_IsGrounded; }
        protected set { m_IsGrounded = value; }
    }
    [SerializeField]
    protected Vector3 m_PawnGravity = new Vector3(0f, -9.82f, 0f);
    public Vector3 PawnGravity
    {
        get { return m_PawnGravity; }
    }
    #endregion

    #region Move behaviour attributes
    [Header("Move behaviour attributes")]
    [SerializeField]
    protected float m_WalkThreshold = 2f;
    public float WalkThreshold
    {
        get { return m_WalkThreshold; }
    }

    public bool IsWalking
    {
        get { return (IsGrounded && Speed > WalkThreshold && Speed < RunThreshold); }
    }

    [SerializeField]
    protected float m_RunThreshold = 4f;
    public float RunThreshold
    {
        get { return m_RunThreshold; }
    }

    public bool IsRunning
    {
        get { return (IsGrounded && Speed >= RunThreshold); }
    }

    [SerializeField]
    protected float m_TurnRate = 360f;
    public float TurnRate
    {
        get { return m_TurnRate; }
    }
    protected Vector3 m_ViewDirection = Vector3.forward;
    public Vector3 ViewDirection
    {
        get { return m_ViewDirection; }
    }
    #endregion
    #endregion
	
	#region Character attributes
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
        set { m_Sanity = Mathf.Clamp01(value); }
    }

    [SerializeField]
    [Range(0f, 1f)]
    protected float m_Holy;
    //[SerializeField]
    protected float m_HolyMax = 1;
    public float Holy
    {
        get { return m_Holy; }
        set { m_Holy = Mathf.Clamp01(value); }
    }

    [SerializeField]
    [Range(0f, 1f)]
    protected float m_Oil;

    public float Oil
    {
        get { return m_Oil; }
        set { m_Oil = Mathf.Clamp01(value); }
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
    
    void Update()
    {
        #region Test movement variable code
        Vector3 VelocityAdd = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical")).normalized * Acceleration * Time.deltaTime;
        if (VelocityAdd == Vector3.zero)
        {
            Speed -= Mathf.Min(Time.deltaTime * Deacceleration, Speed);
            Speed *= Mathf.Pow(1f - Decay, Time.deltaTime);
        }
        else
        {
            Velocity += VelocityAdd;
            Speed = Mathf.Min(MaxSpeed, Speed);

        }

        
        transform.position += Velocity * Time.deltaTime;
        #endregion
        
    }
}
