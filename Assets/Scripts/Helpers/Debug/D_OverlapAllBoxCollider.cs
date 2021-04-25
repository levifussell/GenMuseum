using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class D_OverlapAllBoxCollider : MonoBehaviour
{
    //List<Collider> overlapColliders = new List<Collider>();
    List<Collider> initNearbyColliders = new List<Collider>();

    // Start is called before the first frame update
    void Start()
    {
        //overlapColliders = PhysicsExtensions.OverlapAllAttachedBoxCollidersInChildren(this.gameObject);
        StartCoroutine(CheckForNearbyColliders());
    }

    IEnumerator CheckForNearbyColliders()
    {
        // wait for two physics checks.
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();

        //List<Collider> allOverlaps = PhysicsExtensions.OverlapAllAttachedBoxCollidersInChildren(this.gameObject);
        Collider[] allColliders = this.GetComponentsInChildren<Collider>();
        foreach(Collider c in allColliders)
        {
            foreach(Collider d in initNearbyColliders)
            {
                Physics.IgnoreCollision(c, d);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!initNearbyColliders.Contains(other))
        {
            initNearbyColliders.Add(other);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        foreach(Collider c in initNearbyColliders)
        {
            Gizmos.DrawWireSphere(c.gameObject.transform.position, 0.1f);
        }
        
        foreach(BoxCollider b in this.GetComponentsInChildren<BoxCollider>())
        {
            Gizmos.matrix = b.transform.localToWorldMatrix;
            Gizmos.DrawCube(b.center, b.size);
        }
    }

}
