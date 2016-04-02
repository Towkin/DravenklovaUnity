using UnityEngine;
using System.Collections;

public abstract class Weapon : ItemEquip
{
    private bool m_IsLoaded = false;
    public bool IsLoaded
    {
        get { return m_IsLoaded; }
        set { m_IsLoaded = value; }
    }
    private bool m_IsReloading = false;
    public bool IsReloading
    {
        get { return m_IsReloading; }
        protected set { m_IsReloading = value; }
    }

    [SerializeField]
    private Transform m_Handle;
    public virtual Transform Handle
    {
        get { return m_Handle; }
    }

    private Pawn m_User;
    public Pawn User
    {
        get { return m_User; }
        set { m_User = value; }
    }

    public abstract void Attack();

    public abstract void BeginReload();
    public abstract void StopReload();

    public override void Use(Pawn a_User)
    {
        // Weapon becomes the pawn's weapon.
        a_User.EquippedWeapon = this;


        //Activate();
    }
    public override void Activate()
    {
        //Destroy(this);
    }
}
