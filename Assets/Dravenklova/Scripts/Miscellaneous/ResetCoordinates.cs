using UnityEngine;
using System.Collections;

public class ResetCoordinates : MonoBehaviour {
    [SerializeField]
    Vector3 m_SetCoordinates;
    [SerializeField]
    Vector3 m_SetEulerAngles;

	void Start () {
        transform.position = m_SetCoordinates;

        Quaternion NewRot = new Quaternion();
        NewRot.eulerAngles = m_SetEulerAngles;
        transform.rotation = NewRot;
	}
	
}
