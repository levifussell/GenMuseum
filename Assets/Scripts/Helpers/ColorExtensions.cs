using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ColorExtensions
{
    public static Color RandomHueColor(float saturation=0.8f, float value=1.0f)
    {
        return Color.HSVToRGB(Random.Range(0.0f, 1.0f), saturation, value);
    }
}
