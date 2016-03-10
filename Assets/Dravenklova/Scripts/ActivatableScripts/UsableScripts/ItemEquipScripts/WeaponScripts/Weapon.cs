using UnityEngine;
using System.Collections;
using System;

public abstract class Weapon : ItemEquip
{
    public abstract void Attack();

    public abstract void Reload();

    public override void Use(Pawn User)
    {
        // TODO: Weapon beomces the pawn's weapon.

        Activate();
    }
    public override void Activate()
    {
        Destroy(this);
    }
}
