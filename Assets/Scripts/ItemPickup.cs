using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    [Header("Co to odemyká?")]
    public bool unlocksDash = true;
    public bool unlocksDoubleJump = false;
    public bool unlocksRoll = false;

    [Header("Efekty")]
    public GameObject pickupEffect; 

    [Header("UI Vizuál")]
    public Sprite unlockSprite;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // zkontrolujeme tag "Player"
        if (collision.CompareTag("Player"))
        {
            // najde skript pro pohyb
            PlayerMovement player = collision.GetComponent<PlayerMovement>();

            if (player != null)
            {
                // dash
                if (unlocksDash)
                {
                    player.dashUnlocked = true; 
                    Debug.Log("ZÍSKAL JSI DASH!");
                }

                // double jump
                if (unlocksDoubleJump)
                {
                    player.doubleJumpUnlocked = true; 
                    Debug.Log("ZÍSKAL JSI DOUBLE JUMP!");
                }

                //morph
                if (unlocksRoll)
                {
                    player.rollUnlocked = true;
                    Debug.Log("ZÍSKAL JSI MORPHING!");
                }

                // UI nápis
                UIManager uiManager = FindObjectOfType<UIManager>();
                if (uiManager != null && unlockSprite != null)
                {
                    // zavolá funkci a pøedá jí obrázek
                    uiManager.ShowUnlockSprite(unlockSprite);
                }

                // možná efekt
                if (pickupEffect != null)
                {
                    Instantiate(pickupEffect, transform.position, Quaternion.identity);
                }

                // item se smaže
                Destroy(gameObject);
            }
        }
    }
}
