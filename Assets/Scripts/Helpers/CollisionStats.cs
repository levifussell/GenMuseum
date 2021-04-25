using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class CollisionStats : MonoBehaviour
{
    public int m_collisionsCount { get { return m_collisionList.Count; } }
    public HashSet<GameObject> m_collisionList { get; private set; }

    public Action<CollisionStats> onNoCollisions;
    public Action<CollisionStats> onCollisions;

    private void Awake()
    {
        m_collisionList = new HashSet<GameObject>();
    }

    private void Start()
    {
        // if this object contains many colliders, for now we are going to skip this because it is buggy.
        int colliderCount = this.GetComponentsInChildren<Collider>().Length;

        if(colliderCount < 5)
        {
            foreach (GameObject g in PhysicsExtensions.GetAllUniqueCollidingGameObjectsForBoxColliders(this.gameObject))
                m_collisionList.Add(g);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        bool atZero = m_collisionsCount == 0;

        if (!m_collisionList.Contains(collision.gameObject))
            m_collisionList.Add(collision.gameObject);

        if (m_collisionsCount != 0 && atZero)
            onCollisions?.Invoke(this);
    }

    private void OnCollisionExit(Collision collision)
    {
        bool atNotZero = m_collisionsCount != 0;

        if (m_collisionList.Contains(collision.gameObject))
            m_collisionList.Remove(collision.gameObject);

        if (m_collisionsCount == 0 && atNotZero)
            onNoCollisions?.Invoke(this);
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(CollisionStats))]
public class E_CollisionStats : Editor
{
    public override void OnInspectorGUI()
    {
        EditorGUILayout.LabelField("Collision Count: " + (target as CollisionStats).m_collisionsCount);
    }
}
#endif
