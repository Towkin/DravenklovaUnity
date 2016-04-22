using UnityEngine;
using System.Collections;

// Emanuel Strömgren

public class FaderScript : MonoBehaviour
{
    [SerializeField]
    private MeshRenderer m_FadeMesh;
    public MeshRenderer FadeMesh
    {
        get { return m_FadeMesh; }
    }
    private float m_Target = 0f;
    public float Target
    {
        get { return m_Target; }
        set { m_Target = Mathf.Clamp01(value); }
    }

    [SerializeField]
    private float m_FadeTime = 1f;
    public float FadeTime
    {
        get { return m_FadeTime; }
    }
    
    public void FadeIn()
    {
        Target = 0;
    }
    public void FadeOut()
    {
        Target = 1;
    }
    void Update()
    {
        Color FadeColor = FadeMesh.material.color;
        float Difference = Target - FadeColor.a;
        float Direction = Mathf.Sign(Difference);
        FadeColor.a = Mathf.Clamp01(FadeColor.a + Direction * Time.deltaTime / FadeTime);
        FadeMesh.material.color = FadeColor;
    }
}

