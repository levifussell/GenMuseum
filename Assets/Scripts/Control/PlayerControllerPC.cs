using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerControllerPC : MonoBehaviour
{
    #region serialized parameters
    [SerializeField] float speedPos = 3.0f;
    [SerializeField] float speedRot = 10.0f;
    [SerializeField] float mouseSensitivity = 2.0f;
    [SerializeField] Image pointerImage;
    [SerializeField] FadeView fadeView;
    #endregion

    #region parameters
    Camera camera;
    Rigidbody rigidbody;
    CapsuleCollider playerCollider;

    Vector3 playerSpawnPosition;
    Quaternion playerSpawnOrientation;
    public Action OnRespawnCallback;

    Rigidbody cameraGrabAnchor = null;
    Rigidbody grabbedObject = null;
    ConfigurableJoint grabJoint = null;
    Vector3 grabbedObjectScale;
    Transform grabbedObjectParent;

    float grabDistMax = 10.0f;
    public Vector3 lineOfSightNormal { get => this.camera == null ? Vector3.zero : this.camera.transform.forward; }
    int grabState = 0;

    Color pointerOn = new Color(1.0f, 1.0f, 1.0f, 0.75f);
    Color pointerOff = new Color(1.0f, 1.0f, 1.0f, 0.05f);

    public Action OnStepCallback;
    AudioSource audioSource;

    public Vector3 playerBase
    {
        get
        {
            if (playerCollider == null)
                return Vector3.zero;
            else
            {
                return this.transform.position - new Vector3(
                    0.0f,
                    playerCollider.height * 0.5f * this.transform.localScale.y,
                    0.0f);
            }
        }
    }
    #endregion

    #region unity methods

    private void Awake()
    {
        camera = GetComponentInChildren<Camera>();
        rigidbody = GetComponent<Rigidbody>();

        playerCollider = GetComponent<CapsuleCollider>();

        audioSource = GetComponent<AudioSource>();

    }

    // Start is called before the first frame update
    void Start()
    {
        this.transform.position = new Vector3(
            this.transform.position.x,
            playerCollider.height * 0.6f * this.transform.localScale.y,
            this.transform.position.z);

        this.playerSpawnPosition = this.transform.position;
        this.playerSpawnOrientation = this.transform.rotation;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        //StartCoroutine(Walking(0.25f));
        StartCoroutine(Walking(0.15f));

        fadeView.FadeOut(1.0f);
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
        {
            this.transform.rotation = Quaternion.AngleAxis(-speedRot * Time.deltaTime, Vector3.up);
        }
        else if (Input.GetKey(KeyCode.E))
        {
            this.transform.rotation *= Quaternion.AngleAxis(speedRot * Time.deltaTime, Vector3.up);
        }

        // mouse movement.
        float rotateVertical = Input.GetAxis("Mouse Y");
        float rotateHorizontal = Input.GetAxis("Mouse X");
        this.transform.RotateAround(this.transform.position, this.transform.up, rotateHorizontal * mouseSensitivity);
        camera.transform.RotateAround(camera.transform.position, this.transform.right, -rotateVertical * mouseSensitivity);

        // cursor unlock
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(Cursor.lockState == CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        // grabbing.

        CheckPointerForGrabbable();

        if (Input.GetMouseButtonDown(0))
        {
            if (grabState == 0)
            {
                if(OnToInventory())
                    grabState = 1;
            }
            else if (grabState == 1)
            {
                if(OnToHand())
                    grabState = 2;
            }
            else if(grabState == 2)
            {
                if(OnRelease())
                    grabState = 0;
            }

            //OnGrab();
        }
        //if (Input.GetMouseButtonUp(0))
        //    OnRelease();

        // teleporting
        if (Input.GetKeyDown(KeyCode.T))
            Respawn();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(this.transform.position, this.transform.position + this.lineOfSightNormal);
    }
    #endregion

    #region audio
    IEnumerator Walking(float walkPeriodSeconds)
    {
        float volume = 0.3f;
        float pitchRange = 0.1f;
        audioSource.volume = volume;
        while(true)
        {
            if(this.rigidbody.velocity.sqrMagnitude < 1e-5f)
            {
                if(audioSource.isPlaying)
                    audioSource.Stop();
                yield return new WaitForEndOfFrame();
            }
            else
            {
                audioSource.pitch = UnityEngine.Random.Range(1.0f - pitchRange, 1.0f + pitchRange);
                audioSource.Play();
                OnStepCallback?.Invoke();
                yield return new WaitForSeconds(walkPeriodSeconds);
                audioSource.Stop();
                yield return new WaitForSeconds(walkPeriodSeconds);
            }
        }
    }
    #endregion

    #region respawning
    public void Respawn()
    {
        OnRelease();
        this.transform.position = this.playerSpawnPosition;
        this.transform.rotation = this.playerSpawnOrientation;
        fadeView.FadeOut(1.0f);

        OnRespawnCallback?.Invoke();
    }
    #endregion

    #region grabbing
    void CheckPointerForGrabbable()
    {
        if((Input.GetMouseButton(0) && grabState == 0) || grabState == 2)
            pointerImage.color = pointerOn;
        else
        {
            pointerImage.color = pointerOff;

            if (grabState == 0 && Physics.Raycast(camera.transform.position, camera.transform.forward, out RaycastHit hitInfo, grabDistMax, ~0, QueryTriggerInteraction.Ignore))
            {
                if (hitInfo.rigidbody == null)
                    return;

                Grabbable p = hitInfo.rigidbody.GetComponent<Grabbable>();

                if (p != null) { pointerImage.color = pointerOn; }
            }
        }

    }

    bool OnToInventory()
    {
        // cast a ray forward until it hits something.
        if (Physics.Raycast(camera.transform.position, camera.transform.forward, out RaycastHit hitInfo, grabDistMax, ~0, QueryTriggerInteraction.Ignore))
        {
            if (hitInfo.rigidbody == null || hitInfo.rigidbody.isKinematic)
                return false;

            Grabbable p = hitInfo.rigidbody.GetComponent<Grabbable>();

            if (p != null)
            {
                grabbedObject = p.GetComponent<Rigidbody>();
                grabbedObject.useGravity = false;
                grabbedObject.isKinematic = true;
                grabbedObjectScale = grabbedObject.transform.localScale;
                grabbedObject.transform.localScale *= 0.2f * p.inventoryScaleCorrection;
                grabbedObjectParent = grabbedObject.transform.parent;
                grabbedObject.transform.SetParent(this.camera.transform);
                grabbedObject.transform.position = this.camera.transform.position +
                        this.camera.transform.forward * 0.2f +
                        this.camera.transform.right * 0.1f -
                        this.camera.transform.up * 0.1f;
                grabbedObject.transform.localPosition += p.inventoryPositionOffset;
                grabbedObject.transform.localRotation = Quaternion.AngleAxis(180.0f, Vector3.up) * Quaternion.AngleAxis(p.inventoryRotationOffset.x, new Vector3(
                    p.inventoryRotationOffset.y, p.inventoryRotationOffset.z, p.inventoryRotationOffset.w));
                grabbedObject.detectCollisions = false;

                // invoke any grab events.
                p.StartGrab();

                return true;
            }
        }

        return false;
    }

    bool OnToHand()
    {
        Debug.Assert(grabbedObject != null);

        float toHandDist = 3.0f;
        if(cameraGrabAnchor == null)
        {
            GameObject grabAnchor = new GameObject("GrabAnchor");
            grabAnchor.transform.SetParent(camera.transform);
            grabAnchor.transform.position = this.camera.transform.position + this.camera.transform.forward * toHandDist;
            cameraGrabAnchor = grabAnchor.AddComponent<Rigidbody>();
            cameraGrabAnchor.isKinematic = true;
            cameraGrabAnchor.mass = 10.0f;
        }

        grabbedObject.transform.SetParent(grabbedObjectParent);
        grabbedObject.detectCollisions = true;
        grabbedObject.transform.localScale = grabbedObjectScale;
        if (Physics.Raycast(camera.transform.position + camera.transform.forward * 0.2f, camera.transform.forward, out RaycastHit hitInfo, toHandDist, ~0, QueryTriggerInteraction.Ignore))
        {
            grabbedObject.transform.position = hitInfo.point - this.camera.transform.forward * Painting.DEPTH;
        }
        else
            grabbedObject.transform.position = this.camera.transform.position + this.camera.transform.forward * toHandDist;

        grabbedObject.isKinematic = false;
        grabJoint = cameraGrabAnchor.gameObject.AddComponent<ConfigurableJoint>();
        ConfigurableJointExtensions.CustomSpringPositionRotationJointAutoPair(grabJoint,
            grabbedObject, 300.0f, 10.0f, 100.0f, 1.0f, 100.0f);

        return true;
    }

    void OnGrab()
    {
        // cast a ray forward until it hits something.
        if(Physics.Raycast(camera.transform.position, camera.transform.forward, out RaycastHit hitInfo, grabDistMax, ~0, QueryTriggerInteraction.Ignore))
        {
            if (hitInfo.rigidbody == null)
                return;

            Grabbable p = hitInfo.rigidbody.GetComponent<Grabbable>();

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

                // invoke any grab events.

                p.StartGrab();
            }
        }
    }

    bool OnRelease()
    {
        if(grabbedObject != null)
        {
            Destroy(grabJoint);
            grabbedObject.useGravity = true;
            grabbedObject.WakeUp();
            grabbedObject = null;
        }

        return true;
    }
    #endregion
}
