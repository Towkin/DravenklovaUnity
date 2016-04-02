using UnityEngine;
using System.Collections;

public class DravenklovaMenu : MonoBehaviour
{
    [SerializeField]
    private FMODUnity.StudioEventEmitter m_ButtonPressAudio;
    public FMODUnity.StudioEventEmitter ButtonPressAudio
    {
        get { return m_ButtonPressAudio; }
    }
}
