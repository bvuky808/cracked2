using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{
    //Room camera
    public float speed;
    private float currentPosX;
    private Vector3 velocity = Vector3.zero;

    //Player follow
    public Transform player;
    public float aheadDistance;
    public float cameraSpeed;
    private float lookAhead;
    void Start()
    {
        
    }
    void Update()
    {
        //Room Camera
        //transform.position = Vector3.SmoothDamp(transform.position, new Vector3(currentPosX, transform.position.y, transform.position.z), ref velocity, speed);
        transform.position = new Vector3(player.position.x + lookAhead, transform.position.y,transform.position.z);
        lookAhead = Mathf.Lerp(lookAhead, (aheadDistance * player.localScale.x), Time.deltaTime * cameraSpeed);
    }
    //public void MoveToNewRoom(Transform _newroom)
    //{
      //  currentPosX = _newroom.position.x;
    //}
}
