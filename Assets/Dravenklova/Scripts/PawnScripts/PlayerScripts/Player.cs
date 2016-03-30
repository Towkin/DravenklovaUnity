using UnityEngine;
using System.Collections;

public class Player : Pawn {

    public override Quaternion ViewRotation
    {
        get
        {
            return base.ViewRotation;
        }
        protected set
        {
            if (IsAlive)
            {
                base.ViewRotation = value;
                //Cam.transform.rotation = value;
                HeadBobAnchor = value;
            }
        }
    }
    
    #region Player world interaction
    [Header("World interaction")]
    [SerializeField]
    private float m_SearchDistance = 5f;
    public float SearchDistance
    {
        get { return m_SearchDistance; } 
    }
    [SerializeField]
    private float m_SearchRadius = 0.4f;
    public float SearchRadius
    {
        get { return m_SearchRadius; }
    }
    [SerializeField]
    private LayerMask m_ItemLayerMask;
    public LayerMask ItemLayerMask
    {
        get { return m_ItemLayerMask; }
    }

    #endregion

    #region Player components
    [Header("Player Components")]

    [SerializeField]
    private StatScript m_HealthBar;
    private StatScript HealthBar
    {
        get { return m_HealthBar; }
    }
    [SerializeField]
    private FaderScript m_Fader;
    private FaderScript Fader
    {
        get { return m_Fader; }
    }
    public override float Health
    {
        get { return base.Health; }
        set
        {
            base.Health = value;

            HealthBar.CurrentVal = HealthPercentage;

            if(!m_GameOver && !IsAlive)
            {
                StartPlayerDeath();
            }
        }
    }

    protected bool m_GameOver = false;
    

    [SerializeField]
    private Weapon m_StartingWeapon;
    public Weapon StartingWeapon
    {
        get { return m_StartingWeapon; }
        set { m_StartingWeapon = value; }
    }
    [SerializeField]
    private int m_StartingWeaponAmmo = 5;
    protected int StartingWeaponAmmo
    {
        get { return m_StartingWeaponAmmo; }
    }
    #endregion

    #region Player aim attributes
    [Header("Player Aim Attributes")]
    [SerializeField]
    private Camera m_Cam;
    public Camera Cam
    {
        get { return m_Cam; }
        set { m_Cam = value; }
    }
    [SerializeField]
    private float m_FOVAimed = 45f;
    public float FOVAimed
    {
        get { return m_FOVAimed; }
        set { m_FOVAimed = value; }
    }
    [SerializeField]
    private float m_FOVDefault;
    public float FOVDefault
    {
        get { return m_FOVDefault; }
        set { m_FOVDefault = value; }
    }
    public float FOVTarget
    {
        get { return InputAim ? FOVAimed : FOVDefault; }
    }
    #endregion

    #region Player headbob

    [Header("Headbobbing effects")]
    [SerializeField]
    private float m_HeadBobAngleMin = 0.25f;
    [SerializeField]
    private float m_HeadBobAngleMax = 5f;

    public float HeadBobAngle
    {
        get
        {
            return Mathf.Lerp(
                m_HeadBobAngleMin,
                m_HeadBobAngleMax,
                HeadBobAmount
            );
        }
    }

    [SerializeField]
    private float m_HeadBobFrequencyMin = 0.25f;
    [SerializeField]
    private float m_HeadBobFrequencyMax = 0.85f;

    public float HeadBobFrequency
    {
        get
        {
            return Mathf.Lerp(
                m_HeadBobFrequencyMin,
                m_HeadBobFrequencyMax, 
                HeadBobAmount
            );
        }
    }

    public float HeadBobAmount
    {
        get {
            return IsRunning ?
                m_HeadBobRun : Mathf.Lerp(
                    m_HeadBobIdle,
                    m_HeadBobWalk,
                    Mathf.Clamp01(PlanarSpeed / 0.15f)
            );
        }
    }

    [SerializeField]
    private float m_HeadBobIdle = 0.0f;
    [SerializeField]
    private float m_HeadBobWalk = 0.25f;
    [SerializeField]
    private float m_HeadBobRun = 1f;

    private Quaternion m_HeadBobAnchor;
    public Quaternion HeadBobAnchor
    {
        get { return m_HeadBobAnchor; }
        protected set { m_HeadBobAnchor = value; }
    }

    /*private Quaternion m_HeadBobTarget;
    public Quaternion HeadBobTarget
    {
        get { return m_HeadBobTarget; }
        protected set { m_HeadBobTarget = value; }
    }*/
    #endregion

    protected override void Start ()
    {
        base.Start();

        FOVDefault = Cam.fieldOfView;
        EquippedWeapon = StartingWeapon;
        EquippedAmmo = StartingWeaponAmmo;
        Cursor.lockState = CursorLockMode.Locked;

        HealthBar.Initialize();

        Controller.transform.parent = null;
        if (Fader)
            Fader.FadeIn();
    }
	
	protected override void Update ()
    {
        base.Update();

        UseItems();
        UpdateHeadBob();

        if (Input.GetButtonDown("Menu"))
        {
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }

        if(Input.GetKeyDown(KeyCode.B))
        {
            Health -= 0.5f;
            Debug.Log(Health.ToString());
        }
    }
    
    
    protected override void UpdateInput()
    {
        if(IsAlive)
        {
            InputMoveDirection = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            InputJump = Input.GetButton("Jump");
            InputSprint = Input.GetButton("Sprint");
            InputAim = Input.GetButton("Aim");
            InputUse = Input.GetButtonDown("Use");
            InputAttack = Input.GetButtonDown("Attack");
            InputReload = Input.GetButtonDown("Reload");

            InputView = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        }
        else
        {
            InputMoveDirection = Vector2.zero;
            InputJump = false;
            InputSprint = false;
            InputAim = false;
            InputUse = false;
            InputAttack = false;
            InputReload = false;

            InputView = Vector2.zero;

        }

    }

    protected virtual void UseItems()
    {
        RaycastHit[] ViewHits;
        ViewHits = Physics.SphereCastAll(Cam.transform.position, SearchRadius, Cam.transform.forward, SearchDistance, ItemLayerMask);

        Debug.DrawRay(Cam.transform.position, Cam.transform.forward * SearchDistance, Color.green);

        bool HasUsed = false;
        foreach (RaycastHit Hit in ViewHits)
        {

            Usable Prop = Hit.collider.GetComponent<Usable>();
            if (Prop != null)
            {
                RaycastHit PropHit;
                if (Physics.Raycast(Cam.transform.position, (Hit.point - Cam.transform.position).normalized, out PropHit, SearchDistance, ItemLayerMask)
                && PropHit.transform != Hit.transform)
                {
                    // Ignore if something is in the way.
                    Debug.Log(PropHit.transform.gameObject.ToString() + " is in the way of " + Prop.gameObject.ToString());

                    continue;
                }
                
                // TODO: UI message informing that Item is usable
                Debug.Log(Prop);
                Prop.StartGlow();
                Debug.DrawLine(Cam.transform.position, Prop.transform.position, Color.blue);

                if (InputUse && !HasUsed)
                {
                    Prop.Use(this);
                    HasUsed = true;
                }
            }
        }
    }

    protected override void Aim()
    {
        base.Aim();
        Cam.fieldOfView = Mathf.Lerp(Cam.fieldOfView, FOVTarget, 0.25f);
    }
    
    protected void UpdateHeadBob()
    {
        Quaternion HeadBob = HeadBobAnchor;

        Vector3 EulerBob = HeadBob.eulerAngles;
        EulerBob.x += Mathf.Sin(Time.realtimeSinceStartup * HeadBobFrequency) * HeadBobAngle;

        HeadBob.eulerAngles = EulerBob;

        Cam.transform.rotation = HeadBob;
    }

    protected void StartPlayerDeath()
    {
        Debug.Log("Player died!");

        m_GameOver = true;
        if(Fader)
            Fader.FadeOut();

        Controller.enabled = false;
        Controller.gameObject.GetComponent<CapsuleCollider>().enabled = true;


        PhysicsBody.isKinematic = false;
        PhysicsBody.velocity = Velocity;
        PhysicsBody.useGravity = true;
        PhysicsBody.angularDrag = 0.5f;
        PhysicsBody.drag = 0.5f;
        PhysicsBody.AddForce(PhysicsBody.transform.forward * -5000f);
    }
}
