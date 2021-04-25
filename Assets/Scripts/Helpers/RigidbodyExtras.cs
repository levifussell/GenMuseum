using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RigidbodyExtras : MonoBehaviour
{
    [SerializeField]
    int solverIterations = 6;

    Rigidbody rigidbody;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.solverIterations = solverIterations;
        rigidbody.solverVelocityIterations = solverIterations;
        //rigidbody.maxDepenetrationVelocity = 10.0f; // 0.01f;
    }
}
