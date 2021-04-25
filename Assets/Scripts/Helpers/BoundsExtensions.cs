using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BoundsExtensions 
{
    public static Vector3[] ComputeAndGetCorners(this Bounds instance)
    {
        Vector3[] corners = new Vector3[]
        {
            new Vector3(instance.min.x, instance.min.y, instance.min.z),
            new Vector3(instance.min.x, instance.min.y, instance.max.z),
            new Vector3(instance.min.x, instance.max.y, instance.min.z),
            new Vector3(instance.max.x, instance.min.y, instance.min.z),
            new Vector3(instance.max.x, instance.max.y, instance.max.z),
            new Vector3(instance.max.x, instance.max.y, instance.min.z),
            new Vector3(instance.max.x, instance.min.y, instance.max.z),
            new Vector3(instance.min.x, instance.max.y, instance.max.z),
        };

        return corners;
    }
}
