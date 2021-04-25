using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MathExtra
{
    public static bool IsPowerOfTwo(int x)
    {
        return ((x & (x - 1)) == 0) && (x != 0);
    }
}
