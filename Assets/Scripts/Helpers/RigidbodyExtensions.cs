using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RigidbodyExtensions
{
    /// <summary>
    /// Automatically creates an anchor at the objects origin. An anchor is a kinematic object which is
    ///     connected to the object by a ConfigurableJoint.
    /// </summary>
    /// <param name="positionSpring"></param>
    /// <param name="positionDamp"></param>
    /// <param name="rotationSpring"></param>
    /// <param name="rotationDamp"></param>
    /// <param name="maximumForce"></param>
    /// <returns></returns>
    public static GameObject CreateAnchor(
        this Rigidbody rigidbodyToAddTo,
        float positionSpring, float positionDamp,
        float rotationSpring, float rotationDamp,
        float maximumForce)
    {
        GameObject anchor = new GameObject(string.Format("{0}_anchor", rigidbodyToAddTo.name));
        anchor.transform.position = rigidbodyToAddTo.transform.position;
        Rigidbody anchorRigidbody = anchor.AddComponent<Rigidbody>();
        anchorRigidbody.isKinematic = true;

        ConfigurableJoint joint = anchor.AddComponent<ConfigurableJoint>();
        joint.connectedBody = rigidbodyToAddTo;
        joint.CustomSpringPositionRotationJoint(positionSpring, positionDamp, rotationSpring, rotationDamp, maximumForce);

        return anchor;
    }
}
