using UnityEngine;
using System.Collections;
using UnityEngine.UI;

// Samuel Einheri

public class BarScript : MonoBehaviour
{

    // Affects the (green) content of the Health bar.
    
    [SerializeField] // is only used for testing purposes here, will be deleted.
    private float m_FillAmount; 

    [SerializeField]
    private Image m_Content;

    public float MaxValue { get; set; }
    public float BarValue
    {
        set
        {
            m_FillAmount = Map(value, 0, MaxValue, 0, 1);
            BarHandler();
        }
    }
    

    // Deals with the Health bar.
    private void BarHandler ()
    {
        m_Content.fillAmount = m_FillAmount;
    }

    // Keeps an eye on how much minimum and max health our Player has.
    private float Map(float a_Value, float a_InMin, float a_InMax, float a_OutMin, float a_OutMax)
    {

        // Takes our current health and changes the value between 0 and 1.
        return (a_Value - a_InMin) * (a_OutMax - a_OutMin) / (a_InMax - a_InMin) + a_OutMin;
        // Example: (80 - 0) * (1 - 0) / (100 - 0) + 0
        // 80 * 1 / 100 = 0,8
    }

}
