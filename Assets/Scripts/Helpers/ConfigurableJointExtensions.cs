using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public static class ConfigurableJointExtensions
{
    /// <summary>
    /// Sets the value for all joint motions. Useful for toggling a joint from fixed to configurable.
    /// </summary>
    /// <param name="joint"></param>
    /// <param name="motion"></param>
    public static void SetAllJointMotions(this ConfigurableJoint joint, ConfigurableJointMotion motion)
    {
        joint.xMotion = motion;
        joint.yMotion = motion;
        joint.zMotion = motion;
        joint.angularXMotion = motion;
        joint.angularYMotion = motion;
        joint.angularZMotion = motion;
    }

    /// <summary>
    /// Easy way to quickly set the most important joint params for PD control.
    /// </summary>
    /// <param name="joint"></param>
    /// <param name="posSpring"></param>
    /// <param name="posDamp"></param>
    /// <param name="rotSpring"></param>
    /// <param name="rotDamp"></param>
    /// <param name="maxForce"></param>
    public static void SetPdParamters(
        this ConfigurableJoint joint,
        float positionSpring, float positionDamp,
        float rotationSpring, float rotationDamp,
        float maximumForce)
    {
        JointDrive jd = new JointDrive();
        jd.positionSpring = positionSpring;
        jd.positionDamper = positionDamp;
        jd.maximumForce = maximumForce;
        joint.xDrive = jd;
        joint.yDrive = jd;
        joint.zDrive = jd;
        JointDrive sd = new JointDrive();
        sd.positionSpring = rotationSpring;
        sd.positionDamper = rotationDamp;
        sd.maximumForce = maximumForce;
        joint.slerpDrive = sd;
        joint.rotationDriveMode = RotationDriveMode.Slerp;
    }

    /// <summary>
    /// Easy configurable joint initial setup.
    /// </summary>
    /// <param name="connectJoint"></param>
    /// <param name="positionSpring"></param>
    /// <param name="positionDamp"></param>
    /// <param name="rotationSpring"></param>
    /// <param name="rotationDamp"></param>
    /// <param name="maximumForce"></param>
    public static void CustomSpringPositionRotationJoint(
            this ConfigurableJoint connectJoint,
            float positionSpring, float positionDamp,
            float rotationSpring, float rotationDamp,
            float maximumForce)
    {
        connectJoint.xMotion = ConfigurableJointMotion.Free;
        connectJoint.yMotion = ConfigurableJointMotion.Free;
        connectJoint.zMotion = ConfigurableJointMotion.Free;
        connectJoint.angularXMotion = ConfigurableJointMotion.Free;
        connectJoint.angularYMotion = ConfigurableJointMotion.Free;
        connectJoint.angularZMotion = ConfigurableJointMotion.Free;
        connectJoint.autoConfigureConnectedAnchor = false;
        //connectJoint.anchor = Vector3.zero;
        connectJoint.connectedAnchor = Vector3.zero;

        connectJoint.SetPdParamters(
            positionSpring, positionDamp,
            rotationSpring, rotationDamp,
            maximumForce);
    }

    /// <summary>
    /// Pair joints based on the rigidbody's current positions.
    /// </summary>
    /// <param name="connectJoint"></param>
    /// <param name="connectedTo"></param>
    /// <param name="positionSpring"></param>
    /// <param name="positionDamp"></param>
    /// <param name="rotationSpring"></param>
    /// <param name="rotationDamp"></param>
    /// <param name="maximumForce"></param>
    public static void CustomSpringPositionRotationJointAutoPair(
            this ConfigurableJoint connectJoint, Rigidbody connectedTo,
            float positionSpring, float positionDamp,
            float rotationSpring, float rotationDamp,
            float maximumForce
        )
    {
        connectJoint.CustomSpringPositionRotationJoint(positionSpring, positionDamp, rotationSpring, rotationDamp, maximumForce);
        if(connectedTo != null)
        {
            connectJoint.anchor = connectJoint.transform.InverseTransformVector(connectedTo.transform.position - connectJoint.transform.position);
        }
        connectJoint.connectedBody = connectedTo;
    }

    public static void AutoSetAnchor(
        this ConfigurableJoint connectJoint)
    {
        if(connectJoint.connectedBody == null)
        {
            Debug.LogError("Failed to auto set anchor. There is not connected rigidbody to this joint.");
            return;
        }

        connectJoint.axis = connectJoint.connectedBody.transform.forward;
        connectJoint.secondaryAxis = connectJoint.connectedBody.transform.up;
        connectJoint.anchor = connectJoint.transform.InverseTransformVector(
            connectJoint.connectedBody.transform.position - connectJoint.transform.position);
    }
    public static void AutoSetConnectedAnchor(
        this ConfigurableJoint connectJoint)
    {
        if(connectJoint.connectedBody == null)
        {
            Debug.LogError("Failed to auto set anchor. There is not connected rigidbody to this joint.");
            return;
        }

        connectJoint.connectedAnchor = connectJoint.connectedBody.transform.InverseTransformVector(
            connectJoint.transform.position - connectJoint.connectedBody.transform.position);
    }

    public static bool TryAutoSetAnchor(
        this ConfigurableJoint connectJoint)
    {
        if (connectJoint.connectedBody == null)
        {
            return false;
        }
        else
        {
            connectJoint.AutoSetAnchor();
            return true;
        }
    }

    public static void DisableForces(this ConfigurableJoint connectJoint)
    {
        JointDrive positionDrive = connectJoint.xDrive;
        positionDrive.maximumForce = 0.0f;
        connectJoint.xDrive = positionDrive;
        connectJoint.yDrive = positionDrive;
        connectJoint.zDrive = positionDrive;

        JointDrive rotationDrive = connectJoint.slerpDrive;
        rotationDrive.maximumForce = 0.0f;
        connectJoint.slerpDrive = rotationDrive;
    }

    public static void EnableForces(this ConfigurableJoint connectJoint, float force=float.MaxValue)
    {
        JointDrive positionDrive = connectJoint.xDrive;
        positionDrive.maximumForce = force;
        connectJoint.xDrive = positionDrive;
        connectJoint.yDrive = positionDrive;
        connectJoint.zDrive = positionDrive;

        JointDrive rotationDrive = connectJoint.slerpDrive;
        rotationDrive.maximumForce = force;
        connectJoint.slerpDrive = rotationDrive;
    }

    /// <summary>
    /// Uses a coroutine to smoothly turn on the joint force instead of quickly snapping it on.
    /// </summary>
    /// <param name="connectJoint"></param>
    /// <param name="jointGameObject"></param>
    /// <param name="timeToMax"></param>
    /// <param name="force"></param>
    public static void SmoothlyEnableForces(this ConfigurableJoint connectJoint, MonoBehaviour jointGameObject, float timeToMax=1.0f, float force=float.MaxValue, System.Action onFinish=null)
    {
        //jointGameObject.StartCoroutine(SmoothlyEnableForces(connectJoint, timeToMax, force, onFinish));
        jointGameObject.StartCoroutine(SmoothlyChangeParameter(
            connectJoint,
            timeToMax,
            force,
            (x) => { connectJoint.EnableForces(x); },
            onFinish));
    }

    private static IEnumerator SmoothlyChangeParameter(
        ConfigurableJoint connectJoint, 
        float timeToMax, 
        float targetParamValue, 
        System.Action<float> applyParam,
        System.Action onFinish=null)
    {
        float currentParamValue = 0.0f;
        float paramChangeRate = targetParamValue / timeToMax;
        while(currentParamValue < targetParamValue)
        {
            currentParamValue += paramChangeRate * Time.deltaTime;
            applyParam(currentParamValue);
            yield return new WaitForEndOfFrame();
        }

        applyParam(targetParamValue);

        if(onFinish != null)
        {
            onFinish();
        }
    }

    /// <summary>
    /// Anchor positions in world frame.
    /// </summary>
    /// <param name="joint"></param>
    /// <returns></returns>
    public static Vector3 GlobalAnchorPosition(this ConfigurableJoint joint)
    {
        return joint.transform.TransformPoint(joint.anchor);
    }
    public static Vector3 GlobalConnectedAnchorPosition(this ConfigurableJoint joint)
    {
        if(joint.connectedBody != null)
        {
            return joint.connectedBody.transform.TransformPoint(joint.connectedAnchor);
        }
        else
        {
            return joint.GlobalAnchorPosition();
        }
    }

    /// <summary>
    /// The length of the joint is the distance between the two connecting anchors.
    /// </summary>
    /// <param name="joint"></param>
    /// <returns></returns>
    public static float JointLength(this ConfigurableJoint joint)
    {
        return Vector3.Distance(joint.GlobalAnchorPosition(), joint.GlobalConnectedAnchorPosition());
    }
}
