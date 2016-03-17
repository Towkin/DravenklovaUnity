using UnityEngine;
using System.Collections;

// Doesn't need to be in MonoBehavior, else it won't work the way we want it to do.
public class StatScript 
{

    private BarScript bar;

    private float MaxVal;

    private float CurrentVal;

    public float CurrentVal1
    {
        get
        {
            return CurrentVal;
        }

        set
        {
            this.CurrentVal = value;
            bar.SetBarValue = CurrentVal;
        }
    }

    public float MaxVal1
    {
        get
        {
            return MaxVal;
        }

        set
        {
            this.MaxVal = value;
            bar.MaxValue = MaxVal;
        }
    }
}

