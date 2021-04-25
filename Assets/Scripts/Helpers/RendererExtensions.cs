using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RendererExtensions
{
    public static Material SafeGetMaterial(this Renderer instance)
    {
        if (Application.isPlaying)
            return instance.material;
        else
            return instance.sharedMaterial;
    }

    public static void SafeSetMaterial(this Renderer instance, Material material)
    {
        if (Application.isPlaying)
            instance.material = material;
        else
            instance.sharedMaterial = material;
    }
}
