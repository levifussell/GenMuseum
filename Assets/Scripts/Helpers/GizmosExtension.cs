using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GizmosExtensions
{
    public static void DrawArrow(Vector3 start, Vector3 end, Vector3 up, float arrowPercent=0.2f)
    {
        Gizmos.DrawLine(start, end);
        Vector3 forward = (end - start).normalized;
        Vector3 left = Vector3.Cross(forward, up);

        Gizmos.DrawLine(end, end + (left - forward) * arrowPercent);
        Gizmos.DrawLine(end, end - (left + forward) * arrowPercent);
    }

    public static void DrawDottedLine(Vector3 start, Vector3 end, float spaceSize, float segmentSize)
    {
        Vector3 rate = (end - start).normalized;
        float segCount = (end - start).magnitude / (spaceSize + segmentSize);

        for(int i = 0; i < segCount / 2; ++i)
        {
            if (i % 2 == 1)
                Gizmos.DrawLine(start + rate * (i - 1), start + rate * i);
        }
    }

    public static void DrawCirclePie(
        Vector3 origin, 
        Vector3 right, 
        Vector3 up, 
        float radius, 
        int resolution,
        float minAngle,
        float maxAngle)
    {
        Func<int, Vector3> indexToPosition = (i) => {
            float angle = minAngle* Mathf.Deg2Rad + ((maxAngle - minAngle) * Mathf.Deg2Rad) / (resolution - 1) * i;
            return origin + (right * Mathf.Cos(angle) + up * Mathf.Sin(angle)) * radius;
            };

        for (int i = 0; i < resolution; ++i)
        {
            Vector3 pos = indexToPosition(i);
            Vector3 posNext = indexToPosition((i + 1) % resolution);
            Gizmos.DrawLine(pos, posNext);
        }
    }

    public static void DrawCircle(
        Vector3 origin, 
        Vector3 right, 
        Vector3 up, 
        float radius, 
        int resolution)
    {
        DrawCirclePie(origin, right, up, radius, resolution, 0.0f, 360.0f);
    }

    public static void DrawBounds(Vector3 min, Vector3 max, Transform transform)
    {
        Vector3 tMin = transform.TransformPoint(min);
        Vector3 tMax = transform.TransformPoint(max);

        Vector3 tMinA = transform.TransformPoint(new Vector3(max.x, min.y, min.z));
        Vector3 tMinB = transform.TransformPoint(new Vector3(min.x, max.y, min.z));
        Vector3 tMinC = transform.TransformPoint(new Vector3(min.x, min.y, max.z));

        Vector3 tMaxA = transform.TransformPoint(new Vector3(min.x, max.y, max.z));
        Vector3 tMaxB = transform.TransformPoint(new Vector3(max.x, min.y, max.z));
        Vector3 tMaxC = transform.TransformPoint(new Vector3(max.x, max.y, min.z));

        Gizmos.DrawLine(tMin, tMinA);
        Gizmos.DrawLine(tMin, tMinB);
        Gizmos.DrawLine(tMin, tMinC);

        Gizmos.DrawLine(tMax, tMaxA);
        Gizmos.DrawLine(tMax, tMaxB);
        Gizmos.DrawLine(tMax, tMaxC);
    }
    public static void DrawBounds(Vector3 min, Vector3 max)
    {
        Vector3 tMin = min;
        Vector3 tMax = max;

        Vector3 tMinA = new Vector3(max.x, min.y, min.z);
        Vector3 tMinB = new Vector3(min.x, max.y, min.z);
        Vector3 tMinC = new Vector3(min.x, min.y, max.z);

        Vector3 tMaxA = new Vector3(min.x, max.y, max.z);
        Vector3 tMaxB = new Vector3(max.x, min.y, max.z);
        Vector3 tMaxC = new Vector3(max.x, max.y, min.z);

        Gizmos.DrawLine(tMin, tMinA);
        Gizmos.DrawLine(tMin, tMinB);
        Gizmos.DrawLine(tMin, tMinC);

        Gizmos.DrawLine(tMax, tMaxA);
        Gizmos.DrawLine(tMax, tMaxB);
        Gizmos.DrawLine(tMax, tMaxC);
    }

    public static void DrawWireCube(Vector3 centre, Vector3 size, Transform transform)
    {
        Vector3 halfSize = size * 0.5f;
        Vector3 lowerA = transform.TransformPoint(centre + new Vector3(halfSize.x, -halfSize.y, halfSize.z));
        Vector3 lowerB = transform.TransformPoint(centre + new Vector3(-halfSize.x, -halfSize.y, halfSize.z));
        Vector3 lowerC = transform.TransformPoint(centre + new Vector3(-halfSize.x, -halfSize.y, -halfSize.z));
        Vector3 lowerD = transform.TransformPoint(centre + new Vector3(halfSize.x, -halfSize.y, -halfSize.z));

        Vector3 upperA = transform.TransformPoint(centre + new Vector3(halfSize.x, halfSize.y, halfSize.z));
        Vector3 upperB = transform.TransformPoint(centre + new Vector3(-halfSize.x, halfSize.y, halfSize.z));
        Vector3 upperC = transform.TransformPoint(centre + new Vector3(-halfSize.x, halfSize.y, -halfSize.z));
        Vector3 upperD = transform.TransformPoint(centre + new Vector3(halfSize.x, halfSize.y, -halfSize.z));

        // bottom rectangle.
        Gizmos.DrawLine(lowerA, lowerB);
        Gizmos.DrawLine(lowerB, lowerC);
        Gizmos.DrawLine(lowerC, lowerD);
        Gizmos.DrawLine(lowerD, lowerA);

        // upper rectangle.
        Gizmos.DrawLine(upperA, upperB);
        Gizmos.DrawLine(upperB, upperC);
        Gizmos.DrawLine(upperC, upperD);
        Gizmos.DrawLine(upperD, upperA);

        // side rectangles.
        Gizmos.DrawLine(upperA, lowerA);
        Gizmos.DrawLine(upperB, lowerB);
        Gizmos.DrawLine(upperC, lowerC);
        Gizmos.DrawLine(upperD, lowerD);
    }
}
