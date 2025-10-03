using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public Transform previousRoom;
    public Transform nextRoom;
    public Camera cam;

    //private void OnTriggerEnter2D(Collider2D collision)
    //{
      //  if (collision.tag == "Player")
        //{
          //  if (collision.transform.position.x < transform.position.x)
            //{
              //  cam.MoveToNewRoom(nextRoom);
                //nextRoom.GetComponent<Room>().ActivateRoom(true);
            //    previousRoom.GetComponent<Room>().ActivateRoom(false);
            //}
            //else
            //{
              //  cam.MoveToNewRoom(previousRoom);
                //previousRoom.GetComponent<Room>().ActivateRoom(true);
                //nextRoom.GetComponent<Room>().ActivateRoom(false);
            //}

     //   }
    //}
}
