using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class GlobalLightmapEditor : MonoBehaviour
{
    [HideInInspector]
    public bool visualiseLightmap = true;
    [HideInInspector]
    public bool visualiseLightmapColor = true;
    [HideInInspector]
    public float lightmapShadowStep = 0.5f;
    [HideInInspector]
    public float lightmapShadowAlpha = 0.5f;
    [HideInInspector]
    public float lightmapColorBlend = 0.5f;

    private void Awake()
    {
        this.SetLightmapParameters();
    }

    public void SetLightmapParameters()
    {
        Shader.SetGlobalFloat("_VisualiseLightmap", this.visualiseLightmap ? 1.0f : 0.0f);
        Shader.SetGlobalFloat("_VisualiseLightmapColor", this.visualiseLightmapColor ? 1.0f : 0.0f);
        Shader.SetGlobalFloat("_LightmapShadowStep", this.lightmapShadowStep);
        Shader.SetGlobalFloat("_LightmapShadowAlpha", this.lightmapShadowAlpha);
        Shader.SetGlobalFloat("_LightmapColorBlend", this.lightmapColorBlend);
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(GlobalLightmapEditor))]
public class E_GlobalLightmapEditor : Editor
{
    GlobalLightmapEditor globalLightmapEditor;
    public void OnEnable()
    {
        globalLightmapEditor = (GlobalLightmapEditor)target;
        globalLightmapEditor.SetLightmapParameters();
    }
    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();
        // LIGHTMAP VISUALISE --------------------
        EditorGUILayout.LabelField("Debug Visualise", EditorStyles.boldLabel);
        globalLightmapEditor.visualiseLightmap = EditorGUILayout.Toggle("Visualise Lightmap", globalLightmapEditor.visualiseLightmap);
        if(globalLightmapEditor.visualiseLightmap)
        {
            globalLightmapEditor.visualiseLightmapColor = EditorGUILayout.Toggle("\tVisualise Lightmap Color", globalLightmapEditor.visualiseLightmapColor);

            globalLightmapEditor.lightmapShadowStep = EditorGUILayout.Slider("Shadow Step", globalLightmapEditor.lightmapShadowStep, 0.0f, 1.0f);
            if(globalLightmapEditor.visualiseLightmapColor)
            {
                globalLightmapEditor.lightmapShadowAlpha = EditorGUILayout.Slider("Shadow Alpha", globalLightmapEditor.lightmapShadowAlpha, 0.0f, 1.0f);
                globalLightmapEditor.lightmapColorBlend = EditorGUILayout.Slider("Color Blend", globalLightmapEditor.lightmapColorBlend, 0.0f, 1.0f);
            }
        }

        if (EditorGUI.EndChangeCheck())
            globalLightmapEditor.SetLightmapParameters();
    }
}
#endif
