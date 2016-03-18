using UnityEngine;
using Pathfinding;
using System.Collections;

[RequireComponent(typeof(Seeker))]
[RequireComponent(typeof(NPC))]
public class MoveTargetBehaviour : MonoBehaviour
{
    

    private Vector3 m_TargetLocation;
    private Vector3 TargetLocation
    {
        get { return m_TargetLocation; }
        set { m_TargetLocation = value; }
    }
    private NPC m_Controller;
    private NPC Controller
    {
        get { return m_Controller; }
        set { m_Controller = value; }
    }
    

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

    private float m_PathMaxDistance = 10f;
    private float PathMaxDistance
    {
        get { return m_PathMaxDistance; }
    }

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


    private GameObject GetRandomPointOfInterest()
    {
        GameObject[] Points = GameObject.FindGameObjectsWithTag("PointOfInterest");

        return Points[Random.Range(0, Points.Length)];
    }

    void Start () {
        Pathfinder = GetComponent<Seeker>();
        Controller = GetComponent<NPC>();
        DetectionMask = LayerMask.GetMask("Interior/Wall", "Interior / Ceiling", "Interior/Obstacle");
        Debug.Log("DetectionaMask: " + DetectionMask.ToString());

        StartNewPath(GetRandomPointOfInterest().transform.position);
	}

    public void StartNewPath(Vector3 a_Target)
    {
        TargetLocation = a_Target;
        Pathfinder.StartPath(transform.position, TargetLocation, OnPathComplete);
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
            if (Pathfinder.IsDone())
            {
                StartNewPath(GetRandomPointOfInterest().transform.position);
            }
            Controller.TargetDirection = Vector3.zero;
            return;
        }

        Controller.TargetDirection = (CurrentPath.vectorPath[CurrentPathIndex] - transform.position).normalized;
        Controller.TargetDistance = Vector3.Distance(transform.position, CurrentPath.vectorPath[CurrentPathIndex]);

        // Implement pawn move stuff yeah.

        bool Hit = true;

        if ((CurrentPathIndex + 1) < CurrentPath.vectorPath.Count)
        {
            Ray CastRay = new Ray(transform.position, (CurrentPath.vectorPath[CurrentPathIndex + 1] - transform.position).normalized);
            float Distance = Vector3.Distance(CurrentPath.vectorPath[CurrentPathIndex + 1], transform.position);
            float Radius = Controller.Capsule.radius + 0.05f;
            Hit = Physics.SphereCast(CastRay, Radius, Distance, DetectionMask);
            Color RayColor = Hit ? Color.red : Color.green;
            
            Debug.DrawRay(CastRay.origin + new Vector3(Radius, 0f, 0f), CastRay.direction * Distance, RayColor);
            Debug.DrawRay(CastRay.origin + new Vector3(-Radius, 0f, 0f), CastRay.direction * Distance, RayColor);
            Debug.DrawRay(CastRay.origin + new Vector3(0f, Radius, 0f), CastRay.direction * Distance, RayColor);
            Debug.DrawRay(CastRay.origin + new Vector3(0f, -Radius, 0f), CastRay.direction * Distance, RayColor);
            Debug.DrawRay(CastRay.origin + new Vector3(0f, 0f, Radius), CastRay.direction * Distance, RayColor);
            Debug.DrawRay(CastRay.origin + new Vector3(0f, 0f, -Radius), CastRay.direction * Distance, RayColor);
        }

        if ((Controller.TargetDistance < PathMaxDistance && !Hit) || Controller.TargetDistance < PathMinDistance)
        {
            CurrentPathIndex++;
        }

    }

    void OnDisabled()
    {
        Pathfinder.pathCallback -= OnPathComplete;
    }
}
