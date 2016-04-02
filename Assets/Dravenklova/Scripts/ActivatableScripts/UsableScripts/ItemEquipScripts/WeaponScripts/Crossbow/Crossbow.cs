using UnityEngine;
using System.Collections;

public class Crossbow : Weapon
{
    #region Bolt and String
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
    private Vector3 CurrentStringTarget
    {
        get { return IsLoaded ? StringLoadedLocalPosition : StringFiredLocalPosition; }
    }
    #endregion

    #region Reload
    private float m_ReloadStartTime = 0f;
    public float ReloadStartTime
    {
        get { return m_ReloadStartTime; }
        protected set { m_ReloadStartTime = value; }
    }
    [SerializeField]
    private float m_ReloadTime = 2f;
    public float ReloadTime
    {
        get { return m_ReloadTime; }
    }
    
    #endregion

    #region Audio
    [SerializeField]
    private GameObject m_BoltFireAudioEvent;
    public GameObject BoltFireAudioEvent
    {
        get { return m_BoltFireAudioEvent; }
    }
    [SerializeField]
    private FMODUnity.StudioEventEmitter m_ReloadAudioEvent;
    public FMODUnity.StudioEventEmitter ReloadAudioEvent
    {
        get { return m_ReloadAudioEvent; }
    }
    #endregion

    [SerializeField]
    private float m_BoltImpulse = 50f;
    public float BoltImpulse
    {
        get { return m_BoltImpulse; }
        set { m_BoltImpulse = value; }
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

            // Notch the string up.
            StringTransform.localPosition += new Vector3(0.0f, 0.01f, 0.0f);

            IsLoaded = false;
        }
        else
        {
            // TODO: Inform player that crossbow isn't loaded.
            return;
        }
    }

    public override void BeginReload()
    {
        if(!IsLoaded && User.CrossbowAmmo > 0)
        {
            IsReloading = true;
            ReloadStartTime = Time.realtimeSinceStartup;

            // Normal Reload sound: 0.
            ReloadAudioEvent.SetParameter("reload", 0f);
            ReloadAudioEvent.Play();
        }
    }
    public override void StopReload()
    {
        if(IsReloading)
        {
            if(Time.realtimeSinceStartup - ReloadStartTime > ReloadTime)
            {
                User.CrossbowAmmo -= 1;
                LoadedBolt = Instantiate<GameObject>(BoltTemplateWood);
                LoadedBolt.GetComponent<Rigidbody>().isKinematic = true;
                LoadedBolt.GetComponent<Collider>().enabled = false;
                LoadedBolt.transform.position = BoltSpawnLocation.transform.position;
                LoadedBolt.transform.rotation = BoltSpawnLocation.transform.rotation * LoadedBolt.transform.rotation;
                LoadedBolt.transform.parent = transform;
                SetPrefabRenderLayer(LoadedBolt.transform, gameObject.layer);

                StringTransform.localPosition = StringLoadedLocalPosition;
                IsLoaded = true;
                IsReloading = false;
            }
            // Failed reload
            else
            {
                // Reload fail sound: 1.
                ReloadAudioEvent.SetParameter("reload", 1f);
                IsReloading = false;
            }
        }
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        
        StringTransform.localPosition = Vector3.LerpUnclamped(StringTransform.localPosition, CurrentStringTarget, 1.65f);

        if (IsReloading && Time.realtimeSinceStartup - ReloadStartTime > ReloadTime)
        {
            StopReload();
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
