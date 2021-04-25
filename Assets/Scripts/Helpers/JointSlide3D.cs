using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class JointSlide3D : MonoBehaviour
{
    [SerializeField]
    public Rigidbody connectedBody;

    [SerializeField]
    public Vector3 constrainDirection = new Vector3(1.0f, 0.0f, 0.0f);

    [SerializeField]
    public float constrainMin = -0.5f;

    [SerializeField]
    public float constrainMax = 0.5f;

    [SerializeField]
    public float sensitivity = 0.02f;

    [SerializeField]
    public bool projectDuringUpdate = false;

    [SerializeField]
    public bool disableVelocitiesDuringUpdate = false;

    [SerializeField]
    public bool constrainRotation = true;

    Vector3 previousProjectedPosition;

    Vector3 localConstraintDirection { get { return this.transform.rotation * constrainDirection; } }
    Vector3 minConstrainPosition { get { return this.transform.position + constrainMin * localConstraintDirection; } }
    Vector3 maxConstrainPosition { get { return this.transform.position + constrainMax * localConstraintDirection; } }

    Rigidbody rigidbody;

    private void Awake()
    {
        rigidbody = this.GetComponent<Rigidbody>();
        constrainDirection = constrainDirection.normalized;

        //StartCoroutine("ConstraintConnectedBodyWithDelay", 0.05f);
        //ChildColliderCallback cc = connectedBody.gameObject.AddComponent<ChildColliderCallback>();
        //cc.onCollisionEnterCallback = Depenetrate;

    }

    // Start is called before the first frame update
    void Start()
    {
        Collider cA = this.GetComponent<Collider>();
        Collider cB = connectedBody.GetComponent<Collider>();
        if(cA != null && cB != null)
            Physics.IgnoreCollision(this.GetComponent<Collider>(), connectedBody.GetComponent<Collider>());
    }

    private void FixedUpdate()
    {
        if (!projectDuringUpdate)
            ConstraintConnectedBody();
    }

    void Update()
    {
        if (projectDuringUpdate)
            ConstraintConnectedBody();
    }

    Vector3 GetAndComputeProjectPosition() { return this.transform.position + Vector3.Dot(connectedBody.position - this.transform.position, localConstraintDirection) * localConstraintDirection; }


    public void ConstraintConnectedBody()
    {
        Vector3 projectedPosition = GetAndComputeProjectPosition();

        if (Vector3.Dot(projectedPosition - minConstrainPosition, localConstraintDirection) < 0.0f)
            projectedPosition = minConstrainPosition;
        else if (Vector3.Dot(projectedPosition - maxConstrainPosition, -localConstraintDirection) < 0.0f)
            projectedPosition = maxConstrainPosition;

        if ((projectedPosition - previousProjectedPosition).magnitude < sensitivity && previousProjectedPosition != Vector3.zero)
            projectedPosition = previousProjectedPosition;
        else
            previousProjectedPosition = projectedPosition;


        // move the connected
        Vector3 newPositionConnected = connectedBody.position;
        Vector3 newPositionBase = this.rigidbody.position;
        if(!connectedBody.isKinematic && !this.rigidbody.isKinematic)
        {
            Vector3 updateAmount = projectedPosition - connectedBody.position;
            newPositionConnected = connectedBody.position + 0.5f * updateAmount;
            newPositionBase = this.rigidbody.position - 0.5f * updateAmount;
        }
        else if(!connectedBody.isKinematic) {
            newPositionConnected = projectedPosition;
        }
        else if(!this.rigidbody.isKinematic) {
            newPositionBase = this.rigidbody.position - (projectedPosition - connectedBody.position);
        }

        if(projectDuringUpdate)
        {
            connectedBody.transform.position = newPositionConnected;
            this.rigidbody.transform.position = newPositionBase;

            if(constrainRotation)
                connectedBody.rotation = this.transform.rotation;

            if(disableVelocitiesDuringUpdate)
            {
                this.rigidbody.velocity = Vector3.zero;
                this.rigidbody.angularVelocity = Vector3.zero;
                connectedBody.velocity = Vector3.zero;
                connectedBody.angularVelocity = Vector3.zero;
            }
        }
        else
        {
            connectedBody.MovePosition(newPositionConnected);
            this.rigidbody.MovePosition(newPositionBase);

            this.rigidbody.velocity = Vector3.zero;
            this.rigidbody.angularVelocity = Vector3.zero;
            connectedBody.velocity = Vector3.zero;
            connectedBody.angularVelocity = Vector3.zero;

            //TODO: project rotation.
            if(constrainRotation)
                connectedBody.MoveRotation(this.transform.rotation);
            
            //connectedBody.transform.position = newPositionConnected;
            //this.rigidbody.transform.position = newPositionBase;

            //connectedBody.rotation = this.transform.rotation;
        }
    }

    //private void OnCollisionEnter(Collision collision)
    //{
    //    Depenetrate(collision);
    //}

    // auto de-penetration along axis.
    //void Depenetrate(Collision collision)
    //{
    //    int steps = 10;
    //    Collider colliderFixed = collision.collider;
    //    Collider colliderMovable = connectedBody.GetComponent<Collider>();
    //    //float ep = 0.01f;
    //    //float stepRate = 0.01f;
    //    //for(int i = 0; i < steps; ++i)
    //    //{
    //    //    Vector3 grad = DepenetrationFiniteDifferencesGradient(colliderFixed, colliderMovable, ep);
    //    //    connectedBody.transform.position += grad * stepRate;
    //    //}
    //    Vector3 fixedUpper = Physics.ClosestPoint(
    //        this.transform.position + constrainDirection * 100.0f,
    //        colliderFixed, colliderFixed.bounds.center, colliderFixed.transform.rotation);
    //    Vector3 fixedLower = Physics.ClosestPoint(
    //        this.transform.position - constrainDirection * 100.0f,
    //        colliderFixed, colliderFixed.bounds.center, colliderFixed.transform.rotation);
    //    Vector3 movableUpper = Physics.ClosestPoint(
    //        this.transform.position + constrainDirection * 100.0f,
    //        colliderMovable, colliderMovable.transform.position, colliderMovable.transform.rotation);
    //    Vector3 movableLower = Physics.ClosestPoint(
    //        this.transform.position - constrainDirection * 100.0f,
    //        colliderMovable, colliderMovable.transform.position, colliderMovable.transform.rotation);

    //    Debug.DrawLine(fixedUpper, movableLower, Color.red, 2.0f);
    //    Debug.DrawLine(fixedLower, movableUpper, Color.blue, 2.0f);

    //    Vector3 upperToLower = fixedUpper - movableLower;
    //    Vector3 lowerToUpper = fixedLower - movableUpper;
    //    if (upperToLower.magnitude < lowerToUpper.magnitude)
    //        connectedBody.transform.position = this.transform.position + Vector3.Dot(upperToLower, constrainDirection) * constrainDirection;
    //    else
    //        connectedBody.transform.position = this.transform.position + Vector3.Dot(lowerToUpper, constrainDirection) * constrainDirection;
    //}

    //Vector3 DepenetrationFiniteDifferencesGradient(Collider colliderFixed, Collider colliderMovable, float epsilon)
    //{
    //    Physics.ComputePenetration(
    //        colliderFixed, colliderFixed.bounds.center, colliderFixed.transform.rotation,
    //        colliderMovable, colliderMovable.bounds.center - epsilon * constrainDirection, colliderMovable.transform.rotation,
    //        out Vector3 diffA, out float distanceA);
    //    Physics.ComputePenetration(
    //        colliderFixed, colliderFixed.bounds.center, colliderFixed.transform.rotation,
    //        colliderMovable, colliderMovable.bounds.center + epsilon * constrainDirection, colliderMovable.transform.rotation,
    //        out Vector3 diffB, out float distanceB);

    //    Debug.DrawLine(colliderFixed.bounds.center, colliderMovable.bounds.center, Color.white, 0.1f);

    //    //Vector3 gradient = constrainDirection * (distanceA - distanceB) / epsilon;
    //    Vector3 gradient = constrainDirection * (distanceA - distanceB) / epsilon;

    //    return gradient;
    //}

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(minConstrainPosition, maxConstrainPosition);
        Gizmos.color = Color.green;
        Gizmos.DrawLine(minConstrainPosition, GetAndComputeProjectPosition());
    }
}
