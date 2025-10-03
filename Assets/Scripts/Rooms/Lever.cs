using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lever : MonoBehaviour
{
        [SerializeField] private GameObject tunnelDoor;

        private void OnTriggerEnter(Collider other)
        {
            // zkontroluje, zda do paky narazil hrac
            if (other.CompareTag("Player"))
            {
                // aktivuje dvere
                if (tunnelDoor != null)
                {
                    tunnelDoor.SetActive(true); // aktivani objektu
                }
            }
        }
}
