using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;

public class BakeTextureWindow : EditorWindow
{
    Material imageMaterial;
    string imageSourceFile = "Assets/Resources/Textures/tex_00.png";
    Vector2Int imageSize;

    bool hasMaterial;
    bool hasSource;
    bool hasSize;

    [MenuItem("Tools/Texture Bake")]
    static void OpenWindow()
    {
        BakeTextureWindow window = EditorWindow.GetWindow<BakeTextureWindow>();
        window.Show();
        window.CheckInput();
    }

    public void OnGUI()
    {
        // help dialogue.
        EditorGUILayout.HelpBox("Set the material to bake, the image size, and the target source file. \nClick 'Bake'.", MessageType.Info);

        using (var check = new EditorGUI.ChangeCheckScope())
        {
            imageMaterial = (Material)EditorGUILayout.ObjectField("Image Material", imageMaterial, typeof(Material), true);
            imageSize = EditorGUILayout.Vector2IntField("Image Size", imageSize);
            //imageSourceFile = EditorGUILayout.TextField("Image Source File", imageSourceFile);
            imageSourceFile = FileField(imageSourceFile);

            if(check.changed)
                CheckInput();
        }

        GUI.enabled = hasMaterial && hasSource && hasSize;
        if(GUILayout.Button("Bake"))
            BakeTexture();
        GUI.enabled = true;

        // warning dialogue boxes.
        if(!hasMaterial)
            EditorGUILayout.HelpBox("material property missing.", MessageType.Warning);

        if(!hasSize)
            EditorGUILayout.HelpBox("image size must be greater than zero.", MessageType.Warning);

        if (!hasSource)
            EditorGUILayout.HelpBox("source invalid or the image file is not of type .png", MessageType.Warning);
    }

    string FileField(string path)
    {
        EditorGUILayout.LabelField("Image Source File");
        using (new GUILayout.HorizontalScope())
        {
            path = EditorGUILayout.TextField(path);
            if(GUILayout.Button("select"))
            {
                string directory = "Assets/Resources/Textures";
                string fileName = "tex_00.png";
                try
                {
                    directory = Path.GetDirectoryName(path);
                    fileName = Path.GetFileName(path);
                } catch (ArgumentException) { }
                string selectedFile = EditorUtility.SaveFilePanelInProject("choose texture file", fileName, "png", "enter the file name for the texture", directory);
                if (!string.IsNullOrEmpty(selectedFile))
                    path = selectedFile;

                Repaint();
            }
        }

        return path;
    }

    private void CheckInput()
    {
        hasMaterial = imageMaterial != null;
        hasSize = imageSize.x > 0 && imageSize.y > 0;

        hasSource = false;
        try
        {
            string ext = Path.GetExtension(imageSourceFile);
            hasSource = ext.Equals(".png");
        }
        catch (ArgumentException) { }
    }

    private void BakeTexture()
    {
        // capture material shader in a RenderTexture.
        RenderTexture renderTexture = RenderTexture.GetTemporary(imageSize.x, imageSize.y);
        Graphics.Blit(null, renderTexture, imageMaterial);

        // set the RenderTexture and write the render pixels to a texture2D.
        Texture2D texture = new Texture2D(imageSize.x, imageSize.y);
        RenderTexture.active = renderTexture;
        texture.ReadPixels(new Rect(Vector3.zero, imageSize), 0, 0); // read a rectangle of the whole texture and insert from pixel (0,0).

        // save the texture to a file.
        byte[] pngBytes = texture.EncodeToPNG();
        File.WriteAllBytes(imageSourceFile, pngBytes);
        AssetDatabase.Refresh(); // refresh the asset directory so the new file is visible.

        // clean-up.
        RenderTexture.active = null;
        RenderTexture.ReleaseTemporary(renderTexture);
        DestroyImmediate(texture);
    }
}
