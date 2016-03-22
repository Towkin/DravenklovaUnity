using UnityEngine;
using Pathfinding;
using System.Collections;

[RequireComponent(typeof(Seeker))]
public class PathMoveBehaviour : MonoBehaviour
{
    private Seeker m_Pathfinder;
    private Seeker Pathfinder
    {
        get { return m_Pathfinder; }
        set { m_Pathfinder = value; }
    }

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

    [SerializeField]
    private float m_PathMaxDistance = 10f;
    private float PathMaxDistance
    {
        get { return m_PathMaxDistance; }
    }
    [SerializeField]
    private float m_PathMinDistance = 0.25f;
    private float PathMinDistance
    {
        get { return m_PathMinDistance; }
    }

    private int m_DetectionMask;
    private int DetectionMask
    {
        get { return m_DetectionMask; }
        set { m_DetectionMask = value; }
    }

    private bool m_EndOfPath = false;
    public bool EndOfPath
    {
        get { return m_EndOfPath; }
        private set { m_EndOfPath = value; }
    }
    private bool m_IsBusy = false;
    public bool IsBusy
    {
        get { return m_IsBusy; }
        protected set { m_IsBusy = value; }
    }

    void Awake () {
        Pathfinder = GetComponent<Seeker>();
        DetectionMask = LayerMask.GetMask("Interior/Wall", "Interior / Ceiling", "Interior/Obstacle");
        Debug.Log("DetectionaMask: " + DetectionMask.ToString());
	}

    public void StartNewPath(Vector3 a_Target)
    {
        //Debug.Log("New path requested to " + a_Target.ToString());
        if (!IsBusy)
        {
            IsBusy = true;
            Pathfinder.StartPath(transform.position, a_Target, OnPathComplete);
        }
    }
    public void OnPathComplete(Path a_Path)
    {
        IsBusy = false;
        if(!a_Path.error)
        {
            CurrentPath = a_Path;
            CurrentPathIndex = 0;
        }
    }
    public Vector3 UpdatePathDirection(Vector3 a_CurrentLocation, CapsuleCollider a_Capsule)
    {
        Vector3 Direction = Vector3.zero;

        if (CurrentPath == null)
        {
            return Direction;
        }
        EndOfPath = CurrentPathIndex >= CurrentPath.vectorPath.Count;
        if (EndOfPath)
        {
            return Direction;
        }

        
        // RaycastHit
        bool Hit = true;

        if ((CurrentPathIndex + 1) < CurrentPath.vectorPath.Count)
        {
            Ray CastRay = new Ray(a_CurrentLocation, (CurrentPath.vectorPath[CurrentPathIndex + 1] - a_CurrentLocation).normalized);
            float TestDistance = Vector3.Distance(CurrentPath.vectorPath[CurrentPathIndex + 1], a_CurrentLocation);
            float TestRadius = a_Capsule.radius + 0.05f;
            Hit = Physics.SphereCast(CastRay, TestRadius, TestDistance, DetectionMask);
            Color RayColor = Hit ? Color.red : Color.green;

            Debug.DrawRay(CastRay.origin + new Vector3(TestRadius, 0f, 0f), CastRay.direction * TestDistance, RayColor);
            Debug.DrawRay(CastRay.origin + new Vector3(-TestRadius, 0f, 0f), CastRay.direction * TestDistance, RayColor);
            Debug.DrawRay(CastRay.origin + new Vector3(0f, TestRadius, 0f), CastRay.direction * TestDistance, RayColor);
            Debug.DrawRay(CastRay.origin + new Vector3(0f, -TestRadius, 0f), CastRay.direction * TestDistance, RayColor);
            Debug.DrawRay(CastRay.origin + new Vector3(0f, 0f, TestRadius), CastRay.direction * TestDistance, RayColor);
            Debug.DrawRay(CastRay.origin + new Vector3(0f, 0f, -TestRadius), CastRay.direction * TestDistance, RayColor);
        }


        float Distance = (CurrentPath.vectorPath[CurrentPathIndex] - a_CurrentLocation).magnitude;
        if ((Distance < PathMaxDistance && !Hit) || Distance < PathMinDistance)
        {
            CurrentPathIndex++;
        }




        if (CurrentPathIndex < CurrentPath.vectorPath.Count)
        {
            Direction = (CurrentPath.vectorPath[CurrentPathIndex] - a_CurrentLocation).normalized;
        }

        return Direction;
    }

    void OnDisabled()
    {
        Pathfinder.pathCallback -= OnPathComplete;
    }
}
