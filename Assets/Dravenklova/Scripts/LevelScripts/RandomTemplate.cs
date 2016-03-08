﻿using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(RandomTemplate))]
public class RandomTemplateInspector : Editor
{
    private RandomTemplate m_Creator;

    private void OnEnable()
    {
        m_Creator = target as RandomTemplate;
    }

    private void RefreshCreator()
    {
        m_Creator.CalculateBounds();
    }

    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();
        if(GUILayout.Button("Random Object"))
        {
            m_Creator.CreateTestObject();
        }
        if(GUILayout.Button("Clear Object"))
        {
            m_Creator.ClearTestObject();
        }
        DrawDefaultInspector();
        if(EditorGUI.EndChangeCheck())
        {
            RefreshCreator();
        }
    }
    
    private void OnSceneGUI()
    {
        
    }
}

public class RandomTemplate : MonoBehaviour
{
    [SerializeField] [Tooltip("Add elements to the list, and it'll randomly select one to spawn on game start.\nNote: the spawn object will copy the transform of the GameObject this script is attached to.\nNote2: Right-click the field and select 'Test Random Object' to try it out!")]
    [ContextMenuItem("Test Random Object", "CreateTestObject")]
    [ContextMenuItem("Clear Random Object", "ClearTestObject")]
    protected GameObject[] m_TemplateList;
    protected GameObject[] TemplateList
    {
        get { return m_TemplateList; }
    }
    private GameObject m_EditorTestObject;
    private GameObject EditorTestObject
    {
        get { return m_EditorTestObject; }
        set { m_EditorTestObject = value; }
    }

    private bool m_IsBoundsCalculated = false;
    public bool IsBoundsCalculated
    {
        get { return m_IsBoundsCalculated; }
        private set { m_IsBoundsCalculated = value; }
    }

    private Bounds m_SpawnBounds;
    public Bounds SpawnBounds
    {
        get { return m_SpawnBounds; }
        private set { m_SpawnBounds = value; }
    }
    private Color m_DrawColor = Color.green;
    public Color DrawColor
    {
        get { return m_DrawColor; }
        set { m_DrawColor = value; }
    }

    public void CalculateBounds()
    {
        SpawnBounds = new Bounds();
        //Debug.Log("Calculating Template bounds...");
        foreach (GameObject Template in TemplateList)
        {
            if (Template == null)
            {
                continue;
            }
            if (SpawnBounds.size == Vector3.zero)
            {
                SpawnBounds = GetBounds(Template, Vector3.zero, new Quaternion());
                //Debug.Log("First bounds: " + SpawnBounds.ToString());
            }
            else
            {
                SpawnBounds = CombineBounds(SpawnBounds, GetBounds(Template, Vector3.zero, new Quaternion()));
                //Debug.Log("Combined bounds: " + SpawnBounds.ToString());
            }
            
        }
        IsBoundsCalculated = true;
        //Debug.Log("Final bounds: " + SpawnBounds.ToString());
        //Debug.Log("Template bounds calculation complete!");
    }
    private Bounds CombineBounds(Bounds a, Bounds b)
    {
        Vector3 Min = new Vector3();
        Vector3 Max = new Vector3();
        for (int i = 0; i < 3; i++)
        {
            Min[i] = Mathf.Min(a.min[i], b.min[i]);
            Max[i] = Mathf.Max(a.max[i], b.max[i]);
        }
        Bounds ReturnBounds = new Bounds();
        ReturnBounds.min = Min;
        ReturnBounds.max = Max;

        return ReturnBounds;
    }

    private Bounds GetBounds(GameObject a_Object, Vector3 a_Position, Quaternion a_Rotation)
    {
        Bounds ObjectBounds = new Bounds();
        
        Renderer ObjectRenderer = a_Object.GetComponent<Renderer>();
        if(ObjectRenderer != null)
        {
            ObjectBounds.min = ObjectRenderer.bounds.min;
            ObjectBounds.max = ObjectRenderer.bounds.max;
        }
        RandomTemplate TemplateScript = a_Object.GetComponent<RandomTemplate>();
        if(TemplateScript != null)
        {
            if(!TemplateScript.IsBoundsCalculated)
            {
                TemplateScript.CalculateBounds();
            }

            if (ObjectBounds.size == Vector3.zero)
            {
                ObjectBounds = TemplateScript.SpawnBounds;
            }
            else
            {
                ObjectBounds = CombineBounds(ObjectBounds, TemplateScript.SpawnBounds);
            }
        }
        for(int i = 0; i < a_Object.transform.childCount; i++)
        {
            GameObject Child = a_Object.transform.GetChild(i).gameObject;
            Bounds ChildBounds = GetBounds(Child, a_Position + Child.transform.localPosition, Child.transform.localRotation * a_Rotation);

            if (ObjectBounds.size == Vector3.zero)
            {
                ObjectBounds = ChildBounds;
            }
            else
            {
                ObjectBounds = CombineBounds(ObjectBounds, ChildBounds);
            }
        }
        
        return ObjectBounds;
    }

    void OnDrawGizmos()
    {
        DrawColor = new Color(0.75f, 0.75f, 0.75f, 0.25f);

        if (Selection.activeTransform != null)
        {
            if (transform == Selection.activeTransform)
            {
                DrawColor = new Color(0f, 0.95f, 0f, 1.0f);
            }
            else if (transform.IsChildOf(Selection.activeTransform))
            {
                DrawColor = new Color(0.55f, 0.55f, 0.95f, 0.95f);
            }
        }
        DrawBounds(DrawColor);
    }

    private void DrawBounds(Color a_Color)
    {
        Bounds DrawBounds = SpawnBounds;
        Vector3[] Points = new Vector3[8];

        for (int i = 0; i < 8; i++)
        {
            int x = i / 4;
            int y = i % 2;
            int z = (i / 2) % 2;

            Vector3 Point = new Vector3();
            Point.x = (x == 0 ? DrawBounds.min.x : DrawBounds.max.x);
            Point.y = (y == 0 ? DrawBounds.min.y : DrawBounds.max.y);
            Point.z = (z == 0 ? DrawBounds.min.z : DrawBounds.max.z);
            Points[i] = transform.position + transform.rotation * Point;
        }

        int[] Indices = new int[]
        {
            // Left side
            0, 1, 0, 2,
            3, 1, 3, 2,
            // Right side
            4, 5, 4, 6,
            7, 5, 7, 6,
            // Connecting edges
            0, 4, 1, 5,
            2, 6, 3, 7
        };

        Handles.color = a_Color;
        Handles.DrawDottedLines(
            Points,
            Indices,
            6
        );
    }

    public void CreateTestObject()
    {
        // Clear the EditorTestObject if it exists.
        ClearTestObject();
        // Create a random Object form the list.
        EditorTestObject = InstantiateFromList();
        EditorTestObject.name = EditorTestObject.name + " - Delete me before play!";
        EditorTestObject.transform.parent = this.transform;
    }


    public void ClearTestObject()
    {
        // Make sure the object exists at all.
        if (EditorTestObject != null)
        {
            // Use DestroyImmediate to make sure it works in Editor.
            DestroyImmediate(EditorTestObject);
        }
    }

    private void Start ()
    {
        // Create a random object from the TemplateList;
        InstantiateFromList();
        
        // Destroy this script, and clear it from memory.
        Destroy(this);
    }

    protected virtual GameObject InstantiateFromList()
    {
        // If the user hasn't entered any items into list.
        if (TemplateList.Length == 0)
        {
            // Send error message and return null;
            Debug.LogError("No templates available in TemplateList to spawn.");
            return null;
        }


        // Create an instance of a randomly selected GameObject template in the list.
        GameObject SpawnedObject = Instantiate(TemplateList[Random.Range(0, TemplateList.Length)]);

        // Add the transform attributes from this GameObject.
        CopyTransform(SpawnedObject.transform, this.transform);
        
        return SpawnedObject;
    }

    protected void CopyTransform(Transform a_Target, Transform a_Source)
    {
        NavMeshAgent Agent = a_Target.GetComponent<NavMeshAgent>();
        if (Agent != null)
        {
            Agent.enabled = false;
        }

        a_Target.parent = a_Source.parent;
        //aTarget.localPosition += aSource.localPosition;
        a_Target.position = a_Source.position;
        a_Target.localRotation = a_Source.localRotation * a_Target.localRotation;
        //a_Target.localScale = Vector3.Scale(a_Target.localScale, a_Source.localScale);

        if (Agent != null)
        {
            Agent.enabled = true;
        }
    }
}
