using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Vector3Extensions
{
    public static Vector3 Clamp(this Vector3 instance, float min, float max)
    {
        Vector3 clamped = new Vector3(
            Mathf.Clamp(instance.x, min, max),
            Mathf.Clamp(instance.y, min, max),
            Mathf.Clamp(instance.z, min, max)
            );
        return clamped;
    }

    public static Vector3Int Clamp(this Vector3Int instance, int min, int max)
    {
        Vector3Int clamped = new Vector3Int(
            Mathf.Clamp(instance.x, min, max),
            Mathf.Clamp(instance.y, min, max),
            Mathf.Clamp(instance.z, min, max)
            );
        return clamped;
    }

    public static Vector3 Mul(this Vector3 instance, Vector3 other)
    {
        return new Vector3(
            instance.x * other.x,
            instance.y * other.y,
            instance.z * other.z
            );
    }
    public static Vector3 Div(this Vector3 instance, Vector3 other)
    {
        return new Vector3(
            instance.x / other.x,
            instance.y / other.y,
            instance.z / other.z
            );
    }

    public static Vector3 Min(Vector3 a, Vector3 b)
    {
        return new Vector3(
            Mathf.Min(a.x, b.x),
            Mathf.Min(a.y, b.y),
            Mathf.Min(a.z, b.z)
            );
    }
    public static Vector3 Max(Vector3 a, Vector3 b)
    {
        return new Vector3(
            Mathf.Max(a.x, b.x),
            Mathf.Max(a.y, b.y),
            Mathf.Max(a.z, b.z)
            );
    }

    public static Vector3 RandomSphere(float radius = 1.0f)
    {
        Vector3 sphereVec = new Vector3(
            Random.Range(-1.0f, 1.0f),
            Random.Range(-1.0f, 1.0f),
            Random.Range(-1.0f, 1.0f));
        return sphereVec.normalized * radius;
    }
}
