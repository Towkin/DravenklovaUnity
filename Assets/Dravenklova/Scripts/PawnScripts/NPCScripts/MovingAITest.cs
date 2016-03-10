using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;

public class MovingAITest : MonoBehaviour
{
    // The point to move to
    public Vector3 targetPosition;

    private Seeker seeker;
    private CharacterController controller;

    // The calculated path
    public Path m_Path;

    // The AI´s speed per secound
    public float m_Speed = 100;

    // the waypoints

    public Transform[] m_Waypoints;

    // The waypoint we are currently moving towards
    private int m_CurrentWaypoint = 0;

    // The max distance form the AI to a waypoint for it to continue to the next waypoint
    public float m_NextWaypointDistance = 3;

    
    public void Patrol()
    {

        if(m_CurrentWaypoint == 0)
        {
            Debug.Log("No waypoints");
            return;
        }


        if (m_CurrentWaypoint >= m_Waypoints.Length)
        {
            m_CurrentWaypoint = 0;
        }
        seeker.StartPath(transform.position, m_Waypoints[m_CurrentWaypoint].position, OnPathComplete);
        //Vector3.MoveTowards(Waypoints[CurrentWaypoint].position, Waypoints[CurrentWaypoint + 1].position, Speed);

    }
    public void Start()
    {

        
        // Get a reference to the seeker component we added earlier
        Seeker seeker = GetComponent<Seeker>();
        controller = GetComponent<CharacterController>();

        // Start a new path to the targetPosition, return the result to the OnPathComplete function
        seeker.StartPath(transform.position, m_Waypoints[m_CurrentWaypoint].position, OnPathComplete);
        //seeker.StartPath (transform.position, targetPosition, OnPathComplete);
        
    }
    public void OnPathComplete(Path p)
    {
        Debug.Log("We got a path back, Errors? " + p.error);
        if (!p.error)
        {
            m_Path = p;
            // Reset the waypoint counter
            m_CurrentWaypoint = 0;
        }
    }

    public void Update()
    {
     
        if (m_Path == null)
        {
            //Debug.Log("hello");
            // We have no path to move after yet
            return;
        }
        if(m_CurrentWaypoint >= m_Path.vectorPath.Count)
        {
            Debug.Log("End of path");
         
            return;
           
        }

        // Direction to the next waypoint
        Vector3 dir = (m_Path.vectorPath[m_CurrentWaypoint] - transform.position).normalized;
        dir *= m_Speed * Time.deltaTime;
        controller.SimpleMove(dir);


        // Check if we are close enough to the next waypoint
        // If we are, proceed to follow the next waypoint
        if(Vector3.Distance (transform.position,m_Path.vectorPath[m_CurrentWaypoint]) < m_NextWaypointDistance)
        {
            m_CurrentWaypoint++;
            return;
        }

       
    }
}
