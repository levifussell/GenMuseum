using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class VRPhysicsHand : MonoBehaviour
{
    [SerializeField]
    Rigidbody handObject;

    [SerializeField]
    Collider playerCollider;

    private void Awake()
    {
        ConfigurableJoint connectJoint = this.gameObject.AddComponent<ConfigurableJoint>();
        connectJoint.CustomSpringPositionRotationJointAutoPair(handObject, 10000, 10, 0, 0, 100.0f);
        connectJoint.angularXMotion = ConfigurableJointMotion.Locked;
        connectJoint.angularYMotion = ConfigurableJointMotion.Locked;
        connectJoint.angularZMotion = ConfigurableJointMotion.Locked;
        //SpringJoint connectJoint = this.gameObject.AddComponent<SpringJoint>();
        //connectJoint.autoConfigureConnectedAnchor = false;
        //connectJoint.anchor = Vector3.zero;
        //connectJoint.connectedAnchor = Vector3.zero;
        //connectJoint.connectedBody = handObject;
        //connectJoint.spring = 100000;
        //connectJoint.damper = 10;

        // disable player collision.
        foreach (Collider c in handObject.GetComponentsInChildren<Collider>())
            Physics.IgnoreCollision(c, playerCollider, true);
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void LateUpdate()
    {
        //handObject.rotation = this.transform.rotation * initRotation;
    }
}
