using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SimplePaintGenerator
{
    public static Texture2D Generate(int textureWidth, int textureHeight)
    {
        UnityEngine.Profiling.Profiler.BeginSample("TexGen::CreatePaintTexture");

        Texture2D painting = new Texture2D(textureWidth, textureHeight, TextureFormat.RGB24, true);
        painting.wrapMode = TextureWrapMode.Clamp;
        painting.filterMode = FilterMode.Point;

        UnityEngine.Profiling.Profiler.EndSample();

        UnityEngine.Profiling.Profiler.BeginSample("TexGen::FillTexture");

        for(int i = 0; i < textureWidth; ++i)
        {
            for(int j = 0; j < textureWidth; ++j)
            {
                painting.SetPixel(i, j, new Color(
                    Random.Range(0.0f, 1.0f),
                    Random.Range(0.0f, 1.0f),
                    Random.Range(0.0f, 1.0f)));
            }
        }
        painting.Apply();

        UnityEngine.Profiling.Profiler.EndSample();

        return painting;
    }
}
