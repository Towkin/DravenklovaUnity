using UnityEngine;
using System.Collections;

public class CloseDoor : Activatable
{
    private Quaternion m_TargetRotation;
    public Quaternion TargetRotation
    {
        get { return m_TargetRotation; }
        set { m_TargetRotation = value; }
    }

    public override void Activate()
    {
        TargetRotation *= Quaternion.Euler(0f, 90f, 0f);

     
        
        #region Samuels messy notes (feel free to ignore this)
        /*
        notes:
        look up: axis  
          
            if ("Player") activates/walks through (TriggerActivatable) 
            {
                
                then (TriggerActivatable) 
                goes from false --> true
                
                end of code
            }

        */
        #endregion
    }

    void Start()
    {
        TargetRotation = transform.rotation;
    }

    void Update()
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, TargetRotation, 0.05f);
    }

}
