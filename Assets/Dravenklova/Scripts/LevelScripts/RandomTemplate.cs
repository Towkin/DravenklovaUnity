using UnityEngine;
using System.Collections;


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
    
    protected void CreateTestObject()
    {
        // Clear the EditorTestObject if it exists.
        ClearTestObject();
        // Create a random Object form the list.
        EditorTestObject = InstantiateFromList();
        EditorTestObject.name = EditorTestObject.name + " - Delete me before play!";
        EditorTestObject.transform.parent = this.transform;
    }

    
    protected void ClearTestObject()
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
        GameObject SpawnedObject = Instantiate<GameObject>(TemplateList[Random.Range(0, TemplateList.Length)]);

        // Add the transform attributes from this GameObject.
        CopyTransform(SpawnedObject.transform, this.transform);
        
        return SpawnedObject;
    }

    protected void CopyTransform(Transform aTarget, Transform aSource)
    {
        NavMeshAgent Agent = aTarget.GetComponent<NavMeshAgent>();
        if (Agent != null)
        {
            Agent.enabled = false;
        }

        aTarget.parent = aSource.parent;
        //aTarget.localPosition += aSource.localPosition;
        aTarget.position = aSource.position;
        aTarget.localEulerAngles += aSource.localEulerAngles;
        aTarget.localScale = Vector3.Scale(aTarget.localScale, aSource.localScale);

        if (Agent != null)
        {
            Agent.enabled = true;
        }
    }
}
