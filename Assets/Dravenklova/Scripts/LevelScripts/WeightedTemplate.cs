using UnityEngine;
using System.Collections;

public class WeightedTemplate : RandomTemplate
{
    [SerializeField] [Range(0f, 100f)] [Tooltip("Make sure the length of this list is the same as the TemplateList!\nThe value of each index correspond to the relative ratio of how often each Template is going to spawn. ")]
    private float[] m_WeightList;
    private float[] WeightList
    {
        get { return m_WeightList; }
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

        // Create an instance of the selected GameObject template in the list.
        GameObject SpawnedObject = Instantiate(TemplateList[WeightedRandomIndex]);

        // Add the transform attributes from this GameObject.
        CopyTransform(SpawnedObject.transform, this.transform);

        return SpawnedObject;
    }
}
