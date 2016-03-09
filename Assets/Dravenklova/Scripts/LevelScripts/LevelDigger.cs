using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelDigger : MonoBehaviour {
    [SerializeField]
    private bool m_IgnoreBlockerTests = false;
    [SerializeField]
    private bool m_SpawnBlockerTests = false;
    [SerializeField]
    private bool m_DebugLogs = false;

    [SerializeField]
    private bool m_UseRandomSeed = true;
    private bool UseRandomSeed
    {
        get { return m_UseRandomSeed; }
    }


    [SerializeField]
    private int m_Seed;
    private int Seed
    {
        get { return m_Seed; }
    }

    [SerializeField]
    private GameObject m_Cube;

    private Stack<GameObject> m_RoomBranch = new Stack<GameObject>();
    public Stack<GameObject> RoomBranch
    {
        get { return m_RoomBranch; }
    }
    [SerializeField]
    [Range(1, 10)]
    private int m_LevelLength = 5;
    public int LevelLength
    {
        get { return m_LevelLength; }
    }

    [SerializeField]
    private GameObject m_FirstRoom;
    public GameObject FirstRoom
    {
        get { return m_FirstRoom; }
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

    private GameObject[] m_Enemies;
    public GameObject[] Enemies
    {
        get { return m_Enemies; }
        private set { m_Enemies = value; }
    }

    private Queue<GameObject> m_LevelObjects = new Queue<GameObject>();
    public Queue<GameObject> LevelObjects
    {
        get { return m_LevelObjects; }
    }

    //int Counter = 0;
    
    void Start () {
        RoomBranch.Push(FirstRoom);
        LevelObjects.Enqueue(FirstRoom);

        if(!UseRandomSeed)
        {
            Random.seed = Seed;
        }
        BuildLevel(LevelLength);

        /*NavMeshBuilder.BuildNavMesh();

        ConnectionPoint[] AllConnections = FindObjectsOfType<ConnectionPoint>();
        Transform[] PatrolPoints = new Transform[AllConnections.Length];
        for(int i = 0; i < AllConnections.Length; i++)
        {
            PatrolPoints[i] = AllConnections[i].transform;
        }

        Enemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach(GameObject Enemy in Enemies)
        {
            Enemy.GetComponent<Patrol>().Points = PatrolPoints;
        }*/
    }
	
	void Update () {
	    
	}

    private void BuildLevel(int aLevelLength)
    {
        int AimLength = aLevelLength;
        bool LevelEnded = false;
        //bool ConnectionsLeft = true;

        while(RoomBranch.Count > 0)
        {
            GameObject NewPrefab;
            if (RoomBranch.Count >= aLevelLength && !LevelEnded)
            {
                NewPrefab = BuildShrinePiece();
                if (NewPrefab == null)
                {
                    NewPrefab = BuildEndPiece();
                }
                else
                {
                    LevelEnded = true;
                }
            }
            else if(RoomBranch.Count >= AimLength)
            {
                NewPrefab = BuildEndPiece();
            }
            else
            {
                NewPrefab = BuildNextPiece();
                if(NewPrefab == null)
                {
                    NewPrefab = BuildEndPiece();
                }
            }

            if(NewPrefab == null)
            {
                RoomBranch.Pop();
            }
            else
            {
                RoomBranch.Push(NewPrefab);
                LevelObjects.Enqueue(NewPrefab);
            }
        }

    }

    // Builds the next piece on the current branch, and returns the GameObject. If unsuccessful, returns null.
    private GameObject BuildNextPiece()
    {
        //Debug.Log("BuildNextPiece");
        GameObject NextPiece = null;

        ConnectionPoint[] NextConnections = GetConnections(RoomBranch.Peek());
        if (NextConnections != null)
        {

            foreach (ConnectionPoint Connection in NextConnections)
            {
                if (Connection.Linked)
                {
                    continue;
                }


                Transform BuildFromTransform = Connection.transform;

                NextPiece = TestAndBuildPrefabs(BuildFromTransform, Connection.PreferedPrefabs);
                if (NextPiece == null)
                {
                    NextPiece = TestAndBuildPrefabs(BuildFromTransform, BranchingRooms);
                }
                if (NextPiece != null)
                {
                    Connection.Linked = true;

                    if (m_DebugLogs)
                    {
                        Debug.Log("Found a piece that worked, built a " + NextPiece.name);
                    }

                    break;
                }
            }
        }
        return NextPiece;
    }

    // Builds the last piece on this branch, and returns the GameObject. If there are no Connections left unlinked, returns null.
    private GameObject BuildEndPiece()
    {
        GameObject EndPiece = null;

        ConnectionPoint[] NextConnections = GetConnections(RoomBranch.Peek());
        if (NextConnections != null)
        {
            foreach (ConnectionPoint Connection in NextConnections)
            {
                if (Connection.Linked)
                {
                    continue;
                }

                Transform BuildFromTransform = Connection.transform;

                EndPiece = TestAndBuildPrefabs(BuildFromTransform, EndRooms);
                if (EndPiece == null)
                {
                    EndPiece = TestAndBuildPrefabs(BuildFromTransform, EndWalls);
                }
                if (EndPiece != null)
                {
                    Connection.Linked = true;

                    if (m_DebugLogs)
                    {
                        Debug.Log("Found a piece that worked, built a " + EndPiece.name);
                    }

                    break;
                }
            }
        }
        return EndPiece;
    }

    // Builds a shrine on this branch, and returns the GameObject. If there are no Connections left unlinked, returns null.
    private GameObject BuildShrinePiece()
    {
        GameObject ShrinePiece = null;

        ConnectionPoint[] NextConnections = GetConnections(RoomBranch.Peek());
        if (NextConnections != null)
        {
            foreach (ConnectionPoint Connection in NextConnections)
            {
                if (Connection.Linked)
                {
                    continue;
                }

                Transform BuildFromTransform = Connection.transform;

                ShrinePiece = TestAndBuildPrefabs(BuildFromTransform, Shrines);
                if (ShrinePiece != null)
                {
                    Connection.Linked = true;

                    if (m_DebugLogs)
                    {
                        Debug.Log("Found a piece that worked, built a " + ShrinePiece.name);
                    }

                    break;
                }
            }
        }
        return ShrinePiece;
    }

    // Returns the ConnectionPoints of a prefab in random order. If no ConnectionPoints exist, returns null.
    private ConnectionPoint[] GetConnections(GameObject aPrefab)
    {
        //Debug.Log("Connection");
        ConnectionPoint[] ReturnArray = null;

        ConnectionPoint[] Connections = aPrefab.GetComponentsInChildren<ConnectionPoint>();

        if(Connections.Length > 0)
        {
            ReturnArray = new ConnectionPoint[Connections.Length];
            int RandomAdd = Random.Range(0, Connections.Length);

            for(int i = 0; i < ReturnArray.Length; i++)
            {
                ReturnArray[i] = Connections[(i + RandomAdd) % Connections.Length];
            }
        }


        return ReturnArray;
    }

    // Tests an array of prefabs from a ConnectionPoint's transform, and returns the instantiated GameObject if successful, else null.
    private GameObject TestAndBuildPrefabs(Transform aFromTransform, GameObject[] aPrefabs)
    {
        GameObject TransformMarker = new GameObject();
        GameObject ReturnPrefab = null;

        // To "scramble" the list, we add a random integer to the index (andmod it by length).
        int IndexAdd = Random.Range(0, aPrefabs.Length);
        // Find all GameObjects who'll potentially block our new prefab.
        GameObject[] ExistingPrefabBlockers = GameObject.FindGameObjectsWithTag("PrefabBlocker");

        // Go through all prefabs until we find one who fit. 
        // Resource-intense, it's O(n^2 * ni^2) I think... - TODO: performance check?
        for (int i = 0; i < aPrefabs.Length; i++)
        {
            GameObject TestObject = aPrefabs[(i + IndexAdd) % aPrefabs.Length];
            BoxCollider[] TestBlockers = TestObject.GetComponentsInChildren<BoxCollider>();
            
            ConnectionPoint[] TestConnections = GetConnections(TestObject);

            foreach(ConnectionPoint Connection in TestConnections)
            {
                bool NoCollisions = true;
                
                Quaternion ConnectionRotation = Quaternion.FromToRotation(Connection.transform.forward, -aFromTransform.forward);
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
                    TestTransform.position = ConnectionRotation * TestBlocker.transform.localPosition + aFromTransform.position - ConnectionOffset;
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

                    ReturnPrefab.transform.position = aFromTransform.position - ConnectionOffset;
                    ReturnPrefab.transform.rotation = ConnectionRotation;

                    Debug.Log(TestObject.name + "->" + Connection.name + ".rotation: " + ConnectionRotation.ToString());

                    ConnectionPoint[] PrefabConnections = ReturnPrefab.GetComponentsInChildren<ConnectionPoint>();
                    foreach (ConnectionPoint PrefabConnection in PrefabConnections)
                    {
                        if(PrefabConnection.name == Connection.name)
                        {
                            Debug.Log("Found the connection!");
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

        Destroy(TransformMarker);

        return ReturnPrefab;
    }
}
