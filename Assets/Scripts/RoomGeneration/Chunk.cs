using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ChunkData
{
    #region room data
    private static string roomPrefabDirectory = "Prefabs/Rooms-P/";
    private static GameObject[] _roomPrefabs = null;
    private static GameObject[] roomPrefabs
    {
        get
        {
            if(_roomPrefabs == null) { LoadRoomPrefabs(); }
            return _roomPrefabs;
        }
    }

    private static string roomPrefabStartFile = "Prefabs/StartingRoom_1";
    private static GameObject _startRoomPrefab = null;
    public static GameObject startRoomPrefab
    {
        get
        {
            if(_startRoomPrefab == null) { _startRoomPrefab = Resources.Load<GameObject>(roomPrefabStartFile); }
            return _startRoomPrefab;
        }
    }
    #endregion



    #region chunk stats
    public static int ROOMS_PER_CHUNK_DIM = 3;
    private static int _ROOM_WIDTH = -1;
    public static int ROOM_WIDTH
    {
        get
        {
            if(_ROOM_WIDTH == -1)
            {
                Mesh roomMesh = roomPrefabs[0].GetComponent<MeshFilter>().sharedMesh;
                _ROOM_WIDTH = (int)roomMesh.bounds.size.x;
            }

            return _ROOM_WIDTH;
        }
    }
    public static int CHUNK_WIDTH { get => ROOMS_PER_CHUNK_DIM * ROOM_WIDTH; }
    #endregion

    #region room handling
    static void LoadRoomPrefabs()
    {
        _roomPrefabs = Resources.LoadAll<GameObject>(roomPrefabDirectory);
    }

    public static GameObject GetRandomRoomPrefab()
    {
        return roomPrefabs[Random.Range(0, roomPrefabs.Length)];
    }

    public static Mesh GetRandomRoomMesh()
    {
        GameObject randomRoom = GetRandomRoomPrefab();
        return randomRoom.GetComponent<MeshFilter>().sharedMesh;
    }
    #endregion
}

public class Chunk : MonoBehaviour
{

    #region parameters
    public Vector3Int chunkPos { get; private set; }
    private List<Rigidbody> objectsInChunk = new List<Rigidbody>();

    public Room[,] rooms = new Room[ChunkData.ROOMS_PER_CHUNK_DIM, ChunkData.ROOMS_PER_CHUNK_DIM];

    private PaintingSpawner[] paintingSpawnersInChunk;
    #endregion

    #region unity methods
    // Start is called before the first frame update
    void Awake()
    {
        // find all paintings that start in the room.
        paintingSpawnersInChunk = this.GetComponentsInChildren<PaintingSpawner>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnTriggerEnter(Collider other)
    {
        if (objectsInChunk.Contains(other.attachedRigidbody))
            return;

        objectsInChunk.Add(other.attachedRigidbody);
    }
    private void OnTriggerExit(Collider other)
    {
        if (!objectsInChunk.Contains(other.attachedRigidbody))
            return;

        objectsInChunk.Remove(other.attachedRigidbody);
    }
    #endregion

    #region control
    public void Disable()
    {
        foreach(Rigidbody r in objectsInChunk)
        {
            r.gameObject.SetActive(false);
        }

        this.gameObject.SetActive(false);
    }
    public void Enable()
    {
        foreach(Rigidbody r in objectsInChunk)
        {
            r.gameObject.SetActive(true);
        }

        this.gameObject.SetActive(true);
    }
    #endregion

    #region creation
    public static Chunk Create(Vector3 LocalPosition, Vector3Int ChunkPos)
    {
        GameObject chunkObj = new GameObject("chunk");
        Chunk chunk = chunkObj.AddComponent<Chunk>();

        // room creation.
        int halfRoomDim = ChunkData.ROOMS_PER_CHUNK_DIM / 2;
        for(int x = 0; x < ChunkData.ROOMS_PER_CHUNK_DIM; ++x)
        {
            for(int y = 0; y < ChunkData.ROOMS_PER_CHUNK_DIM; ++y)
            {
                GameObject roomXY; 

                // centre-most chunk is the special start room.
                if(ChunkPos == Vector3Int.zero && x == 1 && y == 1)
                {
                    roomXY = GameObject.Instantiate(ChunkData.startRoomPrefab);
                    chunk.rooms[x, y] = roomXY.AddOrGetComponent<StartRoom>();        
                }
                else
                {
                    roomXY = GameObject.Instantiate(ChunkData.GetRandomRoomPrefab());
                    chunk.rooms[x, y] = roomXY.AddOrGetComponent<Room>();        
                }

                roomXY.transform.SetParent(chunkObj.transform);
                roomXY.transform.localPosition = new Vector3(
                    (-halfRoomDim + x) * ChunkData.ROOM_WIDTH,
                    0.0f,
                    (-halfRoomDim + y) * ChunkData.ROOM_WIDTH);
            }
        }

        // trigger.
        BoxCollider chunkTrigger = chunkObj.AddComponent<BoxCollider>();
        chunkTrigger.size = Vector3.one * ChunkData.CHUNK_WIDTH;
        chunkTrigger.isTrigger = true;

        chunk.Init(LocalPosition, ChunkPos);

        return chunk;
    }

    public void Init(Vector3 LocalPosition, Vector3Int ChunkPos)
    {
        this.transform.localPosition = LocalPosition;
        chunkPos = ChunkPos;
    }
    #endregion
}
