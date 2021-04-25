using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class PhysicsExtensions
{
    public static List<Collider> OverlapAllAttachedBoxCollidersInChildren(GameObject gameObject, bool includeSelf=false)
    {
        BoxCollider[] boxColliders = gameObject.GetComponentsInChildren<BoxCollider>();

        List<Collider> allOverlaps = new List<Collider>();
        foreach(BoxCollider b in boxColliders)
        {
            Collider[] overlaps = Physics.OverlapBox(b.transform.TransformPoint(b.center), b.transform.TransformVector(b.size / 2.0f), b.transform.rotation);

            // remove self from the collision check.
            if (!includeSelf)
            {
                overlaps = overlaps.Where((x) => !boxColliders.Contains(x)).ToArray();
            }
            allOverlaps.AddRange(overlaps);
        }

        return allOverlaps;
    }

    public static List<GameObject> GetAllUniqueCollidingGameObjectsForBoxColliders(GameObject gameObject, bool includeSelf=false)
    {
        List<Collider> colliders = OverlapAllAttachedBoxCollidersInChildren(gameObject, includeSelf);
        List<GameObject> gameObjects = new List<GameObject>();
        foreach(Collider c in colliders)
        {
            if (!gameObjects.Contains(c.gameObject) && c.gameObject != gameObject)
                gameObjects.Add(c.gameObject);
        }

        return gameObjects;
    }
}
