using UnityEngine;
using System.Collections;

// Emanuel Strömgren

public class CopyCamera : MonoBehaviour {

    [SerializeField]
    private Camera m_Source;
    private Camera Source
    {
        get { return m_Source; }
    }
    private Camera m_Local;
    private Camera Local
    {
        get { return m_Local; }
        set { m_Local = value; }
    }

	void Start ()
    {
        Local = GetComponent<Camera>();
    }

	void Update ()
    {
	    if(Local == null)
        {
            Debug.LogError("No local camera!");
            return;
        }

        Local.fieldOfView = Source.fieldOfView;
	}
}
