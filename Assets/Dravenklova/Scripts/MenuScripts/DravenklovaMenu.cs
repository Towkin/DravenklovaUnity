using UnityEngine;
using System.Collections;

// Emanuel Strömgren

public class DravenklovaMenu : MonoBehaviour
{
    [SerializeField]
    private FMODUnity.StudioEventEmitter m_ButtonPressAudio;
    public FMODUnity.StudioEventEmitter ButtonPressAudio
    {
        get { return m_ButtonPressAudio; }
    }
}
