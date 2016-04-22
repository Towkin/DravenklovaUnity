using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// Emanuel Strömgren

public class AmmoHUDScript : MonoBehaviour {

    [SerializeField]
    private Player m_Player;
    public Player PlayerCharacter
    {
        get { return m_Player; }
    }
    [SerializeField]
    private Text m_AmmoText;
    public Text AmmoText
    {
        get { return m_AmmoText; }
    }

    public void FixedUpdate()
    {
        if(PlayerCharacter && AmmoText)
        {
            AmmoText.text = PlayerCharacter.EquippedAmmo.ToString() + "x";
        }
    }
}
