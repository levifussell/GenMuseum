using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderVisualiser : MonoBehaviour
    /*
     * Visualises all the colliders on the object and its children.
     */
{
    Collider[] colliders;

    // Start is called before the first frame update
    void Start()
    {
        colliders = this.GetComponentsInChildren<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void DrawCollider(Collider c)
    {
        switch(c)
        {
            case BoxCollider b:
                Gizmos.DrawWireCube(b.center, b.size);
                break;
            case SphereCollider s:
                Gizmos.DrawWireSphere(s.center, s.radius);
                break;
            default:
                Debug.LogError("type {0} is not supported for visualisation.");
                break;
        }
    }
}
