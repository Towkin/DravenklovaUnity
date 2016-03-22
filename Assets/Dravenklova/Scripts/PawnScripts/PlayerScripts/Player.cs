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
            base.ViewRotation = value;
            Cam.transform.rotation = value;
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

    #endregion

    #region Player components
    [Header("Player Components")]
    

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


    protected override void Start ()
    {
        base.Start();

        FOVDefault = Cam.fieldOfView;
        EquippedWeapon = StartingWeapon;
        EquippedAmmo = StartingWeaponAmmo;
        Cursor.lockState = CursorLockMode.Locked;
    }
	
	protected override void Update ()
    {
        base.Update();

        if (Input.GetButtonDown("Menu"))
        {
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
    }
    

    protected override void FixedUpdate ()
    {
        base.FixedUpdate();

        RaycastHit[] ViewHits;
        //Ray Searching = new Ray(Cam.transform.position, Cam.transform.forward);
        //Debug.DrawRay(Cam.transform.position, Cam.transform.forward * SearchDist, Color.red, 2f);


        //Physics.CapsuleCast(Cam.transform.position, Cam.transform.forward.normalized * SearchDist, SearchWidth, Cam.transform.forward, out Spotted, SearchDist);

        //float SearchRadius = 0.4f;

        ViewHits = Physics.SphereCastAll(Cam.transform.position, SearchRadius, Cam.transform.forward, SearchDistance);

        if (ViewHits.Length > 0)
        {
            bool HasUsed = false;
            foreach(RaycastHit Hit in ViewHits)
            {

                RaycastHit PropHit;
                if (Physics.Raycast(Cam.transform.position, (Hit.point - Cam.transform.position).normalized, out PropHit, SearchDistance)
                && PropHit.transform != Hit.transform)
                {
                    // Ignore if something is in the way.
                    continue;
                }

                Usable Prop = Hit.collider.GetComponent<Usable>();
                if (Prop != null)
                {
                    // TODO: UI message informing that Item is usable
                    //Debug.Log(Prop);
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


        //TODO: Functionality for taking sanity damage
    }

    //void OnTriggerEnter(Collider Coll)
    //{
    //     TODO: Any and all code for player collision, if we decide to have such
    //}
    
    
    protected override void UpdateInput()
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

    protected override void Aim()
    {
        base.Aim();
        Cam.fieldOfView = Mathf.Lerp(Cam.fieldOfView, FOVTarget, 0.25f);
    }

}
