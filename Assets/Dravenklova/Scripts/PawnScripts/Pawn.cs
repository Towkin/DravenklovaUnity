using UnityEngine;
using System.Collections;

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



    // Use this for initialization
    void Start ()
    {
     
	}
	
	// Update is called once per frame
	void Update ()
    {
        Vector3 VelocityAdd = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical")).normalized * Acceleration * Time.deltaTime;
        if (VelocityAdd == Vector3.zero)
        {
            Speed -= Mathf.Min(Time.deltaTime * Deacceleration, Speed);
        }
        else
        {
            Velocity += VelocityAdd;
            Speed = Mathf.Min(MaxSpeed, Speed);

        }
        Speed *= 1f - Decay * Time.deltaTime;
        transform.position += Velocity * Time.deltaTime;

	}
}
