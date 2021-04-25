using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartRoom : Room
{

    #region parameters
    GameObject startRoomParent;
    GameObject normalRoomParent;
    public bool isStartRoom { get; private set; }

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

        Debug.Assert(startRoomParent != null);
        Debug.Assert(normalRoomParent != null);

        ChildColliderCallback ccStartRoom = startRoomParent.AddComponent<ChildColliderCallback>();
        ccStartRoom.onTriggerExitCallback += OnStartRoomExit;

        ChildColliderCallback ccNormalRoom = normalRoomParent.AddComponent<ChildColliderCallback>();
        ccNormalRoom.onTriggerEnterCallback += OnStartRoomEnter;

        ChangeToStartRoom();
    }

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerControllerPC>();
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
    }

    void ChangeToStartRoom()
    {
        isStartRoom = true;
        normalRoomParent.SetActive(false);
        startRoomParent.SetActive(true);

        OnStartRoomChange?.Invoke();
    }
    #endregion
}
