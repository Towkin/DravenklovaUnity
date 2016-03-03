using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Camera))]
public class Player : Pawn {

    
    [SerializeField]
    private float m_SearchDist = 100f;
    public float SearchDist
    {
        get { return m_SearchDist; } 
    }

    #region Components
    private Rigidbody m_PhysicsBody;
    public Rigidbody PhysicsBody
    {
        get { return m_PhysicsBody; }
        set { m_PhysicsBody = value; }
    }
    private Camera m_Cam;
    public Camera Cam
    {
        get { return m_Cam; }
        set { m_Cam = value; }
    }
    #endregion

    void Start ()
    {
        Cam = GetComponent<Camera>();
        PhysicsBody = GetComponent<Rigidbody>();
	}
	
	void Update ()
    {
	    
	}

    void FixedUpdate ()
    {
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
                    //Prop.UseProp();
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
}
