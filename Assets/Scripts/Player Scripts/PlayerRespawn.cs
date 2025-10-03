using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    private Transform currentCheckpoint; //tady se uklada posledni checkpoint
    //reference
    private Health playerHealth;
    private UIManager uiManager;

    private void Awake()
    {
        playerHealth = GetComponent<Health>();
        uiManager = FindObjectOfType<UIManager>(); //vrati prvni aktivni objekt toho typu
    }
    public void CheckRespawn()
    {
        //zkontroluje jestli je sebranej nejakej checkpoint
        if (currentCheckpoint == null)
        {
            //spusti game over screen
            uiManager.GameOver();

            return; //zbytek metody se neexecutne
        }
        transform.position = currentCheckpoint.position; //presune hrace na checkpoint
        playerHealth.Respawn();//Resetovat hp a animace
    }

    //Activate checkpoint
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag == "Checkpoint")
        {
            currentCheckpoint = collision.transform; //ulozim do transformu
            collision.GetComponent<Collider2D>().enabled = false; //vypne checkpoint collider
            collision.GetComponent<Animator>().SetTrigger("appear"); //triiger pro appear
        }
    }
}
