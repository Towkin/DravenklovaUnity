using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(WeightedTemplate))]
public class WeightedTemplateInspector : RandomTemplateInspector
{
    protected override void RefreshCreator()
    {
        WeightedTemplate WeightedCreator = Creator as WeightedTemplate;
        WeightedCreator.CalculateBounds();

        float[] OldList = WeightedCreator.WeightList;
        WeightedCreator.WeightList = new float[WeightedCreator.TemplateList.Length];
        for(int i = 0; i < WeightedCreator.WeightList.Length; i++)
        {
            if (i < OldList.Length)
            {
                WeightedCreator.WeightList[i] = OldList[i];
            }
        }
        if(OldList.Length < WeightedCreator.WeightList.Length)
        {
            for(int i = OldList.Length; i < WeightedCreator.WeightList.Length; i++)
            {
                WeightedCreator.WeightList[i] = 1f;
            }
        }
    }

    protected void DrawWeightColumn(int a_Length)
    {
        GUILayout.BeginVertical(new GUILayoutOption[] { GUILayout.MinWidth(120f), GUILayout.MaxWidth(500f) });
        if (GUILayout.Button(""))
        {
            
        }
        GUILayout.Space(8);

        WeightedTemplate WeightedCreator = Creator as WeightedTemplate;

        for (int i = 0; i < a_Length; i++)
        {
            if (i < WeightedCreator.WeightList.Length)
            {
                GUILayout.BeginHorizontal();
                float NewWeight = EditorGUILayout.Slider(WeightedCreator.WeightList[i], 0f, 100f, new GUILayoutOption[] { GUILayout.Height(19f) });
                GUILayout.EndHorizontal();
                if (NewWeight != WeightedCreator.WeightList[i])
                {
                    WeightedCreator.WeightList[i] = NewWeight;
                }
            }
        }
        GUILayout.EndVertical();
    }

    protected override void DrawTemplateButtons()
    {
        int ListSize = 1;
        if (Creator.TemplateList != null)
        {
            ListSize += Creator.TemplateList.Length;
        }

        GUILayout.Space(8);
        GUILayout.BeginHorizontal();
        DrawSpawnColumn(ListSize);
        DrawWeightColumn(ListSize);
        DrawDeleteColumn(ListSize);
        GUILayout.EndHorizontal();
    }
}

public class WeightedTemplate : RandomTemplate
{
    [SerializeField] [Range(0f, 100f)] [Tooltip("Make sure the length of this list is the same as the TemplateList!\nThe value of each index correspond to the relative ratio of how often each Template is going to spawn. ")]
    private float[] m_WeightList;
    public float[] WeightList
    {
        get { return m_WeightList; }
        set { m_WeightList = value; }
    }

    protected override GameObject InstantiateFromList()
    {
        // If the user hasn't entered any items into list.
        if (TemplateList.Length == 0)
        {
            // Send error message and return null;
            Debug.LogError("No templates available in TemplateList to spawn.");
            return null;
        }
        // If the user has set the length of the two lists differently.
        if (WeightList.Length != TemplateList.Length)
        {
            Debug.LogError("The number of input weights are not equal to the number of templates.");
            return null;
        }

        // Calculate the total of the weights in the WeightList.
        float WeightTotal = 0;
        foreach(float TemplateWeight in WeightList)
        {
            WeightTotal += TemplateWeight;
        }

        // Randomly selected a float in range 0...WeightTotal.
        float RandomWeight = Random.value * WeightTotal;
        // Setup an index variable.
        int WeightedRandomIndex = 0;

        for(int i = 0; i < WeightList.Length; i++)
        {
            // If this RandomWeight is smaller than the current WeightList-element, set the index as the selected one and break.
            if(RandomWeight < WeightList[i])
            {
                WeightedRandomIndex = i;
                break;
            }
            // Else, remove the current WeightList-elements's weight and continue.
            RandomWeight -= WeightList[i];
        }

        if (TemplateList[WeightedRandomIndex] == null)
        {
            return null;
        }

        // Create an instance of the selected GameObject template in the list.
        return SpawnObject(TemplateList[WeightedRandomIndex]);
    }
}
