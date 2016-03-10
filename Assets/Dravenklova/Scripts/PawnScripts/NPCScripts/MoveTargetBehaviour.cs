using UnityEngine;
using Pathfinding;
using System.Collections;

[RequireComponent(typeof(Seeker))]
public class MoveTargetBehaviour : MonoBehaviour {

    private Vector3 m_TargetLocation = new Vector3(5, 0, 5);
    private Vector3 TargetLocation
    {
        get { return m_TargetLocation; }
        set { m_TargetLocation = value; }
    }

    private Seeker m_Pathfinder;
    private Path m_CurrentPath;
    private Path CurrentPath
    {
        get { return m_CurrentPath; }
        set { m_CurrentPath = value; }
    }
    private int m_CurrentPathIndex = 0;
    private int CurrentPathIndex
    {
        get { return m_CurrentPathIndex; }
        set { m_CurrentPathIndex = value; }
    }
    
    private float m_PathMaxDistance = 2f;
    private float PathMaxDistance
    {
        get { return m_PathMaxDistance; }
    }

    private bool m_EndOfPath = false;
    public bool EndOfPath
    {
        get { return m_EndOfPath; }
        private set { m_EndOfPath = value; }
    }




    void Start () {
        m_Pathfinder = GetComponent<Seeker>();

        StartNewPath(TargetLocation);
	}

    public void StartNewPath(Vector3 a_Target)
    {
        TargetLocation = a_Target;
        m_Pathfinder.StartPath(transform.position, TargetLocation, OnPathComplete);
    }
	
    public void OnPathComplete(Path a_Path)
    {
        if(!a_Path.error)
        {
            CurrentPath = a_Path;
            CurrentPathIndex = 0;
        }
    }

	// Update is called once per frame
	void Update () {
	    if(CurrentPath == null)
        {
            return;
        }
        EndOfPath = CurrentPathIndex >= CurrentPath.vectorPath.Count;
        if (EndOfPath)
        {
            return;
        }

        Vector3 Direction = (CurrentPath.vectorPath[CurrentPathIndex] - transform.position).normalized;

        // Implement pawn move stuff yeah.

        if(Vector3.Distance(transform.position, CurrentPath.vectorPath[CurrentPathIndex]) < PathMaxDistance)
        {
            CurrentPathIndex++;
        }
	}
}
