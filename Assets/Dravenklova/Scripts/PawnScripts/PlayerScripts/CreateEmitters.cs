using UnityEngine;
using System.Collections;

public class CreateEmitters : MonoBehaviour {

    [SerializeField]
    [Range(0, 100f)]
    private float m_RangeMin = 10f;
    [SerializeField]
    [Range(0, 100f)]
    private float m_RangeMax = 50f;

    [SerializeField]
    [Range(0, 60f)]
    private float m_TimerMax = 4f;
    [SerializeField]
    [Range(0, 60f)]
    private float m_TimerMin = 2f;
    private float TimerRandom
    {
        get
        {
            float ReturnVal = Random.Range(m_TimerMin, m_TimerMax);

            //Debug.Log(ReturnVal.ToString());

            return ReturnVal;
        }
    }

    [SerializeField]
    private GameObject[] m_TemplateEmitter;

    private float m_Timer = 0f;
    private float Timer
    {
        get { return m_Timer; }
        set
        {
            m_Timer = value;

            if(m_Timer <= 0f)
            {
                CreateEmitter();
                m_Timer = TimerRandom;
            }
        }
    }

    void Start ()
    {
        Timer = TimerRandom;
    }

	void Update ()
    {
        Timer -= Time.deltaTime;
	}


    void CreateEmitter()
    {
        if (m_TemplateEmitter.Length <= 0)
        {
            Debug.LogError("No objects in Template Emitter array!");
            return;
        }

        GameObject Instance = Instantiate(m_TemplateEmitter[Random.Range(0, m_TemplateEmitter.Length)]);

        Instance.transform.position = transform.position + Random.onUnitSphere * Random.Range(m_RangeMin, m_RangeMax);
    }
}
