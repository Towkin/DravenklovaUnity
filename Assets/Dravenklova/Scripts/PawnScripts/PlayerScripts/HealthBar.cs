using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HealthBarScript : MonoBehaviour
{
    private Player m_Player;

    void FixedUpdate ()
    {
        if(m_Player == null)
        {
            m_Player = FindObjectOfType<Player>();
            if(m_Player == null)
            {
                return;
            }
        }
        Image m_HealthBar = GetComponent<Image>();
        m_HealthBar.fillAmount = m_Player.Health;
        
    }
}
