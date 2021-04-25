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

    Rigidbody grabbedObject = null;
    FixedJoint grabJoint = null;

    public Vector3 lineOfSightNormal { get => this.camera == null ? Vector3.zero : this.camera.transform.forward; }
    #endregion

    #region unity methods

    private void Awake()
    {
        camera = GetComponentInChildren<Camera>();
        rigidbody = GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        // positional moveoment.

        if (Input.GetKey(KeyCode.W))
            this.transform.position += this.transform.forward * speedPos * Time.deltaTime;
        else if (Input.GetKey(KeyCode.S))
            this.transform.position -= this.transform.forward * speedPos * Time.deltaTime;

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
            Painting p = hitInfo.rigidbody.GetComponent<Painting>();

            if(p != null)
            {
                grabbedObject = p.GetComponent<Rigidbody>();
                grabJoint = this.camera.gameObject.AddComponent<FixedJoint>();
                grabJoint.connectedBody = grabbedObject;
            }
        }
    }

    void OnRelease()
    {
        if(grabbedObject != null)
        {
            Destroy(grabJoint);
            grabbedObject.WakeUp();
            grabbedObject = null;
        }
    }
    #endregion
}
