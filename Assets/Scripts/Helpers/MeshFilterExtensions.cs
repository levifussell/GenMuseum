using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeshFilterExtensions
{
    public static Mesh SafeGetMesh(this MeshFilter instance)
    {
        if (Application.isPlaying)
            return instance.mesh;
        else
            return instance.sharedMesh;
    }

    public static void SafeSetMesh(this MeshFilter instance, Mesh mesh)
    {
        if (Application.isPlaying)
            instance.mesh = mesh;
        else
            instance.sharedMesh = mesh;
    }
}
