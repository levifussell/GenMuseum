using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HingeHam : MonoBehaviour
{
    HingeJoint hingeJoint;

    Rigidbody connectedBody;

    float damageLengthSeconds = 0.3f;
    float damageCount = 0.0f;
    bool counting = false;

    List<Collider> collidedWith = new List<Collider>();
    List<Collider> collidedWith2 = new List<Collider>();

    // Start is called before the first frame update
    void Start()
    {
        hingeJoint = this.GetComponent<HingeJoint>();
        connectedBody = hingeJoint.connectedBody;
        //ChildColliderCallback cc = connectedBody.gameObject.AddComponent<ChildColliderCallback>();
        //cc.onCollisionEnterCallback = ConnectedCollision; 
        //cc.onCollisionExitCallback = ConnectedLeft; 

        connectedBody.solverIterations = 10;
    }

    void ConnectedCollision(Collision collision)
    {
        //float maxDist = float.MinValue;
        //for (int i = 0; i < collision.contactCount; ++i)
        //    maxDist = Mathf.Max(collision.GetContact(0).separation, maxDist);
        //Physics.ComputePenetration(collision.collider, collision.transform.position, collision.transform.rotation,
        //    connectedBody.GetComponent<Collider>(), this.transform.position, this.transform.rotation, out Vector3 dir, out float dist);
        //Debug.Log(string.Format("PEN DIST: {0}", maxDist));
        Collider c = collision.GetContact(0).otherCollider;
        Collider d = collision.GetContact(0).thisCollider;
        //if (!collidedWith.Contains(c))
        //{
        collidedWith.Add(c);
        collidedWith2.Add(d);
        //}
    }

    void ConnectedLeft(Collision collision)
    {
        //Collider c = collision.GetContact(0).otherCollider;
        //if (collidedWith.Contains(c))
        //    collidedWith.Remove(c);
    }

    // Update is called once per frame
    void Update()
    {
        //float maxPen = float.MinValue;
        //for(int i = 0; i < collidedWith.Count; ++i)
        //{
        //    Collider c = collidedWith[i];
        //    Collider d = collidedWith2[i];
        //    Physics.ComputePenetration(c, c.transform.position, c.transform.rotation,
        //        d, d.transform.position, d.transform.rotation, out Vector3 dir, out float dist);
        //    maxPen = Mathf.Max(dist, maxPen);
        //}
        //Debug.Log(string.Format("PEN DIST: {0}", maxPen));

        //if (hingeJoint.currentTorque.magnitude > 300.0f && maxPen > 0.1f)
        //{
        //    //damageCount = !counting ? damageLengthSeconds : damageCount - Time.deltaTime;
        //    //counting = true;

        //    //if (damageCount <= 0.0f)
        //    //connectedBody.isKinematic = true;
        //}
        //else
        //{
        //    counting = false;
        //}

        //foreach(Collider c in collidedWith)

        //if (hingeJoint == null) {
        //    connectedBody.isKinematic = true;
        //}
        //else
        //{
        //Debug.Log(string.Format("FORCE: {0}", hingeJoint.currentForce.magnitude));
        //Debug.Log(string.Format("TORQUE: {0}", hingeJoint.currentTorque.magnitude));
        //}
    }
}
