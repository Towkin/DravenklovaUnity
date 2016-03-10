using UnityEngine;
using System.Collections;
using System;

public class Bolt : Consumable
{
    Rigidbody BoltBody;

    void Start ()
    {
        BoltBody = GetComponent<Rigidbody>();
	}

    public override void Use(Pawn User)
    {
        User.AmmoCrossbow += 1;
        Debug.Log("Ammo Pickup");
        Activate();
    }

    public override void Activate()
    {
        Destroy(gameObject);
    }

    void FixedUpdate ()
    {
	    /*if(!BoltBody.isKinematic)
        {
            RaycastHit m_Hit;
            Ray m_RayPath = new Ray(transform.position, BoltBody.velocity.normalized);
            Debug.Log("Pew");
            if (Physics.Raycast(m_RayPath, out m_Hit, BoltBody.velocity.magnitude * Time.deltaTime * 3))
            {
                if (m_Hit.collider.GetComponent<Pawn>())
                {
                    // TODO: Deal damage on enemy
                    m_Hit.collider.GetComponent<Pawn>().Health -= .5f;

                    Destroy(gameObject);
                }
                else
                {
                    //float m_Rnd = Random.Range(1, 5);
                    //if (m_Rnd > 3)
                    //    Destroy(this.gameObject);
                    //Debug.Log(m_Rnd);
                    BoltBody.isKinematic = true;
                    transform.position = m_Hit.point;
                    Destroy(BoltBody);

                }

            }
        }*/
	}
}
