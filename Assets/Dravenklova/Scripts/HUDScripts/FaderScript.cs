using UnityEngine;
using System.Collections;

public class FaderScript : MonoBehaviour
{
    public float FadeMax = 1f;
    public float FadeMin = 0f;
    public float FadeLerp = 1 * Time.fixedDeltaTime;
    public void FadeIn()
    {
        GetComponents<MeshRenderer>().material.color.a = Mathf.Lerp(FadeMax, FadeMin, FadeLerp);
    }
    public void FadeOut()
    {
        GetComponents<MeshRenderer>().material.color.a = Mathf.Lerp(FadeMin, FadeMax, FadeLerp);
    }
}

