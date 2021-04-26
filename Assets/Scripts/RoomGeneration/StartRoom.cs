using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class StartRoom : Room
{

    #region parameters
    GameObject startRoomParent;
    GameObject normalRoomParent;
    GameObject startRoomStorage;
    public bool isStartRoom { get; private set; }

    private List<Rigidbody> objectsInRoom = new List<Rigidbody>();
    public int objectCountInRoom { get => objectsInRoom.Count; }

    PlayerControllerPC player;
    Vector3 diffToPlayerNorm { get => (this.transform.position - player.transform.position).normalized; }
    #endregion

    #region actions
    public Action OnStartRoomChange;
    #endregion

    #region unity methods

    private void Awake()
    {
        startRoomParent = this.transform.Find("StartRoomParent").gameObject;
        normalRoomParent = this.transform.Find("NormalRoomParent").gameObject;
        startRoomStorage = this.transform.Find("StartRoomStore").gameObject;

        Debug.Assert(startRoomParent != null);
        Debug.Assert(normalRoomParent != null);

        ChildColliderCallback ccStartRoom = startRoomParent.AddComponent<ChildColliderCallback>();
        ccStartRoom.onTriggerExitCallback += OnStartRoomExit;

        ChildColliderCallback ccNormalRoom = normalRoomParent.AddComponent<ChildColliderCallback>();
        ccNormalRoom.onTriggerEnterCallback += OnStartRoomEnter;

        ChildColliderCallback ccStartRoomStore = startRoomStorage.AddComponent<ChildColliderCallback>();
        ccStartRoomStore.onTriggerEnterCallback += OnStartRoomStoreEnter;
        ccStartRoomStore.onTriggerExitCallback += OnStartRoomStoreExit;

        ChangeToStartRoom();
    }

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerControllerPC>();
        player.OnRespawnCallback += ChangeToStartRoom;
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnStartRoomExit(Collider other, GameObject _)
    {
        if(other.gameObject == player.gameObject)
        {
            ChangeToNormalRoom();
        }
    }

    private void OnStartRoomEnter(Collider other, GameObject _)
    {
        if(other.gameObject == player.gameObject)
        {
            ChangeToStartRoom();
        }
    }

    private void OnStartRoomStoreEnter(Collider other, GameObject _)
    {
        if (objectsInRoom.Contains(other.attachedRigidbody) || other.CompareTag("Player") || other.CompareTag("MainCamera"))

            return;

        objectsInRoom.Add(other.attachedRigidbody);
    }

    private void OnStartRoomStoreExit(Collider other, GameObject _)
    {
        if (!objectsInRoom.Contains(other.attachedRigidbody) || other.CompareTag("Player") || other.CompareTag("MainCamera"))

            return;

        objectsInRoom.Remove(other.attachedRigidbody);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(player.transform.position, player.transform.position + diffToPlayerNorm);
    }
    #endregion

    #region room switching

    void ChangeToNormalRoom()
    {
        isStartRoom = false;
        startRoomParent.SetActive(false);
        normalRoomParent.SetActive(true);

        OnStartRoomChange?.Invoke();

        foreach(Rigidbody r in objectsInRoom)
        {
            r.gameObject.SetActive(false);
        }
    }

    void ChangeToStartRoom()
    {
        isStartRoom = true;
        normalRoomParent.SetActive(false);
        startRoomParent.SetActive(true);

        OnStartRoomChange?.Invoke();

        foreach(Rigidbody r in objectsInRoom)
        {
            r.gameObject.SetActive(true);
        }
    }
    #endregion
}

#if UNITY_EDITOR
[CustomEditor(typeof(StartRoom))]
class E_StartRoom : Editor
{
    StartRoom startRoom;

    private void OnEnable()
    {
        startRoom = (StartRoom)target;        
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.LabelField($"Objects in room: {startRoom.objectCountInRoom}");
    }
}
#endif
