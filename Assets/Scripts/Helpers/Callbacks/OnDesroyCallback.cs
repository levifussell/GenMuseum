using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnDesroyCallback : MonoBehaviour
{
    public Action<GameObject> callbacks;
    private void OnDestroy()
    {
        if(gameObject.scene.isLoaded)
            callbacks?.Invoke(this.gameObject);
    }
}
