using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class GameGrader : MonoBehaviour
{
    #region parameters
    PaintingSpawner[] goalPaintSpawners;
    PaintingRelationship[] goalPaintRelationships;

    public float lastScore { get; private set; }
    public int lastGrade { get; private set; }
    #endregion

    #region unity methods
    // Start is called before the first frame update
    IEnumerator Start()
    {
        GenerationManager generationManager = FindObjectOfType<GenerationManager>(); 
        while(true)
        {
            if (generationManager.startRoom == null)
                yield return new WaitForEndOfFrame();
            else
                break;
        }

        goalPaintSpawners = FindObjectsOfType<PaintingSpawner>().Where(x => x.isPaintingGoalPoint).ToArray();
        goalPaintRelationships = FindObjectsOfType<PaintingRelationship>();
        
        foreach(PaintingSpawner p in goalPaintSpawners)
        {
            p.OnGoalScoreChanged += UpdateScoreAndGrade;
        }

        foreach(PaintingRelationship p in goalPaintRelationships)
        {
            p.OnGoalScoreChanged += UpdateScoreAndGrade;
        }

        UpdateScoreAndGrade();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    #endregion

    #region scoring
    float ComputeCompleteScore()
    {
        float percentSize = 0.25f;

        float sizeScore = 0.0f;
        float synergyScore = 0.0f;

        foreach(PaintingSpawner p in goalPaintSpawners)
        {
            sizeScore += p.lastGoalScore;
        }
        sizeScore /= (float)goalPaintSpawners.Length;

        foreach(PaintingRelationship p in goalPaintRelationships)
        {
            synergyScore += p.lastGoalScore;
        }
        synergyScore /= (float)goalPaintRelationships.Length;

        float score = percentSize * sizeScore + (1.0f - percentSize) * synergyScore;

        return score;
    }

    int ComputeGradeFromScore(float score)
    {
        // A+
        if (score > 0.95f)
            return 0;

        // A
        if (score > 0.9f)
            return 1;

        // B+
        if (score > 0.85f)
            return 2;

        // B
        if (score > 0.8f)
            return 3;

        // C+
        if (score > 0.75f)
            return 4;

        // C
        if (score > 0.7f)
            return 5;

        // F
        return 6;
    }

    void UpdateScoreAndGrade()
    {
       lastScore = ComputeCompleteScore();
        lastGrade = ComputeGradeFromScore(lastScore);
    }

    #endregion
}

#if UNITY_EDITOR
[CustomEditor(typeof(GameGrader))]
public class E_GameGrader : Editor
{
    GameGrader gameGrader;

    private void OnEnable()
    {
        gameGrader = (GameGrader)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.LabelField($"Score {gameGrader.lastScore}");
        EditorGUILayout.LabelField($"Grade {gameGrader.lastGrade}");
    }
}
    #endif
