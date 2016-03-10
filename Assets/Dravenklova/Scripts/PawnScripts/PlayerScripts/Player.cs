using UnityEngine;
using System.Collections;

public class Player : Pawn {

    public override Quaternion ViewDirection
    {
        get
        {
            return base.ViewDirection;
        }
        protected set
        {
            Cam.transform.rotation = value;
            m_ViewDirection = value;
        }
    }

    #region Player world interaction
    [Header("World interaction")]
    [SerializeField]
    private float m_SearchDist = 9999f;
    public float SearchDist
    {
        get { return m_SearchDist; } 
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


    void Start ()
    {
        FOVDefault = Cam.fieldOfView;
        EquippedWeapon = StartingWeapon;
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
