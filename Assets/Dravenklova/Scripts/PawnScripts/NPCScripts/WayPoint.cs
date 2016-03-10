using UnityEngine;
using System.Collections;
using Pathfinding;
using System.Collections.Generic;
using Pathfinding.Util;


public class WayPoint : MonoBehaviour
{
    
    Vector3[] Waypoints;
    ABPath[] Paths;


    int CompletedPaths = -1;
   

    public List<Vector3> VectorPath;
    public List<GraphNode> NodePath;

    public delegate void OnWaypointPathComplete(WayPoint p);

    OnWaypointPathComplete callback;

    public WayPoint(Vector3[] Waypoints, OnWaypointPathComplete callback)
    {
        this.Waypoints = Waypoints;
        this.callback = callback;

    }

    public bool HasError()
    {
        return CompletedPaths != Paths.Length;
    }

    public void StartPath(OnPathDelegate config = null)
    {
        if(CompletedPaths != -1)
        {
            throw new System.Exception("Do not start the path more then once");
        }
        CompletedPaths = 0;

        Paths = new ABPath[Waypoints.Length-1];
        for (int i = 0; i < Paths.Length; i++)
        {
            Paths[i] = ABPath.Construct(Waypoints[i], Waypoints[i + 1], OnPathComplete);
            if (config != null)
            {
                config(Paths[i]);
                // Here you should set all custom parameters for the path
                // e.g tags ??? 
                

                    
               
            }

            AstarPath.StartPath(Paths[i]);
        }
    }

    void OnPathComplete(Path p)
    {
        if(p.error)
        {
            for(int i = 0; i < Paths.Length; i++)
            {
                Paths[i].Error();
            }
            Completed();
            return;
        }
        ABPath path = p as ABPath;
        if(path == null)
        {
            throw new System.Exception("Only ABPaths should be returned to this object");
        }
        CompletedPaths++;

        if(CompletedPaths == Paths.Length)
        {
            Completed();
        }
    }
    void Completed()
    {
        if(CompletedPaths == Paths.Length)
        {
            VectorPath = ListPool<Vector3>.Claim();
            NodePath = ListPool<GraphNode>.Claim();

            for(int i = 0; i < Paths.Length; i++)
            {
                List<Vector3> vp = Paths[i].vectorPath;
                List<GraphNode> np = Paths[i].path;
                for(int j = 0; i <  vp.Count; j++)
                {
                    if (VectorPath.Count == 0 || VectorPath[VectorPath.Count-1] != vp[j]) VectorPath.Add(vp[j]);
                }
                
                for(int j = 0; j < np.Count; j++)
                {
                    if (NodePath.Count == 0 || NodePath[NodePath.Count - 1] != np[j]) NodePath.Add(np[j]);
                }      
            }
        }
        if(callback != null)
        {
            callback(this);

        }
    }
}
