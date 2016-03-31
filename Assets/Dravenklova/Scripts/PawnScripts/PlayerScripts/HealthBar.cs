using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HealthBar : MonoBehaviour
{
    [SerializeField]
    private Player m_Player;
    public Player PlayerCharacter
    {
        get { return m_Player; }
    }
    [SerializeField]
    private Image m_HealthImage;
    public Image HealthImage
    {
        get { return m_HealthImage; }
    }

    void FixedUpdate ()
    {
        if(PlayerCharacter && HealthImage)
        {
            HealthImage.fillAmount = PlayerCharacter.HealthPercentage;
        }
    }
}
