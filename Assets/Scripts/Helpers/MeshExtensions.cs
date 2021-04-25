using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public static class MeshExtensions
{
    #region data structures
    /* Data structures */

    public static Vector3[] GetMeshBoundPoints(this Mesh mesh)
    {
        Vector3[] meshAABBcorners = new Vector3[]
        {
            mesh.bounds.min,
            new Vector3(mesh.bounds.min.x, mesh.bounds.max.y, mesh.bounds.max.z),
            new Vector3(mesh.bounds.max.x, mesh.bounds.min.y, mesh.bounds.max.z),
            new Vector3(mesh.bounds.max.x, mesh.bounds.max.y, mesh.bounds.min.z),
            new Vector3(mesh.bounds.min.x, mesh.bounds.min.y, mesh.bounds.max.z),
            new Vector3(mesh.bounds.min.x, mesh.bounds.max.y, mesh.bounds.min.z),
            new Vector3(mesh.bounds.max.x, mesh.bounds.min.y, mesh.bounds.min.z),
            mesh.bounds.max
        };

        return meshAABBcorners;
    }
    #endregion

    #region vertex searches
    /* Vertex Searches */

    public static Vector3 NearestVertexToLocalPoint(this Mesh mesh, Vector3 localPoint)
    {
        Vector3 nearestPoint = Vector3.zero;
        float nearestDist = float.MaxValue;
        foreach(Vector3 vert in mesh.vertices)
        {
            float distToVert = (vert - localPoint).sqrMagnitude;
            if(distToVert < nearestDist)
            {
                distToVert = nearestDist;
                nearestPoint = localPoint;
            }
        }

        Debug.Assert(nearestPoint != Vector3.zero);

        return nearestPoint;
    }

    public static Vector3 FurthestVertexToLocalPoint(this Mesh mesh, Vector3 localPoint)
    {
        Vector3 furthestPoint = Vector3.zero;
        float furthestDist = float.MinValue;
        foreach(Vector3 vert in mesh.vertices)
        {
            float distToVert = (vert - localPoint).sqrMagnitude;
            if(distToVert > furthestDist)
            {
                distToVert = furthestDist;
                furthestPoint = localPoint;
            }
        }

        Debug.Assert(furthestPoint != Vector3.zero);

        return furthestPoint;
    }

    public static Vector3 FurthestBoundToLocalPoint(this Mesh mesh, Vector3 localPoint)
    {
        Vector3 furthestBound = Vector3.zero;
        float furthestDist = float.MinValue;
        foreach(Vector3 p in mesh.GetMeshBoundPoints())
        {
            float d = (p - localPoint).sqrMagnitude;
            if(d > furthestDist)
            {
                furthestDist = d;
                furthestBound = p;
            }
        }

        return furthestBound;
    }

    public static Vector3 NearestBoundToLocalPoint(this Mesh mesh, Vector3 localPoint)
    {
        Vector3 nearestBound = Vector3.zero;
        float nearestDist = float.MaxValue;
        foreach(Vector3 p in mesh.GetMeshBoundPoints())
        {
            float d = (p - localPoint).sqrMagnitude;
            if(d < nearestDist)
            {
                nearestDist = d;
                nearestBound = p;
            }
        }

        return nearestBound;
    }
    #endregion
}
