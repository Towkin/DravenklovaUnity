using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;


public class MultiTargetPathExample : MonoBehaviour
{

    

    // Use this for initialization
    void Start()
    {
        // Find the seeker component
        Seeker seeker = GetComponent<Seeker>();

        // Make sure all OnComplete calls are called to the OnPathComplete functuon
        seeker.pathCallback = OnPathComplete;

        // Set the target points to all children of this GameObject
        Vector3[] endPoints = new Vector3[transform.childCount];
        int c = 0;

        foreach (Transform child in transform)
        {
            endPoints[c] = child.position;
            c++;
        }

        // Start a multi target path
        seeker.StartMultiTargetPath(transform.position, endPoints, true, null, -1);

        // Alternative - Create a MultiTargetPath from scratch instead
        //MultiTargetPath p = new MultiTargetPath (transform.position, endPoints, null, null);
        //seeker.StartMultiTargetPath (p);


    }

    public void OnPathComplete(Path p)
    {
        Debug.Log("Got callback");

        if (p.error)
        {
            Debug.Log("Ouch, the path returned an error\nError: " + p.errorLog);
            return;
        }
        MultiTargetPath mp = p as MultiTargetPath;
        if(mp == null)
        {
            Debug.LogError("The path was no MultiTargetPath");
            return;

        }
        // All paths
        List<Vector3>[] paths = mp.vectorPaths;

        for(int i = 0; i < paths.Length; i++)
        {
            //Plotting path i
            List<Vector3> path = paths[i];

            if(path == null)
            {
                Debug.Log("Path number " + i + " could not be found");
                continue;
            }

            for (int j = 0; j < path.Count-1; j++)
            {
                //Plot segment j to j+1 with a nice color got from Pathfinding.AstarMath.IntToColor
                Debug.DrawLine(path[j], path[j + 1], AstarMath.IntToColor(i, 0.5F));

            }
        }
    }

}
