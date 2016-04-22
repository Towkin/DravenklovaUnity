using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;

// Emanuel Strömgren

public class DebugTrackerVisualizer : MonoBehaviour {
    private BinaryFormatter m_Formatter = new BinaryFormatter();
    private BinaryFormatter Formatter
    {
        get { return m_Formatter; }
    }
    [SerializeField]
    [ContextMenuItem("Load File Data", "LoadData")]
    private string m_FileName = "";
    public string FileName
    {
        get { return m_FileName; }
    }
    private TrackerData m_Data;
    public TrackerData Data
    {
        get { return m_Data; }
        protected set { m_Data = value; }
    }

    [SerializeField]
    private float m_ColorRate = 10f;
    private float ColorRate
    {
        get { return m_ColorRate / 1000f; }
    }
    [SerializeField]
    private float m_DirectionLength = 1f;
    private float DirectionLength
    {
        get { return m_DirectionLength; }
    }
    [SerializeField]
    [Range(1, 100)]
    private int m_TimeStampStep = 10;
    private int TimeStampStep
    {
        get { return m_TimeStampStep; }
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (Data.TrackerTime != null)
        {
            for (int i = 0; i < Data.TrackerTime.Count; i += TimeStampStep)
            {
                GUIStyle Style = new GUIStyle();
                Style.normal.textColor = Color.HSVToRGB((Data.TrackerTime[i] * ColorRate) % 1f, 1f, 1f);
                UnityEditor.Handles.Label(Data.PlayerPosition[i] + new Vector3(0f, 1f, 0f), Data.TrackerTime[i].ToString("0.00") + "s", Style);
            }
        }
    }
    void OnDrawGizmos()
    {
        if (Data.TrackerTime != null)
        {
            for (int i = 0; i < Data.TrackerTime.Count; i++)
            {
                Gizmos.color = Color.HSVToRGB((Data.TrackerTime[i] * ColorRate) % 1f, 1f, 1f);
                if (i > 0)
                {
                    Gizmos.DrawLine(Data.PlayerPosition[i - 1], Data.PlayerPosition[i]);
                }

                Gizmos.DrawRay(Data.PlayerPosition[i], Data.PlayerDirection[i] * DirectionLength);

            }
        }
    }
#endif
    void LoadData()
    {
        using (Stream FileStream = File.Open(FileName, FileMode.Open))
        {
            Data = ((SerializableTrackerData)Formatter.Deserialize(FileStream)).Deserialize();
        }
    }
}
