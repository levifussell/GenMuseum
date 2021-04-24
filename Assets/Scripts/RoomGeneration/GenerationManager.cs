using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

//public static class ChunkData
//{
//    public GameObject
//}

public class GenerationManager : MonoBehaviour
{
    #region serialized parameters
    [Header("Chunk Parameters")]
    [SerializeField] Chunk chunkPrefab = null;
    [SerializeField] int chunkRoomCount = 3;
    [SerializeField] float roomWidth = 10.0f; // TODO: can get this from the mesh data later.

    [SerializeField] Transform player;

    [Header("Debug")]
    [SerializeField] bool showLoadedChunks = true;
    #endregion

    #region parameters
    /* Chunks */
    public float chunkWidth { get => chunkRoomCount * roomWidth; }
    public int chunksLoaded { get => loadedChunks.Count; }
    public int chunksVisible { get => visibleChunks.Count; }
    List<Chunk> visibleChunks = new List<Chunk>();
    Dictionary<Vector3Int, Chunk> loadedChunks = new Dictionary<Vector3Int, Chunk>();

    /* Player */
    Vector3Int lastPlayerChunkPosition = Vector3Int.zero;

    #endregion

    #region unity methods
    // Start is called before the first frame update
    void Start()
    {
        lastPlayerChunkPosition = WorldPositionToNearestLocalChunkPosition(player.position);
        UpdateVisibleChunks();
    }

    // Update is called once per frame
    void Update()
    {
        if(IsPlayerAtNewChunkPosition())
        {
            UpdateVisibleChunks();
        }
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(LocalChunkPositionToWorldPosition(lastPlayerChunkPosition), Vector3.one * chunkWidth * 0.1f);

        if(showLoadedChunks)
        {
            Gizmos.color = Color.red;
            foreach(Vector3Int chunkPos in loadedChunks.Keys)
            {
                Gizmos.DrawWireSphere(LocalChunkPositionToWorldPosition(chunkPos), chunkWidth * 0.5f);
            }
        }
    }
    #endregion

    #region chunk management

    /* Helpers */

    Vector3Int WorldPositionToLocalChunkPosition(Vector3 worldPosition)
    {
        Vector3 positionLocal = this.transform.InverseTransformPoint(worldPosition);
        return new Vector3Int(
            Mathf.FloorToInt(positionLocal.x / chunkWidth),
            0,
            Mathf.FloorToInt(positionLocal.z / chunkWidth)
            );
    }
    Vector3Int WorldPositionToNearestLocalChunkPosition(Vector3 worldPosition)
    {
        Vector3 positionLocal = this.transform.InverseTransformPoint(worldPosition);
        return new Vector3Int(
            Mathf.RoundToInt(positionLocal.x / chunkWidth),
            0,
            Mathf.RoundToInt(positionLocal.z / chunkWidth)
            );
    }

    Vector3 LocalChunkPositionToLocalPosition(Vector3Int chunkPosition)
    {
        return new Vector3(
            chunkPosition.x * chunkWidth,
            0.0f, 
            chunkPosition.z * chunkWidth
            );
    }

    Vector3 LocalChunkPositionToWorldPosition(Vector3Int chunkPosition)
    {
        return this.transform.TransformPoint(LocalChunkPositionToLocalPosition(chunkPosition));
    }


    /* Chunk creation */

    Chunk CreateNewChunkFromPosition(Vector3Int chunkPos)
    {
        Chunk chunk = GameObject.Instantiate<Chunk>(chunkPrefab);
        chunk.transform.SetParent(this.transform);
        chunk.Init(
            LocalChunkPositionToLocalPosition(chunkPos),
            chunkPos);
        loadedChunks.Add(chunkPos, chunk);
        return chunk;
    }
    Chunk CreateOrLoadChunkFromPosition(Vector3Int chunkPos)
    {
        if (loadedChunks.ContainsKey(chunkPos))
        {
            Chunk chunk = loadedChunks[chunkPos];
            chunk.Enable();
            return chunk;
        }
        else
            return CreateNewChunkFromPosition(chunkPos);
    }

    /* Chunk update */

    List<Vector3Int> GetVisibleChunkPositionsFromPosition(Vector3Int pos)
    {
        return new List<Vector3Int>()
        {
            pos + new Vector3Int(-1, 0, -1),
            pos + new Vector3Int(0, 0, -1),
            pos + new Vector3Int(1, 0, -1),
            pos + new Vector3Int(-1, 0, 0),
            pos + new Vector3Int(0, 0, 0),
            pos + new Vector3Int(1, 0, 0),
            pos + new Vector3Int(-1, 0, 1),
            pos + new Vector3Int(0, 0, 1),
            pos + new Vector3Int(1, 0, 1)
        };
    }

    bool IsPlayerAtNewChunkPosition()
    {
        Vector3Int currentPlayerChunkPosition = WorldPositionToNearestLocalChunkPosition(player.position);
        if(currentPlayerChunkPosition != lastPlayerChunkPosition)
        {
            lastPlayerChunkPosition = currentPlayerChunkPosition;
            return true;      
        }

        return false;
    }

    void UpdateVisibleChunks()
    {
        List<Vector3Int> newVisibleChunksPos = GetVisibleChunkPositionsFromPosition(lastPlayerChunkPosition);
        List<Chunk> newChunks = new List<Chunk>();

        // check which old chunks should be disabled or added.
        List<int> oldChunkIndices = new List<int>();
        foreach (Chunk c in visibleChunks)
        {
            int idx = newVisibleChunksPos.IndexOf(c.chunkPos);
            if(idx != -1)
            {
                newChunks.Add(c);
                oldChunkIndices.Add(idx);
            }
            else
            {
                c.Disable();
            }
        }

        // add new chunks that weren't in the old chunk setup.
        for(int i = 0; i < newVisibleChunksPos.Count; ++i)
        {
            if(!oldChunkIndices.Contains(i))
            {
                Chunk c = CreateOrLoadChunkFromPosition(newVisibleChunksPos[i]);
                newChunks.Add(c);
            }
        }

        visibleChunks = newChunks;

        Debug.Assert(visibleChunks.Count == 9); // always 9 visible chunks (for now).
    }

    #endregion
}

#if UNITY_EDITOR
[CustomEditor(typeof(GenerationManager))]
public class E_GenerationManager : Editor
{
    GenerationManager generationManager;
    private void OnEnable()
    {
        generationManager = (GenerationManager)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.LabelField($"Chunks Loaded: {generationManager.chunksLoaded}");
        EditorGUILayout.LabelField($"Chunks Visible: {generationManager.chunksVisible}");
    }
}
#endif
