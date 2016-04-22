using UnityEngine;
using System.Collections;
using FMODUnity;
using FMOD.Studio;

// Emanuel Strömgren

public class EmitterScript : MonoBehaviour {
    [EventRef]
    [SerializeField]
    private string m_EmitterEvent;
    private EventInstance m_EmitterInstance;
    public EventInstance EmitterInstance
    {
        get { return m_EmitterInstance; }
    }
    
    void Start ()
    {
        m_EmitterInstance = RuntimeManager.CreateInstance(m_EmitterEvent);
        m_EmitterInstance.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));
        m_EmitterInstance.start();
    }
	void Update()
    {
        m_EmitterInstance.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));

        PLAYBACK_STATE EmitterState;
        m_EmitterInstance.getPlaybackState(out EmitterState);
        if (EmitterState == PLAYBACK_STATE.STOPPED)
        {
            Destroy(gameObject);
        }
    }
    void OnDestroy()
    {
        m_EmitterInstance.release();
    }
}
