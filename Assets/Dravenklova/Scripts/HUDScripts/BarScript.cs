using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BarScript : MonoBehaviour {

    // Affects the (green) content of the Health bar.
    
    [SerializeField] // is only used for testing purposes here, will be deleted.
    private float fillAmount; 

    [SerializeField]
    private Image content;

    public float MaxValue { get; set; }
    public float SetBarValue
    {
        set
        {
            fillAmount = Map(value, 0, MaxValue, 0, 1);
        }
    }

	// Use this for initialization
	void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
        BarHandler();
	}

    // Deals with the Health bar.

    // Sidenote: Feel free to change this 
    // to suit the project better.
    private void BarHandler ()
    {
        content.fillAmount = fillAmount;
    }

    // Keeps an eye on how much minimum and max health our Player has.
    private float Map(float value, float inMin, float inMax, float outMin, float outMax)
    {

        // Takes our current health and changes the value between 0 and 1.
        return (value - inMin) * (outMax - outMin) / (inMax - inMin) + outMin;
        // Example: (80 - 0) * (1 - 0) / (100 - 0) + 0
        // 80 * 1 / 100 = 0,8
    }

}
