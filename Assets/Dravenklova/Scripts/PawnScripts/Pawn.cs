﻿using UnityEngine;
using System.Collections;

// Emanuel Strömgren
// Jens Bak
// Samuel Einheri
// Daniel Samuelsson


public abstract class Pawn : MonoBehaviour
{
    #region Pawn components
    [Header("Pawn Components")]
    [SerializeField]
    private Rigidbody m_PhysicsBody;
    public Rigidbody PhysicsBody
    {
        get { return m_PhysicsBody; }
        set { m_PhysicsBody = value; }
    }
    /*[SerializeField]
    private CapsuleCollider m_Collider;
    public CapsuleCollider Capsule
    {
        get { return m_Collider; }
        set { m_Collider = value; }
    }*/
    [SerializeField]
    private CharacterController m_Controller;
    public CharacterController Controller
    {
        get { return m_Controller; }
    }
    #endregion

    #region All move attributes
    #region Basic move attributes
    [Header("Basic move attributes")]


    [SerializeField]
    [Range(0f, 5f)]
    private float m_SprintSpeedMultiplier = 2f;
    private float SprintSpeedMultiplier
    {
        get { return m_SprintSpeedMultiplier; }
    }
    [SerializeField]
    [Range(0f, 1f)]
    private float m_AimSpeedMultiplier = 0.5f;
    private float AimSpeedMultiplier
    {
        get { return m_AimSpeedMultiplier; }
    }
    [SerializeField]
    [Range(0f, 1f)]
    private float m_ReloadSpeedMultipler = 0.5f;
    private float ReloadSpeedMultiplier
    {
        get { return m_ReloadSpeedMultipler; }
    }

    private float MaxSpeedMultiplier
    {
        get
        {
            float Mult = 1f;
            Mult *= (InputSprint && !IsAiming && (EquippedWeapon == null || !EquippedWeapon.IsReloading)) ? SprintSpeedMultiplier : 1f;
            Mult *= IsAiming ? AimSpeedMultiplier : 1f;
            Mult *= (EquippedWeapon != null && EquippedWeapon.IsReloading) ? ReloadSpeedMultiplier : 1f;
            return Mult;
        }
    }

    [SerializeField]
    [Range(0f, 50f)]
    private float m_MaxSpeed = 5f;
    public float MaxSpeed
    {
        get { return m_MaxSpeed * MaxSpeedMultiplier; }
    }

    [SerializeField]
    [Range(0f, 100f)]
    private float m_Acceleration = 1f;
    public float Acceleration
    {
        get { return m_Acceleration; }
    }

    [SerializeField]
    [Range(0f, 100f)]
    private float m_Deacceleration = 1f;
    public float Deacceleration
    {
        get { return m_Deacceleration; }
    }

    [SerializeField]
    [Range(0f, 1f)]
    private float m_Decay = 0.05f;
    public float Decay
    {
        get { return m_Decay; }
    }

    private Vector3 m_Velocity = Vector3.zero;
    private Vector3 m_LastDirection = Vector3.forward;
    private Vector2 m_LastPlanarDirection = Vector2.up;
    private Vector3 m_PlanarNormal = Vector3.up;
    
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
    public Vector3 PlanarNormal
    {
        get { return m_PlanarNormal; }
        protected set { m_PlanarNormal = value; }
    }

    public Vector2 PlanarVelocity
    {
        get
        {
            // Maybe fixed? TODO: Make sure the PlanarVelocity is a vector along the PlanarNormal axis, and not just {x, z}.
            Vector3 ProjectedVelocity = Vector3.ProjectOnPlane(Velocity, PlanarNormal);
            ProjectedVelocity = ProjectedVelocity.normalized * Velocity.magnitude;

            return new Vector2(ProjectedVelocity.x, ProjectedVelocity.z);
        }
        set
        {
            Vector3 ProjectedVelocity = Vector3.ProjectOnPlane(Velocity, PlanarNormal);
            Vector3 FallVector = Velocity - ProjectedVelocity;
            
            Velocity = FallVector + new Vector3(value.x, 0, value.y);
        }
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
        get { return Quaternion.Inverse(transform.rotation) * Velocity; }
        set
        {
            Velocity = Quaternion.Inverse(transform.rotation) * value;
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
    private float m_JumpAcceleration = 10f;
    public float JumpAcceleration
    {
        get { return m_JumpAcceleration; }
    }
    [SerializeField]
    [Range(0f, 1f)]
    private float m_AirControl = 0.05f;
    public float AirControl
    {
        get { return m_AirControl; }
        protected set { m_AirControl = value; }
    }
    private float m_JumpTime = 0f;
    public float JumpTime
    {
        get { return m_JumpTime; }
        protected set { m_JumpTime = value; }
    }
    [SerializeField]
    [Range(0f, 1f)]
    private float m_JumpTimeMax = 0.2f;
    public float JumpTimeMax
    {
        get { return m_JumpTimeMax; }
    }
    private bool m_IsGrounded = true;
    public bool IsGrounded
    {
        get { return m_IsGrounded; }
        protected set { m_IsGrounded = value; }
    }
    private bool m_IsJumping = false;
    virtual public bool IsJumping
    {
        get { return m_IsJumping; }
        set
        {
            //if(IsGrounded && value)
            //{
            //    JumpTime = 0f;
            //}
            m_IsJumping = value;
        }
    }
    [SerializeField]
    private Vector3 m_PawnGravity = new Vector3(0f, -9.82f, 0f);
    public Vector3 PawnGravity
    {
        get { return m_PawnGravity; }
    }
    #endregion

    #region Move behaviour attributes
    [Header("Move behaviour attributes")]
    [SerializeField]
    private float m_WalkThreshold = 2f;
    public float WalkThreshold
    {
        get { return m_WalkThreshold; }
    }

    public bool IsWalking
    {
        get { return (IsGrounded && Speed > WalkThreshold && Speed < RunThreshold); }
    }

    [SerializeField]
    private float m_RunThreshold = 4f;
    public float RunThreshold
    {
        get { return m_RunThreshold; }
    }

    public bool IsRunning
    {
        get { return (IsGrounded && Speed >= RunThreshold); }
    }

    [SerializeField]
    private float m_TurnRate = 360f;
    public float TurnRate
    {
        get { return m_TurnRate; }
    }
    private Quaternion m_ViewRotation = new Quaternion();
    public virtual Quaternion ViewRotation
    {
        get { return m_ViewRotation; }
        protected set
        {
            m_ViewRotation = value;
            Quaternion NewPhysicsRotation = PhysicsBody.transform.rotation;
            NewPhysicsRotation.eulerAngles = new Vector3(NewPhysicsRotation.eulerAngles.x, ViewRotation.eulerAngles.y, NewPhysicsRotation.eulerAngles.z);
            
            PhysicsBody.transform.rotation = NewPhysicsRotation;
        }
    }
    #endregion
    #endregion
	
	#region Character attributes
    [Header ("Character Attributes")]

    [SerializeField]
    private float m_Health = 1f;
    [SerializeField]
    private float m_HealthMax = 1f;
    public virtual float Health
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
    public float HealthPercentage
    {
        get { return Health / m_HealthMax; }
    }

    public bool IsAlive
    {
        get { return Health > 0f; }
        set { Health = 0f; }
    }
    [SerializeField]
    [Range(0f, 1f)]
    private float m_Sanity;
    public float Sanity
    {
        get { return m_Sanity; }
        set { m_Sanity = Mathf.Clamp01(value); }
    }

    [SerializeField]
    [Range(0f, 1f)]
    private float m_Holy;
    public float Holy
    {
        get { return m_Holy; }
        set { m_Holy = Mathf.Clamp01(value); }
    }

    [SerializeField]
    [Range(0f, 1f)]
    private float m_Oil;
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

    private Vector2 m_InputView = Vector2.zero;
    // Input rotation in degrees.
    public Vector2 InputView
    {
        get { return m_InputView; }
        protected set { m_InputView = value; }
    }

    private Vector2 m_InputMoveDirection = Vector2.zero;
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
    private bool m_InputJump = false;
    public bool InputJump
    {
        get { return m_InputJump; }
        protected set { m_InputJump = value; }
    }
    private bool m_InputSprint = false;
    public bool InputSprint
    {
        get { return m_InputSprint; }
        protected set { m_InputSprint = value; }
    }
    private bool m_InputAttack = false;
    public bool InputAttack
    {
        get { return m_InputAttack; }
        protected set { m_InputAttack = value; }
    }
    private bool m_InputReloadBegin = false;
    public bool InputReloadBegin
    {
        get { return m_InputReloadBegin; }
        protected set { m_InputReloadBegin = value; }
    }
    private bool m_InputReloadEnd = false;
    public bool InputReloadEnd
    {
        get { return m_InputReloadEnd; }
        protected set { m_InputReloadEnd = value; }
    }
    private bool m_InputAim = false;
    public bool InputAim
    {
        get { return m_InputAim; }
        protected set { m_InputAim = value; }
    }
    private bool m_InputUse = false;
    public bool InputUse
    {
        get { return m_InputUse; }
        protected set { m_InputUse = value; }
    }
    private bool m_InputPause = false;
    public bool InputPause
    {
        get { return m_InputPause; }
        protected set { m_InputPause = value; }
    }

    #endregion

    #region Aiming Data
    [Header("Aim Stats")]
    [SerializeField]
    private bool m_IsAiming = false;
    public bool IsAiming
    {
        get { return m_IsAiming; }
        set { m_IsAiming = value; }
    }
    [SerializeField]
    [Range(1f, 100f)]
    private float m_AimPrecision = 10f;
    public float AimPrecision
    {
        get { return m_AimPrecision; }
        set { m_AimPrecision = value; }
    }


    #endregion

    #region Weapon Data

    [SerializeField]
    private Transform m_WeaponSlot;
    public Transform WeaponSlot
    {
        get { return m_WeaponSlot; }
        protected set { m_WeaponSlot = value; }
    }


    private Weapon m_EquippedWeapon;
    public Weapon EquippedWeapon
    {
        get { return m_EquippedWeapon; }
        set
        {
            if (EquippedWeapon)
            {
                // Unequip current weapon and put it on the same place the other weapon was at.
                EquippedWeapon.Handle.parent = value.Handle.parent;
                EquippedWeapon.Handle.position = value.Handle.position;
                EquippedWeapon.User = null;
            }

            // Set the new weapon as equipped weapon.
            m_EquippedWeapon = value;

            // Set the newly equipped weapons transform.
            EquippedWeapon.Handle.parent = WeaponSlot;
            EquippedWeapon.Handle.localPosition = Vector3.zero;
            EquippedWeapon.Handle.localRotation = new Quaternion();

            EquippedWeapon.User = this;
        }
    }

    private enum WeaponType : int { None, Crossbow, Claws }

    private WeaponType EquippedType
    {
        get
        {
            if(EquippedWeapon.GetType() == typeof(Crossbow))
            {
                return WeaponType.Crossbow;
            }
            return WeaponType.None;
        }
    }

    private int[] m_WeaponAmmo;
    public int[] WeaponAmmo
    {
        get { return m_WeaponAmmo; }
        private set { m_WeaponAmmo = value; }
    }
    public int CrossbowAmmo
    {
        get { return WeaponAmmo[(int)WeaponType.Crossbow]; }
        set { WeaponAmmo[(int)WeaponType.Crossbow] = value; }
    }

    
    public int EquippedAmmo
    {
        get { return WeaponAmmo[(int)EquippedType]; }
        set { WeaponAmmo[(int)EquippedType] = value; }
    }

    #endregion


    protected virtual void Start()
    {
        WeaponAmmo = new int[System.Enum.GetNames(typeof(WeaponType)).Length];
    }

    protected virtual void FixedUpdate()
    {
        if (IsAlive)
        {
            UpdateRotation();
            UpdatePawnState();
            UpdateMovement(Time.fixedDeltaTime);
            Aim();
        }
    }


    protected virtual void Update()
    {
        if (IsAlive)
        {
            UpdateInput();
            UpdateWeapon();
        }
    }


    public enum WeaponAction : int { Attack, ReloadBegin, ReloadEnd };
    protected void UseWeapon(WeaponAction a_Action)
    {
        if(EquippedWeapon == null)
        {
            return;
        }

        switch(a_Action)
        {
            case WeaponAction.Attack:
            {
                EquippedWeapon.Attack();
                break;
            }
            case WeaponAction.ReloadBegin:
            {
                EquippedWeapon.BeginReload();
                break;
            }
            case WeaponAction.ReloadEnd:
            {
                EquippedWeapon.StopReload();
                break;
            }
            default:
            {
                Debug.Log("You shouldn't be here... How the hell did you input an invalid enum value?");
                break;
            }
        }
    }

    protected virtual void Aim()
    {
        IsAiming = InputAim;
    }

    protected abstract void UpdateInput();
    
    protected virtual void UpdateMovement(float a_DeltaTime)
    {
        
        
        Vector2 MoveAdd = InputMoveDirection * Acceleration * a_DeltaTime * (IsGrounded ? 1.00f : AirControl);
        PlanarSpeed *= Mathf.Pow(1f - Decay * (IsGrounded ? 1.00f : AirControl), a_DeltaTime);

        if (MoveAdd == Vector2.zero || PlanarSpeed > MaxSpeed || (InputSprint && PlanarForwardVelocity.y < -((MaxSpeed - 0.05f) / SprintSpeedMultiplier)))
        {
            if (IsGrounded)
            {
                PlanarSpeed -= Mathf.Min(Deacceleration * a_DeltaTime, PlanarSpeed);
            }
        }
        else
        {
            PlanarForwardVelocity += MoveAdd;
            PlanarSpeed = Mathf.Min(MaxSpeed, PlanarSpeed);
        }

        if (IsGrounded)
        {
            
            if (!InputJump)
            {
                JumpTime = 0f;
            }
            if (JumpTime < JumpTimeMax)
            {
                IsJumping = InputJump;
            }


            Velocity += (PawnGravity / 10f) * a_DeltaTime;
        }
        else
        {
            Velocity += PawnGravity * a_DeltaTime;
        }
        
        if (IsJumping)
        {
            Velocity += Vector3.up * JumpAcceleration * a_DeltaTime;
            JumpTime += a_DeltaTime;
            if (!InputJump || JumpTime >= JumpTimeMax)
            {
                IsJumping = false;
            }
        }
        
        Controller.Move(Velocity * a_DeltaTime);
        Velocity = Controller.velocity;
    }

    protected virtual void UpdateRotation()
    {
        float InputX = InputView.x;
        float InputY = InputView.y;

        if (InputAim)
        {
            InputX /= AimPrecision;
            InputY /= AimPrecision;
        }


        //Vector2 OldPlanarForwardVelocity = PlanarForwardVelocity;

        //Vector3 OldPhysicsRotation = PhysicsBody.transform.eulerAngles;
        //Vector3 NewPhysicsRotation = new Vector3(OldPhysicsRotation.x, OldPhysicsRotation.y + InputX, OldPhysicsRotation.z);
        //PhysicsBody.transform.eulerAngles = NewPhysicsRotation;

        Vector3 OldViewRotation = ViewRotation.eulerAngles;
        if (OldViewRotation.x > 90f)
        {
            OldViewRotation.x -= 360f;
        }
        Vector3 NewViewRotation = new Vector3(Mathf.Clamp(OldViewRotation.x - InputY, -89.9f, 89.9f), OldViewRotation.y, OldViewRotation.z);


        float ViewX = NewViewRotation.x;
        float ViewY = PhysicsBody.transform.rotation.eulerAngles.y + InputX;

        Quaternion View = new Quaternion();
        View.eulerAngles = new Vector3(ViewX, ViewY, 0f);
        ViewRotation = View;

        //PlanarForwardVelocity = OldPlanarForwardVelocity;
    }

    protected virtual void UpdatePawnState()
    {
        if (transform.position.y < -100f)
        {
            if (Controller.tag == "Player")
            {
                IsAlive = false;
            }
            else
            {
                Destroy(Controller.gameObject);
            }
        }

        Vector3 TestPos = PhysicsBody.transform.position + new Vector3(0f, -Controller.height / 2 + Controller.radius - 0.05f, 0f);
        float TestRadius = Controller.radius;

        //Collider[] AllColliders = Physics.OverlapSphere(TestPos, TestRadius);
        RaycastHit[] GroundHits = Physics.SphereCastAll(TestPos, TestRadius, Vector3.down, 0.075f, LayerMask.GetMask("Default"));

        PlanarNormal = Vector3.up;

        IsGrounded = false;
        foreach (RaycastHit GroundHit in GroundHits)
        {
            if (!GroundHit.collider.isTrigger)
            {
                IsGrounded = true;
                PlanarNormal = GroundHit.normal;
                
                break;
            }
        }
        
        //IsGrounded = Controller.isGrounded;
    }

    protected virtual void UpdateWeapon()
    {
        if (InputAttack)
        {
            UseWeapon(WeaponAction.Attack);
        }
        else if (InputReloadBegin)
        {
            UseWeapon(WeaponAction.ReloadBegin);
        }
        else if (InputReloadEnd)
        {
            UseWeapon(WeaponAction.ReloadEnd);
        }
    }
}

