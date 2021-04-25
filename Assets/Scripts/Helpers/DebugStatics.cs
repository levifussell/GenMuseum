using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DebugStatics
{
    private static Material _debugMaterial1;
    public static Material debugMaterial1
    {
        get
        {
            _debugMaterial1 = _debugMaterial1 == null ? Resources.Load<Material>("Materials/Debug/grid3") : _debugMaterial1;
            return _debugMaterial1;
        }
    }

    private static Material _debugMaterial2;
    public static Material debugMaterial2
    {
        get
        {
            _debugMaterial2 = _debugMaterial2 == null ? Resources.Load<Material>("Materials/Debug/grid15") : _debugMaterial2;
            return _debugMaterial2;
        }
    }
}
