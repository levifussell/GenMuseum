using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public static class ComponentExtensions
{
    public static T GetCopyOf<T>(this Component instance, T other) where T : Component
    {
        Type type = instance.GetType();
        if (type != other.GetType()) return null;

        BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Default | BindingFlags.DeclaredOnly;

        PropertyInfo[] pInfos = type.GetProperties(flags);
        foreach(var pInfo in pInfos)
        {
            if(pInfo.CanWrite)
            {
                try
                {
                    pInfo.SetValue(instance, pInfo.GetValue(other, null), null);
                }
                catch { }
            }
        }

        FieldInfo[] fInfos = type.GetFields(flags);
        foreach(var fInfo in fInfos)
        {
            fInfo.SetValue(instance, fInfo.GetValue(other));
        }

        return instance as T;
    }
}
