using UnityEngine;
using System.Collections;

public class FootstepScript : MonoBehaviour {

    [SerializeField]
    private GameObject m_FootstepEvent;
    public GameObject FootstepEvent
    {
        get { return m_FootstepEvent; }
    }

    [SerializeField]
    private float m_FootstepDeltaMin = 0.8f;
    public float FootstepDeltaMin
    {
        get { return m_FootstepDeltaMin; }
    }
    [SerializeField]
    private float m_FootstepDeltaMax = 1.2f;
    public float FootstepDeltaMax
    {
        get { return m_FootstepDeltaMax; }
    }
    [SerializeField]
    private float m_FootstepSpeedThreshold = 0.15f;
    public float FootstepSpeedThreshold
    {
        get { return m_FootstepSpeedThreshold; }
    }
    [SerializeField]
    private float m_FootstepSpeedMin = 1.5f;
    public float FootstepSpeedMin
    {
        get { return m_FootstepSpeedMin; }
    }
    [SerializeField]
    private float m_FootstepSpeedMax = 4f;
    public float FootstepSpeedMax
    {
        get { return m_FootstepSpeedMax; }
    }
    [SerializeField]
    private Pawn m_PawnScript;
    public Pawn PawnScript
    {
        get { return m_PawnScript; }
    }
    [SerializeField]
    private Camera m_PlayerCamera;
    public Camera PlayerCamera
    {
        get { return m_PlayerCamera; }
    }


    public float FootstepDelta
    {
        get
        {
            return Mathf.Lerp(
                FootstepDeltaMin, 
                FootstepDeltaMax,
                Mathf.Clamp01(
                    Mathf.InverseLerp(
                        FootstepSpeedMin, 
                        FootstepSpeedMax, 
                        PawnScript.PlanarSpeed
                    )
                )
            );
        }
    }
    private float m_LastFootstep = 0f;
    public float LastFootstep
    {
        get { return m_LastFootstep; }
        protected set { m_LastFootstep = value; }
    }

    
	void Update ()
    {
	    if(PawnScript == null)
        {
            return;
        }

        if(Time.timeScale > 0 && PawnScript.IsGrounded && PawnScript.PlanarSpeed > FootstepSpeedThreshold)
        {
            if((Time.realtimeSinceStartup - LastFootstep) > FootstepDelta)
            {
                Instantiate(FootstepEvent, transform.position, transform.rotation);
                LastFootstep = Time.realtimeSinceStartup;
            }
        }
	}
}
