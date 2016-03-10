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

    

    void Start ()
    {
        FOVDefault = Cam.fieldOfView;
        EquippedWeapon = StartingWeapon;
        Cursor.lockState = CursorLockMode.Locked;
    }
	
	void Update ()
    {
        UpdateInput();
        UpdateWeapon();

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
    
    
}
