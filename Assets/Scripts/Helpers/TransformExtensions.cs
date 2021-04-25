using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public static class TransformExtensions
{
    public static Transform GetParentOrSelf(this Transform t)
    {
        return t.parent == null ? t : t.parent;
    }

    public static Vector3 TransformPointByParentOrNone(this Transform t, Vector3 point)
    {
        if (t.parent != null)
        {
            return t.parent.TransformPoint(point);
        }
        else
        {
            return point;
        }
    }
}
