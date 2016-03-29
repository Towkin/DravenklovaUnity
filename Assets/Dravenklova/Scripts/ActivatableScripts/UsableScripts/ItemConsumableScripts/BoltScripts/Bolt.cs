using UnityEngine;
using System.Collections;
using System;

public class Bolt : Consumable
{
    Rigidbody BoltBody;

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
}
