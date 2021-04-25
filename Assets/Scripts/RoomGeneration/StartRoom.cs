using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartRoom : Room
{

    #region parameters
    GameObject startRoomParent;
    GameObject normalRoomParent;
    bool isStartRoom = true;

    PlayerControllerPC player;
    Vector3 diffToPlayerNorm { get => (this.transform.position - player.transform.position).normalized; }
    #endregion

    #region unity methods

    private void Awake()
    {
        startRoomParent = this.transform.Find("StartRoomParent").gameObject;
        normalRoomParent = this.transform.Find("StartRoomParent").gameObject;

        Debug.Assert(startRoomParent != null);
        Debug.Assert(normalRoomParent != null);
    }

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerControllerPC>();
    }

    // Update is called once per frame
    void Update()
    {
        // check if the player line of sight looks away from the room, then turn it to a normal room.
        float lineOfSightThresh = 0.5f;
        if(isStartRoom && Vector3.Dot(player.lineOfSightNormal, diffToPlayerNorm) < lineOfSightThresh)
        {
            ChangeToNormalRoom();
            isStartRoom = false;
        }
        else if(!isStartRoom)
        {
            ChangeToStartRoom();
            isStartRoom = true;
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
        startRoomParent.SetActive(false);
        normalRoomParent.SetActive(true);
    }

    void ChangeToStartRoom()
    {
        normalRoomParent.SetActive(false);
        startRoomParent.SetActive(true);
    }
    #endregion
}
