using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class EditorGUILayoutExtensions
{
    public static Texture TextureField(string name, Texture texture)
    {
        GUILayout.BeginVertical();
        var style = new GUIStyle(GUI.skin.label);
        style.alignment = TextAnchor.UpperCenter;
        style.fixedWidth = 70;
        GUILayout.Label(name, style);
        var result = (Texture)EditorGUILayout.ObjectField(texture, typeof(Texture), false, GUILayout.Width(70), GUILayout.Height(70));
        GUILayout.EndVertical();
        return result;
    }

    public delegate void SetMaterialParam<T>(string paramName, T paramValue);
    public delegate T GetMaterialParam<T>(string paramName);
    public delegate T DrawAndGetUI<T>(string paramName, T value);

    public static void ValueToMaterialParam<T>(Material material, string paramName, SetMaterialParam<T> SetMaterial, GetMaterialParam<T> GetMaterial, DrawAndGetUI<T> DrawUI)
    {
        if(material != null)
            EditorGUI.BeginChangeCheck();

        T value = GetMaterial(paramName);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(paramName, GUILayout.Width(200));
        value = DrawUI(paramName, value);
        EditorGUILayout.EndHorizontal();

        if(material != null)
        {
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(material, string.Format("{0} parameter changed of material", paramName));
                SetMaterial(paramName, value);
            }
        }
        else
            SetMaterial(paramName, value);
    }

    public static void SliderToMaterialParam(Material material, string paramName, float minValue, float maxValue)
    {
        ValueToMaterialParam(
            material,
            paramName,
            (string p, float v) => material.SetFloat(p, v),
            (string p) => material.GetFloat(p),
            (string p, float v) => EditorGUILayout.Slider(v, minValue, maxValue, GUILayout.Width(300))
            );
    }
    public static void SliderToShaderGlobalParam(string paramName, float minValue, float maxValue)
    {
        ValueToMaterialParam(
            null,
            paramName,
            (string p, float v) => Shader.SetGlobalFloat(p, v),
            (string p) => Shader.GetGlobalFloat(p),
            (string p, float v) => EditorGUILayout.Slider(v, minValue, maxValue, GUILayout.Width(300))
            );
    }

    public static void ToggleToMaterialParam(Material material, string paramName)
    {
        //EditorGUI.BeginChangeCheck();

        //float value = material.GetFloat(paramName);
        //value = EditorGUILayout.Toggle(paramName, value == 1.0f) ? 1.0f : 0.0f;

        //if(EditorGUI.EndChangeCheck())
        //{
        //    Undo.RecordObject(material, string.Format("{0} parameter changed of material", paramName));
        //    material.SetFloat(paramName, value);
        //}
        ValueToMaterialParam(
            material,
            paramName,
            (string p, float v) => material.SetFloat(p, v),
            (string p) => material.GetFloat(p),
            (string p, float v) => EditorGUILayout.Toggle(p, v == 1.0) ? 1.0f : 0.0f
            );
    }
    public static void Vector3ToMaterialParam(Material material, string paramName)
    {
        //EditorGUI.BeginChangeCheck();

        //Vector4 value = material.GetVector(paramName);
        //value = EditorGUILayout.Vector3Field(paramName, value);

        //if(EditorGUI.EndChangeCheck())
        //{
        //    Undo.RecordObject(material, string.Format("{0} parameter changed of material", paramName));
        //    material.SetVector(paramName, value);
        //}
        ValueToMaterialParam(
            material,
            paramName,
            (string p, Vector4 v) => material.SetVector(p, v),
            (string p) => material.GetVector(p),
            (string p, Vector4 v) => EditorGUILayout.Vector3Field(p, v, GUILayout.Width(300))
            );
    }

    public static void ColorToMaterialParam(Material material, string paramName)
    {
        ValueToMaterialParam(
            material,
            paramName,
            (string p, Color v) => material.SetColor(p, v),
            (string p) => material.GetColor(p),
            (string p, Color v) => EditorGUILayout.ColorField(p, v)
            );
    }

    public static void TextureToMaterialParam(Material material, string paramName)
    {
        ValueToMaterialParam(
            material,
            paramName,
            (string p, Texture v) => material.SetTexture(p, v),
            (string p) => material.GetTexture(p),
            (string p, Texture v) => EditorGUILayoutExtensions.TextureField(p, v)
            );
    }
}
