using UnityEngine;
using System.Collections;

public class TemporaryPlayerScript : MonoBehaviour {

    [SerializeField]
    private StatScript health;

    private void Awake()
    {

        health.Initialize();

    }
	
	// Update is called once per frame
	void Update ()
    {
	    if (Input.GetKeyDown(KeyCode.O))
        {
            health.CurrentVal -= 10;
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            health.CurrentVal += 10;
        }
    }
}
