using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(ItemTemplate))]
public class ItemTemplateInspector : WeightedTemplateInspector
{

    protected override void RefreshCreator()
    {
        if ((Creator as ItemTemplate).LevelGenerator == null)
        {
            (Creator as ItemTemplate).LevelGenerator = FindObjectOfType<LevelDigger>();
        }
        base.RefreshCreator();
    }

    protected override void DrawTemplateButtons()
    {
        base.DrawTemplateButtons();
        ItemTemplate ItemCreator = (Creator as ItemTemplate);
        LevelDigger NewDigger = EditorGUILayout.ObjectField("Level Generator", ItemCreator.LevelGenerator, typeof(LevelDigger), true) as LevelDigger;
        if(NewDigger != ItemCreator.LevelGenerator)
        {
            ItemCreator.LevelGenerator = NewDigger;
        }
    }
}

public class ItemTemplate : WeightedTemplate {
    [SerializeField]
    private LevelDigger m_LevelGenerator;
    public LevelDigger LevelGenerator
    {
        get { return m_LevelGenerator; }
        set { m_LevelGenerator = value; }
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
    
    void Start()
    {
        // Override the Start() function to not automatically spawn and destroy on game start.
    }
}
