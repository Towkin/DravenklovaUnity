using UnityEngine;
using System.Collections;
using System;

// Samuel Einheri (based on guide)

public class FadeOnDeath : MonoBehaviour
{
    #region Version 1
    /*
    // Speed that the screen fades to and from black.
    public float fadeSpeed = 1.5f;

    // Whether or not the Scene i still fading in.
    // private bool sceneStarting = true;

    void Awake()
    {
        //Set the texture so that it's the size of the screen and covers it.
        guiTexture.pixelInset = new Rect(0f, 0f, Screen.width, Screen.height); //guiTexture is obsolete, use GetComponent<GUITexture>() instead
        GetComponent<GUITexture>(new Rect(0f, 0f, Screen.width, Screen.height));
    }

    private void GetComponent<T>(Rect rect)
    {
        throw new NotImplementedException();
        
    }

    void Update()
    {

        // if (sceneStarting) // If the scene is starting...

        //   StartScene();  // ... call the StartScene function.
    }

    /*
    void FadeToClear()
    {
        // Lerp the colour of the texture between itself and transparent.
        guiTexture.color = Color.Lerp(guiTexture.color, Color.clear, fadeSpeed * Time.deltaTime);
    }
    */

    void FadeToBlack()
    {
        // Lerp the colour of the texture between itself and black.
       // GetComponent<GUITexture>().color = Color.Lerp(GetComponent<GUITexture>().color, Color.black, fadeSpeed * Time.deltaTime);
    }

    /*
    void StartScene()
    {
        // Fade the texture to clear.
        FadeToClear();

        // If the texture is almost clear...
        if (guiTexture.color.a <= 0.05f)
        {
            // ... set the colour to clear and disable the GUITexture.
            guiTexture.color = Color.clear;
            guiTexture.enabled = false;

            // The scene is no longer starting.
            sceneStarting = false;
        }
    }

    
    public void EndScene()
    {
        // Make sure the texture is enabled.
        guiTexture.enabled = true;

        // Start fading towards black.
        FadeToBlack();

        // If the screen is almost black...
        if (guiTexture.color.a >= 0.95f)
            // ... reload the level.
            Application.LoadLevel(0);
    }
    */
    #endregion

    #region Version 2

    // Style for background tiling
    private GUIStyle m_BackgroundStyle = new GUIStyle();

    // 1x1 pixel texture used for fading
    private Texture2D m_FadeTexture;

    // default starting color: black and fully transparrent
    private Color m_CurrentScreenOverlayColor = new Color(0, 0, 0, 0);

    // default target color: black and fully transparrent  
    private Color m_TargetScreenOverlayColor = new Color(0, 0, 0, 0);

    // the delta-color is basically the "speed / second" at which the current color should change
    private Color m_DeltaColor = new Color(0, 0, 0, 0);

    // make sure this texture is drawn on top of everything
    private int m_FadeGUIDepth = -1000;


    // initialize the texture, background-style and initial color:
    private void Awake()
    {
        m_FadeTexture = new Texture2D(1, 1);
        m_BackgroundStyle.normal.background = m_FadeTexture;
        SetScreenOverlayColor(m_CurrentScreenOverlayColor);

        // TEMP:
        // usage: use "SetScreenOverlayColor" to set the initial color, then use "StartFade" to set the desired color & fade duration and start the fade
        //SetScreenOverlayColor(new Color(0,0,0,1));
        //StartFade(new Color(1,0,0,1), 5);
    }


    // draw the texture and perform the fade:
    private void OnGUI()
    {
        // if the current color of the screen is not equal to the desired color: keep fading!
        if (m_CurrentScreenOverlayColor != m_TargetScreenOverlayColor)
        {
            // if the difference between the current alpha and the desired alpha is smaller than delta-alpha * deltaTime, then we're pretty much done fading:
            if (Mathf.Abs(m_CurrentScreenOverlayColor.a - m_TargetScreenOverlayColor.a) < Mathf.Abs(m_DeltaColor.a) * Time.deltaTime)
            {
                m_CurrentScreenOverlayColor = m_TargetScreenOverlayColor;
                SetScreenOverlayColor(m_CurrentScreenOverlayColor);
                m_DeltaColor = new Color(0, 0, 0, 0);
            }
            else
            {
                // fade!
                SetScreenOverlayColor(m_CurrentScreenOverlayColor + m_DeltaColor * Time.deltaTime);
            }
        }

        // only draw the texture when the alpha value is greater than 0:
        if (m_CurrentScreenOverlayColor.a > 0)
        {
            GUI.depth = m_FadeGUIDepth;
            GUI.Label(new Rect(-10, -10, Screen.width + 10, Screen.height + 10), m_FadeTexture, m_BackgroundStyle);
        }
    }


    // instantly set the current color of the screen-texture to "newScreenOverlayColor"
    // can be usefull if you want to start a scene fully black and then fade to opague
    public void SetScreenOverlayColor(Color newScreenOverlayColor)
    {
        m_CurrentScreenOverlayColor = newScreenOverlayColor;
        m_FadeTexture.SetPixel(0, 0, m_CurrentScreenOverlayColor);
        m_FadeTexture.Apply();
    }


    // initiate a fade from the current screen color (set using "SetScreenOverlayColor") towards "newScreenOverlayColor" taking "fadeDuration" seconds
    public void StartFade(Color newScreenOverlayColor, float fadeDuration)
    {
        if (fadeDuration <= 0.0f)       // can't have a fade last -2455.05 seconds!
        {
            SetScreenOverlayColor(newScreenOverlayColor);
        }
        else                    // initiate the fade: set the target-color and the delta-color
        {
            m_TargetScreenOverlayColor = newScreenOverlayColor;
            m_DeltaColor = (m_TargetScreenOverlayColor - m_CurrentScreenOverlayColor) / fadeDuration;
        }
    }

    #endregion

    #region Samuels visual coding 

    /*

    // Code taken inspiration of, also in Version 2
    // http://wiki.unity3d.com/index.php?title=FadeInOut


    // Style for background tiling
    private GUIStyle m_BackgroundStyle = new GUIStyle();

    // 1x1 pixel texture used for fading
    private Texture2D m_FadeTexture;

    // default starting color: black and fully transparrent
    private Color m_CurrentScreenOverlayColor = new Color(0, 0, 0, 0);

    // default target color: black and fully transparrent  
    private Color m_TargetScreenOverlayColor = new Color(0, 0, 0, 0);

    // the delta-color is basically the "speed / second" at which the current color should change
    private Color m_DeltaColor = new Color(0, 0, 0, 0);

    // make sure this texture is drawn on top of everything
    private int m_FadeGUIDepth = -1000;

    

    void FadeScreen()
    {
    
        m_TargetScreenOverlayColor();

    }

    void killPlayer()
    {
    
        if (Playerhealth = 0)
        {
        
            
        
        }

    }

    bool PlayerDeath = true

    if (PlayerDeath)
    {

        killPlayer();
        FadeScreen();

    }
    else
    {

        playerLives();
        // Don't do anything.

    }

    */

    #endregion
}



