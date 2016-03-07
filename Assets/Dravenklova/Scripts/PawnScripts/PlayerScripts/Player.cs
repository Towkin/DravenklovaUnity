using UnityEngine;
using System.Collections;

public class Player : Pawn {

    [Header("World interaction")]
    [SerializeField]
    private float m_SearchDist = 100f;
    public float SearchDist
    {
        get { return m_SearchDist; } 
    }

    #region Components
    [Header("Components")]
    [SerializeField]
    private Rigidbody m_PhysicsBody;
    public Rigidbody PhysicsBody
    {
        get { return m_PhysicsBody; }
        set { m_PhysicsBody = value; }
    }
    [SerializeField]
    private CapsuleCollider m_Collider;
    public CapsuleCollider Capsule
    {
        get { return m_Collider; }
        set { m_Collider = value; }
    }
    [SerializeField]
    private Camera m_Cam;
    public Camera Cam
    {
        get { return m_Cam; }
        set { m_Cam = value; }
    }
    [Header("Aim Stats")]
    [SerializeField]
    private float m_AimSpeed = 10f;
    public float AimSpeed
    {
        get { return m_AimSpeed; }
        set { m_AimSpeed = value; }
    }
    [SerializeField]
    private float m_AimMax = 25f;
    public float AimMax
    {
        get { return m_AimMax; }
        set { m_AimMax = value; }
    }
    [SerializeField]
    private float m_AimOrigin;
    public float AimOrigin
    {
        get { return m_AimOrigin; }
        set { m_AimOrigin = value; }
    }
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

    private float m_InputX;
    public float InputX
    {
        get { return m_InputX; }
        set { m_InputX = value; }
    }
    private float m_InputY;
    public float InputY
    {
        get { return m_InputY; }
        set { m_InputY = value; }
    }

    #endregion

    void Start ()
    {
        //Cursor.lockState = CursorLockMode.Locked;
        AimOrigin = Cam.fieldOfView;
        
    }
	
	void Update ()
    {
        //Debug.DrawRay(transform.position, ViewDirection, Color.black, 1f);
        
    }

    void FixedUpdate ()
    {
        UpdateInput();
        UpdateRotation();
        UpdatePawnState();
        UpdateMovement(Time.fixedDeltaTime);
        Aim();

        RaycastHit Spotted;
        Ray Searching = new Ray(m_Cam.transform.position, m_Cam.transform.forward);

        if (Physics.Raycast(Searching, out Spotted, SearchDist))
        {
            UsableProp Prop = Spotted.collider.GetComponent<UsableProp>();
            if (Prop != null)
            {
                // Prop is usable!



                //if(InputUseWorld)
                //{
                    // Not yet implemented.
                    //Prop.UseProp(this);
                    // TODO: Add Debug message
                //}
            }
        }


        //TODO: Functionality for taking sanity damage
    }

    //void OnTriggerEnter(Collider Coll)
    //{
    //     TODO: Any and all code for player collision, if we decide to have such
    //}
    private void UpdatePawnState()
    {
        Vector3 TestPos = PhysicsBody.transform.position + new Vector3(0f, -Capsule.height / 2 + Capsule.radius - 0.05f, 0f);
        float TestRadius = Capsule.radius - 0.025f;

        Collider[] AllColliders = Physics.OverlapSphere(TestPos, TestRadius);

        IsGrounded = false;
        foreach(Collider Collider in AllColliders)
        {
            if(Collider.tag != "Player")
            {
                IsGrounded = true;
                break;
            }
        }
    }
    private void UpdateMovement(float a_DeltaTime)
    {
        Velocity = PhysicsBody.velocity;
        
        
        Vector2 VelocityAdd = InputMoveDirection * Acceleration * a_DeltaTime * (IsGrounded ? 1.00f : AirControl);

        if (VelocityAdd == Vector2.zero || PlanarSpeed > MaxSpeed)
        {
            PlanarSpeed *= Mathf.Pow(1f - Decay, a_DeltaTime);
            PlanarSpeed -= Mathf.Min(Deacceleration * a_DeltaTime, Speed);
        }
        else
        {
            if(VelocityAdd.magnitude > 1f)
            {
                VelocityAdd.Normalize();
            }
            PlanarForwardVelocity += VelocityAdd;
            PlanarSpeed = Mathf.Min(MaxSpeed, PlanarSpeed);
        }
        
        if (IsGrounded)
        {
            IsJumping = InputJump;
        }
        else 
        {
            Velocity += PawnGravity * a_DeltaTime;
        }
        
        if(IsJumping)
        {
            Velocity += Vector3.up * JumpAcceleration * a_DeltaTime;
            JumpTime += a_DeltaTime;
            if(!InputJump || JumpTime >= JumpTimeMax)
            {
                IsJumping = false;
            }
        }

        PhysicsBody.velocity = Velocity;
    }
    private void UpdateInput()
    {
        InputMoveDirection = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        InputJump = Input.GetButton("Jump");
        InputSprint = Input.GetButton("Sprint");
    }
    private void UpdateRotation()
    {
        InputX = Input.GetAxis("Mouse X");
        InputY = Input.GetAxis("Mouse Y");
        
        if(IsAiming)
        {
            InputX /= AimPrecision;
            InputY /= AimPrecision;
        }
        

        Vector2 OldPlanarForwardVelocity = PlanarForwardVelocity;
        
        Vector3 OldPhysicsRotation = PhysicsBody.transform.eulerAngles;
        Vector3 NewPhysicsRotation = new Vector3(OldPhysicsRotation.x, OldPhysicsRotation.y + InputX, OldPhysicsRotation.z);
        PhysicsBody.transform.eulerAngles = NewPhysicsRotation;

        Vector3 OldCamRotation = Cam.transform.eulerAngles;
        if(OldCamRotation.x > 90f)
        {
            OldCamRotation.x -= 360f;
        }
        Vector3 NewCamRotation = new Vector3(Mathf.Clamp(OldCamRotation.x - InputY, -89.9f, 89.9f), OldCamRotation.y, OldCamRotation.z);
        Cam.transform.eulerAngles = NewCamRotation;

        float ViewX = NewCamRotation.x;
        float ViewY = NewPhysicsRotation.y;

        Quaternion View = new Quaternion();
        View.eulerAngles = new Vector3(ViewX, ViewY, 0f);
        ViewDirection = View * Vector3.forward;

        PlanarForwardVelocity = OldPlanarForwardVelocity;
    }
    private void Aim()
    {
        if (Input.GetButton("Aim"))
        {
            Cam.fieldOfView -= AimSpeed;
            IsAiming = true;
            if (Cam.fieldOfView <= AimMax)
            {
                Cam.fieldOfView = AimMax;
            }
        }
        if (!Input.GetButton("Aim"))
        {
            IsAiming = false;
            if (Cam.fieldOfView < AimOrigin)
            {
                Cam.fieldOfView += AimSpeed;
            }
        }
    }
}
