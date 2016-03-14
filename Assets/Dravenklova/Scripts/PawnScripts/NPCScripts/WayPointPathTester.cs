using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WayPointPathTester : MonoBehaviour
{
    public Transform[] targets;

	// Use this for initialization
	void Start ()
    {
        Vector3[] ps = new Vector3[targets.Length];
        for(int i = 0; i < targets.Length; i++)
        {
            ps[i] = targets[i].position;

        }

        WayPoint p = new WayPoint(ps, OnPathComplete);
        p.StartPath();
    }

    void OnPathComplete(WayPoint p)
    {
        if (p.HasError())
        {
            Debug.LogError("Noes, could not find the path");
            return;
        }
        else
        {
            List<Vector3> vp = p.VectorPath;
            for(int i = 0; i < vp.Count-1; i++)
            {
                Debug.DrawLine(vp[i], vp[i + 1], Color.red, 2);
            }
        }
    }
}
