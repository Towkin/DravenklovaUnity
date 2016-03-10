using UnityEngine;
using System.Collections;

public class Player : Pawn {

    [Header("World interaction")]
    [SerializeField]
    private float m_SearchDist = 9999f;
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
    private Weapon m_StartingWeapon;
    public Weapon StartingWeapon
    {
        get { return m_StartingWeapon; }
        set { m_StartingWeapon = value; }
    }
    #endregion

    public enum WeaponAction : int { Attack = 0, Reload = 1 };
    private int a_WeaponAction;


    void Start ()
    {
        FOVDefault = Cam.fieldOfView;
        FOVTarget = FOVDefault;
        EquippedWeapon = StartingWeapon;
        Cursor.lockState = CursorLockMode.Locked;
        AmmoActive = AmmoCrossbow;
    }
	
	void Update ()
    {
        if (Input.GetButtonDown("Attack"))
        {
            a_WeaponAction = (int)WeaponAction.Attack;
            UseWeapon(a_WeaponAction);
        }
        else if (Input.GetButtonDown("Reload"))
        {
            a_WeaponAction = (int)WeaponAction.Reload;
            UseWeapon(a_WeaponAction);
        }

        if (Input.GetButtonDown("Menu"))
        {
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
    }

    void FixedUpdate ()
    {
        
        UpdateInput();
        UpdateRotation();
        UpdatePawnState();
        UpdateMovement(Time.fixedDeltaTime);
        Aim();

        RaycastHit Spotted;
        Ray Searching = new Ray(Cam.transform.position, Cam.transform.forward);
        Debug.DrawRay(Cam.transform.position, Cam.transform.forward * SearchDist, Color.red, 2f);

        if (Physics.Raycast(Searching, out Spotted, SearchDist))
        {
            
            Usable Prop = Spotted.collider.GetComponent<Usable>();
            if (Prop != null)
            {
                // TODO: UI message informing that Item is usable
                Debug.Log(Prop);

                if (InputUse)
                {
                    Debug.Log("Hit!");
                    Prop.Use(this);
                    // TODO: Add Debug message
                }
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
            PlanarSpeed -= Mathf.Min(Deacceleration * a_DeltaTime, PlanarSpeed);
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
        InputAim = Input.GetButton("Aim");
        InputUse = Input.GetButton("Use");
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
    
}
