using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintingRelationship : MonoBehaviour
{
    #region serialized parameters
    [SerializeField] PaintingSpawner spawnerA;
    [SerializeField] PaintingSpawner spawnerB;
    #endregion

    #region parameters
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

    public float lastGoalScore { get; private set; }

    public Action OnGoalScoreChanged;
    #endregion

    #region unity method
    // Start is called before the first frame update
    void Start()
    {
        spawnerA.OnGoalPaintingEnter += EnablePaintingRelationship;
        spawnerB.OnGoalPaintingEnter += EnablePaintingRelationship;

        spawnerA.OnGoalPaintingExit += DisablePaintingRelationship;
        spawnerB.OnGoalPaintingExit += DisablePaintingRelationship;

        Build();

        DisablePaintingRelationship();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    #endregion

    #region line visual
    void Build()
    {
        goalLineVisual = this.gameObject.AddComponent<LineRenderer>();
        this.transform.rotation = Quaternion.AngleAxis(180.0f, Vector3.up);

        float width = 0.015f;

        goalLineVisual.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        goalLineVisual.receiveShadows = false;
        goalLineVisual.alignment = LineAlignment.View;
        goalLineVisual.startWidth = width;
        goalLineVisual.endWidth = width;
        goalLineVisual.positionCount = 2;
        goalLineVisual.useWorldSpace = true;
        goalLineVisual.SetPositions(new Vector3[2]
        {
            spawnerA.transform.position - this.transform.forward * Painting.DEPTH * 0.5f,
            spawnerB.transform.position - this.transform.forward * Painting.DEPTH * 0.5f,
        });
        goalLineVisual.material = goalLineVisualMaterial;
    }
    #endregion

    #region relationship methods
    public float ComputeRelationshipScore()
    {
        if (spawnerA.goalPainting == null || spawnerB.goalPainting == null)
            return 0.0f;

        // get the texture data.

        RenderTexture paintingArT = spawnerA.goalPainting.paintingTexture;
        RenderTexture paintingBrT = spawnerB.goalPainting.paintingTexture;

        Texture2D paintingA = new Texture2D(paintingArT.width, paintingArT.height, TextureFormat.RGB24, false);
        CustomRenderTexture.active = paintingArT;
        paintingA.ReadPixels(new Rect(0, 0, paintingA.width, paintingA.height), 0, 0);
        float[] colorCountsA = new float[3];
        for(int x = 0; x < paintingA.width; ++x)
        {
            for(int y = 0; y < paintingA.height; ++y)
            {
                colorCountsA[0] += paintingA.GetPixel(x, y).r;
                colorCountsA[1] += paintingA.GetPixel(x, y).g;
                colorCountsA[2] += paintingA.GetPixel(x, y).b;
            }
        }

        Texture2D paintingB = new Texture2D(paintingBrT.width, paintingBrT.height, TextureFormat.RGB24, false);
        CustomRenderTexture.active = paintingBrT;
        paintingB.ReadPixels(new Rect(0, 0, paintingB.width, paintingB.height), 0, 0);
        float[] colorCountsB = new float[3];
        for(int x = 0; x < paintingB.width; ++x)
        {
            for(int y = 0; y < paintingB.height; ++y)
            {
                colorCountsB[0] += paintingB.GetPixel(x, y).r;
                colorCountsB[1] += paintingB.GetPixel(x, y).g;
                colorCountsB[2] += paintingB.GetPixel(x, y).b;
            }
        }

        CustomRenderTexture.active = null;

        // normalize.

        for(int i = 0; i < 3; ++i)
        {
            colorCountsA[i] /= colorCountsA[0] + colorCountsA[1] + colorCountsA[2];
            colorCountsB[i] /= colorCountsB[0] + colorCountsB[1] + colorCountsB[2];
        }

        // compare color values.

        float ratioAvg = 0.0f;
        for(int i = 0; i < 3; ++ i)
        {
            ratioAvg += Mathf.Min(colorCountsA[i] / (colorCountsB[i] + 1e-8f), colorCountsB[i] / (colorCountsA[i] + 1e-8f));
        }

        return ratioAvg / 3.0f;
    }
    
    public void EnablePaintingRelationship()
    {
        if (spawnerA.goalPainting == null || spawnerB.goalPainting == null)
            return;

        lastGoalScore = ComputeRelationshipScore();
        goalLineVisual.material.color = Color.Lerp(Color.red, Color.green, lastGoalScore * lastGoalScore * lastGoalScore);

        OnGoalScoreChanged?.Invoke();
    }

    public void DisablePaintingRelationship()
    {
        if (spawnerA.goalPainting != null && spawnerB.goalPainting != null)
            return;

        lastGoalScore = 0.0f;
        goalLineVisual.material.color = new Color(1.0f, 1.0f, 1.0f, 0.1f);

        OnGoalScoreChanged?.Invoke();
    }
    #endregion
}
