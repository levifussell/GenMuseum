using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkData
{

}

public class Chunk : MonoBehaviour
{

    #region parameters
    public Vector3Int chunkPos { get; private set; }
    #endregion

    #region unity methods
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    #endregion

    #region control
    public void Disable()
    {
        this.gameObject.SetActive(false);
    }
    public void Enable()
    {
        this.gameObject.SetActive(true);
    }
    #endregion

    #region creation
    public void Init(Vector3 localPosition, Vector3Int ChunkPos)
    {
        this.transform.localPosition = localPosition;
        chunkPos = ChunkPos;
    }
    #endregion
}
