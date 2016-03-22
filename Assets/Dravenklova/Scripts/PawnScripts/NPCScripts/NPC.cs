using UnityEngine;

[RequireComponent(typeof(Animator))]
public class NPC : Pawn
{
    [Header("NPC Components")]
    [SerializeField]
    private Animator m_PawnAnimatior;
    public Animator PawnAnaimator
    {
        get { return m_PawnAnimatior; }
        protected set { m_PawnAnimatior = value; }
    }

    [SerializeField]
    private PathMoveBehaviour m_MovePath;
    public PathMoveBehaviour MovePath
    {
        get { return m_MovePath; }
    }

    private GameObject m_TargetObject;
    public GameObject TargetObject
    {
        get { return m_TargetObject; }
        set { m_TargetObject = value; }
    }

    private Vector3 m_TargetLocation = Vector3.zero;
    public Vector3 TargetLocation
    {
        get
        {
            if(TargetObject != null)
            {
                TargetLocation = TargetObject.transform.position;
            }
            return m_TargetLocation;
        }
        set
        {
            m_TargetLocation = value;
            // Note: Pathing is slightly delayed, and ignore requests if busy.
            MovePath.StartNewPath(value);
        }
    }
    public Vector3 TargetDirection
    {
        get { return (TargetLocation - transform.position).normalized; }
    }
    public float TargetDistance
    {
        get { return (TargetLocation - transform.position).magnitude; }
    }


    // Daniel Fucking around with variables

    [Header("Hunt Attributes")]
    [SerializeField]
    private float m_AttackRange = 1f;
    public float AttackRange
    {
        get { return m_AttackRange; }
    }

    [SerializeField]
    private GameObject m_Prey;
    public GameObject Prey
    {
        get { return m_Prey; }
        set { m_Prey = value; }
    }
    [SerializeField]
    private bool m_FindAndSetPlayerAsPrey = true;

    private bool m_PreyDetected;
    public bool PreyDetected
    {
        get { return m_PreyDetected; }
        protected set { m_PreyDetected = value; }
    }

    private bool m_IsHunting;
    public bool IsHunting
    {
        get { return m_IsHunting; }
        protected set { m_IsHunting = value; }
    }

    private float m_TargetRadius = 2.5f;
    public bool OnMoveTarget
    {
        get { return TargetDistance < m_TargetRadius; }
    }
    public bool InAttackRange
    {
        get { return TargetDistance < AttackRange; }
    }

    #region IdleTimer
    [Header("Idle attributes")]
    [SerializeField]
    private float m_IdleTimeMax = 2f;
    [SerializeField]
    private float m_IdleTimeVariance = 1f;
    public float NewIdleTime
    {
        get { return Random.Range(m_IdleTimeMax - m_IdleTimeVariance, m_IdleTimeMax + m_IdleTimeVariance); }
    }

    private float m_IdleTimer = 0f;
    public float IdleTimer
    {
        get { return m_IdleTimer; }
        protected set { m_IdleTimer = value; }
    }
    private bool m_IdleTimerActive = false;
    public bool IdleTimerActive
    {
        get { return m_IdleTimerActive; }
        protected set { m_IdleTimerActive = value; }
    }

    private float m_OnTimer;
    public float OnTimer
    {
        get { return m_OnTimer; }
        // set?
    }
    #endregion

    #region Detection attributes
    [Header("Detection Attributes")]
    [SerializeField]
    private float m_ViewConeAngle = 80f;
    public float ViewConeAngle
    {
        get { return m_ViewConeAngle; }
        protected set { m_ViewConeAngle = value; }
    }

    [SerializeField]
    private float m_ViewDistance = 40f;
    public float ViewDistance
    {
        get { return m_ViewDistance; }
        protected set { m_ViewDistance = value; }
    }

    [SerializeField]
    private LayerMask m_ViewBlockers;
    public LayerMask ViewBlockers
    {
        get { return m_ViewBlockers; }
    }
    [SerializeField]
    [Tooltip("Make sure the transform's forward (blue arrow) is pointing in the creatue's forward direction.")]
    private Transform m_HeadTransform;
    public Transform HeadTransform
    {
        get { return m_HeadTransform; }
    }


    #endregion

    protected override void Start()
    {
        base.Start();

        TargetLocation = GetRandomPointOfInterest();

        if(m_FindAndSetPlayerAsPrey)
        {
            Prey = GameObject.FindGameObjectWithTag("Player");
        }
    }

    protected override void UpdateInput()
    {
        PawnAnaimator.SetBool("IsDead", !IsAlive);
        if (!IsAlive)
        {
            InputMoveDirection = Vector2.zero;
            InputSprint = false;
            
            return;
        }

        UpdateDetection();

        InputAttack = false;

        if(PreyDetected)
        {
            if(IsHunting)
            {
                TargetObject = Prey;
                if (TargetDistance < AttackRange)
                {
                    InputAttack = true;
                }
            }
            else
            {
                StartHunting();
            }
        }
        else
        {
            TargetObject = null;
            if(OnMoveTarget)
            {
                if(IdleTimerActive)
                {
                    IdleTimer -= Time.deltaTime;
                    if(IdleTimer <= 0f)
                    {
                        IdleTimerActive = false;
                        if(IsHunting)
                        {
                            StopHunting();
                        }

                        TargetLocation = GetRandomPointOfInterest();
                    }
                }
                else
                {
                    IdleTimerActive = true;
                    IdleTimer = NewIdleTime;
                }
            }
        }

        Vector3 MoveDirection; ;
        if (TargetDistance > 2.5f)
        {
            MoveDirection = MovePath.UpdatePathDirection(transform.position, Capsule);
        }
        else
        {
            MoveDirection = TargetDirection;
            MovePath.UpdatePathDirection(transform.position, Capsule);
        }
        Vector3 RotatedDirection = Quaternion.Inverse(transform.rotation) * MoveDirection;

        
        InputMoveDirection = new Vector2(RotatedDirection.x, RotatedDirection.z).normalized;

        
        ViewRotation = Quaternion.RotateTowards(ViewRotation, ViewRotation * Quaternion.FromToRotation(transform.forward, MoveDirection), Time.deltaTime * 250f);

        InputSprint = IsHunting;



        PawnAnaimator.SetFloat("SpeedX", ForwardVelocity.x);
        PawnAnaimator.SetFloat("SpeedZ", ForwardVelocity.z);
        PawnAnaimator.SetBool("Attack", InputAttack);
    }




    protected void UpdateDetection()
    {
        PreyDetected = false;

        // If there is no prey.
        if(Prey == null)
        {
            return;
        }


        // If the prey is too far away.
        float PreyDistance = (Prey.transform.position - HeadTransform.position).magnitude;
        if (PreyDistance > ViewDistance)
        {
            return;
        }

        // If the prey is outside of the view angle.
        Vector3 PreyDirection = (Prey.transform.position - HeadTransform.position).normalized;
        float PreyAngle = Vector3.Angle(HeadTransform.forward, PreyDirection);
        if (PreyAngle > ViewConeAngle)
        {
            return;
        }

        // If there is anything blocking the NPC's line of sight.
        Ray ViewRay = new Ray(HeadTransform.position, PreyDirection);
        if(Physics.Raycast(ViewRay, PreyDistance, ViewBlockers))
        {
            return;
        }
        
        PreyDetected = true;
    }

    protected void StartHunting()
    {
        IsHunting = true;
    }
    protected void StopHunting()
    {
        IsHunting = false;
    }

    protected Vector3 GetRandomPointOfInterest()
    {
        GameObject[] Points = GameObject.FindGameObjectsWithTag("PointOfInterest");

        return Points[Random.Range(0, Points.Length)].transform.position;
    }
}
