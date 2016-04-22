using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;

// Emanuel Strömgren

public class DebugTracker : MonoBehaviour {

    [SerializeField]
    private bool m_IsTrackingData = true;
    public bool IsTrackingData
    {
        get { return m_IsTrackingData; }
    }

    private BinaryFormatter m_Formatter = new BinaryFormatter();
    private BinaryFormatter Formatter
    {
        get { return m_Formatter; }
    }
    private string m_DirectoryName = "TrackingData";
    public string DirectoryName
    {
        get { return m_DirectoryName; }
        set { m_DirectoryName = value; }
    }
    private string m_FileName = "";
    public string FileName
    {
        get { return m_FileName; }
        private set { m_FileName = value; }
    }
    private TrackerData m_Data;
    private TrackerData Data
    {
        get { return m_Data; }
        set { m_Data = value; }
    }
    #region User Input Data
    [SerializeField]
    [Range(0.1f, 10.0f)]
    private float m_TrackerDeltaTime = 0.5f;
    private float TrackerDeltaTime
    {
        get { return m_TrackerDeltaTime; }
    }
    [SerializeField]
    private GameObject m_PlayerCameraObject;
    private GameObject PlayerCameraObject
    {
        get { return m_PlayerCameraObject; }
    }
    [SerializeField]
    private LevelDigger m_LevelGenerator;
    private LevelDigger LevelGenerator
    {
        get { return m_LevelGenerator; }
        set { m_LevelGenerator = value; }
    }
    #endregion
    private float m_TrackerTime = 0f;
    private float TrackerTime
    {
        get { return m_TrackerTime; }
        set { m_TrackerTime = value; }
    }

    void Start ()
    {
        if (!IsTrackingData)
        {
            return;
        }
        if(LevelGenerator == null)
        {
            LevelGenerator = FindObjectOfType<LevelDigger>();
        }

        FileName = DirectoryName + "\\TrackerFile_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".dat";
        Debug.Log(FileName);
        Data = new TrackerData(LevelGenerator.Seed, LevelGenerator.LevelLength); 
        
    }
	
    void FixedUpdate ()
    {
        if (!IsTrackingData)
        {
            return;
        }

        TrackerTime += Time.fixedDeltaTime;
        if (TrackerDeltaTime <= 0f)
        {
            Debug.LogError("You may not use a tracker deltatime of equal to or less than 0. (TrackerDeltaTime is " + TrackerDeltaTime.ToString() + ")");
            return;
        }
        while(TrackerTime >= TrackerDeltaTime)
        {
            TrackerTime -= TrackerDeltaTime;
            Data.AddData(PlayerCameraObject.transform.position, PlayerCameraObject.transform.forward);
        }
    }
    void OnDestroy ()
    {
        if (!IsTrackingData)
        {
            return;
        }

        SaveData();
    }

    void SaveData ()
    {
        Directory.CreateDirectory(DirectoryName);
        if (!File.Exists(FileName))
        {
            using (Stream FileStream = File.Open(FileName, FileMode.Create))
            {
                Formatter.Serialize(FileStream, Data.MakeSerializable());
            }
        }
        else
        {
            Debug.LogError("File " + FileName + " already exists; tracker data could not be saved.");
        }
    }
}

public struct TrackerData
{
    private int m_MapSeed;
    public int MapSeed
    {
        get { return m_MapSeed; }
        set { m_MapSeed = value; }
    }
    private int m_MapLength;
    public int MapLength
    {
        get { return m_MapLength; }
        set { m_MapLength = value; }
    }
    private List<Vector3> m_PlayerPosition;
    public List<Vector3> PlayerPosition
    {
        get { return m_PlayerPosition; }
    }
    private List<Vector3> m_PlayerDirection;
    public List<Vector3> PlayerDirection
    {
        get { return m_PlayerDirection; }
    }
    private List<float> m_TrackerTime;
    public List<float> TrackerTime
    {
        get { return m_TrackerTime; }
    }

    public TrackerData(int a_Seed, int a_Length)
    {
        m_MapSeed = a_Seed;
        m_MapLength = a_Length;
        m_PlayerPosition = new List<Vector3>();
        m_PlayerDirection = new List<Vector3>();
        m_TrackerTime = new List<float>();
    }

    public void AddData(Vector3 a_PlayerPosition, Vector3 a_PlayerDirection)
    {
        PlayerPosition.Add(a_PlayerPosition);
        PlayerDirection.Add(a_PlayerDirection);
        TrackerTime.Add(Time.realtimeSinceStartup);
    }
    public SerializableTrackerData MakeSerializable()
    {
        return new SerializableTrackerData(MapSeed, MapLength, PlayerPosition.ToArray(), PlayerDirection.ToArray(), TrackerTime.ToArray());
    }
}
[Serializable]
public struct SerializableTrackerData
{
    private int m_Seed;
    private int m_Length;
    private float[] m_PlayerX;
    private float[] m_PlayerY;
    private float[] m_PlayerZ;
    private float[] m_PlayerDirectionX;
    private float[] m_PlayerDirectionY;
    private float[] m_PlayerDirectionZ;
    private float[] m_Time;

    public SerializableTrackerData(int a_Seed, int a_Length, Vector3[] a_PlayerPosition, Vector3[] a_PlayerDirection, float[] a_Time)
    {
        m_Seed = a_Seed;
        m_Length = a_Length;

        m_PlayerX = new float[a_PlayerPosition.Length];
        m_PlayerY = new float[a_PlayerPosition.Length];
        m_PlayerZ = new float[a_PlayerPosition.Length];

        m_PlayerDirectionX = new float[a_PlayerDirection.Length];
        m_PlayerDirectionY = new float[a_PlayerDirection.Length];
        m_PlayerDirectionZ = new float[a_PlayerDirection.Length];

        for (int i = 0; i < a_PlayerPosition.Length; i++)
        {
            m_PlayerX[i] = a_PlayerPosition[i].x;
            m_PlayerY[i] = a_PlayerPosition[i].y;
            m_PlayerZ[i] = a_PlayerPosition[i].z;
        }
        for (int i = 0; i < a_PlayerDirection.Length; i++)
        {
            m_PlayerDirectionX[i] = a_PlayerDirection[i].x;
            m_PlayerDirectionY[i] = a_PlayerDirection[i].y;
            m_PlayerDirectionZ[i] = a_PlayerDirection[i].z;
        }

        m_Time = a_Time;
    }
    public TrackerData Deserialize()
    {
        TrackerData ReturnData = new TrackerData(m_Seed, m_Length);

        for(int i = 0; i < m_Time.Length; i++)
        {
            ReturnData.PlayerPosition.Add(new Vector3(m_PlayerX[i], m_PlayerY[i], m_PlayerZ[i]));
            ReturnData.PlayerDirection.Add(new Vector3(m_PlayerDirectionX[i], m_PlayerDirectionY[i], m_PlayerDirectionZ[i]));
            ReturnData.TrackerTime.Add(m_Time[i]);
        }

        return ReturnData;
    }
}