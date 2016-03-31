using UnityEngine;
using System.Collections;

public class DamageTrigger : MonoBehaviour {

    [SerializeField]
    private float m_Damage = 9999f;
    public float Damage
    {
        get { return m_Damage; }
    }
    
    public void OnTriggerEnter(Collider other)
    {
        Pawn OtherPawn = other.gameObject.GetComponentInChildren<Pawn>();

        if(OtherPawn)
        {
            OtherPawn.Health -= Damage;
        }
    }
}
