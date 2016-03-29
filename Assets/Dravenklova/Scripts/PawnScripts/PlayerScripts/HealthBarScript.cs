using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HealthBarScript : MonoBehaviour
{
    [SerializeField]
    private GameObject MyPlayer;
    private Player m_Player;
    private Image m_HealthBar;

    void Start()
    {
        Player m_Player = MyPlayer.GetComponent<Player>();
        Image m_HealthBar = GetComponent<Image>();
    }

    void FixedUpdate ()
    {
        m_HealthBar.fillAmount = m_Player.Health;
    }
}
