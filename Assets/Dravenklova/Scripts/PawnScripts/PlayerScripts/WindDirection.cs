using UnityEngine;
using System.Collections;
using FMODUnity;
using FMOD.Studio;
using Pathfinding;

[RequireComponent(typeof(Seeker))]
public class WindDirection : MonoBehaviour {

    [SerializeField]
    [EventRef]
    private string m_WindEvent;
    public string WindEvent
    {
        get { return m_WindEvent; }
    }
    private EventInstance m_WindInstance;
    public EventInstance WindInstance
    {
        get { return m_WindInstance; }
        protected set { m_WindInstance = value; }
    }
    private Seeker m_Pathfinder;
    public Seeker Pathfinder
    {
        get { return m_Pathfinder; }
        protected set { m_Pathfinder = value; }
    }
    

    private Path m_ShrinePath;
    public Path ShrinePath
    {
        get { return m_ShrinePath; }
        protected set { m_ShrinePath = value; }
    }
    private int m_PathIndex = 0;
    public int PathIndex
    {
        get { return m_PathIndex; }
        protected set { m_PathIndex = value; }
    }
    [SerializeField]
    private float m_PathUpdateDistance = 2.5f;
    public float PathUpdateDistance
    {
        get { return m_PathUpdateDistance; }
    }

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


    private bool m_IsBusy = false;
    public bool IsBusy
    {
        get { return m_IsBusy; }
        protected set { m_IsBusy = value; }
    }

    void Start () {
        Pathfinder = GetComponent<Seeker>();
        FindPathToShrine();

        WindInstance = RuntimeManager.CreateInstance(WindEvent);
        WindInstance.start();
	}
	
	
	void Update () {
        if((Time.realtimeSinceStartup - LastPathUpdateTime) > PathUpdateTimeMin)
        {
            FindPathToShrine();
        }
	    
        while(ShrinePath.vectorPath.Count < PathIndex + 1 && Vector3.Distance(transform.position, ShrinePath.vectorPath[PathIndex+1]) < PathUpdateDistance)
        {
            PathIndex++;
        }

        Vector3 WindDirection = (ShrinePath.vectorPath[PathIndex] - transform.position).normalized;

        WindInstance.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position + WindDirection * 2.5f));
        Debug.DrawLine(transform.position, transform.position + WindDirection * 2.5f, Color.cyan);
	}

    public void StartNewPath(Vector3 a_Target)
    {
        if (!IsBusy)
        {
            IsBusy = true;
            Pathfinder.StartPath(transform.position, a_Target, OnPathComplete);
        }
    }
    public void OnPathComplete(Path a_Path)
    {
        IsBusy = false;
        if (!a_Path.error)
        {
            ShrinePath = a_Path;
            PathIndex = 0;
        }
    }

    void FindPathToShrine()
    {
        GameObject ShrineEntrance = GameObject.FindGameObjectWithTag("ShrineEntrance");
        if (ShrineEntrance == null)
        {
            return;
        }

        Vector3 ShrineEntrancePosition = ShrineEntrance.transform.position;

        StartNewPath(ShrineEntrancePosition);
    }

    void OnDestroy()
    {
        WindInstance.release();
    }
}
