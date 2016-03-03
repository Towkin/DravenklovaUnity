using UnityEngine;
using System.Collections;

public abstract class Pawn : MonoBehaviour
{

    #region All move attributes
    #region Basic move attributes
    [Header("Basic move attributes")]


    [SerializeField]
    [Range(0f, 5f)]
    private float m_SprintSpeedMultiplier = 2f;
    private float SprintSpeedMultiplier
    {
        get { return m_SprintSpeedMultiplier; }
        //protected set { m_SprintSpeedMultiplier = value; }
    }

    private float MaxSpeedMultiplier
    {
        get
        {
            float Mult = 1f;
            Mult *= InputSprint ? m_SprintSpeedMultiplier : 1f;
            return Mult;
        }
    }

    [SerializeField]
    [Range(0f, 20f)]
    protected float m_MaxSpeed = 5f;
    public float MaxSpeed
    {
        get { return m_MaxSpeed * MaxSpeedMultiplier; }
    }

    [SerializeField]
    [Range(0f, 100f)]
    protected float m_Acceleration = 1f;
    public float Acceleration
    {
        get { return m_Acceleration; }
    }

    [SerializeField]
    [Range(0f, 100f)]
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
    protected Vector2 m_LastPlanarDirection = Vector2.up;
    public Vector3 Velocity
    {
        get { return m_Velocity; }
        protected set
        {
            m_Velocity = value;
            if (m_Velocity != Vector3.zero)
            {
                m_LastDirection = m_Velocity.normalized;

                if(m_LastDirection != Vector3.up && m_LastDirection != Vector3.down)
                {
                    m_LastPlanarDirection = new Vector2(m_LastDirection.x, m_LastDirection.z).normalized;
                }
            }
        }
    }
    public Vector2 PlanarVelocity
    {
        get { return new Vector2(Velocity.x, Velocity.z); }
        set { Velocity = new Vector3(value.x, Velocity.y, value.y); }
    }
    public float Speed
    {
        get { return Velocity.magnitude; }
        protected set { Velocity = VelocityDirection * value; }
    }
    public float PlanarSpeed
    {
        get { return new Vector2(Velocity.x, Velocity.z).magnitude; }
        protected set {
            Vector2 NewPlanarSpeed = PlanarVelocityDirection * value;
            Velocity = new Vector3(NewPlanarSpeed.x, Velocity.y, NewPlanarSpeed.y);
        }
    }

    public Vector3 VelocityDirection
    {
        get { return m_LastDirection; }
        protected set { Velocity = value.normalized * Speed; }
    }
    public Vector2 PlanarVelocityDirection
    {
        get { return m_LastPlanarDirection; }
        protected set
        {
            Vector2 NormalizedValue = value.normalized;
            Velocity = new Vector3(NormalizedValue.x, Velocity.y, NormalizedValue.y);
        }
    }

    public Vector3 ForwardVelocity
    {
        get { return transform.rotation * Velocity; }
        set
        {
            Velocity = transform.rotation * value;
        }
    }
    public Vector2 PlanarForwardVelocity
    {
        get
        {
            float Rad = Mathf.PI * (transform.eulerAngles.y / 180f);
            Vector2 Base = PlanarVelocity;

            var ca = Mathf.Cos(Rad);
            var sa = Mathf.Sin(Rad);

            float NewX = ca * Base.x - sa * Base.y;
            float NewY = sa * Base.x + ca * Base.y;

            return new Vector2(NewX, NewY);
        }
        set
        {
            float Rad = -Mathf.PI * (transform.eulerAngles.y / 180f);
            
            var ca = Mathf.Cos(Rad);
            var sa = Mathf.Sin(Rad);

            float NewX = ca * value.x - sa * value.y;
            float NewY = sa * value.x + ca * value.y;

            PlanarVelocity = new Vector2(NewX, NewY);
        }
    }

    #endregion

    #region Jump and gravity attributes
    [Header("Jump and gravity attributes")]
    [SerializeField]
    [Range(0f, 100f)]
    protected float m_JumpAcceleration = 10f;
    public float JumpAcceleration
    {
        get { return m_JumpAcceleration; }
    }
    [SerializeField]
    [Range(0f, 1f)]
    protected float m_AirControl = 0.05f;
    public float AirControl
    {
        get { return m_AirControl; }
        protected set { m_AirControl = value; }
    }
    protected float m_JumpTime = 0f;
    public float JumpTime
    {
        get { return m_JumpTime; }
        protected set { m_JumpTime = value; }
    }
    [SerializeField]
    [Range(0f, 1f)]
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
    protected bool m_IsJumping = false;
    public bool IsJumping
    {
        get { return m_IsJumping; }
        set
        {
            if(IsGrounded && value)
            {
                JumpTime = 0f;
            }
            m_IsJumping = value;
        }
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
        set
        {
            m_ViewDirection = value.normalized;
        }
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

    #region Input variables

    protected Vector2 m_InputMoveDirection = Vector2.zero;
    protected Vector2 InputMoveDirection
    {
        get { return m_InputMoveDirection; }
        set
        {
            Vector2 InputVal = value;
            if(InputVal.magnitude > 1f)
            {
                InputVal = InputVal.normalized;
            }
            m_InputMoveDirection = InputVal;
        }
    }
    protected float InputMoveScale
    {
        get { return InputMoveDirection.magnitude; }
        set
        {
            InputMoveDirection = InputMoveDirection.normalized * Mathf.Clamp01(value);
        }
    }
    protected bool m_InputJump = false;
    public bool InputJump
    {
        get { return m_InputJump; }
        protected set { m_InputJump = value; }
    }
    protected bool m_InputSprint = false;
    public bool InputSprint
    {
        get { return m_InputSprint; }
        set { m_InputSprint = value; }
    }


    #endregion

}
