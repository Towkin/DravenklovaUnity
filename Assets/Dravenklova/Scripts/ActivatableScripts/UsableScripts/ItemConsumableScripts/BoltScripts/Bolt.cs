using UnityEngine;
using System.Collections;
using System;

// Jens Bak
// Emanuel Strömgren

public class Bolt : Consumable
{
    private Rigidbody BoltBody;
    [SerializeField]
    private GameObject m_BoltAudioHitWallEvent;
    public GameObject BoltAudioHitWallEvent
    {
        get { return m_BoltAudioHitWallEvent; }
    }
    [SerializeField]
    private GameObject m_BoltAudioHitFleshEvent;
    public GameObject BoltAudioHitFleshEvent
    {
        get { return m_BoltAudioHitFleshEvent; }
    }

    private float m_DamageSpeedThreshold = 1f;
    protected float DamageSpeedThreshold
    {
        get { return m_DamageSpeedThreshold; }
    }
    private float m_DamageAmount = 0.35f;
    public float DamageAmount
    {
        get { return m_DamageAmount; }
    }


    protected override void Start ()
    {
        base.Start();
        BoltBody = GetComponent<Rigidbody>();
	}

    public override void Use(Pawn User)
    {
        User.CrossbowAmmo += 1;
        Debug.Log("Ammo Pickup");
        Activate();
    }

    public override void Activate()
    {
        Destroy(gameObject);
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if(BoltBody != null && BoltBody.velocity.magnitude > DamageSpeedThreshold)
        {
            RaycastHit BoltHit;
            if(Physics.SphereCast(transform.position + BoltBody.centerOfMass, 0.1f, BoltBody.velocity, out BoltHit, BoltBody.velocity.magnitude * 1.25f, LayerMask.GetMask("Pawns")))
            {
                Pawn HitPawn = BoltHit.collider.gameObject.GetComponentInChildren<Pawn>();
                if(HitPawn)
                {
                    HitPawn.Health -= DamageAmount;
                    if(HitPawn.GetType() == typeof(NPC))
                    {
                        (HitPawn as NPC).WasHit = true;
                    }
                    if(BoltAudioHitFleshEvent)
                        Instantiate(BoltAudioHitFleshEvent, transform.position, transform.rotation);
                    
                    Destroy(gameObject);
                }
            }
        }
    }

    void OnCollisionEnter()
    {
        if (BoltAudioHitWallEvent)
            Instantiate(BoltAudioHitWallEvent, transform.position, transform.rotation);
    }
}
