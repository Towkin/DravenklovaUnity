using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(WeightedTemplate))]
public class WeightedTemplateInspector : RandomTemplateInspector
{
    

    protected void DrawWeightColumn(int a_Length)
    {
        GUILayout.BeginVertical(new GUILayoutOption[] { GUILayout.MinWidth(30f), GUILayout.MaxWidth(1000f) });
        if (GUILayout.Button(""))
        {
            
        }
        GUILayout.Space(8);

        WeightedTemplate WeightedCreator = Creator as WeightedTemplate;

        if (WeightedCreator.WeightList != null)
        {
            for (int i = 0; i < a_Length; i++)
            {
                if (i < WeightedCreator.WeightList.Length)
                {
                    float NewWeight = EditorGUILayout.Slider(WeightedCreator.WeightList[i], 0f, 100f, new GUILayoutOption[] { GUILayout.Height(19f) });

                    if (NewWeight != WeightedCreator.WeightList[i])
                    {
                        WeightedCreator.WeightList[i] = NewWeight;
                    }
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

    public override void RemoveIndex(int i)
    {
        base.RemoveIndex(i);

        if (WeightList != null && i >= 0 && i < WeightList.Length)
        {
            float[] OldWeights = WeightList;
            WeightList = new float[WeightList.Length - 1];

            for (int j = 0; j < WeightList.Length; j++)
            {
                if (j < i)
                {
                    WeightList[j] = OldWeights[j];
                }
                else
                {
                    WeightList[j] = OldWeights[j + 1];
                }
            }
        }
    }
    public override void AddLength(int a)
    {
        if(a <= 0)
        {
            return;
        }
        base.AddLength(a);

        float[] OldList = WeightList;
        if (OldList != null)
        {
            WeightList = new float[OldList.Length + a];
            OldList.CopyTo(WeightList, 0);
            
            for (int i = Mathf.Max(OldList.Length - 1, 0); i < WeightList.Length; i++)
            {
                WeightList[i] = 1f;
            }
        }
        else
        {
            WeightList = new float[a];
            for(int i = 0; i < WeightList.Length; i++)
            {
                WeightList[i] = 1f;
            }
        }
    }

    protected override GameObject InstantiateFromList()
    {
        // If the user hasn't entered any items into list.
        if (TemplateList == null || TemplateList.Length == 0)
        {
            // Send error message and return null;
            Debug.LogError("No templates available in TemplateList to spawn.");
            return null;
        }
        // If the user has set the length of the two lists differently.
        if (WeightList == null || WeightList.Length != TemplateList.Length)
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
