using UnityEngine;
using System.Collections;

public class substance_wet_system : MonoBehaviour {
    public ProceduralMaterial substance;            
    private float timer;
    private Animator anim;

    // Use this for initialization
    void Start () {

        anim = GetComponent<Animator>();
        ProceduralMaterial.substanceProcessorUsage = ProceduralProcessorUsage.All;
        
    }
	
	// Update is called once per frame
	void Update () {

        if (anim.GetBool("run") == true || anim.GetBool("jump") == true || anim.GetBool("punch") == true || anim.GetBool("hook") == true || anim.GetBool("uppercut") == true)   // List of actions which make your character sweaty

        {
            timer += Time.deltaTime;    // Starts timer when you use actions from list above ('List of actions')
        }

        else

        {
            timer -= Time.deltaTime / 2;    // Takes points from timer value if you are not using actions from 'List of actions'
        }

        if (Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.Space))    // Get manually +1 point to timer value for too short actions of punch and jump (Checking press on keys which bind for those actions) 

        {
            timer += 1;
        }

        if (timer >= 0 & timer < 4)     // Checks if your character not works too much, he is not sweaty (Substance parameter 'Wet Level' = 0)

        {
            substance.SetProceduralEnum("wet_level", 0);
            substance.RebuildTextures();
        }

        if (timer >= 4 & timer < 8)     // Checks if your character works little, he is little bit sweaty (Substance parameter 'Wet Level' = 4)

        {
            substance.SetProceduralEnum("wet_level", 4);
            substance.RebuildTextures();
        }

        if (timer >= 8 & timer < 14)    // Checks if your character works well, he is sweaty (Substance parameter 'Wet Level' = 5)

        {
            substance.SetProceduralEnum("wet_level", 5);
            substance.RebuildTextures();
        }

        if (timer >= 14 & timer < 22)   // Checks if your character works hard, he is really sweaty (Substance parameter 'Wet Level' = 6)

        {
            substance.SetProceduralEnum("wet_level", 6);
            substance.RebuildTextures();
        }

        if (timer >= 22)    // Checks if your character works very hard, he is so sweaty (Substance parameter 'Wet Level' = 7)

        {
            substance.SetProceduralEnum("wet_level", 7);
            substance.RebuildTextures();
        }


    }
}
