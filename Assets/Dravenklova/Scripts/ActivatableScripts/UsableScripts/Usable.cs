using UnityEngine;

public abstract class Usable : Activatable
{
    [SerializeField]
    private MeshRenderer[] m_GlowRenderers;
    protected MeshRenderer[] GlowRenderers
    {
        get { return m_GlowRenderers; }
    }
    private bool m_Glowing = false;
    protected bool Glowing
    {
        get { return m_Glowing; }
        set { m_Glowing = value; }
    }
    
    [SerializeField]
    private float m_HighLightTimerMax = 0.1f;
    protected float HighLightTimerMax
    {
        get { return m_HighLightTimerMax; }
    }

    
    private float m_HighlightTimer;
    protected float HighlightTimer
    {
        get { return m_HighlightTimer; }
        set { m_HighlightTimer = Mathf.Clamp(value, 0f, HighLightTimerMax); }
    }
    protected float HighLightAlpha
    {
        get { return HighlightTimer / HighLightTimerMax; }
    }

    [SerializeField]
    private Shader m_GlowShader = Shader.Find("Self-Illumin/Outlined Diffuse");
    protected Shader GlowShader
    {
        get { return m_GlowShader; }
    }
    private Shader[] m_DefaultShader;
    protected Shader[] DefaultShader
    {
        get { return m_DefaultShader; }
        set { m_DefaultShader = value; }
    }
    
    public void StartGlow()
    {
        for(int i = 0; i < GlowRenderers.Length; i++)
        {
            MeshRenderer GlowRenderer = GlowRenderers[i];

            GlowRenderer.material.shader = GlowShader;
        }

        HighlightTimer = HighLightTimerMax;
        Glowing = true;
    }

    public void StopGlow()
    {
        for (int i = 0; i < GlowRenderers.Length; i++)
        {
            if (i >= DefaultShader.Length || DefaultShader[i] == null)
            {
                Debug.LogError("Failed at index " + i.ToString() + " as DefaultShader only contained " + DefaultShader.Length.ToString() + " items.");
                continue;
            }

            MeshRenderer GlowRenderer = GlowRenderers[i];
            GlowRenderer.material.shader = DefaultShader[i];
        }

        Glowing = false;
    }

    protected virtual void Start()
    {
        DefaultShader = new Shader[GlowRenderers.Length];
        
        for (int i = 0; i < GlowRenderers.Length; i++)
        {
            MeshRenderer GlowRenderer = GlowRenderers[i];

            DefaultShader[i] = GlowRenderer.material.shader;
        }
    }

    void FixedUpdate()
    {
        if (Glowing)
        {
            HighlightTimer -= Time.fixedDeltaTime;
            
            if (HighlightTimer <= 0)
            {
                StopGlow();
            }
        }
    }

    public abstract void Use(Pawn User);

}
