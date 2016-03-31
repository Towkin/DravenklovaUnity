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

    private Vector3 m_LastTargetLocation = Vector3.zero;
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
            if(m_LastTargetLocation != value && (Time.realtimeSinceStartup - LastPathUpdateTime) > PathUpdateTimeMin)
            {
                // Note: Pathing is slightly delayed, and ignore requests if busy.
                MovePath.StartNewPath(value);
                LastPathUpdateTime = Time.realtimeSinceStartup;

            }
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

    

    [Header("Hunt Attributes")]
    [SerializeField]
    private float m_AttackRange = 1f;
    public float AttackRange
    {
        get { return m_AttackRange; }
    }
    private float m_AggroRange = 3f;
    public float AggroRange
    {
        get { return m_AggroRange; }
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
    private float m_StraightLineRadiusEntry = 10f;
    private float m_StraightLineRadiusExit = 20f;
    private bool m_StraightLineCurrently = false;
    public bool UseStraightLine
    {
        get
        {
            float TestDistance = m_StraightLineRadiusEntry;
            if (m_StraightLineCurrently)
            {
                TestDistance = m_StraightLineRadiusExit;
            }
            m_StraightLineCurrently = TargetDistance < TestDistance;

            return m_StraightLineCurrently;
        }
    }

    public bool InAttackRange
    {
        get { return TargetDistance < AttackRange; }
    }
    public bool InAggroRange
    {
        get { return TargetDistance < AggroRange; }
    }

    #region IdleTimer
    [Header("Idle attributes")]
    [SerializeField]
    private float m_IdleTimeMax = 3f;
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

    #region StuckTimer
    private float m_StuckTimeMax = 10f;
    public float StuckTimerMax
    {
        get { return m_StuckTimeMax; }
    }
    private bool m_Stuck = false;
    public bool Stuck
    {
        get { return m_Stuck; }
        protected set
        {
            if (!m_Stuck && value)
            {
                StuckTimer = StuckTimerMax;
            }
            m_Stuck = value;
        }
    }
    private float m_StuckTimer = 0f;
    public float StuckTimer
    {
        get { return m_StuckTimer; }
        protected set { m_StuckTimer = value; }
    }

    #endregion

    #region PathUpdate attributes
    private float m_LastPathUpdateTime = 0f;
    public float LastPathUpdateTime
    {
        get { return m_LastPathUpdateTime; }
        protected set { m_LastPathUpdateTime = value; }
    }
    private float m_PathUpdateTimeMin = 0.1f;
    public float PathUpdateTimeMin
    {
        get { return m_PathUpdateTimeMin; }
        protected set { m_PathUpdateTimeMin = value; }
    }
    [SerializeField]
    private float m_POIMaxDistance = 125f;
    public float POIMaxDistance
    {
        get { return m_POIMaxDistance; }
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
    [SerializeField]
    private Transform m_BaseTransform;
    public Transform BaseTransform
    {
        get { return m_BaseTransform; }
    }


    #endregion

    protected override void Start()
    {
        base.Start();

        TargetLocation = GetRandomPointOfInterest();
    }

    protected override void Update()
    {
        base.Update();
        UpdateAnimator();
    }

    protected virtual void UpdateAnimator()
    {
        PawnAnaimator.SetBool("IsDead", !IsAlive);
        PawnAnaimator.SetFloat("SpeedX", Mathf.Lerp(PawnAnaimator.GetFloat("SpeedX"), ForwardVelocity.x, 0.5f));
        PawnAnaimator.SetFloat("SpeedZ", Mathf.Lerp(PawnAnaimator.GetFloat("SpeedZ"), ForwardVelocity.z, 0.5f));
        PawnAnaimator.SetBool("Attack", InputAttack);
        
        
        //PawnAnaimator.SetBool("WasHit", )
    }

    protected override void UpdateInput()
    {
        if (!IsAlive)
        {
            return;
        }

        
        UpdateDetection();

        InputAttack = false;

        if (PreyDetected)
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
                    //Debug.Log("Idle Time left: " + IdleTimer.ToString());
                    if(IdleTimer <= 0f)
                    {
                        IdleTimerActive = false;
                        //Debug.Log("Deactivated idle");
                        if (IsHunting)
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
                    //Debug.Log("Activated idle for " + IdleTimer.ToString() + " seconds");
                }
            }
            else
            {
                Stuck = Speed < 0.5f;
                if (Stuck)
                {
                    StuckTimer -= Time.deltaTime;
                    if(StuckTimer <= 0f)
                    {
                        TargetLocation = GetRandomPointOfInterest();
                    }
                }
            }
        }

        // Note: In the UpdatePathDirection-function must be run at least once per frame.
        Vector3 MoveDirection = MovePath.UpdatePathDirection(transform.position, Controller.radius);
        if (UseStraightLine && !Physics.Raycast(HeadTransform.position, (TargetLocation - HeadTransform.position).normalized, (TargetLocation - HeadTransform.position).magnitude, ViewBlockers)
            && !Physics.Raycast(BaseTransform.position, (TargetLocation - BaseTransform.position).normalized, (TargetLocation - BaseTransform.position).magnitude, ViewBlockers))
        {
            
            MoveDirection = TargetDirection;
        }

        Vector3 RotatedDirection = Quaternion.Inverse(transform.rotation) * MoveDirection;

        
        InputMoveDirection = new Vector2(RotatedDirection.x, RotatedDirection.z).normalized * Mathf.Clamp01(TargetDistance - 1.5f);

        //new Vector2(Vector2.Angle(new Vector2(1f, 0f), InputMoveDirection), 0f);

        Vector2 Forward2D = new Vector2(transform.forward.x, transform.forward.z);
        Vector2 Move2D = new Vector2(RotatedDirection.x, RotatedDirection.z);
        
        InputView = new Vector2(
            Mathf.Clamp(
                (360f * Mathf.Atan2(Move2D.x, Move2D.y) / (Mathf.PI * 2f)),
                -Time.deltaTime * TurnRate,
                Time.deltaTime * TurnRate
            ), 
            0f
        );

        InputSprint = IsHunting;

        Debug.DrawRay(transform.position, TargetDirection * TargetDistance, Color.blue, 0.2f);

        
    }


    protected void UpdateTarget()
    {

    }

    protected void UpdateDetection()
    {
        PreyDetected = false;

        // If there is no prey.
        if(Prey == null)
        {
            if (m_FindAndSetPlayerAsPrey)
            {
                Prey = GameObject.FindGameObjectWithTag("Player");
            }
            return;
        }

        // If the prey is too far away.
        float PreyDistance = (Prey.transform.position - HeadTransform.position).magnitude;
        if (PreyDistance > ViewDistance)
        {
            Debug.DrawLine(HeadTransform.position, Prey.transform.position, Color.white);
            return;
        }

        // If the prey is outside of the view angle.
        Vector3 PreyDirection = (Prey.transform.position - HeadTransform.position).normalized;
        float PreyAngle = Vector3.Angle(HeadTransform.forward, PreyDirection);
        if (PreyAngle > ViewConeAngle)
        {
            Debug.DrawLine(HeadTransform.position, Prey.transform.position, Color.yellow);
            return;
        }

        // If there is anything blocking the NPC's line of sight.
        Ray ViewRay = new Ray(HeadTransform.position + HeadTransform.forward * 0.5f, PreyDirection);
        if(Physics.Raycast(ViewRay, PreyDistance, ViewBlockers))
        {
            Debug.DrawLine(HeadTransform.position, Prey.transform.position, Color.red);
            return;
        }

        Debug.DrawLine(HeadTransform.position, Prey.transform.position, Color.green);

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

        System.Collections.Generic.List<GameObject> PointsInRange = new System.Collections.Generic.List<GameObject>();

        foreach(GameObject Point in Points)
        {
            if((Point.transform.position - transform.position).magnitude < POIMaxDistance)
            {
                PointsInRange.Add(Point);
            }
        }

        if (PointsInRange.Count <= 1)
        {
            return Points[Random.Range(0, Points.Length)].transform.position;
        }
        
        return PointsInRange[Random.Range(0, PointsInRange.Count)].transform.position;

    }

   
    
    void OnCollisionEnter(Collision Coll)
    {
        if (Coll.gameObject.GetComponent<Bolt>())
        {
            IsAlive = false;
            InputAttack = false;
            InputSprint = false;
            InputView = Vector2.zero;
            InputMoveDirection = Vector2.zero;

            Controller.enabled = false;
        }
    }
}
