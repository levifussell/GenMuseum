using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JointLinearSpring3D : MonoBehaviour
{
    [SerializeField]
    public Rigidbody connectedBody;

    [SerializeField]
    public float strength;

    [SerializeField]
    public float smoothThreshold = 0.02f;

    [SerializeField]
    public Vector3 offset;

    [SerializeField]
    public bool useForce = true;

    public Vector3 lastPositionUpdate { get; private set; }

    Rigidbody rigidbody;

    private void Awake()
    {
        rigidbody = this.GetComponent<Rigidbody>();
    }

    void Start()
    {
        Collider cA = this.GetComponent<Collider>();
        Collider cB = connectedBody.GetComponent<Collider>();
        if(cA != null && cB != null)
            Physics.IgnoreCollision(this.GetComponent<Collider>(), connectedBody.GetComponent<Collider>());
    }

    private void FixedUpdate()
    {
        ConstraintConnectedBody();
    }

    void ConstraintConnectedBody()
    {
        Vector3 diff = (rigidbody.position + offset) - connectedBody.position;

        float distScalar = diff.magnitude > smoothThreshold ? 1.0f : (diff.magnitude / smoothThreshold);

        lastPositionUpdate = diff.normalized * strength * distScalar;
        Vector3 newPosition = connectedBody.position + lastPositionUpdate;
        //connectedBody.MovePosition(newPosition);
        //connectedBody.MovePosition(newPosition);
        if (useForce)
            connectedBody.AddForce(lastPositionUpdate);
        else
            connectedBody.MovePosition(newPosition);
        //connectedBody.velocity = Vector3.zero;
        //connectedBody.angularVelocity = Vector3.zero;
    }
}






