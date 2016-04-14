using UnityEngine;
using System.Collections.Generic;
using Pathfinding;

public class LevelDigger : MonoBehaviour {
    [SerializeField]
    private bool m_IgnoreBlockerTests = false;
    [SerializeField]
    private bool m_SpawnBlockerTests = false;
    [SerializeField]
    private bool m_DebugLogs = false;
    private bool m_IsDebugLevel = false;
    private bool IsDebugLevel
    {
        get { return m_IsDebugLevel; }
        set { m_IsDebugLevel = value; }
    }
    [SerializeField]
    private bool m_TestConnectionScale = false;

    [SerializeField]
    private bool m_UseRandomSeed = true;
    private bool UseRandomSeed
    {
        get { return m_UseRandomSeed; }
    }

    [SerializeField]
    [ContextMenuItem("Remove debug map", "ClearDebugLevel")]
    [ContextMenuItem("Generate debug map by TrackingData", "DebugLevelTrackingData")]
    private DebugTrackerVisualizer m_TrackingData;
    public DebugTrackerVisualizer TrackingData
    {
        get { return m_TrackingData; }
    }

    
    [SerializeField]
    [ContextMenuItem("Remove debug map", "ClearDebugLevel")]
    [ContextMenuItem("Generate debug map by settings", "DebugLevelSettings")]
    private int m_Seed;
    public int Seed
    {
        get { return m_Seed; }
        private set { m_Seed = value; }
    }

    [SerializeField]
    private GameObject m_Cube;

    private Stack<GameObject> m_RoomBranch = new Stack<GameObject>();
    public Stack<GameObject> RoomBranch
    {
        get { return m_RoomBranch; }
        set { m_RoomBranch = value; }
    }
    [SerializeField]
    [Range(1, 30)]
    private int m_LevelLength = 5;
    public int LevelLength
    {
        get { return m_LevelLength; }
        set { m_LevelLength = value; }
    }

    private enum RoomTypes { Spawn, Branch, End, Shrine };
    private enum SortTypes { Random, Ascending, Descending };

    [SerializeField]
    private GameObject m_FirstRoom;
    public GameObject FirstRoom
    {
        get { return m_FirstRoom; }
    }
    private GameObject m_LastShrine;
    public GameObject LastShrine
    {
        get { return m_LastShrine; }
        set { m_LastShrine = value; }
    }

    [SerializeField]
    private GameObject[] m_BranchingRooms;
    public GameObject[] BranchingRooms
    {
        get { return m_BranchingRooms; }
    }

    [SerializeField]
    private GameObject[] m_EndRooms;
    public GameObject[] EndRooms
    {
        get { return m_EndRooms; }
    }
    

    [SerializeField]
    private GameObject[] m_EndWalls;
    public GameObject[] EndWalls
    {
        get { return m_EndWalls; }
    }
    [SerializeField]
    private GameObject[] m_Shrines;
    public GameObject[] Shrines
    {
        get { return m_Shrines; }
    }
    [SerializeField]
    private GameObject[] m_ItemPrefabs;
    public GameObject[] ItemPrefabs
    {
        get { return m_ItemPrefabs; }
    }
    [SerializeField]
    private float m_ItemMultiplier = 2.0f;
    public int ItemCount
    {
        get { return Mathf.RoundToInt(LevelLength * m_ItemMultiplier); }
    }

    //[SerializeField]
    //private RecastGraph m_PathfindingGraph;
    //public RecastGraph PathfindingGraph
    //{
    //    get { return m_PathfindingGraph; }
    //    private set { m_PathfindingGraph = value; }
    //}

    [SerializeField]
    private GameObject[] m_EnemyPrefabs;
    public GameObject[] EnemyPrefabs
    {
        get { return m_EnemyPrefabs; }
    }
    [SerializeField]
    private float m_EnemyMultiplier = 0.75f;
    public float EnemyMultiplier
    {
        get { return m_EnemyMultiplier; }
        set { m_EnemyMultiplier = value; }
    }
    [SerializeField]
    private float m_EnemyMultiplierAdd = 0.35f;
    public int EnemyCount
    {
        get { return Mathf.RoundToInt(LevelLength * m_EnemyMultiplier); }
    }

    
    private List<GameObject> m_Enemies;
    public List<GameObject> Enemies
    {
        get { return m_Enemies; }
        private set { m_Enemies = value; }
    }

    private Queue<GameObject> m_LevelObjects = new Queue<GameObject>();
    public Queue<GameObject> LevelObjects
    {
        get { return m_LevelObjects; }
        set { m_LevelObjects = value; }
    }
    private GameObject m_LastBuiltPrefab = null;
    public GameObject LastBuiltPrefab
    {
        get { return m_LastBuiltPrefab; }
        set { m_LastBuiltPrefab = value; }
    }

    private GameObject m_LevelParent;
    public GameObject LevelParent
    {
        get { return m_LevelParent; }
        private set { m_LevelParent = value; }
    }
    private GameObject m_LevelGraph;
    public GameObject LevelGraph
    {
        get { return m_LevelGraph; }
        private set { m_LevelGraph = value; }
    }

    [SerializeField]
    private LayerMask m_PathfinderMask;
    public LayerMask PathfinderMask
    {
        get { return m_PathfinderMask; }
    }

    private Stack<GameObject> m_DebugBranch = new Stack<GameObject>();
    public Stack<GameObject> DebugBranch
    {
        get { return m_DebugBranch; }
        private set { m_DebugBranch = value; }
    }

    private Queue<GameObject> m_DebugObjects = new Queue<GameObject>();
    public Queue<GameObject> DebugObjects
    {
        get { return m_DebugObjects; }
        private set { m_DebugObjects = value; }
    }

    //int Counter = 0;


    private bool m_LevelEnded = false;

    void Awake ()
    {
        if (UseRandomSeed)
        {
            Seed = Random.Range(int.MinValue, int.MaxValue);
        }
        else
        {
            Random.seed = Seed;
        }
    }
    void Start ()
    {
        IsDebugLevel = false;
        Random.seed = Seed;
        BuildLevel(RoomBranch, LevelObjects, LevelLength, Instantiate(FirstRoom), new GameObject("Level Objects"));
    }

    public void DebugLevelTrackingData()
    {
        ClearDebugLevel();

        if (TrackingData != null)
        {
            SpawnDebugLevel(TrackingData.Data.MapLength, TrackingData.Data.MapSeed);
        }
        else
        {
            Debug.Log("Tracking data unreadable.");
        }
    }
    public void DebugLevelSettings()
    {
        ClearDebugLevel();

        if (UseRandomSeed)
        {
            Seed = Random.Range(int.MinValue, int.MaxValue);
        }
        SpawnDebugLevel(LevelLength, Seed);
        
    }

    private void SpawnDebugLevel(int a_LevelLength, int a_Seed)
    {
        int OldSeed = Random.seed;
        Random.seed = a_Seed;

        IsDebugLevel = true;
        GameObject DebugLevel = new GameObject("Debug Level - Delete me before play!");
        BuildLevel(DebugBranch, DebugObjects, a_LevelLength, Instantiate(FirstRoom), DebugLevel);
        LevelParent = DebugLevel;

        Random.seed = OldSeed;
    }

    private void ClearDebugLevel()
    {
        Debug.Log("Clearing level of " + DebugObjects.Count.ToString() + " objects.");

        while(DebugObjects.Count > 0)
        {
            DestroyImmediate(DebugObjects.Dequeue());
        }

        DebugObjects = new Queue<GameObject>();
        DebugBranch = new Stack<GameObject>();
        DestroyImmediate(LevelGraph);
        DestroyImmediate(LevelParent);
    }

    public void LoadNextLevel()
    {
        ClearLastLevel();

        LevelLength++;
        EnemyMultiplier += m_EnemyMultiplierAdd;

        BuildLevel(RoomBranch, LevelObjects, LevelLength, LastShrine, new GameObject("Level Objects"));
    }

    private void ClearLastLevel()
    {
        while(LevelObjects.Count > 0)
        {
            Destroy(LevelObjects.Dequeue());
        }
        foreach(GameObject Enemy in Enemies)
        {
            Destroy(Enemy);
        }
        Enemies = null;
        LevelObjects = new Queue<GameObject>();
        RoomBranch = new Stack<GameObject>();
    }

    private void BuildLevel(Stack<GameObject> a_Branch, Queue<GameObject> a_Level, int a_LevelLength, GameObject a_FirstRoom, GameObject a_Parent)
    {
        Debug.Log("Building level with length " + a_LevelLength.ToString() + " and seed " + Random.seed.ToString());


        int AimLength = a_LevelLength;
        bool LevelEnded = false;
        m_LevelEnded = false;
        //bool ConnectionsLeft = true;

        a_FirstRoom.transform.parent = a_Parent.transform;
        a_Branch.Push(a_FirstRoom);
        a_Level.Enqueue(a_FirstRoom);
        

        while(a_Branch.Count > 0)
        {
            GameObject NewPrefab;
            if (a_Branch.Count >= a_LevelLength && !LevelEnded)
            {
                NewPrefab = BuildLevelPiece(a_Branch, RoomTypes.Shrine);
                if (NewPrefab == null)
                {
                    NewPrefab = BuildLevelPiece(a_Branch, RoomTypes.End);
                }
                else
                {
                    LevelEnded = true;
                    m_LevelEnded = true;
                    
                    LastShrine = NewPrefab;
                    // Make the leveldigger back up again.
                    continue;
                }
            }
            else if(a_Branch.Count >= AimLength)
            {
                NewPrefab = BuildLevelPiece(a_Branch, RoomTypes.End);
            }
            else
            {
                NewPrefab = BuildLevelPiece(a_Branch, RoomTypes.Branch);
                if(NewPrefab == null)
                {
                    NewPrefab = BuildLevelPiece(a_Branch, RoomTypes.End);
                }
            }

            if(NewPrefab == null)
            {
                a_Branch.Pop();
            }
            else
            {
                a_Branch.Push(NewPrefab);
                a_Level.Enqueue(NewPrefab);
                NewPrefab.transform.parent = a_Parent.transform;
            }
        }
        if (LevelGraph == null)
        {
            LevelGraph = new GameObject("A* Graph Object");
        }
        LevelGraph.transform.parent = a_Parent.transform;

        BuildPathfinding(LevelGraph);

        List<ItemTemplate> ItemSpawners = new List<ItemTemplate>(FindObjectsOfType<ItemTemplate>());
        int ItemsSpawned = 0;

        while(ItemSpawners.Count > 0)
        {
            int SpawnIndex = Random.Range(0, ItemSpawners.Count);
            if(ItemsSpawned < ItemCount && ItemPrefabs != null)
            {
                ItemSpawners[SpawnIndex].Spawn(ItemPrefabs[Random.Range(0, ItemPrefabs.Length)]);
            }
            else
            {
                ItemSpawners[SpawnIndex].Spawn();
            }
            ItemSpawners.RemoveAt(SpawnIndex);
            ItemsSpawned++;
        }
        
        List<EnemyTemplate> EnemySpawners = new List<EnemyTemplate>(FindObjectsOfType<EnemyTemplate>());
        //int EnemiesSpawned = 0;
        Enemies = new List<GameObject>();
        while(EnemySpawners.Count > 0)
        {
            int SpawnIndex = Random.Range(0, EnemySpawners.Count);
            if(Enemies.Count < EnemyCount && EnemyPrefabs != null)
            {
                if(EnemyPrefabs.Length > 0)
                {
                    Enemies.Add(Instantiate(EnemyPrefabs[0], EnemySpawners[SpawnIndex].transform.position, EnemySpawners[SpawnIndex].transform.rotation) as GameObject);
                }
            }

            EnemySpawners.RemoveAt(SpawnIndex);
        }
        Debug.Log("Enemies spawned: " + Enemies.Count.ToString());

    }

    private void BuildPathfinding(GameObject a_GraphObject)
    {

        AstarPath GraphScript = a_GraphObject.GetComponent<AstarPath>();
        if (GraphScript == null)
        {
            GraphScript = a_GraphObject.AddComponent<AstarPath>();
        }
        if(GraphScript == null)
        {
            Debug.LogError("Couldn't build AstarPath - does the scene already contain any?");
        }


        PointGraph Graph = GraphScript.astarData.pointGraph;
        if (Graph == null)
        {
            Graph = GraphScript.astarData.AddGraph(typeof(PointGraph)) as PointGraph;
        }
        if(Graph == null)
        {
            Debug.LogError("PointGraph creation failed!");
            return;
        }

        Graph.searchTag = "AINode";
        Graph.maxDistance = 5f;
        Graph.limits = new Vector3(0.0f, 1.0f, 0.0f);
        Graph.raycast = true;
        Graph.thickRaycast = true;
        Graph.thickRaycastRadius = 0.25f;
        Graph.mask = PathfinderMask;

        GraphScript.Scan();
    }

    // Tries to build a level piece of the a_Type type. If succesful it returns the room prefab, else returns null.
    private GameObject BuildLevelPiece(Stack<GameObject> a_Branch, RoomTypes a_Type)
    {
        GameObject LevelPiece = null;

        
        ConnectionPoint[] NextConnections = m_LevelEnded ? GetConnections(a_Branch.Peek(), SortTypes.Random) : GetConnections(a_Branch.Peek(), SortTypes.Descending);
        if (NextConnections != null)
        {
            foreach (ConnectionPoint Connection in NextConnections)
            {
                if (Connection.Linked)
                {
                    continue;
                }


                Transform BuildFromTransform = Connection.transform;

                switch (a_Type)
                {
                    case RoomTypes.Spawn:
                        Debug.LogError("Can't build a spawn room after first one!");
                        break;

                    case RoomTypes.Branch:
                        LevelPiece = TestAndBuildPrefabs(BuildFromTransform, Connection.PreferedPrefabs, LastBuiltPrefab);
                        if (LevelPiece == null)
                        {
                            LevelPiece = TestAndBuildPrefabs(BuildFromTransform, BranchingRooms, LastBuiltPrefab);
                        }
                        break;

                    case RoomTypes.End:
                        LevelPiece = TestAndBuildPrefabs(BuildFromTransform, EndRooms, LastBuiltPrefab);
                        if (LevelPiece == null)
                        {
                            LevelPiece = TestAndBuildPrefabs(BuildFromTransform, EndWalls, null);
                        }
                        break;

                    case RoomTypes.Shrine:
                        LevelPiece = TestAndBuildPrefabs(BuildFromTransform, Shrines, null);
                        break;

                    default:
                        Debug.LogError("Unknown room type.");
                        break;
                }

                if (LevelPiece != null)
                {
                    Connection.Linked = true;

                    if (m_DebugLogs)
                    {
                        Debug.Log("Found a piece that worked, built a " + LevelPiece.name);
                    }

                    break;
                }
            }
        }
        return LevelPiece;
    }

    // Returns the ConnectionPoints of a prefab in either sorted by height or random order. If no ConnectionPoints exist, returns null.
    private ConnectionPoint[] GetConnections(GameObject a_Prefab, SortTypes a_Sorted)
    {
        //Debug.Log("Connection");
        ConnectionPoint[] ReturnArray = null;

        ConnectionPoint[] Connections = a_Prefab.GetComponentsInChildren<ConnectionPoint>();

        if(Connections.Length > 0)
        {
            ReturnArray = new ConnectionPoint[Connections.Length];
            if (a_Sorted != SortTypes.Random)
            {
                if (a_Sorted == SortTypes.Ascending)
                {
                    System.Array.Sort(Connections);
                    for (int i = 0; i < Connections.Length; i++)
                    {
                        ReturnArray[Connections.Length - 1 - i] = Connections[i];
                    }
                }
                else if (a_Sorted == SortTypes.Descending)
                {
                    System.Array.Sort(Connections);
                    ReturnArray = Connections;
                }

                for(int i = 1; i < ReturnArray.Length; i++)
                {
                    // If two array indicies are within a meter height of each other.
                    if(Mathf.Abs(ReturnArray[i].transform.position.y - ReturnArray[i - 1].transform.position.y) < 1f)
                    {
                        if(Random.value > 0.5f)
                        {
                            ConnectionPoint Holder = ReturnArray[i];
                            ReturnArray[i] = ReturnArray[i - 1];
                            ReturnArray[i - 1] = Holder;
                        }
                    }
                }

            }
            else
            {
                int RandomAdd = Random.Range(0, Connections.Length);

                for (int i = 0; i < ReturnArray.Length; i++)
                {
                    ReturnArray[i] = Connections[(i + RandomAdd) % Connections.Length];
                }
            }
            
        }


        return ReturnArray;
    }

    // Tests an array of prefabs from a ConnectionPoint's transform, and returns the instantiated GameObject if successful, else null.
    private GameObject TestAndBuildPrefabs(Transform a_FromTransform, GameObject[] a_Prefabs, GameObject a_AvoidPrefab)
    {
        GameObject TransformMarker = new GameObject();
        GameObject ReturnPrefab = null;

        // To "scramble" the list, we add a random integer to the index (and later we mod the iterator by length).
        int IndexAdd = Random.Range(0, a_Prefabs.Length);
        // Find all GameObjects who'll potentially block our new prefab.
        GameObject[] ExistingPrefabBlockers = GameObject.FindGameObjectsWithTag("PrefabBlocker");

        // Go through all prefabs until we find one who fit. 
        // Resource-intense, it's O(n^2 * ni^2) I think... - TODO: performance check?
        for (int i = 0; i < a_Prefabs.Length; i++)
        {
            GameObject TestObject = a_Prefabs[(i + IndexAdd) % a_Prefabs.Length];
            if(TestObject == a_AvoidPrefab)
            {
                continue;
            }

            BoxCollider[] TestBlockers = TestObject.GetComponentsInChildren<BoxCollider>();

            ConnectionPoint[] TestConnections = m_LevelEnded ? GetConnections(TestObject, SortTypes.Ascending) : GetConnections(TestObject, SortTypes.Descending);
            
            foreach(ConnectionPoint Connection in TestConnections)
            {
                if (Connection.ExitOnly)
                {
                    continue;
                }
                if(m_TestConnectionScale && Connection.transform.lossyScale != a_FromTransform.lossyScale)
                {
                    continue;
                }
                bool NoCollisions = true;
                
                Quaternion ConnectionRotation = Quaternion.FromToRotation(Connection.transform.forward, -a_FromTransform.forward);
                ConnectionRotation.eulerAngles = new Vector3(0, ConnectionRotation.eulerAngles.y, 0);

                Vector3 ConnectionOffset = ConnectionRotation * Connection.transform.localPosition;

                // Go through all BoxColliders in this prefab.
                foreach (BoxCollider TestBlocker in TestBlockers)
                {
                    if (m_IgnoreBlockerTests)
                    {
                        continue;
                    }

                    // If the BoxCollider isn't a PrefabBlocker, we skip to the next.
                    if (TestBlocker.tag != "PrefabBlocker")
                    {
                        continue;
                    }

                    // The "blueprint" transform.
                    Transform TestTransform = TransformMarker.transform;

                    // Move the blueprint transform through the other ConnectionPoint's transform, and then offset with our prefab's transform.
                    TestTransform.position = ConnectionRotation * TestBlocker.transform.localPosition + a_FromTransform.position - ConnectionOffset;
                    TestTransform.rotation = TestBlocker.transform.rotation * ConnectionRotation;
                    TestTransform.localScale = TestBlocker.transform.localScale;

                    if (m_SpawnBlockerTests)
                    {
                        GameObject TestingObject = Instantiate(m_Cube);
                        TestingObject.name = TestBlocker.transform.parent.name + "->" + TestBlocker.name + " pretender";
                        TestingObject.transform.position = TestTransform.position;
                        TestingObject.transform.rotation = TestTransform.rotation;
                        TestingObject.transform.localScale = TestTransform.localScale;
                    }

                    // Go through ALL other potential blockers (tagged "PrefabBlocker").
                    foreach (GameObject OtherBlockers in ExistingPrefabBlockers)
                    {
                        BoxCollider OtherCollider = OtherBlockers.GetComponent<BoxCollider>();

                        // Make sure the Collider actually exists, then see if it blocks this blocker. If it does, we can break from this Connection, and try the next.
                        if (OtherCollider != null && BoundingBoxCollision.TestCollision(TestTransform, TestBlocker.size, OtherCollider.transform, OtherCollider.size))
                        {
                            if (m_DebugLogs)
                            {
                                Debug.Log("Collision found on prefab " + TestObject.name + " block " + TestBlocker.name + " against " + OtherCollider.transform.parent.name + "'s block " + OtherCollider.gameObject.name);
                            }
                            NoCollisions = false;
                            break;
                        }
                    }
                    if (NoCollisions == false)
                    {
                        break;
                    }
                }
                if(NoCollisions)
                {
                    // Build the thing from this place, then break.
                    ReturnPrefab = Instantiate(TestObject);
                    LastBuiltPrefab = TestObject;

                    ReturnPrefab.transform.position = a_FromTransform.position - ConnectionOffset;
                    ReturnPrefab.transform.rotation = ConnectionRotation;

                    if (m_DebugLogs)
                    {
                        Debug.Log(TestObject.name + "->" + Connection.name + ".rotation: " + ConnectionRotation.ToString());
                    }

                    ConnectionPoint[] PrefabConnections = ReturnPrefab.GetComponentsInChildren<ConnectionPoint>();
                    foreach (ConnectionPoint PrefabConnection in PrefabConnections)
                    {
                        if(PrefabConnection.name == Connection.name)
                        {
                            if (m_DebugLogs)
                            {
                                Debug.Log("Found the connection!");
                            }
                            PrefabConnection.Linked = true;
                            break;
                        }
                    }

                    break;
                }
            }

            // The latest prefab was successfully built, no need to check any more.
            if(ReturnPrefab != null)
            {
                break;
            }
        }

#if UNITY_EDITOR
        if (IsDebugLevel)
        {
            DestroyImmediate(TransformMarker);
        }
#endif
        if(TransformMarker)
        {
            Destroy(TransformMarker);
        }

        return ReturnPrefab;
    }
}
