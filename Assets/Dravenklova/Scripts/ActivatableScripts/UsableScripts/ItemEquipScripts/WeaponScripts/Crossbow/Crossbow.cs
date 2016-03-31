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
    [SerializeField]
    private Transform m_StringTransform;
    private Transform StringTransform
    {
        get { return m_StringTransform; }
    }
    private Vector3 m_StringLoadedLocalPosition;
    private Vector3 StringLoadedLocalPosition
    {
        get { return m_StringLoadedLocalPosition; }
        set { m_StringLoadedLocalPosition = value; }
    }
    private Vector3 m_StringFiredOffset = new Vector3(0.09f, 0.01f, 0.0f);
    private Vector3 StringFiredLocalPosition
    {
        get { return m_StringLoadedLocalPosition + m_StringFiredOffset; }
    }

    private GameObject m_LoadedBolt;
    private GameObject LoadedBolt
    {
        get { return m_LoadedBolt; }
        set { m_LoadedBolt = value; }
    }
    [SerializeField]
    private GameObject m_BoltFireAudioEvent;
    public GameObject BoltFireAudioEvent
    {
        get { return m_BoltFireAudioEvent; }
    }
    [SerializeField]
    private GameObject m_ReloadAudioEvent;
    public GameObject ReloadAudioEvent
    {
        get { return m_ReloadAudioEvent; }
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

    protected override void Start()
    {
        base.Start();

        StringLoadedLocalPosition = StringTransform.localPosition;
        StringTransform.localPosition = StringFiredLocalPosition;
    }

    public override void Attack()
    {
        if(IsLoaded)
        {
            
            Rigidbody BoltBody = LoadedBolt.GetComponent<Rigidbody>();
            LoadedBolt.transform.parent = null;
            BoltBody.isKinematic = false;
            // Default the bolt's layer.
            SetPrefabRenderLayer(LoadedBolt.transform, 0);
            LoadedBolt.GetComponent<Collider>().enabled = true;
            BoltBody.AddForce(BoltSpawnLocation.transform.forward * BoltImpulse, ForceMode.Impulse);

            Instantiate(BoltFireAudioEvent, transform.position, transform.rotation);

            StringTransform.localPosition = StringFiredLocalPosition;
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
            LoadedBolt.GetComponent<Rigidbody>().isKinematic = true;
            LoadedBolt.GetComponent<Collider>().enabled = false;
            LoadedBolt.transform.position = BoltSpawnLocation.transform.position;
            LoadedBolt.transform.rotation = BoltSpawnLocation.transform.rotation * LoadedBolt.transform.rotation;
            LoadedBolt.transform.parent = transform;
            SetPrefabRenderLayer(LoadedBolt.transform, gameObject.layer);

            StringTransform.localPosition = StringLoadedLocalPosition;
            IsLoaded = true;
        }
    }
    
    private void SetPrefabRenderLayer(Transform a_Parent, int a_Layer)
    {
        a_Parent.gameObject.layer = a_Layer;
        for(int i = 0; i < a_Parent.childCount; i++)
        {
            SetPrefabRenderLayer(a_Parent.GetChild(i), a_Layer);
        }
    }
}
