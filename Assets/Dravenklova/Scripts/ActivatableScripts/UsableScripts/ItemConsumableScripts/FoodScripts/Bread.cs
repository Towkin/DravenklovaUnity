using UnityEngine;
using System.Collections;
using System;

public class Bread : Consumable
{

    [SerializeField]
    private float m_HealAmount = .4f;
    private float HealAmount
    {
        get { return m_HealAmount; }
        set { m_HealAmount = value; }
    }

    public override void Use(Pawn User)
    {
        User.Health += HealAmount;

        Activate();
    }

    public override void Activate()
    {
        Destroy(gameObject);
    }

}
