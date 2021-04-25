using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Hashing
{
    public delegate int GetDimension(int i);
    public static int ThreeDToIndex(int i, int j, int k, GetDimension g)
    {
        return ThreeDToIndex(i, j, k, g(0), g(1), g(2));
    }
    public static int ThreeDToIndex(int i, int j, int k, int l, int w, int h)
    {
        return i * l*w + j * w + k;
    }
    public static void IndexToThreeD(int idx, GetDimension g, out int i, out int j, out int k)
    {
        IndexToThreeD(idx, g(0), g(1), g(2), out i, out j, out k);
    }
    public static void IndexToThreeD(int idx, int l, int w, int h, out int i, out int j, out int k)
    {
        int area = l * w;
        i = Mathf.FloorToInt(idx / area);
        j = Mathf.FloorToInt((idx - i * area) / w);
        k = (idx - i * area - j * w);
    }
}
