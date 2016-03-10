using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(ItemTemplate))]
public class ItemTemplateInspector : WeightedTemplateInspector
{

}

public class ItemTemplate : WeightedTemplate {
    [SerializeField]
    private LevelDigger m_LevelGenerator;
    public LevelDigger LevelGenerator
    {
        get { return m_LevelGenerator; }
    }
	
    public override void CalculateBounds()
    {
        base.CalculateBounds();

        if(LevelGenerator == null)
        {
            return;
        }
        foreach(GameObject Item in LevelGenerator.ItemPrefabs)
        {
            if(Item == null)
            {
                continue;
            }

            if(SpawnBounds.size == Vector3.zero)
            {
                SpawnBounds = GetBounds(Item, Vector3.zero, new Quaternion());
            }
            else
            {
                SpawnBounds = CombineBounds(SpawnBounds, GetBounds(Item, Vector3.zero, new Quaternion()));
            }
        }
    }
    /*public override void CreateTestObject()
    {
        // Clear the EditorTestObject if it exists.
        ClearTestObject();
        // Create a random Object form the list.
        EditorTestObject = Spawn();
        if (EditorTestObject == null)
        {
            return;
        }
        EditorTestObject.name = EditorTestObject.name + " - Delete me before play!";
        EditorTestObject.transform.parent = this.transform;
    }*/

    public GameObject Spawn()
    {
        return Spawn(null);
    }
    public GameObject Spawn(GameObject a_Item)
    {
        if(a_Item == null)
        {
            return InstantiateFromList();
        }
        else
        {
            return SpawnObject(a_Item);
        }
    }
}
