using UnityEngine;
using System.Collections;
using System;

public class Crossbow : Weapon
{
    [SerializeField]
    private GameObject m_BoltTemplateWood;
    public GameObject BoltTemplateWood
    {
        get { return m_BoltTemplateWood; }
    }
    [SerializeField]
    private GameObject m_BoltSpawnLocation;
    private GameObject BoltSpawnLocation
    {
        get { return m_BoltSpawnLocation; }
    }

    private GameObject m_LoadedBolt;
    private GameObject LoadedBolt
    {
        get { return m_LoadedBolt; }
        set { m_LoadedBolt = value; }
    }

    [SerializeField]
    private float m_BoltImpulse = 50f;
    public float BoltImpulse
    {
        get { return m_BoltImpulse; }
        set { m_BoltImpulse = value; }
    }

    [SerializeField]
    private bool m_IsLoaded = false;
    public bool IsLoaded
    {
        get { return m_IsLoaded; }
        set { m_IsLoaded = value; }
    }

    public override void Attack()
    {
        if(IsLoaded)
        {
            Rigidbody BoltBody = LoadedBolt.GetComponent<Rigidbody>();
            LoadedBolt.transform.parent = null;
            BoltBody.isKinematic = false;
            LoadedBolt.GetComponent<CapsuleCollider>().enabled = true;
            BoltBody.AddForce(BoltSpawnLocation.transform.forward * BoltImpulse, ForceMode.Impulse);
            
            IsLoaded = false;
        }
        else
        {
            // TODO: Inform player that crossbow isn't loaded.
            return;
        }
    }

    public override void Reload()
    {
        if(!IsLoaded)
        {
            LoadedBolt = Instantiate<GameObject>(BoltTemplateWood);
            LoadedBolt.transform.position = BoltSpawnLocation.transform.position;
            LoadedBolt.transform.rotation = BoltSpawnLocation.transform.rotation * LoadedBolt.transform.rotation;
            LoadedBolt.transform.parent = transform;

            IsLoaded = true;
        }
    }
    
}
