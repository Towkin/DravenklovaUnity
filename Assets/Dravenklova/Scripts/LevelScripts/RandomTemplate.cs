using UnityEngine;

// Emanuel Strömgren

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(RandomTemplate))]
public class RandomTemplateInspector : Editor
{
    private RandomTemplate m_Creator;
    protected RandomTemplate Creator
    {
        get { return m_Creator; }
        private set { m_Creator = value; }
    }

    private void OnEnable()
    {
        Creator = target as RandomTemplate;
    }

    protected virtual void RefreshCreator()
    {
        Creator.CalculateBounds();
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUI.BeginChangeCheck();

        DrawTemplateButtons();

        Creator.CopyParent = GUILayout.Toggle(Creator.CopyParent, "Copy Parent");

        if(EditorGUI.EndChangeCheck())
        {
            RefreshCreator();
        }

        serializedObject.ApplyModifiedProperties();
    }

    protected void DrawSpawnColumn(int a_Length)
    {
        GUILayout.BeginVertical(new GUILayoutOption[] { GUILayout.MinWidth(100f), GUILayout.MaxWidth(1000f) });
        if (GUILayout.RepeatButton("Random Object"))
        {
            Creator.CreateTestObject();
        }
        
        GUILayout.Space(8);

        for (int i = 0; i < a_Length; i++)
        {
            if (Creator.TemplateList != null && i < Creator.TemplateList.Length)
            {
                GUILayoutOption[] Options = new GUILayoutOption[] { GUILayout.Width(50f), GUILayout.Height(19f) };

                GUILayout.BeginHorizontal();
                GameObject Template = Creator.TemplateList[i];
                if (Template == null)
                {
                    GUILayout.Button("Empty", Options);
                }
                else if (GUILayout.Button("Spawn", Options))
                {
                    Creator.CreateTestObject(Template);
                }
                GameObject NewObject = EditorGUILayout.ObjectField(Template, typeof(GameObject), false) as GameObject;
                if (NewObject != Template)
                {
                    Creator.TemplateList[i] = NewObject;
                    GUI.changed = true;
                }

                GUILayout.EndHorizontal();
            }
            else
            {
                if (GUILayout.Button("+ Add new template"))
                {
                    Creator.AddLength();
                    GUI.changed = true;
                }
            }
        }
        GUILayout.EndVertical();
    }
    protected void DrawDeleteColumn(int a_Length)
    {
        GUILayout.BeginVertical(new GUILayoutOption[] { GUILayout.MinWidth(60f), GUILayout.MaxWidth(60f) });
        if (GUILayout.Button("Clear Objects"))
        {
            Creator.ClearTestObject();
        }
        GUILayout.Space(8);

        for (int i = 0; i < a_Length; i++)
        {
            if (Creator.TemplateList != null && i < Creator.TemplateList.Length)
            {
                if (GUILayout.Button("Delete Template"))
                {
                    Creator.RemoveIndex(i);
                    GUI.changed = true;
                }
            }
        }
        GUILayout.EndVertical();
    }

    protected virtual void DrawTemplateButtons()
    {
        int ListSize = 1;
        if (Creator.TemplateList != null)
        {
            ListSize += Creator.TemplateList.Length;
        }
        
        GUILayout.Space(8);
        GUILayout.BeginHorizontal();
        DrawSpawnColumn(ListSize);
        DrawDeleteColumn(ListSize);
        GUILayout.EndHorizontal();
    }
}
#endif

public class RandomTemplate : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Add elements to the list, and it'll randomly select one to spawn on game start.\nNote: the spawn object will copy the transform of the GameObject this script is attached to.")]
    //[ContextMenuItem("Test Random Object", "CreateTestObject")]
    //[ContextMenuItem("Clear Random Object", "ClearTestObject")]
    protected GameObject[] m_TemplateList;
    public GameObject[] TemplateList
    {
        get { return m_TemplateList; }
        set { m_TemplateList = value; }
    }

    [SerializeField]
    private GameObject m_EditorTestObject;
    public GameObject EditorTestObject
    {
        get { return m_EditorTestObject; }
        set { m_EditorTestObject = value; }
    }
    [SerializeField]
    private bool m_CopyParent = true;
    public bool CopyParent
    {
        get { return m_CopyParent; }
        set { m_CopyParent = value; }
    }

    [SerializeField]
    private bool m_IsBoundsCalculated = false;
    public bool IsBoundsCalculated
    {
        get { return m_IsBoundsCalculated; }
        private set { m_IsBoundsCalculated = value; }
    }

    [SerializeField]
    private Bounds m_SpawnBounds;
    public Bounds SpawnBounds
    {
        get { return m_SpawnBounds; }
        protected set { m_SpawnBounds = value; }
    }
    private Color m_DrawColor = Color.green;
    public Color DrawColor
    {
        get { return m_DrawColor; }
        set { m_DrawColor = value; }
    }

    public virtual void RemoveIndex(int i)
    {
        if(i >= 0 && i < TemplateList.Length)
        {
            GameObject[] OldList = TemplateList;
            TemplateList = new GameObject[TemplateList.Length - 1];
            for(int j = 0; j < TemplateList.Length; j++)
            {
                if(j < i)
                {
                    TemplateList[j] = OldList[j];
                }
                else
                {
                    TemplateList[j] = OldList[j + 1];
                }
            }
        }
    }
    public void AddLength()
    {
        AddLength(1);
    }
    public virtual void AddLength(int a)
    {
        if (a <= 0)
        {
            return;
        }
        GameObject[] OldList = TemplateList;
        if (OldList != null)
        {
            TemplateList = new GameObject[OldList.Length + a];
            OldList.CopyTo(TemplateList, 0);
        }
        else
        {
            TemplateList = new GameObject[a];
        }
    }

    public virtual void CalculateBounds()
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
    protected Bounds TransformBounds(Bounds a_Bounds, Vector3 a_Offset, Quaternion a_Rotation)
    {
        Bounds ReturnBounds = new Bounds();

        ReturnBounds.min = a_Offset + a_Rotation * a_Bounds.min;
        ReturnBounds.max = a_Offset + a_Rotation * a_Bounds.max;

        return ReturnBounds;
    }
    protected Bounds CombineBounds(Bounds a, Bounds b)
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
    protected Bounds GetBounds(GameObject a_Object, Vector3 a_Position, Quaternion a_Rotation)
    {
        Bounds ObjectBounds = new Bounds();
        
        Renderer ObjectRenderer = a_Object.GetComponent<Renderer>();
        if(ObjectRenderer != null)
        {
            ObjectBounds = TransformBounds(ObjectRenderer.bounds, a_Position - a_Object.transform.localPosition, a_Rotation * Quaternion.Inverse(a_Object.transform.localRotation));
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
                ObjectBounds = TransformBounds(TemplateScript.SpawnBounds, a_Position, a_Rotation);
            }
            else
            {
                ObjectBounds = CombineBounds(ObjectBounds, TransformBounds(TemplateScript.SpawnBounds, a_Position, a_Rotation));
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
#if UNITY_EDITOR
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

    public virtual void CreateTestObject()
    {
        CreateTestObject(null);
    }
    public virtual void CreateTestObject(GameObject a_Template)
    {
        // Clear the EditorTestObject if it exists.
        ClearTestObject();
        // Create a random Object form the list.
        if(a_Template == null)
        {
            EditorTestObject = InstantiateFromList();
        }
        else
        {
            EditorTestObject = SpawnObject(a_Template);
        }
        
        if(EditorTestObject == null)
        {
            return;
        }
        EditorTestObject.name = EditorTestObject.name + " - Delete me before play!";
        EditorTestObject.transform.parent = this.transform;

        RandomTemplate[] SubTemplates = EditorTestObject.GetComponentsInChildren<RandomTemplate>();
        foreach(RandomTemplate SubTemplate in SubTemplates)
        {
            SubTemplate.CreateTestObject();
        }
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
#endif
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
        if (TemplateList == null || TemplateList.Length == 0)
        {
            // Send error message and return null;
            Debug.LogError("No templates available in TemplateList to spawn.");
            return null;
        }

        int RandomIndex = Random.Range(0, TemplateList.Length);
        if(TemplateList[RandomIndex] == null)
        {
            return null;
        }

        // Create an instance of a randomly selected GameObject template in the list.
        return SpawnObject(TemplateList[RandomIndex]);
    }
    public GameObject SpawnObject(GameObject a_Template)
    {
        GameObject SpawnedObject = Instantiate(a_Template);

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
        if (CopyParent)
        {
            a_Target.parent = a_Source.parent;
        }
        //aTarget.localPosition += aSource.localPosition;
        a_Target.position = a_Source.position;
        a_Target.localRotation = a_Source.localRotation;
        //a_Target.localScale = Vector3.Scale(a_Target.localScale, a_Source.localScale);

        if (Agent != null)
        {
            Agent.enabled = true;
        }
    }
}
