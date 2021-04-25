using System;
using UnityEngine;

public static class GameObjectExtensions
{
    #region components
    public static T AddOrGetComponent<T>(this GameObject instance) where T : Component
    {
        T comp = instance.GetComponent<T>();
        if (comp == null)
            comp = instance.AddComponent<T>();
        return comp;
    }

    public static T AddComponent<T>(this GameObject instance, T toAdd) where T : Component
    {
        return instance.AddComponent<T>().GetCopyOf(toAdd) as T;
    }

    public static bool DoFunctionIfComponentExists<T>(this GameObject instance, Action<T> func) where T : Component
    {
        //NOTE: Action<> is just a Func<> which supports zero return types.
        T component = instance.GetComponent<T>();
        if(component != null)
        {
            func(component);
            return true;
        }

        return false;
    }
    #endregion

    #region destroying
    public static void SafeDestroy(UnityEngine.Object obj)
    {
        if (Application.isPlaying)
            GameObject.Destroy(obj);
        else
            GameObject.DestroyImmediate(obj);
    }
    public static void SafeDestroy(UnityEngine.Object[] objs)
    {
        foreach(UnityEngine.Object o in objs)
        {
            SafeDestroy(o);
        }
    }

    public static bool TrySafeDestroy(UnityEngine.Object obj)
    {
        if (obj == null)
            return false;
        SafeDestroy(obj);
        return true;
    }
    #endregion

    #region constructs
    public static GameObject CreatePrimitiveNoCollider(PrimitiveType type)
    {
        GameObject obj = GameObject.CreatePrimitive(type);
        GameObject.DestroyImmediate(obj.GetComponent<Collider>());
        return obj;
    }
    #endregion
}
