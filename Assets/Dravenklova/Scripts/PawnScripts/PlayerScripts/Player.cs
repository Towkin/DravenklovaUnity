using UnityEngine;
using System.Collections;

public class Player : Pawn {

    Camera m_Cam = Camera.main;
    [SerializeField]
    private float m_SearchDist = 100f;
    public float SearchDist
    {
        get { return m_SearchDist; } 
    }

	// Use this for initialization
	void Start ()
    {
	    
	}
	
	// Update is called once per frame
	void Update () {
	
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
}
