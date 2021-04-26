using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintingSpawner : MonoBehaviour
{
    #region data
    Material _backShadowMaterial = null;
    Material backShadowMaterial
    {
        get
        {
            if(_backShadowMaterial == null)
            {
                _backShadowMaterial = Resources.Load<Material>("Materials/Paintings/BackShadow");
            }

            return _backShadowMaterial;
        }
    }
    #endregion

    #region serialized parameters
    [SerializeField] [Range(0.0f, 9.0f)] float _minWidth = 0.1f;
    [SerializeField] [Range(0.0f, 10.0f)] float _maxWidth = 0.3f;
    [SerializeField] [Range(0.0f, 9.0f)] float _minHeight = 0.1f;
    [SerializeField] [Range(0.0f, 10.0f)] float _maxHeight = 0.3f;
    [SerializeField] bool spawnPaintingOnStart = true;
    [SerializeField] bool isPaintingGoalPoint = false;
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

    /* Goal Painting */
    public Painting goalPainting { get; private set; }
    int goalPaintingTriggerCount = 0;

    LineRenderer goalLineVisual = null;
    static Material _goalLineVisualMaterial = null;
    static Material goalLineVisualMaterial
    {
        get
        {
            if(_goalLineVisualMaterial == null) { _goalLineVisualMaterial = Resources.Load<Material>("Materials/Paintings/GoalLine"); }
            return _goalLineVisualMaterial;
        }
    }

    public Action OnGoalPaintingEnter;
    public Action OnGoalPaintingExit;
    #endregion

    #region unity methods
    // Start is called before the first frame update
    void Start()
    {
        ChoosePaintingParameters();
        CreatePaintingStand();

        if(spawnPaintingOnStart)
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

    /* Painting Goal */

    private void OnTriggerEnter(Collider other)
    {
        if (!isPaintingGoalPoint)
            return;

        Painting painting = other.GetComponentInParent<Painting>();
        if (painting == null)
            return;

        // need to check for multiple triggers of same painting.
        if (painting == goalPainting)
            goalPaintingTriggerCount++;
        else
        {
            goalPainting = painting;
            goalPaintingTriggerCount = 1;
            EnableGoalVisual();

            OnGoalPaintingEnter?.Invoke();
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (!isPaintingGoalPoint)
            return;

        Painting painting = other.GetComponentInParent<Painting>();
        if (painting == null)
            return;

        // need to check for multiple triggers of same painting.
        if (painting == goalPainting)
        {
            goalPaintingTriggerCount--;
            Debug.Assert(goalPaintingTriggerCount >= 0);
        }

        if(goalPaintingTriggerCount == 0)
        {
            goalPainting = null;
            DisableGoalVisual();

            OnGoalPaintingExit?.Invoke();
        }
    }

    #endregion

    #region spawn
    private void ChoosePaintingParameters()
    {
        spawnWidth = UnityEngine.Random.Range(minWidth, maxWidth);
        spawnHeight = UnityEngine.Random.Range(minHeight, maxHeight);
    }

    private void CreatePaintingStand()
    {
        float nailWidth = 0.02f;

        // nail left.

        GameObject nailLeft = GameObject.CreatePrimitive(PrimitiveType.Cube);
        nailLeft.transform.localScale = Vector3.one * nailWidth;
        nailLeft.transform.SetParent(this.transform);
        nailLeft.transform.position = this.transform.position + this.transform.rotation * (
            -Vector3.right * ((spawnWidth - nailWidth) / 2.0f - Painting.FRAME_WIDTH) +
            Vector3.up * ((spawnHeight - nailWidth) / 2.0f - Painting.FRAME_WIDTH));

        // nail right.

        GameObject nailRight = GameObject.CreatePrimitive(PrimitiveType.Cube);
        nailRight.transform.localScale = Vector3.one * nailWidth;
        nailRight.transform.SetParent(this.transform);
        nailRight.transform.position = this.transform.position + this.transform.rotation * (
            Vector3.right * ((spawnWidth - nailWidth) / 2.0f - Painting.FRAME_WIDTH) +
            Vector3.up * ((spawnHeight - nailWidth) / 2.0f - Painting.FRAME_WIDTH));

        // back painting shadow.
        GameObject backShadow = GameObject.CreatePrimitive(PrimitiveType.Quad);
        Destroy(backShadow.GetComponent<Collider>());
        backShadow.transform.localScale = new Vector3(spawnWidth, spawnHeight, 1.0f);
        backShadow.transform.SetParent(this.transform);
        backShadow.transform.position = this.transform.position + this.transform.rotation * (
            -Vector3.forward * Painting.DEPTH / 2.0f);
        backShadow.transform.localRotation = Quaternion.AngleAxis(180.0f, Vector3.up);
        Renderer paintRenderer = backShadow.GetComponent<Renderer>();
        paintRenderer.material = backShadowMaterial;

        /* Goal painting */

        if(isPaintingGoalPoint)
        {
            // add trigger for detecting paintings.
            BoxCollider boxCollider = this.gameObject.AddComponent<BoxCollider>();
            boxCollider.size = new Vector3(spawnWidth, spawnHeight, Painting.DEPTH * 2.0f);
            boxCollider.isTrigger = true;

            // add line renderer for visualising goal.
            GameObject goalVisual = new GameObject("Goal visual");
            goalVisual.transform.SetParent(this.transform);
            goalVisual.transform.localPosition = Vector3.zero;
            goalVisual.transform.localRotation = Quaternion.AngleAxis(180.0f, this.transform.up);
            goalLineVisual = goalVisual.AddComponent<LineRenderer>();

            BuildGoalVisual();

            DisableGoalVisual();
        }
    }

    public void SpawnPainting()
    {
        if (spawnedPainting != null)
            Destroy(spawnedPainting);

        Painting.Init(spawnPoint, this.transform.rotation, spawnWidth, spawnHeight);
    }

    /* Goal Painting */

    private void BuildGoalVisual()
    {
        float offset = 0.03f;
        float width = 0.015f;
        float halfWidth = spawnWidth * 0.5f;
        float halfHeight = spawnHeight * 0.5f;

        goalLineVisual.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        goalLineVisual.receiveShadows = false;
        goalLineVisual.alignment = LineAlignment.TransformZ;
        goalLineVisual.loop = true;
        goalLineVisual.startWidth = width;
        goalLineVisual.endWidth = width;
        goalLineVisual.positionCount = 8;
        goalLineVisual.useWorldSpace = false;
        goalLineVisual.SetPositions(new Vector3[8]
        {
            new Vector3(-halfWidth, -halfHeight - offset, 0.0f),
            new Vector3(halfWidth, -halfHeight - offset, 0.0f),
            new Vector3(halfWidth + offset, -halfHeight, 0.0f),
            new Vector3(halfWidth + offset, halfHeight, 0.0f),
            new Vector3(halfWidth, halfHeight + offset, 0.0f),
            new Vector3(-halfWidth, halfHeight + offset, 0.0f),
            new Vector3(-halfWidth - offset, halfHeight, 0.0f),
            new Vector3(-halfWidth - offset, -halfHeight, 0.0f),
        });
        goalLineVisual.material = goalLineVisualMaterial;
    }

    public void EnableGoalVisual()
    {
        float goalScore = ComputeGoalScore();
        goalLineVisual.material.color = Color.Lerp(Color.red, Color.green, goalScore * goalScore * goalScore);
    }

    public void DisableGoalVisual()
    {
        goalLineVisual.material.color = new Color(1.0f, 1.0f, 1.0f, 0.1f);
    }

    public float ComputeGoalScore()
    {
        float goalWidth = goalPainting != null ? goalPainting.width : 0.0f;
        float goalHeight = goalPainting != null ? goalPainting.height : 0.0f;

        float ratioWidth = Mathf.Min(goalWidth / (spawnWidth + 1e-8f), spawnWidth / (goalWidth + 1e-8f));
        float ratioHeight = Mathf.Min(goalHeight / (spawnHeight + 1e-8f), spawnHeight / (goalHeight + 1e-8f));

        return (ratioWidth + ratioHeight) * 0.5f;
    }

    #endregion
}
