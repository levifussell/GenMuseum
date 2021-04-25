using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    #region parameters
    Material material;
    Vector4 abyssMask = Vector4.zero;

    Transform player;

    List<Room> neighbours = new List<Room>();
    #endregion

    #region unity methods
    private void Awake()
    {
        material = this.GetComponent<MeshRenderer>().material;

        //player = GameObject.FindGameObjectWithTag("Player").transform;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            material.SetFloat("_AbyssRadius", 0.0f);
        }
    }
    #endregion

    #region update abyss
    //void UpdateAbyss()
    //{
    //    float edgeTriggerMargin = 0.1f;
    //    Vector3 diffToPlayer = player.position - this.transform.position;

    //    abyssMask.x = diffToPlayer.x < ChunkData.ROOM_WIDTH / 2.0f + edgeTriggerMargin ? 0 : 1;
    //    abyssMask.y = diffToPlayer.y < ChunkData.ROOM_WIDTH / 2.0f + edgeTriggerMargin ? 0 : 1;
    //    abyssMask.z = diffToPlayer.x > -ChunkData.ROOM_WIDTH / 2.0f + edgeTriggerMargin ? 0 : 1;
    //    abyssMask.w = diffToPlayer.y > -ChunkData.ROOM_WIDTH / 2.0f + edgeTriggerMargin ? 0 : 1;
    //}
    #endregion
}
