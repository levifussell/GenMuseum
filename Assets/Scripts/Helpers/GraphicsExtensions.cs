using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class GraphicsExtensions
{
    public static void DrawMeshNowWithMaterialPassAndMeshCleanup(Mesh mesh, Matrix4x4 matrix, Material material, bool cleanupMesh=true)
    {
        material.SetPass(0);
        Graphics.DrawMeshNow(mesh, matrix);
        if(cleanupMesh) // TODO: understand why a cleanup is needed here if the mesh is made in local scope. Some memory seems to leak...
            GameObjectExtensions.SafeDestroy(mesh);
    }
    public static void DrawMeshGroupNowWithMaterialPassAndMeshCleanup(IEnumerable<Mesh> meshes, IEnumerable<Matrix4x4> matrices, Material material, bool cleanupMesh = true)
    {
        UnityEngine.Assertions.Assert.IsTrue(meshes.Count() == matrices.Count());

        material.SetPass(0);
        var meshesAndMatrix = meshes.Zip(matrices, (mesh, matrix) => new { mesh, matrix });
        foreach (var item in meshesAndMatrix)
        {
            Graphics.DrawMeshNow(item.mesh, item.matrix);
            if(cleanupMesh)
                GameObjectExtensions.SafeDestroy(item.mesh);
        }
    }
}
