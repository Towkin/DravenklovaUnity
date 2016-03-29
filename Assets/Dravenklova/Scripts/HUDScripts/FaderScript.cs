using UnityEngine;
using System.Collections;

public class FaderScript : MonoBehaviour
{
    [SerializeField]
    private MeshRenderer m_FadeMesh;
    public MeshRenderer FadeMesh
    {
        get { return m_FadeMesh; }
    }
    private float m_FadeMax = 1f;
    public float FadeMax
    {
        get { return m_FadeMax; }
    }
    private float m_FadeMin = 0f;
    public float FadeMin
    {
       get { return m_FadeMin; }
    }

    [SerializeField]
    private float m_FadeTime = 1f;
    public float FadeTime
    {
        get { return m_FadeTime; }
    }

    private int m_Direction = 0;
    public int Direction
    {
        get { return m_Direction; }
        private set { m_Direction = value; }
    }

    public void FadeIn()
    {
        Direction = -1;
    }
    public void FadeOut()
    {
        Direction = 1;
    }
    void Update()
    {
        Color FadeColor = FadeMesh.material.color;
        FadeColor.a = Mathf.Clamp(FadeColor.a + Direction * Time.deltaTime / FadeTime, FadeMin, FadeMax);
        FadeMesh.material.color = FadeColor;
    }
}

