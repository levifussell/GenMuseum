using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintingSpawner : MonoBehaviour
{
    #region serialized parameters
    [SerializeField] [Range(0.0f, 9.0f)] float _minWidth = 0.1f;
    [SerializeField] [Range(0.0f, 10.0f)] float _maxWidth = 0.3f;
    [SerializeField] [Range(0.0f, 9.0f)] float _minHeight = 0.1f;
    [SerializeField] [Range(0.0f, 10.0f)] float _maxHeight = 0.3f;
    #endregion

    #region parameters
    private float minWidth { get => _minWidth; }
    private float maxWidth { get => Mathf.Max(_minWidth, _maxWidth); }
    private float minHeight { get => _minHeight; }
    private float maxHeight { get => Mathf.Max(_minHeight, _maxHeight); }

    public Painting spawnedPainting { get; private set; }
    private float spawnWidth;
    private float spawnHeight;
    public bool hasSpawned { get => spawnedPainting != null; }

    public Vector3 spawnPoint
    {
        get => this.transform.position;
    }
    #endregion

    #region unity methods
    // Start is called before the first frame update
    void Start()
    {
        ChoosePaintingParameters();
        CreatePaintingStand();
        SpawnPainting();
    }

    private void OnDrawGizmos()
    {
        // draw base.
        Gizmos.color = Color.green;
        Gizmos.matrix = Matrix4x4.TRS(this.transform.position, this.transform.rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, Vector3.one * 0.02f);

        // draw the bounds of possible paintings.
        Gizmos.color = Color.cyan;
        Gizmos.matrix = Matrix4x4.TRS(spawnPoint, this.transform.rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(minWidth, minHeight, Painting.DEPTH));
        Gizmos.color = Color.cyan;
        Gizmos.matrix = Matrix4x4.TRS(spawnPoint, this.transform.rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(maxWidth, maxHeight, Painting.DEPTH));

        // draw the bounds of the spawned painting.
        Gizmos.color = Color.red;
        Gizmos.matrix = Matrix4x4.TRS(spawnPoint, this.transform.rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(spawnWidth, spawnHeight, Painting.DEPTH));
    }

    #endregion

    #region spawn
    private void ChoosePaintingParameters()
    {
        spawnWidth = Random.Range(minWidth, maxWidth);
        spawnHeight = Random.Range(minHeight, maxHeight);
    }

    private void CreatePaintingStand()
    {
        float nailWidth = 0.01f;

        GameObject nailLeft = GameObject.CreatePrimitive(PrimitiveType.Cube);
        nailLeft.transform.localScale = Vector3.one * nailWidth;
        nailLeft.transform.SetParent(this.transform);
        nailLeft.transform.position = this.transform.position + this.transform.rotation * (
            -Vector3.right * ((spawnWidth - nailWidth) / 2.0f - Painting.FRAME_WIDTH) +
            Vector3.up * ((spawnHeight - nailWidth) / 2.0f - Painting.FRAME_WIDTH));

        GameObject nailRight = GameObject.CreatePrimitive(PrimitiveType.Cube);
        nailRight.transform.localScale = Vector3.one * nailWidth;
        nailRight.transform.SetParent(this.transform);
        nailRight.transform.position = this.transform.position + this.transform.rotation * (
            Vector3.right * ((spawnWidth - nailWidth) / 2.0f - Painting.FRAME_WIDTH) +
            Vector3.up * ((spawnHeight - nailWidth) / 2.0f - Painting.FRAME_WIDTH));
    }

    public void SpawnPainting()
    {
        if (spawnedPainting != null)
            Destroy(spawnedPainting);

        Painting.Init(spawnPoint, this.transform.rotation, spawnWidth, spawnHeight);
    }
    #endregion
}
