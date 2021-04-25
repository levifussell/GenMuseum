using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerPC : MonoBehaviour
{
    #region serialized parameters
    [SerializeField] float speedPos = 3.0f;
    [SerializeField] float speedRot = 10.0f;
    [SerializeField] float mouseSensitivity = 2.0f;
    #endregion

    #region parameters
    Camera camera;
    Rigidbody rigidbody;
    CapsuleCollider playerCollider;

    Rigidbody cameraGrabAnchor = null;
    Rigidbody grabbedObject = null;
    ConfigurableJoint grabJoint = null;

    public Vector3 lineOfSightNormal { get => this.camera == null ? Vector3.zero : this.camera.transform.forward; }
    #endregion

    #region unity methods

    private void Awake()
    {
        camera = GetComponentInChildren<Camera>();
        rigidbody = GetComponent<Rigidbody>();

        playerCollider = GetComponent<CapsuleCollider>();
    }

    // Start is called before the first frame update
    void Start()
    {
        this.transform.position = new Vector3(
            this.transform.position.x,
            playerCollider.height * 0.6f * this.transform.localScale.y,
            this.transform.position.z);

        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        // positional movement.

        this.rigidbody.velocity = Vector3.zero;

        if (Input.GetKey(KeyCode.W))
        {
            this.rigidbody.velocity += this.transform.forward;
        }

        if (Input.GetKey(KeyCode.S))
        {

            this.rigidbody.velocity += -this.transform.forward;
        }

        if (Input.GetKey(KeyCode.D))
        {

            this.rigidbody.velocity += this.transform.right;
        }
        if (Input.GetKey(KeyCode.A))
        {

            this.rigidbody.velocity += -this.transform.right;
        }

        this.rigidbody.velocity = this.rigidbody.velocity.normalized * speedPos;

        // rotational movement.

        if (Input.GetKey(KeyCode.Q))
            this.transform.rotation *= Quaternion.AngleAxis(-speedRot * Time.deltaTime, Vector3.up);
        else if (Input.GetKey(KeyCode.E))
            this.transform.rotation *= Quaternion.AngleAxis(speedRot * Time.deltaTime, Vector3.up);

        // mouse movement.
        float rotateVertical = Input.GetAxis("Mouse Y");
        float rotateHorizontal = Input.GetAxis("Mouse X");
        this.transform.RotateAround(this.transform.position, this.transform.up, rotateHorizontal * mouseSensitivity);
        camera.transform.RotateAround(camera.transform.position, this.transform.right, -rotateVertical * mouseSensitivity);

        // cursor unlock
        if (Input.GetKeyDown(KeyCode.Escape))
            Cursor.lockState = CursorLockMode.None;

        // grabbing.
        if (Input.GetMouseButtonDown(0))
            OnGrab();
        if (Input.GetMouseButtonUp(0))
            OnRelease();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(this.transform.position, this.transform.position + this.lineOfSightNormal);
    }
    #endregion

    #region grabbing
    void OnGrab()
    {
        float grabDistMax = 10.0f;
        // cast a ray forward until it hits something.
        if(Physics.Raycast(camera.transform.position, camera.transform.forward, out RaycastHit hitInfo, grabDistMax, ~0, QueryTriggerInteraction.Ignore))
        {
            if (hitInfo.rigidbody == null)
                return;

            Painting p = hitInfo.rigidbody.GetComponent<Painting>();

            if(p != null)
            {
                if(cameraGrabAnchor == null)
                {
                    GameObject grabAnchor = new GameObject("GrabAnchor");
                    grabAnchor.transform.SetParent(camera.transform);
                    grabAnchor.transform.position = hitInfo.point;
                    cameraGrabAnchor = grabAnchor.AddComponent<Rigidbody>();
                    cameraGrabAnchor.isKinematic = true;
                    cameraGrabAnchor.mass = 10.0f;
                }

                grabbedObject = p.GetComponent<Rigidbody>();
                grabbedObject.useGravity = false;
                grabJoint = cameraGrabAnchor.gameObject.AddComponent<ConfigurableJoint>();
                ConfigurableJointExtensions.CustomSpringPositionRotationJointAutoPair(grabJoint,
                    grabbedObject, 300.0f, 10.0f, 100.0f, 1.0f, 100.0f);
            }
        }
    }

    void OnRelease()
    {
        if(grabbedObject != null)
        {
            Destroy(grabJoint);
            grabbedObject.useGravity = true;
            grabbedObject.WakeUp();
            grabbedObject = null;
        }
    }
    #endregion
}
