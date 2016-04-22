using UnityEngine;
using System.Collections;

// Emanuel Strömgren

public class GhoulAnimationCallback : MonoBehaviour {

    [SerializeField]
    GameObject m_FootstepEvent;
    [SerializeField]
    GameObject m_AttackEvent;

    private float m_LastFootstep = 0f;
    private GameObject m_Player;

    public void Start()
    {
        m_Player = FindObjectOfType<Player>().gameObject;
    }

    public void Attack()
    {
        Instantiate(m_AttackEvent, transform.position, transform.rotation);
        //GameObject Voice = Instantiate(m_AttackEvent);
        //Voice.transform.parent = transform.parent;
        //Voice.transform.position = transform.position;
        //Voice.transform.rotation = transform.rotation;

        Collider[] PawnHits = Physics.OverlapSphere(transform.position + transform.forward * 0.8f, 0.8f, LayerMask.GetMask("Pawns"));

        foreach(Collider PawnHit in PawnHits)
        {
            Player HitPlayer = PawnHit.gameObject.GetComponentInChildren<Player>();
            if (HitPlayer != null)
            {
                //HitPlayer.Health -= 0.4f;
                HitPlayer.DamagePlayer(0.4f, transform.position + Vector3.up * 1.2f);
            }
        }
    }
    public void Footstep()
    {
        if (Time.timeSinceLevelLoad - m_LastFootstep > 0.1f)
        {
            if (Vector3.Distance(transform.position, m_Player.transform.position) < 20f)
            {
                m_LastFootstep = Time.timeSinceLevelLoad;
                Instantiate(m_FootstepEvent, transform.position, transform.rotation);
            }
        }
    }
}
